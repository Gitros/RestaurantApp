namespace RestaurantApp.Entities;

public partial class Order
{
    public int OrderId { get; set; }
    public int TableId { get; set; }
    public int? CustomerId { get; set; }
    public int WaiterId { get; set; }
    public DateTime OrderDateTime { get; set; }
    public string Status { get; set; } = null!;
    public decimal TotalAmount { get; set; }
    public string? Notes { get; set; }

    public virtual RestaurantTable Table { get; set; } = null!;
    public virtual Customer? Customer { get; set; }
    public virtual Waiter Waiter { get; set; } = null!;
    public virtual ICollection<OrderItem> OrderItems { get; set; } = new List<OrderItem>();
    public virtual ICollection<Payment> Payments { get; set; } = new List<Payment>();
}
