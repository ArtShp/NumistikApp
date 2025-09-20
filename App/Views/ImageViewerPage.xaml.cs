using App.ViewModels;

namespace App.Views;

public partial class ImageViewerPage : ContentPage
{
    public ImageViewerPage(ImageViewerViewModel viewModel)
    {
        InitializeComponent();
        BindingContext = viewModel;
    }
}
