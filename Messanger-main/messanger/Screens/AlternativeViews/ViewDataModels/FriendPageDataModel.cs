using messanger.DataObjects.Conversations;
using messanger.DataObjects.LocalDataPackets;
using messanger.Scripts.NetworkCommunications;
using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Diagnostics;
using System.Linq;
using System.Runtime.CompilerServices;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Input;

namespace messanger.Screens.AlternativeViews.ViewDataModels
{
    internal class FriendPageDataModel : INotifyPropertyChanged
    {


        public ICommand ShowOnline { get; }
        public ICommand ShowAll { get; }
        public ICommand ShowPending { get; }
        public ICommand ShowBlocked { get; }
        public ICommand ShowAddFriend { get; }
        public ICommand SendFriendRequest { get; }



        private async static Task AddFriend(string name)
        {
            try
            {
                Debug.WriteLine("Sending friend request to: "+ name);

                var request = new ConversationByName
                {
                    foriegnUserName = name,
                    localUserName = "username",
                };

                var result = await RestfulRequests.RequestNewConversationByUsername(request);

                if (result.IsSuccess)
                {
                    Debug.WriteLine("Friend request sent successfully!");
                }
                else
                {
                    Debug.WriteLine("Failed to send friend request.");
                }
            }
            catch (Exception ex)
            {
                Debug.WriteLine($"Error: {ex.Message}");
            }
        }


        private bool _isOnlineVisible = false;
        private bool _isAllVisible = false;
        private bool _isPendingVisible = false;
        private bool _isBlockedVisible = false;
        private bool _isAddFriendVisible = true;





        public bool IsOnlineVisible
        {
            get => _isOnlineVisible;
            set
            {
                _isOnlineVisible = value;
                OnPropertyChanged();
            }
        }


        public bool IsAllVisible
        {
            get => _isAllVisible;
            set
            {
                _isAllVisible = value;
                OnPropertyChanged();
            }
        }


        public bool IsPendingVisible
        {
            get => _isPendingVisible;
            set
            {
                _isPendingVisible = value;
                OnPropertyChanged();
            }
        }


        
        public bool IsBlockedVisible
        {
            get => _isBlockedVisible;
            set
            {
                
                _isBlockedVisible = value;
                OnPropertyChanged();
            }
        }



           public bool IsAddFriendVisible
        {
            get => _isAddFriendVisible;
            set
            {
                _isAddFriendVisible = value;
                OnPropertyChanged();
            }
        }




        public FriendPageDataModel()
        {


            SendFriendRequest = new Command<string>(async (name) => await AddFriend(name));
            // TODO :  Find a way to optimise this.

            ShowOnline = new Command(() =>
            {
                IsOnlineVisible = true;
                IsAllVisible = false;
                IsPendingVisible = false;
                IsBlockedVisible = false;
                IsAddFriendVisible = false;
            });

            ShowAll = new Command(() =>
            {
                IsOnlineVisible = false;
                IsAllVisible = true;
                IsPendingVisible = false;
                IsBlockedVisible = false;
                IsAddFriendVisible = false;
            });
            ShowPending = new Command(() =>
            {
                IsOnlineVisible = false;
                IsAllVisible = false;
                IsPendingVisible = true;
                IsBlockedVisible = false;
                IsAddFriendVisible = false;
            });
            ShowBlocked = new Command(() =>
            {
                IsOnlineVisible = false;
                IsAllVisible = false;
                IsPendingVisible = false;
                IsBlockedVisible = true;
                IsAddFriendVisible = false;
            });
            ShowAddFriend = new Command(() =>
            {
                IsOnlineVisible = false;
                IsAllVisible = false;
                IsPendingVisible = false;
                IsBlockedVisible = false;
                IsAddFriendVisible = true;
            });

        }



        public ServerListObj ServerList { get; private set; }


        // Function for initally creating the class, so the server list class can be fetched 
        public static async Task<FriendPageDataModel> InitilialiseWithData()
        {
            var selfDataModel = new FriendPageDataModel();
            var serverListRequest = await RestfulRequests.RequestAllUsers();

            if (serverListRequest.IsSuccess)
            {

                selfDataModel.ServerList = serverListRequest.Value;



                return selfDataModel;
            }
            else
            {
                //TODO Check and fix this;
                return null;
            }
            
        }

    


        public event PropertyChangedEventHandler PropertyChanged;

        protected void OnPropertyChanged([CallerMemberName] string propertyName = null)
        {
            PropertyChanged?.Invoke(this, new PropertyChangedEventArgs(propertyName));
        }
    }
}
