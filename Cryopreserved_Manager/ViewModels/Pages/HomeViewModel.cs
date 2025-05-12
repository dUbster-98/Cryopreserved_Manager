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
            cellManageService.ModifyCellDB(SelectedCell);
        }
        [RelayCommand]
        private void Delete()
        {
            cellManageService.DeleteCellInfo(SelectedCell.Key);
        }
    }
}
