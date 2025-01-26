using MessangerServer.ObjectClasses.Conversations;
using MessangerServer.Scripts.Wrappers;
using Newtonsoft.Json.Linq;
using System;
using System.Collections.Generic;
using System.Data;
using System.Linq;
using System.Net;
using System.Text;
using System.Text.RegularExpressions;
using System.Threading.Tasks;
using WS.Test.ObjectClasses;

namespace WS.Test.Scripts
{
    internal class RequestBodyExtractor
    {
        public static JObject ExtractRequestBodyValues(string requestBody)
        {
            try
            {
                JObject formBody = new JObject();
                
                formBody.Add(new JProperty("Result", "OK"));

                var formData = System.Web.HttpUtility.ParseQueryString(requestBody);

                foreach (string key in formData.AllKeys)
                {
                    formBody.Add(new JProperty(key, formData[key]));

                }

                return formBody;
            }
            catch(Exception ex) 
            {
                Console.Write(ex);
                
                return errorReturn(ex);

            }
            
        }
        public static JObject errorReturn(Exception ex)
        {
            JObject errorReturn = new JObject();//TODO: 
            errorReturn.Add(new JProperty("Result", "Error"));
            errorReturn.Add(new JProperty("Error:", ex));
            return errorReturn;
        }


        public static Result<ConversationClass> ParseConversationIDs(string requestBody)
        {
            

            try
            {
                JObject JBody = RequestBodyExtractor.ExtractRequestBodyValues(requestBody);

                if (JBody["Result"].ToString() != "OK")
                {
                    return Result<ConversationClass>.Failure("Error extracting request body");
                }



                // TODO: Check better solution
                int user_ID_1 = Int32.Parse(JBody["user_ID_1"].ToString());
                int user_ID_2 = Int32.Parse(JBody["user_ID_2"].ToString());

                int[] user_Ids = { user_ID_1, user_ID_2 };
                Array.Sort(user_Ids);



                // Checks if either userid is less than 0 and returns an error if so

                if (user_ID_1 <= 0 || user_ID_2 <=0)
                {
                    return Result<ConversationClass>.Failure("Provided User ID less than or equal to 0");
                }


                // Checks if identical userids was provided, if so returns error
                if (user_ID_1 == user_ID_2)
                {
                    return Result<ConversationClass>.Failure("Identical User Ids Provided");
                }


                ConversationClass conversationDetails = new ConversationClass
                {
                    LowerUserID = user_Ids[0],
                    HigherUserID = user_Ids[1]
                };

                return Result<ConversationClass>.Success(conversationDetails);


            }
            catch (Exception ex)
            {
                Console.WriteLine($"{ex.Message}");
                return Result<ConversationClass>.Failure("Internal Server Error");
            }
        }

        public static Result<ConversationByName> ParseConversationNames(string requestBody)
        {
            

            try
            {
                JObject JBody = RequestBodyExtractor.ExtractRequestBodyValues(requestBody);

                if (JBody["Result"].ToString() != "OK")
                {
                    return Result<ConversationByName>.Failure("Error extracting request body");

                }


                string F_UserName = JBody["foriegnUserName"].ToString();
                string Lo_UserName = JBody["localUserName"].ToString();

                // Checks if identical names was provided, if so returns error
                if (F_UserName.Equals(Lo_UserName))
                {
                    return Result<ConversationByName>.Failure("Identical User names Provided");
                }

                if (F_UserName.Equals(null) || Lo_UserName.Equals(null)){
                    return Result<ConversationByName>.Failure("A username provided is null");
                }




                ConversationByName conversationDetails = new ConversationByName
                {
                    F_UserName = F_UserName,
                    Lo_UserName = Lo_UserName
                };

                return Result<ConversationByName>.Success(conversationDetails);


            }
            catch (Exception ex)
            {
                Console.WriteLine($"Error message while parsing conversation by names: {ex.Message}");

                return Result<ConversationByName>.Failure("Error message while parsing conversation by names: {ex.Message}");
            }
        }


        public static CleanDetailsForm ParseAccountCredentials(string requestBody)
        {
            try
            {
                JObject JBody = RequestBodyExtractor.ExtractRequestBodyValues(requestBody);

                if (JBody["Result"].ToString() != "OK")
                {

                    return new CleanDetailsForm
                    {
                        Result = "Reject",
                        ErrorMessage = "Invalid Form Data"
                    };
                }

                if (JBody["username"] == null || JBody["password"] == null)
                {
                    return new CleanDetailsForm
                    {
                        Result = "Reject",
                        ErrorMessage = "Invalid form request. Username and password fields not present."
                    };
                }

                string userN = JBody["username"].ToString();
                string userP = JBody["password"].ToString();
                string regex = @"^(?=.*[a-zA-Z])[a-zA-Z0-9]+$";

                if (!Regex.IsMatch(userN, regex))
                {
                    return new CleanDetailsForm
                    {
                        Result = "Reject",
                        ErrorMessage = "Username can only contain letters and numbers."
                    };
                }

                if (userN.Length >= 20)
                {
                    return new CleanDetailsForm
                    {
                        Result = "Reject",
                        ErrorMessage = "Usernames can not be more than 20 characters."
                    };
                } else if (userN.Length < 3)
                {
                    return new CleanDetailsForm
                    {
                        Result = "Reject",
                        ErrorMessage = "Usernames must be atleast 3 characters long."
                    };
                }



                if (string.IsNullOrWhiteSpace(userN) || string.IsNullOrWhiteSpace(userP))
                {

                    return new CleanDetailsForm
                    {
                        Result = "Reject",
                        ErrorMessage = "Username or Password can not be empty."
                    };
                }




                CleanDetailsForm returnMessage = new CleanDetailsForm
                {
                    Result = "OK",
                    Username = userN,
                    Password = userP
                };
                return returnMessage;

            }
            catch (Exception ex)
            {
                Console.WriteLine($"{ex.Message}");
                CleanDetailsForm returnMessage = new CleanDetailsForm
                {
                    Result = "Error",
                    ErrorMessage = "Unexpected server error"
                };
                return returnMessage;
            }
        }

    }
    
}

