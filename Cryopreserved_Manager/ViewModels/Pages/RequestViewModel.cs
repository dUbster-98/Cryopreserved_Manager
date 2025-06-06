﻿using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Wpf.Ui.Abstractions.Controls;

namespace Cryopreserved_Manager.ViewModels.Pages
{
    public partial class RequestViewModel : ObservableObject, INavigationAware
    {
        private bool _isInitialized = false;

        [ObservableProperty]
        private string? docNum;
        [ObservableProperty]
        private string? department;
        [ObservableProperty]
        private string? userName;
        [ObservableProperty]
        private string? cellToUse;
        [ObservableProperty]
        private string? requestDate;
        [ObservableProperty]
        private string? useDate;

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
