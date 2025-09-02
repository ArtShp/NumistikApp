using CommunityToolkit.Mvvm.ComponentModel;
using CommunityToolkit.Mvvm.Input;
using System.Windows.Input;

namespace App.ViewModels;

public partial class AdminViewModel : ObservableObject
{
    public ICommand BackToMainCommand { get; init; }

    public AdminViewModel()
    {
        BackToMainCommand = new AsyncRelayCommand(OnBackToMain);
    }

    private async Task OnBackToMain()
    {
        await Shell.Current.GoToAsync("//MainTabs");
    }
}
