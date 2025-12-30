namespace RestaurantApp.Entities;

public partial class Shift
{
    public int ShiftId { get; set; }
    public int WaiterId { get; set; }
    public DateTime StartDateTime { get; set; }
    public DateTime EndDateTime { get; set; }

    public virtual Waiter Waiter { get; set; } = null!;
}
