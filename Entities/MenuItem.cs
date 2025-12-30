namespace RestaurantApp.Entities;

public partial class MenuItem
{
    public int MenuItemId { get; set; }
    public int MenuCategoryId { get; set; }
    public string Name { get; set; } = null!;
    public string? Description { get; set; }
    public decimal BasePrice { get; set; }
    public bool IsActive { get; set; }

    public virtual MenuCategory MenuCategory { get; set; } = null!;
    public virtual ICollection<OrderItem> OrderItems { get; set; } = new List<OrderItem>();
    public virtual ICollection<MenuItemIngredient> MenuItemIngredients { get; set; } = new List<MenuItemIngredient>();
}
