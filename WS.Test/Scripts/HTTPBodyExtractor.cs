using Newtonsoft.Json.Linq;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Net;
using System.Text;
using System.Text.RegularExpressions;
using System.Threading.Tasks;
using WS.Test.ObjectClasses;

namespace WS.Test.Scripts
{
    internal class HTTPBodyExtractor
    {
        public static JObject getFormBody(string requestBody)
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


        public static ConversationClass ParseConversationIDs(string requestBody)
        {
            

            try
            {
                JObject JBody = HTTPBodyExtractor.getFormBody(requestBody);

                if (JBody["Result"].ToString() != "OK")
                {
                    ConversationClass errorDetails = new ConversationClass
                    {
                        Result = "OK",
                        ErrorMessage = "Error"
                    };


                    return errorDetails;
                }



                // TODO: Check better solution
                int user_ID_1 = Int32.Parse(JBody["user_ID_1"].ToString());
                int user_ID_2 = Int32.Parse(JBody["user_ID_2"].ToString());

                int[] user_Ids = { user_ID_1, user_ID_2 };
                Array.Sort(user_Ids);
                if (user_Ids[0] <= 0)
                {

                }

                ConversationClass conversationDetails = new ConversationClass
                {
                    Result = "OK",
                    LowerUserID = user_Ids[0],
                    HigherUserID = user_Ids[1]
                };

                return conversationDetails;


            }
            catch (Exception ex)
            {
                Console.WriteLine($"{ex.Message}");
                ConversationClass errorDetails = new ConversationClass
                {
                    Result = "OK",
                    ErrorMessage = "Error"
                };
                return errorDetails;
            }
        }


        public static CleanDetailsForm CleanLoginDetails(string requestBody)
        {
            try
            {
                JObject JBody = HTTPBodyExtractor.getFormBody(requestBody);

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

