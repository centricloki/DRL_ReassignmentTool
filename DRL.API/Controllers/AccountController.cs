using DRL.Core.Interface;
using DRL.Core.Service;
using DRL.Entity;
using DRL.Library;

using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Cors;
using Microsoft.AspNetCore.Hosting;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.Options;
using Microsoft.IdentityModel.Tokens;

using System;
using System.Collections.Generic;
using System.IdentityModel.Tokens.Jwt;
using System.Linq;
using System.Security.Claims;
using System.Security.Principal;
using System.Text;
using System.Threading.Tasks;
using System.Xml.Linq;


// For more information on enabling Web API for empty projects, visit https://go.microsoft.com/fwlink/?LinkID=397860

namespace DRL.API.Controllers
{
    [AllowAnonymous]
    [EnableCors("CorsPolicy")]
    [Route("api/[controller]")]
    public class AccountController : BaseController
    {
        private readonly IAuthenticationService _authenticationService;
        private readonly AppSettings _appSettings;
        private readonly IHostingEnvironment _env;
        private readonly IConfiguration _configuration;
        private readonly INavigationPermissionService _navigationPermissionService;
        public AccountController(IAuthenticationService authenticationService
            , IOptions<AppSettings> appSettings, IHostingEnvironment env
            , IConfiguration configuration
            , INavigationPermissionService navigationPermissionService)
        {
            _authenticationService = authenticationService;
            _appSettings = appSettings.Value;
            _env = env;
            _configuration = configuration ?? throw new ArgumentNullException(nameof(configuration));
            _navigationPermissionService = navigationPermissionService ?? throw new ArgumentNullException(nameof(navigationPermissionService));
        }

        [HttpGet]
        public async Task<BaseResponse<ENTAppUser>> Authenticate()
        {
            BaseResponse<ENTAppUser> response = new BaseResponse<ENTAppUser>()
            {
                IsSuccess = false,
                Message = "Please try again! Something went wrong."
            };
            try
            {
                ENTLoginRequest request = new ENTLoginRequest();
                var activeGroups = await _navigationPermissionService.GetActiveUserGroupsAsync();
                string activeUserGroup = string.Empty;
                if (_env != null && (_env.IsProduction() || _env.IsStaging()))
                {
                    if (User.Identity.IsAuthenticated)
                    {
                        WindowsIdentity windowsIdentity = User.Identity as WindowsIdentity;
                        request.Username = CurrentUserId.ToString();
                        if (windowsIdentity != null)
                        {
                            List<string> groups = windowsIdentity.Groups.Select(y => y.Value).ToList();
                            foreach (var winGroupitem in groups)
                            {
                                var name = new System.Security.Principal.SecurityIdentifier(winGroupitem)
                                    .Translate(typeof(NTAccount))
                                    .ToString().ToLower();
                                foreach (var item in activeGroups)
                                {
                                    if (name == item.GroupName.ToLower() || name.EndsWith("\\" + item.GroupName.ToLower()))
                                    {
                                        response.Message = null;
                                        response.IsSuccess = true;
                                        response.Data = new ENTAppUser()
                                        {
                                            Name = windowsIdentity.Name,
                                            UserName = request.Username,
                                            WindowGroupName = name,
                                            UserGroup = item.GroupName,
                                        };
                                        activeUserGroup = item.GroupName;
                                        break;
                                    }
                                }
                            }
                        }
                    }
                }
                else //UAT & Developement
                {
                    response.Message = null;
                    response.IsSuccess = true;
                    response.Data = new ENTAppUser()
                    {
                        DisplayName = "TestUser",
                        UserName = "drladmin",
                        UserGroup = "drl it",
                        IsAdmin = true
                    };
                    activeUserGroup = "drl it";
                }


                if (response.IsSuccess && response.Data != null)
                {
                    // Fetch dynamic permissions from DB
                    response.Data.LinkPermissions =
                        await _navigationPermissionService.GetPermissionsForGroupAsync(activeUserGroup);
                }

            }
            catch (Exception ex)
            {
                response.IsSuccess = false;
                response.Message = ex.Message;
            }
            return response;
        }

        [HttpGet("GetLDAPGroupsNames")]
        public BaseResponse<List<string>> GetLDAPGroupsNames()
        {
            BaseResponse<List<string>> response = new BaseResponse<List<string>>();
            try
            {

                List<string> groups = ((System.Security.Principal.WindowsIdentity)((System.Security.Principal.WindowsPrincipal)User).Identity).Groups.Select(y => y.Value).ToList();
                List<string> translatedGroups = new List<string>();
                foreach (var item in groups)
                {
                    translatedGroups.Add(new System.Security.Principal.SecurityIdentifier(item).Translate(typeof(System.Security.Principal.NTAccount)).ToString().ToLower());
                }
                response.Data = translatedGroups;
            }
            catch (Exception ex)
            {
                response.IsSuccess = false;
                response.Message = ex.Message;
            }
            return response;
        }


        [HttpGet("GetCurrentUserName")]
        public BaseResponse<string> GetCurrentUserName()
        {
            BaseResponse<string> response = new BaseResponse<string>();
            try
            {
                if (User.Identity.IsAuthenticated)
                {
                    response.Data = CurrentUserName;
                }
                else
                {
                    response.Data = "User is not Authenticated";
                }
            }
            catch (Exception ex)
            {
                response.IsSuccess = false;
                response.Message = ex.Message;
            }
            return response;
        }

        [HttpGet("ClearUserGroupCache")]
        public BaseResponse<string> ClearUserGroupCache()
        {
            BaseResponse<string> response = new BaseResponse<string>();
            try
            {
                _navigationPermissionService.ClearUserGroupCache();
                response.Data = "User Group cache cleared successfully.";
                response.IsSuccess = true;
            }
            catch (Exception ex)
            {
                response.IsSuccess = false;
                response.Message = ex.Message;
            }
            return response;
        }
    }
}
