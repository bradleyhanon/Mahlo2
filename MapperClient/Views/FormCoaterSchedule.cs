using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;
using MapperClient.Ipc;

namespace MapperClient.Views
{
  partial class FormCoaterSchedule : Form
  {
    public FormCoaterSchedule(IMahloClient mahloClient)
    {
      InitializeComponent();
    }

    protected override void OnLoad(EventArgs e)
    {
      base.OnLoad(e);


    }
  }
}
