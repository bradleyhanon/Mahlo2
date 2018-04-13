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
      const string strGreigeRolls = "GreigeRolls";
      const string strRollId = "RollId";
      const string strFeet = "Feet";
      const string strCutId = "CutId";
      const string strBow = "Bow";
      const string strSkew = "Skew";
      const string strElongation = "Elongation";

      this.Create.Table("ProgramState")
        .WithColumn("Key").AsInt32().PrimaryKey().WithDefaultValue(0)
        .WithColumn("Value").AsAnsiString().NotNullable().WithDefaultValue("<root/>");

      this.Execute.Sql("ALTER TABLE ProgramState ADD CONSTRAINT ProgramState_KeyConstraint CHECK([Key] = 0)");

      this.Insert.IntoTable("ProgramState")
        .Row(new { Key = 0, Value = "{}" });

      this.Create.Table(strGreigeRolls)
        .WithColumn(strRollId).AsInt32().PrimaryKey()
        .WithColumn("GridImage").AsAnsiString().NotNullable()
        .WithColumn("G2ROLL").AsAnsiString(10).NotNullable().Indexed("IX_G2ROLL").Unique()
        .WithColumn("G2SCH").AsAnsiString(12).NotNullable()
        .WithColumn("G2LTF").AsInt32().NotNullable()
        .WithColumn("G2STYL").AsAnsiString(5).NotNullable()
        .WithColumn("F2SDSC").AsAnsiString(40).NotNullable()
        .WithColumn("G2CLR").AsAnsiString(6).NotNullable()
        .WithColumn("F2CDSC").AsAnsiString(40).NotNullable()
        .WithColumn("G2SBK").AsAnsiString(2).NotNullable()
        .WithColumn("G2WTF").AsAnsiString(2).NotNullable()
        .WithColumn("G2WTI").AsInt32().NotNullable()
        .WithColumn("DefaultRecipe").AsAnsiString(50).NotNullable()
        .WithColumn("G2RPLN").AsDecimal().NotNullable()
        .WithColumn("G2SJUL").AsInt32().NotNullable()
        .WithColumn("G2STME").AsAnsiString(6).NotNullable()
        .WithColumn("ProductImageURL").AsAnsiString(500);

      this.Create.Table("MahloRolls")
        .WithColumn(strRollId).AsInt32().PrimaryKey()
        .WithColumn(strFeet).AsInt32();

      this.Create.Table("BowAndSkewRolls")
        .WithColumn(strRollId).AsInt32().PrimaryKey().ForeignKey(strGreigeRolls, strRollId).OnDeleteOrUpdate(System.Data.Rule.Cascade)
        .WithColumn(strFeet).AsInt32().NotNullable()
        .WithColumn(strBow).AsDouble().NotNullable()
        .WithColumn(strSkew).AsDouble().NotNullable();

      this.Create.Table("BowAndSkewMaps")
        .WithColumn(strRollId).AsInt32().NotNullable().ForeignKey(strGreigeRolls, strRollId).OnDeleteOrUpdate(System.Data.Rule.Cascade)
        .WithColumn(strFeet).AsInt32().NotNullable()
        .WithColumn(strBow).AsDouble().NotNullable()
        .WithColumn(strSkew).AsDouble().NotNullable();

      this.Create.PrimaryKey().OnTable("BowAndSkewMaps").Columns(strRollId, strFeet);

      this.Create.Table("PatternRepeatRolls")
        .WithColumn(strRollId).AsInt32().PrimaryKey().ForeignKey(strGreigeRolls, strRollId).OnDeleteOrUpdate(System.Data.Rule.Cascade)
        .WithColumn(strFeet).AsInt32()
        .WithColumn(strElongation).AsDouble();

      this.Create.Table("PatternRepeatMaps")
        .WithColumn(strRollId).AsInt32().NotNullable().ForeignKey(strGreigeRolls, strRollId).OnDeleteOrUpdate(System.Data.Rule.Cascade)
        .WithColumn(strFeet).AsInt32().NotNullable()
        .WithColumn(strElongation).AsDouble();

      this.Create.PrimaryKey().OnTable("PatternRepeatMaps").Columns(strRollId, strFeet);

      this.Create.Table("FinishedRolls")
        .WithColumn(strRollId).AsInt32().ForeignKey(strGreigeRolls, strRollId).OnDeleteOrUpdate(System.Data.Rule.Cascade)
        .WithColumn(strCutId).AsInt32()
        .WithColumn("SapRoll").AsAnsiString().NotNullable()
        .WithColumn(strFeet).AsInt32()
        .WithColumn(strBow).AsDouble()
        .WithColumn(strSkew).AsDouble()
        .WithColumn(strElongation).AsDouble();

      this.Create.PrimaryKey().OnTable("FinishedRolls").Columns(strRollId, strCutId);

      this.Create.Table("FinishedRollMaps")
        .WithColumn(strRollId).AsInt32().NotNullable()
        .WithColumn(strCutId).AsInt32().NotNullable()
        .WithColumn(strFeet).AsInt32().NotNullable()
        .WithColumn(strBow).AsDouble().NotNullable()
        .WithColumn(strSkew).AsDouble().NotNullable()
        .WithColumn(strElongation).AsDouble().NotNullable();

      this.Create.PrimaryKey().OnTable("FinishedRollMaps").Columns(strRollId, strCutId, strFeet);

      this.Create.ForeignKey()
        .FromTable("FinishedRollMaps").ForeignColumns(strRollId, strCutId)
        .ToTable("FinishedRolls").PrimaryColumns(strRollId, strCutId)
        .OnDeleteOrUpdate(System.Data.Rule.Cascade);
    }

    public override void Down()
    {
      throw new NotImplementedException();
    }
  }
}
