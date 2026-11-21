using System.ComponentModel.DataAnnotations;

namespace CarRentalServices.Entities;

/// <summary>ISO country catalog entry returned by the country stored procedures.</summary>
public sealed class Country
{
    [Key]
    public int CountryId { get; set; }

    [Required, StringLength(2, MinimumLength = 2)]
    public string Iso2Code { get; set; } = string.Empty;

    [Required, StringLength(3, MinimumLength = 3)]
    public string Iso3Code { get; set; } = string.Empty;

    [Required, StringLength(100)]
    public string CountryName { get; set; } = string.Empty;
}
