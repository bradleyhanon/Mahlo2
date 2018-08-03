using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Drawing;
using System.IO;
using System.Linq;
using System.Runtime.Serialization;
using System.Text;
using System.Threading.Tasks;

namespace MahloService
{
  static class ServiceExtensions
  {
    public static double MetersToFeet(double meters)
    {
      return meters * 3.28084;
    }

    public static double FeetToMeters(double feet)
    {
      return feet / 3.28084;
    }

    public static void AddRange<T>(this BindingList<T> list, IEnumerable<T> items)
    {
      foreach(var item in items)
      {
        list.Add(item);
      }
    }

    public static IEnumerable<T> FindNewItems<T>(this IEnumerable<T> newItems, IEnumerable<T> oldItems, Func<T, T, bool> predicate)
    {
      int newCount = newItems.Count();
      int oldCount = oldItems.Count();

      for (int j = 0; j < oldCount; j++)
      {
        var oldSeq = oldItems.Skip(j);
        var newSeq = newItems.Take(oldCount - j);
        if (oldSeq.Equals(newSeq))
        {
          return newSeq.Skip(oldCount - j);
        }
      }

      return newItems;
    }

    public static void ForEach<T>(this IEnumerable<T> items, Action<T> action)
    {
      foreach(T item in items)
      {
        action(item);
      }
    }

    public static Color ContrastColor(this Color color)
    {
      int d = 0;

      // Counting the perceptive luminance - human eye favors green color... 
      double a = 1 - (0.299 * color.R + 0.587 * color.G + 0.114 * color.B) / 255;

      if (a < 0.5)
        d = 0; // bright colors - black font
      else
        d = 255; // dark colors - white font

      return Color.FromArgb(d, d, d);
    }
  }
} 
