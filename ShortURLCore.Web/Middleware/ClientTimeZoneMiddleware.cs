using System;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Http;
using ShortURLCore.Web.Services;
using TimeZoneConverter;

namespace ShortURLCore.Web.Middleware
{
    public class ClientTimeZoneMiddleware
    {
        private readonly RequestDelegate _next;

        public ClientTimeZoneMiddleware(RequestDelegate next)
        {
            _next = next;
        }

        public async Task Invoke(HttpContext context)
        {
            TimeZoneInfo tzInfo = TimeZoneInfo.Utc;

            string? tz = context.Request.Headers["X-Timezone"].FirstOrDefault();
            if (string.IsNullOrWhiteSpace(tz))
            {
                tz = context.Request.Cookies["tz"];
            }

            if (!string.IsNullOrWhiteSpace(tz))
            {
                try
                {
                    if (TZConvert.TryGetTimeZoneInfo(tz, out var info))
                    {
                        tzInfo = info;
                    }
                    else
                    {
                        tzInfo = TimeZoneInfo.FindSystemTimeZoneById(tz);
                    }
                }
                catch
                {
                    tzInfo = TimeZoneInfo.Utc;
                }
            }

            context.Items[ClientTimeZoneAccessor.HttpContextItemKey] = tzInfo;
            await _next(context);
        }
    }
}
