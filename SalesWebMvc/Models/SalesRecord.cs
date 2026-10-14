using System.ComponentModel.DataAnnotations;
using SalesWebMvc.Models.Enums;
using SalesWebMvc.Models.Validation;

namespace SalesWebMvc.Models;

public class SalesRecord
{
    public int Id { get; set; }

    [Required(ErrorMessage = "{0} é obrigatória.")]
    [NotInFuture]
    [Display(Name = "Data da venda")]
    [DataType(DataType.Date)]
    public DateTime Date { get; set; }

    [Required(ErrorMessage = "{0} é obrigatório.")]
    [Range(typeof(decimal), "0.01", "10000000", ErrorMessage = "{0} deve ser maior que zero e menor que R$ 10 milhões.")]
    [Display(Name = "Valor")]
    [DataType(DataType.Currency)]
    public decimal Amount { get; set; }

    [Required(ErrorMessage = "{0} é obrigatório.")]
    [EnumDataType(typeof(SaleStatus), ErrorMessage = "Status inválido.")]
    [Display(Name = "Status")]
    public SaleStatus Status { get; set; }

    public Seller? Seller { get; set; }

    [Required(ErrorMessage = "Selecione um vendedor.")]
    [Display(Name = "Vendedor")]
    public int? SellerId { get; set; }

    public SalesRecord() { }

    public SalesRecord(int id, DateTime date, decimal amount, SaleStatus status, Seller seller)
    {
        Id = id;
        Date = date;
        Amount = amount;
        Status = status;
        Seller = seller;
    }
}
