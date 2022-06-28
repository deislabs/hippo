namespace Hippo.Core.Models;

public static class FilteringExtensions
{
    public static IEnumerable<T> SortBy<T>(this IEnumerable<T> items, string sortBy, bool isSortedAscending)
    {
        IOrderedEnumerable<T> orderedItems;
        var sortPropInfo = typeof(T).GetProperty(sortBy);

        if (sortPropInfo == null)
        {
            throw new Exception("Sort field does it exist");
        }

        if (isSortedAscending)
        {
            orderedItems = items.OrderBy(app => sortPropInfo.GetValue(app, null));
        }
        else
        {
            orderedItems = items.OrderByDescending(app => sortPropInfo.GetValue(app, null));
        }

        return orderedItems;
    }

    public static IEnumerable<T> ScalarSortBy<T>(this IEnumerable<T> items, bool isSortedAscending)
    {
        IOrderedEnumerable<T> orderedItems;

        if (isSortedAscending)
        {
            orderedItems = items.OrderBy(item => item);
        }
        else
        {
            orderedItems = items.OrderByDescending(item => item);
        }

        return orderedItems;
    }
}
