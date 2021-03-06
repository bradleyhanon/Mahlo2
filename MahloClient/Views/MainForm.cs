﻿using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Linq;
using System.Reactive.Linq;
using System.Threading.Tasks;
using System.Windows.Forms;
using MahloClient.Ipc;
using MahloClient.Logic;
using MahloService.Logic;
using MahloService.Models;
using MahloService.Settings;

namespace MahloClient.Views
{
  internal partial class MainForm : Form
  {
    private delegate void CellFormattingAction(DataGridViewCellFormattingEventArgs args, IServiceSettings settings);

    private CellColor defaultCellColor;
    private string[] sewinQueueColumnNames = { nameof(GreigeRoll.RollNo), nameof(GreigeRoll.StyleCode), nameof(GreigeRoll.ColorCode), nameof(GreigeRoll.BackingCode), nameof(GreigeRoll.RollLength), nameof(GreigeRoll.RollWidthStr), nameof(GreigeRoll.PatternRepeatLength) };
    private string[] mahloColumnNames = { nameof(GreigeRoll.MalFeet) };
    private string[] bowAndSkewColumnNames = { nameof(GreigeRoll.BasFeet), nameof(GreigeRoll.Bow), nameof(GreigeRoll.Skew) };
    private string[] patternRepeatColumnNames = { nameof(GreigeRoll.PrsFeet), nameof(GreigeRoll.Elongation) };

    private List<IDisposable> disposables = new List<IDisposable>();

    private ICarpetProcessor carpetProcessor;
    private ICutRollList cutRollList;
    private IInspectionAreaList inspectionAreaList;
    private IMahloIpcClient mahloClient;
    private IServiceSettings serviceSettings;

    private bool autoScroll = true;

    public MainForm(ICarpetProcessor carpetProcessor, ICutRollList cutRollList, IInspectionAreaList inspectionAreaList, IMahloIpcClient mahloClient, IServiceSettings serviceSettings)
    {
      this.InitializeComponent();
      this.carpetProcessor = carpetProcessor;
      this.cutRollList = cutRollList;
      this.inspectionAreaList = inspectionAreaList;
      this.mahloClient = mahloClient;
      this.serviceSettings = serviceSettings;
      this.statusBar1.StatusBarInfo = (IStatusBarInfo)this.carpetProcessor.PatternRepeatLogic;
      this.defaultCellColor = new CellColor
      {
        ForeColor = this.grdGreigeRoll.DefaultCellStyle.ForeColor,
        BackColor = this.grdGreigeRoll.DefaultCellStyle.BackColor
      };

      this.disposables.Add(
        Observable.FromEventPattern<PropertyChangedEventHandler, PropertyChangedEventArgs>(
          h => ((INotifyPropertyChanged)this.carpetProcessor.MahloLogic).PropertyChanged += h,
          h => ((INotifyPropertyChanged)this.carpetProcessor.MahloLogic).PropertyChanged -= h)
          .Where(args => args.EventArgs.PropertyName == nameof(this.carpetProcessor.MahloLogic.CurrentRoll))
          .Subscribe(args =>
          {
            this.mahloRollSrc.DataSource = this.carpetProcessor.MahloLogic.CurrentRoll;
            this.DoAutoScroll();
            this.grdGreigeRoll.Invalidate();
          }));

      this.disposables.Add(
        Observable.FromEventPattern<PropertyChangedEventHandler, PropertyChangedEventArgs>(
          h => ((INotifyPropertyChanged)this.carpetProcessor.BowAndSkewLogic).PropertyChanged += h,
          h => ((INotifyPropertyChanged)this.carpetProcessor.BowAndSkewLogic).PropertyChanged -= h)
          .Where(args => args.EventArgs.PropertyName == nameof(this.carpetProcessor.BowAndSkewLogic.CurrentRoll))
          .Subscribe(args =>
          {
            this.bowAndSkewRollSrc.DataSource = this.carpetProcessor.BowAndSkewLogic.CurrentRoll;
            this.DoAutoScroll();
            this.grdGreigeRoll.Invalidate();
          }));

      this.disposables.Add(
        Observable.FromEventPattern<PropertyChangedEventHandler, PropertyChangedEventArgs>(
          h => ((INotifyPropertyChanged)this.carpetProcessor.PatternRepeatLogic).PropertyChanged += h,
          h => ((INotifyPropertyChanged)this.carpetProcessor.PatternRepeatLogic).PropertyChanged -= h)
          .Where(args => args.EventArgs.PropertyName == nameof(this.carpetProcessor.PatternRepeatLogic.CurrentRoll))
          .Subscribe(args =>
          {
            this.patternRepeatRollSrc.DataSource = this.carpetProcessor.PatternRepeatLogic.CurrentRoll;
            this.DoAutoScroll();
            this.grdGreigeRoll.Invalidate();
          }));

      // Make column heading alignment match column data alignment
      foreach (DataGridViewColumn column in this.grdGreigeRoll.Columns)
      {
        column.HeaderCell.Style.Alignment = column.DefaultCellStyle.Alignment;
      }

      foreach (DataGridViewColumn column in this.grdCutRoll.Columns)
      {
        column.HeaderCell.Style.Alignment = column.DefaultCellStyle.Alignment;
      }

      foreach (DataGridViewColumn column in this.grdInspectionArea.Columns)
      {
        column.HeaderCell.Style.Alignment = column.DefaultCellStyle.Alignment;
      }

      this.colMalFeet.Tag =
        this.colBasFeet.Tag =
        this.colPrsFeet.Tag = new CellFormattingAction(this.SetFeetColor);

      this.colBow.Tag = new CellFormattingAction(this.SetBowColor);
      this.colSkew.Tag = new CellFormattingAction(this.SetSkewColor);
      this.colElongation.Tag = new CellFormattingAction(this.SetElongationColor);

      this.colCutBow.Tag = new CellFormattingAction(this.SetCutBowColor);
      this.colCutSkew.Tag = new CellFormattingAction(this.SetCutSkewColor);
      this.colCutElongation.Tag = new CellFormattingAction(this.SetCutEPEColor);
    }

    /// <summary>
    /// Clean up any resources being used.
    /// </summary>
    /// <param name="disposing">true if managed resources should be disposed; otherwise, false.</param>
    protected override void Dispose(bool disposing)
    {
      if (disposing)
      {
        this.disposables.ForEach(item => item.Dispose());
        this.components?.Dispose();
      }

      base.Dispose(disposing);
    }

    private void MainForm_Load(object sender, EventArgs e)
    {
      this.carpetProcessor.Start();
      this.sewinQueueSrc.DataSource = this.carpetProcessor.SewinQueue.Rolls;

      //this.grdGreigeRoll.DataSource = this.carpetProcessor.SewinQueue.Rolls;
      this.mahloRollSrc.DataSource = this.carpetProcessor.MahloLogic.CurrentRoll;
      this.bowAndSkewRollSrc.DataSource = this.carpetProcessor.BowAndSkewLogic.CurrentRoll;
      this.patternRepeatRollSrc.DataSource = this.carpetProcessor.PatternRepeatLogic.CurrentRoll;

      this.mahloLogicSrc.DataSource = this.carpetProcessor.MahloLogic;
      this.bowAndSkewLogicSrc.DataSource = this.carpetProcessor.BowAndSkewLogic;
      this.patternRepeatLogicSrc.DataSource = this.carpetProcessor.PatternRepeatLogic;
      this.cutRollSrc.DataSource = this.cutRollList;
      this.inspectionAreaBindingSource.DataSource = this.inspectionAreaList;

      this.grpMahlo.Tag = nameof(IMahloLogic);
      this.grpBowAndSkew.Tag = nameof(IBowAndSkewLogic);
      this.grpPatternRepeat.Tag = nameof(IPatternRepeatLogic);

      this.grdGreigeRoll.ClearSelection();
    }

    private void GrdGreigeRoll_CellPainting(object sender, DataGridViewCellPaintingEventArgs e)
    {
      var col = this.grdGreigeRoll.Columns[e.ColumnIndex];
      e.Handled = string.IsNullOrWhiteSpace(col.DataPropertyName);
      if (e.Handled)
      {
        Rectangle rect = e.CellBounds;
        rect.X--;
        rect.Width++;
        e.Graphics.FillRectangle(SystemBrushes.AppWorkspace, rect);
      }
    }

    private void DoAutoScroll()
    {
      if (this.autoScroll)
      {
        var cp = this.carpetProcessor;
        int minIndex = Math.Min(cp.MahloLogic.CurrentRollIndex, cp.BowAndSkewLogic.CurrentRollIndex);
        minIndex = Math.Min(minIndex, cp.PatternRepeatLogic.CurrentRollIndex);
        this.myScrollBar.AutoScrollPosition = minIndex - 2;
      }
    }

    private async void BtnForeMahlo2_Click(object sender, EventArgs e)
    {
      await this.ExecuteMoveToNextButtonCmdAsync(
        sender,
        this.carpetProcessor.MahloLogic.CurrentRoll.RollNo,
        this.carpetProcessor.MahloLogic.MeasuredLength);
    }

    private async void BtnForeBowAndSkew_Click(object sender, EventArgs e)
    {
      await this.ExecuteMoveToNextButtonCmdAsync(
        sender,
        this.carpetProcessor.BowAndSkewLogic.CurrentRoll.RollNo,
        this.carpetProcessor.BowAndSkewLogic.MeasuredLength);
    }

    private async void BtnForePatternRepeat_Click(object sender, EventArgs e)
    {
      await this.ExecuteMoveToNextButtonCmdAsync(
        sender,
        this.carpetProcessor.PatternRepeatLogic.CurrentRoll.RollNo,
        this.carpetProcessor.PatternRepeatLogic.MeasuredLength);
    }

    private async void BtnBack_Click(object sender, EventArgs e)
    {
      await this.ExecuteButtonCmdAsync(sender, Ipc.MahloIpcClient.MoveToPriorRollCommand);
    }

    private async void BtnWaitForSeam_Click(object sender, EventArgs e)
    {
      await this.ExecuteButtonCmdAsync(sender, Ipc.MahloIpcClient.WaitForSeamCommand);
    }

    private async Task ExecuteMoveToNextButtonCmdAsync(object sender, string rollNo, long measuredLength)
    {
      using (var dlg = new MoveToNextDialog
      {
        RollNumber = rollNo,
        RollLength = (int)measuredLength,
        MaxLength = (int)measuredLength,
      })
      {
        if (dlg.ShowDialog() == System.Windows.Forms.DialogResult.OK)
        {
          await this.ExecuteButtonCmdAsync(sender, Ipc.MahloIpcClient.MoveToNextRollCommand, dlg.RollLength);
        }
      }
    }

    private async Task ExecuteButtonCmdAsync(object sender, string command, params object[] args)
    {
      Button button = (Button)sender;
      string name = (string)button.Parent.Tag;
      var buttons = button.Parent.Controls.OfType<Button>();
      buttons.ForEach(item => item.Enabled = false);
      args = Enumerable.Repeat(name, 1).Concat(args).ToArray();
      await this.mahloClient.CallAsync(command, args);
      buttons.ForEach(item => item.Enabled = true);
    }

    private void DataGridView1_SelectionChanged(object sender, EventArgs e)
    {
      // Prevent selection
      this.grdGreigeRoll.ClearSelection();
    }

    private void GrdGreigeRoll_CellFormatting(object sender, DataGridViewCellFormattingEventArgs e)
    {
      var col = this.grdGreigeRoll.Columns[e.ColumnIndex];
      if (e.RowIndex >= 0 && this.sewinQueueColumnNames.Contains(col.DataPropertyName))
      {
        this.SetCompletedColor(e);
      }

      GreigeRoll gridRoll = this.carpetProcessor.SewinQueue.Rolls[e.RowIndex];
      SetColor(this.carpetProcessor.MahloLogic, this.mahloColumnNames);
      SetColor(this.carpetProcessor.BowAndSkewLogic, this.bowAndSkewColumnNames);
      SetColor(this.carpetProcessor.PatternRepeatLogic, this.patternRepeatColumnNames);

      void SetColor(IMeterLogic logic, string[] names)
      {
        if (names.Contains(col.DataPropertyName))
        {
          if (logic.CurrentRoll == gridRoll)
          {
            (e.CellStyle.ForeColor, e.CellStyle.BackColor) = CellColor.ActiveColor;
          }
          else if (e.RowIndex >= 0 && e.RowIndex < logic.CurrentRollIndex)
          {
            var action = this.grdGreigeRoll.Columns[e.ColumnIndex].Tag as CellFormattingAction;
            action?.Invoke(e, this.serviceSettings);
          }
        }
      }
    }

    private void GrdCutRoll_CellFormatting(object sender, DataGridViewCellFormattingEventArgs e)
    {
      if (e.RowIndex >= 0)
      {
        var action = this.grdCutRoll.Columns[e.ColumnIndex].Tag as CellFormattingAction;
        action?.Invoke(e, this.serviceSettings);
      }
    }

    public void SetCompletedColor(DataGridViewCellFormattingEventArgs args)
    {
      GreigeRoll roll = (GreigeRoll)this.grdGreigeRoll.Rows[args.RowIndex].DataBoundItem;
      (args.CellStyle.ForeColor, args.CellStyle.BackColor) =
        roll.IsComplete ? CellColor.GetIsCompletedColorColor() : this.defaultCellColor;
    }

    public void SetFeetColor(DataGridViewCellFormattingEventArgs args, IServiceSettings settings)
    {
      GreigeRoll roll = (GreigeRoll)this.grdGreigeRoll.Rows[args.RowIndex].DataBoundItem;
      long measuredLength = (long)args.Value;

      (args.CellStyle.ForeColor, args.CellStyle.BackColor) =
        roll.RollLength == 0 ? this.defaultCellColor :
        CellColor.GetFeetColor(roll.RollLength, (long)args.Value, this.serviceSettings);
    }

    public void SetBowColor(DataGridViewCellFormattingEventArgs args, IServiceSettings settings)
    {
      GreigeRoll roll = (GreigeRoll)this.grdGreigeRoll.Rows[args.RowIndex].DataBoundItem;

      (args.CellStyle.ForeColor, args.CellStyle.BackColor) =
        CellColor.GetBowColor(roll.BackingCode, (double)args.Value, this.serviceSettings);
    }

    public void SetSkewColor(DataGridViewCellFormattingEventArgs args, IServiceSettings settings)
    {
      GreigeRoll roll = (GreigeRoll)this.grdGreigeRoll.Rows[args.RowIndex].DataBoundItem;

      (args.CellStyle.ForeColor, args.CellStyle.BackColor) =
        CellColor.GetSkewColor(roll.BackingCode, (double)args.Value, this.serviceSettings);
    }

    public void SetElongationColor(DataGridViewCellFormattingEventArgs args, IServiceSettings settings)
    {

    }

    public void SetCutBowColor(DataGridViewCellFormattingEventArgs args, IServiceSettings settings)
    {
      GreigeRoll roll = this.carpetProcessor.PatternRepeatLogic.CurrentRoll;

      (args.CellStyle.ForeColor, args.CellStyle.BackColor) =
        CellColor.GetBowColor(roll.BackingCode, (double)args.Value, this.serviceSettings);
    }

    public void SetCutSkewColor(DataGridViewCellFormattingEventArgs args, IServiceSettings settings)
    {
      GreigeRoll roll = this.carpetProcessor.PatternRepeatLogic.CurrentRoll;

      (args.CellStyle.ForeColor, args.CellStyle.BackColor) =
        CellColor.GetSkewColor(roll.BackingCode, (double)args.Value, this.serviceSettings);
    }

    public void SetCutEPEColor(DataGridViewCellFormattingEventArgs args, IServiceSettings settings)
    {

    }
  }
}
