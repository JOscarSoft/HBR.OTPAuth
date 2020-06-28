using GalaSoft.MvvmLight.Command;
using HBR.OTPAuthenticator.BLL.Models;
using HBR.OTPAuthenticator.BLL.Services;
using System;
using System.Collections.Generic;
using System.Text;
using System.Windows.Input;
using Rg.Plugins.Popup;
using Rg.Plugins.Popup.Services;
using HBR.OTPAuthenticator.Views;
using Xamarin.Forms;
using HBR.OTPAuthenticator.Resources;

namespace HBR.OTPAuthenticator.ViewModels
{
    public class EditOTPViewModel : BaseViewModel
    {
        public string Label { get; set; }
        public string Issuer { get; set; }
        public OTPGenerator OtpGenerator;

        public ICommand EditCommand => new RelayCommand(EditOtpGenerator);
        private async void EditOtpGenerator()
        {
            if (string.IsNullOrEmpty(Label))
            {
                await Application.Current.MainPage.DisplayAlert(StringResources.Error, StringResources.EmptyLabel, StringResources.Ok);
                return;
            }
            
            OtpGenerator.Label = Label;
            OtpGenerator.Issuer = Issuer;
            
            var storageService = new OTPStorageService();
            await storageService.InsertOrReplaceAsync(OtpGenerator);

            MainViewModel.GetInstance().OTPListModel = new OTPListViewModel();
            App.Current.MainPage = new MasterPage();
            await PopupNavigation.Instance.PopAllAsync();
        }

        public EditOTPViewModel(OTPGenerator oTPGenerator)
        {
            this.OtpGenerator = oTPGenerator;
            Label = oTPGenerator.Label;
            Issuer = oTPGenerator.Issuer;
        }
    }
}
