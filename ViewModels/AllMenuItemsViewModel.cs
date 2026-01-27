using RestaurantApp.Data;
using RestaurantApp.Dtos;
using RestaurantApp.Helper;
using System.Collections.ObjectModel;
using System.ComponentModel;
using System.Windows.Data;
using System.Windows.Input;

namespace RestaurantApp.ViewModels;

public class AllMenuItemsViewModel : WorkspaceViewModel
{
    private readonly RestaurantDbContext _context = new();

    public ObservableCollection<MenuItemListItemDto> MenuItems { get; } = new();

    public event Action? RequestAddMenuItem;
    public ICommand AddMenuItemCommand { get; }

    // To bindujesz do DataGrid
    public ICollectionView MenuItemsView { get; }

    private string _searchText = "";
    public string SearchText
    {
        get => _searchText;
        set
        {
            _searchText = value ?? "";
            OnPropertyChanged(() => SearchText);
            MenuItemsView.Refresh();
            OnPropertyChanged(() => FilteredCountText);
        }
    }

    private bool _onlyActive;
    public bool OnlyActive
    {
        get => _onlyActive;
        set
        {
            _onlyActive = value;
            OnPropertyChanged(() => OnlyActive);
            MenuItemsView.Refresh();
            OnPropertyChanged(() => FilteredCountText);
        }
    }

    // prosta filtracja po nazwie kategorii (textbox), żeby nie robić od razu comboboxa
    private string _categoryFilter = "";
    public string CategoryFilter
    {
        get => _categoryFilter;
        set
        {
            _categoryFilter = value ?? "";
            OnPropertyChanged(() => CategoryFilter);
            MenuItemsView.Refresh();
            OnPropertyChanged(() => FilteredCountText);
        }
    }

    public string FilteredCountText => $"Pozycji: {MenuItemsView.Cast<object>().Count()}";

    public AllMenuItemsViewModel()
    {
        DisplayName = "Menu";

        AddMenuItemCommand = new BaseCommand(() => RequestAddMenuItem?.Invoke());

        MenuItemsView = CollectionViewSource.GetDefaultView(MenuItems);
        MenuItemsView.Filter = FilterRow;

        LoadMenuItems();
        OnPropertyChanged(() => FilteredCountText);
    }

    public void Refresh()
    {
        LoadMenuItems();
    }


    private void LoadMenuItems()
    {
        MenuItems.Clear();

        var data = _context.MenuItems
            .OrderBy(m => m.MenuItemId)
            .Select(m => new MenuItemListItemDto
            {
                MenuItemId = m.MenuItemId,
                MenuCategoryId = m.MenuCategoryId,
                MenuCategoryName = m.MenuCategory.Name,
                Name = m.Name,
                BasePrice = m.BasePrice,
                IsActive = m.IsActive
            })
            .ToList();

        foreach (var row in data)
            MenuItems.Add(row);

        MenuItemsView.Refresh();
        OnPropertyChanged(() => FilteredCountText);
    }

    private bool FilterRow(object obj)
    {
        if (obj is not MenuItemListItemDto row) return false;

        // Search po nazwie menu itemu
        if (!string.IsNullOrWhiteSpace(SearchText))
        {
            if (string.IsNullOrWhiteSpace(row.Name) ||
                !row.Name.Contains(SearchText, StringComparison.OrdinalIgnoreCase))
                return false;
        }

        // Filtr po kategorii (po nazwie)
        if (!string.IsNullOrWhiteSpace(CategoryFilter))
        {
            if (string.IsNullOrWhiteSpace(row.MenuCategoryName) ||
                !row.MenuCategoryName.Contains(CategoryFilter, StringComparison.OrdinalIgnoreCase))
                return false;
        }

        // Tylko aktywne
        if (OnlyActive && !row.IsActive)
            return false;

        return true;
    }
}
