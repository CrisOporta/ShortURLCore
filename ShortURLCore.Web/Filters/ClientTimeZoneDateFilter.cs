using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Filters;
using ShortURLCore.Web.Services.IServices;

namespace ShortURLCore.Web.Filters
{
    // Minimal single filter handling both incoming (action) and outgoing (result) conversions
    public class ClientTimeZoneDateFilter : IAsyncActionFilter, IAsyncResultFilter
    {
        private readonly IClientTimeZoneAccessor _tzAccessor;

        public ClientTimeZoneDateFilter(IClientTimeZoneAccessor tzAccessor)
        {
            _tzAccessor = tzAccessor;
        }

        public Task OnActionExecutionAsync(ActionExecutingContext context, ActionExecutionDelegate next)
        {
            var tz = _tzAccessor.TimeZone;
            // Handle primitives first by replacing in dictionary
            var argsSnapshot = context.ActionArguments.ToList();
            foreach (var kv in argsSnapshot)
            {
                if (kv.Value is DateTime dt)
                {
                    context.ActionArguments[kv.Key] = ToUtc(dt, tz);
                }
                else if (kv.Value is DateTimeOffset dto)
                {
                    context.ActionArguments[kv.Key] = dto.ToUniversalTime();
                }
                else
                {
                    ConvertDates(kv.Value, dt2 => ToUtc(dt2, tz), dto2 => dto2.ToUniversalTime(), new HashSet<object>(ReferenceEqualityComparer.Instance));
                }
            }
            return next();
        }

        public Task OnResultExecutionAsync(ResultExecutingContext context, ResultExecutionDelegate next)
        {
            var tz = _tzAccessor.TimeZone;

            if (context.Result is ObjectResult or JsonResult or ViewResult)
            {
                switch (context.Result)
                {
                    case ObjectResult orObj:
                        ConvertDates(orObj.Value, dt => FromUtc(dt, tz), dto => TimeZoneInfo.ConvertTime(dto, tz), new HashSet<object>(ReferenceEqualityComparer.Instance));
                        break;
                    case JsonResult json:
                        ConvertDates(json.Value, dt => FromUtc(dt, tz), dto => TimeZoneInfo.ConvertTime(dto, tz), new HashSet<object>(ReferenceEqualityComparer.Instance));
                        break;
                    case ViewResult view:
                        ConvertDates(view.Model, dt => FromUtc(dt, tz), dto => TimeZoneInfo.ConvertTime(dto, tz), new HashSet<object>(ReferenceEqualityComparer.Instance));
                        break;
                }
            }

            return next();
        }

        private static DateTime ToUtc(DateTime dt, TimeZoneInfo tz)
        {
            if (dt.Kind == DateTimeKind.Utc)
                return dt;
            // Interpret Unspecified/Local as client local
            var unspecified = DateTime.SpecifyKind(dt, DateTimeKind.Unspecified);
            return TimeZoneInfo.ConvertTimeToUtc(unspecified, tz);
        }

        private static DateTime FromUtc(DateTime dt, TimeZoneInfo tz)
        {
            // Treat Unspecified as UTC coming from server storage
            var utc = dt.Kind == DateTimeKind.Utc ? dt : DateTime.SpecifyKind(dt, DateTimeKind.Utc);
            var local = TimeZoneInfo.ConvertTimeFromUtc(utc, tz);
            // Keep as Unspecified to avoid double conversions by serializers
            return DateTime.SpecifyKind(local, DateTimeKind.Unspecified);
        }

        private static void ConvertDates(object? obj,
            Func<DateTime, DateTime> convertDate,
            Func<DateTimeOffset, DateTimeOffset> convertOffset,
            HashSet<object> visited)
        {
            if (obj is null)
                return;

            var type = obj.GetType();

            if (type.IsPrimitive || type.IsEnum || type == typeof(string))
                return;

            if (!type.IsValueType && !visited.Add(obj))
                return; // prevent cycles

            if (obj is DateTime dt)
            {
                obj = convertDate(dt);
                return;
            }

            if (obj is DateTimeOffset dto)
            {
                obj = convertOffset(dto);
                return;
            }

            if (obj is IEnumerable enumerable)
            {
                foreach (var item in enumerable)
                {
                    ConvertDates(item, convertDate, convertOffset, visited);
                }
                return;
            }

            // Convert settable properties
            foreach (var prop in type.GetProperties(BindingFlags.Instance | BindingFlags.Public))
            {
                if (!prop.CanRead || !prop.CanWrite)
                    continue;

                try
                {
                    var value = prop.GetValue(obj);
                    if (value is null) continue;

                    if (value is DateTime vdt)
                    {
                        prop.SetValue(obj, convertDate(vdt));
                    }
                    else if (value is DateTimeOffset vdto)
                    {
                        prop.SetValue(obj, convertOffset(vdto));
                    }
                    else
                    {
                        ConvertDates(value, convertDate, convertOffset, visited);
                    }
                }
                catch
                {
                    // ignore property we cannot set/read
                }
            }
        }

        private sealed class ReferenceEqualityComparer : IEqualityComparer<object>
        {
            public static readonly ReferenceEqualityComparer Instance = new();
            public new bool Equals(object? x, object? y) => ReferenceEquals(x, y);
            public int GetHashCode(object obj) => System.Runtime.CompilerServices.RuntimeHelpers.GetHashCode(obj);
        }
    }
}
