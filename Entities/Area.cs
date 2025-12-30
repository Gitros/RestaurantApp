namespace RestaurantApp.Entities;

public partial class Area
{
    public int AreaId { get; set; }
    public int RestaurantId { get; set; }
    public string Name { get; set; } = null!;
    public string? Description { get; set; }

    public virtual Restaurant Restaurant { get; set; } = null!;
    public virtual ICollection<RestaurantTable> Tables { get; set; } = new List<RestaurantTable>();
}
