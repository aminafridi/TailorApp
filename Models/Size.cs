namespace TailorApp.Models;

public class Size
{
    public int SizeID { get; set; }
    public int Customer_ID { get; set; }
    public int RegisterNo { get; set; }

    // Kameez Measurements
    public string Lambai { get; set; } = string.Empty;      // Length
    public string Bazo { get; set; } = string.Empty;        // Sleeve
    public int BazoType { get; set; }                       // Sleeve Type
    public string? BazoDetail { get; set; }                 // Sleeve Detail
    public string Tera { get; set; } = string.Empty;        // Shoulder
    public string Calar { get; set; } = string.Empty;       // Collar
    public int CalarType { get; set; }                      // Collar Type
    public string? CalarDetail { get; set; }                // Collar Detail
    public string Chati { get; set; } = string.Empty;       // Chest
    public string Kamar { get; set; } = string.Empty;       // Waist
    public string Ghera { get; set; } = string.Empty;       // Hem / Width
    public int GheraType { get; set; }                      // Hem Type

    // Shalwar Measurements
    public string ShalwarLambai { get; set; } = string.Empty; // Shalwar Length
    public string Pancha { get; set; } = string.Empty;        // Trouser Bottom

    // Options / Pocket Flags
    public bool IsDoubleSidePocket { get; set; }
    public bool IsFrontPocket { get; set; }
    public bool IsShalwarPocket { get; set; }
    public bool IsCheckPatiKaj { get; set; }

    // Additional
    public string? Pati { get; set; }           // Placket
    public string? Design { get; set; }         // Design Notes
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
