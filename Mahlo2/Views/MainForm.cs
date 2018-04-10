using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;
using Mahlo.Logic;

namespace Mahlo.Views
{
  partial class MainForm : Form
  {
    private ISewinQueue sewinQueue;
    private IMahloLogic mahloLogic;
    private IBowAndSkewLogic bowAndSkewLogic;
    private IPatternRepeatLogic patternRepeatLogic;

    public MainForm(ISewinQueue sewinQueue, IMahloLogic mahloLogic, IBowAndSkewLogic bowAndSkewLogic, IPatternRepeatLogic patternRepeatLogic)
    {
      InitializeComponent();
      this.sewinQueue = sewinQueue;
      this.mahloLogic = mahloLogic;
      this.bowAndSkewLogic = bowAndSkewLogic;
      this.patternRepeatLogic = patternRepeatLogic;
    }

    private void MainForm_Load(object sender, EventArgs e)
    {
      this.sewinGrid.DataSource = sewinQueue.Rolls;
      this.sewinQueueSource.DataSource = sewinQueue.Rolls;
      this.mahlo2Source.DataSource = mahloLogic.Rolls;
    }
  }
}
