using Wpf.Ui.Appearance;
using Wpf.Ui.Abstractions.Controls;
using Cryopreserved_Manager.Services;
using System.Collections.ObjectModel;
using Cryopreserved_Manager.Models;
using System.Text.RegularExpressions;
using System.Net;
using System.Security;
using System.Data.Entity.Core.Metadata.Edm;
using System.Windows.Controls;

namespace Cryopreserved_Manager.ViewModels.Pages
{
    public partial class SettingsViewModel(IUserManagementService userManagementService) : ObservableObject, INavigationAware
    {
        private bool _isInitialized = false;

        [ObservableProperty]
        private string _appVersion = "Cyropreserved_Manager_v1.0";

        [ObservableProperty]
        private ApplicationTheme _currentTheme = ApplicationTheme.Unknown;

        [ObservableProperty]
        private bool isBtnVisible = false;
        [ObservableProperty]
        private bool isModifyVisible = false;
        [ObservableProperty]
        private List<string> statusItems = new List<string> { "Active", "InActive", "Lock" };
        [ObservableProperty]
        private List<string> roleItems = new List<string> { "Operator", "Administrator" };

        [ObservableProperty]
        private string firstName = string.Empty;
        [ObservableProperty]
        private string lastName = string.Empty;
        [ObservableProperty]
        private string userId = string.Empty;
        [ObservableProperty]
        private string role = string.Empty;
        [ObservableProperty]
        private string status = string.Empty;
        [ObservableProperty]
        private string department = string.Empty;
        [ObservableProperty]
        private string phone = string.Empty;
        [ObservableProperty]
        private string email = string.Empty;
        [ObservableProperty]
        private string password = string.Empty;
        [ObservableProperty]
        private string confirmPW = string.Empty;

        [ObservableProperty]
        private string okBtnText = "Done";

        public ObservableCollection<UserInfo> UserList { get; set; } = new ObservableCollection<UserInfo>();
        [ObservableProperty]
        private UserInfo selectedUser = new();

        public Task OnNavigatedToAsync()
        {
            if (!_isInitialized)
                InitializeViewModel();

            if (userManagementService.GetLoggedInUser().Role == "Administrator")
                IsBtnVisible = true;
            else
                IsBtnVisible = false;

            return Task.CompletedTask;
        }

        public Task OnNavigatedFromAsync() => Task.CompletedTask;

        private void InitializeViewModel()
        {
            CurrentTheme = ApplicationThemeManager.GetAppTheme();
            AppVersion = $"UiDesktopApp1 - {GetAssemblyVersion()}";

            _isInitialized = true;

            UpdateDBData();
        }

        private void UpdateDBData()
        {
            UserList.Clear();
            userManagementService.LoadAllUsers();

            var userListFromService = userManagementService.GetUserList();
            foreach (var user in userListFromService)
            {
                UserList.Add(user);
            }
        }

        [RelayCommand]
        private void OnSelectionChanged(UserInfo selectedUser)
        {
            if (selectedUser == null)
            {
                return;
            }

            SelectedUser = selectedUser;
            FirstName = selectedUser.FirstName;
            LastName = selectedUser.LastName;
            UserId = selectedUser.UserID;
            Role = selectedUser.Role;
            Status = selectedUser.Status;
            Department = selectedUser.Department;
            Phone = selectedUser.Phone;
            Email = selectedUser.Email;
        }

        [RelayCommand]
        private void NewUser()
        {
            OkBtnText = "Add New";
            IsModifyVisible = true;
        }
        [RelayCommand]
        private void ModifyUser()
        {
            OkBtnText = "Modify";
            IsModifyVisible = true;
        }
        [RelayCommand]
        private void DeleteUser()
        {
            OkBtnText = "Delete";
            IsModifyVisible = true;
        }

        // Update the code where the error occurs
        [RelayCommand]
        private void OnDone()
        {
            if (OkBtnText == "Add New")
            {
                if (FirstName == "")
                {
                    MessageBox.Show("Please enter the first name.");
                    return;
                }

                if (LastName == "")
                {
                    MessageBox.Show("Please enter the last name.");
                    return;
                }

                if (UserId == "")
                {
                    MessageBox.Show("Please enter the user ID.");
                    return;
                }

                // 6-12자의 영문, 숫자, 특수문자만 사용 가능
                if (!IsValidInput(UserId))
                {
                    MessageBox.Show("User ID must be 6 ~ 12 letters, numbers, and special characters");
                    return;
                }

                if (Password == "")
                {
                    MessageBox.Show("Please enter the password.");
                    return;
                }

                if (!IsValidPassword(Password))
                {
                    MessageBox.Show("Password must be 8 ~ 20 characters and must contain letters and numbers");
                    return;
                }

                if (Password != ConfirmPW)
                {
                    MessageBox.Show("Please confirm the password");
                    return;
                }

                DateTime password_changed = DateTime.Today;
                userManagementService.InsertUserDB(FirstName, LastName, UserId, Status, Role, Department, Phone, Email, Password, false, false, password_changed.ToString(), "0");
                UpdateDBData();
                CleanUserInput();
            }

            else if (OkBtnText == "Modify")
            {
                if (FirstName == "")
                {
                    MessageBox.Show("Please enter the first name.");
                    return;
                }

                if (LastName == "")
                {
                    MessageBox.Show("Please enter the last name.");
                    return;
                }
                
                if (Password != "")
                {
                    if (Password != ConfirmPW)
                    {
                        MessageBox.Show("Please confirm the password");
                        return;
                    }
                }
                UpdateDBData();
                CleanUserInput();
            }

            else if (OkBtnText == "Delete")
            {
                userManagementService.DeleteUser(UserId);
                UpdateDBData();
                CleanUserInput();
            }
        }
        [RelayCommand]
        private void OnCancel()
        {
            CleanUserInput();
            IsModifyVisible = false;
        }

        private void CleanUserInput()
        {
            FirstName = string.Empty;
            LastName = string.Empty;
            Department = string.Empty;
            UserId = string.Empty;
            Role = string.Empty;
            Status = string.Empty;
            Phone = string.Empty;
            Email = string.Empty;
            Password = string.Empty;
            ConfirmPW = string.Empty;
        }

        private bool IsValidInput(string inputString)
        {
            string pattern = "^[a-zA-Z0-9!@#$%^&*()_+{}|:\"<>?\\[\\];',./\\\\]+$";
            return Regex.IsMatch(inputString, pattern) && inputString.Length >= 6 && inputString.Length <= 12;
        }

        private bool IsValidPassword(string passwordString)
        {
            string pattern = "^(?=.*[a-zA-Z])(?=.*[0-9])[a-zA-Z0-9!@#$%^&*()_+{}|:\"<>?\\[\\];',./\\\\]{8,20}$";
            return Regex.IsMatch(passwordString, pattern);
        }

        private string GetAssemblyVersion()
        {
            return System.Reflection.Assembly.GetExecutingAssembly().GetName().Version?.ToString()
                ?? String.Empty;
        }

        [RelayCommand]
        private void OnChangeTheme(string parameter)
        {
            switch (parameter)
            {
                case "theme_light":
                    if (CurrentTheme == ApplicationTheme.Light)
                        break;

                    ApplicationThemeManager.Apply(ApplicationTheme.Light);
                    CurrentTheme = ApplicationTheme.Light;

                    break;

                default:
                    if (CurrentTheme == ApplicationTheme.Dark)
                        break;

                    ApplicationThemeManager.Apply(ApplicationTheme.Dark);
                    CurrentTheme = ApplicationTheme.Dark;

                    break;
            }
        }
    }
}
