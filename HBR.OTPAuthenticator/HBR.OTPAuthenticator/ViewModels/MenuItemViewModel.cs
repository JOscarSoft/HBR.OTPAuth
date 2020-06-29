using GalaSoft.MvvmLight.Command;
using HBR.OTPAuthenticator.Helper;
using HBR.OTPAuthenticator.Views;
using Rg.Plugins.Popup.Services;
using System;
using System.Collections.Generic;
using System.Text;
using System.Windows.Input;

namespace HBR.OTPAuthenticator.ViewModels
{
    public class MenuItemViewModel : Menu
    {
        public ICommand SelectMenuCommand => new RelayCommand(SelectMenu);
        public ICommand SelectMethodCommand => new RelayCommand(SelectMethod);

        private async void SelectMethod()
        {
            await PopupNavigation.Instance.PopAllAsync();
            MainViewModel.GetInstance().AddOTPModel = new AddOTPViewModel();

            switch (this.PageName)
            {
                case "ScanCode":
                    MainViewModel.GetInstance().AddOTPModel.ScanQRCode();
                    break;
                case "ManualCode":
                    await App.Navigator.PushAsync(new AddOTP());
                    break;
            }
        }

        private async void SelectMenu()
        {
            App.Master.IsPresented = false;

            switch (this.PageName)
            {
                case "AboutPage":
                    await App.Navigator.PushAsync(new AboutPage());
                    break;
                case "SetupPage":
                    await App.Navigator.PushAsync(new SetupPage());
                    break;
            }
        }
    }
}
