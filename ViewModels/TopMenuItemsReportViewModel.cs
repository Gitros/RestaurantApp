using RestaurantApp.Data;
using RestaurantApp.Dtos;
using System.Collections.ObjectModel;

namespace RestaurantApp.ViewModels;

public class TopMenuItemsReportViewModel : WorkspaceViewModel
{
    private readonly RestaurantDbContext _context = new();

    public ObservableCollection<TopMenuItemsReportRowDto> Rows { get; } = new();

    public TopMenuItemsReportViewModel()
    {
        DisplayName = "Raport: TOP pozycje menu";
        Load();
    }

    private void Load()
    {
        Rows.Clear();

        var data = _context.OrderItems
            .OrderByDescending(oi => oi.TotalPrice)
            .GroupBy(oi => new
            {
                oi.MenuItemId,
                MenuItemName = oi.MenuItem.Name,
                CategoryName = oi.MenuItem.MenuCategory.Name
            })
            .Select(g => new TopMenuItemsReportRowDto
            {
                MenuItemId = g.Key.MenuItemId,
                MenuItemName = g.Key.MenuItemName,
                CategoryName = g.Key.CategoryName,
                QuantitySold = g.Sum(x => x.Quantity),
                Revenue = g.Sum(x => x.TotalPrice)
            })
            .OrderByDescending(x => x.QuantitySold)
            .ThenByDescending(x => x.Revenue)
            .Take(10)
            .ToList();

        foreach (var row in data)
            Rows.Add(row);
    }
}
