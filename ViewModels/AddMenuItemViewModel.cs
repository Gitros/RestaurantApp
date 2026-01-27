using RestaurantApp.Data;
using RestaurantApp.Entities;
using RestaurantApp.Helper;
using System.Collections.ObjectModel;
using System.Windows.Input;

namespace RestaurantApp.ViewModels;

public class AddMenuItemViewModel : WorkspaceViewModel
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
            _name = value;
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
        }
    }

    private decimal _basePrice;
    public decimal BasePrice
    {
        get => _basePrice;
        set
        {
            _basePrice = value;
            OnPropertyChanged(() => BasePrice);
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

        SaveCommand = new BaseCommand(Save, CanSave);

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

    private bool CanSave()
        => SelectedCategory != null
           && !string.IsNullOrWhiteSpace(Name)
           && BasePrice > 0;

    private void Save()
    {
        var item = new MenuItem
        {
            MenuCategoryId = SelectedCategory!.MenuCategoryId, // FK
            Name = Name.Trim(),
            Description = string.IsNullOrWhiteSpace(Description) ? null : Description.Trim(),
            BasePrice = BasePrice,
            IsActive = IsActive
        };

        _context.MenuItems.Add(item);
        _context.SaveChanges();

        MenuItemSaved?.Invoke();
        OnRequestClose();
    }
}
