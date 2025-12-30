using RestaurantApp.Data;
using RestaurantApp.Dtos;
using RestaurantApp.Entities;
using System.Collections.ObjectModel;
using System.Linq;

namespace RestaurantApp.ViewModels;

public class AllPaymentsViewModel : WorkspaceViewModel
{
    private readonly RestaurantDbContext _context = new();

    public ObservableCollection<PaymentListItemDto> Payments { get; } = new();

    public AllPaymentsViewModel()
    {
        base.DisplayName = "Płatności";
        LoadPayments();
    }

    private void LoadPayments()
    {
        Payments.Clear();

        var data = _context.Payments
                    .OrderByDescending(p => p.PaymentDateTime)
                    .Select(p => new PaymentListItemDto
                    {
                        PaymentId = p.PaymentId,
                        OrderId = p.OrderId,
                        PaymentMethodName = p.PaymentMethod.Name,
                        Amount = p.Amount,
                        PaymentDateTime = p.PaymentDateTime,
                        Status = p.Status
                    })
                    .ToList();

        foreach (var row in data)
            Payments.Add(row);
    }
}
