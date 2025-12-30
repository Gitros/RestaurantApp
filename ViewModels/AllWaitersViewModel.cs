using RestaurantApp.Data;
using RestaurantApp.Entities;
using System.Collections.ObjectModel;
using System.Linq;

namespace RestaurantApp.ViewModels;

public class AllWaitersViewModel : WorkspaceViewModel
{
    private readonly RestaurantDbContext _context = new();

    public ObservableCollection<Waiter> Waiters { get; } = new();

    public AllWaitersViewModel()
    {
        base.DisplayName = "Kelnerzy";
        LoadWaiters();
    }

    private void LoadWaiters()
    {
        Waiters.Clear();

        var data = _context.Waiters
            .OrderBy(w => w.WaiterId)
            .ToList();

        foreach (var w in data)
            Waiters.Add(w);
    }
}
