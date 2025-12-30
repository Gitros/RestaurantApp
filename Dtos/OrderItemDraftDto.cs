namespace RestaurantApp.Dtos;

public class OrderItemDraftDto
{
    public int MenuItemId { get; set; }
    public string MenuItemName { get; set; } = "";
    public int Quantity { get; set; }
    public decimal UnitPrice { get; set; }
    public decimal TotalPrice => UnitPrice * Quantity;
}
