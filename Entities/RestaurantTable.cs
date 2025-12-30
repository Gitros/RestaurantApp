namespace RestaurantApp.Entities;

public partial class RestaurantTable
{
    public int TableId { get; set; }
    public int AreaId { get; set; }
    public string Name { get; set; } = null!;
    public int SeatsCount { get; set; }
    public bool IsActive { get; set; }

    public virtual Area Area { get; set; } = null!;
    public virtual ICollection<Order> Orders { get; set; } = new List<Order>();
    public virtual ICollection<Reservation> Reservations { get; set; } = new List<Reservation>();
}

