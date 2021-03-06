﻿namespace MahloService.Simulation
{
  partial class FormSim
  {
    /// <summary>
    /// Required designer variable.
    /// </summary>
    private System.ComponentModel.IContainer components = null;

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
      this.groupBox1 = new System.Windows.Forms.GroupBox();
      this.btnExit = new System.Windows.Forms.Button();
      this.btnRemoveRoll = new System.Windows.Forms.Button();
      this.label4 = new System.Windows.Forms.Label();
      this.textBox4 = new System.Windows.Forms.TextBox();
      this.btnRun = new System.Windows.Forms.Button();
      this.btnAddRoll = new System.Windows.Forms.Button();
      this.groupBox2 = new System.Windows.Forms.GroupBox();
      this.label5 = new System.Windows.Forms.Label();
      this.textBox5 = new System.Windows.Forms.TextBox();
      this.label1 = new System.Windows.Forms.Label();
      this.textBox1 = new System.Windows.Forms.TextBox();
      this.groupBox3 = new System.Windows.Forms.GroupBox();
      this.label6 = new System.Windows.Forms.Label();
      this.textBox6 = new System.Windows.Forms.TextBox();
      this.label2 = new System.Windows.Forms.Label();
      this.textBox2 = new System.Windows.Forms.TextBox();
      this.groupBox4 = new System.Windows.Forms.GroupBox();
      this.label7 = new System.Windows.Forms.Label();
      this.textBox7 = new System.Windows.Forms.TextBox();
      this.label3 = new System.Windows.Forms.Label();
      this.textBox3 = new System.Windows.Forms.TextBox();
      this.dataGridView1 = new System.Windows.Forms.DataGridView();
      this.orderNoColumn = new System.Windows.Forms.DataGridViewTextBoxColumn();
      this.groupBox5 = new System.Windows.Forms.GroupBox();
      this.rollNoDataGridViewTextBoxColumn = new System.Windows.Forms.DataGridViewTextBoxColumn();
      this.rollLengthDataGridViewTextBoxColumn = new System.Windows.Forms.DataGridViewTextBoxColumn();
      this.styleCodeDataGridViewTextBoxColumn = new System.Windows.Forms.DataGridViewTextBoxColumn();
      this.styleNameDataGridViewTextBoxColumn = new System.Windows.Forms.DataGridViewTextBoxColumn();
      this.colorCodeDataGridViewTextBoxColumn = new System.Windows.Forms.DataGridViewTextBoxColumn();
      this.colorNameDataGridViewTextBoxColumn = new System.Windows.Forms.DataGridViewTextBoxColumn();
      this.backingCodeDataGridViewTextBoxColumn = new System.Windows.Forms.DataGridViewTextBoxColumn();
      this.rollWidthStrDataGridViewTextBoxColumn = new System.Windows.Forms.DataGridViewTextBoxColumn();
      this.colDefaultRecipe = new System.Windows.Forms.DataGridViewTextBoxColumn();
      this.patternRepeatLengthDataGridViewTextBoxColumn = new System.Windows.Forms.DataGridViewTextBoxColumn();
      this.srcGrid = new System.Windows.Forms.BindingSource(this.components);
      this.srcSimInfo = new System.Windows.Forms.BindingSource(this.components);
      this.srcFormSim = new System.Windows.Forms.BindingSource(this.components);
      this.groupBox1.SuspendLayout();
      this.groupBox2.SuspendLayout();
      this.groupBox3.SuspendLayout();
      this.groupBox4.SuspendLayout();
      ((System.ComponentModel.ISupportInitialize)(this.dataGridView1)).BeginInit();
      ((System.ComponentModel.ISupportInitialize)(this.srcGrid)).BeginInit();
      ((System.ComponentModel.ISupportInitialize)(this.srcSimInfo)).BeginInit();
      ((System.ComponentModel.ISupportInitialize)(this.srcFormSim)).BeginInit();
      this.SuspendLayout();
      // 
      // groupBox1
      // 
      this.groupBox1.Controls.Add(this.btnExit);
      this.groupBox1.Controls.Add(this.btnRemoveRoll);
      this.groupBox1.Controls.Add(this.label4);
      this.groupBox1.Controls.Add(this.textBox4);
      this.groupBox1.Controls.Add(this.btnRun);
      this.groupBox1.Controls.Add(this.btnAddRoll);
      this.groupBox1.Location = new System.Drawing.Point(13, 13);
      this.groupBox1.Name = "groupBox1";
      this.groupBox1.Size = new System.Drawing.Size(200, 290);
      this.groupBox1.TabIndex = 0;
      this.groupBox1.TabStop = false;
      this.groupBox1.Text = "Sew-in Queue";
      // 
      // btnExit
      // 
      this.btnExit.Location = new System.Drawing.Point(105, 261);
      this.btnExit.Name = "btnExit";
      this.btnExit.Size = new System.Drawing.Size(75, 23);
      this.btnExit.TabIndex = 5;
      this.btnExit.Text = "Exit";
      this.btnExit.UseVisualStyleBackColor = true;
      this.btnExit.Click += new System.EventHandler(this.BtnExit_Click);
      // 
      // btnRemoveRoll
      // 
      this.btnRemoveRoll.Location = new System.Drawing.Point(15, 261);
      this.btnRemoveRoll.Name = "btnRemoveRoll";
      this.btnRemoveRoll.Size = new System.Drawing.Size(75, 23);
      this.btnRemoveRoll.TabIndex = 4;
      this.btnRemoveRoll.Text = "Remove Roll";
      this.btnRemoveRoll.UseVisualStyleBackColor = true;
      this.btnRemoveRoll.Click += new System.EventHandler(this.BtnRemoveRoll_Click);
      // 
      // label4
      // 
      this.label4.AutoSize = true;
      this.label4.Location = new System.Drawing.Point(6, 22);
      this.label4.Name = "label4";
      this.label4.Size = new System.Drawing.Size(84, 13);
      this.label4.TabIndex = 0;
      this.label4.Text = "Feet per Minute:";
      // 
      // textBox4
      // 
      this.textBox4.DataBindings.Add(new System.Windows.Forms.Binding("Text", this.srcSimInfo, "FeetPerMinute", true));
      this.textBox4.Location = new System.Drawing.Point(94, 19);
      this.textBox4.Name = "textBox4";
      this.textBox4.Size = new System.Drawing.Size(100, 20);
      this.textBox4.TabIndex = 1;
      this.textBox4.Text = "200";
      // 
      // btnRun
      // 
      this.btnRun.Anchor = ((System.Windows.Forms.AnchorStyles)((System.Windows.Forms.AnchorStyles.Bottom | System.Windows.Forms.AnchorStyles.Left)));
      this.btnRun.Location = new System.Drawing.Point(105, 232);
      this.btnRun.Name = "btnRun";
      this.btnRun.Size = new System.Drawing.Size(75, 23);
      this.btnRun.TabIndex = 3;
      this.btnRun.Text = "Run";
      this.btnRun.UseVisualStyleBackColor = true;
      this.btnRun.Click += new System.EventHandler(this.btnRun_Click);
      // 
      // btnAddRoll
      // 
      this.btnAddRoll.Anchor = ((System.Windows.Forms.AnchorStyles)((System.Windows.Forms.AnchorStyles.Bottom | System.Windows.Forms.AnchorStyles.Left)));
      this.btnAddRoll.Location = new System.Drawing.Point(15, 232);
      this.btnAddRoll.Name = "btnAddRoll";
      this.btnAddRoll.Size = new System.Drawing.Size(75, 23);
      this.btnAddRoll.TabIndex = 2;
      this.btnAddRoll.Text = "Add Roll";
      this.btnAddRoll.UseVisualStyleBackColor = true;
      this.btnAddRoll.Click += new System.EventHandler(this.BtnAddRoll_Click);
      // 
      // groupBox2
      // 
      this.groupBox2.Controls.Add(this.label5);
      this.groupBox2.Controls.Add(this.textBox5);
      this.groupBox2.Controls.Add(this.label1);
      this.groupBox2.Controls.Add(this.textBox1);
      this.groupBox2.Location = new System.Drawing.Point(219, 13);
      this.groupBox2.Name = "groupBox2";
      this.groupBox2.Size = new System.Drawing.Size(200, 290);
      this.groupBox2.TabIndex = 1;
      this.groupBox2.TabStop = false;
      this.groupBox2.Text = "Mahlo2";
      // 
      // label5
      // 
      this.label5.AutoSize = true;
      this.label5.Location = new System.Drawing.Point(6, 48);
      this.label5.Name = "label5";
      this.label5.Size = new System.Drawing.Size(71, 13);
      this.label5.TabIndex = 2;
      this.label5.Text = "Feet Counter:";
      // 
      // textBox5
      // 
      this.textBox5.DataBindings.Add(new System.Windows.Forms.Binding("Text", this.srcFormSim, "MalFeetCounter", true));
      this.textBox5.Location = new System.Drawing.Point(94, 45);
      this.textBox5.Name = "textBox5";
      this.textBox5.Size = new System.Drawing.Size(100, 20);
      this.textBox5.TabIndex = 3;
      this.textBox5.Text = "200";
      // 
      // label1
      // 
      this.label1.AutoSize = true;
      this.label1.Location = new System.Drawing.Point(6, 22);
      this.label1.Name = "label1";
      this.label1.Size = new System.Drawing.Size(69, 13);
      this.label1.TabIndex = 0;
      this.label1.Text = "Web Length:";
      // 
      // textBox1
      // 
      this.textBox1.DataBindings.Add(new System.Windows.Forms.Binding("Text", this.srcSimInfo, "MalWebLength", true));
      this.textBox1.Location = new System.Drawing.Point(94, 19);
      this.textBox1.Name = "textBox1";
      this.textBox1.Size = new System.Drawing.Size(100, 20);
      this.textBox1.TabIndex = 1;
      this.textBox1.Text = "200";
      // 
      // groupBox3
      // 
      this.groupBox3.Controls.Add(this.label6);
      this.groupBox3.Controls.Add(this.textBox6);
      this.groupBox3.Controls.Add(this.label2);
      this.groupBox3.Controls.Add(this.textBox2);
      this.groupBox3.Location = new System.Drawing.Point(426, 13);
      this.groupBox3.Name = "groupBox3";
      this.groupBox3.Size = new System.Drawing.Size(200, 290);
      this.groupBox3.TabIndex = 2;
      this.groupBox3.TabStop = false;
      this.groupBox3.Text = "Bow and Skew";
      // 
      // label6
      // 
      this.label6.AutoSize = true;
      this.label6.Location = new System.Drawing.Point(6, 48);
      this.label6.Name = "label6";
      this.label6.Size = new System.Drawing.Size(71, 13);
      this.label6.TabIndex = 4;
      this.label6.Text = "Feet Counter:";
      // 
      // textBox6
      // 
      this.textBox6.DataBindings.Add(new System.Windows.Forms.Binding("Text", this.srcFormSim, "BasFeetCounter", true));
      this.textBox6.Location = new System.Drawing.Point(94, 45);
      this.textBox6.Name = "textBox6";
      this.textBox6.Size = new System.Drawing.Size(100, 20);
      this.textBox6.TabIndex = 5;
      this.textBox6.Text = "200";
      // 
      // label2
      // 
      this.label2.AutoSize = true;
      this.label2.Location = new System.Drawing.Point(6, 22);
      this.label2.Name = "label2";
      this.label2.Size = new System.Drawing.Size(69, 13);
      this.label2.TabIndex = 0;
      this.label2.Text = "Web Length:";
      // 
      // textBox2
      // 
      this.textBox2.DataBindings.Add(new System.Windows.Forms.Binding("Text", this.srcSimInfo, "BasWebLength", true));
      this.textBox2.Location = new System.Drawing.Point(94, 19);
      this.textBox2.Name = "textBox2";
      this.textBox2.Size = new System.Drawing.Size(100, 20);
      this.textBox2.TabIndex = 1;
      this.textBox2.Text = "200";
      // 
      // groupBox4
      // 
      this.groupBox4.Controls.Add(this.label7);
      this.groupBox4.Controls.Add(this.textBox7);
      this.groupBox4.Controls.Add(this.label3);
      this.groupBox4.Controls.Add(this.textBox3);
      this.groupBox4.Location = new System.Drawing.Point(632, 13);
      this.groupBox4.Name = "groupBox4";
      this.groupBox4.Size = new System.Drawing.Size(200, 290);
      this.groupBox4.TabIndex = 3;
      this.groupBox4.TabStop = false;
      this.groupBox4.Text = "Pattern Repeat";
      // 
      // label7
      // 
      this.label7.AutoSize = true;
      this.label7.Location = new System.Drawing.Point(6, 48);
      this.label7.Name = "label7";
      this.label7.Size = new System.Drawing.Size(71, 13);
      this.label7.TabIndex = 4;
      this.label7.Text = "Feet Counter:";
      // 
      // textBox7
      // 
      this.textBox7.DataBindings.Add(new System.Windows.Forms.Binding("Text", this.srcFormSim, "PrsFeetCounter", true));
      this.textBox7.Location = new System.Drawing.Point(94, 45);
      this.textBox7.Name = "textBox7";
      this.textBox7.Size = new System.Drawing.Size(100, 20);
      this.textBox7.TabIndex = 5;
      this.textBox7.Text = "200";
      // 
      // label3
      // 
      this.label3.AutoSize = true;
      this.label3.Location = new System.Drawing.Point(6, 22);
      this.label3.Name = "label3";
      this.label3.Size = new System.Drawing.Size(69, 13);
      this.label3.TabIndex = 0;
      this.label3.Text = "Web Length:";
      // 
      // textBox3
      // 
      this.textBox3.DataBindings.Add(new System.Windows.Forms.Binding("Text", this.srcSimInfo, "PrsWebLength", true));
      this.textBox3.Location = new System.Drawing.Point(94, 19);
      this.textBox3.Name = "textBox3";
      this.textBox3.Size = new System.Drawing.Size(100, 20);
      this.textBox3.TabIndex = 1;
      this.textBox3.Text = "200";
      // 
      // dataGridView1
      // 
      this.dataGridView1.AllowUserToAddRows = false;
      this.dataGridView1.AllowUserToResizeRows = false;
      this.dataGridView1.Anchor = ((System.Windows.Forms.AnchorStyles)((((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Bottom) 
            | System.Windows.Forms.AnchorStyles.Left) 
            | System.Windows.Forms.AnchorStyles.Right)));
      this.dataGridView1.AutoGenerateColumns = false;
      this.dataGridView1.ColumnHeadersHeightSizeMode = System.Windows.Forms.DataGridViewColumnHeadersHeightSizeMode.AutoSize;
      this.dataGridView1.Columns.AddRange(new System.Windows.Forms.DataGridViewColumn[] {
            this.rollNoDataGridViewTextBoxColumn,
            this.orderNoColumn,
            this.rollLengthDataGridViewTextBoxColumn,
            this.styleCodeDataGridViewTextBoxColumn,
            this.styleNameDataGridViewTextBoxColumn,
            this.colorCodeDataGridViewTextBoxColumn,
            this.colorNameDataGridViewTextBoxColumn,
            this.backingCodeDataGridViewTextBoxColumn,
            this.rollWidthStrDataGridViewTextBoxColumn,
            this.colDefaultRecipe,
            this.patternRepeatLengthDataGridViewTextBoxColumn});
      this.dataGridView1.DataSource = this.srcGrid;
      this.dataGridView1.Location = new System.Drawing.Point(13, 309);
      this.dataGridView1.MultiSelect = false;
      this.dataGridView1.Name = "dataGridView1";
      this.dataGridView1.SelectionMode = System.Windows.Forms.DataGridViewSelectionMode.FullRowSelect;
      this.dataGridView1.Size = new System.Drawing.Size(1025, 219);
      this.dataGridView1.TabIndex = 5;
      // 
      // orderNoColumn
      // 
      this.orderNoColumn.DataPropertyName = "OrderNo";
      this.orderNoColumn.HeaderText = "Order #";
      this.orderNoColumn.Name = "orderNoColumn";
      this.orderNoColumn.SortMode = System.Windows.Forms.DataGridViewColumnSortMode.NotSortable;
      this.orderNoColumn.Width = 70;
      // 
      // groupBox5
      // 
      this.groupBox5.Location = new System.Drawing.Point(838, 13);
      this.groupBox5.Name = "groupBox5";
      this.groupBox5.Size = new System.Drawing.Size(200, 290);
      this.groupBox5.TabIndex = 4;
      this.groupBox5.TabStop = false;
      this.groupBox5.Text = "Cut Rolls";
      // 
      // rollNoDataGridViewTextBoxColumn
      // 
      this.rollNoDataGridViewTextBoxColumn.DataPropertyName = "RollNo";
      dataGridViewCellStyle1.Font = new System.Drawing.Font("Microsoft Sans Serif", 8.25F, System.Drawing.FontStyle.Bold, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
      this.rollNoDataGridViewTextBoxColumn.DefaultCellStyle = dataGridViewCellStyle1;
      this.rollNoDataGridViewTextBoxColumn.HeaderText = "Greige Roll";
      this.rollNoDataGridViewTextBoxColumn.Name = "rollNoDataGridViewTextBoxColumn";
      this.rollNoDataGridViewTextBoxColumn.Resizable = System.Windows.Forms.DataGridViewTriState.True;
      this.rollNoDataGridViewTextBoxColumn.SortMode = System.Windows.Forms.DataGridViewColumnSortMode.NotSortable;
      this.rollNoDataGridViewTextBoxColumn.Width = 70;
      // 
      // rollLengthDataGridViewTextBoxColumn
      // 
      this.rollLengthDataGridViewTextBoxColumn.DataPropertyName = "RollLength";
      dataGridViewCellStyle2.Alignment = System.Windows.Forms.DataGridViewContentAlignment.MiddleRight;
      this.rollLengthDataGridViewTextBoxColumn.DefaultCellStyle = dataGridViewCellStyle2;
      this.rollLengthDataGridViewTextBoxColumn.HeaderText = "Length";
      this.rollLengthDataGridViewTextBoxColumn.Name = "rollLengthDataGridViewTextBoxColumn";
      this.rollLengthDataGridViewTextBoxColumn.SortMode = System.Windows.Forms.DataGridViewColumnSortMode.NotSortable;
      this.rollLengthDataGridViewTextBoxColumn.Width = 60;
      // 
      // styleCodeDataGridViewTextBoxColumn
      // 
      this.styleCodeDataGridViewTextBoxColumn.DataPropertyName = "StyleCode";
      this.styleCodeDataGridViewTextBoxColumn.HeaderText = "Style";
      this.styleCodeDataGridViewTextBoxColumn.Name = "styleCodeDataGridViewTextBoxColumn";
      this.styleCodeDataGridViewTextBoxColumn.SortMode = System.Windows.Forms.DataGridViewColumnSortMode.NotSortable;
      this.styleCodeDataGridViewTextBoxColumn.Width = 60;
      // 
      // styleNameDataGridViewTextBoxColumn
      // 
      this.styleNameDataGridViewTextBoxColumn.DataPropertyName = "StyleName";
      this.styleNameDataGridViewTextBoxColumn.HeaderText = "Description";
      this.styleNameDataGridViewTextBoxColumn.Name = "styleNameDataGridViewTextBoxColumn";
      this.styleNameDataGridViewTextBoxColumn.SortMode = System.Windows.Forms.DataGridViewColumnSortMode.NotSortable;
      this.styleNameDataGridViewTextBoxColumn.Width = 125;
      // 
      // colorCodeDataGridViewTextBoxColumn
      // 
      this.colorCodeDataGridViewTextBoxColumn.DataPropertyName = "ColorCode";
      this.colorCodeDataGridViewTextBoxColumn.HeaderText = "Color";
      this.colorCodeDataGridViewTextBoxColumn.Name = "colorCodeDataGridViewTextBoxColumn";
      this.colorCodeDataGridViewTextBoxColumn.SortMode = System.Windows.Forms.DataGridViewColumnSortMode.NotSortable;
      this.colorCodeDataGridViewTextBoxColumn.Width = 60;
      // 
      // colorNameDataGridViewTextBoxColumn
      // 
      this.colorNameDataGridViewTextBoxColumn.DataPropertyName = "ColorName";
      this.colorNameDataGridViewTextBoxColumn.HeaderText = "Description";
      this.colorNameDataGridViewTextBoxColumn.Name = "colorNameDataGridViewTextBoxColumn";
      this.colorNameDataGridViewTextBoxColumn.SortMode = System.Windows.Forms.DataGridViewColumnSortMode.NotSortable;
      this.colorNameDataGridViewTextBoxColumn.Width = 125;
      // 
      // backingCodeDataGridViewTextBoxColumn
      // 
      this.backingCodeDataGridViewTextBoxColumn.DataPropertyName = "BackingCode";
      this.backingCodeDataGridViewTextBoxColumn.HeaderText = "Backing";
      this.backingCodeDataGridViewTextBoxColumn.Name = "backingCodeDataGridViewTextBoxColumn";
      this.backingCodeDataGridViewTextBoxColumn.SortMode = System.Windows.Forms.DataGridViewColumnSortMode.NotSortable;
      this.backingCodeDataGridViewTextBoxColumn.Width = 50;
      // 
      // rollWidthStrDataGridViewTextBoxColumn
      // 
      this.rollWidthStrDataGridViewTextBoxColumn.DataPropertyName = "RollWidthStr";
      this.rollWidthStrDataGridViewTextBoxColumn.HeaderText = "Width";
      this.rollWidthStrDataGridViewTextBoxColumn.Name = "rollWidthStrDataGridViewTextBoxColumn";
      this.rollWidthStrDataGridViewTextBoxColumn.ReadOnly = true;
      this.rollWidthStrDataGridViewTextBoxColumn.SortMode = System.Windows.Forms.DataGridViewColumnSortMode.NotSortable;
      this.rollWidthStrDataGridViewTextBoxColumn.Width = 55;
      // 
      // colDefaultRecipe
      // 
      this.colDefaultRecipe.DataPropertyName = "DefaultRecipe";
      this.colDefaultRecipe.HeaderText = "Default Recipe";
      this.colDefaultRecipe.Name = "colDefaultRecipe";
      this.colDefaultRecipe.Resizable = System.Windows.Forms.DataGridViewTriState.True;
      this.colDefaultRecipe.SortMode = System.Windows.Forms.DataGridViewColumnSortMode.NotSortable;
      this.colDefaultRecipe.Width = 120;
      // 
      // patternRepeatLengthDataGridViewTextBoxColumn
      // 
      this.patternRepeatLengthDataGridViewTextBoxColumn.DataPropertyName = "PatternRepeatLength";
      dataGridViewCellStyle3.Alignment = System.Windows.Forms.DataGridViewContentAlignment.MiddleRight;
      dataGridViewCellStyle3.Format = "N3";
      dataGridViewCellStyle3.NullValue = null;
      this.patternRepeatLengthDataGridViewTextBoxColumn.DefaultCellStyle = dataGridViewCellStyle3;
      this.patternRepeatLengthDataGridViewTextBoxColumn.HeaderText = "Pattern Repeat";
      this.patternRepeatLengthDataGridViewTextBoxColumn.Name = "patternRepeatLengthDataGridViewTextBoxColumn";
      this.patternRepeatLengthDataGridViewTextBoxColumn.SortMode = System.Windows.Forms.DataGridViewColumnSortMode.NotSortable;
      this.patternRepeatLengthDataGridViewTextBoxColumn.Width = 75;
      // 
      // srcGrid
      // 
      this.srcGrid.DataSource = typeof(MahloService.Models.GreigeRoll);
      // 
      // srcSimInfo
      // 
      this.srcSimInfo.DataSource = typeof(MahloService.Simulation.SimInfo);
      // 
      // srcFormSim
      // 
      this.srcFormSim.DataSource = typeof(MahloService.Simulation.FormSim);
      // 
      // FormSim
      // 
      this.AutoScaleDimensions = new System.Drawing.SizeF(6F, 13F);
      this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
      this.ClientSize = new System.Drawing.Size(1085, 540);
      this.Controls.Add(this.groupBox5);
      this.Controls.Add(this.dataGridView1);
      this.Controls.Add(this.groupBox4);
      this.Controls.Add(this.groupBox3);
      this.Controls.Add(this.groupBox2);
      this.Controls.Add(this.groupBox1);
      this.Name = "FormSim";
      this.Text = "Simulator";
      this.groupBox1.ResumeLayout(false);
      this.groupBox1.PerformLayout();
      this.groupBox2.ResumeLayout(false);
      this.groupBox2.PerformLayout();
      this.groupBox3.ResumeLayout(false);
      this.groupBox3.PerformLayout();
      this.groupBox4.ResumeLayout(false);
      this.groupBox4.PerformLayout();
      ((System.ComponentModel.ISupportInitialize)(this.dataGridView1)).EndInit();
      ((System.ComponentModel.ISupportInitialize)(this.srcGrid)).EndInit();
      ((System.ComponentModel.ISupportInitialize)(this.srcSimInfo)).EndInit();
      ((System.ComponentModel.ISupportInitialize)(this.srcFormSim)).EndInit();
      this.ResumeLayout(false);

    }

    #endregion

    private System.Windows.Forms.GroupBox groupBox1;
    private System.Windows.Forms.Button btnAddRoll;
    private System.Windows.Forms.GroupBox groupBox2;
    private System.Windows.Forms.GroupBox groupBox3;
    private System.Windows.Forms.GroupBox groupBox4;
    private System.Windows.Forms.BindingSource srcGrid;
    private System.Windows.Forms.DataGridView dataGridView1;
    private System.Windows.Forms.GroupBox groupBox5;
    private System.Windows.Forms.Button btnRun;
    private System.Windows.Forms.Label label1;
    private System.Windows.Forms.TextBox textBox1;
    private System.Windows.Forms.Label label2;
    private System.Windows.Forms.TextBox textBox2;
    private System.Windows.Forms.DataGridViewTextBoxColumn rollNoDataGridViewTextBoxColumn;
    private System.Windows.Forms.DataGridViewTextBoxColumn orderNoColumn;
    private System.Windows.Forms.DataGridViewTextBoxColumn rollLengthDataGridViewTextBoxColumn;
    private System.Windows.Forms.DataGridViewTextBoxColumn styleCodeDataGridViewTextBoxColumn;
    private System.Windows.Forms.DataGridViewTextBoxColumn styleNameDataGridViewTextBoxColumn;
    private System.Windows.Forms.DataGridViewTextBoxColumn colorCodeDataGridViewTextBoxColumn;
    private System.Windows.Forms.DataGridViewTextBoxColumn colorNameDataGridViewTextBoxColumn;
    private System.Windows.Forms.DataGridViewTextBoxColumn backingCodeDataGridViewTextBoxColumn;
    private System.Windows.Forms.DataGridViewTextBoxColumn rollWidthStrDataGridViewTextBoxColumn;
    private System.Windows.Forms.DataGridViewTextBoxColumn colDefaultRecipe;
    private System.Windows.Forms.DataGridViewTextBoxColumn patternRepeatLengthDataGridViewTextBoxColumn;
    private System.Windows.Forms.Label label3;
    private System.Windows.Forms.TextBox textBox3;
    private System.Windows.Forms.BindingSource srcSimInfo;
    private System.Windows.Forms.Label label4;
    private System.Windows.Forms.TextBox textBox4;
    private System.Windows.Forms.Button btnRemoveRoll;
    private System.Windows.Forms.Label label5;
    private System.Windows.Forms.TextBox textBox5;
    private System.Windows.Forms.Label label6;
    private System.Windows.Forms.TextBox textBox6;
    private System.Windows.Forms.Label label7;
    private System.Windows.Forms.TextBox textBox7;
    private System.Windows.Forms.Button btnExit;
    private System.Windows.Forms.BindingSource srcFormSim;
  }
}