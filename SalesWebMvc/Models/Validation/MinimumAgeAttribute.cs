using System.ComponentModel.DataAnnotations;
using Microsoft.AspNetCore.Mvc.ModelBinding.Validation;

namespace SalesWebMvc.Models.Validation;

/// <summary>
/// Garante que a data informada corresponda a uma idade mínima (ex.: maior de 18 anos)
/// e que não seja absurda (idade máxima). Validado no servidor e no cliente (jQuery Validation).
/// </summary>
[AttributeUsage(AttributeTargets.Property)]
public class MinimumAgeAttribute : ValidationAttribute, IClientModelValidator
{
    public int MinimumAge { get; }
    public int MaximumAge { get; }

    public MinimumAgeAttribute(int minimumAge, int maximumAge = 100)
    {
        MinimumAge = minimumAge;
        MaximumAge = maximumAge;
        ErrorMessage = $"O vendedor deve ter entre {minimumAge} e {maximumAge} anos.";
    }

    public override bool IsValid(object? value)
    {
        if (value is not DateTime birthDate) return true; // [Required] cuida do valor ausente

        var today = DateTime.Today;
        var age = today.Year - birthDate.Year;
        if (birthDate.Date > today.AddYears(-age)) age--;

        return age >= MinimumAge && age <= MaximumAge;
    }

    public void AddValidation(ClientModelValidationContext context)
    {
        context.Attributes.TryAdd("data-val", "true");
        context.Attributes.TryAdd("data-val-minage", FormatErrorMessage(context.ModelMetadata.GetDisplayName()));
        context.Attributes.TryAdd("data-val-minage-min", MinimumAge.ToString());
        context.Attributes.TryAdd("data-val-minage-max", MaximumAge.ToString());
    }
}
