using System;
using System.Linq;
using System.Windows.Forms;
using MahloClient.Ipc;
using MahloService.Ipc;
using MahloService.Models;

namespace MahloClient.Views
{
  internal partial class FormSetRecipe : Form
  {
    public const string ManualModeRecipeName = "None (Run in Manual)";
    private (RadioButton button, string name)[] recipeMap;
    private (RadioButton button, RecipeApplyToEnum applyTo)[] applyToMap;

    private IMahloIpcClient mahloClient;
    private GreigeRoll currentRoll;
    private GreigeRoll selectedRoll;


    public FormSetRecipe(IMahloIpcClient mahloClient, GreigeRoll currentRoll, GreigeRoll selectedRoll)
    {
      this.InitializeComponent();

      this.mahloClient = mahloClient;
      this.currentRoll = currentRoll;
      this.selectedRoll = selectedRoll;
      this.srcSelectedRoll.DataSource = this.selectedRoll;

      this.recipeMap = new (RadioButton button, string name)[]
      {
        (this.radNoRecipe, ManualModeRecipeName),
        (this.radPatternDetection, "Pattern Detection"),
        (this.radLineDetection, "Line Detection"),
        (this.radFRETI, "FRETI"),
      };

      this.applyToMap = new (RadioButton button, RecipeApplyToEnum name)[]
      {
        (this.radApplyToRoll, RecipeApplyToEnum.Roll),
        (this.radApplyToStyle, RecipeApplyToEnum.Style),
      };

      this.recipeMap.ForEach(item => item.button.Checked = string.Equals(this.selectedRoll.DefaultRecipe, item.name, StringComparison.OrdinalIgnoreCase));

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
      (string message, string caption) = await this.mahloClient.BasSetRecipeAsync(this.selectedRoll.RollNo, this.selectedRoll.StyleCode, recipeName, applyTo);
      if (!string.IsNullOrEmpty(message))
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
