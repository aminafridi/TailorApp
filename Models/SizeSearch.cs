namespace TailorApp.Models;

// The three search boxes on the Measurements page. Empty boxes are ignored;
// filled ones are combined, so name + mobile narrows the list further.
public class SizeSearch
{
    public string? Name { get; set; }        // part of the customer name
    public int? RegisterNo { get; set; }     // exact register number
    public string? Mobile { get; set; }      // part of either mobile number, digits only

    public bool IsEmpty => string.IsNullOrEmpty(Name) && RegisterNo is null && string.IsNullOrEmpty(Mobile);
}
