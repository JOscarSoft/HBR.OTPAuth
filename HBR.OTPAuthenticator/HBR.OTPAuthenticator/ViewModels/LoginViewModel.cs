using GalaSoft.MvvmLight.Command;
using HBR.OTPAuthenticator.BLL.Models;
using HBR.OTPAuthenticator.BLL.Services;
using HBR.OTPAuthenticator.Helper;
using HBR.OTPAuthenticator.Resources;
using System;
using System.Collections.Generic;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Input;
using Xamarin.Forms;

namespace HBR.OTPAuthenticator.ViewModels
{
    public class LoginViewModel
    {
        public string Password { get; set; }
        public ICommand LoginCommand => new RelayCommand(PasswordLogin);

        private readonly LoginService service;
        private Login loginInformation;
        public LoginViewModel()
        {
            service = new LoginService();
        }

        public async void VerifyLoginInformation()
        {
            loginInformation = await service.GetLoginInformationAsync();
            if (loginInformation != null)
            {
                if (loginInformation.UseBiometricAuth)
                {
                    var biometricAuthSuccess = await BiometricAuthHelper.ValidateBiometrics(false);

                    if (biometricAuthSuccess)
                        GetLogin();
                }
            }
            else
                GetLogin();
        }
        private async void PasswordLogin()
        {
            if (Password == loginInformation.SecretCode)
                GetLogin();
            else
                await Application.Current.MainPage.DisplayAlert(StringResources.Error, StringResources.LoginFailed, StringResources.Ok);
        }

        private void GetLogin()
        {
            MainViewModel.GetInstance().OTPListModel = new OTPListViewModel();
            App.Current.MainPage = new Views.MasterPage();
        }
    }
}
