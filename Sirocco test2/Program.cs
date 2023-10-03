using Microsoft.Xrm.Sdk;
using Microsoft.Xrm.Sdk.Query;
using Microsoft.Xrm.Tooling.Connector;
using System;

namespace Sirocco_test2
{
    internal class Program
    {
        static void Main(string[] args)
        {
            Console.WriteLine("Dynamics 365 API Console App");

            try
            {
                // Use the correct connection string name
                string clientId = "ae081934-3503-4107-afa4-796b3cf38304";
                string clientSecret = "C908Q~46P-ak6AgswEnDWJnnDVkW3XVbYeFgodiU";
                string authority = "https://login.microsoftonline.com/9e10273d-7f32-4f0c-ad01-30d397aa4041";
                string crmUrl = "https://org3e92bd9b.crm4.dynamics.com/";
                string connectionString = $"AuthType=ClientSecret;Url={crmUrl};ClientId={clientId};ClientSecret={clientSecret};Authority={authority};RequireNewInstance=True";
                CrmServiceClient crmServiceClient = new CrmServiceClient(connectionString);

                if (crmServiceClient.IsReady)
                {
                    Console.WriteLine("Connected to Dynamics 365");
                    CrmActionsPicker(crmServiceClient);
                }
                else
                {
                    Console.WriteLine("Connection to CRM failed");
                }
            }
            catch (Exception ex)
            {
                Console.WriteLine("An error occurred: " + ex.Message);
            }


            Console.WriteLine("Press any key to exit.");
            Console.ReadLine();

            void CrmActionsPicker(CrmServiceClient crmServiceClient)
            {
                Console.WriteLine("Choose an action\n1.To create an account\n2.To create a contact\n3.To QueryTheDb");
                string action = Console.ReadLine();

                switch (action)
                {

                    case "1":
                        CreateAccount(crmServiceClient);
                        break;
                    case "2":
                        CreateContact(crmServiceClient);
                        break;
                    case "3":
                        QueryTheDb(crmServiceClient);
                        break;

                    default:
                        break;
                }
            }
            void QueryTheDb(CrmServiceClient crmServiceClient)
            {
                QueryExpression query = new QueryExpression("account");
                query.ColumnSet = new ColumnSet(true);

                EntityCollection result = crmServiceClient.RetrieveMultiple(query);
            }
            void CreateAccount(CrmServiceClient crmServiceClient)
            {
                Console.WriteLine("Creating account...");

                Entity account = new Entity("account");
                account["name"] = "Test Account";
                account["telephone1"] = "555-555-5555";
                account["address1_line1"] = "123 Main St.";
                account["address1_city"] = "Seattle";
                account["address1_stateorprovince"] = "WA";
                account["address1_postalcode"] = "98101";
                account["address1_country"] = "US";

                var result = crmServiceClient.Create(account);
                Console.WriteLine($"created {result}");
            }
            void CreateContact(CrmServiceClient crmServiceClient)
            {
                Console.WriteLine("Creating contact...");

                Entity account = new Entity("account");
                account["name"] = "Test Account";
                account["telephone1"] = "555-555-5555";
                account["address1_line1"] = "123 Main St.";
                account["address1_city"] = "Seattle";
                account["address1_stateorprovince"] = "WA";
                account["address1_postalcode"] = "98101";
                account["address1_country"] = "US";

                var result = crmServiceClient.Create(account);
                Console.WriteLine($"created {result}");
            }
        }


    }
}
