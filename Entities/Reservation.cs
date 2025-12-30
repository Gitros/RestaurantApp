namespace RestaurantApp.Entities;

public partial class Reservation
{
    public int ReservationId { get; set; }
    public int TableId { get; set; }
    public int CustomerId { get; set; }
    public DateTime ReservationDateTime { get; set; }
    public int GuestsCount { get; set; }
    public string Status { get; set; } = null!;
    public string? Notes { get; set; }

    public virtual RestaurantTable Table { get; set; } = null!;
    public virtual Customer Customer { get; set; } = null!;
}
