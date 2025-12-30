namespace RestaurantApp.Dtos;

public class AreaListItemDto
{
    public int AreaId { get; set; }
    public int RestaurantId { get; set; }
    public string RestaurantName { get; set; } = "";
    public string Name { get; set; } = "";
    public string? Description { get; set; }
}
