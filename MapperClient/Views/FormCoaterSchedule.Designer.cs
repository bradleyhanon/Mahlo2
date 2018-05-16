namespace MapperClient.Views
{
  partial class FormCoaterSchedule
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
      this.statusBar1 = new System.Windows.Forms.StatusBar();
      this.pnlMessage = new System.Windows.Forms.StatusBarPanel();
      this.btnToggleView = new System.Windows.Forms.Button();
      this.btnClose = new System.Windows.Forms.Button();
      this.btnWhichOrders = new System.Windows.Forms.Button();
      this.dbgCoaterSchedule = new System.Windows.Forms.DataGridView();
      this.dbgBackingSummary = new System.Windows.Forms.DataGridView();
      ((System.ComponentModel.ISupportInitialize)(this.pnlMessage)).BeginInit();
      ((System.ComponentModel.ISupportInitialize)(this.dbgCoaterSchedule)).BeginInit();
      ((System.ComponentModel.ISupportInitialize)(this.dbgBackingSummary)).BeginInit();
      this.SuspendLayout();
      // 
      // statusBar1
      // 
      this.statusBar1.Location = new System.Drawing.Point(0, 426);
      this.statusBar1.Name = "statusBar1";
      this.statusBar1.Panels.AddRange(new System.Windows.Forms.StatusBarPanel[] {
            this.pnlMessage});
      this.statusBar1.ShowPanels = true;
      this.statusBar1.Size = new System.Drawing.Size(876, 27);
      this.statusBar1.TabIndex = 7;
      // 
      // pnlMessage
      // 
      this.pnlMessage.AutoSize = System.Windows.Forms.StatusBarPanelAutoSize.Spring;
      this.pnlMessage.Name = "pnlMessage";
      this.pnlMessage.Width = 859;
      // 
      // btnToggleView
      // 
      this.btnToggleView.Anchor = ((System.Windows.Forms.AnchorStyles)((System.Windows.Forms.AnchorStyles.Bottom | System.Windows.Forms.AnchorStyles.Left)));
      this.btnToggleView.BackColor = System.Drawing.SystemColors.ControlLight;
      this.btnToggleView.ImageAlign = System.Drawing.ContentAlignment.MiddleLeft;
      this.btnToggleView.ImageIndex = 1;
      this.btnToggleView.Location = new System.Drawing.Point(12, 384);
      this.btnToggleView.Name = "btnToggleView";
      this.btnToggleView.Size = new System.Drawing.Size(116, 36);
      this.btnToggleView.TabIndex = 4;
      this.btnToggleView.Text = "Show Summary";
      this.btnToggleView.UseVisualStyleBackColor = false;
      // 
      // btnClose
      // 
      this.btnClose.Anchor = ((System.Windows.Forms.AnchorStyles)((System.Windows.Forms.AnchorStyles.Bottom | System.Windows.Forms.AnchorStyles.Right)));
      this.btnClose.BackColor = System.Drawing.SystemColors.ControlLight;
      this.btnClose.DialogResult = System.Windows.Forms.DialogResult.Cancel;
      this.btnClose.Location = new System.Drawing.Point(744, 384);
      this.btnClose.Name = "btnClose";
      this.btnClose.Size = new System.Drawing.Size(116, 36);
      this.btnClose.TabIndex = 6;
      this.btnClose.Text = "Close";
      this.btnClose.UseVisualStyleBackColor = false;
      // 
      // btnWhichOrders
      // 
      this.btnWhichOrders.Anchor = ((System.Windows.Forms.AnchorStyles)((System.Windows.Forms.AnchorStyles.Bottom | System.Windows.Forms.AnchorStyles.Left)));
      this.btnWhichOrders.BackColor = System.Drawing.SystemColors.ControlLight;
      this.btnWhichOrders.ImageAlign = System.Drawing.ContentAlignment.MiddleLeft;
      this.btnWhichOrders.ImageIndex = 1;
      this.btnWhichOrders.Location = new System.Drawing.Point(140, 384);
      this.btnWhichOrders.Name = "btnWhichOrders";
      this.btnWhichOrders.Size = new System.Drawing.Size(116, 36);
      this.btnWhichOrders.TabIndex = 5;
      this.btnWhichOrders.Text = "All Orders";
      this.btnWhichOrders.UseVisualStyleBackColor = false;
      // 
      // dbgCoaterSchedule
      // 
      this.dbgCoaterSchedule.Anchor = ((System.Windows.Forms.AnchorStyles)((((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Bottom) 
            | System.Windows.Forms.AnchorStyles.Left) 
            | System.Windows.Forms.AnchorStyles.Right)));
      this.dbgCoaterSchedule.ColumnHeadersHeightSizeMode = System.Windows.Forms.DataGridViewColumnHeadersHeightSizeMode.AutoSize;
      this.dbgCoaterSchedule.Location = new System.Drawing.Point(13, 13);
      this.dbgCoaterSchedule.Name = "dbgCoaterSchedule";
      this.dbgCoaterSchedule.Size = new System.Drawing.Size(847, 365);
      this.dbgCoaterSchedule.TabIndex = 8;
      // 
      // dbgBackingSummary
      // 
      this.dbgBackingSummary.ColumnHeadersHeightSizeMode = System.Windows.Forms.DataGridViewColumnHeadersHeightSizeMode.AutoSize;
      this.dbgBackingSummary.Location = new System.Drawing.Point(13, 13);
      this.dbgBackingSummary.Name = "dbgBackingSummary";
      this.dbgBackingSummary.Size = new System.Drawing.Size(274, 162);
      this.dbgBackingSummary.TabIndex = 9;
      // 
      // FormCoaterSchedule
      // 
      this.AutoScaleDimensions = new System.Drawing.SizeF(6F, 13F);
      this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
      this.ClientSize = new System.Drawing.Size(876, 453);
      this.Controls.Add(this.dbgBackingSummary);
      this.Controls.Add(this.dbgCoaterSchedule);
      this.Controls.Add(this.statusBar1);
      this.Controls.Add(this.btnToggleView);
      this.Controls.Add(this.btnClose);
      this.Controls.Add(this.btnWhichOrders);
      this.FormBorderStyle = System.Windows.Forms.FormBorderStyle.FixedDialog;
      this.Name = "FormCoaterSchedule";
      this.Text = "FormCoaterSchedule";
      ((System.ComponentModel.ISupportInitialize)(this.pnlMessage)).EndInit();
      ((System.ComponentModel.ISupportInitialize)(this.dbgCoaterSchedule)).EndInit();
      ((System.ComponentModel.ISupportInitialize)(this.dbgBackingSummary)).EndInit();
      this.ResumeLayout(false);

    }

    #endregion

    private System.Windows.Forms.StatusBar statusBar1;
    private System.Windows.Forms.StatusBarPanel pnlMessage;
    private System.Windows.Forms.Button btnToggleView;
    private System.Windows.Forms.Button btnClose;
    private System.Windows.Forms.Button btnWhichOrders;
    private System.Windows.Forms.DataGridView dbgCoaterSchedule;
    private System.Windows.Forms.DataGridView dbgBackingSummary;
  }
}