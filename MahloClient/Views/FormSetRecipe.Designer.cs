namespace MahloClient.Views
{
  partial class FormSetRecipe
  {
    /// <summary>
    /// Required designer variable.
    /// </summary>
    private System.ComponentModel.IContainer components = null;

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
    }

    #region Windows Form Designer generated code

    /// <summary>
    /// Required method for Designer support - do not modify
    /// the contents of this method with the code editor.
    /// </summary>
    private void InitializeComponent()
    {
      this.components = new System.ComponentModel.Container();
      this.grpSelectRecipe = new System.Windows.Forms.GroupBox();
      this.radFRETI = new System.Windows.Forms.RadioButton();
      this.radNoRecipe = new System.Windows.Forms.RadioButton();
      this.radPatternDetection = new System.Windows.Forms.RadioButton();
      this.radLineDetection = new System.Windows.Forms.RadioButton();
      this.lblStyleCode = new System.Windows.Forms.Label();
      this.lblRollNumber = new System.Windows.Forms.Label();
      this.label2 = new System.Windows.Forms.Label();
      this.label1 = new System.Windows.Forms.Label();
      this.btnSave = new System.Windows.Forms.Button();
      this.btnCancel = new System.Windows.Forms.Button();
      this.grpApplyTo = new System.Windows.Forms.GroupBox();
      this.radApplyToStyle = new System.Windows.Forms.RadioButton();
      this.radApplyToRoll = new System.Windows.Forms.RadioButton();
      this.srcSelectedRoll = new System.Windows.Forms.BindingSource(this.components);
      this.grpSelectRecipe.SuspendLayout();
      this.grpApplyTo.SuspendLayout();
      ((System.ComponentModel.ISupportInitialize)(this.srcSelectedRoll)).BeginInit();
      this.SuspendLayout();
      // 
      // grpSelectRecipe
      // 
      this.grpSelectRecipe.Controls.Add(this.radFRETI);
      this.grpSelectRecipe.Controls.Add(this.radNoRecipe);
      this.grpSelectRecipe.Controls.Add(this.radPatternDetection);
      this.grpSelectRecipe.Controls.Add(this.radLineDetection);
      this.grpSelectRecipe.Font = new System.Drawing.Font("Microsoft Sans Serif", 8.25F, System.Drawing.FontStyle.Bold, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
      this.grpSelectRecipe.Location = new System.Drawing.Point(14, 45);
      this.grpSelectRecipe.Name = "grpSelectRecipe";
      this.grpSelectRecipe.Size = new System.Drawing.Size(612, 64);
      this.grpSelectRecipe.TabIndex = 12;
      this.grpSelectRecipe.TabStop = false;
      this.grpSelectRecipe.Text = "Select a recipe from the following options:";
      // 
      // radFRETI
      // 
      this.radFRETI.Font = new System.Drawing.Font("Microsoft Sans Serif", 8.25F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
      this.radFRETI.Location = new System.Drawing.Point(316, 28);
      this.radFRETI.Name = "radFRETI";
      this.radFRETI.Size = new System.Drawing.Size(136, 20);
      this.radFRETI.TabIndex = 3;
      this.radFRETI.Text = "FRETI";
      // 
      // radNoRecipe
      // 
      this.radNoRecipe.Font = new System.Drawing.Font("Microsoft Sans Serif", 8.25F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
      this.radNoRecipe.Location = new System.Drawing.Point(464, 28);
      this.radNoRecipe.Name = "radNoRecipe";
      this.radNoRecipe.Size = new System.Drawing.Size(136, 20);
      this.radNoRecipe.TabIndex = 2;
      this.radNoRecipe.Text = "None (Run in Manual)";
      // 
      // radPatternDetection
      // 
      this.radPatternDetection.Font = new System.Drawing.Font("Microsoft Sans Serif", 8.25F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
      this.radPatternDetection.Location = new System.Drawing.Point(168, 28);
      this.radPatternDetection.Name = "radPatternDetection";
      this.radPatternDetection.Size = new System.Drawing.Size(136, 20);
      this.radPatternDetection.TabIndex = 1;
      this.radPatternDetection.Text = "Pattern Detection";
      // 
      // radLineDetection
      // 
      this.radLineDetection.Font = new System.Drawing.Font("Microsoft Sans Serif", 8.25F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
      this.radLineDetection.Location = new System.Drawing.Point(20, 28);
      this.radLineDetection.Name = "radLineDetection";
      this.radLineDetection.Size = new System.Drawing.Size(136, 20);
      this.radLineDetection.TabIndex = 0;
      this.radLineDetection.Text = "Line Detection";
      // 
      // lblStyleCode
      // 
      this.lblStyleCode.DataBindings.Add(new System.Windows.Forms.Binding("Text", this.srcSelectedRoll, "StyleCode", true));
      this.lblStyleCode.Font = new System.Drawing.Font("Microsoft Sans Serif", 8.25F, System.Drawing.FontStyle.Bold, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
      this.lblStyleCode.Location = new System.Drawing.Point(62, 13);
      this.lblStyleCode.Name = "lblStyleCode";
      this.lblStyleCode.Size = new System.Drawing.Size(116, 20);
      this.lblStyleCode.TabIndex = 9;
      this.lblStyleCode.UseMnemonic = false;
      // 
      // lblRollNumber
      // 
      this.lblRollNumber.DataBindings.Add(new System.Windows.Forms.Binding("Text", this.srcSelectedRoll, "RollNo", true));
      this.lblRollNumber.Font = new System.Drawing.Font("Microsoft Sans Serif", 8.25F, System.Drawing.FontStyle.Bold, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
      this.lblRollNumber.Location = new System.Drawing.Point(234, 13);
      this.lblRollNumber.Name = "lblRollNumber";
      this.lblRollNumber.Size = new System.Drawing.Size(116, 20);
      this.lblRollNumber.TabIndex = 11;
      this.lblRollNumber.UseMnemonic = false;
      // 
      // label2
      // 
      this.label2.Location = new System.Drawing.Point(22, 13);
      this.label2.Name = "label2";
      this.label2.Size = new System.Drawing.Size(36, 20);
      this.label2.TabIndex = 8;
      this.label2.Text = "Style:";
      // 
      // label1
      // 
      this.label1.Location = new System.Drawing.Point(194, 13);
      this.label1.Name = "label1";
      this.label1.Size = new System.Drawing.Size(36, 20);
      this.label1.TabIndex = 10;
      this.label1.Text = "Roll:";
      // 
      // btnSave
      // 
      this.btnSave.BackColor = System.Drawing.SystemColors.ControlLight;
      this.btnSave.ImageAlign = System.Drawing.ContentAlignment.MiddleLeft;
      this.btnSave.ImageIndex = 1;
      this.btnSave.Location = new System.Drawing.Point(378, 221);
      this.btnSave.Name = "btnSave";
      this.btnSave.Size = new System.Drawing.Size(116, 36);
      this.btnSave.TabIndex = 14;
      this.btnSave.Text = "Save";
      this.btnSave.UseVisualStyleBackColor = false;
      this.btnSave.Click += new System.EventHandler(this.BtnSave_Click);
      // 
      // btnCancel
      // 
      this.btnCancel.BackColor = System.Drawing.SystemColors.ControlLight;
      this.btnCancel.DialogResult = System.Windows.Forms.DialogResult.Cancel;
      this.btnCancel.ImageAlign = System.Drawing.ContentAlignment.MiddleLeft;
      this.btnCancel.ImageIndex = 1;
      this.btnCancel.Location = new System.Drawing.Point(506, 221);
      this.btnCancel.Name = "btnCancel";
      this.btnCancel.Size = new System.Drawing.Size(116, 36);
      this.btnCancel.TabIndex = 15;
      this.btnCancel.Text = "Cancel";
      this.btnCancel.UseVisualStyleBackColor = false;
      this.btnCancel.Click += new System.EventHandler(this.BtnCancel_Click);
      // 
      // grpApplyTo
      // 
      this.grpApplyTo.Controls.Add(this.radApplyToStyle);
      this.grpApplyTo.Controls.Add(this.radApplyToRoll);
      this.grpApplyTo.Font = new System.Drawing.Font("Microsoft Sans Serif", 8.25F, System.Drawing.FontStyle.Bold, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
      this.grpApplyTo.Location = new System.Drawing.Point(14, 117);
      this.grpApplyTo.Name = "grpApplyTo";
      this.grpApplyTo.Size = new System.Drawing.Size(612, 96);
      this.grpApplyTo.TabIndex = 13;
      this.grpApplyTo.TabStop = false;
      this.grpApplyTo.Text = "Choose how to apply this recipe:";
      // 
      // radApplyToStyle
      // 
      this.radApplyToStyle.Font = new System.Drawing.Font("Microsoft Sans Serif", 8.25F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
      this.radApplyToStyle.Location = new System.Drawing.Point(20, 28);
      this.radApplyToStyle.Name = "radApplyToStyle";
      this.radApplyToStyle.Size = new System.Drawing.Size(368, 20);
      this.radApplyToStyle.TabIndex = 0;
      this.radApplyToStyle.Text = "Apply to this roll and ALL future rolls of this Style";
      // 
      // radApplyToRoll
      // 
      this.radApplyToRoll.Font = new System.Drawing.Font("Microsoft Sans Serif", 8.25F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
      this.radApplyToRoll.Location = new System.Drawing.Point(20, 60);
      this.radApplyToRoll.Name = "radApplyToRoll";
      this.radApplyToRoll.Size = new System.Drawing.Size(180, 20);
      this.radApplyToRoll.TabIndex = 1;
      this.radApplyToRoll.Text = "Apply only to this Roll";
      // 
      // srcSelectedRoll
      // 
      this.srcSelectedRoll.DataSource = typeof(MahloService.Models.GreigeRoll);
      // 
      // FormSetRecipe
      // 
      this.AcceptButton = this.btnSave;
      this.AutoScaleDimensions = new System.Drawing.SizeF(6F, 13F);
      this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
      this.CancelButton = this.btnCancel;
      this.ClientSize = new System.Drawing.Size(640, 271);
      this.Controls.Add(this.grpSelectRecipe);
      this.Controls.Add(this.lblStyleCode);
      this.Controls.Add(this.lblRollNumber);
      this.Controls.Add(this.label2);
      this.Controls.Add(this.label1);
      this.Controls.Add(this.btnSave);
      this.Controls.Add(this.btnCancel);
      this.Controls.Add(this.grpApplyTo);
      this.FormBorderStyle = System.Windows.Forms.FormBorderStyle.FixedDialog;
      this.Name = "FormSetRecipe";
      this.Text = "Set Recipe";
      this.grpSelectRecipe.ResumeLayout(false);
      this.grpApplyTo.ResumeLayout(false);
      ((System.ComponentModel.ISupportInitialize)(this.srcSelectedRoll)).EndInit();
      this.ResumeLayout(false);

    }

    #endregion

    private System.Windows.Forms.GroupBox grpSelectRecipe;
    private System.Windows.Forms.RadioButton radFRETI;
    private System.Windows.Forms.RadioButton radNoRecipe;
    private System.Windows.Forms.RadioButton radPatternDetection;
    private System.Windows.Forms.RadioButton radLineDetection;
    private System.Windows.Forms.Label lblStyleCode;
    private System.Windows.Forms.Label lblRollNumber;
    private System.Windows.Forms.Label label2;
    private System.Windows.Forms.Label label1;
    private System.Windows.Forms.Button btnSave;
    private System.Windows.Forms.Button btnCancel;
    private System.Windows.Forms.GroupBox grpApplyTo;
    private System.Windows.Forms.RadioButton radApplyToStyle;
    private System.Windows.Forms.RadioButton radApplyToRoll;
    private System.Windows.Forms.BindingSource srcSelectedRoll;
  }
}