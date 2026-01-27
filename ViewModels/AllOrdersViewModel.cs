using CommunityToolkit.Mvvm.Messaging;
using Microsoft.EntityFrameworkCore;
using RestaurantApp.Data;
using RestaurantApp.Dtos;
using RestaurantApp.Helper;
using RestaurantApp.Messages;
using System.Collections.ObjectModel;
using System.Windows.Input;

namespace RestaurantApp.ViewModels;

public class AllOrdersViewModel : WorkspaceViewModel
{
    private readonly RestaurantDbContext _context = new();

    public ObservableCollection<OrderListItemDto> Orders { get; } = new();

    private OrderListItemDto? _selectedOrder;
    public OrderListItemDto? SelectedOrder
    {
        get => _selectedOrder;
        set
        {
            _selectedOrder = value;
            OnPropertyChanged(() => SelectedOrder);
            ((BaseCommand)CloseOrderCommand).RaiseCanExecuteChanged();
        }
    }

    public ICommand AddOrderCommand { get; }
    public ICommand CloseOrderCommand { get; }

    public event Action? RequestAddOrder;

    public AllOrdersViewModel()
    {
        DisplayName = "Zamówienia";

        AddOrderCommand = new BaseCommand(() => RequestAddOrder?.Invoke());
        CloseOrderCommand = new BaseCommand(CloseOrder, CanCloseOrder);

        WeakReferenceMessenger.Default.Register<OrderSavedMessage>(this, (_, __) => Refresh());

        LoadOrders();
    }

    private void LoadOrders()
    {
        Orders.Clear();

        var data = _context.Orders
            //.Include(o => o.Table)
            //.Include(o => o.Waiter)
            //.Include(o => o.Customer)
            .OrderByDescending(o => o.OrderDateTime)
            .Select(o => new OrderListItemDto
            {
                OrderId = o.OrderId,
                OrderDateTime = o.OrderDateTime,
                Status = o.Status,
                TotalAmount = o.TotalAmount,

                TableName = o.Table.Name,

                WaiterName = (o.Waiter.FirstName + " " + o.Waiter.LastName),

                CustomerName = o.Customer == null
                    ? null
                    : (o.Customer.FirstName + " " + o.Customer.LastName)
            })
            .ToList();

        foreach (var row in data)
            Orders.Add(row);
    }

    public void Refresh() => LoadOrders();

    private bool CanCloseOrder()
        => SelectedOrder != null && SelectedOrder.Status != "Closed";

    private void CloseOrder()
    {
        if (SelectedOrder == null) return;

        var order = _context.Orders.FirstOrDefault(o => o.OrderId == SelectedOrder.OrderId);
        if (order == null) return;

        order.Status = "Closed";
        _context.SaveChanges();

        WeakReferenceMessenger.Default.Send(
    new UiNotificationMessage(
        UiNotificationType.Success,
        $"Zamówienie #{order.OrderId} zostało zamknięte"
    )
);

        LoadOrders();
    }
}
