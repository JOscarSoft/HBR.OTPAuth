using HBR.OTPAuthenticator.BLL.Models;
using HBR.OTPAuthenticator.BLL.Services;
using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Linq;
using System.Text;
using System.Threading;
using Xamarin.Forms;

namespace HBR.OTPAuthenticator.ViewModels
{
    public class OTPListViewModel : BaseViewModel
    {
        private OTPStorageService storageService { get; set; }
        private ObservableCollection<OTPItemViewModel> oTPList;
        public ObservableCollection<OTPItemViewModel> OTPList 
        {
            get { return this.oTPList; }
            set { this.SetValue(ref this.oTPList, value); }
        }
        public OTPListViewModel()
        {
            storageService = new OTPStorageService();
            this.LoadOTPList();
        }

        private async void LoadOTPList()
        {
            var response = await storageService.GetAllAsync();
            var listViewModel = response.Select(p => new OTPItemViewModel(p));
            this.OTPList = new ObservableCollection<OTPItemViewModel>(listViewModel);
        }
    }
}
