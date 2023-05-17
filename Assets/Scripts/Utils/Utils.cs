using System;
using System.Collections.Generic;

public static class Utils
{

    public static void Foreach<T>(this IEnumerable<T> self, Action<T> action)
    {
        foreach (T elem in self)
            action(elem);
    }

}