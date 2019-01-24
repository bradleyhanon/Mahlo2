using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using FluentMigrator;

namespace MahloService.DbMigrations
{
  [Migration(2)]
  public class M002CutRollPatternRepeatLength : Migration
  {
    public override void Up()
    {
      this.Alter.Table("CutRolls")
        .AddColumn("Elongation").AsDouble().NotNullable().WithDefaultValue(0.0);
    }

    public override void Down()
    {
      this.Delete.Column("Elongation").FromTable("CutRolls");  
    }
  }
}
