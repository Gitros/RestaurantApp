namespace RestaurantApp.Entities;

public partial class OrderItem
{
    public int OrderItemId { get; set; }
    public int OrderId { get; set; }
    public int MenuItemId { get; set; }
    public int Quantity { get; set; }
    public decimal UnitPrice { get; set; }
    public decimal TotalPrice { get; set; }
    public string? Notes { get; set; }

    public virtual Order Order { get; set; } = null!;
    public virtual MenuItem MenuItem { get; set; } = null!;
}
