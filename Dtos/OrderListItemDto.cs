namespace RestaurantApp.Dtos;

public class OrderListItemDto
{
    public int OrderId { get; set; }
    public DateTime OrderDateTime { get; set; }
    public string Status { get; set; } = "";
    public decimal TotalAmount { get; set; }

    public string TableName { get; set; } = "";
    public string WaiterName { get; set; } = "";
    public string? CustomerName { get; set; }
}
