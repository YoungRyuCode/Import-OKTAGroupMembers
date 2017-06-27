using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Okta.Core;
using Okta.Core.Clients;
using Okta.Core.Models;

namespace Import_OKTAGroupMembers
{
    public class Utilities
    {
        public static bool IsCurrnetAppMember(OktaClient oktaClient, Group appGroupName, string appUser)
        {
            bool existed = false;
            var existingUsers = oktaClient.GetGroupUsersClient(appGroupName);

            foreach (User user in existingUsers)
            {
                if (appUser.ToLower().Replace("\r", "") == user.Profile.Email.ToLower())
                {
                    existed = true;
                    break;
                }
                else
                {
                    continue;
                }
            }
            return existed;
        }

        public static bool IsCurrnetAppMemberLINQ(OktaClient oktaClient, Group appGroupName, string appUser)
        {
            bool existed = false;
            var existingUsers = oktaClient.GetGroupUsersClient(appGroupName);
            
            var users = from e in existingUsers.ToList()
                        where appUser.ToLower().Replace("\r", "") == e.Profile.Email.ToLower()
                        select e;

            var arrUsers = users.ToArray();

            if (arrUsers.Length == 0)
            {
                existed = false;
            }

            else if (arrUsers.Length == 1)
            {
                existed = true;
            }
           
            return existed;
        }
    }
}
