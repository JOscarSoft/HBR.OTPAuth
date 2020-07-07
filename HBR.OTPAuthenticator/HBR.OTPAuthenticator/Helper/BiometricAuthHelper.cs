using HBR.OTPAuthenticator.Resources;
using Plugin.Fingerprint;
using Plugin.Fingerprint.Abstractions;
using System;
using System.Collections.Generic;
using System.Text;
using System.Threading.Tasks;
using Xamarin.Forms;

namespace HBR.OTPAuthenticator.Helper
{
    public class BiometricAuthHelper
    {

        public static async Task<bool> ValidateBiometrics(bool configuring)
        {
            bool allowed = false;
            var result = await CrossFingerprint.Current.IsAvailableAsync(true);

            if (result)
            {
                AuthenticationRequestConfiguration conf = new AuthenticationRequestConfiguration(StringResources.UseBiometrics, StringResources.UseBiometricsInfo);
                var auth = await CrossFingerprint.Current.AuthenticateAsync(conf);
                allowed = auth.Authenticated;
            }
            else
            {
                if(configuring)
                    await Application.Current.MainPage.DisplayAlert(StringResources.Error, StringResources.BiometricUnavailable, StringResources.Ok);
            }
            return allowed;
        }

    }
}
