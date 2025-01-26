using Newtonsoft.Json.Linq;
using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Net;
using System.Text;
using System.Threading.Tasks;
using WS.Test.ObjectClasses;
using System.Diagnostics;
using Npgsql;
using Microsoft.VisualBasic;

namespace WS.Test.Scripts
{
    internal class RestfulMethods
    {
        public static async Task SignIn(string requestBody, DataBase DBCon, HttpListenerContext context)
        {
            // turn the form into a jobject TODO: Turn into custom object class, with error handling for null



            HttpListenerResponse response = context.Response; // Initialises response
            response.ContentType = "application/json"; // Sets response Type, it will currently be changed to application/json


            CleanDetailsForm loginDetails = RequestBodyExtractor.ParseAccountCredentials(requestBody); // Server Validation for username/password requirements (stops dbcon usage)
            
            // If there is an issue with the login details then send bad response
            if (loginDetails.Result == "Error" || loginDetails.Result== "Reject")
            {
                response.StatusCode = (int)HttpStatusCode.BadRequest;// Set status code as bad
                await SendHttpResponse(response, new { message = loginDetails.ErrorMessage }); // Attach error message provided by clean login details as content
                return; // End function after sending response
            }

         
            // Gets account details from Database
            AccInfoObj accountDetails = await DBCon.GetAccountByName(loginDetails.Username);

            // Error Handling for database request
            if (accountDetails.Result == "Error")
            {
                // Do more logging here, it returns bad request if there is an error in the checking process
                response.StatusCode = (int)HttpStatusCode.BadRequest;
                await SendHttpResponse(response, new { message = "Internal Server Error" });
                // Todo: Log error
                return;
            } else if (accountDetails.Result == "NONE")
            {
                // If no username is present then send back bad username or password. MUST be identical to incorrect password.
                response.StatusCode = (int)HttpStatusCode.BadRequest;
                await SendHttpResponse(response, new { message = "Username or password is incorrect." });
                return;
            }

            // Call Check Hash function to check if match with given password
            bool hashMatchesPassword = HashMethodsClass.ValidateHash(loginDetails.Password, accountDetails.passwordSalt, accountDetails.passwordHash);

            // If password does not match the account details then respond bad username or password
            if (!hashMatchesPassword)
            {
                // If password is incorrect then send back bad username or password. MUST be identical to incorrect password.
                response.StatusCode = (int)HttpStatusCode.BadRequest;
                await SendHttpResponse(response, new { message = "Username or password is incorrect." });
                return;
            }

            // If everything is matching then send back the OK signal and message user logged in
            // TODO: Introduce security token
            response.StatusCode = (int)HttpStatusCode.OK;
            await SendHttpResponse(response, new { message = "User Logged in successfully.", username = loginDetails.Username });
        }



        // Testing function returns all usernames
        public static async Task ReturnAll(string requestBody, DataBase DBCon, HttpListenerContext context)
        {
            // Try catch to prevent errors crashing server
            try
            {

                // Debug line to show that program has made it inside return all function
                Debug.WriteLine("Inside return all");

                // Gets response and content type prepared
                HttpListenerResponse response = context.Response; 
                response.ContentType = "application/json";


                // returns a dictionary string string to collect all the  <userID, username> userid is string in this instance 

                //TODO : investigate more appropriate response for having int string reponse
                List<UserData> userData = await DBCon.ReturnAllUsers();

                // Status code is ok and returns data fethced
                response.StatusCode = (int)HttpStatusCode.OK;
                await SendHttpResponse(response, new
                {
                    message = "User data retrieved successfully.",
                    users = userData
                });
            }
            catch (Exception ex)
            {
                // Handle exceptions and set the appropriate HTTP response
                HttpListenerResponse response = context.Response;
                response.ContentType = "application/json";
                response.StatusCode = (int)HttpStatusCode.InternalServerError;

                await SendHttpResponse(response, new
                {
                    message = "An error occurred.",
                    error = ex.Message
                });
            }
        }


        /*
        public static async Task CreateNewConversation(string requestBody, DataBase DBCon, HttpListenerContext context)
        {
            try
            {


                //TODO clean the ids, make sure they are real ids and that they are unique
                // Prepare response
                HttpListenerResponse response = context.Response;
                response.ContentType = "application/json";


                var conversationObject = RequestBodyExtractor.ParseConversationIDs(requestBody);


                // If error extracting or issue with variables being null then return an error
                if (!conversationObject.IsSuccess)
                {
                    response.StatusCode = (int)HttpStatusCode.InternalServerError;
                    await SendHttpResponse(response, new { message = conversationObject.ErrorMessage });
                    return;
                }
                


                // Database search to check both IDS already exist
                bool idExists = await DBCon.CheckUserIDExists((int)conversationObject.Value.LowerUserID);

                idExists = idExists && await DBCon.CheckUserIDExists((int)conversationObject.Value.HigherUserID);

                // If an id does not exist repond with badrequest
                if (!idExists)
                {
                    response.StatusCode = (int)HttpStatusCode.BadRequest;
                    await SendHttpResponse(response, new { message = "A UserID Does not exist." });
                    return;
                }






                bool conversationExists = await DBCon.CheckConversationExists(conversationObject.Value);

                if (conversationExists)
                {
                    response.StatusCode = (int)HttpStatusCode.Conflict;
                    await SendHttpResponse(response, new { message = "Conversation Already Exists" });
                    return;
                }

                var insertionStatus = await DBCon.AddNewConversation(conversationObject.Value);

                if (insertionStatus.IsSuccess)
                {
                    response.StatusCode = (int)HttpStatusCode.OK;
                    await SendHttpResponse(response, new { message = "Converesation Created" });
                    Console.WriteLine("New Conversation Created");
                }
                else
                {
                    throw new NpgsqlException("Error inserting new conversation into database");
                }
            }
            catch (NpgsqlException ex)
            {

                Console.WriteLine(ex.ToString());

                HttpListenerResponse response = context.Response;
                response.ContentType = "application/json";
                response.StatusCode = (int)HttpStatusCode.InternalServerError;
                await SendHttpResponse(response, new { message = "Error inserting into database" });
                return;
            }
            catch (Exception ex)
            {
                Console.WriteLine(ex.ToString());

                HttpListenerResponse response = context.Response;
                response.ContentType = "application/json";
                response.StatusCode = (int)HttpStatusCode.InternalServerError;
                await SendHttpResponse(response, new { message = "Error processing request" });
                return;

            }
        }
        */



        public static async Task CreateNewConversationByName(string requestBody, DataBase DBCon, HttpListenerContext context)
        {
            try
            {


                //TODO clean the ids, make sure they are real ids and that they are unique
                // Prepare response
                HttpListenerResponse response = context.Response;
                response.ContentType = "application/json";


                var conversationObject = RequestBodyExtractor.ParseConversationNames(requestBody);

                // If error extracting or issue with variables being null then return an error
                if (!conversationObject.IsSuccess)
                {
                    response.StatusCode = (int)HttpStatusCode.InternalServerError;
                    await SendHttpResponse(response, new { message = conversationObject.ErrorMessage });
                    return;
                }



                // Database search to check both IDS already exist
                var F_UserData = await DBCon.GetUserDetailsByUsername(conversationObject.Value.F_UserName);
                
                var Lo_UserData = await DBCon.GetUserDetailsByUsername(conversationObject.Value.Lo_UserName);
                


                // If an id does not exist repond with badrequest
                if (!F_UserData.IsSuccess || !Lo_UserData.IsSuccess)
                {
                    response.StatusCode = (int)HttpStatusCode.BadRequest;
                    await SendHttpResponse(response, new { message = "A Username Does not exist." });
                    return;
                }

                Console.WriteLine($"\n{Lo_UserData.Value.Username} (UID: {Lo_UserData.Value.Id}) Requests Conversation with: {F_UserData.Value.Username} (UID: {F_UserData.Value.Id})");


                List<UserData> userList = new List<UserData> { F_UserData.Value, Lo_UserData.Value };

                // Sort the list by Id in ascending order . Good function to remember
                var sortedUserList = userList.OrderBy(user => user.Id).ToList();

                ConversationClass conversation = new ConversationClass
                {
                    LowerUserID = userList[0].Id,
                    HigherUserID = userList[1].Id
                };



                bool conversationExists = await DBCon.CheckConversationExists(conversation);

                if (conversationExists)
                {
                    response.StatusCode = (int)HttpStatusCode.Conflict;
                    await SendHttpResponse(response, new { message = "Conversation Already Exists" });
                    return;
                }
                
                var insertionStatus = await DBCon.AddNewConversation(conversation);

                if (insertionStatus.IsSuccess)
                {
                    response.StatusCode = (int)HttpStatusCode.OK;
                    await SendHttpResponse(response, new { message = "Converesation Created" });
                    Console.WriteLine("New Conversation Created");
                }
                else
                {
                    throw new NpgsqlException("Error inserting new conversation into database");
                }
            }
            catch (NpgsqlException ex)
            {
                HttpListenerResponse response = context.Response;
                response.ContentType = "application/json";
                response.StatusCode = (int)HttpStatusCode.InternalServerError;
                await SendHttpResponse(response, new { message = "Error inserting into database" });
                return;
            }
            catch (Exception ex)
            {
                Console.WriteLine(ex.ToString());

                HttpListenerResponse response = context.Response;
                response.ContentType = "application/json";
                response.StatusCode = (int)HttpStatusCode.InternalServerError;
                await SendHttpResponse(response, new { message = "Error processing request" });
                return;

            }
            
        }




        public static async Task ProcessAccountRegistration(string requestBody, DataBase DBCon, HttpListenerContext context)
        {
            try
            {
                // Prepare initial reponse content type
                HttpListenerResponse response = context.Response;
                response.ContentType = "application/json";


                // Clean the details to ensure that they meet requirements
                CleanDetailsForm loginDetails = RequestBodyExtractor.ParseAccountCredentials(requestBody);

                if (loginDetails.Result == "Error" || loginDetails.Result == "Reject")
                {
                    response.StatusCode = (int)HttpStatusCode.BadRequest;
                    await SendHttpResponse(response, new { message = loginDetails.ErrorMessage });
                    return;
                }



                // Check Username Exists
                bool usernameExists = await DBCon.CheckAccountExisting(loginDetails.Username);
                if (usernameExists)
                {
                    response.StatusCode = (int)HttpStatusCode.BadRequest;
                    await SendHttpResponse(response, new { message = "Username is taken." });
                    return;
                }



                // Generate Hash salt and out
                HashInformation hashObj = HashMethodsClass.GenerateHashInformation(loginDetails.Password);


                // If there is an issue with the hash repond with the error TODO Logging
                if (hashObj.Result != "OK")
                {
                    response.StatusCode = (int)HttpStatusCode.InternalServerError;
                    await SendHttpResponse(response, new { message = "Failed to generate password hash." });
                    return;
                }


                // Password and username is fine, register the account
                JObject responseJs = await DBCon.CreateNewAccount(loginDetails.Username, hashObj.PasswordHash, hashObj.PasswordSalt);

                // Check if internal error occured
                if (responseJs["Result"].ToString() != "OK")
                {
                    response.StatusCode = (int)HttpStatusCode.Conflict; // TODO :Investigate correct http status codes
                    await SendHttpResponse(response, new { message = "Registration failed. Internal Error." });
                    return;
                }


                // Final check that account is inside the database
                AccInfoObj accountDetails = await DBCon.GetAccountDetailsAsync(loginDetails.Username, hashObj.PasswordHash);

                if ( accountDetails.Result != "OK")
                {
                    response.StatusCode = (int)HttpStatusCode.Conflict; // TODO :Investigate correct http status codes
                    await SendHttpResponse(response, new { message = "Registration failed. Internal Error." });
                    return;
                }

                response.StatusCode = (int)HttpStatusCode.OK;
                await SendHttpResponse(response, new { message = "User registered successfully." });
            }
            catch (Exception ex)
            {
                Console.WriteLine(ex.ToString());

            }
           
        }



        private static async Task SendHttpResponse(HttpListenerResponse response, object responseBody)
        {
            try
            {
                string jsonResponse = JsonConvert.SerializeObject(responseBody);
                byte[] buffer = Encoding.UTF8.GetBytes(jsonResponse);
                response.ContentLength64 = buffer.Length;
                await response.OutputStream.WriteAsync(buffer, 0, buffer.Length);
                response.Close();
            }
            catch (Exception ex)
            {
                Console.WriteLine(ex.ToString());
            }
            
        }
    }
}
