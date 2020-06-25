using GalaSoft.MvvmLight.Command;
using HBR.OTPAuthenticator.BLL.Models;
using HBR.OTPAuthenticator.BLL.Services;
using HBR.OTPAuthenticator.Resources;
using HBR.OTPAuthenticator.Views;
using System;
using System.Collections.Generic;
using System.Text;
using System.Windows.Input;
using Xamarin.Forms;

namespace HBR.OTPAuthenticator.ViewModels
{
    public class AddOTPViewModel
    {
        public string Name { get; set; }
        public string SecretKey { get; set; }
        public bool TimeBased { get; set; }
        public string Issuer { get; set; }

        public ICommand AddOtpCommand => new RelayCommand(AddOtp);

        private async void AddOtp()
        {
            if (string.IsNullOrEmpty(Name))
            {
                await Application.Current.MainPage.DisplayAlert(StringResources.Error, StringResources.EmptyLabel, StringResources.Ok);
                return;
            }
            if (string.IsNullOrEmpty(SecretKey))
            {
                await Application.Current.MainPage.DisplayAlert(StringResources.Error, StringResources.EmptySecret, StringResources.Ok);
                return;
            }
            if(SecretKey.Length < 12)
            {
                await Application.Current.MainPage.DisplayAlert(StringResources.Error, StringResources.ShortSecret, StringResources.Ok);
                return;
            }

            var otp = new OTPGenerator()
            {
                Label = Name,
                SecretBase32 = SecretKey,
                Issuer = Issuer,
                TimeBased = TimeBased
            };
            var storageService = new OTPStorageService();
            await storageService.InsertOrReplaceAsync(otp);
            MainViewModel.GetInstance().OTPListModel = new OTPListViewModel();

            await Application.Current.MainPage.DisplayAlert(StringResources.Confirm, StringResources.AddedSucccess, StringResources.Ok);
            await App.Navigator.PushAsync(new OTPList());
        }
    }
}
