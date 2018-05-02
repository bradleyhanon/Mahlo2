using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using FluentMigrator;

namespace Mahlo.DbMigrations
{
  [Migration(1)]
  public class M001CreateDatabase : Migration
  {

    public override void Up()
    {
      const string strCarpetRolls = "CarpetRolls";
      const string strId = "Id";
      const string strFeet = "Feet";
      const string strCutId = "CutId";
      const string strBow = "Bow";
      const string strSkew = "Skew";
      const string strElongation = "Elongation";

      this.Create.Table("ProgramState")
        .WithColumn("Key").AsInt32().PrimaryKey().WithDefaultValue(0)
        .WithColumn("Value").AsAnsiString(8000).NotNullable().WithDefaultValue("<root/>");

      this.Execute.Sql("ALTER TABLE ProgramState ADD CONSTRAINT ProgramState_KeyConstraint CHECK([Key] = 0)");

      this.Insert.IntoTable("ProgramState")
        .Row(new { Key = 0, Value = "{}" });

      this.Create.Table(strCarpetRolls)
        .WithColumn(strId).AsInt32().PrimaryKey()
        .WithColumn("RollNo").AsAnsiString(10).NotNullable().Indexed("IX_G2ROLL").Unique()
        .WithColumn("StyleCode").AsAnsiString(5).NotNullable()
        .WithColumn("StyleName").AsAnsiString(40).NotNullable()
        .WithColumn("ColorCode").AsAnsiString(6).NotNullable()
        .WithColumn("ColorName").AsAnsiString(40).NotNullable()
        .WithColumn("BackingCode").AsAnsiString(2).NotNullable()
        .WithColumn("RollLength").AsInt32().NotNullable()
        .WithColumn("RollWidth").AsDouble().NotNullable()
        .WithColumn("DefaultRecipe").AsAnsiString(50).NotNullable()
        .WithColumn("PatternRepeatLength").AsDouble().NotNullable()
        .WithColumn("ProductImageURL").AsAnsiString(500)
        .WithColumn("MalFeet").AsInt32().NotNullable()
        .WithColumn("BasFeet").AsInt32().NotNullable()
        .WithColumn("PrsFeet").AsInt32().NotNullable()
        .WithColumn("Bow").AsDouble().NotNullable()
        .WithColumn("Skew").AsDouble().NotNullable()
        .WithColumn("Elongation").AsDouble().NotNullable();

      //this.Create.Table("MahloRolls")
      //  .WithColumn(strId).AsInt32().PrimaryKey()
      //  .WithColumn(strFeet).AsInt32();

      //this.Create.Table("BowAndSkewRolls")
      //  .WithColumn(strId).AsInt32().PrimaryKey().ForeignKey(strCarpetRolls, strId).OnDeleteOrUpdate(System.Data.Rule.Cascade)
      //  .WithColumn(strFeet).AsInt32().NotNullable()
      //  .WithColumn(strBow).AsDouble().NotNullable()
      //  .WithColumn(strSkew).AsDouble().NotNullable();

      //this.Create.Table("BowAndSkewMaps")
      //  .WithColumn(strId).AsInt32().NotNullable().ForeignKey(strCarpetRolls, strId).OnDeleteOrUpdate(System.Data.Rule.Cascade)
      //  .WithColumn(strFeet).AsInt32().NotNullable()
      //  .WithColumn(strBow).AsDouble().NotNullable()
      //  .WithColumn(strSkew).AsDouble().NotNullable();

      //this.Create.PrimaryKey().OnTable("BowAndSkewMaps").Columns(strId, strFeet);

      //this.Create.Table("PatternRepeatRolls")
      //  .WithColumn(strId).AsInt32().PrimaryKey().ForeignKey(strCarpetRolls, strId).OnDeleteOrUpdate(System.Data.Rule.Cascade)
      //  .WithColumn(strFeet).AsInt32()
      //  .WithColumn(strElongation).AsDouble();

      //this.Create.Table("PatternRepeatMaps")
      //  .WithColumn(strId).AsInt32().NotNullable().ForeignKey(strCarpetRolls, strId).OnDeleteOrUpdate(System.Data.Rule.Cascade)
      //  .WithColumn(strFeet).AsInt32().NotNullable()
      //  .WithColumn(strElongation).AsDouble();

      //this.Create.PrimaryKey().OnTable("PatternRepeatMaps").Columns(strId, strFeet);

      this.Create.Table("CutRolls")
        .WithColumn(strId).AsInt32().ForeignKey(strCarpetRolls, strId).OnDeleteOrUpdate(System.Data.Rule.Cascade)
        .WithColumn(strCutId).AsInt32()
        .WithColumn("SapRoll").AsAnsiString().NotNullable()
        .WithColumn(strFeet).AsInt32()
        .WithColumn(strBow).AsDouble()
        .WithColumn(strSkew).AsDouble()
        .WithColumn(strElongation).AsDouble();

      this.Create.PrimaryKey().OnTable("CutRolls").Columns(strId, strCutId);

      this.Create.Table("CutRollMaps")
        .WithColumn(strId).AsInt32().NotNullable()
        .WithColumn(strCutId).AsInt32().NotNullable()
        .WithColumn(strFeet).AsInt32().NotNullable()
        .WithColumn(strBow).AsDouble().NotNullable()
        .WithColumn(strSkew).AsDouble().NotNullable()
        .WithColumn(strElongation).AsDouble().NotNullable();

      this.Create.PrimaryKey().OnTable("CutRollMaps").Columns(strId, strCutId, strFeet);

      this.Create.ForeignKey()
        .FromTable("CutRollMaps").ForeignColumns(strId, strCutId)
        .ToTable("CutRolls").PrimaryColumns(strId, strCutId)
        .OnDeleteOrUpdate(System.Data.Rule.Cascade);
    }

    public override void Down()
    {
      throw new NotImplementedException();
    }
  }
}
