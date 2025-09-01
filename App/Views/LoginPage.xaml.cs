using App.ViewModels;

namespace App.Views;

public partial class LoginPage : ContentPage
{
    public LoginPage(LoginViewModel loginViewModel)
    {
        InitializeComponent();
        BindingContext = loginViewModel;
    }

    protected override async void OnAppearing()
    {
        base.OnAppearing();
        await ((LoginViewModel) BindingContext).TryReLogin();
    }
}
