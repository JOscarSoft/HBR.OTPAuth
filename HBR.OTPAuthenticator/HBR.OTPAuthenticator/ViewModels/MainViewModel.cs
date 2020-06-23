using System;
using System.Collections.Generic;
using System.Text;

namespace HBR.OTPAuthenticator.ViewModels
{
    public class MainViewModel
    {
        private static MainViewModel instance;
        public AddOTPViewModel AddOTPModel { get; set; }
        public OTPListViewModel OTPListModel { get; set; }
        
        public MainViewModel()
        {
            instance = this;
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
