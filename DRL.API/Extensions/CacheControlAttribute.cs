namespace DRL.API.Extensions
{
    using DRL.Entity;

    using Microsoft.AspNetCore.Http;
    using Microsoft.AspNetCore.Mvc;
    using Microsoft.AspNetCore.Mvc.Filters;

    using System.Collections;
    using System.Collections.Generic;
    using System.Linq;

    public class CacheControlAttribute : ActionFilterAttribute
    {
        private readonly string _cacheControlHeader;

        public CacheControlAttribute(string cacheControlHeader)
        {
            _cacheControlHeader = cacheControlHeader;
        }

        public override void OnResultExecuting(ResultExecutingContext context)
        {
            var result = context.Result as ObjectResult;

            // Ensure response is successful (200 OK) and has a value
            if (context.HttpContext.Response.StatusCode == StatusCodes.Status200OK && result?.Value != null)
            {
                var responseType = result.Value.GetType();

                // Check if the object has an "IsSuccess" property
                var isSuccessProperty = responseType.GetProperty("IsSuccess");
                bool? isSuccessValue = isSuccessProperty?.GetValue(result.Value) as bool?;

                if (isSuccessValue.HasValue && isSuccessValue.Value) // Only proceed if IsSuccess is true
                {
                    //// Try extracting the 'Data' property dynamically
                    //var dataProperty = responseType.GetProperty("Data");
                    //var dataValue = dataProperty?.GetValue(result.Value);

                    //if (dataValue is IEnumerable enumerable && enumerable.Cast<object>().Any())
                    //{
                    //// Cache only if 'Data' contains items
                    context.HttpContext.Response.Headers["Cache-Control"] = _cacheControlHeader;
                    base.OnResultExecuting(context);
                    return;
                    //}
                }
            }

            // No data or an error? Prevent caching
            context.HttpContext.Response.Headers["Cache-Control"] = "no-store";
            base.OnResultExecuting(context);
        }
    }

}
