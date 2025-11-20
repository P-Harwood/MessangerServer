using messanger.DataObjects.WebSocketPackets;
using messanger.Scripts.NetworkCommunications;
using System.Diagnostics;
using System.Net.WebSockets;




namespace messanger
{
    public partial class MainPage : ContentPage
    {
        private string username;
        private string password;

        private ClientWebSocket webSocket;
        private readonly WebSockTraffic _socket;
        private readonly RestfulRequests HttpTrafficObj = new RestfulRequests();

        // Comment Test

        public MainPage()
        {
            InitializeComponent();
            webSocket = new ClientWebSocket();
            _socket = new WebSockTraffic(webSocket);
            

        }

        private async void InitializeWebSocketConnection()
        {
            try
            {
                // Connect to the WebSocket server
                await webSocket.ConnectAsync(new Uri("ws://127.0.0.1:5000/ws/"), CancellationToken.None);
                Debug.WriteLine("WebSocket connection established.");

                _socket.StartListening();  //# TODO: This line was moved to WebsockTraffic. Delete if everything starts working
            }
            catch (Exception ex)
            {
                DisplayAlert("Internet Connection Error", "Websocket Failed to connect", ex.Message);
            }
        }



        private async void SignInButtonPressed(object sender, EventArgs e)
        {
            try
            {
                // Send Signin restful request
                var signInRequest = await RestfulRequests.SignInHTTP(new SignInObject { username = NameLoginBox.Text, password = PasswordBox.Text });

                // if sign in was good
                if (signInRequest.IsSuccess)
                {
                    InitializeWebSocketConnection();
                    await _socket.WSOut.SendSignInAsync(NameLoginBox.Text, PasswordBox.Text);
                    await Shell.Current.GoToAsync("mainApplication");
                }

                //await _socket.WSOut.SendSignInAsync(username, password);
            }
            catch (Exception ex)
            {
                Console.WriteLine($"{ex.Message}");
            }
            
        }

        private void RegisterButtonPressed(object sender, EventArgs e)
        {
            DisplayAlert("Register", PasswordBox.Text, "OK");
        }

        private void UsernameBoxChanged(object sender, TextChangedEventArgs e)
        {

        }
    }

}
