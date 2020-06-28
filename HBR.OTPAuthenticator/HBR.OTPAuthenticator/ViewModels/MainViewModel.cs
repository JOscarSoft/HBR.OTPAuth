using GalaSoft.MvvmLight.Command;
using HBR.OTPAuthenticator.Views;
using System;
using System.Collections.Generic;
using System.Text;
using System.Windows.Input;

namespace HBR.OTPAuthenticator.ViewModels
{
    public class MainViewModel : BaseViewModel
    {
        private static MainViewModel instance;
        public AddOTPViewModel AddOTPModel { get; set; }
        public OTPListViewModel OTPListModel { get; set; }
        public ICommand AddOTPCommand => new RelayCommand(GoAddOTP);

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
