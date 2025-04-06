using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Kuizlet.Application.Interfaces.Repositories;
using Microsoft.AspNetCore.Http;

namespace Kuizlet.Infrastructure.Implementations
{
    public class CurrentUserInfo : ICurrentUserInfo
    {
        private readonly IHttpContextAccessor _httpContextAccessor;

        public CurrentUserInfo(
            IHttpContextAccessor httpContextAccessor)
        {
            _httpContextAccessor = httpContextAccessor;
        }

        public string GetCurrentLogin()
        {
            return _httpContextAccessor.HttpContext?.User?.Identity?.Name
                ?? throw new Exception("User not authenticated");
        }
    }
}
