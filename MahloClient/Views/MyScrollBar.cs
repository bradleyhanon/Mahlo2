using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Reactive.Linq;
using System.Windows.Forms;

namespace MahloClient.Views
{
  public partial class MyScrollBar : UserControl
  {
    private DataGridView grid;
    private List<IDisposable> disposables = new List<IDisposable>();
    private IDisposable dataSourceSubscription;
    private BindingSource bindingSrc;
    private bool autoScroll = true;
    private int autoScrollPosition;

    public MyScrollBar()
    {
      this.InitializeComponent();
    }

    public new int AutoScrollPosition
    {
      get => this.autoScrollPosition;
      set
      {
        if (this.autoScrollPosition != value)
        {
          this.autoScrollPosition = value;
          this.DoAutoScroll();
        }
      }
    }

    public DataGridView Grid
    {
      get => this.grid;
      set
      {
        if (this.grid != value)
        {
          if (this.grid != null)
          {
            this.dataSourceSubscription.Dispose();
            this.UnwireEvents();
          }

          this.grid = value;

          if (value != null)
          {
            this.WireUpEvents();

            this.dataSourceSubscription =
              Observable.FromEventPattern<EventHandler, EventArgs>(
                h => this.grid.DataSourceChanged += h,
                h => this.grid.DataSourceChanged -= h)
                .Subscribe(args => this.WireUpEvents());
          }
        }
      }
    }

    private void UnwireEvents()
    {
      this.disposables.ForEach(item => item.Dispose());
    }

    private void WireUpEvents()
    {
      this.UnwireEvents();
      this.bindingSrc = this.grid.DataSource as BindingSource;
      if (this.bindingSrc == null)
      {
        return;
      }

      this.scrollBar.Maximum = this.bindingSrc.Count;
      this.scrollBar.LargeChange = this.grid.DisplayedRowCount(false);
      this.disposables.Add(
        Observable.FromEventPattern<ListChangedEventHandler, ListChangedEventArgs>(
          h => this.bindingSrc.ListChanged += h,
          h => this.bindingSrc.ListChanged -= h)
          .Subscribe(args =>
          {
            this.scrollBar.Maximum = this.bindingSrc.Count;
            this.scrollBar.LargeChange = this.grid.DisplayedRowCount(false);
          }));

      this.disposables.Add(
        Observable.FromEventPattern<ScrollEventHandler, ScrollEventArgs>(
          h => this.scrollBar.Scroll += h,
          h => this.scrollBar.Scroll -= h)
          .Subscribe(args =>
          {
            this.grid.FirstDisplayedScrollingRowIndex = args.EventArgs.NewValue;
            this.autoScroll = false;
          }));

      this.disposables.Add(
        Observable.FromEventPattern<EventHandler, EventArgs>(
          h => this.grid.SizeChanged += h,
          h => this.grid.SizeChanged -= h)
          .Subscribe(args =>
          {
            this.scrollBar.LargeChange = this.grid.DisplayedRowCount(false);
          }));

      this.disposables.Add(
        Observable.FromEventPattern<EventHandler, EventArgs>(
          h => this.btnAutoScroll.Click += h,
          h => this.btnAutoScroll.Click -= h)
          .Subscribe(args =>
          {
            this.autoScroll = true;
            this.DoAutoScroll();
          }));
    }

    private void DoAutoScroll()
    {
      if (this.autoScroll && this.grid != null && this.bindingSrc.Count > 0)
      {
        this.grid.FirstDisplayedScrollingRowIndex =
          this.bindingSrc.Position =
          this.scrollBar.Value = Math.Max(0, this.AutoScrollPosition);
      }
    }
  }
}
