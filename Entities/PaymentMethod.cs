namespace RestaurantApp.Entities;

public partial class PaymentMethod
{
    public int PaymentMethodId { get; set; }
    public string Name { get; set; } = null!;
    public bool IsActive { get; set; }

    public virtual ICollection<Payment> Payments { get; set; } = new List<Payment>();
}
