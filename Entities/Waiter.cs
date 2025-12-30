namespace RestaurantApp.Entities;

public partial class Waiter
{
    public int WaiterId { get; set; }
    public string FirstName { get; set; } = null!;
    public string LastName { get; set; } = null!;
    public string? Phone { get; set; }
    public DateTime HireDate { get; set; }
    public bool IsActive { get; set; }

    public virtual ICollection<Order> Orders { get; set; } = new List<Order>();
    public virtual ICollection<Shift> Shifts { get; set; } = new List<Shift>();
}
