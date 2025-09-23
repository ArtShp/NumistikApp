using App.ViewModels;

namespace App.Views;

public partial class CollectionItemDetailsPage : ContentPage
{
    public CollectionItemDetailsPage(CollectionItemDetailsViewModel viewModel)
    {
        InitializeComponent();
        BindingContext = viewModel;
    }
}
