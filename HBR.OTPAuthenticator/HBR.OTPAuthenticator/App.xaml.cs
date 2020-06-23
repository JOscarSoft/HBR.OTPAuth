using HBR.OTPAuthenticator.ViewModels;
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

            MainViewModel.GetInstance().OTPListModel = new OTPListViewModel();
            this.MainPage = new NavigationPage(new OTPList());
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
