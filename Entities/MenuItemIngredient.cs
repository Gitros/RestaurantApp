namespace RestaurantApp.Entities;

public partial class MenuItemIngredient
{
    public int MenuItemId { get; set; }
    public int IngredientId { get; set; }
    public decimal Quantity { get; set; }

    public virtual MenuItem MenuItem { get; set; } = null!;
    public virtual Ingredient Ingredient { get; set; } = null!;
}
