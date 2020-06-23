using GalaSoft.MvvmLight.Command;
using HBR.OTPAuthenticator.BLL.Models;
using HBR.OTPAuthenticator.BLL.Services;
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
                await Application.Current.MainPage.DisplayAlert("Error", "El campo \"Nombre\" es obligatorio.", "Aceptar");
                return;
            }
            if (string.IsNullOrEmpty(SecretKey))
            {
                await Application.Current.MainPage.DisplayAlert("Error", "El campo \"Clave\" es obligatorio.", "Aceptar");
                return;
            }

            var otp = new OTPGenerator()
            {
                Label = Name,
                SecretBase32 = SecretKey,
                Issuer = Issuer,
                AllowExporting = TimeBased
            };
            var storageService = new StorageService();
            await storageService.InsertOrReplaceAsync(otp);
            await Application.Current.MainPage.DisplayAlert("Success", "Fuck yeah carajo!!!", "Aceptar");
        }
    }
}
