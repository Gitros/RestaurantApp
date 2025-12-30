namespace RestaurantApp.Dtos;

public class TableUsageReportRowDto
{
    public int TableId { get; set; }
    public string AreaName { get; set; } = "";
    public string TableName { get; set; } = "";

    public int SeatsCount { get; set; }
    public bool IsActive { get; set; }

    public int OrdersCount { get; set; }
    public int ReservationsCount { get; set; }
    public decimal OrdersRevenue { get; set; }
}
