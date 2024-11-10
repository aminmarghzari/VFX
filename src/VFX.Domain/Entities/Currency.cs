using System.ComponentModel.DataAnnotations;
using VFX.Application.Common.Models;

namespace VFX.Domain.Entities;

// Represents a currency entity in the system
public class Currency : BaseModel
{
    // The 3-character code for the currency (e.g., "USD", "EUR")
    [Required]
    [StringLength(3, ErrorMessage = "Code must be 3 characters or less.")]
    public string Code { get; set; }

    // The name of the currency (e.g., "United States Dollar", "Euro")
    [Required]
    public string Name { get; set; }
}
