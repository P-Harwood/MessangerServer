using messanger.DataObjects.LocalDataPackets;
using messanger.DataObjects.HTTPObjs;
using messanger.DataObjects.WebSocketPackets;
using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using System.Net;
using System.Net.Http;
using System.Text;
using System.Threading.Tasks;
using messanger.DataObjects.Conversations;
using messanger.WrapperClasses;

namespace messanger.Scripts.NetworkCommunications
{
    public class RestfulRequests
    {
        private static HttpClient sharedClient = new()
        {
            BaseAddress = new Uri("http://127.0.0.1:5000/ws"),
        };


        public async static Task<Result<HttpJsonObj>> GetAsync(string urlExetnsion)
        {
            try
            {
                // Get the full url to be posted, for the logs
                string fullUrl = new Uri(sharedClient.BaseAddress, urlExetnsion).ToString();

                // Log the full URL to ensure it's formatted correctly
                Debug.WriteLine($"Sending GET to: {fullUrl}");


                //sends the packet with url extension and the form attached
                using (HttpResponseMessage response = await sharedClient.GetAsync(
                    urlExetnsion))
                {
                    // throws exception if httpsstatuscode comes back as not ok
                    response.EnsureSuccessStatusCode();

                    // turn the response body to json string
                    var jsonResponse = await response.Content.ReadAsStringAsync();

                    //fetch  status code
                    HttpStatusCode responseCode = response.StatusCode;

                    // double check the status code is ok TODO: test remove
                    if (responseCode == HttpStatusCode.OK)
                    {
                        //create the dataobject to return
                        HttpJsonObj dataObj = new HttpJsonObj
                        {
                            successfull = true,
                            responseCode = responseCode,
                            jsonRespnse = jsonResponse
                        };
                        //return the response
                        return Result<HttpJsonObj>.Success(dataObj);
                    }

                    return Result<HttpJsonObj>.Failure(("Get Failure Network Code: " + responseCode));

                }

                
            }
            catch (HttpRequestException ex)
            {
                Debug.WriteLine($"Request error: {ex.Message}");
                return Result<HttpJsonObj>.Failure(("HttpRequestException: " + ex.Message));

            }
            catch (TaskCanceledException ex)
            {
                Debug.WriteLine($"TaskCanceledException: {ex.Message}");
                return Result<HttpJsonObj>.Failure(("TaskCanceledException: " + ex.Message));

            }
            catch (Exception ex)
            {
                Debug.WriteLine($"Unknown Exception in GetAsync: {ex.Message}");
                return Result<HttpJsonObj>.Failure(("Unknown Exception in GetAsync: " + ex.Message));

            }
        }


        /* modular method which can be used for post requests, takes url extension (note: extension is ws/"extension"
         * takes dictionary to send ( dataobjects should have a create dictionary method), default of formdata null incase it is not needed */

        private async static Task<Result<HttpJsonObj>> PostAsync(string urlExetnsion, Dictionary<string, string> formData = null)
        {
            try
            {
                // Get the full url to be posted, for the logs
                string fullUrl = new Uri(sharedClient.BaseAddress, urlExetnsion).ToString();

                // Log the full URL to ensure it's formatted correctly
                Debug.WriteLine($"Sending POST to: {fullUrl}");

                //encodes the form data as x-www encoded formdata
                using (FormUrlEncodedContent urlEncodedContent = new(formData))
                {
                    
                    //sends the packet with url extension and the form attached
                    using (HttpResponseMessage response = await sharedClient.PostAsync(
                     urlExetnsion,
                     urlEncodedContent))
                    {
                        // throws exception if httpsstatuscode comes back as not ok
                        response.EnsureSuccessStatusCode();

                        // turn the response body to json string
                        var jsonResponse = await response.Content.ReadAsStringAsync();

                        //fetch  status code
                        HttpStatusCode responseCode = response.StatusCode;

                        // double check the status code is ok TODO: test remove
                        if (responseCode == HttpStatusCode.OK)
                        {
                            //create the dataobject to return
                            HttpJsonObj dataObj = new HttpJsonObj
                            {
                                successfull = true,
                                responseCode = responseCode,
                                jsonRespnse = jsonResponse
                            };
                            //return the response
                            return Result<HttpJsonObj>.Success(dataObj);
                        }


                        return Result<HttpJsonObj>.Failure(("POST Failure Network Code: " + responseCode));

                    }

                }
            }
            catch (HttpRequestException ex)
            {
                Debug.WriteLine($"Request error: {ex.Message}");
                return Result<HttpJsonObj>.Failure(("HttpRequestException: " + ex.Message));

            }
            catch (TaskCanceledException ex)
            {
                Debug.WriteLine($"TaskCanceledException: {ex.Message}");
                return Result<HttpJsonObj>.Failure(("TaskCanceledException: " + ex.Message));

            }
            catch (Exception ex)
            {
                Debug.WriteLine($"Unknown Exception in POSTAsync: {ex.Message}");
                return Result<HttpJsonObj>.Failure(("Unknown Exception in POSTAsync: " + ex.Message));

            }
        }



        private async static Task<Result<HttpJsonObj>> PutAsync(string urlExetnsion, Dictionary<string, string> formData = null)
        {
            try
            {
                // Get the full url to be posted, for the logs
                string fullUrl = new Uri(sharedClient.BaseAddress, urlExetnsion).ToString();

                // Log the full URL to ensure it's formatted correctly
                Debug.WriteLine($"Sending POST to: {fullUrl}");

                //encodes the form data as x-www encoded formdata
                using (FormUrlEncodedContent urlEncodedContent = new(formData))
                {

                    //sends the packet with url extension and the form attached
                    using (HttpResponseMessage response = await sharedClient.PutAsync(
                     urlExetnsion,
                     urlEncodedContent))
                    {
                        // throws exception if httpsstatuscode comes back as not ok
                        response.EnsureSuccessStatusCode();

                        // turn the response body to json string
                        var jsonResponse = await response.Content.ReadAsStringAsync();

                        //fetch  status code
                        HttpStatusCode responseCode = response.StatusCode;

                        // double check the status code is ok TODO: test remove
                        if (responseCode == HttpStatusCode.OK)
                        {
                            //create the dataobject to return
                            HttpJsonObj dataObj = new HttpJsonObj
                            {
                                successfull = true,
                                responseCode = responseCode,
                                jsonRespnse = jsonResponse
                            };
                            //return the response
                            return Result<HttpJsonObj>.Success(dataObj);
                        }


                        return Result<HttpJsonObj>.Failure(("POST Failure Network Code: " + responseCode));

                    }

                }
            }
            catch (HttpRequestException ex)
            {
                Debug.WriteLine($"Request error: {ex.Message}");
                return Result<HttpJsonObj>.Failure(("HttpRequestException: " + ex.Message));

            }
            catch (TaskCanceledException ex)
            {
                Debug.WriteLine($"TaskCanceledException: {ex.Message}");
                return Result<HttpJsonObj>.Failure(("TaskCanceledException: " + ex.Message));

            }
            catch (Exception ex)
            {
                Debug.WriteLine($"Unknown Exception in POSTAsync: {ex.Message}");
                return Result<HttpJsonObj>.Failure(("Unknown Exception in POSTAsync: " + ex.Message));

            }
        }







        public async static Task<Result<ServerListObj>> RequestAllUsers()
        {
            Debug.WriteLine("Sending request for all users");
            try
            {
                // Do get action
                var dataObj = await GetAsync("ws/AllUsers");


                // check response from get action
                if (!dataObj.IsSuccess)
                {
                    return Result<ServerListObj>.Failure((dataObj.ErrorMessage));
                }

                // create ServerList Object
                ServerListObj serverList = new ServerListObj();

                // Add serverlist information to object
                serverList.ProcessJsonResponse(dataObj.Value.jsonRespnse);

                return Result<ServerListObj>.Success(serverList);
            }
            catch (Exception ex)
            {
                // DEBUG log server list
                Debug.WriteLine("Error while requesting all users: ", ex);
                return Result<ServerListObj>.Failure(ex.Message);
            }
        }


        public async static Task<Result<SignInObject>> SignInHTTP(SignInObject signInObj)
        {
            try 
            { 
                var formData = signInObj.returnDictionary();

                if (!formData.IsSuccess)
                {
                    return Result<SignInObject>.Failure(formData.ErrorMessage);
                }


       
                // Send Http response
                var dataObj = await PostAsync("ws/SignIn", formData.Value);

                // if Response was not successful
                if (!dataObj.IsSuccess)
                {
                    return Result<SignInObject>.Failure("HTTP Sign in network Failure");
                }

                // Parse the Json Response into a sign in object
                var parsedSignIn = ParseJson.ParseSignIn(dataObj.Value.jsonRespnse);

                // check if it is successful
                if (!parsedSignIn.IsSuccess)
                {
                    return Result<SignInObject>.Failure(parsedSignIn.ErrorMessage);
                }

                //return successfull sign in object
                return Result<SignInObject>.Success(parsedSignIn.Value);
               
            }
            catch (Exception ex)
            {
                return Result<SignInObject>.Failure(ex.ToString());
            }
        }



        // Function for requesting new conversation, returns a conversationobject wrapped by result
        public async static Task<Result<ConversationObject>> RequestNewConversation(NewConversRequest passedConversation)
        {

            try
            {
                // Collapse the NewConvers into a dictionary
                var conversationUserIDs = passedConversation.ReturnDictionary();


                // If it collapses bad, (it can fail for issues such as inadaquete ids this error occurs)
                if (!conversationUserIDs.IsSuccess)
                {
                    // DEBUG write error emssage
                    Debug.WriteLine(conversationUserIDs.ErrorMessage);

                    // Return wrapped failure
                    return Result<ConversationObject>.Failure(conversationUserIDs.ErrorMessage);
                }

                // Send the post request for creating the conversation
                var serverResponse = await PostAsync("ws/NewConversation", conversationUserIDs.Value);

                // If issue in server request
                if (!serverResponse.IsSuccess)
                {
                    // TODO : Include network response
                    // Return Failure
                    return Result<ConversationObject>.Failure("Server Error");
                }

                // Parse Json Response (error handling is passed on through result wrap)
                return ParseJson.ParseConversation(serverResponse.Value.jsonRespnse);

            }
            catch (Exception ex)
            {
                return Result<ConversationObject>.Failure(ex.Message);
            }
        }

        public async static Task<Result<ConversationObject>> RequestNewConversationByUsername(ConversationByName passedConversation)
        {

            try
            {
                // Collapse the NewConvers into a dictionary
                var conversationUserIDs = passedConversation.ReturnDictionary();


                // If it collapses bad, (it can fail for issues such as inadaquete ids this error occurs)
                if (!conversationUserIDs.IsSuccess)
                {
                    // DEBUG write error emssage
                    Debug.WriteLine(conversationUserIDs.ErrorMessage);

                    // Return wrapped failure
                    return Result<ConversationObject>.Failure(conversationUserIDs.ErrorMessage);
                }

                // Send the post request for creating the conversation
                var serverResponse = await PutAsync("ws/NewConversationByName", conversationUserIDs.Value);

                // If issue in server request
                if (!serverResponse.IsSuccess)
                {
                    // TODO : Include network response
                    // Return Failure
                    return Result<ConversationObject>.Failure("Server Error");
                }

                // Parse Json Response (error handling is passed on through result wrap)
                return ParseJson.ParseConversation(serverResponse.Value.jsonRespnse);

            }
            catch (Exception ex)
            {
                return Result<ConversationObject>.Failure(ex.Message);
            }
        }







        public async static Task<Result<RegisterObject>> RegisterHTTP(RegisterObject regiObj)
        {
            try
            {

                Dictionary<string, string> formData = regiObj.returnDictionary();
                if (formData == null)
                {
                    Debug.WriteLine("Null formdata");
                    //TODO: enter password prompt
                    return Result<RegisterObject>.Failure("Null Data in register https function");
                }


                // Send Register http put reques
                var serverResponse = await PutAsync("ws/Register", formData);
                if (!serverResponse.IsSuccess)
                {
                    return Result<RegisterObject>.Failure(serverResponse.ErrorMessage);
                }


                // try and parse response TODO this will need a new sign in
                var parsedAccDetails = ParseJson.ParseSignIn(serverResponse.Value.jsonRespnse);

                if (!parsedAccDetails.IsSuccess)
                {
                    return Result<RegisterObject>.Failure(parsedAccDetails.ErrorMessage);
                }

                //TODO Add content to this
                RegisterObject resultObj = new RegisterObject
                {
                    successfull = true,

                };

                // Return result
                return Result<RegisterObject>.Success(resultObj);
   
            }
            catch (Exception ex)
            {
                return Result<RegisterObject>.Failure(ex.Message);
            }
        }
    }
}
