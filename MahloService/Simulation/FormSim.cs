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
using PropertyChanged;

namespace MahloService.Simulation
{
  [AddINotifyPropertyChangedInterface]
  partial class FormSim : Form
  {
    private IDbMfgSim dbMfgSim;
    private IMahloSrc mahloSrc;
    private IBowAndSkewSrc bowAndSkewSrc;
    private IPatternRepeatSrc patternRepeatSrc;
    private IProgramState programState;
    private SimInfo simInfo;


    public FormSim(
      IDbMfg dbMfg,
      IMahloSrc mahloSrc,
      IBowAndSkewSrc bowAndSkewSrc,
      IPatternRepeatSrc patternRepeatSrc,
      IProgramState programState)
    {
      InitializeComponent();

      this.dbMfgSim = (IDbMfgSim)dbMfg;
      this.mahloSrc = mahloSrc;
      this.bowAndSkewSrc = bowAndSkewSrc;
      this.patternRepeatSrc = patternRepeatSrc;
      this.programState = programState;

      this.simInfo = new SimInfo(this.dbMfgSim, this.programState);
      this.srcSimInfo.DataSource = this.simInfo;
      this.srcFormSim.DataSource = this;
      this.srcGrid.DataSource = this.dbMfgSim.SewinQueue;
    }

    public double MalFeetCounter
    {
      get => this.mahloSrc.FeetCounter;
      set => this.mahloSrc.FeetCounter = value;
    }

    public double BasFeetCounter
    {
      get => this.bowAndSkewSrc.FeetCounter;
      set => this.bowAndSkewSrc.FeetCounter = value;
    }

    public double PrsFeetCounter
    {
      get => this.patternRepeatSrc.FeetCounter;
      set => this.patternRepeatSrc.FeetCounter = value;
    }

    /// <summary>
    /// Clean up any resources being used.
    /// </summary>
    /// <param name="disposing">true if managed resources should be disposed; otherwise, false.</param>
    protected override void Dispose(bool disposing)
    {
      if (disposing && (components != null))
      {
        components.Dispose();
      }
      base.Dispose(disposing);
      this.simInfo.Dispose();
    }

    protected override void OnLoad(EventArgs e)
    {
      base.OnLoad(e);
      if (MalFeetCounter <= 0.0)
      {
        MalFeetCounter = 400;
        BasFeetCounter = 200;
        PrsFeetCounter = 0;
      }
    }

    private void BtnAddRoll_Click(object sender, EventArgs e)
    {
      this.dbMfgSim.AddRoll();
    }

    private void BtnRemoveRoll_Click(object sender, EventArgs e)
    {
      this.dbMfgSim.RemoveRoll();
    }

    private void btnRun_Click(object sender, EventArgs e)
    {
      if (btnRun.Text == "Run")
      {
        this.btnRun.Text = "Stop";
        ((OpcSrcSim<MahloModel>)this.mahloSrc).Start(this.simInfo.FeetPerMinute);
        ((OpcSrcSim<BowAndSkewModel>)this.bowAndSkewSrc).Start(this.simInfo.FeetPerMinute);
        ((OpcSrcSim<PatternRepeatModel>)this.patternRepeatSrc).Start(this.simInfo.FeetPerMinute);
      }
      else
      {
        btnRun.Text = "Run";
        ((OpcSrcSim<MahloModel>)this.mahloSrc).Stop();
        ((OpcSrcSim<BowAndSkewModel>)this.bowAndSkewSrc).Stop();
        ((OpcSrcSim<PatternRepeatModel>)this.patternRepeatSrc).Stop();
      }
    }

    private void BtnExit_Click(object sender, EventArgs e)
    {
      this.Close();
    }
  }
}
