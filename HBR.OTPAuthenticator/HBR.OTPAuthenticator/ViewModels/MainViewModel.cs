using System;
using System.Collections.Generic;
using System.Text;

namespace HBR.OTPAuthenticator.ViewModels
{
    public class MainViewModel
    {
        public AddOTPViewModel AddOTPModel { get; set; }

        public MainViewModel()
        {
            this.AddOTPModel = new AddOTPViewModel();
        }
    }
}
