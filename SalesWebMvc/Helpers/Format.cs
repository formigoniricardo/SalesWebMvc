using System.Globalization;

namespace SalesWebMvc.Helpers;

/// <summary>
/// Formatação de exibição em pt-BR. A cultura da requisição fica em en-US para que
/// inputs numéricos (que sempre enviam ponto decimal) sejam lidos corretamente pelo model binding.
/// </summary>
public static class Format
{
    public static readonly CultureInfo PtBr = CultureInfo.GetCultureInfo("pt-BR");

    public static string Money(decimal value) => value.ToString("C", PtBr);
    public static string Date(DateTime value) => value.ToString("dd/MM/yyyy", PtBr);
    public static string Number(int value) => value.ToString("N0", PtBr);
    public static string Percent(decimal value) => (value / 100m).ToString("P1", PtBr);

    public static string Initials(string name)
    {
        var parts = name.Split(' ', StringSplitOptions.RemoveEmptyEntries);
        return parts.Length switch
        {
            0 => "?",
            1 => parts[0][..1].ToUpperInvariant(),
            _ => string.Concat(parts[0][0], parts[^1][0]).ToUpperInvariant()
        };
    }
}
