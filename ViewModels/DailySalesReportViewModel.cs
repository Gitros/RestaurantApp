using Microsoft.EntityFrameworkCore;
using RestaurantApp.Data;
using RestaurantApp.Dtos;
using RestaurantApp.Helper;
using System.Collections.ObjectModel;
using System.Windows.Input;

namespace RestaurantApp.ViewModels;

public class DailySalesReportViewModel : WorkspaceViewModel
{
    private readonly RestaurantDbContext _context = new();

    public ObservableCollection<DailySalesReportRowDto> Rows { get; } = new();

    private DateTime _dateFrom = DateTime.Today.AddDays(-7);
    public DateTime DateFrom
    {
        get => _dateFrom;
        set
        {
            _dateFrom = value;
            OnPropertyChanged(() => DateFrom);
            ((BaseCommand)FilterCommand).RaiseCanExecuteChanged();
        }
    }

    private DateTime _dateTo = DateTime.Today;
    public DateTime DateTo
    {
        get => _dateTo;
        set
        {
            _dateTo = value;
            OnPropertyChanged(() => DateTo);
            ((BaseCommand)FilterCommand).RaiseCanExecuteChanged();
        }
    }

    public ICommand FilterCommand { get; }
    public ICommand ResetCommand { get; }

    public DailySalesReportViewModel()
    {
        DisplayName = "Raport: Sprzedaż dzienna";

        FilterCommand = new BaseCommand(ApplyFilter, CanFilter);
        ResetCommand = new BaseCommand(Reset);

        Load(DateTime.Today.AddYears(-5), DateTime.Today);
    }

    private bool CanFilter()
        => DateFrom.Date <= DateTo.Date;

    private void ApplyFilter()
        => Load(DateFrom, DateTo);

    private void Reset()
    {
        DateFrom = DateTime.Today.AddDays(-7);
        DateTo = DateTime.Today;

        Load(DateFrom, DateTo);
    }

    private void Load(DateTime from, DateTime to)
    {
        Rows.Clear();

        var fromDate = from.Date;
        var toExclusive = to.Date.AddDays(1);

        var data = _context.Payments
            .AsNoTracking()
            .Where(p => p.Status == "Paid")
            .Where(p => p.PaymentDateTime >= fromDate && p.PaymentDateTime < toExclusive)
            .GroupBy(p => p.PaymentDateTime.Date)
            .Select(g => new DailySalesReportRowDto
            {
                Date = g.Key,
                Revenue = g.Sum(x => x.Amount),
                OrdersCount = g.Select(x => x.OrderId).Distinct().Count(),
                AvgOrderValue =
                    g.Select(x => x.OrderId).Distinct().Count() == 0
                        ? 0
                        : g.Sum(x => x.Amount) / g.Select(x => x.OrderId).Distinct().Count()
            })
            .OrderByDescending(x => x.Date)
            .ToList();

        foreach (var row in data)
            Rows.Add(row);
    }
}
