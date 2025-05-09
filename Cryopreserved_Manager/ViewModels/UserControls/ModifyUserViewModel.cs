using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Wpf.Ui.Abstractions.Controls;

namespace Cryopreserved_Manager.ViewModels.UserControls
{
    public partial class ModifyUserViewModel :ObservableObject, INavigationAware
    {
        private bool _isInitialized = false;
        [ObservableProperty]
        private string userId = string.Empty;
        [ObservableProperty]
        private string password = string.Empty;
        [ObservableProperty]
        private string name = string.Empty;
        [ObservableProperty]
        private string department = string.Empty;
        [ObservableProperty]
        private string position = string.Empty;
        [ObservableProperty]
        private bool isAdmin = false;
        public Task OnNavigatedToAsync()
        {
            if (!_isInitialized)
                InitializeViewModel();
            return Task.CompletedTask;
        }
        public Task OnNavigatedFromAsync() => Task.CompletedTask;
        private void InitializeViewModel()
        {
            _isInitialized = true;
        }

    }
}
