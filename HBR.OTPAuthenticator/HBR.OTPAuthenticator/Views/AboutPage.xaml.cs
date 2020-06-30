using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Xamarin.Essentials;
using Xamarin.Forms;
using Xamarin.Forms.Xaml;

namespace HBR.OTPAuthenticator.Views
{
    [XamlCompilation(XamlCompilationOptions.Compile)]
    public partial class AboutPage : ContentPage
    {
        public AboutPage()
        {
            InitializeComponent();
        }

        private void SuportLabel_Tapped(object sender, EventArgs e)
        {
            Launcher.OpenAsync(OTPAuthenticator.Resources.StringResources.AboutSupportURL);
        }
    }
}