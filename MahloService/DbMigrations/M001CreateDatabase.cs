using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using FluentMigrator;

namespace MahloService.DbMigrations
{
  [Migration(1)]
  public class M001CreateDatabase : Migration
  {

    public override void Up()
    {
      const string strGreigeRolls = "GreigeRolls";
      const string strId = "Id";
      const string strFeetCounter = "FeetCounter";
      const string strBow = "Bow";
      const string strSkew = "Skew";
      const string strElongation = "Elongation";
      const string strTimeStamp = "TimeStamp";

      this.Create.Table("ProgramState")
        .WithColumn("Key").AsInt32().PrimaryKey().WithDefaultValue(0)
        .WithColumn("Value").AsAnsiString(8000).NotNullable().WithDefaultValue("<root/>");

      this.Execute.Sql("ALTER TABLE ProgramState ADD CONSTRAINT ProgramState_KeyConstraint CHECK([Key] = 0)");

      this.Insert.IntoTable("ProgramState")
        .Row(new { Key = 0, Value = "{}" });

      // ---- //
      this.Create.Table(strGreigeRolls)
        .WithColumn(strId).AsInt32().PrimaryKey()
        .WithColumn("IsComplete").AsBoolean().NotNullable().WithDefaultValue(0)

        // Data from Mfg database
        .WithColumn("RollNo").AsAnsiString(10).NotNullable()
        .WithColumn("OrderNo").AsAnsiString(10).NotNullable()
        .WithColumn("StyleCode").AsAnsiString(5).NotNullable()
        .WithColumn("StyleName").AsAnsiString(40).NotNullable()
        .WithColumn("ColorCode").AsAnsiString(6).NotNullable()
        .WithColumn("ColorName").AsAnsiString(40).NotNullable()
        .WithColumn("BackingCode").AsAnsiString(2).NotNullable()
        .WithColumn("RollLength").AsInt32().NotNullable()
        .WithColumn("RollWidth").AsDouble().NotNullable()
        .WithColumn("DefaultRecipe").AsAnsiString(50).NotNullable()
        .WithColumn("PatternRepeatLength").AsDouble().NotNullable()
        .WithColumn("ProductImageURL").AsAnsiString(500).Nullable()

        // Runtime calculated data
        .WithColumn("MalFeetCounterStart").AsInt64().NotNullable()
        .WithColumn("MalFeetCounterEnd").AsInt64().NotNullable()
        .WithColumn("MalSpeed").AsInt32().NotNullable()
        .WithColumn("MalMapValid").AsBoolean().NotNullable()

        .WithColumn("BasFeetCounterStart").AsInt64().NotNullable()
        .WithColumn("BasFeetCounterEnd").AsInt64().NotNullable()
        .WithColumn("BasSpeed").AsInt32().NotNullable()
        .WithColumn("BasMapValid").AsBoolean().NotNullable()

        .WithColumn("PrsFeetCounterStart").AsInt64().NotNullable()
        .WithColumn("PrsFeetCounterEnd").AsInt64().NotNullable()
        .WithColumn("PrsSpeed").AsInt32().NotNullable()
        .WithColumn("PrsMapValid").AsBoolean().NotNullable()

        .WithColumn("Bow").AsDouble().NotNullable()
        .WithColumn("Skew").AsDouble().NotNullable()
        .WithColumn("Elongation").AsDouble().NotNullable();

      this.Create.Table("CutRolls")
        .WithColumn(strId).AsInt32().PrimaryKey()
        .WithColumn("GreigeRollId").AsInt32().ForeignKey(strGreigeRolls, strId).OnDeleteOrUpdate(System.Data.Rule.Cascade)
        .WithColumn("SapRoll").AsAnsiString(20).NotNullable()
        .WithColumn("FeetCounterStart").AsInt64().NotNullable()
        .WithColumn("FeetCounterEnd").AsInt64().NotNullable()
        .WithColumn("Bow").AsDouble()
        .WithColumn("Skew").AsDouble()
        .WithColumn("EPE").AsDouble()
        .WithColumn("Dlot").AsAnsiString(10);

      this.Create.Table("Mahlo2Seam")
        .WithColumn(strFeetCounter).AsInt64().PrimaryKey()
        .WithColumn(strTimeStamp).AsDateTime().NotNullable().WithDefault(SystemMethods.CurrentDateTime);

      this.Create.Table("Mahlo2Map")
        .WithColumn(strFeetCounter).AsInt64().PrimaryKey()
        .WithColumn(strTimeStamp).AsDateTime().NotNullable().WithDefault(SystemMethods.CurrentDateTime);

      this.Create.Table("BowAndSkewSeam")
        .WithColumn(strFeetCounter).AsInt64().PrimaryKey()
        .WithColumn(strTimeStamp).AsDateTime().NotNullable().WithDefault(SystemMethods.CurrentDateTime);

      this.Create.Table("BowAndSkewMap")
        .WithColumn(strFeetCounter).AsInt64().PrimaryKey()
        .WithColumn(strBow).AsDouble().NotNullable()
        .WithColumn(strSkew).AsDouble().NotNullable()
        .WithColumn(strTimeStamp).AsDateTime().NotNullable().WithDefault(SystemMethods.CurrentDateTime);

      this.Create.Table("PatternRepeatSeam")
        .WithColumn(strFeetCounter).AsInt64().PrimaryKey()
        .WithColumn(strTimeStamp).AsDateTime().NotNullable().WithDefault(SystemMethods.CurrentDateTime);

      this.Create.Table("PatternRepeatMap")
        .WithColumn(strFeetCounter).AsInt64().PrimaryKey()
        .WithColumn(strElongation).AsDouble()
        .WithColumn(strTimeStamp).AsDateTime().NotNullable().WithDefault(SystemMethods.CurrentDateTime);
    }

    public override void Down()
    {
      throw new NotImplementedException();
    }
  }
}
