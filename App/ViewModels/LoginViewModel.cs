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

    public string ServerUrl
    {
        get => AppSettings.ServerUrl;
        set
        {
            if (AppSettings.ServerUrl != value)
            {
                AppSettings.ServerUrl = value;
                OnPropertyChanged();
            }
        }
    }

    private bool _isSettingsVisible;
    public bool IsSettingsVisible
    {
        get => _isSettingsVisible;
        set => SetProperty(ref _isSettingsVisible, value);
    }

    public ICommand LoginCommand { get; init; }
    public ICommand ToggleSettingsCommand { get; init; }
    public ICommand SaveServerUrlCommand { get; init; }
    
    public LoginViewModel(ILoginService loginService)
    {
        _loginService = loginService;

        LoginCommand = new AsyncRelayCommand(OnLogin);
        ToggleSettingsCommand = new RelayCommand(OnToggleSettings);
        SaveServerUrlCommand = new AsyncRelayCommand(OnSaveServerUrl);
    }

    private void OnToggleSettings() => IsSettingsVisible = !IsSettingsVisible;

    private async Task OnSaveServerUrl()
    {
        await Shell.Current.CurrentPage.DisplayAlert("Settings", "Server URL updated.", "OK");
        IsSettingsVisible = false;
    }

    private async Task OnLogin()
    {
        if (!string.IsNullOrWhiteSpace(Username) && !string.IsNullOrWhiteSpace(Password))
        {
            if (await _loginService.TryLoginAsync(_creds))
            {
                ClearCredentials();
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

    public async Task TryReLogin()
    {
        if (!string.IsNullOrWhiteSpace(AppSettings.Username) && !string.IsNullOrWhiteSpace(AppSettings.RefreshToken))
        {
            if (await _loginService.TryReLoginAsync())
            {
                ClearCredentials();
                await Shell.Current.GoToAsync("//MainPage");
            }
        }
    }

    private void ClearCredentials()
    {
        Username = string.Empty;
        Password = string.Empty;
    }
}
