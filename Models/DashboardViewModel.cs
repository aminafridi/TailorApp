namespace TailorApp.Models;

public class DashboardViewModel
{
    public int TotalCustomers { get; set; }
    public int ActiveCustomers { get; set; }
    public int TotalMeasurements { get; set; }
    public List<Customer> RecentCustomers { get; set; } = new();
}
