using Cryopreserved_Manager.ViewModels.Pages;
using Wpf.Ui.Abstractions.Controls;

namespace Cryopreserved_Manager.Views.Pages
{
    public partial class DataPage
    {
        public DataPage(DataViewModel viewModel)
        {
            DataContext = viewModel;

            InitializeComponent();
        }
    }
}
