using System;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Builder;
using Microsoft.AspNetCore.Http;

namespace DRL.API.Extensions
{
    /// <summary>
    /// </summary>
    public class HeaderkeyAuthorizationPipeline
    {
        /// <summary>
        /// </summary>
        /// <param name="applicationBuilder"></param>
        public void Configure(IApplicationBuilder applicationBuilder)
        {
            applicationBuilder.UseHeaderkeyAuthorization();
        }
    }

    /// <summary>
    /// </summary>
    public static class HeaderkeyAuthorizationMiddlewareExtension
    {
        /// <summary>
        /// </summary>
        /// <param name="app"></param>
        /// <returns></returns>
        public static IApplicationBuilder UseHeaderkeyAuthorization(this IApplicationBuilder app)
        {
            if (app == null) throw new ArgumentNullException(nameof(app));

            return app.UseMiddleware<HeaderkeyAuthorizationMiddleware>();
        }
    }

    /// <summary>
    /// </summary>
    public class HeaderkeyAuthorizationMiddleware
    {
        private readonly RequestDelegate _next;

        /// <summary>
        /// </summary>
        /// <param name="next"></param>
        public HeaderkeyAuthorizationMiddleware(RequestDelegate next)
        {
            _next = next;
        }

        /// <summary>
        /// </summary>
        /// <param name="context"></param>
        /// <returns></returns>
        public async Task Invoke(HttpContext context)
        {
            try
            {
                string programid = context.Request.Headers["program-id"];
                if (!string.IsNullOrEmpty(programid))
                {
                    await _next.Invoke(context);
                    return;
                }

                //Reject request if there is no authorization header or if it is not valid
                context.Response.StatusCode = 401;
                await context.Response.WriteAsync("Header key is missing!");
            }
            catch (Exception ex)
            {
                ex.ToString();
                throw;
            }
            
        }
    }
}