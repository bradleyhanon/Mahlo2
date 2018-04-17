using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.IO;
using System.Linq;
using System.Runtime.Serialization;
using System.Text;
using System.Threading.Tasks;
using Mahlo.Opc;

namespace Mahlo
{
  static class Extensions
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
  }
}
