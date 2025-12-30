namespace RestaurantApp.Entities;

public partial class Ingredient
{
    public int IngredientId { get; set; }
    public string Name { get; set; } = null!;
    public bool IsAllergen { get; set; }
    public string Unit { get; set; } = null!;

    public virtual ICollection<MenuItemIngredient> MenuItemIngredients { get; set; } = new List<MenuItemIngredient>();
}
