using RestaurantApp.Data;
using RestaurantApp.Dtos;
using RestaurantApp.Entities;
using System.Collections.ObjectModel;
using System.Linq;

namespace RestaurantApp.ViewModels;

public class AllMenuItemsViewModel : WorkspaceViewModel
{
    private readonly RestaurantDbContext _context = new();

    public ObservableCollection<MenuItemListItemDto> MenuItems { get; } = new();

    public AllMenuItemsViewModel()
    {
        base.DisplayName = "Menu";
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
                MenuCategoryName = m.MenuCategory.Name, // FK -> nazwa kategorii
                Name = m.Name,
                BasePrice = m.BasePrice,
                IsActive = m.IsActive
            })
            .ToList();

        foreach (var row in data)
            MenuItems.Add(row);
    }
}
