namespace messanger
{
    public partial class App : Application
    {
        public App()
        {
            InitializeComponent();

            MainPage = new AppShell();
        }

        protected override Window CreateWindow(IActivationState activationState)
        {
            var window = base.CreateWindow(activationState);

            // Set minimum window size (desktop only)
            window.MinimumWidth = 940;
            window.MinimumHeight = 512;

            return window;
        }




    }
}
