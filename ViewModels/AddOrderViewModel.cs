using CommunityToolkit.Mvvm.Messaging;
using Microsoft.EntityFrameworkCore;
using RestaurantApp.Data;
using RestaurantApp.Dtos;
using RestaurantApp.Entities;
using RestaurantApp.Helper;
using RestaurantApp.Messages;
using System.Collections.ObjectModel;
using System.Windows.Input;

namespace RestaurantApp.ViewModels;

public class AddOrderViewModel : WorkspaceViewModel
{
    private readonly RestaurantDbContext _context = new();

    public ObservableCollection<RestaurantTable> Tables { get; } = new();
    public ObservableCollection<Waiter> Waiters { get; } = new();
    public ObservableCollection<Customer> Customers { get; } = new();
    public ObservableCollection<MenuItem> MenuItems { get; } = new();

    public ObservableCollection<OrderItemDraftDto> Items { get; } = new();

    private RestaurantTable? _selectedTable;
    public RestaurantTable? SelectedTable
    {
        get => _selectedTable;
        set { _selectedTable = value; OnPropertyChanged(() => SelectedTable); RaiseButtons(); }
    }

    private Waiter? _selectedWaiter;
    public Waiter? SelectedWaiter
    {
        get => _selectedWaiter;
        set { _selectedWaiter = value; OnPropertyChanged(() => SelectedWaiter); RaiseButtons(); }
    }

    private Customer? _selectedCustomer;
    public Customer? SelectedCustomer
    {
        get => _selectedCustomer;
        set { _selectedCustomer = value; OnPropertyChanged(() => SelectedCustomer); }
    }

    private MenuItem? _selectedMenuItem;
    public MenuItem? SelectedMenuItem
    {
        get => _selectedMenuItem;
        set { _selectedMenuItem = value; OnPropertyChanged(() => SelectedMenuItem); RaiseButtons(); }
    }

    private int _quantity = 1;
    public int Quantity
    {
        get => _quantity;
        set { _quantity = value; OnPropertyChanged(() => Quantity); RaiseButtons(); }
    }

    private string? _notes;
    public string? Notes
    {
        get => _notes;
        set { _notes = value; OnPropertyChanged(() => Notes); }
    }

    public decimal TotalAmount => Items.Sum(i => i.TotalPrice);

    public ICommand AddItemCommand { get; }
    public ICommand RemoveItemCommand { get; }
    public ICommand SaveCommand { get; }

    private OrderItemDraftDto? _selectedRow;
    public OrderItemDraftDto? SelectedRow
    {
        get => _selectedRow;
        set { _selectedRow = value; OnPropertyChanged(() => SelectedRow); RaiseButtons(); }
    }

    public AddOrderViewModel()
    {
        DisplayName = "Dodaj zamówienie";

        AddItemCommand = new BaseCommand(AddItem, CanAddItem);
        RemoveItemCommand = new BaseCommand(RemoveItem, CanRemoveItem);
        SaveCommand = new BaseCommand(Save, CanSave);

        Items.CollectionChanged += (_, __) =>
        {
            OnPropertyChanged(() => TotalAmount);
            ((BaseCommand)SaveCommand).RaiseCanExecuteChanged();
        };

        LoadLookups();
    }

    private void LoadLookups()
    {
        Tables.Clear();
        Waiters.Clear();
        Customers.Clear();
        MenuItems.Clear();

        foreach (var t in _context.Tables.Where(t => t.IsActive).OrderBy(t => t.TableId))
            Tables.Add(t);

        foreach (var w in _context.Waiters.OrderBy(w => w.WaiterId))
        {
            Waiters.Add(w);
            System.Diagnostics.Debug.WriteLine($"WAITERS COUNT = {Waiters.Count}");
        }

        foreach (var c in _context.Customers.OrderBy(c => c.LastName).ThenBy(c => c.FirstName))
            Customers.Add(c);

        foreach (var m in _context.MenuItems.Where(m => m.IsActive).OrderBy(m => m.Name))
            MenuItems.Add(m);

        SelectedTable = Tables.FirstOrDefault();
        SelectedWaiter = Waiters.FirstOrDefault();
    }

    private bool CanAddItem()
        => SelectedMenuItem != null && Quantity > 0;

    private void AddItem()
    {
        var mi = SelectedMenuItem!;

        // Jeśli już jest taki item w liście, to tylko zwiększ ilość
        var existing = Items.FirstOrDefault(x => x.MenuItemId == mi.MenuItemId);
        if (existing != null)
        {
            existing.Quantity += Quantity;
            // wymuś odświeżenie wiersza w DataGrid (najprościej: usuń i dodaj)
            Items.Remove(existing);
            Items.Add(existing);
        }
        else
        {
            Items.Add(new OrderItemDraftDto
            {
                MenuItemId = mi.MenuItemId,
                MenuItemName = mi.Name,
                Quantity = Quantity,
                UnitPrice = mi.BasePrice
            });
        }

        Quantity = 1;
        OnPropertyChanged(() => TotalAmount);
    }

    private bool CanRemoveItem()
        => SelectedRow != null;

    private void RemoveItem()
    {
        if (SelectedRow == null) return;
        Items.Remove(SelectedRow);
        SelectedRow = null;
        OnPropertyChanged(() => TotalAmount);
    }

    private bool CanSave()
        => SelectedTable != null
           && SelectedWaiter != null
           && Items.Count > 0;

    private void Save()
    {
        // ważne: TotalAmount ustawiamy z Items (nie z UI)
        var order = new Order
        {
            TableId = SelectedTable!.TableId,
            WaiterId = SelectedWaiter!.WaiterId,
            CustomerId = SelectedCustomer?.CustomerId,
            OrderDateTime = DateTime.Now,
            Status = "New",
            Notes = string.IsNullOrWhiteSpace(Notes) ? null : Notes.Trim(),
            TotalAmount = Items.Sum(x => x.TotalPrice)
        };

        _context.Orders.Add(order);
        _context.SaveChanges();

        var orderItems = Items.Select(x => new OrderItem
        {
            OrderId = order.OrderId,
            MenuItemId = x.MenuItemId,
            Quantity = x.Quantity,
            UnitPrice = x.UnitPrice,
            TotalPrice = x.TotalPrice
        }).ToList();

        _context.OrderItems.AddRange(orderItems);
        _context.SaveChanges();

        WeakReferenceMessenger.Default.Send(
            new UiNotificationMessage(
                UiNotificationType.Success,
                $"Zapisano zamówienie #{order.OrderId}"
            )
        );

        OnRequestClose();
    }

    private void RaiseButtons()
    {
        ((BaseCommand)AddItemCommand).RaiseCanExecuteChanged();
        ((BaseCommand)RemoveItemCommand).RaiseCanExecuteChanged();
        ((BaseCommand)SaveCommand).RaiseCanExecuteChanged();
    }
}
