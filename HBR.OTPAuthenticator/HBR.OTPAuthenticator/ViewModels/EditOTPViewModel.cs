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
        private readonly OTPStorageService storageService;
        public string Label { get; set; }
        public string Issuer { get; set; }
        public OTPGenerator OtpGenerator;
        
        public ICommand EditCommand => new RelayCommand(EditOtpGenerator);

        public EditOTPViewModel(OTPGenerator oTPGenerator)
        {
            storageService = new OTPStorageService();
            OtpGenerator = oTPGenerator;
            Label = oTPGenerator.Label;
            Issuer = oTPGenerator.Issuer;
        }
        private async void EditOtpGenerator()
        {
            if (string.IsNullOrEmpty(Label))
            {
                await Application.Current.MainPage.DisplayAlert(StringResources.Error, StringResources.EmptyLabel, StringResources.Ok);
                return;
            }

            OtpGenerator.Label = Label;
            OtpGenerator.Issuer = Issuer;

            await storageService.InsertOrReplaceAsync(OtpGenerator);

            MainViewModel.GetInstance().OTPListModel = new OTPListViewModel();
            App.Current.MainPage = new MasterPage();
            await PopupNavigation.Instance.PopAllAsync();
        }

        public async void DeleteOtpGenerator()
        {
            var accepted = await Application.Current.MainPage.DisplayAlert(StringResources.Confirm, StringResources.AcceptDelete, StringResources.Ok, StringResources.Cancel);
            if (accepted)
            {
                await storageService.DeleteAsync(OtpGenerator);
                MainViewModel.GetInstance().OTPListModel = new OTPListViewModel();
                App.Current.MainPage = new MasterPage();
            }
        }
    }
}
