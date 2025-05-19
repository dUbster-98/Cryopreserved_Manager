using Cryopreserved_Manager.ViewModels.Windows;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Data;
using System.Windows.Documents;
using System.Windows.Input;
using System.Windows.Media;
using System.Windows.Media.Imaging;
using System.Windows.Shapes;

namespace Cryopreserved_Manager.Views.Windows
{
    /// <summary>
    /// CellEditWindow.xaml에 대한 상호 작용 논리
    /// </summary>
    public partial class CellEditWindow : Window
    {
        public CellEditWindow(CellEditViewModel cellEditViewModel)
        {
            DataContext = cellEditViewModel;
            InitializeComponent();
        }
    }
}
