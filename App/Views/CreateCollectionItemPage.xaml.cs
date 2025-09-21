using App.ViewModels;

namespace App.Views;

public partial class CreateCollectionItemPage : ContentPage
{
    public CreateCollectionItemPage(CreateCollectionItemViewModel viewModel)
    {
        InitializeComponent();
        BindingContext = viewModel;
    }

    protected override async void OnAppearing()
    {
        base.OnAppearing();
        await ((CreateCollectionItemViewModel)BindingContext).InitializeAsync();
    }
}
