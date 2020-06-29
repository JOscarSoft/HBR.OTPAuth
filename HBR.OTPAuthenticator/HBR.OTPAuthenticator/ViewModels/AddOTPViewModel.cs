using GalaSoft.MvvmLight.Command;
using HBR.OTPAuthenticator.BLL.Models;
using HBR.OTPAuthenticator.BLL.Services;
using HBR.OTPAuthenticator.Resources;
using HBR.OTPAuthenticator.Views;
using Plugin.Permissions;
using Plugin.Permissions.Abstractions;
using System;
using System.Collections.Generic;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Input;
using Xamarin.Forms;
using ZXing.Net.Mobile.Forms;

namespace HBR.OTPAuthenticator.ViewModels
{
    public class AddOTPViewModel
    {
        public string Name { get; set; }
        public string SecretKey { get; set; }
        public bool TimeBased { get; set; }
        public string Issuer { get; set; }
        private readonly OTPStorageService storageService;

        public ICommand AddOtpCommand => new RelayCommand(AddOtp);

        public AddOTPViewModel()
        {
            storageService = new OTPStorageService();
        }

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


            try
            {
                var otp = new OTPGenerator()
                {
                    Label = Name,
                    SecretBase32 = SecretKey,
                    Issuer = Issuer,
                    TimeBased = TimeBased
                };

                await storageService.InsertOrReplaceAsync(otp);
                MainViewModel.GetInstance().OTPListModel = new OTPListViewModel();

                await Application.Current.MainPage.DisplayAlert(StringResources.Confirm, StringResources.AddedSucccess, StringResources.Ok);
                
                App.Current.MainPage = new MasterPage();
            }
            catch
            {
                await Application.Current.MainPage.DisplayAlert(StringResources.Error, StringResources.AddedFailed, StringResources.Ok);
            }
        }

        public async void ScanQRCode()
        {
            try
            {
                if (await PermissionGranted())
                {
                    var scan = new ZXingScannerPage();

                    await App.Navigator.PushAsync(scan);
                    scan.OnScanResult += (result) =>
                    {
                        Device.BeginInvokeOnMainThread(async () =>
                        {
                            await App.Navigator.PopAsync();

                            var otp = OTPGenerator.FromString(result.Text);
                            if (otp == null)
                            {
                                await Application.Current.MainPage.DisplayAlert(StringResources.Error, StringResources.InvalidQR, StringResources.Ok);
                            }
                            else
                            {
                                await storageService.InsertOrReplaceAsync(otp);
                                MainViewModel.GetInstance().OTPListModel = new OTPListViewModel();

                                await Application.Current.MainPage.DisplayAlert(StringResources.Confirm, StringResources.AddedSucccess, StringResources.Ok);

                                App.Current.MainPage = new MasterPage();
                            }
                        });
                    };
                }
            }
            catch
            {
                await Application.Current.MainPage.DisplayAlert(StringResources.Error, StringResources.ScanQRError, StringResources.Ok);
            }
        }

        private async Task<bool> PermissionGranted()
        {
            var permissionStatus = await CrossPermissions.Current.CheckPermissionStatusAsync(Permission.Camera);
            if (permissionStatus != PermissionStatus.Granted)
            {
                var userResponse = await CrossPermissions.Current.RequestPermissionsAsync(Permission.Camera);
                return userResponse[Permission.Camera] == PermissionStatus.Granted;
            }
            else
                return true;
        }
    }
}
