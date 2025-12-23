namespace vyaparsathi;

public partial class AppShell : Shell
{
    public AppShell()
    {
        InitializeComponent();

        // Register routes for navigation (optional)
        Routing.RegisterRoute(nameof(Views.SettingsPage), typeof(Views.SettingsPage));
        Routing.RegisterRoute(nameof(Views.AddCategoryPage), typeof(Views.AddCategoryPage));
        Routing.RegisterRoute(nameof(Views.AddItemPage), typeof(Views.AddItemPage));
    }
}
