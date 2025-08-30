using System.Windows.Input;
using App.Models;
using CommunityToolkit.Mvvm.ComponentModel;
using CommunityToolkit.Mvvm.Input;

namespace App.ViewModels;

public partial class LoginViewModel : ObservableObject
{
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

    public LoginViewModel()
    {
        LoginCommand = new AsyncRelayCommand(OnLogin);
    }

    private async Task OnLogin()
    {
        if (!string.IsNullOrWhiteSpace(Username) && !string.IsNullOrWhiteSpace(Password))
        {
            await Shell.Current.GoToAsync("//MainPage");
        }
        else
        {
            await Shell.Current.CurrentPage.DisplayAlert("Login Failed", "Please enter both username and password.", "OK");
        }
    }
}
