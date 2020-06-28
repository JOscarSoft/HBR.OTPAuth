using GalaSoft.MvvmLight.Command;
using HBR.OTPAuthenticator.Views;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Windows.Input;
using Rg.Plugins.Popup.Services;

namespace HBR.OTPAuthenticator.ViewModels
{
    public class MainViewModel : BaseViewModel
    {
        private static MainViewModel instance;
        public AddOTPViewModel AddOTPModel { get; set; }
        public OTPListViewModel OTPListModel { get; set; }
        public EditOTPViewModel EditOTPModel { get; set; }
        public ICommand AddOTPCommand => new RelayCommand(GoAddOTP); 
        public ICommand ShowEditCommand => new RelayCommand(ShowEdit);

        private bool onEditing;

        public bool OnEditing
        {
            get { return this.onEditing; }
            set { this.SetValue(ref this.onEditing, value); }
        }
        public MainViewModel()
        {
            instance = this;
        }

        private async void ShowEdit()
        {
            var OtpGenerator = OTPListModel.OTPList.FirstOrDefault(p => p.IsSelected).Generator;
            this.EditOTPModel = new EditOTPViewModel(OtpGenerator);
            await PopupNavigation.Instance.PushAsync(new EditOTPModal());
        }

        private async void GoAddOTP()
        {
            this.AddOTPModel = new AddOTPViewModel();
            await App.Navigator.PushAsync(new AddOTP());
        }

        public static MainViewModel GetInstance()
        {
            if(instance == null)
            {
                return new MainViewModel();
            }

            return instance;
        }
    }
}
