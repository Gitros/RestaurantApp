using RestaurantApp.Data;
using RestaurantApp.Entities;
using System.Collections.ObjectModel;
using System.Linq;

namespace RestaurantApp.ViewModels;

public class AllTablesViewModel : WorkspaceViewModel
{
    private readonly RestaurantDbContext _context = new();

    public ObservableCollection<RestaurantTable> Tables { get; } = new();

    public AllTablesViewModel()
    {
        base.DisplayName = "Stoliki";
        LoadTables();
    }

    private void LoadTables()
    {
        Tables.Clear();

        var data = _context.Tables
            .OrderBy(t => t.TableId)
            .ToList();

        foreach (var t in data)
            Tables.Add(t);
    }
}
