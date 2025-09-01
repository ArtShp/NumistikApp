using System.Windows.Input;
using CommunityToolkit.Mvvm.ComponentModel;
using CommunityToolkit.Mvvm.Input;
using App.Services;
using App.Models;
using Shared.Models.Auth;

namespace App.ViewModels;

public partial class RegisterViewModel : ObservableObject
{
    private readonly ILoginService _loginService;

    private readonly RegisterCredentials _creds = new();

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

    public string InviteToken
    {
        get => _creds.InviteToken;
        set
        {
            if (_creds.InviteToken != value)
            {
                _creds.InviteToken = value;
                OnPropertyChanged();
            }
        }
    }

    public ICommand RegisterCommand { get; init; }
    public ICommand BackCommand { get; init; }

    public RegisterViewModel(ILoginService service)
    {
        _loginService = service;

        RegisterCommand = new AsyncRelayCommand(RegisterAsync);
        BackCommand = new AsyncRelayCommand(BackAsync);
    }

    private async Task RegisterAsync()
    {
        if (string.IsNullOrWhiteSpace(Username) ||
            string.IsNullOrWhiteSpace(Password))
        {
            await Shell.Current.DisplayAlert("Error", "Please fill username and password.", "OK");
            return;
        }

        if (!string.IsNullOrWhiteSpace(InviteToken) && !Guid.TryParse(InviteToken, out _))
        {
            await Shell.Current.DisplayAlert("Error", "Invalid invite token format.", "OK");
            return;
        }

        var success = await _loginService.Register(new UserRegistrationDto.Request
        {
            Username = Username,
            Password = Password,
            InviteToken = string.IsNullOrWhiteSpace(InviteToken) ? null : Guid.Parse(InviteToken)
        });

        if (success)
        {
            await Shell.Current.DisplayAlert("Success", "Registration successful", "OK");
            await Shell.Current.GoToAsync("//LoginPage");
        }
        else
        {
            await Shell.Current.DisplayAlert("Error", "Registration failed", "OK");
        }
    }

    private async Task BackAsync()
    {
        await Shell.Current.GoToAsync("//LoginPage");
    }
}
