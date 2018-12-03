using System;
using System.ComponentModel;
using System.Globalization;
using System.Windows.Forms;

namespace MahloClient.Views
{
  public partial class MoveToNextDialog : Form
  {
    public MoveToNextDialog()
    {
      this.InitializeComponent();
      Application.Idle += this.Application_Idle;
    }

    public string RollNumber
    {
      get => this.lblRollNumber.Text;
      set => this.lblRollNumber.Text = $"Roll number {value}";
    }

    public int RollLength
    {
      get => int.Parse(this.tbxLength.Text, CultureInfo.CurrentCulture);
      set => this.tbxLength.Text = value.ToString(CultureInfo.CurrentCulture);
    }

    public int MaxLength { get; set; }

    protected override void OnFormClosed(FormClosedEventArgs e)
    {
      base.OnFormClosed(e);
      Application.Idle -= this.Application_Idle;
    }

    private void Application_Idle(object sender, EventArgs e)
    {
      this.btnOk.Enabled = this.tbxLength.Text.Length > 0;
    }

    private void TbxLength_KeyPress(object sender, KeyPressEventArgs e)
    {
      e.Handled = e.KeyChar >= ' ' && (e.KeyChar < '0' || e.KeyChar > '9');
    }

    private void TbxLength_Validating(object sender, CancelEventArgs e)
    {
      if (!int.TryParse(this.tbxLength.Text, out int newLength))
      {
        this.errorProvider1.SetError(this.tbxLength, "Not an integer value");
        e.Cancel = true;
      }
      else if (newLength > this.MaxLength)
      {
        this.RollLength = this.MaxLength;
      }
    }
  }
}
