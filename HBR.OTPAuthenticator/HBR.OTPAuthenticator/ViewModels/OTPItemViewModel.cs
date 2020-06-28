using HBR.OTPAuthenticator.BLL.Models;
using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Text;
using System.Windows.Input;
using Xamarin.Forms;
using GalaSoft.MvvmLight.Command;
using HBR.OTPAuthenticator.BLL.Services;
using System.Threading;
using System.Linq;

namespace HBR.OTPAuthenticator.ViewModels
{
    public class OTPItemViewModel : BaseViewModel
    {
        private string otp;
        private double progress;
        private bool allowRefresh;
        private bool isSelected;
        private OTPGenerator Generator { get; }

        private OTPStorageService storageService { get; set; }
        private Timer RefreshTimer;
        private DateTime NextUpdateTime { get; set; } = DateTime.UtcNow;
        private bool redProgress;

        public string OTPInformation => GenerateInformation();

        public bool TimeBased => Generator.TimeBased;
        public ICommand ManualRefreshCommand => new RelayCommand(ManualRefresh);
        public ICommand SelectOTPCommand => new RelayCommand(SelectOTP);
        public ICommand ChangeSelectCommand => new RelayCommand(ChangeSelect);

        public string OTP
        {
            get { return $"{this.otp.Substring(0, 3)} {this.otp.Substring(3, 3)}"; }
            set { this.SetValue(ref this.otp, value); }
        }
        public bool AllowRefresh
        {
            get { return this.allowRefresh; }
            set { this.SetValue(ref this.allowRefresh, value); }
        }
        public bool IsSelected
        {
            get { return this.isSelected; }
            set { this.SetValue(ref this.isSelected, value); }
        }
        public bool RedProgress
        {
            get { return this.redProgress; }
            set { this.SetValue(ref this.redProgress, value); }
        }
        public double Progress
        {
            get { return this.progress; }
            set { this.SetValue(ref this.progress, value); }
        }

        private void SelectOTP()
        {
            if (MainViewModel.GetInstance().OnEditing)
                return;

            MainViewModel.GetInstance().OnEditing = true;
            this.IsSelected = true;
        }
        private void ChangeSelect()
        {
            if (MainViewModel.GetInstance().OnEditing)
            {
                if (this.IsSelected)
                {
                    MainViewModel.GetInstance().OnEditing = false;
                    this.IsSelected = false;
                    return;
                }

                MainViewModel.GetInstance().OTPListModel.OTPList
                    .FirstOrDefault(p => p.IsSelected)
                    .IsSelected = false;

                this.IsSelected = true;

            }
        }

        public OTPItemViewModel(OTPGenerator gen)
        {
            Generator = gen;
            storageService = new OTPStorageService();

            if (TimeBased)
            {
                UpdateOTP(DateTime.UtcNow);
                RefreshTimer = new Timer(d => Device.BeginInvokeOnMainThread(TimeBasedHandled), null, TimeSpan.FromMilliseconds(50), TimeSpan.FromMilliseconds(50));
            }
            else
            {
                OTP = "------";
                AllowRefresh = true;
            }
        }
        private async void ManualRefresh()
        {
            if (!TimeBased && AllowRefresh)
            {
                UpdateOTP(DateTime.UtcNow);
                this.AllowRefresh = false;

                Generator.Counter++;
                await storageService.InsertOrReplaceAsync(Generator);

                RefreshTimer = new Timer(d => Device.BeginInvokeOnMainThread(RefreshTimerHandled), null, TimeSpan.FromSeconds(5), TimeSpan.FromSeconds(0));
            }
        }
        private void UpdateOTP(DateTime time)
        {
            OTP = Generator.GenerateOTP(time);
        }

        public void UpdateProgressTimer(DateTime time)
        {
            double countDown = ((1000 * (time.Second % OTPGenerator.TimeStepSeconds)) + time.Millisecond) / 100000f;
            Progress = countDown * 3.34f;

            UpdateOTP(time);

            if (countDown > 0.2)
                RedProgress = true;
            else
                RedProgress = false;
        }
        private void RefreshTimerHandled()
        {
            AllowRefresh = true;
        }
        private void TimeBasedHandled()
        {
            var currentTime = DateTime.UtcNow;

            if (currentTime.CompareTo(NextUpdateTime) >= 0)
            {
                UpdateProgressTimer(currentTime);
                NextUpdateTime = new DateTime(currentTime.AddSeconds(OTPGenerator.TimeStepSeconds).Ticks % (TimeSpan.TicksPerSecond * OTPGenerator.TimeStepSeconds));
            }
        }

        private string GenerateInformation()
        {
            if (string.IsNullOrEmpty(Generator.Issuer))
                return Generator.Label;

            return $"{Generator.Label} - {Generator.Issuer}";
        }
    }
}
