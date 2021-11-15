using System;
using System.Collections.Generic;
using System.Linq;
using Microsoft.AspNetCore.Mvc.Rendering;

namespace Hippo.ViewModels;

internal static class Converters
{
    internal static IList<SelectListItem> AsSelectList<T>(this IEnumerable<T> source, Func<T, string> selector)
    {
        return source.Select(o => new SelectListItem(selector(o), selector(o))).ToList();
    }

    internal static IList<SelectListItem> EnumValuesAsSelectList<T>()
        where T : struct, Enum
    {
        return Enum.GetValues<T>().Select(o => new SelectListItem(Enum.GetName<T>(o), Enum.GetName<T>(o))).ToList();
    }
}
