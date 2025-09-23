using App.ViewModels;

namespace App.Views;

public partial class UpdateCollectionItemPage : ContentPage
{
    public UpdateCollectionItemPage(UpdateCollectionItemViewModel viewModel)
    {
        InitializeComponent();
        BindingContext = viewModel;
    }

    protected override async void OnAppearing()
    {
        base.OnAppearing();
        await ((UpdateCollectionItemViewModel)BindingContext).InitializeAsync();
    }
}
