namespace TailorApp.Models;

public class Customer
{
    public int CustomerID { get; set; }
    public string CustomerName { get; set; } = string.Empty;
    public string? MobileNo1 { get; set; }
    public string? MobileNo2 { get; set; }
    public bool Status { get; set; }

    // Computed
    public int TotalMeasurements { get; set; }
    public string? RegisterNo { get; set; }
}
