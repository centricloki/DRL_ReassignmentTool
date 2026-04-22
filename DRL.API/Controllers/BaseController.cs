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
using Microsoft.AspNetCore.Http;
using DRL.Core.Manager;
using Microsoft.Extensions.DependencyInjection;

namespace DRL.API.Controllers
{
    public class BaseController : ControllerBase
    {
        public string CurrentUserName
        {
            get { return User.Identity.Name != null ? User.Identity.Name.Substring(User.Identity.Name.IndexOf("\\") + 1).ToString() : "drladmin"; }
        }

        private long _CurrentUserId;
        public long CurrentUserId
        {
            get
            {
                if (HttpContext.Session.GetInt32("UserId") == null)
                {
                    _CurrentUserId = GetUserId(CurrentUserName);
                    HttpContext.Session.SetString("UserId", _CurrentUserId.ToString());
                    return _CurrentUserId;
                }
                else
                {
                    return _CurrentUserId = Convert.ToInt64(HttpContext.Session.GetString("UserId"));
                }
            }
        }

        private int GetUserId(string CurrentUserName)
        {
            int UserId = 0;
            try
            {
                var userservice = HttpContext.RequestServices.GetService<IUserService>();
                UserId = userservice.GetUserIdByUserName(CurrentUserName);
            }
            catch (Exception ex)
            {
                throw;
            }
            return UserId;
        }

    }
}
