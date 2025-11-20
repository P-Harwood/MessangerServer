using messanger.DataObjects;
using messanger.DataObjects.LocalDataPackets;
using messanger.DataObjects.WebSocketPackets;
using NLog;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Net.WebSockets;
using System.Text;
using System.Text.Json;
using System.Threading;
using System.Threading.Tasks;

namespace messanger.Scripts.NetworkCommunications
{
    internal class WebSockOut
    {
        private readonly ILogger logger = LogManager.GetCurrentClassLogger();
        private readonly ClientWebSocket _socket;

        public WebSockOut(ClientWebSocket socket, ILogger loggerObject)
        {
            _socket = socket;
            logger = loggerObject;

        }

        // fucntion for formatting the message the user wants to send, creates a json string
        public async Task SendMessageAsync(string username, string message)
        {
            logger.Info("Sending Message: " + message, " Username: " + username);
            try
            {
                if (string.IsNullOrEmpty(message))
                {
                    logger.Error("ERROR: Error in FormatMessage. Message to send is empty or null");
                }
                else if (string.IsNullOrEmpty(username))
                {
                    logger.Error("ERROR: Error in FormatMessage. Sender Username is empty or null");
                }
                else
                {
                    MessageObject messageToSend = new MessageObject
                    {
                        Sender = username,
                        Content = message
                    };

                    string formattedMessage = JsonSerializer.Serialize(messageToSend, new JsonSerializerOptions
                    {
                        WriteIndented = true // This option enables pretty-printing
                    });

                    await SendJson(formattedMessage);
                }

            }
            catch (Exception ex)
            {
                logger.Error(ex.ToString());
            }


        }
        // Method for formatting the signin packet
        public async Task SendSignInAsync(string username, string password)
        {
            try
            {
                if (string.IsNullOrEmpty(username))
                {
                    logger.Error("ERROR: Error in FormatMessage. Username to send is empty or null");
                }
                else if (string.IsNullOrEmpty(password))
                {
                    logger.Error("ERROR: Error in FormatMessage. Password is empty or null");
                }
                else
                {
                    SignInObject messageToSend = new SignInObject
                    {
                        username = username,
                        password = password
                    };

                    string formattedMessage = JsonSerializer.Serialize(messageToSend, new JsonSerializerOptions
                    {
                        WriteIndented = true // This option enables pretty-printing
                    });

                    await SendJson(formattedMessage);
                }

            }
            catch (Exception ex)
            {
                logger.Error(ex.ToString());
            }


        }

        // Function responable for sending the json message to the server via the websocket
        private async Task SendJson(string stringJson)
        {
            try
            {
                byte[] messageBytes = Encoding.UTF8.GetBytes(stringJson);
                ArraySegment<byte> buffer = new ArraySegment<byte>(messageBytes);

                await _socket.SendAsync(buffer, WebSocketMessageType.Text, true, CancellationToken.None);
                logger.Info($"Sent Packet Successfully: {stringJson}");

            }
            catch (Exception ex)
            {
                logger.Error(ex.ToString());
            }


        }
    }
}
