namespace Borderlands4.ItemSerialCodec.WinApp
{
    partial class Form1
    {
        /// <summary>
        ///  Required designer variable.
        /// </summary>
        private System.ComponentModel.IContainer components = null;

        /// <summary>
        ///  Clean up any resources being used.
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
        ///  Required method for Designer support - do not modify
        ///  the contents of this method with the code editor.
        /// </summary>
        private void InitializeComponent()
        {
            System.ComponentModel.ComponentResourceManager resources = new System.ComponentModel.ComponentResourceManager(typeof(Form1));
            ItemSerialLabel = new Label();
            ItemSerial = new TextBox();
            ItemPartsLabel = new Label();
            ItemParts = new TextBox();
            ItemSerialCopyButton = new Button();
            ItemSerialPasteButton = new Button();
            ItemPartsCopyButton = new Button();
            ItemPartsPasteButton = new Button();
            label1 = new Label();
            label2 = new Label();
            StatusTip = new Label();
            ItemSerialCutButton = new Button();
            ItemPartsCutButton = new Button();
            GenerateYamlButton = new Button();
            tableLayoutPanel1 = new TableLayoutPanel();
            tableLayoutPanel2 = new TableLayoutPanel();
            tableLayoutPanel3 = new TableLayoutPanel();
            tableLayoutPanel1.SuspendLayout();
            tableLayoutPanel2.SuspendLayout();
            tableLayoutPanel3.SuspendLayout();
            SuspendLayout();
            // 
            // ItemSerialLabel
            // 
            ItemSerialLabel.Anchor = AnchorStyles.Top | AnchorStyles.Bottom | AnchorStyles.Left;
            ItemSerialLabel.AutoSize = true;
            ItemSerialLabel.Location = new Point(3, 0);
            ItemSerialLabel.Name = "ItemSerialLabel";
            ItemSerialLabel.Size = new Size(161, 33);
            ItemSerialLabel.TabIndex = 3;
            ItemSerialLabel.Text = "物品序列号（Item Serial）: ";
            ItemSerialLabel.TextAlign = ContentAlignment.MiddleCenter;
            // 
            // ItemSerial
            // 
            ItemSerial.Dock = DockStyle.Fill;
            ItemSerial.Location = new Point(3, 33);
            ItemSerial.Margin = new Padding(3, 0, 3, 3);
            ItemSerial.Multiline = true;
            ItemSerial.Name = "ItemSerial";
            ItemSerial.ScrollBars = ScrollBars.Vertical;
            ItemSerial.Size = new Size(784, 229);
            ItemSerial.TabIndex = 2;
            ItemSerial.TextChanged += ItemSerial_TextChanged;
            // 
            // ItemPartsLabel
            // 
            ItemPartsLabel.Anchor = AnchorStyles.Top | AnchorStyles.Bottom | AnchorStyles.Left;
            ItemPartsLabel.AutoSize = true;
            ItemPartsLabel.Location = new Point(3, 3);
            ItemPartsLabel.Margin = new Padding(3);
            ItemPartsLabel.Name = "ItemPartsLabel";
            ItemPartsLabel.Size = new Size(158, 27);
            ItemPartsLabel.TabIndex = 4;
            ItemPartsLabel.Text = "物品配件码（Item Parts）: ";
            ItemPartsLabel.TextAlign = ContentAlignment.MiddleCenter;
            // 
            // ItemParts
            // 
            ItemParts.Dock = DockStyle.Fill;
            ItemParts.Location = new Point(3, 298);
            ItemParts.Margin = new Padding(3, 0, 3, 3);
            ItemParts.Multiline = true;
            ItemParts.Name = "ItemParts";
            ItemParts.ScrollBars = ScrollBars.Vertical;
            ItemParts.Size = new Size(784, 229);
            ItemParts.TabIndex = 5;
            ItemParts.TextChanged += ItemParts_TextChanged;
            // 
            // ItemSerialCopyButton
            // 
            ItemSerialCopyButton.Anchor = AnchorStyles.Top | AnchorStyles.Bottom | AnchorStyles.Left;
            ItemSerialCopyButton.Font = new Font("Microsoft YaHei UI", 9F, FontStyle.Regular, GraphicsUnit.Point, 134);
            ItemSerialCopyButton.Location = new Point(601, 3);
            ItemSerialCopyButton.Name = "ItemSerialCopyButton";
            ItemSerialCopyButton.Size = new Size(90, 27);
            ItemSerialCopyButton.TabIndex = 6;
            ItemSerialCopyButton.Text = "复制 COPY";
            ItemSerialCopyButton.UseVisualStyleBackColor = true;
            ItemSerialCopyButton.Click += ItemSerialCopyButton_Click;
            // 
            // ItemSerialPasteButton
            // 
            ItemSerialPasteButton.Anchor = AnchorStyles.Top | AnchorStyles.Bottom | AnchorStyles.Left;
            ItemSerialPasteButton.Font = new Font("Microsoft YaHei UI", 9F, FontStyle.Regular, GraphicsUnit.Point, 134);
            ItemSerialPasteButton.Location = new Point(505, 3);
            ItemSerialPasteButton.Name = "ItemSerialPasteButton";
            ItemSerialPasteButton.Size = new Size(90, 27);
            ItemSerialPasteButton.TabIndex = 6;
            ItemSerialPasteButton.Text = "粘贴 PASTE";
            ItemSerialPasteButton.UseVisualStyleBackColor = true;
            ItemSerialPasteButton.Click += ItemSerialPasteButton_Click;
            // 
            // ItemPartsCopyButton
            // 
            ItemPartsCopyButton.Anchor = AnchorStyles.Bottom | AnchorStyles.Left;
            ItemPartsCopyButton.Font = new Font("Microsoft YaHei UI", 9F, FontStyle.Regular, GraphicsUnit.Point, 134);
            ItemPartsCopyButton.Location = new Point(601, 3);
            ItemPartsCopyButton.Name = "ItemPartsCopyButton";
            ItemPartsCopyButton.Size = new Size(90, 27);
            ItemPartsCopyButton.TabIndex = 6;
            ItemPartsCopyButton.Text = "复制 COPY";
            ItemPartsCopyButton.UseVisualStyleBackColor = true;
            ItemPartsCopyButton.Click += ItemPartsCopyButton_Click;
            // 
            // ItemPartsPasteButton
            // 
            ItemPartsPasteButton.Anchor = AnchorStyles.Bottom | AnchorStyles.Left;
            ItemPartsPasteButton.Font = new Font("Microsoft YaHei UI", 9F, FontStyle.Regular, GraphicsUnit.Point, 134);
            ItemPartsPasteButton.Location = new Point(505, 3);
            ItemPartsPasteButton.Name = "ItemPartsPasteButton";
            ItemPartsPasteButton.Size = new Size(90, 27);
            ItemPartsPasteButton.TabIndex = 6;
            ItemPartsPasteButton.Text = "粘贴 PASTE";
            ItemPartsPasteButton.UseVisualStyleBackColor = true;
            ItemPartsPasteButton.Click += ItemPartsPasteButton_Click;
            // 
            // label1
            // 
            label1.Anchor = AnchorStyles.Top | AnchorStyles.Bottom | AnchorStyles.Left;
            label1.AutoSize = true;
            label1.ForeColor = Color.Gray;
            label1.Location = new Point(170, 0);
            label1.Name = "label1";
            label1.Size = new Size(107, 33);
            label1.TabIndex = 3;
            label1.Text = "@Ugy3L+2}TYg...";
            label1.TextAlign = ContentAlignment.MiddleCenter;
            // 
            // label2
            // 
            label2.Anchor = AnchorStyles.Top | AnchorStyles.Bottom | AnchorStyles.Left;
            label2.AutoSize = true;
            label2.ForeColor = Color.Gray;
            label2.Location = new Point(167, 3);
            label2.Margin = new Padding(3);
            label2.Name = "label2";
            label2.Size = new Size(129, 27);
            label2.TabIndex = 3;
            label2.Text = "24, 0, 1, 50| 2, 3379...";
            label2.TextAlign = ContentAlignment.MiddleCenter;
            // 
            // StatusTip
            // 
            StatusTip.AutoSize = true;
            StatusTip.ForeColor = Color.Red;
            StatusTip.Location = new Point(3, 530);
            StatusTip.Name = "StatusTip";
            StatusTip.Size = new Size(61, 17);
            StatusTip.TabIndex = 7;
            StatusTip.Text = "StatusTip";
            StatusTip.Click += StatusTip_Click;
            // 
            // ItemSerialCutButton
            // 
            ItemSerialCutButton.Anchor = AnchorStyles.Top | AnchorStyles.Bottom | AnchorStyles.Left;
            ItemSerialCutButton.Font = new Font("Microsoft YaHei UI", 9F, FontStyle.Regular, GraphicsUnit.Point, 134);
            ItemSerialCutButton.Location = new Point(697, 3);
            ItemSerialCutButton.Name = "ItemSerialCutButton";
            ItemSerialCutButton.Size = new Size(90, 27);
            ItemSerialCutButton.TabIndex = 6;
            ItemSerialCutButton.Text = "剪切 CUT";
            ItemSerialCutButton.UseVisualStyleBackColor = true;
            ItemSerialCutButton.Click += ItemSerialCutButton_Click;
            // 
            // ItemPartsCutButton
            // 
            ItemPartsCutButton.Anchor = AnchorStyles.Bottom | AnchorStyles.Left;
            ItemPartsCutButton.Font = new Font("Microsoft YaHei UI", 9F, FontStyle.Regular, GraphicsUnit.Point, 134);
            ItemPartsCutButton.Location = new Point(697, 3);
            ItemPartsCutButton.Name = "ItemPartsCutButton";
            ItemPartsCutButton.Size = new Size(90, 27);
            ItemPartsCutButton.TabIndex = 6;
            ItemPartsCutButton.Text = "剪切 CUT";
            ItemPartsCutButton.UseVisualStyleBackColor = true;
            ItemPartsCutButton.Click += ItemPartsCutButton_Click;
            // 
            // GenerateYamlButton
            // 
            GenerateYamlButton.Anchor = AnchorStyles.Top | AnchorStyles.Bottom | AnchorStyles.Left;
            GenerateYamlButton.Location = new Point(399, 3);
            GenerateYamlButton.Name = "GenerateYamlButton";
            GenerateYamlButton.Size = new Size(100, 27);
            GenerateYamlButton.TabIndex = 8;
            GenerateYamlButton.Text = "YAML Snippet";
            GenerateYamlButton.UseVisualStyleBackColor = true;
            GenerateYamlButton.Click += GenerateYamlButton_Click;
            // 
            // tableLayoutPanel1
            // 
            tableLayoutPanel1.ColumnCount = 1;
            tableLayoutPanel1.ColumnStyles.Add(new ColumnStyle(SizeType.Percent, 100F));
            tableLayoutPanel1.Controls.Add(ItemSerial, 0, 1);
            tableLayoutPanel1.Controls.Add(ItemParts, 0, 3);
            tableLayoutPanel1.Controls.Add(tableLayoutPanel2, 0, 0);
            tableLayoutPanel1.Controls.Add(tableLayoutPanel3, 0, 2);
            tableLayoutPanel1.Controls.Add(StatusTip, 0, 4);
            tableLayoutPanel1.Dock = DockStyle.Fill;
            tableLayoutPanel1.Location = new Point(5, 5);
            tableLayoutPanel1.Name = "tableLayoutPanel1";
            tableLayoutPanel1.RowCount = 5;
            tableLayoutPanel1.RowStyles.Add(new RowStyle(SizeType.Absolute, 33F));
            tableLayoutPanel1.RowStyles.Add(new RowStyle(SizeType.Percent, 50F));
            tableLayoutPanel1.RowStyles.Add(new RowStyle(SizeType.Absolute, 33F));
            tableLayoutPanel1.RowStyles.Add(new RowStyle(SizeType.Percent, 50F));
            tableLayoutPanel1.RowStyles.Add(new RowStyle(SizeType.Absolute, 20F));
            tableLayoutPanel1.Size = new Size(790, 551);
            tableLayoutPanel1.TabIndex = 9;
            // 
            // tableLayoutPanel2
            // 
            tableLayoutPanel2.ColumnCount = 7;
            tableLayoutPanel2.ColumnStyles.Add(new ColumnStyle());
            tableLayoutPanel2.ColumnStyles.Add(new ColumnStyle());
            tableLayoutPanel2.ColumnStyles.Add(new ColumnStyle(SizeType.Percent, 100F));
            tableLayoutPanel2.ColumnStyles.Add(new ColumnStyle());
            tableLayoutPanel2.ColumnStyles.Add(new ColumnStyle());
            tableLayoutPanel2.ColumnStyles.Add(new ColumnStyle());
            tableLayoutPanel2.ColumnStyles.Add(new ColumnStyle());
            tableLayoutPanel2.Controls.Add(ItemSerialLabel, 0, 0);
            tableLayoutPanel2.Controls.Add(GenerateYamlButton, 3, 0);
            tableLayoutPanel2.Controls.Add(label1, 1, 0);
            tableLayoutPanel2.Controls.Add(ItemSerialPasteButton, 4, 0);
            tableLayoutPanel2.Controls.Add(ItemSerialCopyButton, 5, 0);
            tableLayoutPanel2.Controls.Add(ItemSerialCutButton, 6, 0);
            tableLayoutPanel2.Dock = DockStyle.Fill;
            tableLayoutPanel2.Location = new Point(0, 0);
            tableLayoutPanel2.Margin = new Padding(0);
            tableLayoutPanel2.Name = "tableLayoutPanel2";
            tableLayoutPanel2.RowCount = 1;
            tableLayoutPanel2.RowStyles.Add(new RowStyle(SizeType.Percent, 100F));
            tableLayoutPanel2.Size = new Size(790, 33);
            tableLayoutPanel2.TabIndex = 6;
            // 
            // tableLayoutPanel3
            // 
            tableLayoutPanel3.ColumnCount = 7;
            tableLayoutPanel3.ColumnStyles.Add(new ColumnStyle());
            tableLayoutPanel3.ColumnStyles.Add(new ColumnStyle());
            tableLayoutPanel3.ColumnStyles.Add(new ColumnStyle(SizeType.Percent, 100F));
            tableLayoutPanel3.ColumnStyles.Add(new ColumnStyle());
            tableLayoutPanel3.ColumnStyles.Add(new ColumnStyle());
            tableLayoutPanel3.ColumnStyles.Add(new ColumnStyle());
            tableLayoutPanel3.ColumnStyles.Add(new ColumnStyle());
            tableLayoutPanel3.Controls.Add(ItemPartsPasteButton, 4, 0);
            tableLayoutPanel3.Controls.Add(ItemPartsLabel, 0, 0);
            tableLayoutPanel3.Controls.Add(label2, 1, 0);
            tableLayoutPanel3.Controls.Add(ItemPartsCopyButton, 5, 0);
            tableLayoutPanel3.Controls.Add(ItemPartsCutButton, 6, 0);
            tableLayoutPanel3.Dock = DockStyle.Fill;
            tableLayoutPanel3.Location = new Point(0, 265);
            tableLayoutPanel3.Margin = new Padding(0);
            tableLayoutPanel3.Name = "tableLayoutPanel3";
            tableLayoutPanel3.RowCount = 1;
            tableLayoutPanel3.RowStyles.Add(new RowStyle(SizeType.Percent, 100F));
            tableLayoutPanel3.Size = new Size(790, 33);
            tableLayoutPanel3.TabIndex = 7;
            // 
            // Form1
            // 
            AutoScaleDimensions = new SizeF(7F, 17F);
            AutoScaleMode = AutoScaleMode.Font;
            ClientSize = new Size(800, 561);
            Controls.Add(tableLayoutPanel1);
            Icon = (Icon)resources.GetObject("$this.Icon");
            MinimumSize = new Size(816, 600);
            Name = "Form1";
            Padding = new Padding(5);
            StartPosition = FormStartPosition.CenterScreen;
            Text = "无主之地4物品序列号编解码器 - Borderlands4 Item Serials Codec by ZiGMa GaN";
            Load += Form1_Load;
            tableLayoutPanel1.ResumeLayout(false);
            tableLayoutPanel1.PerformLayout();
            tableLayoutPanel2.ResumeLayout(false);
            tableLayoutPanel2.PerformLayout();
            tableLayoutPanel3.ResumeLayout(false);
            tableLayoutPanel3.PerformLayout();
            ResumeLayout(false);
        }

        #endregion

        private Label ItemSerialLabel;
        private TextBox ItemSerial;
        private Label ItemPartsLabel;
        private TextBox ItemParts;
        private Button ItemSerialCopyButton;
        private Button ItemSerialPasteButton;
        private Button ItemPartsCopyButton;
        private Button ItemPartsPasteButton;
        private Label label1;
        private Label label2;
        private Label StatusTip;
        private Button ItemSerialCutButton;
        private Button ItemPartsCutButton;
        private Button GenerateYamlButton;
        private TableLayoutPanel tableLayoutPanel1;
        private TableLayoutPanel tableLayoutPanel2;
        private TableLayoutPanel tableLayoutPanel3;
    }
}
