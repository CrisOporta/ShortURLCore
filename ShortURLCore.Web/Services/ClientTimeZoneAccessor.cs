using System;
using Microsoft.AspNetCore.Http;
using ShortURLCore.Web.Services.IServices;

namespace ShortURLCore.Web.Services
{
    public class ClientTimeZoneAccessor : IClientTimeZoneAccessor
    {
        public const string HttpContextItemKey = "ClientTimeZone";

        private readonly IHttpContextAccessor _httpContextAccessor;

        public ClientTimeZoneAccessor(IHttpContextAccessor httpContextAccessor)
        {
            _httpContextAccessor = httpContextAccessor;
        }

        public TimeZoneInfo TimeZone
            => (_httpContextAccessor.HttpContext?.Items[HttpContextItemKey] as TimeZoneInfo) ?? TimeZoneInfo.Utc;
    }
}
