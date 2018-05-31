using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Dapper.Contrib.Extensions
{
  /// <summary>
  /// Specifies that this is a computed column.
  /// This attribute is needed to satisfy the compiler
  /// </summary>
  [AttributeUsage(AttributeTargets.Property)]
  public class ComputedAttribute : Attribute
  {
  }
}
