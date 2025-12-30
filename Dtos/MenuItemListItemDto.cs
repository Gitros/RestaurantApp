namespace RestaurantApp.Dtos;

public class MenuItemListItemDto
{
    public int MenuItemId { get; set; }

    public int MenuCategoryId { get; set; }
    public string MenuCategoryName { get; set; } = "";

    public string Name { get; set; } = "";
    public decimal BasePrice { get; set; }
    public bool IsActive { get; set; }
}
