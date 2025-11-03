using System;
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

        public SessionManager(IHttpContextAccessor httpContextAccessor)
        {
            httpContextAccessor_ = httpContextAccessor;
        }
    }
}
