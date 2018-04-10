namespace Mahlo.Views
{
  partial class MainForm
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
      this.sewinGrid = new System.Windows.Forms.DataGridView();
      this.g2ROLLDataGridViewTextBoxColumn = new System.Windows.Forms.DataGridViewTextBoxColumn();
      this.g2STYLDataGridViewTextBoxColumn = new System.Windows.Forms.DataGridViewTextBoxColumn();
      this.f2SDSCDataGridViewTextBoxColumn = new System.Windows.Forms.DataGridViewTextBoxColumn();
      this.g2CLRDataGridViewTextBoxColumn = new System.Windows.Forms.DataGridViewTextBoxColumn();
      this.f2CDSCDataGridViewTextBoxColumn = new System.Windows.Forms.DataGridViewTextBoxColumn();
      this.g2SBKDataGridViewTextBoxColumn = new System.Windows.Forms.DataGridViewTextBoxColumn();
      this.g2RPLNDataGridViewTextBoxColumn = new System.Windows.Forms.DataGridViewTextBoxColumn();
      this.g2WTFDataGridViewTextBoxColumn = new System.Windows.Forms.DataGridViewTextBoxColumn();
      this.g2WTIDataGridViewTextBoxColumn = new System.Windows.Forms.DataGridViewTextBoxColumn();
      this.g2LTFDataGridViewTextBoxColumn = new System.Windows.Forms.DataGridViewTextBoxColumn();
      this.sewinQueueSource = new System.Windows.Forms.BindingSource(this.components);
      this.mahlo2Grid = new System.Windows.Forms.DataGridView();
      this.mahlo2Source = new System.Windows.Forms.BindingSource(this.components);
      this.bowAndSkewGrid = new System.Windows.Forms.DataGridView();
      this.bowDataGridViewTextBoxColumn = new System.Windows.Forms.DataGridViewTextBoxColumn();
      this.skewDataGridViewTextBoxColumn = new System.Windows.Forms.DataGridViewTextBoxColumn();
      this.bowAndSkewSource = new System.Windows.Forms.BindingSource(this.components);
      this.patternRepeatGrid = new System.Windows.Forms.DataGridView();
      this.elongationDataGridViewTextBoxColumn = new System.Windows.Forms.DataGridViewTextBoxColumn();
      this.patternRepeatSource = new System.Windows.Forms.BindingSource(this.components);
      this.dataGridView5 = new System.Windows.Forms.DataGridView();
      this.sapRollDataGridViewTextBoxColumn = new System.Windows.Forms.DataGridViewTextBoxColumn();
      this.lengthDataGridViewTextBoxColumn1 = new System.Windows.Forms.DataGridViewTextBoxColumn();
      this.maxBowDataGridViewTextBoxColumn = new System.Windows.Forms.DataGridViewTextBoxColumn();
      this.maxSkewDataGridViewTextBoxColumn = new System.Windows.Forms.DataGridViewTextBoxColumn();
      this.maxEPEDataGridViewTextBoxColumn = new System.Windows.Forms.DataGridViewTextBoxColumn();
      this.dlotDataGridViewTextBoxColumn = new System.Windows.Forms.DataGridViewTextBoxColumn();
      this.finishRollSource = new System.Windows.Forms.BindingSource(this.components);
      this.tableLayoutPanel1 = new System.Windows.Forms.TableLayoutPanel();
      this.vScrollBar1 = new System.Windows.Forms.VScrollBar();
      this.label1 = new System.Windows.Forms.Label();
      this.label3 = new System.Windows.Forms.Label();
      this.label4 = new System.Windows.Forms.Label();
      this.label2 = new System.Windows.Forms.Label();
      this.label5 = new System.Windows.Forms.Label();
      this.dataGridView1 = new System.Windows.Forms.DataGridView();
      this.rollIdDataGridViewTextBoxColumn = new System.Windows.Forms.DataGridViewTextBoxColumn();
      this.feetDataGridViewTextBoxColumn = new System.Windows.Forms.DataGridViewTextBoxColumn();
      this.metersDataGridViewTextBoxColumn = new System.Windows.Forms.DataGridViewTextBoxColumn();
      ((System.ComponentModel.ISupportInitialize)(this.sewinGrid)).BeginInit();
      ((System.ComponentModel.ISupportInitialize)(this.sewinQueueSource)).BeginInit();
      ((System.ComponentModel.ISupportInitialize)(this.mahlo2Grid)).BeginInit();
      ((System.ComponentModel.ISupportInitialize)(this.mahlo2Source)).BeginInit();
      ((System.ComponentModel.ISupportInitialize)(this.bowAndSkewGrid)).BeginInit();
      ((System.ComponentModel.ISupportInitialize)(this.bowAndSkewSource)).BeginInit();
      ((System.ComponentModel.ISupportInitialize)(this.patternRepeatGrid)).BeginInit();
      ((System.ComponentModel.ISupportInitialize)(this.patternRepeatSource)).BeginInit();
      ((System.ComponentModel.ISupportInitialize)(this.dataGridView5)).BeginInit();
      ((System.ComponentModel.ISupportInitialize)(this.finishRollSource)).BeginInit();
      this.tableLayoutPanel1.SuspendLayout();
      ((System.ComponentModel.ISupportInitialize)(this.dataGridView1)).BeginInit();
      this.SuspendLayout();
      // 
      // sewinGrid
      // 
      this.sewinGrid.AllowUserToAddRows = false;
      this.sewinGrid.AllowUserToDeleteRows = false;
      this.sewinGrid.AllowUserToResizeRows = false;
      this.sewinGrid.Anchor = ((System.Windows.Forms.AnchorStyles)(((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Bottom) 
            | System.Windows.Forms.AnchorStyles.Left)));
      this.sewinGrid.AutoGenerateColumns = false;
      this.sewinGrid.ColumnHeadersHeightSizeMode = System.Windows.Forms.DataGridViewColumnHeadersHeightSizeMode.AutoSize;
      this.sewinGrid.Columns.AddRange(new System.Windows.Forms.DataGridViewColumn[] {
            this.g2ROLLDataGridViewTextBoxColumn,
            this.g2STYLDataGridViewTextBoxColumn,
            this.f2SDSCDataGridViewTextBoxColumn,
            this.g2CLRDataGridViewTextBoxColumn,
            this.f2CDSCDataGridViewTextBoxColumn,
            this.g2SBKDataGridViewTextBoxColumn,
            this.g2RPLNDataGridViewTextBoxColumn,
            this.g2WTFDataGridViewTextBoxColumn,
            this.g2WTIDataGridViewTextBoxColumn,
            this.g2LTFDataGridViewTextBoxColumn});
      this.sewinGrid.DataSource = this.sewinQueueSource;
      this.sewinGrid.Location = new System.Drawing.Point(3, 19);
      this.sewinGrid.Name = "sewinGrid";
      this.sewinGrid.RowHeadersVisible = false;
      this.sewinGrid.ScrollBars = System.Windows.Forms.ScrollBars.None;
      this.sewinGrid.Size = new System.Drawing.Size(667, 148);
      this.sewinGrid.TabIndex = 0;
      // 
      // g2ROLLDataGridViewTextBoxColumn
      // 
      this.g2ROLLDataGridViewTextBoxColumn.DataPropertyName = "G2ROLL";
      this.g2ROLLDataGridViewTextBoxColumn.HeaderText = "GreigeRoll";
      this.g2ROLLDataGridViewTextBoxColumn.Name = "g2ROLLDataGridViewTextBoxColumn";
      this.g2ROLLDataGridViewTextBoxColumn.Width = 67;
      // 
      // g2STYLDataGridViewTextBoxColumn
      // 
      this.g2STYLDataGridViewTextBoxColumn.DataPropertyName = "G2STYL";
      this.g2STYLDataGridViewTextBoxColumn.HeaderText = "Style";
      this.g2STYLDataGridViewTextBoxColumn.Name = "g2STYLDataGridViewTextBoxColumn";
      this.g2STYLDataGridViewTextBoxColumn.Width = 60;
      // 
      // f2SDSCDataGridViewTextBoxColumn
      // 
      this.f2SDSCDataGridViewTextBoxColumn.DataPropertyName = "F2SDSC";
      this.f2SDSCDataGridViewTextBoxColumn.HeaderText = "Description";
      this.f2SDSCDataGridViewTextBoxColumn.Name = "f2SDSCDataGridViewTextBoxColumn";
      this.f2SDSCDataGridViewTextBoxColumn.Width = 120;
      // 
      // g2CLRDataGridViewTextBoxColumn
      // 
      this.g2CLRDataGridViewTextBoxColumn.DataPropertyName = "G2CLR";
      this.g2CLRDataGridViewTextBoxColumn.HeaderText = "Color";
      this.g2CLRDataGridViewTextBoxColumn.Name = "g2CLRDataGridViewTextBoxColumn";
      this.g2CLRDataGridViewTextBoxColumn.Width = 67;
      // 
      // f2CDSCDataGridViewTextBoxColumn
      // 
      this.f2CDSCDataGridViewTextBoxColumn.DataPropertyName = "F2CDSC";
      this.f2CDSCDataGridViewTextBoxColumn.HeaderText = "Description";
      this.f2CDSCDataGridViewTextBoxColumn.Name = "f2CDSCDataGridViewTextBoxColumn";
      this.f2CDSCDataGridViewTextBoxColumn.Width = 120;
      // 
      // g2SBKDataGridViewTextBoxColumn
      // 
      this.g2SBKDataGridViewTextBoxColumn.DataPropertyName = "G2SBK";
      this.g2SBKDataGridViewTextBoxColumn.HeaderText = "Backing";
      this.g2SBKDataGridViewTextBoxColumn.Name = "g2SBKDataGridViewTextBoxColumn";
      this.g2SBKDataGridViewTextBoxColumn.Width = 57;
      // 
      // g2RPLNDataGridViewTextBoxColumn
      // 
      this.g2RPLNDataGridViewTextBoxColumn.DataPropertyName = "G2RPLN";
      this.g2RPLNDataGridViewTextBoxColumn.HeaderText = "Repeat";
      this.g2RPLNDataGridViewTextBoxColumn.Name = "g2RPLNDataGridViewTextBoxColumn";
      this.g2RPLNDataGridViewTextBoxColumn.Width = 50;
      // 
      // g2WTFDataGridViewTextBoxColumn
      // 
      this.g2WTFDataGridViewTextBoxColumn.DataPropertyName = "G2WTF";
      this.g2WTFDataGridViewTextBoxColumn.HeaderText = "Width";
      this.g2WTFDataGridViewTextBoxColumn.Name = "g2WTFDataGridViewTextBoxColumn";
      this.g2WTFDataGridViewTextBoxColumn.Width = 67;
      // 
      // g2WTIDataGridViewTextBoxColumn
      // 
      this.g2WTIDataGridViewTextBoxColumn.DataPropertyName = "G2WTI";
      this.g2WTIDataGridViewTextBoxColumn.HeaderText = "Width In";
      this.g2WTIDataGridViewTextBoxColumn.Name = "g2WTIDataGridViewTextBoxColumn";
      this.g2WTIDataGridViewTextBoxColumn.Visible = false;
      // 
      // g2LTFDataGridViewTextBoxColumn
      // 
      this.g2LTFDataGridViewTextBoxColumn.DataPropertyName = "G2LTF";
      this.g2LTFDataGridViewTextBoxColumn.HeaderText = "Length";
      this.g2LTFDataGridViewTextBoxColumn.Name = "g2LTFDataGridViewTextBoxColumn";
      this.g2LTFDataGridViewTextBoxColumn.Width = 50;
      // 
      // sewinQueueSource
      // 
      this.sewinQueueSource.DataSource = typeof(Mahlo.Models.GreigeRoll);
      // 
      // mahlo2Grid
      // 
      this.mahlo2Grid.AllowUserToAddRows = false;
      this.mahlo2Grid.AllowUserToDeleteRows = false;
      this.mahlo2Grid.AllowUserToResizeRows = false;
      this.mahlo2Grid.Anchor = ((System.Windows.Forms.AnchorStyles)(((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Bottom) 
            | System.Windows.Forms.AnchorStyles.Left)));
      this.mahlo2Grid.AutoGenerateColumns = false;
      this.mahlo2Grid.ColumnHeadersHeightSizeMode = System.Windows.Forms.DataGridViewColumnHeadersHeightSizeMode.AutoSize;
      this.mahlo2Grid.Columns.AddRange(new System.Windows.Forms.DataGridViewColumn[] {
            this.rollIdDataGridViewTextBoxColumn,
            this.feetDataGridViewTextBoxColumn,
            this.metersDataGridViewTextBoxColumn});
      this.mahlo2Grid.DataSource = this.mahlo2Source;
      this.mahlo2Grid.Location = new System.Drawing.Point(676, 19);
      this.mahlo2Grid.Name = "mahlo2Grid";
      this.mahlo2Grid.RowHeadersVisible = false;
      this.mahlo2Grid.ScrollBars = System.Windows.Forms.ScrollBars.None;
      this.mahlo2Grid.Size = new System.Drawing.Size(56, 148);
      this.mahlo2Grid.TabIndex = 1;
      // 
      // mahlo2Source
      // 
      this.mahlo2Source.DataSource = typeof(Mahlo.Models.MahloRoll);
      // 
      // bowAndSkewGrid
      // 
      this.bowAndSkewGrid.AllowUserToAddRows = false;
      this.bowAndSkewGrid.AllowUserToDeleteRows = false;
      this.bowAndSkewGrid.AllowUserToResizeRows = false;
      this.bowAndSkewGrid.Anchor = ((System.Windows.Forms.AnchorStyles)(((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Bottom) 
            | System.Windows.Forms.AnchorStyles.Left)));
      this.bowAndSkewGrid.AutoGenerateColumns = false;
      this.bowAndSkewGrid.ColumnHeadersHeightSizeMode = System.Windows.Forms.DataGridViewColumnHeadersHeightSizeMode.AutoSize;
      this.bowAndSkewGrid.Columns.AddRange(new System.Windows.Forms.DataGridViewColumn[] {
            this.bowDataGridViewTextBoxColumn,
            this.skewDataGridViewTextBoxColumn});
      this.bowAndSkewGrid.DataSource = this.bowAndSkewSource;
      this.bowAndSkewGrid.Location = new System.Drawing.Point(738, 19);
      this.bowAndSkewGrid.Name = "bowAndSkewGrid";
      this.bowAndSkewGrid.RowHeadersVisible = false;
      this.bowAndSkewGrid.ScrollBars = System.Windows.Forms.ScrollBars.None;
      this.bowAndSkewGrid.Size = new System.Drawing.Size(190, 148);
      this.bowAndSkewGrid.TabIndex = 2;
      // 
      // bowDataGridViewTextBoxColumn
      // 
      this.bowDataGridViewTextBoxColumn.DataPropertyName = "Bow";
      this.bowDataGridViewTextBoxColumn.HeaderText = "Bow";
      this.bowDataGridViewTextBoxColumn.Name = "bowDataGridViewTextBoxColumn";
      this.bowDataGridViewTextBoxColumn.Width = 67;
      // 
      // skewDataGridViewTextBoxColumn
      // 
      this.skewDataGridViewTextBoxColumn.DataPropertyName = "Skew";
      this.skewDataGridViewTextBoxColumn.HeaderText = "Skew";
      this.skewDataGridViewTextBoxColumn.Name = "skewDataGridViewTextBoxColumn";
      this.skewDataGridViewTextBoxColumn.Width = 67;
      // 
      // bowAndSkewSource
      // 
      this.bowAndSkewSource.DataSource = typeof(Mahlo.Models.BowAndSkewRoll);
      // 
      // patternRepeatGrid
      // 
      this.patternRepeatGrid.AllowUserToAddRows = false;
      this.patternRepeatGrid.AllowUserToDeleteRows = false;
      this.patternRepeatGrid.AllowUserToResizeRows = false;
      this.patternRepeatGrid.Anchor = ((System.Windows.Forms.AnchorStyles)(((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Bottom) 
            | System.Windows.Forms.AnchorStyles.Left)));
      this.patternRepeatGrid.AutoGenerateColumns = false;
      this.patternRepeatGrid.ColumnHeadersHeightSizeMode = System.Windows.Forms.DataGridViewColumnHeadersHeightSizeMode.AutoSize;
      this.patternRepeatGrid.Columns.AddRange(new System.Windows.Forms.DataGridViewColumn[] {
            this.elongationDataGridViewTextBoxColumn});
      this.patternRepeatGrid.DataSource = this.patternRepeatSource;
      this.patternRepeatGrid.Location = new System.Drawing.Point(934, 19);
      this.patternRepeatGrid.Name = "patternRepeatGrid";
      this.patternRepeatGrid.RowHeadersVisible = false;
      this.patternRepeatGrid.ScrollBars = System.Windows.Forms.ScrollBars.None;
      this.patternRepeatGrid.Size = new System.Drawing.Size(125, 148);
      this.patternRepeatGrid.TabIndex = 3;
      // 
      // elongationDataGridViewTextBoxColumn
      // 
      this.elongationDataGridViewTextBoxColumn.DataPropertyName = "Elongation";
      this.elongationDataGridViewTextBoxColumn.HeaderText = "Elongation";
      this.elongationDataGridViewTextBoxColumn.Name = "elongationDataGridViewTextBoxColumn";
      this.elongationDataGridViewTextBoxColumn.Width = 67;
      // 
      // patternRepeatSource
      // 
      this.patternRepeatSource.DataSource = typeof(Mahlo.Models.PatternRepeatRoll);
      // 
      // dataGridView5
      // 
      this.dataGridView5.AllowUserToAddRows = false;
      this.dataGridView5.AllowUserToDeleteRows = false;
      this.dataGridView5.AllowUserToResizeRows = false;
      this.dataGridView5.Anchor = ((System.Windows.Forms.AnchorStyles)((((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Bottom) 
            | System.Windows.Forms.AnchorStyles.Left) 
            | System.Windows.Forms.AnchorStyles.Right)));
      this.dataGridView5.AutoGenerateColumns = false;
      this.dataGridView5.ColumnHeadersHeightSizeMode = System.Windows.Forms.DataGridViewColumnHeadersHeightSizeMode.AutoSize;
      this.dataGridView5.Columns.AddRange(new System.Windows.Forms.DataGridViewColumn[] {
            this.sapRollDataGridViewTextBoxColumn,
            this.lengthDataGridViewTextBoxColumn1,
            this.maxBowDataGridViewTextBoxColumn,
            this.maxSkewDataGridViewTextBoxColumn,
            this.maxEPEDataGridViewTextBoxColumn,
            this.dlotDataGridViewTextBoxColumn});
      this.dataGridView5.DataSource = this.finishRollSource;
      this.dataGridView5.Location = new System.Drawing.Point(1085, 19);
      this.dataGridView5.Name = "dataGridView5";
      this.dataGridView5.RowHeadersVisible = false;
      this.dataGridView5.ScrollBars = System.Windows.Forms.ScrollBars.Vertical;
      this.dataGridView5.Size = new System.Drawing.Size(461, 148);
      this.dataGridView5.TabIndex = 4;
      // 
      // sapRollDataGridViewTextBoxColumn
      // 
      this.sapRollDataGridViewTextBoxColumn.DataPropertyName = "SapRoll";
      this.sapRollDataGridViewTextBoxColumn.HeaderText = "SapRoll";
      this.sapRollDataGridViewTextBoxColumn.Name = "sapRollDataGridViewTextBoxColumn";
      this.sapRollDataGridViewTextBoxColumn.Width = 67;
      // 
      // lengthDataGridViewTextBoxColumn1
      // 
      this.lengthDataGridViewTextBoxColumn1.DataPropertyName = "Length";
      this.lengthDataGridViewTextBoxColumn1.HeaderText = "Length";
      this.lengthDataGridViewTextBoxColumn1.Name = "lengthDataGridViewTextBoxColumn1";
      this.lengthDataGridViewTextBoxColumn1.Width = 50;
      // 
      // maxBowDataGridViewTextBoxColumn
      // 
      this.maxBowDataGridViewTextBoxColumn.DataPropertyName = "MaxBow";
      this.maxBowDataGridViewTextBoxColumn.HeaderText = "MaxBow";
      this.maxBowDataGridViewTextBoxColumn.Name = "maxBowDataGridViewTextBoxColumn";
      this.maxBowDataGridViewTextBoxColumn.Width = 67;
      // 
      // maxSkewDataGridViewTextBoxColumn
      // 
      this.maxSkewDataGridViewTextBoxColumn.DataPropertyName = "MaxSkew";
      this.maxSkewDataGridViewTextBoxColumn.HeaderText = "MaxSkew";
      this.maxSkewDataGridViewTextBoxColumn.Name = "maxSkewDataGridViewTextBoxColumn";
      this.maxSkewDataGridViewTextBoxColumn.Width = 67;
      // 
      // maxEPEDataGridViewTextBoxColumn
      // 
      this.maxEPEDataGridViewTextBoxColumn.DataPropertyName = "MaxEPE";
      this.maxEPEDataGridViewTextBoxColumn.HeaderText = "MaxEPE";
      this.maxEPEDataGridViewTextBoxColumn.Name = "maxEPEDataGridViewTextBoxColumn";
      this.maxEPEDataGridViewTextBoxColumn.Width = 67;
      // 
      // dlotDataGridViewTextBoxColumn
      // 
      this.dlotDataGridViewTextBoxColumn.DataPropertyName = "Dlot";
      this.dlotDataGridViewTextBoxColumn.HeaderText = "Dlot";
      this.dlotDataGridViewTextBoxColumn.Name = "dlotDataGridViewTextBoxColumn";
      this.dlotDataGridViewTextBoxColumn.Width = 45;
      // 
      // finishRollSource
      // 
      this.finishRollSource.DataSource = typeof(Mahlo.Models.CutRoll);
      // 
      // tableLayoutPanel1
      // 
      this.tableLayoutPanel1.ColumnCount = 6;
      this.tableLayoutPanel1.ColumnStyles.Add(new System.Windows.Forms.ColumnStyle());
      this.tableLayoutPanel1.ColumnStyles.Add(new System.Windows.Forms.ColumnStyle());
      this.tableLayoutPanel1.ColumnStyles.Add(new System.Windows.Forms.ColumnStyle());
      this.tableLayoutPanel1.ColumnStyles.Add(new System.Windows.Forms.ColumnStyle());
      this.tableLayoutPanel1.ColumnStyles.Add(new System.Windows.Forms.ColumnStyle(System.Windows.Forms.SizeType.Absolute, 20F));
      this.tableLayoutPanel1.ColumnStyles.Add(new System.Windows.Forms.ColumnStyle(System.Windows.Forms.SizeType.Percent, 100F));
      this.tableLayoutPanel1.Controls.Add(this.vScrollBar1, 4, 1);
      this.tableLayoutPanel1.Controls.Add(this.label1, 0, 0);
      this.tableLayoutPanel1.Controls.Add(this.mahlo2Grid, 1, 1);
      this.tableLayoutPanel1.Controls.Add(this.label3, 2, 0);
      this.tableLayoutPanel1.Controls.Add(this.label4, 3, 0);
      this.tableLayoutPanel1.Controls.Add(this.sewinGrid, 0, 1);
      this.tableLayoutPanel1.Controls.Add(this.bowAndSkewGrid, 2, 1);
      this.tableLayoutPanel1.Controls.Add(this.dataGridView5, 5, 1);
      this.tableLayoutPanel1.Controls.Add(this.label2, 1, 0);
      this.tableLayoutPanel1.Controls.Add(this.patternRepeatGrid, 3, 1);
      this.tableLayoutPanel1.Controls.Add(this.label5, 5, 0);
      this.tableLayoutPanel1.Location = new System.Drawing.Point(12, 12);
      this.tableLayoutPanel1.Name = "tableLayoutPanel1";
      this.tableLayoutPanel1.RowCount = 2;
      this.tableLayoutPanel1.RowStyles.Add(new System.Windows.Forms.RowStyle());
      this.tableLayoutPanel1.RowStyles.Add(new System.Windows.Forms.RowStyle(System.Windows.Forms.SizeType.Percent, 100F));
      this.tableLayoutPanel1.Size = new System.Drawing.Size(1549, 170);
      this.tableLayoutPanel1.TabIndex = 5;
      // 
      // vScrollBar1
      // 
      this.vScrollBar1.Anchor = ((System.Windows.Forms.AnchorStyles)((((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Bottom) 
            | System.Windows.Forms.AnchorStyles.Left) 
            | System.Windows.Forms.AnchorStyles.Right)));
      this.vScrollBar1.Location = new System.Drawing.Point(1062, 16);
      this.vScrollBar1.Name = "vScrollBar1";
      this.vScrollBar1.Size = new System.Drawing.Size(20, 154);
      this.vScrollBar1.TabIndex = 6;
      // 
      // label1
      // 
      this.label1.Anchor = System.Windows.Forms.AnchorStyles.Top;
      this.label1.AutoSize = true;
      this.label1.Font = new System.Drawing.Font("Microsoft Sans Serif", 9.75F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
      this.label1.Location = new System.Drawing.Point(293, 0);
      this.label1.Name = "label1";
      this.label1.Size = new System.Drawing.Size(87, 16);
      this.label1.TabIndex = 0;
      this.label1.Text = "Sewin Queue";
      // 
      // label3
      // 
      this.label3.Anchor = System.Windows.Forms.AnchorStyles.Top;
      this.label3.AutoSize = true;
      this.label3.Font = new System.Drawing.Font("Microsoft Sans Serif", 9.75F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
      this.label3.Location = new System.Drawing.Point(785, 0);
      this.label3.Name = "label3";
      this.label3.Size = new System.Drawing.Size(96, 16);
      this.label3.TabIndex = 3;
      this.label3.Text = "Bow and Skew";
      // 
      // label4
      // 
      this.label4.Anchor = System.Windows.Forms.AnchorStyles.Top;
      this.label4.AutoSize = true;
      this.label4.Font = new System.Drawing.Font("Microsoft Sans Serif", 9.75F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
      this.label4.Location = new System.Drawing.Point(947, 0);
      this.label4.Name = "label4";
      this.label4.Size = new System.Drawing.Size(98, 16);
      this.label4.TabIndex = 5;
      this.label4.Text = "Pattern Repeat";
      // 
      // label2
      // 
      this.label2.Anchor = System.Windows.Forms.AnchorStyles.Top;
      this.label2.AutoSize = true;
      this.label2.Font = new System.Drawing.Font("Microsoft Sans Serif", 9.75F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
      this.label2.Location = new System.Drawing.Point(678, 0);
      this.label2.Name = "label2";
      this.label2.Size = new System.Drawing.Size(52, 16);
      this.label2.TabIndex = 1;
      this.label2.Text = "Mahlo2";
      // 
      // label5
      // 
      this.label5.Anchor = System.Windows.Forms.AnchorStyles.Top;
      this.label5.AutoSize = true;
      this.label5.Font = new System.Drawing.Font("Microsoft Sans Serif", 9.75F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
      this.label5.Location = new System.Drawing.Point(1269, 0);
      this.label5.Name = "label5";
      this.label5.Size = new System.Drawing.Size(93, 16);
      this.label5.TabIndex = 6;
      this.label5.Text = "Finished Rolls";
      // 
      // dataGridView1
      // 
      this.dataGridView1.ColumnHeadersHeightSizeMode = System.Windows.Forms.DataGridViewColumnHeadersHeightSizeMode.AutoSize;
      this.dataGridView1.Location = new System.Drawing.Point(15, 220);
      this.dataGridView1.Name = "dataGridView1";
      this.dataGridView1.Size = new System.Drawing.Size(1079, 152);
      this.dataGridView1.TabIndex = 6;
      // 
      // rollIdDataGridViewTextBoxColumn
      // 
      this.rollIdDataGridViewTextBoxColumn.DataPropertyName = "RollId";
      this.rollIdDataGridViewTextBoxColumn.HeaderText = "RollId";
      this.rollIdDataGridViewTextBoxColumn.Name = "rollIdDataGridViewTextBoxColumn";
      // 
      // feetDataGridViewTextBoxColumn
      // 
      this.feetDataGridViewTextBoxColumn.DataPropertyName = "Feet";
      this.feetDataGridViewTextBoxColumn.HeaderText = "Feet";
      this.feetDataGridViewTextBoxColumn.Name = "feetDataGridViewTextBoxColumn";
      this.feetDataGridViewTextBoxColumn.ReadOnly = true;
      // 
      // metersDataGridViewTextBoxColumn
      // 
      this.metersDataGridViewTextBoxColumn.DataPropertyName = "Meters";
      this.metersDataGridViewTextBoxColumn.HeaderText = "Meters";
      this.metersDataGridViewTextBoxColumn.Name = "metersDataGridViewTextBoxColumn";
      // 
      // MainForm
      // 
      this.AutoScaleDimensions = new System.Drawing.SizeF(6F, 13F);
      this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
      this.ClientSize = new System.Drawing.Size(1573, 540);
      this.Controls.Add(this.dataGridView1);
      this.Controls.Add(this.tableLayoutPanel1);
      this.Name = "MainForm";
      this.Text = "MainForm";
      this.Load += new System.EventHandler(this.MainForm_Load);
      ((System.ComponentModel.ISupportInitialize)(this.sewinGrid)).EndInit();
      ((System.ComponentModel.ISupportInitialize)(this.sewinQueueSource)).EndInit();
      ((System.ComponentModel.ISupportInitialize)(this.mahlo2Grid)).EndInit();
      ((System.ComponentModel.ISupportInitialize)(this.mahlo2Source)).EndInit();
      ((System.ComponentModel.ISupportInitialize)(this.bowAndSkewGrid)).EndInit();
      ((System.ComponentModel.ISupportInitialize)(this.bowAndSkewSource)).EndInit();
      ((System.ComponentModel.ISupportInitialize)(this.patternRepeatGrid)).EndInit();
      ((System.ComponentModel.ISupportInitialize)(this.patternRepeatSource)).EndInit();
      ((System.ComponentModel.ISupportInitialize)(this.dataGridView5)).EndInit();
      ((System.ComponentModel.ISupportInitialize)(this.finishRollSource)).EndInit();
      this.tableLayoutPanel1.ResumeLayout(false);
      this.tableLayoutPanel1.PerformLayout();
      ((System.ComponentModel.ISupportInitialize)(this.dataGridView1)).EndInit();
      this.ResumeLayout(false);

    }

    #endregion

    private System.Windows.Forms.DataGridView sewinGrid;
    private System.Windows.Forms.BindingSource sewinQueueSource;
    private System.Windows.Forms.BindingSource mahlo2Source;
    private System.Windows.Forms.DataGridView mahlo2Grid;
    private System.Windows.Forms.BindingSource bowAndSkewSource;
    private System.Windows.Forms.DataGridView bowAndSkewGrid;
    private System.Windows.Forms.DataGridView patternRepeatGrid;
    private System.Windows.Forms.BindingSource patternRepeatSource;
    private System.Windows.Forms.BindingSource finishRollSource;
    private System.Windows.Forms.DataGridViewTextBoxColumn lengthDataGridViewTextBoxColumn;
    private System.Windows.Forms.DataGridViewTextBoxColumn positionDataGridViewTextBoxColumn;
    private System.Windows.Forms.DataGridViewTextBoxColumn bowDataGridViewTextBoxColumn;
    private System.Windows.Forms.DataGridViewTextBoxColumn skewDataGridViewTextBoxColumn;
    private System.Windows.Forms.DataGridViewTextBoxColumn positionDataGridViewTextBoxColumn1;
    private System.Windows.Forms.DataGridViewTextBoxColumn elongationDataGridViewTextBoxColumn;
    private System.Windows.Forms.DataGridView dataGridView5;
    private System.Windows.Forms.TableLayoutPanel tableLayoutPanel1;
    private System.Windows.Forms.Label label1;
    private System.Windows.Forms.Label label2;
    private System.Windows.Forms.Label label3;
    private System.Windows.Forms.Label label4;
    private System.Windows.Forms.Label label5;
    private System.Windows.Forms.DataGridViewTextBoxColumn g2ROLLDataGridViewTextBoxColumn;
    private System.Windows.Forms.DataGridViewTextBoxColumn g2STYLDataGridViewTextBoxColumn;
    private System.Windows.Forms.DataGridViewTextBoxColumn f2SDSCDataGridViewTextBoxColumn;
    private System.Windows.Forms.DataGridViewTextBoxColumn g2CLRDataGridViewTextBoxColumn;
    private System.Windows.Forms.DataGridViewTextBoxColumn f2CDSCDataGridViewTextBoxColumn;
    private System.Windows.Forms.DataGridViewTextBoxColumn g2SBKDataGridViewTextBoxColumn;
    private System.Windows.Forms.DataGridViewTextBoxColumn g2RPLNDataGridViewTextBoxColumn;
    private System.Windows.Forms.DataGridViewTextBoxColumn g2WTFDataGridViewTextBoxColumn;
    private System.Windows.Forms.DataGridViewTextBoxColumn g2WTIDataGridViewTextBoxColumn;
    private System.Windows.Forms.DataGridViewTextBoxColumn g2LTFDataGridViewTextBoxColumn;
    private System.Windows.Forms.DataGridViewTextBoxColumn greigeRollDataGridViewTextBoxColumn;
    private System.Windows.Forms.DataGridViewTextBoxColumn sapRollDataGridViewTextBoxColumn;
    private System.Windows.Forms.DataGridViewTextBoxColumn lengthDataGridViewTextBoxColumn1;
    private System.Windows.Forms.DataGridViewTextBoxColumn maxBowDataGridViewTextBoxColumn;
    private System.Windows.Forms.DataGridViewTextBoxColumn maxSkewDataGridViewTextBoxColumn;
    private System.Windows.Forms.DataGridViewTextBoxColumn maxEPEDataGridViewTextBoxColumn;
    private System.Windows.Forms.DataGridViewTextBoxColumn dlotDataGridViewTextBoxColumn;
    private System.Windows.Forms.VScrollBar vScrollBar1;
    private System.Windows.Forms.DataGridView dataGridView1;
    private System.Windows.Forms.DataGridViewTextBoxColumn rollIdDataGridViewTextBoxColumn;
    private System.Windows.Forms.DataGridViewTextBoxColumn feetDataGridViewTextBoxColumn;
    private System.Windows.Forms.DataGridViewTextBoxColumn metersDataGridViewTextBoxColumn;
  }
}