﻿using System;
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
using System.Windows.Navigation;
using System.Windows.Shapes;
using Wpf.Ui.Abstractions.Controls;
using Cryopreserved_Manager.ViewModels.Pages;

namespace Cryopreserved_Manager.Views.Pages
{
    /// <summary>
    /// HomePage.xaml에 대한 상호 작용 논리
    /// </summary>
    public partial class HomePage
    {
        public HomePage(HomeViewModel viewModel)
        {
            DataContext = viewModel;

            InitializeComponent();
        }
    }
}
