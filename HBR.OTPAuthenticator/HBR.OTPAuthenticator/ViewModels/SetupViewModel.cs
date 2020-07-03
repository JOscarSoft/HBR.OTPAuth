﻿using GalaSoft.MvvmLight.Command;
using HBR.OTPAuthenticator.BLL.Models;
using HBR.OTPAuthenticator.BLL.Services;
using HBR.OTPAuthenticator.Resources;
using HBR.OTPAuthenticator.Views;
using Rg.Plugins.Popup.Services;
using System;
using System.Collections.Generic;
using System.Text;
using System.Threading.Tasks;
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
        private bool editingPassword;
        private string actualPassword;
        private string newPassword;
        private string confirmPassword;

        private bool manualSwitch;
        public ICommand ButtonEditCommand => new RelayCommand(ButtonEdit);
        public ICommand UseBiometricCommand => new RelayCommand(UseBiometric);
        public ICommand ChangePasswordCommand => new RelayCommand(ChangePassword);

        public bool LoginEnabled
        {
            get { 
                return this.loginEnabled; 
            }
            set {
                if (manualSwitch)
                {
                    this.SetValue(ref this.loginEnabled, !value);
                    SetLoginEnabled();
                }
                else
                {
                    this.SetValue(ref this.loginEnabled, value);
                    manualSwitch = true;
                }
            }
        }

        public bool EditingPassword
        {
            get { return this.editingPassword; }
            set { this.SetValue(ref this.editingPassword, value); }
        }

        public bool UseBiometricAuth
        {
            get { return this.useBiometricAuth; }
            set { this.SetValue(ref this.useBiometricAuth, value); }
        }

        public string ActualPassword
        {
            get { return this.actualPassword; }
            set { this.SetValue(ref this.actualPassword, value); }
        }
        public string NewPassword
        {
            get { return this.newPassword; }
            set { this.SetValue(ref this.newPassword, value); }
        }
        public string ConfirmPassword
        {
            get { return this.confirmPassword; }
            set { this.SetValue(ref this.confirmPassword, value); }
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
            else
                manualSwitch = true;
        }

        private async void ButtonEdit()
        {
            EditingPassword = true;
            await PopupNavigation.Instance.PushAsync(new AddEditPasswordModal());
        }

        public async void SetLoginEnabled()
        {
            if (loginInformation != null)
            {
                var accepted = await Application.Current.MainPage.DisplayAlert(StringResources.Confirm, StringResources.DisableLogin, StringResources.Ok, StringResources.Cancel);
                if (accepted)
                {
                    await loginService.DeleteAsync(loginInformation);
                    loginInformation = null;
                    manualSwitch = false;
                    LoginEnabled = false;
                }
            }
            else
            {
                EditingPassword = false;
                await PopupNavigation.Instance.PushAsync(new AddEditPasswordModal());
            }
        }

        private async void ChangePassword()
        {
            var errors = validatePasswordField();
            if (errors == null)
            {
                if (EditingPassword)
                {
                    loginInformation.SecretCode = NewPassword;
                    await loginService.InsertOrReplaceAsync(loginInformation);
                    await Application.Current.MainPage.DisplayAlert(StringResources.Confirm, StringResources.PasswordChangeSucces, StringResources.Ok);
                }
                else
                {
                    loginInformation = new Login()
                    {
                        SecretCode = NewPassword,
                        UseBiometricAuth = false
                    };

                    await loginService.InsertOrReplaceAsync(loginInformation);
                    manualSwitch = false;
                    LoginEnabled = true;
                }

                await PopupNavigation.Instance.PopAllAsync();

                ActualPassword = NewPassword = ConfirmPassword = string.Empty;
            }
            else
                await Application.Current.MainPage.DisplayAlert(StringResources.Error, errors, StringResources.Ok);
        }

        private string validatePasswordField()
        {
            if (EditingPassword && string.IsNullOrEmpty(ActualPassword))
                return StringResources.PasswordEmptyActual;

            if (string.IsNullOrEmpty(NewPassword))
                return StringResources.PasswordEmptyNew;

            if (NewPassword.Length < 4)
                return StringResources.PasswordLength;

            if (NewPassword != ConfirmPassword)
                return StringResources.PasswordErrorConfirm;

            if (EditingPassword && loginInformation.SecretCode != ActualPassword)
                return StringResources.PasswordNotActual;

            return null;
        }

        private void UseBiometric()
        {
        }
    }
}
