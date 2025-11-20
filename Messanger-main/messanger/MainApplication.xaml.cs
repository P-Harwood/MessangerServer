using System.Diagnostics;

namespace messanger;

public partial class MainApplication : ContentPage
{
	public MainApplication()
	{
		InitializeComponent();
	}

	public void SendFriendRequestButton(object sender, EventArgs e)
	{
        Debug.WriteLine("send friend request");
    }
}