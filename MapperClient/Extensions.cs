using System;
using System.Collections.Generic;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Newtonsoft.Json;
using Newtonsoft.Json.Linq;

namespace MapperClient
{
  static class Extensions
  {
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

    public static void ForEach<T>(this IEnumerable<T> items, Action<T> action)
    {
      foreach(var item in items)
      {
        action(item);
      }
    }

    public static void Populate<T>(this JToken value, T target) 
      where T:class
    {
      using (var sr = value.CreateReader())
      {
        JsonSerializerSettings settings = new JsonSerializerSettings
        {
         
        };

        JsonSerializer.CreateDefault().Populate(sr, target);
      }
    }
  }
}
