using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using Microsoft.AspNetCore.Mvc;

namespace fifa.Models
{
    public class Club : IValidatableObject

    {
    public int Id { get; set; }
    public string Name { get; set; }
    [Remote( "ValidateLogo", "Validator", ErrorMessage = "Please don't use this logo")]
    public string Logo { get; set; }
    public int LeagueId { get; set; }
    public IEnumerable<ValidationResult> Validate(ValidationContext validationContext)
    {
        if (this.Name == "Toronto Club Air Astana")
        {
            yield return new ValidationResult("Are you crazy?");
        }
    }
    }
}