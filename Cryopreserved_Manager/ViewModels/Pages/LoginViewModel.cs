using CommunityToolkit.Mvvm.Messaging;
using Cryopreserved_Manager.Models;
using Cryopreserved_Manager.Services;
using Cryopreserved_Manager.Views.Pages;
using System;
using System.Collections.Generic;
using System.Data.Entity.Core.Metadata.Edm;
using System.Linq;
using System.Net;
using System.Text;
using System.Threading.Tasks;
using Wpf.Ui;
using Wpf.Ui.Abstractions.Controls;
using Wpf.Ui.Controls;
using static System.Runtime.InteropServices.JavaScript.JSType;
using MessageBox = System.Windows.MessageBox;

namespace Cryopreserved_Manager.ViewModels.Pages
{
    public partial class LoginViewModel(INavigationService navigationService, IUserManagementService userManagementService, 
                                        ISnackbarService snackbarService) : ObservableObject, INavigationAware
    {
        private bool _isInitialized = false;
        [ObservableProperty]
        private string userId = "";
        [ObservableProperty]
        private string password = "";

        private ControlAppearance _snackbarAppearance = ControlAppearance.Secondary;
        private int _snackbarTimeout = 2;

        public Task OnNavigatedToAsync()
        {
            if (!_isInitialized)
                InitializeViewModel();
            return Task.CompletedTask;
        }
        public Task OnNavigatedFromAsync() => Task.CompletedTask;
        private void InitializeViewModel()
        {
            _ = navigationService.Navigate(typeof(HomePage)); // 로그인 성공
            var message = new TransferMenuVisibility { isVisible = true };
            WeakReferenceMessenger.Default.Send(message);

            _isInitialized = true;
        }

        private void UserLogin()
        {
            if (UserId == "")
            {
                MessageBox.Show("Please enter the User ID.");
                return;
            }

            string pw = Password;
            if (new NetworkCredential(string.Empty, pw).Password == "")
            {
                MessageBox.Show("Please enter the password.");
                return;
            }

            if (!userManagementService.UserLogin(UserId, new NetworkCredential(string.Empty, pw).Password))
            {
                int ret = userManagementService.GetPWRetry(UserId);
                if (ret == -1)
                {
                    MessageBox.Show("Please check your ID and password.");
                    return;
                }

                else if (ret < 5)
                {
                    if (UserId == "Admin")
                    {
                        MessageBox.Show("Please check your ID and password.");
                        return;
                    }

                    userManagementService.ModifyDBPWRetry(UserId, "Active", (ret + 1).ToString());
                    userManagementService.LoadAllUsers();
                    MessageBox.Show("Please check your ID and password.");
                    return;
                }
                else
                {
                    userManagementService.ModifyDBPWRetry(UserId, "Lock", ret.ToString());
                    userManagementService.LoadAllUsers();
                    MessageBox.Show("Your account has been locked. Please contact the administrator.");
                    return;
                }
            }

            else
            {
                userManagementService.ModifyDBPWRetry(UserId, "Active", "0");
                userManagementService.LoadAllUsers();

                string status = userManagementService.GetUserStatus(UserId);
                if (status == "InActive")
                {
                    MessageBox.Show("Your account has been disabled. Please contact the administrator.");
                    return;
                }

                else if (status == "Lock")
                {
                    MessageBox.Show("Your account has been locked. Please contact the administrator.");
                    return;
                }

                // 비밀번호 변경 필요
                if (userManagementService.IsNeedChangePassword(UserId))
                {
                    MessageBox.Show("The Password change cycle has passed. Please change your password.");
                }
            }

            _ = navigationService.Navigate(typeof(HomePage)); // 로그인 성공
            var message = new TransferMenuVisibility { isVisible = true };
            WeakReferenceMessenger.Default.Send(message);
        }

        [RelayCommand]
        private void Login()
        {
            UserLogin();
        }
    }
}
