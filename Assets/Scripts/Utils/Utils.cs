using System;
using System.Collections.Generic;
using System.Linq;

public static class Utils
{

    public static void Foreach<T>(this IEnumerable<T> self, Action<T> action)
    {
        foreach (T elem in self)
            action(elem);
    }

    public static IEnumerable<T> Merge<T>(this IEnumerable<IEnumerable<T>> self)
    {
        return self.SelectMany(t => t);
    }

    public static IEnumerable<(T item, int index)> Enumerate<T>(this IEnumerable<T> self) 
    {
        return self.Select((x, i) => (x, i));
    }
    public static Dictionary<T, int> AssignUniqueIDs<T>(this IEnumerable<T> self) 
    {
        return self.Enumerate().ToDictionary(k => k.item, k => k.index);
    }

}