using App.ViewModels;

namespace App.Views;

public partial class MyCollectionsPage : ContentPage
{
    public MyCollectionsPage(MyCollectionsViewModel viewModel)
    {
        InitializeComponent();
        BindingContext = viewModel;
    }

    protected override async void OnAppearing()
    {
        base.OnAppearing();
        await ((MyCollectionsViewModel) BindingContext).InitializeAsync();
    }
}
