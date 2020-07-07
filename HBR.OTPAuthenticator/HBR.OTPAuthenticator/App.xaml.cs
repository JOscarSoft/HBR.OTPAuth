using HBR.OTPAuthenticator.ViewModels;
using HBR.OTPAuthenticator.Views;
using System;
using Xamarin.Forms;
using Xamarin.Forms.Xaml;

namespace HBR.OTPAuthenticator
{
    public partial class App : Application
    {
        public static NavigationPage Navigator { get; internal set; }
        public static MasterPage Master { get; internal set; }

        public App()
        {
            InitializeComponent();

            MainViewModel.GetInstance().LoginModel = new LoginViewModel();
            this.MainPage = new LoginPage();
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
