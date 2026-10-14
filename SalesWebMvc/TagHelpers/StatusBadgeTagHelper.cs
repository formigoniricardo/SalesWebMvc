using Microsoft.AspNetCore.Razor.TagHelpers;
using SalesWebMvc.Helpers;
using SalesWebMvc.Models.Enums;

namespace SalesWebMvc.TagHelpers;

/// <summary>
/// Uso: &lt;status-badge status="@item.Status" /&gt; — renderiza um badge colorido com ícone + texto
/// (a cor nunca carrega o significado sozinha).
/// </summary>
[HtmlTargetElement("status-badge", TagStructure = TagStructure.WithoutEndTag)]
public class StatusBadgeTagHelper : TagHelper
{
    public SaleStatus Status { get; set; }

    public override void Process(TagHelperContext context, TagHelperOutput output)
    {
        output.TagName = "span";
        output.TagMode = TagMode.StartTagAndEndTag;
        output.Attributes.SetAttribute("class", $"status-badge {Status.BadgeClass()}");
        output.Content.SetHtmlContent($"<i class=\"bi {Status.Icon()}\" aria-hidden=\"true\"></i>");
        output.Content.Append(Status.DisplayName());
    }
}
