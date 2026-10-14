using Microsoft.EntityFrameworkCore;

namespace SalesWebMvc.Helpers;

/// <summary>Metadados de paginação (não genérico), consumidos pela partial _Pagination.</summary>
public interface IPagedList
{
    int PageNumber { get; }
    int PageSize { get; }
    int TotalCount { get; }
    int TotalPages { get; }
    bool HasPrevious { get; }
    bool HasNext { get; }
    int FirstItemIndex { get; }
    int LastItemIndex { get; }
}

/// <summary>
/// Página de resultados obtida no banco com Skip/Take assíncronos, para listagens que podem crescer.
/// </summary>
public class PagedList<T> : IPagedList
{
    public IReadOnlyList<T> Items { get; }
    public int PageNumber { get; }
    public int PageSize { get; }
    public int TotalCount { get; }

    public int TotalPages => Math.Max(1, (int)Math.Ceiling(TotalCount / (double)PageSize));
    public bool HasPrevious => PageNumber > 1;
    public bool HasNext => PageNumber < TotalPages;
    public int FirstItemIndex => TotalCount == 0 ? 0 : (PageNumber - 1) * PageSize + 1;
    public int LastItemIndex => Math.Min(PageNumber * PageSize, TotalCount);

    public PagedList(IReadOnlyList<T> items, int totalCount, int pageNumber, int pageSize)
    {
        Items = items;
        TotalCount = totalCount;
        PageNumber = pageNumber;
        PageSize = pageSize;
    }

    public static async Task<PagedList<T>> CreateAsync(IQueryable<T> source, int pageNumber, int pageSize)
    {
        pageSize = Math.Clamp(pageSize, 5, 100);
        var totalCount = await source.CountAsync();
        var totalPages = Math.Max(1, (int)Math.Ceiling(totalCount / (double)pageSize));
        pageNumber = Math.Clamp(pageNumber, 1, totalPages);

        var items = await source.Skip((pageNumber - 1) * pageSize).Take(pageSize).ToListAsync();
        return new PagedList<T>(items, totalCount, pageNumber, pageSize);
    }
}
