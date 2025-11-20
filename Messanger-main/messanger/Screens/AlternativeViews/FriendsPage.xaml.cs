using messanger.Screens.AlternativeViews.ViewDataModels;
using System.Diagnostics;

namespace messanger.Screens.AlternativeViews;

public partial class FriendsPage : ContentView
{
    public FriendsPage()
    {
        InitializeComponent();
        PrepareBindingContext();
    }

    private async void PrepareBindingContext()
    {
        // This function does an async request, when friend page is initalised it will do a http request
        BindingContext = await FriendPageDataModel.InitilialiseWithData();
    }

    public void SendFriendRequestButton(object sender, EventArgs e)
    {
        Debug.WriteLine("send friend request");
    }
}