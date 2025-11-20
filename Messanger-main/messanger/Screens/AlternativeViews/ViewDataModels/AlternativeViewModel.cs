using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.ComponentModel;
using System.Diagnostics;
using System.Runtime.Intrinsics.X86;
using System.Runtime.CompilerServices;
using System.Windows.Input;
using messanger.Scripts.NetworkCommunications;

namespace messanger.Screens.AlternativeViews.ViewDataModels
{
    public class AlternativeViewModel : INotifyPropertyChanged
    {
        public ICommand ShowFriendsPage { get; }
        public ICommand ShowDirectMessagingPage { get; }

        private bool _isFriendsVisible = true;
        public bool IsFriendsVisible
        {
            get => _isFriendsVisible;
            set
            {
                _isFriendsVisible = value;
                OnPropertyChanged();
            }
        }

        private bool _isDmsVisible;
        public bool IsDmsVisible
        {
            get => _isDmsVisible;
            set
            {
                _isDmsVisible = value;
                OnPropertyChanged();
            }
        }

        public AlternativeViewModel()
        {
            ShowFriendsPage = new Command(() =>
            {
                //RequestAllUsers();
                IsFriendsVisible = true;
                IsDmsVisible = false;
            });

            ShowDirectMessagingPage = new Command(() =>
            {
                IsFriendsVisible = false;
                IsDmsVisible = true;
            });
        }

        public event PropertyChangedEventHandler PropertyChanged;

        protected void OnPropertyChanged([CallerMemberName] string propertyName = null)
        {
            PropertyChanged?.Invoke(this, new PropertyChangedEventArgs(propertyName));
        }
    }
}
