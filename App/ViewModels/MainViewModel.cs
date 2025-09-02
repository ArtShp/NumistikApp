using App.Services;
using CommunityToolkit.Mvvm.ComponentModel;
using CommunityToolkit.Mvvm.Input;
using System.Windows.Input;

namespace App.ViewModels;

public partial class MainViewModel : ObservableObject
{
    private readonly IRestApiService _restApiService;

    public ICommand LogoutCommand { get; init; }
    public ICommand NavigateToAdminCommand { get; init; }

    public MainViewModel(IRestApiService service)
    {
        LogoutCommand = new AsyncRelayCommand(OnLogout);
        NavigateToAdminCommand = new AsyncRelayCommand(OnNavigateToAdmin);

        _restApiService = service;
    }

    private async Task OnLogout()
    {
        _restApiService.Logout();
        await Shell.Current.GoToAsync("//LoginPage");
    }

    private async Task OnNavigateToAdmin()
    {
        await Shell.Current.GoToAsync("//AdminTabs");
    }
}
