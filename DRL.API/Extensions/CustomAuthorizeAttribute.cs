using Microsoft.AspNetCore.Authorization;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using System;
using System.Collections.Generic;
using System.IdentityModel.Tokens.Jwt;
using System.Linq;
using System.Security.Claims;
using System.Text;
using Microsoft.Extensions.Options;
using Microsoft.IdentityModel.Tokens;
using System.Threading.Tasks;
using DRL.Core.Interface;
using DRL.Entity;
using DRL.Library;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Cors;
using Microsoft.AspNetCore.Mvc.Filters;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.AspNetCore.Hosting;
using Microsoft.Extensions.Configuration;
using System.Security.Principal;

namespace DRL.API.Extensions
{
    [EnableCors("CorsPolicy")]
    public class CustomAuthorizeAttribute : AuthorizeAttribute, IAsyncAuthorizationFilter
    {
        public async Task OnAuthorizationAsync(AuthorizationFilterContext context)
        {
            var user = context.HttpContext.User;
            bool isValid = false;

            var navigationService = context.HttpContext.RequestServices.GetService<INavigationPermissionService>();
            var activeGroups = await navigationService.GetActiveUserGroupsAsync();
            string salesGroup = activeGroups.ContainsKey(1) ? activeGroups[1] : "rpb sales admin";
            string drlITGroup = activeGroups.ContainsKey(2) ? activeGroups[2] : "drl it";

            // Use IHostingEnvironment (compatible with .NET Core 2.0)
            var env = context.HttpContext.RequestServices.GetService<IHostingEnvironment>();
            if (user.Identity.IsAuthenticated)
            {
                try
                {
                    if (env != null && (env.IsProduction() || env.IsStaging()))
                    {
                        var windowsIdentity = user.Identity as WindowsIdentity;
                        if (windowsIdentity != null)
                        {
                            List<string> groups = windowsIdentity.Groups.Select(y => y.Value).ToList();
                            foreach (var item in groups)
                            {
                                var name = new System.Security.Principal.SecurityIdentifier(item)
                                    .Translate(typeof(NTAccount))
                                    .ToString().ToLower();

                                if (name == salesGroup.ToLower() || name.EndsWith("\\" + salesGroup.ToLower()) || 
                                    name == drlITGroup.ToLower() || name.EndsWith("\\" + drlITGroup.ToLower()))
                                {
                                    isValid = true;
                                    return;
                                }
                            }
                        }
                    }
                    else
                    {
                        isValid = true;
                        return;
                    }
                }
                catch (Exception ex)
                {
                    // Log the error for debugging
                    context.HttpContext.Response.Headers.Add("X-Auth-Debug", ex.Message);
                    context.Result = new JsonResult(new { error = "Authorization Failed", details = ex.Message })
                    { StatusCode = 500 };
                    return;
                }

                if (!isValid)
                {
                    context.Result = new JsonResult(new { error = "Unauthorized" }) { StatusCode = 401 };
                }
            }
            else
            {
                context.Result = new JsonResult(new { error = "Unauthorized" }) { StatusCode = 401 };
            }
        }
    }
}
