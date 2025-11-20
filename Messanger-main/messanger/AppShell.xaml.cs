namespace messanger
{
    public partial class AppShell : Shell
    {
        public AppShell()
        {
            InitializeComponent();
            Routing.RegisterRoute("mainApplication", typeof(MainApplication));
        }
    }
}
