using System;
using Xamarin.Forms;
using Xamarin.Forms.Xaml;
using RainTracker2.Services;
using RainTracker2.Views;

namespace RainTracker2
{
    public partial class App : Application
    {

        public App()
        {
            InitializeComponent();

            DependencyService.Register<MockDataStore>();
            MainPage = new MainPage();
        }

        protected override void OnStart()
        {
        }

        protected override void OnSleep()
        {
        }

        protected override void OnResume()
        {
        }
    }
}
