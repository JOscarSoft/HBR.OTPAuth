using HBR.OTPAuthenticator.Views;
using System;
using Xamarin.Forms;
using Xamarin.Forms.Xaml;

namespace HBR.OTPAuthenticator
{
    public partial class App : Application
    {
        public App()
        {
            InitializeComponent();

            MainPage = new NavigationPage(new AddOTP());
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
