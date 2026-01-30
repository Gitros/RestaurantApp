using RestaurantApp.Data;
using RestaurantApp.Dtos;
using RestaurantApp.Helper;
using System.Collections.ObjectModel;
using System.ComponentModel;
using System.Windows.Data;
using System.Windows.Input;

namespace RestaurantApp.ViewModels;

public class TableUsageReportViewModel : WorkspaceViewModel
{
    private readonly RestaurantDbContext _context = new();

    public ObservableCollection<TableUsageReportRowDto> Rows { get; } = new();

    // do DataGrid
    public ICollectionView RowsView { get; }

    public ICommand FilterCommand { get; }
    public ICommand ResetCommand { get; }

    // --- FILTRY ---
    private string _areaFilter = "";
    public string AreaFilter
    {
        get => _areaFilter;
        set
        {
            _areaFilter = value ?? "";
            OnPropertyChanged(() => AreaFilter);
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
        }
    }

    private int _minOrders;
    public int MinOrders
    {
        get => _minOrders;
        set
        {
            _minOrders = value;
            OnPropertyChanged(() => MinOrders);
        }
    }

    private int _minReservations;
    public int MinReservations
    {
        get => _minReservations;
        set
        {
            _minReservations = value;
            OnPropertyChanged(() => MinReservations);
        }
    }

    // --- SELECTED ---
    private TableUsageReportRowDto? _selectedRow;
    public TableUsageReportRowDto? SelectedRow
    {
        get => _selectedRow;
        set
        {
            _selectedRow = value;
            OnPropertyChanged(() => SelectedRow);
            OnPropertyChanged(() => SelectedOrdersShareText);
            OnPropertyChanged(() => SelectedRevenueShareText);
        }
    }

    private bool _showErrors = false;
    public bool ShowErrors
    {
        get => _showErrors;
        set
        {
            _showErrors = value;
            OnPropertyChanged(() => ShowErrors);
        }
    }


    // --- PODSUMOWANIA (na kafelki) ---
    public int TotalTables => RowsView.Cast<TableUsageReportRowDto>().Count();
    public int TotalOrders => RowsView.Cast<TableUsageReportRowDto>().Sum(r => r.OrdersCount);
    public int TotalReservations => RowsView.Cast<TableUsageReportRowDto>().Sum(r => r.ReservationsCount);
    public decimal TotalRevenue => RowsView.Cast<TableUsageReportRowDto>().Sum(r => r.OrdersRevenue);

    public string SelectedOrdersShareText =>
        SelectedRow == null || TotalOrders <= 0 ? "-" : $"{(SelectedRow.OrdersCount / (double)TotalOrders) * 100:0.0}%";

    public string SelectedRevenueShareText =>
        SelectedRow == null || TotalRevenue <= 0 ? "-" : $"{(SelectedRow.OrdersRevenue / TotalRevenue) * 100:0.0}%";

    public string FilteredCountText => $"Stolików: {TotalTables}";

    public ICommand QuickActiveCommand { get; }
    public ICommand QuickHasOrdersCommand { get; }

    public TableUsageReportViewModel()
    {
        DisplayName = "Raport: obłożenie stolików";

        RowsView = CollectionViewSource.GetDefaultView(Rows);
        RowsView.Filter = FilterRow;

        FilterCommand = new BaseCommand(ApplyFilter);
        ResetCommand = new BaseCommand(Reset);

        QuickActiveCommand = new BaseCommand(() =>
        {
            OnlyActive = true;
            ApplyFilter();
        });

        QuickHasOrdersCommand = new BaseCommand(() =>
        {
            MinOrders = 1;
            ApplyFilter();
        });

        Load();
        ApplyFilter();
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

        SelectedRow = Rows.FirstOrDefault();
    }

    private void ApplyFilter()
    {
        RowsView.Refresh();
        RefreshSummary();
    }

    private void Reset()
    {
        AreaFilter = "";
        OnlyActive = false;
        MinOrders = 0;
        MinReservations = 0;

        ApplyFilter();
    }

    private bool FilterRow(object obj)
    {
        if (obj is not TableUsageReportRowDto row) return false;

        if (OnlyActive && !row.IsActive)
            return false;

        if (!string.IsNullOrWhiteSpace(AreaFilter))
        {
            if (string.IsNullOrWhiteSpace(row.AreaName) ||
                !row.AreaName.Contains(AreaFilter, StringComparison.OrdinalIgnoreCase))
                return false;
        }

        if (row.OrdersCount < MinOrders)
            return false;

        if (row.ReservationsCount < MinReservations)
            return false;

        return true;
    }

    private void RefreshSummary()
    {
        OnPropertyChanged(() => TotalTables);
        OnPropertyChanged(() => TotalOrders);
        OnPropertyChanged(() => TotalReservations);
        OnPropertyChanged(() => TotalRevenue);
        OnPropertyChanged(() => FilteredCountText);
        OnPropertyChanged(() => SelectedOrdersShareText);
        OnPropertyChanged(() => SelectedRevenueShareText);
    }
}
