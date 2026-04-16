namespace Borderlands4.ItemSerialCodec.WinApp
{
    partial class SlotSequenceInputDialog
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
            System.ComponentModel.ComponentResourceManager resources = new System.ComponentModel.ComponentResourceManager(typeof(SlotSequenceInputDialog));
            label1 = new Label();
            label2 = new Label();
            tableLayoutPanel1 = new TableLayoutPanel();
            button1 = new Button();
            tableLayoutPanel2 = new TableLayoutPanel();
            SlotSequence = new NumericUpDown();
            ItemTag = new ComboBox();
            tableLayoutPanel1.SuspendLayout();
            tableLayoutPanel2.SuspendLayout();
            ((System.ComponentModel.ISupportInitialize)SlotSequence).BeginInit();
            SuspendLayout();
            // 
            // label1
            // 
            label1.Anchor = AnchorStyles.Top;
            label1.AutoSize = true;
            label1.Location = new Point(41, 30);
            label1.Name = "label1";
            label1.Size = new Size(285, 17);
            label1.TabIndex = 0;
            label1.Text = "Please enter the starting slot sequence number.";
            // 
            // label2
            // 
            label2.Anchor = AnchorStyles.Top;
            label2.AutoSize = true;
            label2.Location = new Point(96, 0);
            label2.Name = "label2";
            label2.Size = new Size(176, 17);
            label2.TabIndex = 0;
            label2.Text = "请输入起始栏位序号和标签类型";
            // 
            // tableLayoutPanel1
            // 
            tableLayoutPanel1.ColumnCount = 1;
            tableLayoutPanel1.ColumnStyles.Add(new ColumnStyle(SizeType.Percent, 100F));
            tableLayoutPanel1.Controls.Add(label2, 0, 0);
            tableLayoutPanel1.Controls.Add(label1, 0, 1);
            tableLayoutPanel1.Controls.Add(button1, 0, 3);
            tableLayoutPanel1.Controls.Add(tableLayoutPanel2, 0, 2);
            tableLayoutPanel1.Location = new Point(12, 12);
            tableLayoutPanel1.Name = "tableLayoutPanel1";
            tableLayoutPanel1.RowCount = 4;
            tableLayoutPanel1.RowStyles.Add(new RowStyle(SizeType.Absolute, 30F));
            tableLayoutPanel1.RowStyles.Add(new RowStyle(SizeType.Absolute, 30F));
            tableLayoutPanel1.RowStyles.Add(new RowStyle(SizeType.Absolute, 30F));
            tableLayoutPanel1.RowStyles.Add(new RowStyle());
            tableLayoutPanel1.Size = new Size(368, 147);
            tableLayoutPanel1.TabIndex = 2;
            // 
            // button1
            // 
            button1.Anchor = AnchorStyles.Top;
            button1.Location = new Point(47, 93);
            button1.Name = "button1";
            button1.Size = new Size(274, 50);
            button1.TabIndex = 4;
            button1.Text = "生成 YAML 并复制到剪贴板\r\nGenerate YAML and copy to clipboard";
            button1.UseVisualStyleBackColor = true;
            button1.Click += button1_Click;
            // 
            // tableLayoutPanel2
            // 
            tableLayoutPanel2.ColumnCount = 2;
            tableLayoutPanel2.ColumnStyles.Add(new ColumnStyle(SizeType.Percent, 50F));
            tableLayoutPanel2.ColumnStyles.Add(new ColumnStyle(SizeType.Percent, 50F));
            tableLayoutPanel2.Controls.Add(SlotSequence, 0, 0);
            tableLayoutPanel2.Controls.Add(ItemTag, 1, 0);
            tableLayoutPanel2.Dock = DockStyle.Fill;
            tableLayoutPanel2.Location = new Point(0, 60);
            tableLayoutPanel2.Margin = new Padding(0);
            tableLayoutPanel2.Name = "tableLayoutPanel2";
            tableLayoutPanel2.RowCount = 1;
            tableLayoutPanel2.RowStyles.Add(new RowStyle(SizeType.Percent, 100F));
            tableLayoutPanel2.RowStyles.Add(new RowStyle(SizeType.Absolute, 20F));
            tableLayoutPanel2.Size = new Size(368, 30);
            tableLayoutPanel2.TabIndex = 3;
            // 
            // SlotSequence
            // 
            SlotSequence.Dock = DockStyle.Right;
            SlotSequence.Location = new Point(88, 3);
            SlotSequence.Margin = new Padding(3, 3, 15, 3);
            SlotSequence.Maximum = new decimal(new int[] { 9999, 0, 0, 0 });
            SlotSequence.Name = "SlotSequence";
            SlotSequence.Size = new Size(81, 23);
            SlotSequence.TabIndex = 2;
            // 
            // ItemTag
            // 
            ItemTag.DropDownStyle = ComboBoxStyle.DropDownList;
            ItemTag.FormattingEnabled = true;
            ItemTag.Items.AddRange(new object[] { "无", "收藏", "垃圾", "银行", "标签1", "标签2", "标签3", "标签4" });
            ItemTag.Location = new Point(199, 3);
            ItemTag.Margin = new Padding(15, 3, 3, 3);
            ItemTag.Name = "ItemTag";
            ItemTag.Size = new Size(81, 25);
            ItemTag.TabIndex = 3;
            // 
            // SlotSequenceInputDialog
            // 
            AutoScaleDimensions = new SizeF(7F, 17F);
            AutoScaleMode = AutoScaleMode.Font;
            ClientSize = new Size(392, 172);
            Controls.Add(tableLayoutPanel1);
            FormBorderStyle = FormBorderStyle.FixedDialog;
            Icon = (Icon)resources.GetObject("$this.Icon");
            MaximizeBox = false;
            MinimizeBox = false;
            Name = "SlotSequenceInputDialog";
            ShowInTaskbar = false;
            SizeGripStyle = SizeGripStyle.Hide;
            StartPosition = FormStartPosition.CenterParent;
            Text = "背包栏位序号 - Backpack Slot Sequence";
            Load += SlotSequenceInputDialog_Load;
            tableLayoutPanel1.ResumeLayout(false);
            tableLayoutPanel1.PerformLayout();
            tableLayoutPanel2.ResumeLayout(false);
            ((System.ComponentModel.ISupportInitialize)SlotSequence).EndInit();
            ResumeLayout(false);
        }

        #endregion

        private Label label1;
        private Label label2;
        private TableLayoutPanel tableLayoutPanel1;
        private Button button1;
        private TableLayoutPanel tableLayoutPanel2;
        private NumericUpDown SlotSequence;
        private ComboBox ItemTag;
    }
}