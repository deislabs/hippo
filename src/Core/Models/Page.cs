namespace Hippo.Core.Models;

public class Page<TModel>
{
    public Page()
    {
    }

    public Page(IReadOnlyCollection<TModel> items, int totalItems)
    {
        Items = items;
        TotalItems = totalItems;
    }

    /// <summary>
    /// the fetched items
    /// </summary>
    public IReadOnlyCollection<TModel> Items { get; set; }

    /// <summary>
    /// the total number of items for the requested query
    /// </summary>
    public int TotalItems { get; set; }

    /// <summary>
    /// The zero based page index of the current data
    /// </summary>
    public int? PageIndex { get; set; }

    /// <summary>
    /// The requested page size, not to be confused with the number of quered records
    /// </summary>
    public int? PageSize { get; set; }

    public bool? IsLastPage
    {
        get
        {
            if (!PageIndex.HasValue || !PageSize.HasValue)
                return null;
                
            return TotalItems == PageIndex * PageSize + Items.Count;
        }
    }
}
