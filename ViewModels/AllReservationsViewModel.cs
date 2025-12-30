using RestaurantApp.Data;
using RestaurantApp.Entities;
using System.Collections.ObjectModel;

namespace RestaurantApp.ViewModels;

public class AllReservationsViewModel : WorkspaceViewModel
{
    private readonly RestaurantDbContext _context = new();

    public ObservableCollection<Reservation> Reservations { get; } = new();

    public AllReservationsViewModel()
    {
        base.DisplayName = "Rezerwacje";
        LoadReservations();
    }

    private void LoadReservations()
    {
        Reservations.Clear();

        var data = _context.Reservations
            .OrderByDescending(r => r.ReservationDateTime)
            .ToList();

        foreach (var r in data)
            Reservations.Add(r);
    }
}
