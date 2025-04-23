using Wpf.Ui;
using Wpf.Ui.Controls;

namespace Cryopreserved_Manager.ViewModels.Pages
{
    public partial class DashboardViewModel(INavigationService navigationService) : ObservableObject
    {
        [ObservableProperty]
        private int _counter = 0;

        [RelayCommand]
        private void OnCounterIncrement()
        {
            Counter++;
        }

        [RelayCommand]
        private void DoSomthing()
        {

        }

        [RelayCommand]
        private void NavigateForward(Type type)
        {
            _ = navigationService.NavigateWithHierarchy(type);
        }
    }
}
