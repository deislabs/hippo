using System.ComponentModel.DataAnnotations;

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
    [Required]
    public IReadOnlyCollection<TModel> Items { get; set; } = new List<TModel>();

    /// <summary>
    /// the total number of items for the requested query
    /// </summary>
    [Required]
    public int TotalItems { get; set; }

    /// <summary>
    /// The zero based page index of the current data
    /// </summary>
    [Required]
    public int PageIndex { get; set; }

    /// <summary>
    /// The requested page size, not to be confused with the number of quered records
    /// </summary>
    [Required]
    public int PageSize { get; set; }

    [Required]
    public bool IsLastPage
    {
        get
        {
            return TotalItems == PageIndex * PageSize + Items.Count;
        }
    }
}
