namespace RestaurantApp.Dtos;

public class TopMenuItemsReportRowDto
{
    public int MenuItemId { get; set; }
    public string MenuItemName { get; set; } = "";
    public string CategoryName { get; set; } = "";

    public int QuantitySold { get; set; }
    public decimal Revenue { get; set; }
}
