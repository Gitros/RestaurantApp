namespace RestaurantApp.Dtos;

public class TableListItemDto
{
    public int TableId { get; set; }
    public string Name { get; set; } = "";
    public int SeatsCount { get; set; }
    public bool IsActive { get; set; }

    public int AreaId { get; set; }
    public string AreaName { get; set; } = "";
}
