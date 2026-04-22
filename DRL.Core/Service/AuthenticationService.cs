using DRL.Core.Interface;
using DRL.Entity;
using DRL.Library;
using Microsoft.Extensions.Options;
using System;
using System.Linq;
using System.Collections.Generic;
using System.Text;
using System.DirectoryServices.ActiveDirectory;
using System.DirectoryServices;

namespace DRL.Core.Hepler
{
    public static class ADProperties
    {
        public const string Name = "name";
        public const string DisplayName = "displayName";
        public const string MemberOf = "memberof";
    }

    public class AuthenticationService : IAuthenticationService
    {
        public ActionStatus Authenticate(string username, string password, string domain)
        {

            ActionStatus result = new ActionStatus();
            try
            {
                DirectoryEntry de = new DirectoryEntry("LDAP://" + domain, username, password);
                DirectorySearcher dsearch = new DirectorySearcher(de);
                dsearch.Sort = new SortOption(ADProperties.Name, SortDirection.Ascending);
                dsearch.PropertiesToLoad.Add(ADProperties.Name);
                dsearch.PropertiesToLoad.Add(ADProperties.DisplayName);
                dsearch.PropertiesToLoad.Add(ADProperties.MemberOf);
                dsearch.Filter = "(sAMAccountName=" + username + ")";
                SearchResult results = dsearch.FindOne();
                if (results != null)
                {
                    ENTAppUser appUser = new ENTAppUser();
                    if (results.Properties[ADProperties.Name].Count > 0)
                        appUser.UserName = results.Properties[ADProperties.Name][0].ToString();

                    if (results.Properties[ADProperties.DisplayName].Count > 0)
                        appUser.DisplayName = results.Properties[ADProperties.DisplayName][0].ToString();

                    if (results.Properties[ADProperties.MemberOf].Count > 0)
                    {
                        appUser.UserGroup = string.Empty;
                        foreach (string item in results.Properties[ADProperties.MemberOf])
                        {
                            appUser.UserGroup += string.Format("{0},", GetUserGroupName(item));
                        }
                        appUser.UserGroup = appUser.UserGroup.TrimEnd(',');
                    }
                    result.Success = true;
                    result.Result = appUser;
                }
                else
                {
                    result.Success = false;
                    result.Message = "Login Failed! Provided login details are invalid.";
                }

            }
            catch
            {
                result.Success = false;
                result.Message = "Please try again! Something went wrong.";
            }
            return result;
        }

        private static string GetUserGroupName(string stringValue)
        {
            List<string> lst = stringValue.Split(',').ToList();
            if (lst != null && lst.Count > 0)
            {
                string strVal = lst.Where(s => s.StartsWith("CN=")).FirstOrDefault();
                if (!string.IsNullOrWhiteSpace(strVal))
                {
                    strVal = strVal.Replace("CN=", "");
                }
                return strVal;
            }
            return "";
        }
    }
}