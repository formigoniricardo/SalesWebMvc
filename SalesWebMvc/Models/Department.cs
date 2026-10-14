using System.ComponentModel.DataAnnotations;

namespace SalesWebMvc.Models;

public class Department
{
    public int Id { get; set; }

    [Required(ErrorMessage = "{0} é obrigatório.")]
    [StringLength(50, MinimumLength = 2, ErrorMessage = "{0} deve ter entre {2} e {1} caracteres.")]
    [Display(Name = "Nome")]
    public string Name { get; set; } = string.Empty;

    public ICollection<Seller> Sellers { get; set; } = new List<Seller>();

    public Department() { }

    public Department(int id, string name)
    {
        Id = id;
        Name = name;
    }

    public void AddSeller(Seller seller) => Sellers.Add(seller);
    public decimal TotalSales(DateTime initial, DateTime final) => Sellers.Sum(seller => seller.TotalSales(initial, final));
}
