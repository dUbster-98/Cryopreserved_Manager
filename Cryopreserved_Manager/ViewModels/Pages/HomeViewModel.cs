using Cryopreserved_Manager.Models;
using Cryopreserved_Manager.Services;
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
    public partial class HomeViewModel(ICellManageService cellManageService) : ObservableObject, INavigationAware
    {
        private bool _isInitialized = false;

        private ObservableCollection<CellInfo> CellCollection = new();
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

            foreach (CellInfo cellInfo in cellInfos)
            {
                CellCollection.Add(cellInfo);
            }

            _isInitialized = true;
        }

        [RelayCommand]
        private void OnEdit()
        {
            cellManageService.ModifyCellDB(selectedCell);
        }
        [RelayCommand]
        private void onDelete()
        {
            cellManageService.DeleteCellInfo(selectedCell.Id);
        }
    }
}
