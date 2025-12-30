namespace RestaurantApp.Entities;

public partial class MenuCategory
{
    public int MenuCategoryId { get; set; }
    public string Name { get; set; } = null!;
    public int? ParentCategoryId { get; set; }

    public virtual MenuCategory? ParentCategory { get; set; }
    public virtual ICollection<MenuCategory> SubCategories { get; set; } = new List<MenuCategory>();
    public virtual ICollection<MenuItem> MenuItems { get; set; } = new List<MenuItem>();
}
