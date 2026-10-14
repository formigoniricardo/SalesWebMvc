using SalesWebMvc.Models.Enums;

namespace SalesWebMvc.Helpers;

public static class SaleStatusExtensions
{
    public static string DisplayName(this SaleStatus status) => status switch
    {
        SaleStatus.Pending => "Pendente",
        SaleStatus.Billed => "Faturado",
        SaleStatus.Canceled => "Cancelado",
        _ => status.ToString()
    };

    public static string BadgeClass(this SaleStatus status) => status switch
    {
        SaleStatus.Pending => "status-pending",
        SaleStatus.Billed => "status-billed",
        SaleStatus.Canceled => "status-canceled",
        _ => string.Empty
    };

    public static string Icon(this SaleStatus status) => status switch
    {
        SaleStatus.Pending => "bi-hourglass-split",
        SaleStatus.Billed => "bi-check-circle-fill",
        SaleStatus.Canceled => "bi-x-circle-fill",
        _ => "bi-circle"
    };
}
