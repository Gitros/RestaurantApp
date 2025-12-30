namespace RestaurantApp.Dtos;

public class DailySalesReportRowDto
{
    public DateTime Date { get; set; }
    public int OrdersCount { get; set; }
    public decimal Revenue { get; set; }
    public decimal AvgOrderValue { get; set; }
}
