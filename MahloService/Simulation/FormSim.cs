using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;
using MahloService.Models;
using MahloService.Opc;
using MahloService.Repository;

namespace MahloService.Simulation
{
  partial class FormSim : Form
  {
    private IDbMfgSim dbMfgSim;
    private IMahloSrc mahloSrc;
    private IBowAndSkewSrc bowAndSkewSrc;
    private IPatternRepeatSrc patternRepeatSrc;

    private SimInfo simInfo;

    public FormSim(
      IDbMfg dbMfg,
      IMahloSrc mahloSrc,
      IBowAndSkewSrc bowAndSkewSrc,
      IPatternRepeatSrc patternRepeatSrc)
    {
      InitializeComponent();

      this.dbMfgSim = (IDbMfgSim)dbMfg;
      this.mahloSrc = mahloSrc;
      this.bowAndSkewSrc = bowAndSkewSrc;
      this.patternRepeatSrc = patternRepeatSrc;

      this.simInfo = new SimInfo(this.dbMfgSim);
      this.srcSimInfo.DataSource = this.simInfo;
    }


    protected override void OnLoad(EventArgs e)
    {
      base.OnLoad(e);
      this.srcGrid.DataSource = this.dbMfgSim.SewinQueue;
    }

    private void btnAddRoll_Click(object sender, EventArgs e)
    {
      this.dbMfgSim.AddRoll();
    }

    private void btnRun_Click(object sender, EventArgs e)
    {
      if (btnRun.Text == "Run")
      {
        this.btnRun.Text = "Stop";
        ((OpcSrcSim<MahloRoll>)this.mahloSrc).Start(400, this.simInfo.FeetPerMinute);
        ((OpcSrcSim<BowAndSkewRoll>)this.bowAndSkewSrc).Start(200, this.simInfo.FeetPerMinute);
        ((OpcSrcSim<PatternRepeatRoll>)this.patternRepeatSrc).Start(0, this.simInfo.FeetPerMinute);
      }
      else
      {
        btnRun.Text = "Run";
        ((OpcSrcSim<MahloRoll>)this.mahloSrc).Stop();
        ((OpcSrcSim<BowAndSkewRoll>)this.bowAndSkewSrc).Stop();
        ((OpcSrcSim<PatternRepeatRoll>)this.patternRepeatSrc).Stop();
      }
    }
  }
}
