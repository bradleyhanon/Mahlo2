using System;
using System.ComponentModel;
using System.Drawing;
using System.Reactive.Linq;
using System.Windows.Forms;
using MahloClient.Logic;

namespace MahloClient.Views
{
  internal partial class MyStatusBar : UserControl
  {
    private const int PnlMessage = 0;
    private const int PnlIndicator = 1;
    private const int PnlUserAttention = 2;
    private const int PnlAlarmIndex = 3;
    private const int PnlAlertMessage = 4;
    private const int PnlQueueMessage = 5;

    private IStatusBarInfo _statusBarInfo;
    private IDisposable propertyChangedSubscription;

    public MyStatusBar()
    {
      this.InitializeComponent();
    }

    [DesignerSerializationVisibility(DesignerSerializationVisibility.Hidden)]
    public IStatusBarInfo StatusBarInfo
    {
      get => this._statusBarInfo;
      set
      {
        this.propertyChangedSubscription?.Dispose();
        this._statusBarInfo = value;
        this.propertyChangedSubscription = Observable
          .FromEventPattern<PropertyChangedEventHandler, PropertyChangedEventArgs>(
            h => ((INotifyPropertyChanged)value).PropertyChanged += h,
            h => ((INotifyPropertyChanged)value).PropertyChanged -= h)
          .Subscribe(arg =>
          {
            switch (arg.EventArgs.PropertyName)
            {
              case nameof(this._statusBarInfo.IsSeamDetectEnabled):
              case nameof(this._statusBarInfo.IsSeamDetected):
              case nameof(this._statusBarInfo.UserAttentions):
              case nameof(this._statusBarInfo.CriticalStops):
              case nameof(this._statusBarInfo.AlertMessage):
              case nameof(this._statusBarInfo.CriticalAlarmMessage):
              case nameof(this._statusBarInfo.IgnoringSeams):
                this.statusBar1.Invalidate();
                break;

              case nameof(this._statusBarInfo.ConnectionStatusMessage):
                this.statusBar1.Panels[PnlMessage].Text = $"Service: {this._statusBarInfo.ConnectionStatusMessage}";
                break;

              case nameof(this._statusBarInfo.QueueMessage):
                this.statusBar1.Panels[PnlQueueMessage].Text = this._statusBarInfo.QueueMessage;
                break;
            }
          });
      }
    }

    /// <summary> 
    /// Clean up any resources being used.
    /// </summary>
    /// <param name="disposing">true if managed resources should be disposed; otherwise, false.</param>
    protected override void Dispose(bool disposing)
    {
      base.Dispose(disposing);
      if (disposing)
      {
        this.propertyChangedSubscription?.Dispose();
      }
    }

    private void statusBar1_DrawItem(object sender, StatusBarDrawItemEventArgs sbdevent)
    {
      if (this.StatusBarInfo == null)
      {
        return;
      }

      StatusBarPanel pnl = sbdevent.Panel;
      Font fnt;
      Brush brsh;
      Brush textBrush;
      brsh = Brushes.Transparent;
      StringFormat sf = new StringFormat
      {
        Alignment = StringAlignment.Center,
        LineAlignment = StringAlignment.Center
      };

      string txt = "";

      switch (sbdevent.Index)
      {
        case PnlIndicator:
          fnt = new Font(this.Font.FontFamily, this.Font.Size, FontStyle.Bold);
          txt = "Seam Detect";
          textBrush = Brushes.White;
          if (this.StatusBarInfo.IsSeamDetected)
          {
            brsh = Brushes.RoyalBlue;
          }
          else if (!this.StatusBarInfo.IsSeamDetectEnabled)
          {
            brsh = Brushes.Red;
            textBrush = Brushes.White;
          }
          else
          {
            if (this.StatusBarInfo.IgnoringSeams)
            {
              fnt = new Font(this.Font.FontFamily, this.Font.Size, FontStyle.Regular);
              brsh = Brushes.Transparent;
              textBrush = Brushes.Black;
            }
            else
            {
              brsh = Brushes.Green;
            }
          }
          sbdevent.Graphics.FillRectangle(brsh, sbdevent.Bounds);
          sbdevent.Graphics.DrawString(txt, fnt, textBrush, sbdevent.Bounds, sf);
          break;

        case PnlUserAttention:
          fnt = new Font(this.Font.FontFamily, this.Font.Size, FontStyle.Regular);
          txt = "User Alert";
          textBrush = Brushes.Black;
          if (this.StatusBarInfo.UserAttentions.Any)
          {
            fnt = new Font(this.Font.FontFamily, this.Font.Size, FontStyle.Bold);
            brsh = Brushes.Orange;
            textBrush = Brushes.White;
          }
          sbdevent.Graphics.FillRectangle(brsh, sbdevent.Bounds);
          sbdevent.Graphics.DrawString(txt, fnt, textBrush, sbdevent.Bounds, sf);
          break;

        case PnlAlarmIndex:
          fnt = new Font(this.Font.FontFamily, this.Font.Size, FontStyle.Regular);
          txt = "Major Alarm";
          textBrush = Brushes.Black;
          if (this.StatusBarInfo.CriticalStops.Any)
          {
            fnt = new Font(this.Font.FontFamily, this.Font.Size, FontStyle.Bold);
            brsh = Brushes.Red;
            textBrush = Brushes.White;
          }
          sbdevent.Graphics.FillRectangle(brsh, sbdevent.Bounds);
          sbdevent.Graphics.DrawString(txt, fnt, textBrush, sbdevent.Bounds, sf);
          break;

        case PnlAlertMessage:
          fnt = new Font(this.Font.FontFamily, this.Font.Size, FontStyle.Regular);
          textBrush = Brushes.Black;
          if (this.StatusBarInfo.CriticalStops.Any)
          {
            txt = this.StatusBarInfo.CriticalAlarmMessage;
          }
          else
          {
            txt = this.StatusBarInfo.AlertMessage;
          }
          //brsh = Brushes.LightGray;
          sf.Alignment = StringAlignment.Near;
          sf.LineAlignment = StringAlignment.Center;
          //sbdevent.Graphics.FillRectangle(brsh, sbdevent.Bounds);
          sbdevent.Graphics.DrawString(txt, fnt, textBrush, sbdevent.Bounds, sf);
          break;
      }
    }
  }
}
