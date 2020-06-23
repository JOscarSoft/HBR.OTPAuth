using HBR.OTPAuthenticator.BLL.Models;
using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Text;
using Xamarin.Forms;

namespace HBR.OTPAuthenticator.ViewModels
{
    public class OTPItemViewModel : BaseViewModel
    {
        public OTPGenerator Generator { get; }
        private string otp;
        private double progress = 0;

        public string OTPInformation => $"{Generator.Label} - {Generator.Issuer}";

        public string OTP
        {
            get { return this.otp; }
            set { this.SetValue(ref this.otp, value); }
        }
        public double Progress
        {
            get { return this.progress; }
            set { this.SetValue(ref this.progress, value); }
        }

        public OTPItemViewModel(OTPGenerator gen)
        {
            Generator = gen;
            UpdateOTP(DateTime.UtcNow);
        }

        public void UpdateOTP(DateTime time)
        {
            OTP = Generator.GenerateOTP(time);

            double countDown = ((1000 * (time.Second % OTPGenerator.TimeStepSeconds)) + time.Millisecond) / 100000f;
            Progress = countDown * 3.34f;
        }
    }
}
