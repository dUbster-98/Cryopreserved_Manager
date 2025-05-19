using CommunityToolkit.Mvvm.Messaging;
using Cryopreserved_Manager.Models;
using Cryopreserved_Manager.Services;
using Cryopreserved_Manager.Views.Windows;
using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Controls;
using Wpf.Ui.Abstractions.Controls;

namespace Cryopreserved_Manager.ViewModels.Pages
{
    public partial class HomeViewModel(ICellManageService cellManageService, WindowsProviderService windowsProviderService) : ObservableObject, INavigationAware
    {
        private bool _isInitialized = false;

        [ObservableProperty]
        private ObservableCollection<CellInfo> cellCollection = new();
        [ObservableProperty]
        private CellInfo selectedCell = new();

        public Task OnNavigatedToAsync()
        {
            if (!_isInitialized)
                InitializeViewModel();
            return Task.CompletedTask;
        }
        public Task OnNavigatedFromAsync() => Task.CompletedTask;
        private void InitializeViewModel()
        {
            List<CellInfo> cellInfos = cellManageService.GetAllCellInfos();

            CellCollection.Clear();
            foreach (CellInfo cellInfo in cellInfos)
            {
                CellCollection.Add(cellInfo);
            }

            //_isInitialized = true;
        }

        [RelayCommand]
        private void Edit()
        {
            windowsProviderService.Show<CellEditWindow>();
            WeakReferenceMessenger.Default.Send(new TransferCellInfo { cellInfo = SelectedCell });
        }
        [RelayCommand]
        private void Delete()
        {
            if (MessageBox.Show("선택하신 정보가 삭제됩니다.", "경고", MessageBoxButton.YesNo) == MessageBoxResult.Yes)
            {
                cellManageService.DeleteCellInfo(SelectedCell.Key);
            }
            else
            {
                
            }
        }
    }
}
