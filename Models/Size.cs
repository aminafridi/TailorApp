using System.ComponentModel.DataAnnotations;

namespace TailorApp.Models;

public class Size
{
    public int SizeID { get; set; }
    public int Customer_ID { get; set; }
    public int RegisterNo { get; set; }

    // Kameez Measurements
    [StringLength(10, ErrorMessage = "Max 10 characters.")]
    public string Lambai { get; set; } = string.Empty;      // Length
    [StringLength(10, ErrorMessage = "Max 10 characters.")]
    public string Bazo { get; set; } = string.Empty;        // Sleeve
    [Range(1, 2, ErrorMessage = "Select a sleeve style.")]
    public int BazoType { get; set; }                       // Sleeve Type
    [StringLength(200, ErrorMessage = "Max 200 characters.")]
    public string? BazoDetail { get; set; }                 // Sleeve Detail
    [StringLength(10, ErrorMessage = "Max 10 characters.")]
    public string Tera { get; set; } = string.Empty;        // Shoulder
    [StringLength(10, ErrorMessage = "Max 10 characters.")]
    public string Calar { get; set; } = string.Empty;       // Collar
    [Range(1, 2, ErrorMessage = "Select a collar style.")]
    public int CalarType { get; set; }                      // Collar Type
    [StringLength(200, ErrorMessage = "Max 200 characters.")]
    public string? CalarDetail { get; set; }                // Collar Detail
    [StringLength(10, ErrorMessage = "Max 10 characters.")]
    public string Chati { get; set; } = string.Empty;       // Chest
    [StringLength(10, ErrorMessage = "Max 10 characters.")]
    public string Kamar { get; set; } = string.Empty;       // Waist
    [StringLength(10, ErrorMessage = "Max 10 characters.")]
    public string Ghera { get; set; } = string.Empty;       // Hem / Width
    [Range(1, 2, ErrorMessage = "Select a hem style.")]
    public int GheraType { get; set; }                      // Hem Type

    // Shalwar Measurements
    [StringLength(10, ErrorMessage = "Max 10 characters.")]
    public string ShalwarLambai { get; set; } = string.Empty; // Shalwar Length
    [StringLength(50, ErrorMessage = "Max 50 characters.")]
    public string Pancha { get; set; } = string.Empty;        // Trouser Bottom

    // Options / Pocket Flags
    public bool IsDoubleSidePocket { get; set; }
    public bool IsFrontPocket { get; set; }
    public bool IsShalwarPocket { get; set; }
    public bool IsCheckPatiKaj { get; set; }

    // Additional
    [StringLength(10, ErrorMessage = "Max 10 characters.")]
    public string? Pati { get; set; }           // Placket
    [StringLength(500, ErrorMessage = "Max 500 characters.")]
    public string? Design { get; set; }         // Design Notes
    [StringLength(500, ErrorMessage = "Max 500 characters.")]
    public string? OtherDetails { get; set; }   // Other Notes

    // Navigation
    public string CustomerName { get; set; } = string.Empty;
    public string? MobileNo1 { get; set; }
    public string? MobileNo2 { get; set; }

    // Helpers
    public string BazoTypeName => BazoType switch
    {
        1 => "Sada (Simple)",
        2 => "Kaf",
        _ => BazoType.ToString()
    };

    public string CalarTypeName => CalarType switch
    {
        1 => "Calar",
        2 => "Ban",
        _ => CalarType.ToString()
    };

    public string GheraTypeName => GheraType switch
    {
        1 => "Gol (Round)",
        2 => "Choras (Square)",
        _ => GheraType.ToString()
    };
}
