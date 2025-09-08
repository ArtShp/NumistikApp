using App.ViewModels;

namespace App.Views;

public partial class CollectionItemsPage : ContentPage
{
    public CollectionItemsPage(CollectionItemsViewModel viewModel)
    {
        InitializeComponent();
        BindingContext = viewModel;
    }

    protected override async void OnAppearing()
    {
        base.OnAppearing();
        await ((CollectionItemsViewModel) BindingContext).InitializeAsync();
    }
}
