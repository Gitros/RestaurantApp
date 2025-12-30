using CommunityToolkit.Mvvm.Messaging;
using RestaurantApp.Data;
using RestaurantApp.Entities;
using RestaurantApp.Helper;
using RestaurantApp.Messages;
using System.Collections.ObjectModel;
using System.Windows.Input;

namespace RestaurantApp.ViewModels;

public class AllOrdersViewModel : WorkspaceViewModel
{
    private readonly RestaurantDbContext _context = new RestaurantDbContext();

    public ObservableCollection<Order> Orders { get; set; } = new();

    public event Action? RequestAddOrder;
    public ICommand AddOrderCommand { get; }

    public AllOrdersViewModel()
    {
        DisplayName = "Zamówienia";

        AddOrderCommand = new BaseCommand(() => RequestAddOrder?.Invoke());


        WeakReferenceMessenger.Default.Register<OrderSavedMessage>(this, (_, msg) =>
        {
            Refresh();
        });

        LoadOrders();
    }

    private void LoadOrders()
    {
        Orders.Clear();

        var data = _context.Orders
            .OrderByDescending(o => o.OrderDateTime)
            .ToList();

        foreach (var order in data)
        {
            Orders.Add(order);
        }
    }

    public void Refresh() => LoadOrders();
}