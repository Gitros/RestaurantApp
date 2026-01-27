using RestaurantApp.Data;
using RestaurantApp.Entities;
using RestaurantApp.Helper;
using System.Collections.ObjectModel;
using System.Globalization;
using System.Windows.Input;

namespace RestaurantApp.ViewModels;

public class AddMenuItemViewModel : ValidatableViewModel
{
    private readonly RestaurantDbContext _context = new();

    public event Action? MenuItemSaved;

    public ObservableCollection<MenuCategory> Categories { get; } = new();

    private MenuCategory? _selectedCategory;
    public MenuCategory? SelectedCategory
    {
        get => _selectedCategory;
        set
        {
            _selectedCategory = value;
            OnPropertyChanged(() => SelectedCategory);
            ((BaseCommand)SaveCommand).RaiseCanExecuteChanged();
        }
    }

    private string _name = "";
    public string Name
    {
        get => _name;
        set
        {
            _name = value ?? "";
            OnPropertyChanged(() => Name);
            ((BaseCommand)SaveCommand).RaiseCanExecuteChanged();
        }
    }

    private string? _description;
    public string? Description
    {
        get => _description;
        set
        {
            _description = value;
            OnPropertyChanged(() => Description);
            ((BaseCommand)SaveCommand).RaiseCanExecuteChanged();
        }
    }

    // ZAMIANA: zamiast decimal w TextBox -> string
    private string _basePriceText = "";
    public string BasePriceText
    {
        get => _basePriceText;
        set
        {
            _basePriceText = value ?? "";
            OnPropertyChanged(() => BasePriceText);
            ((BaseCommand)SaveCommand).RaiseCanExecuteChanged();
        }
    }

    private bool _isActive = true;
    public bool IsActive
    {
        get => _isActive;
        set
        {
            _isActive = value;
            OnPropertyChanged(() => IsActive);
        }
    }

    public ICommand SaveCommand { get; }

    public AddMenuItemViewModel()
    {
        DisplayName = "Dodaj pozycję menu";
        SaveCommand = new BaseCommand(Save, () => true);

        LoadCategories();
    }

    private void LoadCategories()
    {
        Categories.Clear();

        var data = _context.MenuCategories
            .OrderBy(c => c.MenuCategoryId)
            .ToList();

        foreach (var c in data)
            Categories.Add(c);

        SelectedCategory = Categories.FirstOrDefault();
    }

    private void Save()
    {
        ShowErrors = true;

        // wymuś „przeliczenie” błędów w UI
        OnPropertyChanged(() => SelectedCategory);
        OnPropertyChanged(() => Name);
        OnPropertyChanged(() => Description);
        OnPropertyChanged(() => BasePriceText);

        if (!IsValid)
            return;

        var basePrice = ParsePrice(BasePriceText);

        var item = new MenuItem
        {
            MenuCategoryId = SelectedCategory!.MenuCategoryId,
            Name = Name.Trim(),
            Description = string.IsNullOrWhiteSpace(Description) ? null : Description.Trim(),
            BasePrice = basePrice,
            IsActive = IsActive
        };

        _context.MenuItems.Add(item);
        _context.SaveChanges();

        MenuItemSaved?.Invoke();
        OnRequestClose();
    }

    private static decimal ParsePrice(string text)
    {
        text = (text ?? "").Trim();

        // pozwól na 12.50 i 12,50
        text = text.Replace(',', '.');

        decimal.TryParse(text, NumberStyles.Number, CultureInfo.InvariantCulture, out var value);
        return value;
    }


    public override string this[string columnName]
    {
        get
        {
            if (!ShowErrors)
                return "";

            switch (columnName)
            {
                case nameof(SelectedCategory):
                    return SelectedCategory == null ? "Wybierz kategorię." : "";

                case nameof(Name):
                    if (string.IsNullOrWhiteSpace(Name))
                        return "Nazwa jest wymagana.";
                    if (Name.Trim().Length < 2)
                        return "Nazwa musi mieć co najmniej 2 znaki.";
                    if (Name.Trim().Length > 80)
                        return "Nazwa może mieć max 80 znaków.";
                    return "";

                case nameof(Description):
                    if (!string.IsNullOrWhiteSpace(Description) && Description.Trim().Length > 200)
                        return "Opis może mieć max 200 znaków.";
                    return "";

                case nameof(BasePriceText):
                    var p = ParsePrice(BasePriceText);
                    if (string.IsNullOrWhiteSpace(BasePriceText))
                        return "Cena jest wymagana.";
                    if (p <= 0)
                        return "Cena musi być większa od 0.";
                    return "";
            }

            return "";
        }
    }

    public override bool IsValid =>
        string.IsNullOrEmpty(this[nameof(SelectedCategory)]) &&
        string.IsNullOrEmpty(this[nameof(Name)]) &&
        string.IsNullOrEmpty(this[nameof(Description)]) &&
        string.IsNullOrEmpty(this[nameof(BasePriceText)]);
}
