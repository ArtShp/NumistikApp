using App.ViewModels;

namespace App.Views;

public partial class AdminUsersPage : ContentPage
{
    public AdminUsersPage(AdminViewModel viewModel)
    {
        InitializeComponent();
        BindingContext = viewModel;
    }
}
