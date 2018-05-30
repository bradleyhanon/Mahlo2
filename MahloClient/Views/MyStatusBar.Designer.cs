namespace MahloClient.Views
{
  partial class MyStatusBar
  {
    #region Component Designer generated code

    /// <summary> 
    /// Required method for Designer support - do not modify 
    /// the contents of this method with the code editor.
    /// </summary>
    private void InitializeComponent()
    {
      this.statusBar1 = new System.Windows.Forms.StatusBar();
      this.pnlMessage = new System.Windows.Forms.StatusBarPanel();
      this.pnlIndicator = new System.Windows.Forms.StatusBarPanel();
      this.pnlUserAttention = new System.Windows.Forms.StatusBarPanel();
      this.pnlAlarm = new System.Windows.Forms.StatusBarPanel();
      this.pnlAlertMessage = new System.Windows.Forms.StatusBarPanel();
      this.pnlQueueMessage = new System.Windows.Forms.StatusBarPanel();
      ((System.ComponentModel.ISupportInitialize)(this.pnlMessage)).BeginInit();
      ((System.ComponentModel.ISupportInitialize)(this.pnlIndicator)).BeginInit();
      ((System.ComponentModel.ISupportInitialize)(this.pnlUserAttention)).BeginInit();
      ((System.ComponentModel.ISupportInitialize)(this.pnlAlarm)).BeginInit();
      ((System.ComponentModel.ISupportInitialize)(this.pnlAlertMessage)).BeginInit();
      ((System.ComponentModel.ISupportInitialize)(this.pnlQueueMessage)).BeginInit();
      this.SuspendLayout();
      // 
      // statusBar1
      // 
      this.statusBar1.Dock = System.Windows.Forms.DockStyle.Fill;
      this.statusBar1.Font = new System.Drawing.Font("Microsoft Sans Serif", 8.25F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
      this.statusBar1.Location = new System.Drawing.Point(0, 0);
      this.statusBar1.Name = "statusBar1";
      this.statusBar1.Panels.AddRange(new System.Windows.Forms.StatusBarPanel[] {
            this.pnlMessage,
            this.pnlIndicator,
            this.pnlUserAttention,
            this.pnlAlarm,
            this.pnlAlertMessage,
            this.pnlQueueMessage});
      this.statusBar1.ShowPanels = true;
      this.statusBar1.Size = new System.Drawing.Size(963, 24);
      this.statusBar1.TabIndex = 8;
      this.statusBar1.Text = "statusBar1";
      this.statusBar1.DrawItem += new System.Windows.Forms.StatusBarDrawItemEventHandler(this.statusBar1_DrawItem);
      // 
      // pnlMessage
      // 
      this.pnlMessage.Name = "pnlMessage";
      this.pnlMessage.Width = 250;
      // 
      // pnlIndicator
      // 
      this.pnlIndicator.Alignment = System.Windows.Forms.HorizontalAlignment.Center;
      this.pnlIndicator.BorderStyle = System.Windows.Forms.StatusBarPanelBorderStyle.Raised;
      this.pnlIndicator.Name = "pnlIndicator";
      this.pnlIndicator.Style = System.Windows.Forms.StatusBarPanelStyle.OwnerDraw;
      this.pnlIndicator.Text = "Ind";
      this.pnlIndicator.Width = 90;
      // 
      // pnlUserAttention
      // 
      this.pnlUserAttention.Alignment = System.Windows.Forms.HorizontalAlignment.Center;
      this.pnlUserAttention.BorderStyle = System.Windows.Forms.StatusBarPanelBorderStyle.Raised;
      this.pnlUserAttention.Name = "pnlUserAttention";
      this.pnlUserAttention.Style = System.Windows.Forms.StatusBarPanelStyle.OwnerDraw;
      this.pnlUserAttention.Text = "Att";
      this.pnlUserAttention.Width = 90;
      // 
      // pnlAlarm
      // 
      this.pnlAlarm.Alignment = System.Windows.Forms.HorizontalAlignment.Center;
      this.pnlAlarm.BorderStyle = System.Windows.Forms.StatusBarPanelBorderStyle.Raised;
      this.pnlAlarm.Name = "pnlAlarm";
      this.pnlAlarm.Style = System.Windows.Forms.StatusBarPanelStyle.OwnerDraw;
      this.pnlAlarm.Text = "Err";
      this.pnlAlarm.Width = 90;
      // 
      // pnlAlertMessage
      // 
      this.pnlAlertMessage.AutoSize = System.Windows.Forms.StatusBarPanelAutoSize.Spring;
      this.pnlAlertMessage.Name = "pnlAlertMessage";
      this.pnlAlertMessage.Style = System.Windows.Forms.StatusBarPanelStyle.OwnerDraw;
      this.pnlAlertMessage.Width = 326;
      // 
      // pnlQueueMessage
      // 
      this.pnlQueueMessage.Alignment = System.Windows.Forms.HorizontalAlignment.Center;
      this.pnlQueueMessage.BorderStyle = System.Windows.Forms.StatusBarPanelBorderStyle.Raised;
      this.pnlQueueMessage.Name = "pnlQueueMessage";
      // 
      // UserControl1
      // 
      this.AutoScaleDimensions = new System.Drawing.SizeF(6F, 13F);
      this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
      this.Controls.Add(this.statusBar1);
      this.Name = "UserControl1";
      this.Size = new System.Drawing.Size(963, 24);
      ((System.ComponentModel.ISupportInitialize)(this.pnlMessage)).EndInit();
      ((System.ComponentModel.ISupportInitialize)(this.pnlIndicator)).EndInit();
      ((System.ComponentModel.ISupportInitialize)(this.pnlUserAttention)).EndInit();
      ((System.ComponentModel.ISupportInitialize)(this.pnlAlarm)).EndInit();
      ((System.ComponentModel.ISupportInitialize)(this.pnlAlertMessage)).EndInit();
      ((System.ComponentModel.ISupportInitialize)(this.pnlQueueMessage)).EndInit();
      this.ResumeLayout(false);

    }

    #endregion

    private System.Windows.Forms.StatusBar statusBar1;
    private System.Windows.Forms.StatusBarPanel pnlMessage;
    private System.Windows.Forms.StatusBarPanel pnlIndicator;
    private System.Windows.Forms.StatusBarPanel pnlUserAttention;
    private System.Windows.Forms.StatusBarPanel pnlAlarm;
    private System.Windows.Forms.StatusBarPanel pnlAlertMessage;
    private System.Windows.Forms.StatusBarPanel pnlQueueMessage;
  }
}
