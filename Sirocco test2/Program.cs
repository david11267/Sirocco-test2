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




            void CrmActionsPicker(CrmServiceClient crmServiceClient)
            {
                bool runLoop = true;


                while (runLoop)
                {
                    Console.WriteLine("Choose an action" +
                  "\n1.To create an account" +
                  "\n2.To create a contact" +
                  "\n3.To QueryTheDb" +
                  "\n4.To Update a account" +
                  "\n5.To Update a contact" +
                  "\n6.To Create a note" +

                  "\n\nPress any other key to exit");
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
                            QueryTheDb3(crmServiceClient);
                            break;
                        case "4":
                            UpdateAccount(crmServiceClient);
                            break;
                        case "5":
                            UpdateContact(crmServiceClient);
                            break;
                        case "6":
                            CreateNote(crmServiceClient);
                            break;

                        default:
                            runLoop = false;

                            break;
                    }
                }

            }

            void QueryTheDb(CrmServiceClient crmServiceClient)
            {
                QueryExpression accountQuery = new QueryExpression("account");
                accountQuery.ColumnSet = new ColumnSet(true);

                QueryExpression contactQuery = new QueryExpression("contact");
                contactQuery.ColumnSet = new ColumnSet(true);

                QueryExpression annotationQuery = new QueryExpression("annotation");
                annotationQuery.ColumnSet = new ColumnSet(true);

                EntityCollection accountResults = crmServiceClient.RetrieveMultiple(accountQuery);
                EntityCollection contactResults = crmServiceClient.RetrieveMultiple(contactQuery);
                EntityCollection notesResult = crmServiceClient.RetrieveMultiple(annotationQuery);

            }

            void QueryTheDb2(CrmServiceClient crmServiceClient)
            {
                string fetchXml = @"
        <fetch version='1.0' output-format='xml-platform' mapping='logical' distinct='false'>
            <entity name='account'>
                <all-attributes />
                <link-entity name='contact' from='parentcustomerid' to='accountid' link-type='inner'>
                    <all-attributes />
                </link-entity>
                <link-entity name='annotation' from='objectid' to='accountid' link-type='outer'>
                    <all-attributes />
                </link-entity>
            </entity>
        </fetch>";

                EntityCollection result = crmServiceClient.RetrieveMultiple(new FetchExpression(fetchXml));

            }

            void QueryTheDb3(CrmServiceClient crmServiceClient)
            {
                string fetchXml = @"
        <fetch version='1.0' output-format='xml-platform' mapping='logical' distinct='false'>
            <entity name='contact'>
                <all-attributes />
            </entity>
            <entity name='account'>
                <all-attributes />
            </entity>
            <entity name='annotation'>
                <all-attributes />
            </entity>
        </fetch>";

                EntityCollection result = crmServiceClient.RetrieveMultiple(new FetchExpression(fetchXml));
            }

            bool YesOrNo(string yesOrNoQuestion)
            {
                Console.WriteLine(yesOrNoQuestion);
                string yesOrNo = Console.ReadLine();
                if (yesOrNo == "y")
                {
                    return true;
                }
                else
                {
                    return false;
                }
            }
            void CreateAccount(CrmServiceClient crmServiceClient)
            {
                Console.WriteLine("What do you wish to name the new account?");
                string accountName = Console.ReadLine();
                bool toAddParent = YesOrNo("Do you wish to assign a parent account? y/n");

                Entity account = new Entity("account");
                account["name"] = accountName;
                account["telephone1"] = "555-555-5555";
                account["address1_line1"] = "123 Main St.";
                account["address1_city"] = "Seattle";
                account["address1_stateorprovince"] = "WA";
                account["address1_postalcode"] = "98101";
                account["address1_country"] = "US";

                if (toAddParent)
                {
                    Console.WriteLine("insert the parent id?");
                    var parentId = Console.ReadLine();
                    Console.WriteLine("assigning parent to account...");
                    account["parentaccountid"] = new EntityReference("account", new Guid(parentId));
                }

                var result = crmServiceClient.Create(account);
                Console.WriteLine($"created {result}");

            }
            void CreateContact(CrmServiceClient crmServiceClient)
            {
                Console.WriteLine("What is the FirstName of the new Contact?");
                string fName = Console.ReadLine();
                Console.WriteLine("What is the LastName of the new Contact?");
                string lName = Console.ReadLine();


                Console.WriteLine("Creating contact...");
                Entity contact = new Entity("contact");
                contact["firstname"] = fName;
                contact["lastname"] = lName;
                contact["emailaddress1"] = "createdFrom@consoleApp.com";
                contact["telephone1"] = "555-555-5556";

                bool toAssignToAccount = YesOrNo("Do you wish to assign the contact to an account? y/n");
                if (toAssignToAccount)
                {
                    Console.WriteLine($"provice the accountId you wish to assign {fName} {lName} to");
                    string accountId = Console.ReadLine();
                    // Create a EntityReference to the account and set it as the parent customer.
                    EntityReference parentAccount = new EntityReference("account", Guid.Parse(accountId));
                    contact["parentcustomerid"] = parentAccount;
                }


                var contactId = crmServiceClient.Create(contact);
                Console.WriteLine($"Contact created with ID: {contactId}");
            }
            void UpdateContact(CrmServiceClient crmServiceClient)
            {
                Console.WriteLine("What is the id of the contact you wish to update?");
                var contactId = Console.ReadLine();

                Console.WriteLine("what do you want to change the firstname to?");
                string fName = Console.ReadLine();
                Console.WriteLine("what do you want to change the lastname to?");
                string lName = Console.ReadLine();
                Console.WriteLine("what do you want to change the email to?");
                string email = Console.ReadLine();
                Console.WriteLine("what do you want to change the phone number to?");
                string phone = Console.ReadLine();

                Entity contactToUpdate = new Entity("contact");
                contactToUpdate["contactid"] = Guid.Parse(contactId);
                contactToUpdate["firstname"] = fName;
                contactToUpdate["lastname"] = lName;
                contactToUpdate["emailaddress1"] = email;
                contactToUpdate["telephone1"] = phone;

                crmServiceClient.Update(contactToUpdate);
                Console.WriteLine("Contact updated successfully.");

            }
            void UpdateAccount(CrmServiceClient crmServiceClient)
            {
                Console.WriteLine("What is the id of the account you wish to update?");
                var accountId = Console.ReadLine();
                Console.WriteLine("what name do you want?");
                var accountName = Console.ReadLine();
                Console.WriteLine("what phone number do you want?");
                var accountPhone = Console.ReadLine();
                Console.WriteLine("what address do you want?");
                var accountAddress = Console.ReadLine();

                Entity accountToUpdate = new Entity("account");
                accountToUpdate["accountid"] = Guid.Parse(accountId);
                accountToUpdate["name"] = accountName;
                accountToUpdate["telephone1"] = accountPhone;
                accountToUpdate["address1_line1"] = accountAddress;
                crmServiceClient.Update(accountToUpdate);

                Console.WriteLine("Account updated successfully.");
            }
            void CreateNote(CrmServiceClient crmServiceClient)
            {
                Console.WriteLine("What do you want to write in your note?");
                var noteText = Console.ReadLine();

                bool toAssignToAccount = YesOrNo("Do you wish to assign the note to an account? y/n");
                bool toAssignToContact = YesOrNo("Do you wish to assign the note to an contact? y/n");

                Entity note = new Entity("annotation");
                note["notetext"] = noteText;

                if (toAssignToAccount)
                {
                    Console.WriteLine("What is the id of the account you wish to assign the note to?");
                    var accountId = Console.ReadLine();
                    EntityReference accountReference = new EntityReference("account", Guid.Parse(accountId));
                    note["objectid"] = accountReference;

                    var noteId = crmServiceClient.Create(note);
                    Console.WriteLine($"Note created with ID: {noteId} and associated with Account ID: {accountId}");
                }
                else if (toAssignToContact)
                {
                    Console.WriteLine("What is the id of the contact you wish to assign the note to?");
                    var contactId = Console.ReadLine();
                    EntityReference contactReference = new EntityReference("contact", Guid.Parse(contactId));
                    note["objectid"] = contactReference;

                    var noteId = crmServiceClient.Create(note);
                    Console.WriteLine($"Note created with ID: {noteId} and associated with Account ID: {contactId}");
                }



            }

        }


    }


}

