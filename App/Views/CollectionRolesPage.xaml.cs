using App.ViewModels;

namespace App.Views;

public partial class CollectionRolesPage : ContentPage
{
    public CollectionRolesPage(CollectionRolesViewModel viewModel)
    {
        InitializeComponent();
        BindingContext = viewModel;
    }

    protected override async void OnAppearing()
    {
        base.OnAppearing();
        await ((CollectionRolesViewModel)BindingContext).InitializeAsync();
    }
}
