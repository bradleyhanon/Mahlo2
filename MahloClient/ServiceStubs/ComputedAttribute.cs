using System;

namespace Dapper.Contrib.Extensions
{
  /// <summary>
  /// Specifies that this is a computed column.
  /// This attribute normally comes from Dapper,
  /// but since we aren't using dapper,
  /// we supply it here to satisfy the compiler.
  /// </summary>
  [AttributeUsage(AttributeTargets.Property)]
  public class ComputedAttribute : Attribute
  {
  }
}
