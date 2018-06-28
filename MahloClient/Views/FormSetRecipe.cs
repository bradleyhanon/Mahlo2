using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;
using MahloService.Ipc;
using MahloService.Models;
using MahloClient.Ipc;

namespace MahloClient.Views
{
  partial class FormSetRecipe : Form
  {
    public const string ManualModeRecipeName = "None (Run in Manual)";

    (RadioButton button, string name)[] recipeMap;
    (RadioButton button, RecipeApplyToEnum applyTo)[] applyToMap;

    private IMahloIpcClient mahloClient;
    private GreigeRoll currentRoll;
    private GreigeRoll selectedRoll;


    public FormSetRecipe(IMahloIpcClient mahloClient, GreigeRoll currentRoll, GreigeRoll selectedRoll)
    {
      InitializeComponent();

      this.mahloClient = mahloClient;
      this.currentRoll = currentRoll;
      this.selectedRoll = selectedRoll;
      this.srcSelectedRoll.DataSource = this.selectedRoll;

      this.recipeMap = new(RadioButton button, string name)[]
      {
        (this.radNoRecipe, ManualModeRecipeName),
        (this.radPatternDetection, "Pattern Detection"),
        (this.radLineDetection, "Line Detection"),
        (this.radFRETI, "FRETI"),
      };

      this.applyToMap = new(RadioButton button, RecipeApplyToEnum name)[]
      {
        (this.radApplyToRoll, RecipeApplyToEnum.Roll),
        (this.radApplyToStyle, RecipeApplyToEnum.Style),
      };

      this.recipeMap.ForEach(item => item.button.Checked = this.selectedRoll.DefaultRecipe.ToLower() == item.name.ToLower());
      
      if (this.selectedRoll.Id >= this.currentRoll.Id)
      {
        // User can apply to style or individual roll
        this.radApplyToStyle.Checked = true;
      }
      else
      {
        // User can only apply to style
        this.radApplyToStyle.Checked = true;
        this.radApplyToRoll.Enabled = false;
      }
    }

    private async void BtnSave_Click(object sender, EventArgs e)
    {
      string recipeName = this.recipeMap.FirstOrDefault(item => item.button.Checked).name;
      var applyTo = this.applyToMap.FirstOrDefault(item => item.button.Checked).applyTo;
      (string message, string caption) = await this.mahloClient.BasSetRecipe(this.selectedRoll.RollNo, this.selectedRoll.StyleCode, recipeName, applyTo);
      if (message != string.Empty)
      {
        MessageBox.Show(this, message, caption, MessageBoxButtons.OK, MessageBoxIcon.Exclamation);
      }

      this.Close();
    }

    private void BtnCancel_Click(object sender, EventArgs e)
    {
      this.Close();
    }
  }
}
