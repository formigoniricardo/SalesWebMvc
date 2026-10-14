using Microsoft.AspNetCore.Mvc.ViewFeatures;

namespace SalesWebMvc.Helpers;

/// <summary>
/// Mensagens "flash" (sobrevivem ao redirect via TempData) exibidas como toast SweetAlert2 no _Layout.
/// </summary>
public static class ToastExtensions
{
    public const string TypeKey = "toast.type";
    public const string MessageKey = "toast.message";

    public static void ToastSuccess(this ITempDataDictionary tempData, string message) => Set(tempData, "success", message);
    public static void ToastError(this ITempDataDictionary tempData, string message) => Set(tempData, "error", message);

    private static void Set(ITempDataDictionary tempData, string type, string message)
    {
        tempData[TypeKey] = type;
        tempData[MessageKey] = message;
    }
}
