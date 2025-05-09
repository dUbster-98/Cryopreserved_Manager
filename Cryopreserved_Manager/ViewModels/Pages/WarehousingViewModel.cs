using bpac;
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
    public partial class WarehousingViewModel : ObservableObject, INavigationAware
    {
        private const string TEMPLATE_DIRECTORY = @"C:\Users\shkim3\Documents\MyLabel\";
        private const string TEMPLATE_PATH = "Bar_Code.LBX";

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
        private int amount = 0;
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

        [RelayCommand]
        private void OnSave()
        {
            string templatePath = TEMPLATE_DIRECTORY;
            templatePath += TEMPLATE_PATH;

            bpac.DocumentClass doc = new DocumentClass();
            if (doc.Open(templatePath) != false)
            {
                doc.GetObject("Barcode").Text = $"{CellName}_{ReceiptDay}";
                doc.GetObject("Desc").Text = Desc;

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
    }
}
