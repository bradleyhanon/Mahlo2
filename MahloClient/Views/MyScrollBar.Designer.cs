namespace MahloClient.Views
{
  partial class MyScrollBar
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

    #region Component Designer generated code

    /// <summary> 
    /// Required method for Designer support - do not modify 
    /// the contents of this method with the code editor.
    /// </summary>
    private void InitializeComponent()
    {
      this.components = new System.ComponentModel.Container();
      this.scrollBar = new System.Windows.Forms.VScrollBar();
      this.btnAutoScroll = new System.Windows.Forms.Button();
      this.toolTip1 = new System.Windows.Forms.ToolTip(this.components);
      this.SuspendLayout();
      // 
      // scrollBar
      // 
      this.scrollBar.Anchor = ((System.Windows.Forms.AnchorStyles)(((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Bottom) 
            | System.Windows.Forms.AnchorStyles.Left)));
      this.scrollBar.Location = new System.Drawing.Point(0, 2);
      this.scrollBar.Name = "scrollBar";
      this.scrollBar.Size = new System.Drawing.Size(17, 182);
      this.scrollBar.TabIndex = 0;
      // 
      // btnAutoScroll
      // 
      this.btnAutoScroll.Anchor = ((System.Windows.Forms.AnchorStyles)((System.Windows.Forms.AnchorStyles.Bottom | System.Windows.Forms.AnchorStyles.Left)));
      this.btnAutoScroll.BackgroundImageLayout = System.Windows.Forms.ImageLayout.Center;
      this.btnAutoScroll.FlatAppearance.BorderColor = System.Drawing.Color.White;
      this.btnAutoScroll.FlatAppearance.MouseDownBackColor = System.Drawing.Color.Gray;
      this.btnAutoScroll.FlatAppearance.MouseOverBackColor = System.Drawing.Color.FromArgb(((int)(((byte)(224)))), ((int)(((byte)(224)))), ((int)(((byte)(224)))));
      this.btnAutoScroll.FlatStyle = System.Windows.Forms.FlatStyle.Flat;
      this.btnAutoScroll.Image = global::MahloClient.Properties.Resources.StackedV;
      this.btnAutoScroll.Location = new System.Drawing.Point(0, 188);
      this.btnAutoScroll.Name = "btnAutoScroll";
      this.btnAutoScroll.Size = new System.Drawing.Size(17, 17);
      this.btnAutoScroll.TabIndex = 26;
      this.btnAutoScroll.Text = "";
      this.toolTip1.SetToolTip(this.btnAutoScroll, "Auto Scroll");
      this.btnAutoScroll.UseVisualStyleBackColor = true;
      // 
      // MyScrollBar
      // 
      this.AutoScaleDimensions = new System.Drawing.SizeF(6F, 13F);
      this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
      this.Controls.Add(this.btnAutoScroll);
      this.Controls.Add(this.scrollBar);
      this.Name = "MyScrollBar";
      this.Size = new System.Drawing.Size(17, 207);
      this.ResumeLayout(false);

    }

    #endregion

    private System.Windows.Forms.VScrollBar scrollBar;
    private System.Windows.Forms.Button btnAutoScroll;
    private System.Windows.Forms.ToolTip toolTip1;
  }
}
