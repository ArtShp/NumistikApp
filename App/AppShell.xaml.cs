using App.Views;

namespace App
{
    public partial class AppShell : Shell
    {
        public AppShell()
        {
            InitializeComponent();

            Routing.RegisterRoute(nameof(CollectionItemsPage), typeof(CollectionItemsPage));
            Routing.RegisterRoute(nameof(CreateCollectionItemPage), typeof(CreateCollectionItemPage));
            Routing.RegisterRoute(nameof(CollectionRolesPage), typeof(CollectionRolesPage));
            Routing.RegisterRoute(nameof(CollectionItemDetailsPage), typeof(CollectionItemDetailsPage));
            Routing.RegisterRoute(nameof(ImageViewerPage), typeof(ImageViewerPage));
            Routing.RegisterRoute(nameof(UpdateCollectionItemPage), typeof(UpdateCollectionItemPage));
        }
    }
}
