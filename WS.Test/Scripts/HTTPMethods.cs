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

namespace WS.Test.Scripts
{
    internal class HTTPMethods
    {
        public static async Task SignIn(string requestBody, DataBase DBCon, HttpListenerContext context)
        {
            // turn the form into a jobject TODO: Turn into custom object class, with error handling for null
            HttpListenerResponse response = context.Response;
            response.ContentType = "application/json";

            CleanDetailsForm loginDetails = HTTPBodyExtractor.CleanLoginDetails(requestBody);

            if (loginDetails.Result == "Error")
            {
                response.StatusCode = (int)HttpStatusCode.BadRequest;
                await SendHttpResponse(response, new { message = loginDetails.ErrorMessage });
                return;
            }

            string userN = loginDetails.Username;
            string userP = loginDetails.Password;



            AccInfoObj accountDetails = await DBCon.GetAccountByName(userN);
            if (accountDetails.Result == "Error")
            {
                response.StatusCode = (int)HttpStatusCode.BadRequest;
                await SendHttpResponse(response, new { message = "Username or password is incorrect." });
                return;
            } else if (accountDetails.Result == "NONE")
            {
                response.StatusCode = (int)HttpStatusCode.BadRequest;
                await SendHttpResponse(response, new { message = "Username or password is incorrect." });
                return;
            }

            byte[] passHash = accountDetails.passwordHash;
            byte[] passSalt = accountDetails.passwordSalt;
            bool correct = DBCon.checkHash(userP, passSalt, passHash);

            if (!correct)
            {
                response.StatusCode = (int)HttpStatusCode.BadRequest;
                await SendHttpResponse(response, new { message = "Username or password is incorrect." });
                return;
            }

            response.StatusCode = (int)HttpStatusCode.OK;
            await SendHttpResponse(response, new { message = "User Logged in successfully.", username = userN });
        }


        public static async Task ReturnAll(string requestBody, DataBase DBCon, HttpListenerContext context)
        {
            try
            {
                Debug.WriteLine("Inside return all");
                HttpListenerResponse response = context.Response;
                response.ContentType = "application/json";



                Dictionary<string, string> userData = await DBCon.ReturnAllUsers();
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





        public static async Task RegisterAccount(string requestBody, DataBase DBCon, HttpListenerContext context)
        {
            try
            {
                Debug.WriteLine("register account:" + requestBody);
                HttpListenerResponse response = context.Response;
                response.ContentType = "application/json";

                CleanDetailsForm loginDetails = HTTPBodyExtractor.CleanLoginDetails(requestBody);

                if (loginDetails.Result == "Error")
                {
                    response.StatusCode = (int)HttpStatusCode.BadRequest;
                    await SendHttpResponse(response, new { message = loginDetails.ErrorMessage });
                    return;
                }

                string userN = loginDetails.Username;
                string userP = loginDetails.Password;

                HashInformation hashObj = DBCon.generateHash(userP);

                if (hashObj.Result != "OK")
                {
                    response.StatusCode = (int)HttpStatusCode.InternalServerError;
                    await SendHttpResponse(response, new { message = "Failed to generate password hash." });
                    return;
                }

                JObject responseJs = await DBCon.RegisterAccount(userN, hashObj.PasswordHash, hashObj.PasswordSalt);
                if (responseJs["Result"].ToString() != "OK")
                {
                    response.StatusCode = (int)HttpStatusCode.Conflict;
                    await SendHttpResponse(response, new { message = "Registration failed. User may already exist." });
                    return;
                }
                AccInfoObj accountDetails = await DBCon.GetAccountDetailsAsync(userN, hashObj.PasswordHash);

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
