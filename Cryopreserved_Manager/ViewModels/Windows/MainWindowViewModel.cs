using CommunityToolkit.Mvvm.Messaging;
using Cryopreserved_Manager.Models;
using Cryopreserved_Manager.Services;
using Cryopreserved_Manager.ViewModels.Pages;
using Cryopreserved_Manager.Views.Pages;
using System.Collections.ObjectModel;
using System.Text;
using System.Windows.Documents;
using System.Windows.Navigation;
using Wpf.Ui;
using Wpf.Ui.Controls;

namespace Cryopreserved_Manager.ViewModels.Windows
{
    public partial class MainWindowViewModel : ObservableObject
    {
        private readonly INavigationService _navigationService;
        private readonly IUserManagementService _userManagementService;

        private bool bSettingLoadSuccess_ = false;
        private bool bUserLoadSuccess_ = false;
        private bool bAuditTrailLoadSuccess_ = false;

        [ObservableProperty]
        private bool isMenuVisible = false;

        public MainWindowViewModel(INavigationService navigationService, IUserManagementService userManagementService)
        {
            _navigationService = navigationService;
            _userManagementService = userManagementService;

            Initialize();
        }

        public void Initialize()
        {
            WeakReferenceMessenger.Default.Register<TransferMenuVisibility>(this, (r, m) =>
            {
                if (m.isVisible)
                    IsMenuVisible = true;
                else
                    IsMenuVisible = false;
            });


            LoadingUser();
            //LoadingNetworkSetting();
        }

        public void LoadingUser()
        {
            if (_userManagementService.LoadAllUsers())
                bUserLoadSuccess_ = true;
            else
                bUserLoadSuccess_ = false;
        }

        [ObservableProperty]
        private string _applicationTitle = "Cryopreserved Cells Manage App";


        [ObservableProperty]
        private ObservableCollection<MenuItem> _trayMenuItems = new()
        {
            new MenuItem { Header = "Home", Tag = "tray_home", FontSize = 25 }
        };

        [RelayCommand]
        private void OnNavigateForward(Type type)
        {
            if (type == typeof(LoginPage))
            {
                var message = new TransferMenuVisibility { isVisible = false };
                WeakReferenceMessenger.Default.Send(message);
            }

            _ = _navigationService.NavigateWithHierarchy(type);
        }
    }
}
