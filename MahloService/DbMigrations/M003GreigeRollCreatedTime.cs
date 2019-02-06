using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using FluentMigrator;

namespace MahloService.DbMigrations
{
  [Migration(3)]
  public class M003GreigeRollCreatedTime : Migration
  {
    public override void Up()
    {
      this.Alter.Table("GreigeRolls")
        .AddColumn("CreatedTime").AsDateTime().NotNullable().WithDefault(SystemMethods.CurrentDateTime);

      var cmdText =
        "UPDATE GreigeRolls " +
        "SET CreatedTime = " +
        "  COALESCE((SELECT MIN(TimeStamp) " +
        "            FROM Mahlo2Map " +
        "            WHERE FeetCounter >= MalFeetCounterStart), CreatedTime) " +
        "WHERE MalFeetCounterStart > 0 ";
        
      this.Execute.Sql(cmdText);
    }

    public override void Down()
    {
      this.Delete.Column("CreatedTime").FromTable("GreigeRolls");
    }
  }
}
