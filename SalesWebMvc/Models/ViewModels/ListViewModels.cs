using SalesWebMvc.Helpers;
using SalesWebMvc.Models.Enums;

namespace SalesWebMvc.Models.ViewModels;

public class SalesRecordFormViewModel
{
    public SalesRecord SalesRecord { get; set; } = new SalesRecord();
    public ICollection<Seller> Sellers { get; set; } = new List<Seller>();
}

public class SellerListViewModel
{
    public PagedList<Seller> Sellers { get; set; } = null!;
    public ICollection<Department> Departments { get; set; } = new List<Department>();

    // Filtros atuais (preservados na paginação)
    public string? Search { get; set; }
    public int? DepartmentId { get; set; }
}

/// <summary>
/// Parâmetros de busca da listagem de vendas (vindos da query string). Reutilizado pela exportação CSV.
/// </summary>
public class SalesRecordFilter
{
    public DateTime? MinDate { get; set; }
    public DateTime? MaxDate { get; set; }
    public int? DepartmentId { get; set; }
    public SaleStatus? Status { get; set; }
    public string? Search { get; set; }
    public string Sort { get; set; } = "date_desc";
    public int Page { get; set; } = 1;
    public int PageSize { get; set; } = 10;
}

public class SalesRecordListViewModel
{
    public SalesRecordFilter Filter { get; set; } = new();
    public PagedList<SalesRecord> Records { get; set; } = null!;
    public ICollection<Department> Departments { get; set; } = new List<Department>();

    // Totais de todo o resultado filtrado (não só da página)
    public decimal FilteredBilledTotal { get; set; }
    public decimal FilteredPendingTotal { get; set; }
}

public class DepartmentListItem
{
    public Department Department { get; set; } = null!;
    public int SellersCount { get; set; }
    public decimal BilledTotal { get; set; }
}
