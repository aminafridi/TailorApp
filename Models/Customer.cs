using System.ComponentModel.DataAnnotations;

namespace TailorApp.Models;

public class Customer
{
    public int CustomerID { get; set; }
    [StringLength(50, ErrorMessage = "Name can be at most 50 characters.")]
    public string CustomerName { get; set; } = string.Empty;
    [StringLength(20, ErrorMessage = "Mobile number can be at most 20 characters.")]
    public string? MobileNo1 { get; set; }
    [StringLength(20, ErrorMessage = "Mobile number can be at most 20 characters.")]
    public string? MobileNo2 { get; set; }
    public bool Status { get; set; }

    // Computed
    public int TotalMeasurements { get; set; }
    public string? RegisterNo { get; set; }
}
