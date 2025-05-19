using CommunityToolkit.Mvvm.Messaging;
using Cryopreserved_Manager.Models;
using Cryopreserved_Manager.Services;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Wpf.Ui.Abstractions.Controls;

namespace Cryopreserved_Manager.ViewModels.Windows
{
    public partial class CellEditViewModel : ObservableObject
    {
        private CellInfo cellInfo = new();

        [ObservableProperty]
        private string cellName = "";
        partial void OnCellNameChanged(string? name)
        {
            if (name != null && DateTime.TryParse(ReceiptDay, out DateTime parsedDate))
            {
                string formattedDate = parsedDate.ToString("yyMMdd");
                BarcodeText = $"{CellName}_{formattedDate}";
            }
        }
        [ObservableProperty]
        private string quantity = "0";
        [ObservableProperty]
        private string location = "";
        [ObservableProperty]
        private string receiptDay = DateTime.Today.ToString("yyyy-MM-dd");
        partial void OnReceiptDayChanged(string? day)
        {
            if (day != null && DateTime.TryParse(day, out DateTime parsedDate))
            {
                string formattedDate = parsedDate.ToString("yyMMdd");
                BarcodeText = $"{CellName}_{formattedDate}";
            }
        }
        [ObservableProperty]
        private string cellState = "";

        [ObservableProperty]
        private List<string> state = [];
        [ObservableProperty]
        private string barcodeText = "";
        [ObservableProperty]
        private string desc = "";
        private bool _isInitialized = false;

        private readonly CellManageService _cellManageService;

        public CellEditViewModel()
        {
            WeakReferenceMessenger.Default.Register<TransferCellInfo>(this, OnTransferCellInfo);
        }

        [RelayCommand]
        private void Save()
        {
            _cellManageService.ModifyCellDB(new CellInfo()
            {
                Key = cellInfo.Key,
                Name = CellName,
                Quantity = Quantity,
                Location = Location,
                ReceiptDay = ReceiptDay,
                BarcodeText = BarcodeText,
                State = CellState,
                Desc = Desc
            });
        }
        [RelayCommand]
        private void Cancel()
        {
            // Cancel logic here
        }
        private void OnTransferCellInfo(object recipient, TransferCellInfo message)
        {
            cellInfo = message.cellInfo;

            CellName = cellInfo.Name;
            Quantity = cellInfo.Quantity;
            Location = cellInfo.Location;
            ReceiptDay = cellInfo.ReceiptDay;
            BarcodeText = cellInfo.BarcodeText;
            CellState = cellInfo.State;
            Desc = cellInfo.Desc;
        }
    }
}
