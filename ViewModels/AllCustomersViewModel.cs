using RestaurantApp.Data;
using RestaurantApp.Entities;
using System.Collections.ObjectModel;

namespace RestaurantApp.ViewModels;

public class AllCustomersViewModel : WorkspaceViewModel
{
    private readonly RestaurantDbContext _context = new();

    public ObservableCollection<Customer> Customers { get; } = new();

    public AllCustomersViewModel()
    {
        base.DisplayName = "Klienci";
        LoadCustomers();
    }

    private void LoadCustomers()
    {
        Customers.Clear();

        var data = _context.Customers
            .OrderBy(c => c.LastName)
            .ThenBy(c => c.FirstName)
            .ToList();

        foreach (var c in data)
            Customers.Add(c);
    }
}
