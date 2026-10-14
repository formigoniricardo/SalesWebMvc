namespace SalesWebMvc.Models.ViewModels;

public class ErrorViewModel
{
    public string? RequestId { get; set; }
    public bool ShowRequestId => !string.IsNullOrEmpty(RequestId);

    // Adicionamos a propriedade Message aqui
    public string? Message { get; set; }
}