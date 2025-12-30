namespace RestaurantApp.Entities;

public partial class Restaurant
{
    public int RestaurantId { get; set; }
    public string Name { get; set; } = null!;
    public string Address { get; set; } = null!;
    public string? Phone { get; set; }
    public bool IsActive { get; set; }

    public virtual ICollection<Area> Areas { get; set; } = new List<Area>();
}
