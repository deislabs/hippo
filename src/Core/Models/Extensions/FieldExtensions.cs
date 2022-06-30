namespace Hippo.Core.Models;

public static class FieldExtensions
{
    public static bool IsSet<T>(this Field<T> field)
    {
        return field is not null;
    }

    public static void WhenSet<T>(this Field<T> field, Action<T> action)
    {
        if (!field.IsSet())
            return;

        action(field.Value);
    }

    public static Task WhenSet<T>(this Field<T> field, Func<T, Task> action)
    {
        if (!field.IsSet())
            return Task.CompletedTask;

        return action(field.Value);
    }
}
