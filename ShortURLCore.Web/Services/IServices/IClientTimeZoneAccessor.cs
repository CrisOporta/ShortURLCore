using System;

namespace ShortURLCore.Web.Services.IServices
{
    public interface IClientTimeZoneAccessor
    {
        TimeZoneInfo TimeZone { get; }
    }
}
