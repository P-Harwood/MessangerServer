using System;
using System.Net;
using System.Net.WebSockets;
using System.Text;
using System.Threading;
using System.Threading.Tasks;
using Newtonsoft.Json;
using System.Collections.Concurrent;
using System.Data.Common;
using Newtonsoft.Json.Linq;
using System.Collections.Generic;
using Newtonsoft.Json.Schema;
using WS.Test.ObjectClasses;
using WS.Test.Scripts;
using System.Data;
using System.Diagnostics;


namespace WS.Test
{
    class Program
    {
        static ConcurrentDictionary<Guid, WebSocket> connectedUsers = new ConcurrentDictionary<Guid, WebSocket>();
        static ConcurrentDictionary<Guid, string> connectedUsernames = new ConcurrentDictionary<Guid, string>();
        static string url = "http://127.0.0.1:5000/ws/";

        static async Task Main(string[] args)
        {
            DataBase DBCon = new DataBase();
            HttpListener httpListener = new HttpListener();
            httpListener.Prefixes.Add(url);
            httpListener.Start();
            Console.WriteLine("WebSocket server started at ws://localhost:5000/ws/");

            while (true)
            {

                // Wait for an incoming request
                HttpListenerContext context = await httpListener.GetContextAsync();

                if (context.Request.IsWebSocketRequest)
                {
                    HttpListenerWebSocketContext wsContext = await context.AcceptWebSocketAsync(null);
                    Console.WriteLine("Client connected.");
                    _ = HandleWebSocketConnection(wsContext.WebSocket);
                }
                else
                {
                    await HTTPInbound(context, DBCon);
                }
            }
        }




        private static async Task HTTPInbound(HttpListenerContext inboundMessage, DataBase DBCon)
        {
            string httpMethod = inboundMessage.Request.HttpMethod.ToString();
            string httpURL = inboundMessage.Request.Url.ToString();

            string requestBody;
            
            using (StreamReader reader = new StreamReader(inboundMessage.Request.InputStream, inboundMessage.Request.ContentEncoding))
            {
                requestBody = await reader.ReadToEndAsync();
            }
            Console.Write(httpURL);
            if (inboundMessage.Request.ContentType == "application/x-www-form-urlencoded")
            {
                if (httpMethod == "PUT" && httpURL == url + "Register")
                {
                    await HTTPMethods.RegisterAccount(requestBody, DBCon, inboundMessage);

                } else if (httpMethod == "POST" && httpURL == url + "SignIn")
                {
                    await HTTPMethods.SignIn(requestBody, DBCon, inboundMessage);

                }
                else if (httpMethod == "GET" && httpURL == url + "AddFriend")
                {
                    await HTTPMethods.SignIn(requestBody, DBCon, inboundMessage);

                }

            }
            else
            {
                if (httpMethod == "GET" && httpURL == url + "AllUsers")
                {
                    Debug.WriteLine("Working on returing all users");
                    await HTTPMethods.ReturnAll(requestBody, DBCon, inboundMessage);

                }
            }



            var body = new StreamReader(inboundMessage.Request.InputStream).ReadToEnd();
            Console.WriteLine(body);


        }






        private static async Task ProcessPacket(string clientMessage, Guid userId)
        {
            try
            {
                JObject clientJson = JObject.Parse(clientMessage);



                if (clientJson != null)
                {
                    Debug.WriteLine(clientMessage);
                    switch (clientJson["PacketHeader"].ToString())
                    {
                        case "SendMessage":
                            await BroadcastMessageAsync(clientMessage);
                            break;
                        case "SignIn":
                            connectedUsernames.TryAdd(userId, clientJson["username"].ToString());
                            SendUserList();
                            break;


                    }
                }
            }
            catch (Exception ex)
            {
                Console.WriteLine(ex.ToString());
            }

        }





        private static async Task HandleWebSocketConnection(WebSocket webSocket)
        {
            var userId = Guid.NewGuid();  // Generate a unique ID for each connection
            connectedUsers.TryAdd(userId, webSocket);  // Add the new connection to the dictionary
            byte[] buffer = new byte[1024 * 4];
            WebSocketReceiveResult result;

            try
            {
                while (webSocket.State == WebSocketState.Open)
                {
                    result = await webSocket.ReceiveAsync(new ArraySegment<byte>(buffer), CancellationToken.None);

                    if (result.MessageType == WebSocketMessageType.Text)
                    {
                        string clientMessage = Encoding.UTF8.GetString(buffer, 0, result.Count);
                        Console.WriteLine($"Received: {clientMessage}");


                        ProcessPacket(clientMessage, userId);

                        
                        /*
                        string serverMessage = $"Echo: {clientMessage}";
                        byte[] serverMessageBytes = Encoding.UTF8.GetBytes(clientMessage);
                        await webSocket.SendAsync(new ArraySegment<byte>(serverMessageBytes), WebSocketMessageType.Text, true, CancellationToken.None);
                        */
                    }
                    else if (result.MessageType == WebSocketMessageType.Close)
                    {
                        Console.WriteLine("Client disconnected.");
                        await webSocket.CloseAsync(WebSocketCloseStatus.NormalClosure, "Closing", CancellationToken.None);
                    }
                }
            }
            catch (Exception ex)
            {
                Console.WriteLine($"WebSocket error: {ex.Message}");
            }
            finally
            {
                connectedUsers.TryRemove(userId, out _);  // Remove the connection when done
                connectedUsernames.TryRemove(userId, out _);  // Remove the connection when done
                webSocket.Dispose();
                SendUserList();
            }
        }


        private async static void SendUserList()
        {
            try
            {
                // Select all usernames from the connectedUsernames dictionary and convert them to an array
                JArray userList = new JArray(connectedUsernames.Values.ToArray());

                UserListObject userLO = new UserListObject
                {
                    userList = userList
                };

                await BroadcastMessageAsync(JsonConvert.SerializeObject(userLO, Formatting.Indented));

                // Output the user list to the console for debugging
                Console.WriteLine("Connected Users: " + string.Join(", ", userList));

            }
            catch (Exception ex)
            {
                Console.Write(ex.ToString());

            }
        }

    
        private static async Task BroadcastMessageAsync(string message)
        {
            byte[] messageBytes = Encoding.UTF8.GetBytes(message);
            var tasks = new List<Task>();

            foreach (var user in connectedUsers.Values)
            {
                if (user.State == WebSocketState.Open)
                {
                    tasks.Add(user.SendAsync(new ArraySegment<byte>(messageBytes), WebSocketMessageType.Text, true, CancellationToken.None));
                }
            }

            try
            {
                await Task.WhenAll(tasks);
            }
            catch (Exception ex)
            {
                Console.WriteLine($"Error broadcasting message: {ex.Message}");
            }
        }
    }
}
