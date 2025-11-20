using System;
using System.Collections.Generic;
using System.Linq;
using System.Net.WebSockets;
using System.Security.Cryptography.X509Certificates;
using System.Text;
using System.Threading;
using System.Threading.Tasks;
using NLog;
using System.Text.Json;
using System.Diagnostics;
using messanger.DataObjects;
using messanger.DataObjects.LocalDataPackets;

namespace messanger.Scripts.NetworkCommunications
{

    internal class WebSockTraffic
    {

        private static readonly ILogger logger = LogManager.GetCurrentClassLogger();
        private readonly ClientWebSocket _socket;
        public WebSockOut WSOut;

        public event Action<MessageObject> OnMessageReceived;
        public event Action<ServerListObj> OnUserListReceived;


        //Constructer class, stores socket and starts listening to traffic
        public WebSockTraffic(ClientWebSocket socket)
        {
            _socket = socket;
            WSOut = new WebSockOut(socket, logger);

        }
        public void StartListening()
        {
            // Starts the listening task when the connection is active
            _ = RecieveTraffic();
        }


        private async Task RecieveTraffic()
        {
            try
            {
                var buffer = new byte[1024 * 4];
                while (_socket.State == WebSocketState.Open)
                {
                    var result = await _socket.ReceiveAsync(new ArraySegment<byte>(buffer), CancellationToken.None);
                    string message = Encoding.UTF8.GetString(buffer, 0, result.Count);
                   logger.Info($"Received: {message}");

                    string header = ParseJson.parseJsonHeader(message);


                    switch (header)
                    {
                        case "SendMessage":
                            Debug.WriteLine("Message recieved:" + message);
                            MessageObject MessObj = ParseJson.ParseMesObj(message);
                            Console.WriteLine("Message recieved");
                            OnMessageReceived?.Invoke(MessObj);
                            //_mainClass.AddMessage(JMessage);
                            break;
                        case "UserList":
                            Debug.WriteLine("UserList recieved:" + message);
                            //ServerListObj ServLiObj = JsonExtractor.ParseServerList(message); 
                            //OnUserListReceived?.Invoke(ServLiObj);
                            //_mainClass.AddServerList(JMessage);
                            break;
                        case "Alert":
                            Debug.WriteLine("Alert:" + message);
                            break;
                    }

                }
            }
            catch(Exception ex)
            {
                logger.Error(ex.ToString());
            }
            
            
        }




    }

}
