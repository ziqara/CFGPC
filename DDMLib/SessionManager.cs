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
            if (string.IsNullOrWhiteSpace(email))
                return;

        }
    }
}
