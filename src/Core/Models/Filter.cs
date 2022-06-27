using System;

namespace Hippo.Core.Models;
public interface IPageFilter
{
    int PageIndex { get; set; }
    int PageSize { get; set; }
    int Offset { get; }
}

public interface ISortFilter
{
    string SortBy { get; set; }
    bool IsSortedAscending { get; set; }
}

public interface ISearchFilter
{
    string SearchText { get; set; }
}

public class PageAndSortFilter : IPageFilter, ISortFilter
{
    public int PageIndex { get; set; }
    public int PageSize { get; set; }
    public int Offset { get { return PageIndex * PageSize; } }
    public string SortBy { get; set; }
    public bool IsSortedAscending { get; set; }

    public PageAndSortFilter()
    {
        PageIndex = 0;
        PageSize = 10;
    }

    public void MaxPage()
    {
        PageIndex = 0;
        PageSize = int.MaxValue;
    }
}

public class SearchFilter : PageAndSortFilter, ISearchFilter
{
    public string SearchText { get; set; } = "";
}

public class DateIntervalFilter : SearchFilter
{
    public DateTime? From { get; set; }
    public DateTime? To { get; set; }
}
