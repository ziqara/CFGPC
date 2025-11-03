using System;
using System.Collections.Concurrent;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Http;

namespace DDMLib
{
    public class SessionManager : ISessionManager
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

                var sessionData = new SessionData
                {
                    SessionId = sessionId,
                    Email = email,
                    UserName = userName,
                    CreatedAt = DateTime.Now,
                    ExpiresAt = expiresAt
                };

                sessions_[sessionId] = sessionData;

                var cookieOptions = new CookieOptions
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
    }
}
