using System.ComponentModel.DataAnnotations;

namespace SalesWebMvc.Models.Enums;

public enum SaleStatus : int
{
    [Display(Name = "Pendente")]
    Pending = 0,

    [Display(Name = "Faturado")]
    Billed = 1,

    [Display(Name = "Cancelado")]
    Canceled = 2
}
