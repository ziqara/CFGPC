using System;
using System.Collections.Concurrent;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Http;
using MySqlX.XDevAPI;

namespace DDMLib
{
    public class SessionManager
    {
        private readonly IHttpContextAccessor httpContextAccessor_;
        private static readonly ConcurrentDictionary<string, SessionData> sessions_ = new ConcurrentDictionary<string, SessionData>();

        public SessionManager(IHttpContextAccessor httpContextAccessor)
        {
            httpContextAccessor_ = httpContextAccessor;
        }

        public void CreateSession(string email, string userName)
        {
            try
            {
                if (string.IsNullOrWhiteSpace(email))
                    return;

                string sessionId = Guid.NewGuid().ToString();

                TimeSpan sessionTimeout = TimeSpan.FromHours(12);
                DateTime expiresAt = DateTime.Now.Add(sessionTimeout);

                SessionData sessionData = new SessionData
                {
                    SessionId = sessionId,
                    Email = email,
                    UserName = userName,
                    CreatedAt = DateTime.Now,
                    ExpiresAt = expiresAt
                };

                sessions_[sessionId] = sessionData;

                CookieOptions cookieOptions = new CookieOptions
                {
                    HttpOnly = true,     // Недоступна из JavaScript
                    Secure = true,       // Только по HTTPS
                    SameSite = SameSiteMode.Strict, // Защита от CSRF
                    Expires = expiresAt, // Время жизни
                    Path = "/"           // Доступна для всего приложения
                };

                httpContextAccessor_.HttpContext.Response.Cookies.Append("SessionId", sessionId, cookieOptions);
            }
            catch (Exception ex)
            {
                ErrorLogger.LogError("CreateSession", ex.Message);
            }
        }

        public bool IsUserAuthenticated()
        {
            try
            {
                return IsActiveCookie(out _);
            }
            catch (Exception ex)
            {
                ErrorLogger.LogError("IsUserAuthenticated", ex.Message);
                return false;
            }
        }

        public string GetUserEmailFromSession()
        {
            try
            {
                if (IsActiveCookie(out SessionData sessionData))
                    return sessionData.Email;

                return null;
            }
            catch (Exception ex)
            {
                ErrorLogger.LogError("GetUserEmailFromSession", ex.Message);
                return null;
            }
        }

        public void CloseSession()
        {
            try
            {
                string sessionId = httpContextAccessor_.HttpContext?.Request.Cookies["SessionId"];

                if (string.IsNullOrEmpty(sessionId))
                    return;

                sessions_.TryRemove(sessionId, out _);

                httpContextAccessor_.HttpContext?.Response.Cookies.Delete("SessionId");

                httpContextAccessor_.HttpContext?.Response.Cookies.Delete("UserName");
            }
            catch (Exception ex)
            {
                ErrorLogger.LogError("InvalidateSession", ex.Message);
            }
        }

        private bool IsActiveCookie(out SessionData sessionData)
        {
            sessionData = null;

            if (httpContextAccessor_.HttpContext == null)
                return false;

            string sessionId = httpContextAccessor_.HttpContext.Request.Cookies["SessionId"];
            if (string.IsNullOrEmpty(sessionId))
                return false;

            if (!sessions_.TryGetValue(sessionId, out sessionData))
                return false;

            if (sessionData.ExpiresAt < DateTime.Now)
            {
                sessions_.TryRemove(sessionId, out _);
                sessionData = null;
                return false;
            }

            return true;
        }
    }
}
