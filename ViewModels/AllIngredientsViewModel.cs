using RestaurantApp.Data;
using RestaurantApp.Entities;
using System.Collections.ObjectModel;

namespace RestaurantApp.ViewModels;

public class AllIngredientsViewModel : WorkspaceViewModel
{
    private readonly RestaurantDbContext _context = new();

    public ObservableCollection<Ingredient> Ingredients { get; } = new();

    public AllIngredientsViewModel()
    {
        base.DisplayName = "Składniki";
        LoadIngredients();
    }

    private void LoadIngredients()
    {
        Ingredients.Clear();

        var data = _context.Ingredients
            .OrderBy(i => i.IngredientId)
            .ToList();

        foreach (var ingredient in data)
            Ingredients.Add(ingredient);
    }
}
