using HBR.OTPAuthenticator.BLL.Models;
using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Text;
using System.Windows.Input;
using Xamarin.Forms;
using GalaSoft.MvvmLight.Command;

namespace HBR.OTPAuthenticator.ViewModels
{
    public class OTPItemViewModel : BaseViewModel
    {
        public OTPGenerator Generator { get; }
        private string otp;
        private double progress = 0;
        private bool allowRefresh;
        public string OTPInformation => $"{Generator.Label} - {Generator.Issuer}";
        public bool TimeBased => Generator.TimeBased;
        public ICommand ManualRefreshCommand => new RelayCommand(ManualRefresh);
        public string OTP
        {
            get { return this.otp; }
            set { this.SetValue(ref this.otp, value); }
        }
        public bool AllowRefresh
        {
            get { return this.allowRefresh; }
            set { this.SetValue(ref this.allowRefresh, value); }
        }
        public double Progress
        {
            get { return this.progress; }
            set { this.SetValue(ref this.progress, value); }
        }

        public OTPItemViewModel(OTPGenerator gen)
        {
            Generator = gen;

            if (TimeBased)
                UpdateOTP(DateTime.UtcNow);
            else
            {
                OTP = "--- ---";
                AllowRefresh = true;
            }
        }
        private async void ManualRefresh()
        {
            UpdateOTP(DateTime.UtcNow);
            AllowRefresh = false;
        }
        private void UpdateOTP(DateTime time)
        {
            OTP = Generator.GenerateOTP(time);
        }
        
        public void UpdateProgressTimer(DateTime time)
        {
            if (TimeBased)
            {
                double countDown = ((1000 * (time.Second % OTPGenerator.TimeStepSeconds)) + time.Millisecond) / 100000f;
                Progress = countDown * 3.34f;

                UpdateOTP(time);
            }
            else if (!AllowRefresh)
            {
                Progress = ((1000 * (time.Second % OTPGenerator.TimeStepSeconds)) + time.Millisecond) / 1000;

                if ((progress % 5) == 0)
                    AllowRefresh = true;
            }
        }
    }
}
