namespace MahloClient.Views
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
      this.components = new System.ComponentModel.Container();
      System.Windows.Forms.DataGridViewCellStyle dataGridViewCellStyle1 = new System.Windows.Forms.DataGridViewCellStyle();
      System.Windows.Forms.DataGridViewCellStyle dataGridViewCellStyle2 = new System.Windows.Forms.DataGridViewCellStyle();
      System.Windows.Forms.DataGridViewCellStyle dataGridViewCellStyle3 = new System.Windows.Forms.DataGridViewCellStyle();
      System.Windows.Forms.DataGridViewCellStyle dataGridViewCellStyle4 = new System.Windows.Forms.DataGridViewCellStyle();
      System.Windows.Forms.DataGridViewCellStyle dataGridViewCellStyle5 = new System.Windows.Forms.DataGridViewCellStyle();
      System.Windows.Forms.DataGridViewCellStyle dataGridViewCellStyle6 = new System.Windows.Forms.DataGridViewCellStyle();
      System.Windows.Forms.DataGridViewCellStyle dataGridViewCellStyle7 = new System.Windows.Forms.DataGridViewCellStyle();
      System.Windows.Forms.DataGridViewCellStyle dataGridViewCellStyle8 = new System.Windows.Forms.DataGridViewCellStyle();
      System.Windows.Forms.DataGridViewCellStyle dataGridViewCellStyle9 = new System.Windows.Forms.DataGridViewCellStyle();
      System.Windows.Forms.DataGridViewCellStyle dataGridViewCellStyle10 = new System.Windows.Forms.DataGridViewCellStyle();
      System.Windows.Forms.DataGridViewCellStyle dataGridViewCellStyle11 = new System.Windows.Forms.DataGridViewCellStyle();
      System.Windows.Forms.DataGridViewCellStyle dataGridViewCellStyle12 = new System.Windows.Forms.DataGridViewCellStyle();
      System.Windows.Forms.DataGridViewCellStyle dataGridViewCellStyle13 = new System.Windows.Forms.DataGridViewCellStyle();
      this.statusBar1 = new System.Windows.Forms.StatusBar();
      this.pnlMessage = new System.Windows.Forms.StatusBarPanel();
      this.btnToggleView = new System.Windows.Forms.Button();
      this.btnClose = new System.Windows.Forms.Button();
      this.btnWhichOrders = new System.Windows.Forms.Button();
      this.dbgCoaterSchedule = new System.Windows.Forms.DataGridView();
      this.dbgBackingSummary = new System.Windows.Forms.DataGridView();
      this.seqNoDataGridViewTextBoxColumn = new System.Windows.Forms.DataGridViewTextBoxColumn();
      this.schedNoDataGridViewTextBoxColumn = new System.Windows.Forms.DataGridViewTextBoxColumn();
      this.styleDataGridViewTextBoxColumn = new System.Windows.Forms.DataGridViewTextBoxColumn();
      this.colorDataGridViewTextBoxColumn = new System.Windows.Forms.DataGridViewTextBoxColumn();
      this.backingDataGridViewTextBoxColumn = new System.Windows.Forms.DataGridViewTextBoxColumn();
      this.cutLengthDataGridViewTextBoxColumn = new System.Windows.Forms.DataGridViewTextBoxColumn();
      this.faceWtDataGridViewTextBoxColumn = new System.Windows.Forms.DataGridViewTextBoxColumn();
      this.promisedDataGridViewTextBoxColumn = new System.Windows.Forms.DataGridViewTextBoxColumn();
      this.processDataGridViewTextBoxColumn = new System.Windows.Forms.DataGridViewTextBoxColumn();
      this.rollsDataGridViewTextBoxColumn = new System.Windows.Forms.DataGridViewTextBoxColumn();
      this.feetDataGridViewTextBoxColumn = new System.Windows.Forms.DataGridViewTextBoxColumn();
      this.minutesDataGridViewTextBoxColumn = new System.Windows.Forms.DataGridViewTextBoxColumn();
      this.origSeqDataGridViewTextBoxColumn = new System.Windows.Forms.DataGridViewTextBoxColumn();
      this.sewninDataGridViewTextBoxColumn = new System.Windows.Forms.DataGridViewTextBoxColumn();
      this.rushDataGridViewTextBoxColumn = new System.Windows.Forms.DataGridViewTextBoxColumn();
      this.commentDataGridViewTextBoxColumn = new System.Windows.Forms.DataGridViewTextBoxColumn();
      this.comment2DataGridViewTextBoxColumn = new System.Windows.Forms.DataGridViewTextBoxColumn();
      this.srcCoaterSchedule = new System.Windows.Forms.BindingSource(this.components);
      ((System.ComponentModel.ISupportInitialize)(this.pnlMessage)).BeginInit();
      ((System.ComponentModel.ISupportInitialize)(this.dbgCoaterSchedule)).BeginInit();
      ((System.ComponentModel.ISupportInitialize)(this.dbgBackingSummary)).BeginInit();
      ((System.ComponentModel.ISupportInitialize)(this.srcCoaterSchedule)).BeginInit();
      this.SuspendLayout();
      // 
      // statusBar1
      // 
      this.statusBar1.Location = new System.Drawing.Point(0, 426);
      this.statusBar1.Name = "statusBar1";
      this.statusBar1.Panels.AddRange(new System.Windows.Forms.StatusBarPanel[] {
            this.pnlMessage});
      this.statusBar1.ShowPanels = true;
      this.statusBar1.Size = new System.Drawing.Size(1034, 27);
      this.statusBar1.TabIndex = 7;
      // 
      // pnlMessage
      // 
      this.pnlMessage.AutoSize = System.Windows.Forms.StatusBarPanelAutoSize.Spring;
      this.pnlMessage.Name = "pnlMessage";
      this.pnlMessage.Width = 1017;
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
      this.btnToggleView.Click += new System.EventHandler(this.btnToggleView_Click);
      // 
      // btnClose
      // 
      this.btnClose.Anchor = ((System.Windows.Forms.AnchorStyles)((System.Windows.Forms.AnchorStyles.Bottom | System.Windows.Forms.AnchorStyles.Right)));
      this.btnClose.BackColor = System.Drawing.SystemColors.ControlLight;
      this.btnClose.DialogResult = System.Windows.Forms.DialogResult.Cancel;
      this.btnClose.Location = new System.Drawing.Point(902, 384);
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
      this.btnWhichOrders.Click += new System.EventHandler(this.btnWhichOrders_Click);
      // 
      // dbgCoaterSchedule
      // 
      this.dbgCoaterSchedule.AllowUserToAddRows = false;
      this.dbgCoaterSchedule.AllowUserToDeleteRows = false;
      this.dbgCoaterSchedule.AllowUserToResizeRows = false;
      this.dbgCoaterSchedule.Anchor = ((System.Windows.Forms.AnchorStyles)((((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Bottom) 
            | System.Windows.Forms.AnchorStyles.Left) 
            | System.Windows.Forms.AnchorStyles.Right)));
      this.dbgCoaterSchedule.AutoGenerateColumns = false;
      this.dbgCoaterSchedule.AutoSizeColumnsMode = System.Windows.Forms.DataGridViewAutoSizeColumnsMode.AllCells;
      this.dbgCoaterSchedule.ColumnHeadersHeightSizeMode = System.Windows.Forms.DataGridViewColumnHeadersHeightSizeMode.AutoSize;
      this.dbgCoaterSchedule.Columns.AddRange(new System.Windows.Forms.DataGridViewColumn[] {
            this.seqNoDataGridViewTextBoxColumn,
            this.schedNoDataGridViewTextBoxColumn,
            this.styleDataGridViewTextBoxColumn,
            this.colorDataGridViewTextBoxColumn,
            this.backingDataGridViewTextBoxColumn,
            this.cutLengthDataGridViewTextBoxColumn,
            this.faceWtDataGridViewTextBoxColumn,
            this.promisedDataGridViewTextBoxColumn,
            this.processDataGridViewTextBoxColumn,
            this.rollsDataGridViewTextBoxColumn,
            this.feetDataGridViewTextBoxColumn,
            this.minutesDataGridViewTextBoxColumn,
            this.origSeqDataGridViewTextBoxColumn,
            this.sewninDataGridViewTextBoxColumn,
            this.rushDataGridViewTextBoxColumn,
            this.commentDataGridViewTextBoxColumn,
            this.comment2DataGridViewTextBoxColumn});
      this.dbgCoaterSchedule.DataSource = this.srcCoaterSchedule;
      this.dbgCoaterSchedule.Location = new System.Drawing.Point(7, 10);
      this.dbgCoaterSchedule.Name = "dbgCoaterSchedule";
      this.dbgCoaterSchedule.ReadOnly = true;
      this.dbgCoaterSchedule.RowHeadersVisible = false;
      this.dbgCoaterSchedule.Size = new System.Drawing.Size(1018, 368);
      this.dbgCoaterSchedule.TabIndex = 8;
      // 
      // dbgBackingSummary
      // 
      this.dbgBackingSummary.AllowUserToAddRows = false;
      this.dbgBackingSummary.AllowUserToDeleteRows = false;
      this.dbgBackingSummary.AllowUserToResizeColumns = false;
      this.dbgBackingSummary.AllowUserToResizeRows = false;
      this.dbgBackingSummary.ColumnHeadersHeightSizeMode = System.Windows.Forms.DataGridViewColumnHeadersHeightSizeMode.AutoSize;
      this.dbgBackingSummary.Location = new System.Drawing.Point(7, 10);
      this.dbgBackingSummary.Name = "dbgBackingSummary";
      this.dbgBackingSummary.RowHeadersVisible = false;
      this.dbgBackingSummary.Size = new System.Drawing.Size(220, 156);
      this.dbgBackingSummary.TabIndex = 9;
      this.dbgBackingSummary.Visible = false;
      // 
      // seqNoDataGridViewTextBoxColumn
      // 
      this.seqNoDataGridViewTextBoxColumn.DataPropertyName = "SeqNo";
      dataGridViewCellStyle1.Alignment = System.Windows.Forms.DataGridViewContentAlignment.MiddleCenter;
      dataGridViewCellStyle1.NullValue = null;
      this.seqNoDataGridViewTextBoxColumn.DefaultCellStyle = dataGridViewCellStyle1;
      this.seqNoDataGridViewTextBoxColumn.HeaderText = "Seq #";
      this.seqNoDataGridViewTextBoxColumn.Name = "seqNoDataGridViewTextBoxColumn";
      this.seqNoDataGridViewTextBoxColumn.ReadOnly = true;
      this.seqNoDataGridViewTextBoxColumn.SortMode = System.Windows.Forms.DataGridViewColumnSortMode.NotSortable;
      this.seqNoDataGridViewTextBoxColumn.Width = 42;
      // 
      // schedNoDataGridViewTextBoxColumn
      // 
      this.schedNoDataGridViewTextBoxColumn.DataPropertyName = "SchedNo";
      this.schedNoDataGridViewTextBoxColumn.HeaderText = "Sched #";
      this.schedNoDataGridViewTextBoxColumn.Name = "schedNoDataGridViewTextBoxColumn";
      this.schedNoDataGridViewTextBoxColumn.ReadOnly = true;
      this.schedNoDataGridViewTextBoxColumn.SortMode = System.Windows.Forms.DataGridViewColumnSortMode.NotSortable;
      this.schedNoDataGridViewTextBoxColumn.Width = 54;
      // 
      // styleDataGridViewTextBoxColumn
      // 
      this.styleDataGridViewTextBoxColumn.DataPropertyName = "Style";
      dataGridViewCellStyle2.Font = new System.Drawing.Font("Microsoft Sans Serif", 8.25F, System.Drawing.FontStyle.Bold, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
      this.styleDataGridViewTextBoxColumn.DefaultCellStyle = dataGridViewCellStyle2;
      this.styleDataGridViewTextBoxColumn.HeaderText = "Style";
      this.styleDataGridViewTextBoxColumn.Name = "styleDataGridViewTextBoxColumn";
      this.styleDataGridViewTextBoxColumn.ReadOnly = true;
      this.styleDataGridViewTextBoxColumn.SortMode = System.Windows.Forms.DataGridViewColumnSortMode.NotSortable;
      this.styleDataGridViewTextBoxColumn.Width = 36;
      // 
      // colorDataGridViewTextBoxColumn
      // 
      this.colorDataGridViewTextBoxColumn.DataPropertyName = "Color";
      this.colorDataGridViewTextBoxColumn.HeaderText = "Color";
      this.colorDataGridViewTextBoxColumn.Name = "colorDataGridViewTextBoxColumn";
      this.colorDataGridViewTextBoxColumn.ReadOnly = true;
      this.colorDataGridViewTextBoxColumn.SortMode = System.Windows.Forms.DataGridViewColumnSortMode.NotSortable;
      this.colorDataGridViewTextBoxColumn.Width = 37;
      // 
      // backingDataGridViewTextBoxColumn
      // 
      this.backingDataGridViewTextBoxColumn.DataPropertyName = "Backing";
      dataGridViewCellStyle3.Alignment = System.Windows.Forms.DataGridViewContentAlignment.MiddleCenter;
      this.backingDataGridViewTextBoxColumn.DefaultCellStyle = dataGridViewCellStyle3;
      this.backingDataGridViewTextBoxColumn.HeaderText = "Backing";
      this.backingDataGridViewTextBoxColumn.Name = "backingDataGridViewTextBoxColumn";
      this.backingDataGridViewTextBoxColumn.ReadOnly = true;
      this.backingDataGridViewTextBoxColumn.SortMode = System.Windows.Forms.DataGridViewColumnSortMode.NotSortable;
      this.backingDataGridViewTextBoxColumn.Width = 52;
      // 
      // cutLengthDataGridViewTextBoxColumn
      // 
      this.cutLengthDataGridViewTextBoxColumn.DataPropertyName = "CutLength";
      dataGridViewCellStyle4.Alignment = System.Windows.Forms.DataGridViewContentAlignment.MiddleCenter;
      this.cutLengthDataGridViewTextBoxColumn.DefaultCellStyle = dataGridViewCellStyle4;
      this.cutLengthDataGridViewTextBoxColumn.HeaderText = "Cut Length";
      this.cutLengthDataGridViewTextBoxColumn.Name = "cutLengthDataGridViewTextBoxColumn";
      this.cutLengthDataGridViewTextBoxColumn.ReadOnly = true;
      this.cutLengthDataGridViewTextBoxColumn.SortMode = System.Windows.Forms.DataGridViewColumnSortMode.NotSortable;
      this.cutLengthDataGridViewTextBoxColumn.Width = 65;
      // 
      // faceWtDataGridViewTextBoxColumn
      // 
      this.faceWtDataGridViewTextBoxColumn.DataPropertyName = "FaceWt";
      dataGridViewCellStyle5.Alignment = System.Windows.Forms.DataGridViewContentAlignment.MiddleCenter;
      this.faceWtDataGridViewTextBoxColumn.DefaultCellStyle = dataGridViewCellStyle5;
      this.faceWtDataGridViewTextBoxColumn.HeaderText = "Face Wt";
      this.faceWtDataGridViewTextBoxColumn.Name = "faceWtDataGridViewTextBoxColumn";
      this.faceWtDataGridViewTextBoxColumn.ReadOnly = true;
      this.faceWtDataGridViewTextBoxColumn.SortMode = System.Windows.Forms.DataGridViewColumnSortMode.NotSortable;
      this.faceWtDataGridViewTextBoxColumn.Width = 54;
      // 
      // promisedDataGridViewTextBoxColumn
      // 
      this.promisedDataGridViewTextBoxColumn.DataPropertyName = "Promised";
      dataGridViewCellStyle6.Alignment = System.Windows.Forms.DataGridViewContentAlignment.MiddleCenter;
      dataGridViewCellStyle6.Format = "MM/dd/yy";
      dataGridViewCellStyle6.NullValue = null;
      this.promisedDataGridViewTextBoxColumn.DefaultCellStyle = dataGridViewCellStyle6;
      this.promisedDataGridViewTextBoxColumn.HeaderText = "Promised";
      this.promisedDataGridViewTextBoxColumn.Name = "promisedDataGridViewTextBoxColumn";
      this.promisedDataGridViewTextBoxColumn.ReadOnly = true;
      this.promisedDataGridViewTextBoxColumn.SortMode = System.Windows.Forms.DataGridViewColumnSortMode.NotSortable;
      this.promisedDataGridViewTextBoxColumn.Width = 56;
      // 
      // processDataGridViewTextBoxColumn
      // 
      this.processDataGridViewTextBoxColumn.DataPropertyName = "Process";
      dataGridViewCellStyle7.Alignment = System.Windows.Forms.DataGridViewContentAlignment.MiddleCenter;
      this.processDataGridViewTextBoxColumn.DefaultCellStyle = dataGridViewCellStyle7;
      this.processDataGridViewTextBoxColumn.HeaderText = "Process";
      this.processDataGridViewTextBoxColumn.Name = "processDataGridViewTextBoxColumn";
      this.processDataGridViewTextBoxColumn.ReadOnly = true;
      this.processDataGridViewTextBoxColumn.SortMode = System.Windows.Forms.DataGridViewColumnSortMode.NotSortable;
      this.processDataGridViewTextBoxColumn.Width = 51;
      // 
      // rollsDataGridViewTextBoxColumn
      // 
      this.rollsDataGridViewTextBoxColumn.DataPropertyName = "Rolls";
      dataGridViewCellStyle8.Alignment = System.Windows.Forms.DataGridViewContentAlignment.MiddleCenter;
      this.rollsDataGridViewTextBoxColumn.DefaultCellStyle = dataGridViewCellStyle8;
      this.rollsDataGridViewTextBoxColumn.HeaderText = "Rolls";
      this.rollsDataGridViewTextBoxColumn.Name = "rollsDataGridViewTextBoxColumn";
      this.rollsDataGridViewTextBoxColumn.ReadOnly = true;
      this.rollsDataGridViewTextBoxColumn.SortMode = System.Windows.Forms.DataGridViewColumnSortMode.NotSortable;
      this.rollsDataGridViewTextBoxColumn.Width = 36;
      // 
      // feetDataGridViewTextBoxColumn
      // 
      this.feetDataGridViewTextBoxColumn.DataPropertyName = "Feet";
      dataGridViewCellStyle9.Alignment = System.Windows.Forms.DataGridViewContentAlignment.MiddleCenter;
      dataGridViewCellStyle9.Format = "#";
      this.feetDataGridViewTextBoxColumn.DefaultCellStyle = dataGridViewCellStyle9;
      this.feetDataGridViewTextBoxColumn.HeaderText = "Feet";
      this.feetDataGridViewTextBoxColumn.Name = "feetDataGridViewTextBoxColumn";
      this.feetDataGridViewTextBoxColumn.ReadOnly = true;
      this.feetDataGridViewTextBoxColumn.SortMode = System.Windows.Forms.DataGridViewColumnSortMode.NotSortable;
      this.feetDataGridViewTextBoxColumn.Width = 34;
      // 
      // minutesDataGridViewTextBoxColumn
      // 
      this.minutesDataGridViewTextBoxColumn.DataPropertyName = "Minutes";
      dataGridViewCellStyle10.Alignment = System.Windows.Forms.DataGridViewContentAlignment.MiddleCenter;
      dataGridViewCellStyle10.Format = "#";
      this.minutesDataGridViewTextBoxColumn.DefaultCellStyle = dataGridViewCellStyle10;
      this.minutesDataGridViewTextBoxColumn.HeaderText = "Minutes";
      this.minutesDataGridViewTextBoxColumn.Name = "minutesDataGridViewTextBoxColumn";
      this.minutesDataGridViewTextBoxColumn.ReadOnly = true;
      this.minutesDataGridViewTextBoxColumn.SortMode = System.Windows.Forms.DataGridViewColumnSortMode.NotSortable;
      this.minutesDataGridViewTextBoxColumn.Width = 50;
      // 
      // origSeqDataGridViewTextBoxColumn
      // 
      this.origSeqDataGridViewTextBoxColumn.DataPropertyName = "OrigSeq";
      dataGridViewCellStyle11.Alignment = System.Windows.Forms.DataGridViewContentAlignment.MiddleCenter;
      this.origSeqDataGridViewTextBoxColumn.DefaultCellStyle = dataGridViewCellStyle11;
      this.origSeqDataGridViewTextBoxColumn.HeaderText = "Orig Seq";
      this.origSeqDataGridViewTextBoxColumn.Name = "origSeqDataGridViewTextBoxColumn";
      this.origSeqDataGridViewTextBoxColumn.ReadOnly = true;
      this.origSeqDataGridViewTextBoxColumn.SortMode = System.Windows.Forms.DataGridViewColumnSortMode.NotSortable;
      this.origSeqDataGridViewTextBoxColumn.Width = 54;
      // 
      // sewninDataGridViewTextBoxColumn
      // 
      this.sewninDataGridViewTextBoxColumn.DataPropertyName = "Sewnin";
      dataGridViewCellStyle12.Alignment = System.Windows.Forms.DataGridViewContentAlignment.MiddleCenter;
      this.sewninDataGridViewTextBoxColumn.DefaultCellStyle = dataGridViewCellStyle12;
      this.sewninDataGridViewTextBoxColumn.HeaderText = "Sewn-in";
      this.sewninDataGridViewTextBoxColumn.Name = "sewninDataGridViewTextBoxColumn";
      this.sewninDataGridViewTextBoxColumn.ReadOnly = true;
      this.sewninDataGridViewTextBoxColumn.SortMode = System.Windows.Forms.DataGridViewColumnSortMode.NotSortable;
      this.sewninDataGridViewTextBoxColumn.Width = 51;
      // 
      // rushDataGridViewTextBoxColumn
      // 
      this.rushDataGridViewTextBoxColumn.DataPropertyName = "Rush";
      dataGridViewCellStyle13.Alignment = System.Windows.Forms.DataGridViewContentAlignment.MiddleCenter;
      this.rushDataGridViewTextBoxColumn.DefaultCellStyle = dataGridViewCellStyle13;
      this.rushDataGridViewTextBoxColumn.HeaderText = "Rush";
      this.rushDataGridViewTextBoxColumn.Name = "rushDataGridViewTextBoxColumn";
      this.rushDataGridViewTextBoxColumn.ReadOnly = true;
      this.rushDataGridViewTextBoxColumn.SortMode = System.Windows.Forms.DataGridViewColumnSortMode.NotSortable;
      this.rushDataGridViewTextBoxColumn.Width = 38;
      // 
      // commentDataGridViewTextBoxColumn
      // 
      this.commentDataGridViewTextBoxColumn.DataPropertyName = "Comment";
      this.commentDataGridViewTextBoxColumn.HeaderText = "Comment";
      this.commentDataGridViewTextBoxColumn.Name = "commentDataGridViewTextBoxColumn";
      this.commentDataGridViewTextBoxColumn.ReadOnly = true;
      this.commentDataGridViewTextBoxColumn.SortMode = System.Windows.Forms.DataGridViewColumnSortMode.NotSortable;
      this.commentDataGridViewTextBoxColumn.Width = 57;
      // 
      // comment2DataGridViewTextBoxColumn
      // 
      this.comment2DataGridViewTextBoxColumn.DataPropertyName = "Comment2";
      this.comment2DataGridViewTextBoxColumn.HeaderText = "Comment2";
      this.comment2DataGridViewTextBoxColumn.Name = "comment2DataGridViewTextBoxColumn";
      this.comment2DataGridViewTextBoxColumn.ReadOnly = true;
      this.comment2DataGridViewTextBoxColumn.SortMode = System.Windows.Forms.DataGridViewColumnSortMode.NotSortable;
      this.comment2DataGridViewTextBoxColumn.Visible = false;
      this.comment2DataGridViewTextBoxColumn.Width = 63;
      // 
      // srcCoaterSchedule
      // 
      this.srcCoaterSchedule.DataSource = typeof(MahloService.Models.CoaterScheduleRoll);
      // 
      // FormCoaterSchedule
      // 
      this.AutoScaleDimensions = new System.Drawing.SizeF(6F, 13F);
      this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
      this.CancelButton = this.btnClose;
      this.ClientSize = new System.Drawing.Size(1034, 453);
      this.Controls.Add(this.dbgBackingSummary);
      this.Controls.Add(this.statusBar1);
      this.Controls.Add(this.btnToggleView);
      this.Controls.Add(this.btnClose);
      this.Controls.Add(this.btnWhichOrders);
      this.Controls.Add(this.dbgCoaterSchedule);
      this.Name = "FormCoaterSchedule";
      this.Text = "FormCoaterSchedule";
      ((System.ComponentModel.ISupportInitialize)(this.pnlMessage)).EndInit();
      ((System.ComponentModel.ISupportInitialize)(this.dbgCoaterSchedule)).EndInit();
      ((System.ComponentModel.ISupportInitialize)(this.dbgBackingSummary)).EndInit();
      ((System.ComponentModel.ISupportInitialize)(this.srcCoaterSchedule)).EndInit();
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
    private System.Windows.Forms.BindingSource srcCoaterSchedule;
    private System.Windows.Forms.DataGridViewTextBoxColumn seqNoDataGridViewTextBoxColumn;
    private System.Windows.Forms.DataGridViewTextBoxColumn schedNoDataGridViewTextBoxColumn;
    private System.Windows.Forms.DataGridViewTextBoxColumn styleDataGridViewTextBoxColumn;
    private System.Windows.Forms.DataGridViewTextBoxColumn colorDataGridViewTextBoxColumn;
    private System.Windows.Forms.DataGridViewTextBoxColumn backingDataGridViewTextBoxColumn;
    private System.Windows.Forms.DataGridViewTextBoxColumn cutLengthDataGridViewTextBoxColumn;
    private System.Windows.Forms.DataGridViewTextBoxColumn faceWtDataGridViewTextBoxColumn;
    private System.Windows.Forms.DataGridViewTextBoxColumn promisedDataGridViewTextBoxColumn;
    private System.Windows.Forms.DataGridViewTextBoxColumn processDataGridViewTextBoxColumn;
    private System.Windows.Forms.DataGridViewTextBoxColumn rollsDataGridViewTextBoxColumn;
    private System.Windows.Forms.DataGridViewTextBoxColumn feetDataGridViewTextBoxColumn;
    private System.Windows.Forms.DataGridViewTextBoxColumn minutesDataGridViewTextBoxColumn;
    private System.Windows.Forms.DataGridViewTextBoxColumn origSeqDataGridViewTextBoxColumn;
    private System.Windows.Forms.DataGridViewTextBoxColumn sewninDataGridViewTextBoxColumn;
    private System.Windows.Forms.DataGridViewTextBoxColumn rushDataGridViewTextBoxColumn;
    private System.Windows.Forms.DataGridViewTextBoxColumn commentDataGridViewTextBoxColumn;
    private System.Windows.Forms.DataGridViewTextBoxColumn comment2DataGridViewTextBoxColumn;
  }
}