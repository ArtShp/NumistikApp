using App.ViewModels;

namespace App.Views;

public partial class CreateCollectionItemPage : ContentPage
{
    public CreateCollectionItemPage(CreateCollectionItemViewModel viewModel)
    {
        InitializeComponent();
        BindingContext = viewModel;
    }
}
