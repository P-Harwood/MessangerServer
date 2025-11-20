using messanger.Screens.AlternativeViews;
using System.ComponentModel;
using System.Diagnostics;
using System.Runtime.CompilerServices;
using messanger.Screens.AlternativeViews.ViewDataModels;
using System.Runtime.Intrinsics.X86;

namespace messanger.Screens;

public partial class AlternativeView : ContentView
{

    public AlternativeView()
    {
        InitializeComponent();

        // Set BindingContext to this class for property binding
        BindingContext = new AlternativeViewModel();

        // Optionally toggle between views dynamically
    }

    
    public void message(object sender, EventArgs e)
    {
        Debug.WriteLine("Clicked friends thingy magig");
    }
}
