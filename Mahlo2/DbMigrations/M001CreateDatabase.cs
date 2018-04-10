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
      const string strPosition = "Position";
      const string strCutId = "CutId";

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

      this.Create.Table("Mahlo2Rolls")
        .WithColumn(strRollId).AsInt32().PrimaryKey()
        .WithColumn("Meters").AsDouble();

      this.Create.Table("BowAndSkewRolls")
        .WithColumn(strRollId).AsInt32().PrimaryKey().ForeignKey(strGreigeRolls, strRollId).OnDeleteOrUpdate(System.Data.Rule.Cascade)
        .WithColumn("Length").AsDouble().NotNullable()
        .WithColumn("Bow").AsDouble().NotNullable()
        .WithColumn("Skew").AsDouble().NotNullable();

      this.Create.Table("BowAndSkewMaps")
        .WithColumn(strRollId).AsInt32().NotNullable().ForeignKey(strGreigeRolls, strRollId).OnDeleteOrUpdate(System.Data.Rule.Cascade)
        .WithColumn("Position").AsDouble().NotNullable()
        .WithColumn("Bow").AsDouble().NotNullable()
        .WithColumn("Skew").AsDouble().NotNullable();

      this.Create.PrimaryKey().OnTable("BowAndSkewDetails").Columns(strRollId, strPosition);

      this.Create.Table("PatternRepeatRolls")
        .WithColumn(strRollId).AsInt32().PrimaryKey().ForeignKey(strGreigeRolls, strRollId).OnDeleteOrUpdate(System.Data.Rule.Cascade)
        .WithColumn("Length").AsDouble()
        .WithColumn("Elongation").AsFloat();

      this.Create.Table("PatternRepeatMaps")
        .WithColumn(strRollId).AsInt32().NotNullable().ForeignKey(strGreigeRolls, strRollId).OnDeleteOrUpdate(System.Data.Rule.Cascade)
        .WithColumn(strPosition).AsDouble().NotNullable()
        .WithColumn("Elongation").AsDouble();

      this.Create.PrimaryKey().OnTable("PatternRepeatDetails").Columns(strRollId, strPosition);

      this.Create.Table("FinishedRolls")
        .WithColumn(strRollId).AsInt32().ForeignKey(strGreigeRolls, strRollId).OnDeleteOrUpdate(System.Data.Rule.Cascade)
        .WithColumn(strCutId).AsInt32()
        .WithColumn("SapRoll").AsAnsiString().NotNullable()
        .WithColumn("Length").AsInt32()
        .WithColumn("Bow").AsFloat()
        .WithColumn("Skew").AsFloat()
        .WithColumn("Elongation").AsFloat();

      this.Create.PrimaryKey().OnTable("FinishedRolls").Columns(strRollId, strCutId);

      this.Create.Table("FinishedRollDetails")
        .WithColumn(strRollId).AsInt32().NotNullable()
        .WithColumn(strCutId).AsInt32().NotNullable()
        .WithColumn("Position").AsInt32().NotNullable()
        .WithColumn("Bow").AsFloat().NotNullable()
        .WithColumn("Skew").AsFloat().NotNullable()
        .WithColumn("Elongation").AsFloat().NotNullable();

      this.Create.PrimaryKey().OnTable("FinishedRollDetails").Columns(strRollId, strCutId, strPosition);

      this.Create.ForeignKey()
        .FromTable("FinishedRollDetails").ForeignColumns(strRollId, strCutId)
        .ToTable("FinishedRolls").PrimaryColumns(strRollId, strCutId)
        .OnDeleteOrUpdate(System.Data.Rule.Cascade);
    }

    public override void Down()
    {
      throw new NotImplementedException();
    }
  }
}
