using messanger.DataObjects;
using messanger.DataObjects.Conversations;
using messanger.DataObjects.LocalDataPackets;
using messanger.DataObjects.WebSocketPackets;
using messanger.WrapperClasses;
using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using System.Net.Http.Headers;
using System.Reflection.Metadata;
using System.Text;
using System.Text.Json;
using System.Text.Json.Nodes;
using System.Threading.Tasks;

namespace messanger.Scripts
{
    internal class ParseJson
    {




        public static string parseJsonHeader(string jsonString)
        {
            using (JsonDocument document = JsonDocument.Parse(jsonString))
            {
                JsonElement root = document.RootElement;

                string header = root.GetProperty("PacketHeader").GetString();

                return header;

            }
        }

        public static MessageObject ParseMesObj(string jsonString)
        {
            try
            {
                using (JsonDocument document = JsonDocument.Parse(jsonString))
                {
                    JsonElement root = document.RootElement;

                    string sender = root.GetProperty("Sender").GetString();
                    string content = root.GetProperty("Content").GetString();

                    MessageObject message = new MessageObject
                    {
                        Result = "OK",
                        Content = content,
                        Sender = sender
                    };
                    return message;
                }
            }
            catch (Exception ex)
            {
                Console.WriteLine(ex.Message);
                MessageObject message = new MessageObject
                {
                    Result = "ERROR",
                    ErrorMessage = ex.Message
                };
                return message;
            }
        }

        // Parse Json recieved after attempting to login
        public static Result<SignInObject> ParseSignIn(string jsonString)
        {
            try
            {
                using (JsonDocument document = JsonDocument.Parse(jsonString))
                {
                    JsonElement root = document.RootElement;

                    // get whether username was returned in http reponse
                    bool usernameExists = root.TryGetProperty("username", out JsonElement value);

                    // TODO there should always be a username since it is parsing a successful network output, but validation regardless
                    if (!usernameExists)
                    {
                        return Result<SignInObject>.Failure("Internal server error");
                    }

                    // get value of messageContent and username
                    string messageContent = root.GetProperty("message").GetString();
                    string username = root.GetProperty("username").GetString();

                    //return messagecontent and username

                    SignInObject SignInResult = new SignInObject { username = username };

                    return Result<SignInObject>.Success(SignInResult);
                }
            }
            catch (Exception ex)
            {

                return Result<SignInObject>.Failure(ex.Message);
            }
        }


        public static Result<ConversationObject >ParseConversation(string jsonString)
        {
            // Function used for parsing the json response and returns it in a wrapped result

            try
            {
                using (JsonDocument document = JsonDocument.Parse(jsonString))
                {
                    JsonElement root = document.RootElement;

                    
                    // get value of messageContent and username
                    int conversation_Id = root.GetProperty("conversation_Id").GetInt32();
                    int user_id_1 = root.GetProperty("user_id_1").GetInt32();
                    int user_id_2 = root.GetProperty("user_id_2").GetInt32();
                    string created_at = root.GetProperty("created_at").GetString();

                    //return messagecontent and username

                    ConversationObject Conversation = new ConversationObject
                        {
                            conversation_Id = conversation_Id,
                            user_id_1 = user_id_1,
                            user_id_2 = user_id_2,
                            created_at = created_at
                        };

                    return Result<ConversationObject>.Success(Conversation);


                }
            }
            catch (Exception ex)
            {
                Debug.WriteLine(ex.Message);
                return Result<ConversationObject>.Failure("ex.Message");
            }
        }




        public static ServerListObj ParseServerList(string jsonString)
        {
            try
            {
                Debug.WriteLine("parsing server list");
                Debug.WriteLine(jsonString);

                using (JsonDocument document = JsonDocument.Parse(jsonString))
                {
                    JsonElement rootElement = document.RootElement;

                    // Check if "userList" exists and is an array
                    if (rootElement.TryGetProperty("userList", out JsonElement userListElement) && userListElement.ValueKind == JsonValueKind.Array)
                    {
                        List<string> userList = new List<string>();

                        // Iterate through the array and add elements to the list
                        foreach (JsonElement element in userListElement.EnumerateArray())
                        {
                            userList.Add(element.GetString());
                        }

                        // Convert the list to a string array
                        string[] userArray = userList.ToArray();

                        // Return object with the extracted array
                        ServerListObj returnObj = new ServerListObj
                        {
                            Result = "Success",
                            //Users = userArray,
                            ErrorMessage = null // No error
                        };

                        return returnObj;
                    }
                }

                // In case no user list is found
                ServerListObj emptyReturnObj = new ServerListObj
                {
                    Result = "Error",
                    ErrorMessage = "No userList found"
                };
                return emptyReturnObj;

            }
            catch (Exception ex)
            {
                Console.WriteLine(ex.Message);
                ServerListObj returnObj = new ServerListObj
                {
                    ErrorMessage = ex.Message
                };
                return returnObj;
            }
        }
    }
}

