using RestaurantApp.Data;
using RestaurantApp.Dtos;
using System.Collections.ObjectModel;

namespace RestaurantApp.ViewModels;

public class TableUsageReportViewModel : WorkspaceViewModel
{
    private readonly RestaurantDbContext _context = new();

    public ObservableCollection<TableUsageReportRowDto> Rows { get; } = new();

    public TableUsageReportViewModel()
    {
        DisplayName = "Raport: obłożenie stolików";
        Load();
    }

    private void Load()
    {
        Rows.Clear();

        var data = _context.Tables
            .Select(t => new TableUsageReportRowDto
            {
                TableId = t.TableId,
                AreaName = t.Area.Name,
                TableName = t.Name,
                SeatsCount = t.SeatsCount,
                IsActive = t.IsActive,

                OrdersCount = t.Orders.Count(),
                ReservationsCount = t.Reservations.Count(),
                OrdersRevenue = t.Orders.Sum(o => (decimal?)o.TotalAmount) ?? 0m
            })
            .OrderByDescending(x => x.OrdersCount)
            .ThenByDescending(x => x.ReservationsCount)
            .ToList();

        foreach (var row in data)
            Rows.Add(row);
    }
}
