using System.Windows.Input;
using App.Models;
using App.Services;
using CommunityToolkit.Mvvm.ComponentModel;
using CommunityToolkit.Mvvm.Input;

namespace App.ViewModels;

public partial class LoginViewModel : ObservableObject
{
    private readonly ILoginService _loginService;

    private readonly LoginCredentials _creds = new();

    public string Username
    {
        get => _creds.Username;
        set
        {
            if (_creds.Username != value)
            {
                _creds.Username = value;
                OnPropertyChanged();
            }
        }
    }

    public string Password
    {
        get => _creds.Password;
        set
        {
            if (_creds.Password != value)
            {
                _creds.Password = value;
                OnPropertyChanged();
            }
        }
    }

    public ICommand LoginCommand { get; init; }

    private LoginViewModel()
    {
        LoginCommand = new AsyncRelayCommand(OnLogin);
    }

    public LoginViewModel(ILoginService loginService) : this()
    {
        _loginService = loginService;
    }

    private async Task OnLogin()
    {
        if (!string.IsNullOrWhiteSpace(Username) && !string.IsNullOrWhiteSpace(Password))
        {
            if (await _loginService.TryLoginAsync(_creds))
            {
                await Shell.Current.GoToAsync("//MainPage");
            }
            else
            {
                await Shell.Current.CurrentPage.DisplayAlert("Login Failed", "Invalid username or password.", "OK");
            }
        }
        else
        {
            await Shell.Current.CurrentPage.DisplayAlert("Login Failed", "Please enter both username and password.", "OK");
        }
    }
}
