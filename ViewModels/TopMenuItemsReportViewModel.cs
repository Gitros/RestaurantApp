using RestaurantApp.Data;
using RestaurantApp.Dtos;
using RestaurantApp.Helper;
using System.Collections.ObjectModel;
using System.Windows.Input;

namespace RestaurantApp.ViewModels;

public class TopMenuItemsReportViewModel : WorkspaceViewModel
{
    private readonly RestaurantDbContext _context = new();

    public ObservableCollection<TopMenuItemsReportRowDto> Rows { get; } = new();

    public TopMenuItemsReportViewModel()
    {
        DisplayName = "Raport: TOP pozycje menu";
        FilterCommand = new BaseCommand(Load, () => DateFrom.Date <= DateTo.Date && TopN > 0);
        ResetCommand = new BaseCommand(() =>
        {
            DateFrom = DateTime.Today.AddDays(-30);
            DateTo = DateTime.Today;
            TopN = 10;
            Load();
        });

        Load();
    }

    private DateTime _dateFrom = DateTime.Today.AddDays(-30);
    public DateTime DateFrom
    {
        get => _dateFrom;
        set { _dateFrom = value; OnPropertyChanged(() => DateFrom); ((BaseCommand)FilterCommand).RaiseCanExecuteChanged(); }
    }

    private DateTime _dateTo = DateTime.Today;
    public DateTime DateTo
    {
        get => _dateTo;
        set { _dateTo = value; OnPropertyChanged(() => DateTo); ((BaseCommand)FilterCommand).RaiseCanExecuteChanged(); }
    }

    private int _topN = 10;
    public int TopN
    {
        get => _topN;
        set { _topN = value; OnPropertyChanged(() => TopN); ((BaseCommand)FilterCommand).RaiseCanExecuteChanged(); }
    }

    private TopMenuItemsReportRowDto? _selectedRow;
    public TopMenuItemsReportRowDto? SelectedRow
    {
        get => _selectedRow;
        set { _selectedRow = value; OnPropertyChanged(() => SelectedRow); OnPropertyChanged(() => SelectedShareText); }
    }

    public decimal TotalRevenue => Rows.Sum(r => r.Revenue);
    public int TotalQty => Rows.Sum(r => r.QuantitySold);
    public string SelectedShareText =>
        SelectedRow == null || TotalRevenue <= 0 ? "-" : $"{(SelectedRow.Revenue / TotalRevenue) * 100:0.0}%";

    public ICommand FilterCommand { get; }
    public ICommand ResetCommand { get; }


    private void Load()
    {
        Rows.Clear();

        var from = DateFrom.Date;
        var toEx = DateTo.Date.AddDays(1);

        var data = _context.OrderItems
            .Where(oi => oi.Order.OrderDateTime >= from && oi.Order.OrderDateTime < toEx)
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
            .Take(TopN)
            .ToList();

        foreach (var row in data)
            Rows.Add(row);

        OnPropertyChanged(() => TotalRevenue);
        OnPropertyChanged(() => TotalQty);
        SelectedRow = Rows.FirstOrDefault();
    }
}
