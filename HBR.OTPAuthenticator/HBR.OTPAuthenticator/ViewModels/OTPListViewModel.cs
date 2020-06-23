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
        private StorageService storageService { get; set; }
        private ObservableCollection<OTPItemViewModel> oTPList;
        private static readonly TimeSpan BackgroudRefreshInterval = TimeSpan.FromMilliseconds(50);
        private DateTime NextUpdateTime { get; set; } = DateTime.UtcNow;
        private Timer BackgroundRefreshTimer;
        public ObservableCollection<OTPItemViewModel> OTPList 
        {
            get { return this.oTPList; }
            set { this.SetValue(ref this.oTPList, value); }
        }

        public OTPListViewModel()
        {
            storageService = new StorageService();
            this.LoadOTPList();
        }

        private async void LoadOTPList()
        {
            var response = await storageService.GetAllAsync();
            var listViewModel = response.Select(p => new OTPItemViewModel(p));
            this.OTPList = new ObservableCollection<OTPItemViewModel>(listViewModel);
            if (BackgroundRefreshTimer == null)
            {
                BackgroundRefreshTimer = new Timer(d => Device.BeginInvokeOnMainThread(UIRefresh), null, BackgroudRefreshInterval, BackgroudRefreshInterval);
            }
        }

        private void UIRefresh()
        {
            var currentTime = DateTime.UtcNow;

            if (currentTime.CompareTo(NextUpdateTime) >= 0)
            {
                foreach (var i in OTPList)
                    i.UpdateOTP(currentTime);

                NextUpdateTime = new DateTime(currentTime.AddSeconds(OTPGenerator.TimeStepSeconds).Ticks % (TimeSpan.TicksPerSecond * OTPGenerator.TimeStepSeconds));
            }
        }
    }
}
