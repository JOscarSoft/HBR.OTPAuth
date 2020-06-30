using GalaSoft.MvvmLight.Command;
using HBR.OTPAuthenticator.Views;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Windows.Input;
using Rg.Plugins.Popup.Services;
using System.Collections.ObjectModel;
using HBR.OTPAuthenticator.Helper;
using HBR.OTPAuthenticator.Resources;

namespace HBR.OTPAuthenticator.ViewModels
{
    public class MainViewModel : BaseViewModel
    {
        private static MainViewModel instance;
        public ObservableCollection<MenuItemViewModel> Menus { get; set; }
        public ObservableCollection<MenuItemViewModel> MethodMenu { get; set; }
        public AddOTPViewModel AddOTPModel { get; set; }
        public SetupViewModel SetupModel { get; set; }
        public OTPListViewModel OTPListModel { get; set; }
        public EditOTPViewModel EditOTPModel { get; set; }
        public ICommand AddOTPCommand => new RelayCommand(GoAddOTP); 
        public ICommand ShowEditCommand => new RelayCommand(ShowEdit);
        public ICommand DeleteCommand => new RelayCommand(DeleteOTP);

        private bool onEditing;

        public bool OnEditing
        {
            get { return this.onEditing; }
            set { this.SetValue(ref this.onEditing, value); }
        }
        public MainViewModel()
        {
            instance = this;
            this.LoadMenus(); 
        }

        private void LoadMenus()
        {
            var menus = new List<Menu>
            {
                new Menu
                {
                    Icon = "ic_action_security",
                    Tittle = StringResources.MenuSetup,
                    PageName = "SetupPage"
                },
                new Menu
                {
                    Icon = "ic_action_about",
                    Tittle = StringResources.MenuAbout,
                    PageName = "AboutPage"
                }
            };

            
            this.Menus = new ObservableCollection<MenuItemViewModel>(menus.Select(m => new MenuItemViewModel()
            {
                Icon = m.Icon,
                Tittle = m.Tittle,
                PageName = m.PageName
            }).ToList());
        }

        private async void ShowEdit()
        {
            var OtpGenerator = OTPListModel.OTPList.FirstOrDefault(p => p.IsSelected).Generator;
            EditOTPModel = new EditOTPViewModel(OtpGenerator);
            await PopupNavigation.Instance.PushAsync(new EditOTPModal());
        }
        private void DeleteOTP()
        {
            var OtpGenerator = OTPListModel.OTPList.FirstOrDefault(p => p.IsSelected).Generator;
            EditOTPModel = new EditOTPViewModel(OtpGenerator);
            EditOTPModel.DeleteOtpGenerator();
        }

        private async void GoAddOTP()
        {
            var menus = new List<Menu>
            {
                new Menu
                {
                    Tittle = StringResources.MenuScanCode,
                    PageName = "ScanCode"
                },
                new Menu
                {
                    Tittle = StringResources.MenuManualCode,
                    PageName = "ManualCode"
                }
            };

            this.MethodMenu = new ObservableCollection<MenuItemViewModel>(menus.Select(m => new MenuItemViewModel()
            {
                Icon = m.Icon,
                Tittle = m.Tittle,
                PageName = m.PageName
            }).ToList());

            await PopupNavigation.Instance.PushAsync(new SelectMethodModal());
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
