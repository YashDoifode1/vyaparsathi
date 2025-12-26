namespace vyaparsathi.Views;

public partial class StartupPage : ContentPage
{
    public StartupPage()
    {
        InitializeComponent();
    }

    protected override async void OnAppearing()
    {
        base.OnAppearing();

        // Simulate loading (DB, user session, settings)
        await Task.Delay(2000);

        // Navigate to Dashboard (Shell root)
        await Shell.Current.GoToAsync("//dashboard");
    }
}
