using RestaurantApp.Data;
using RestaurantApp.Dtos;
using RestaurantApp.Helper;
using System.Collections.ObjectModel;
using System.Windows.Input;

namespace RestaurantApp.ViewModels;

public class AllTablesViewModel : WorkspaceViewModel
{
    private readonly RestaurantDbContext _context = new();

    public ObservableCollection<TableListItemDto> Tables { get; } = new();

    public event Action? RequestAddTable;
    public ICommand AddTableCommand { get; }

    public AllTablesViewModel()
    {
        DisplayName = "Stoliki";
        AddTableCommand = new BaseCommand(() => RequestAddTable?.Invoke());
        LoadTables();
    }

    private void LoadTables()
    {
        Tables.Clear();

        var data = _context.Tables
            .OrderBy(t => t.TableId)
            .Select(t => new TableListItemDto
            {
                TableId = t.TableId,
                Name = t.Name,
                SeatsCount = t.SeatsCount,
                IsActive = t.IsActive,
                AreaId = t.AreaId,
                AreaName = t.Area.Name
            })
            .ToList();

        foreach (var row in data)
            Tables.Add(row);
    }

    public void Refresh() => LoadTables();
}
