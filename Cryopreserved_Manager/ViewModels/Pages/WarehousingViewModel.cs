using bpac;
using Cryopreserved_Manager.Models;
using Cryopreserved_Manager.Services;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Controls;
using System.Xml.Linq;
using Wpf.Ui.Abstractions.Controls;

namespace Cryopreserved_Manager.ViewModels.Pages
{
    public partial class WarehousingViewModel(ICellManageService cellManageService) : ObservableObject, INavigationAware
    {
        private const string TEMPLATE_DIRECTORY = @"C:\Users\shkim3\Documents\MyLabel\";
        private const string TEMPLATE_PATH = "Bar_Code.LBX";

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
        public Task OnNavigatedToAsync()
        {
            if (!_isInitialized)
                InitializeViewModel();
            return Task.CompletedTask;
        }
        public Task OnNavigatedFromAsync() => Task.CompletedTask;
        private void InitializeViewModel()
        {
            State = new List<string>()
            {
                "미사용","진행","피드백","보류","사용완료","폐기"
            };

            _isInitialized = true;
        }


        private void PrintBarcode()
        {
            string templatePath = TEMPLATE_DIRECTORY;
            templatePath += TEMPLATE_PATH;
            try
            {
                bpac.DocumentClass doc = new DocumentClass();
                if (doc.Open(templatePath) != false)
                {
                    doc.GetObject("objBarcode").Text = BarcodeText;
                    doc.GetObject("objDesc").Text = Desc;

                    // doc.SetMediaById(doc.Printer.GetMediaId(), true);
                    doc.StartPrint("", PrintOptionConstants.bpoDefault);
                    doc.PrintOut(1, PrintOptionConstants.bpoDefault);
                    doc.EndPrint();
                    doc.Close();
                }

                else
                {
                    MessageBox.Show("Open() Error: " + doc.ErrorCode);
                }
            }
            catch (Exception ex)
            {
                MessageBox.Show(ex.Message);
            }
        }

        [RelayCommand]
        private void OnSave()
        {
            if (CellName == "")
            {
                MessageBox.Show("Please enter the Cell name.");
                return;
            }
            if (ReceiptDay == "")
            {
                MessageBox.Show("Please enter the Receipt Day.");
                return;
            }
            if (CellState == "")
            {
                MessageBox.Show("Please enter the Cell State.");
                return;
            }

            cellInfo = new CellInfo()
            {
                Name = CellName,
                Quantity = Quantity,
                Location = Location,
                ReceiptDay = ReceiptDay,
                BarcodeText = BarcodeText,
                State = CellState,
                Desc = Desc
            };
            cellManageService.InsertCellDB(cellInfo);

            PrintBarcode();
        }
    }
}
