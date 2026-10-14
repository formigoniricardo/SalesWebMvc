using System.ComponentModel.DataAnnotations;
using SalesWebMvc.Models.Validation;

namespace SalesWebMvc.Models;

public class Seller
{
    public int Id { get; set; }

    [Required(ErrorMessage = "{0} é obrigatório.")]
    [StringLength(60, MinimumLength = 3, ErrorMessage = "{0} deve ter entre {2} e {1} caracteres.")]
    [Display(Name = "Nome")]
    public string Name { get; set; } = string.Empty;

    [Required(ErrorMessage = "{0} é obrigatório.")]
    [EmailAddress(ErrorMessage = "Informe um e-mail válido.")]
    [StringLength(100, ErrorMessage = "{0} deve ter no máximo {1} caracteres.")]
    [Display(Name = "E-mail")]
    [DataType(DataType.EmailAddress)]
    public string Email { get; set; } = string.Empty;

    [Required(ErrorMessage = "{0} é obrigatória.")]
    [MinimumAge(18, 100)]
    [Display(Name = "Data de nascimento")]
    [DataType(DataType.Date)]
    public DateTime BirthDate { get; set; }

    [Required(ErrorMessage = "{0} é obrigatório.")]
    [Range(typeof(decimal), "0", "1000000", ErrorMessage = "{0} não pode ser negativo nem maior que R$ 1 milhão.")]
    [Display(Name = "Salário base")]
    [DataType(DataType.Currency)]
    public decimal BaseSalary { get; set; }

    public Department? Department { get; set; }

    [Required(ErrorMessage = "Selecione um departamento.")]
    [Display(Name = "Departamento")]
    public int? DepartmentId { get; set; }

    public ICollection<SalesRecord> Sales { get; set; } = new List<SalesRecord>();

    public Seller() { }

    public Seller(int id, string name, string email, DateTime birthDate, decimal baseSalary, Department department)
    {
        Id = id;
        Name = name;
        Email = email;
        BirthDate = birthDate;
        BaseSalary = baseSalary;
        Department = department;
    }

    public void AddSales(SalesRecord sr) => Sales.Add(sr);
    public void RemoveSales(SalesRecord sr) => Sales.Remove(sr);
    public decimal TotalSales(DateTime initial, DateTime final) => Sales.Where(sr => sr.Date >= initial && sr.Date <= final).Sum(sr => sr.Amount);
}
