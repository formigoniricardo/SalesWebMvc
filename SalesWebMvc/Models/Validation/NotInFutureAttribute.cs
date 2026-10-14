using System.ComponentModel.DataAnnotations;
using Microsoft.AspNetCore.Mvc.ModelBinding.Validation;

namespace SalesWebMvc.Models.Validation;

/// <summary>
/// Impede datas futuras (ex.: uma venda não pode ser registrada para amanhã).
/// Validado no servidor e no cliente (jQuery Validation).
/// </summary>
[AttributeUsage(AttributeTargets.Property)]
public class NotInFutureAttribute : ValidationAttribute, IClientModelValidator
{
    public NotInFutureAttribute()
    {
        ErrorMessage = "{0} não pode ser uma data futura.";
    }

    public override bool IsValid(object? value)
    {
        return value is not DateTime date || date.Date <= DateTime.Today;
    }

    public void AddValidation(ClientModelValidationContext context)
    {
        context.Attributes.TryAdd("data-val", "true");
        context.Attributes.TryAdd("data-val-notfuture", FormatErrorMessage(context.ModelMetadata.GetDisplayName()));
    }
}
