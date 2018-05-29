using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Drawing;
using System.Linq;
using System.Reactive.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;
using MahloClient.Logic;

namespace MahloClient.Views
{
  class MyStatusBar : StatusBar
  {
    private const int PnlMessage = 0;
    private const int PnlIndicator = 1;
    private const int PnlUserAttention = 2;
    private const int PnlAlarmIndex = 3;
    private const int PnlAlertMessage = 4;
    private const int PnlQueueMessage = 5;

    private IStatusBarInfo _statusBarInfo;
    private IDisposable propertyChangedSubscription;

    [DesignerSerializationVisibility(DesignerSerializationVisibility.Hidden)]
    public IStatusBarInfo StatusBarInfo
    {
      get => _statusBarInfo;
      set
      {
        this.propertyChangedSubscription?.Dispose();
        _statusBarInfo = value;
        this.propertyChangedSubscription = Observable
          .FromEventPattern<PropertyChangedEventHandler, PropertyChangedEventArgs>(
            h => ((INotifyPropertyChanged)value).PropertyChanged += h,
            h => ((INotifyPropertyChanged)value).PropertyChanged -= h)
          .Subscribe(arg => 
          {
            //Console.WriteLine(arg.EventArgs.PropertyName);
            switch (arg.EventArgs.PropertyName)
            {
              case nameof(_statusBarInfo.IsSeamDetectEnabled):
              case nameof(_statusBarInfo.IsSeamDetected):
              case nameof(_statusBarInfo.UserAttentions):
              case nameof(_statusBarInfo.CriticalStops):
              case nameof(_statusBarInfo.AlertMessage):
              case nameof(_statusBarInfo.CriticalAlarmMessage):
              case nameof(_statusBarInfo.IgnoringSeams):
                this.Invalidate();
                break;

              case nameof(_statusBarInfo.ConnectionStatusMessage):
                this.Panels[PnlMessage].Text = $"Service: {_statusBarInfo.ConnectionStatusMessage}";
                break;

              case nameof(_statusBarInfo.QueueMessage):
                this.Panels[PnlQueueMessage].Text = _statusBarInfo.QueueMessage;
                break;
            }
          });
      }
    }

    protected override void Dispose(bool disposing)
    {
      base.Dispose(disposing);
      if (disposing)
      {
        this.propertyChangedSubscription?.Dispose();
      }
    }

    protected override void OnDrawItem(StatusBarDrawItemEventArgs sbdevent)
    {
      base.OnDrawItem(sbdevent);
      if (this.StatusBarInfo == null)
      {
        return;
      }

      StatusBarPanel pnl = sbdevent.Panel;
      Font fnt;
      SolidBrush brsh;
      Brush textBrush;
      brsh = new SolidBrush(Color.Transparent);
      StringFormat sf = new StringFormat();
      sf.Alignment = StringAlignment.Center;
      sf.LineAlignment = StringAlignment.Center;
      string txt = "";

      switch (sbdevent.Index)
      {
        case PnlIndicator:
          fnt = new Font(this.Font.FontFamily, this.Font.Size, FontStyle.Bold);
          txt = "Seam Detect";
          textBrush = new SolidBrush(Color.White);
          if (this.StatusBarInfo.IsSeamDetected)
            brsh = new SolidBrush(Color.RoyalBlue);
          else if (!this.StatusBarInfo.IsSeamDetectEnabled)
          {
            brsh = new SolidBrush(Color.Red);
            textBrush = new SolidBrush(Color.White);
          }
          else
          {
            if (this.StatusBarInfo.IgnoringSeams)
            {
              fnt = new Font(this.Font.FontFamily, this.Font.Size, FontStyle.Regular);
              brsh = new SolidBrush(Color.Transparent);
              textBrush = new SolidBrush(Color.Black);
            }
            else
              brsh = new SolidBrush(Color.Green);
          }
          sbdevent.Graphics.FillRectangle(brsh, sbdevent.Bounds);
          sbdevent.Graphics.DrawString(txt, fnt, textBrush, sbdevent.Bounds, sf);
          break;

        case PnlUserAttention:
          fnt = new Font(this.Font.FontFamily, this.Font.Size, FontStyle.Regular);
          txt = "User Alert";
          textBrush = new SolidBrush(Color.Black);
          if (this.StatusBarInfo.UserAttentions.Any)
          {
            fnt = new Font(this.Font.FontFamily, this.Font.Size, FontStyle.Bold);
            brsh = new SolidBrush(Color.Orange);
            textBrush = new SolidBrush(Color.White);
          }
          sbdevent.Graphics.FillRectangle(brsh, sbdevent.Bounds);
          sbdevent.Graphics.DrawString(txt, fnt, textBrush, sbdevent.Bounds, sf);
          break;

        case PnlAlarmIndex:
          fnt = new Font(this.Font.FontFamily, this.Font.Size, FontStyle.Regular);
          txt = "Major Alarm";
          textBrush = new SolidBrush(Color.Black);
          if (this.StatusBarInfo.CriticalStops.Any)
          {
            fnt = new Font(this.Font.FontFamily, this.Font.Size, FontStyle.Bold);
            brsh = new SolidBrush(Color.Red);
            textBrush = new SolidBrush(Color.White);
          }
          sbdevent.Graphics.FillRectangle(brsh, sbdevent.Bounds);
          sbdevent.Graphics.DrawString(txt, fnt, textBrush, sbdevent.Bounds, sf);
          break;

        case PnlAlertMessage:
          fnt = new Font(this.Font.FontFamily, this.Font.Size, FontStyle.Regular);
          textBrush = new SolidBrush(Color.Black);
          if (this.StatusBarInfo.CriticalStops.Any)
            txt = this.StatusBarInfo.CriticalAlarmMessage;
          else
            txt = this.StatusBarInfo.AlertMessage;
          //brsh = new SolidBrush(Color.LightGray);
          sf.Alignment = StringAlignment.Near;
          sf.LineAlignment = StringAlignment.Center;
          //sbdevent.Graphics.FillRectangle(brsh, sbdevent.Bounds);
          sbdevent.Graphics.DrawString(txt, fnt, textBrush, sbdevent.Bounds, sf);
          break;
      }
    }
  }
}
