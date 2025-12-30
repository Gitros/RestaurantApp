namespace RestaurantApp.Entities;

public partial class Customer
{
    public int CustomerId { get; set; }
    public string FirstName { get; set; } = null!;
    public string LastName { get; set; } = null!;
    public string? Phone { get; set; }
    public string? Email { get; set; }

    public virtual ICollection<Order> Orders { get; set; } = new List<Order>();
    public virtual ICollection<Reservation> Reservations { get; set; } = new List<Reservation>();
}
