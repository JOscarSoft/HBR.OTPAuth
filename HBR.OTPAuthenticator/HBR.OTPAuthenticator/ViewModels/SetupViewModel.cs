using GalaSoft.MvvmLight.Command;
using HBR.OTPAuthenticator.BLL.Models;
using HBR.OTPAuthenticator.BLL.Services;
using HBR.OTPAuthenticator.Resources;
using System;
using System.Collections.Generic;
using System.Text;
using System.Windows.Input;
using Xamarin.Forms;

namespace HBR.OTPAuthenticator.ViewModels
{
    public class SetupViewModel : BaseViewModel
    {
        private readonly LoginService loginService;
        private Login loginInformation { get; set; }

        private bool loginEnabled;
        private bool useBiometricAuth;
        
        public ICommand EditSecretCommand => new RelayCommand(EditSecret);
        public ICommand LoginEnabledCommand => new RelayCommand(SetLoginEnabled);
        public ICommand UseBiometricCommand => new RelayCommand(UseBiometric);

        public bool LoginEnabled
        {
            get { return this.loginEnabled; }
            set { this.SetValue(ref this.loginEnabled, value); }
        }

        public bool UseBiometricAuth
        {
            get { return this.useBiometricAuth; }
            set { this.SetValue(ref this.useBiometricAuth, value); }
        }
        
        public SetupViewModel()
        {
            loginService = new LoginService();
            LoadLoginInformation();
        }

        private async void LoadLoginInformation()
        {
            loginInformation = await loginService.GetLoginInformationAsync();

            if (loginInformation != null)
            {
                LoginEnabled = true;
                UseBiometricAuth = loginInformation.UseBiometricAuth;
            }
        }

        private void EditSecret()
        {
        }

        private async void SetLoginEnabled()
        {
            if(loginInformation != null)
            {
                var accepted = await Application.Current.MainPage.DisplayAlert(StringResources.Confirm, StringResources.DisableLogin, StringResources.Ok, StringResources.Cancel);
                if (accepted)
                {
                    await loginService.DeleteAsync(loginInformation);
                    LoginEnabled = false;
                }
            }
        }

        private void UseBiometric()
        {
        }
    }
}
