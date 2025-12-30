namespace RestaurantApp.Dtos;

public class PaymentListItemDto
{
    public int PaymentId { get; set; }
    public int OrderId { get; set; }
    public string PaymentMethodName { get; set; } = "";
    public decimal Amount { get; set; }
    public DateTime PaymentDateTime { get; set; }
    public string Status { get; set; } = "";
}
