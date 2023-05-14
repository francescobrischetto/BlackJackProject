using System;
using System.Collections.Generic;

/// <summary>
/// This class provides extension methods for lists.
/// </summary>
public static class ListExtension
{
    private static Random rand = new Random();

    /// <summary>
    /// This extension method shuffle in-place a list based on the Fisher–Yates shuffle 
    /// </summary>
    /// <param name="transform"></param>
    /// <returns></returns>
    public static void Shuffle<T>(this IList<T> values)
    {
        for (int i = values.Count - 1; i > 0; i--)
        {
            int k = rand.Next(i + 1);
            T value = values[k];
            values[k] = values[i];
            values[i] = value;
        }
    }

}