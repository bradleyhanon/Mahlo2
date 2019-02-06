using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

[AttributeUsage(AttributeTargets.Class)]
internal class TableAttribute : Attribute
{
  public TableAttribute(string tableName)
  {
    this.TableName = tableName;
  }

  public string TableName { get; }
}
