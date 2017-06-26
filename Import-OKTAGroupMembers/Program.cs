using System;
using System.Configuration;
using Okta.Core;
using Okta.Core.Clients;
using Okta.Core.Models;
using System.IO;

namespace Import_OKTAGroupMembers
{
    class Program
    {
        static void Main(string[] args)
        {
            var appToken = ConfigurationManager.AppSettings["AppToken"].ToString();   // 002z0R5A_Yi6nNZTU8CTf8v0m_o3FnH7vZQSQJNhjh
            var subDomain = ConfigurationManager.AppSettings["Subdomain"].ToString(); // nutanix
            //var groupName = ConfigurationManager.AppSettings["GroupName"].ToString(); // App-Workplace
            var OutFilePath = ConfigurationManager.AppSettings["OutFilePath"].ToString(); // Output File Path for "Unimported User"
            var FileName = ConfigurationManager.AppSettings["FileName"].ToString(); // Input FileName for list of users

            Console.Write("Please enter OKTA App Group Name for Import: ");
            string groupName = Console.ReadLine();

            // Set OKTAClient using AppToken and Subdomain
            var oktaClient = new OktaClient(appToken, subDomain);

            // Set GroupClient
            var groupClient = oktaClient.GetGroupsClient();

            // Get OKTA group by Name            
            var appGroupName = groupClient.GetByName(groupName);

            // Get OKTA Group client (appGroupName = App-Workplace)
            var groupUsersClient = new GroupUsersClient(appGroupName, appToken, subDomain);

            // Set UserClient and User
            var userClient = new UsersClient(appToken, subDomain);

            // Get all the users from OKTA app-group
            var appUsers = oktaClient.GetGroupUsersClient(appGroupName);
            
            User appUser = null;
                        
            string fileName = FileName;
            // Read UPN from CSV file
            var upn = File.ReadAllText(fileName).Split('\n'); // upn = "young.ryu@nutanix.net"

            int importedUserCount = 0;
            int unimportedUserCount = 0;

            var export = new CsvExport();
            for (int i = 0; i < upn.Length; i++)
            {                
                try
                {                                        
                    bool currentAppUser = false;

                    currentAppUser = Utilities.IsCurrnetAppMember(oktaClient, appGroupName, upn[i]);
                    
                    if (currentAppUser == false)
                    {
                        appUser = userClient.GetByUsername(upn[i]);
                        groupUsersClient.Add(appUser);
                        importedUserCount++;
                        Console.WriteLine("Imported User: " + upn[i]);
                    }
                }
                catch (Exception e)
                {                    
                    export.AddRow();
                    export["Not Imported User"] = upn[i];
                    unimportedUserCount++;
                    Console.WriteLine("Non-Imported User: " + upn[i]);
                }
            }
            export.ExportToFile(OutFilePath + appGroupName.Profile.Name + "-OMG-Not Imported-Users.csv");

            Console.WriteLine("The number of Users Imported is " + importedUserCount);
            Console.WriteLine("The number of Users NOT imported is " + unimportedUserCount);
            Console.WriteLine("Please validate a list of not imported user at " + OutFilePath);
            Console.Write("\nPress any key to exit... ");
            Console.ReadLine();
        }
    }
}