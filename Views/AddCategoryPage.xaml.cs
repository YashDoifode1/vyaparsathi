using vyaparsathi.Models;

namespace vyaparsathi.Views;

public partial class AddCategoryPage : ContentPage
{
    private List<Category> _allCategories;
    private Category _editingCategory;

    public AddCategoryPage()
    {
        InitializeComponent();
    }

    protected override async void OnAppearing()
    {
        base.OnAppearing();
        await LoadCategories();
    }

    private async Task LoadCategories()
    {
        _allCategories = await App.Database.GetCategoriesAsync();
        CategoryCollectionView.ItemsSource = _allCategories;
    }

    // Save or Update
    private async void OnSaveClicked(object sender, EventArgs e)
    {
        if (string.IsNullOrWhiteSpace(CategoryNameEntry.Text))
        {
            await DisplayAlert("Error", "Category name is required", "OK");
            return;
        }

        if (_editingCategory != null)
        {
            // Update existing
            _editingCategory.Name = CategoryNameEntry.Text.Trim();
            _editingCategory.Description = CategoryDescriptionEditor.Text?.Trim();
            await App.Database.SaveCategoryAsync(_editingCategory);

            _editingCategory = null;
            FormTitle.Text = "Add Category";
            SaveButton.Text = "Save Category";
        }
        else
        {
            // New category
            var category = new Category
            {
                Name = CategoryNameEntry.Text.Trim(),
                Description = CategoryDescriptionEditor.Text?.Trim(),
                CreatedAt = DateTime.UtcNow
            };
            await App.Database.SaveCategoryAsync(category);
        }

        // Clear fields
        CategoryNameEntry.Text = "";
        CategoryDescriptionEditor.Text = "";

        await LoadCategories();
    }

    // Delete
    private async void OnDeleteClicked(object sender, EventArgs e)
    {
        var button = sender as Button;
        var category = button?.CommandParameter as Category;
        if (category == null) return;

        bool confirm = await DisplayAlert(
            "Delete Category",
            $"Delete '{category.Name}'?",
            "Delete",
            "Cancel");

        if (!confirm) return;

        await App.Database.DeleteCategoryAsync(category);
        await LoadCategories();
    }

    // Edit
    private void OnEditClicked(object sender, EventArgs e)
    {
        var button = sender as Button;
        var category = button?.CommandParameter as Category;
        if (category == null) return;

        _editingCategory = category;
        CategoryNameEntry.Text = category.Name;
        CategoryDescriptionEditor.Text = category.Description;

        FormTitle.Text = "Edit Category";
        SaveButton.Text = "Update Category";
    }

    // Search
    private void OnSearchTextChanged(object sender, TextChangedEventArgs e)
    {
        var text = e.NewTextValue?.ToLower() ?? "";
        var filtered = _allCategories
            .Where(c => c.Name.ToLower().Contains(text) ||
                        (c.Description?.ToLower().Contains(text) ?? false))
            .ToList();

        CategoryCollectionView.ItemsSource = filtered;
    }
}
