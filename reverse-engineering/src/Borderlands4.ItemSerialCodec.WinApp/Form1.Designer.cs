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
            SuspendLayout();
            // 
            // ItemSerialLabel
            // 
            ItemSerialLabel.AutoSize = true;
            ItemSerialLabel.Location = new Point(12, 21);
            ItemSerialLabel.Name = "ItemSerialLabel";
            ItemSerialLabel.Size = new Size(161, 17);
            ItemSerialLabel.TabIndex = 3;
            ItemSerialLabel.Text = "物品序列号（Item Serial）: ";
            // 
            // ItemSerial
            // 
            ItemSerial.Location = new Point(12, 41);
            ItemSerial.Multiline = true;
            ItemSerial.Name = "ItemSerial";
            ItemSerial.ScrollBars = ScrollBars.Vertical;
            ItemSerial.Size = new Size(776, 230);
            ItemSerial.TabIndex = 2;
            ItemSerial.TextChanged += ItemSerial_TextChanged;
            // 
            // ItemPartsLabel
            // 
            ItemPartsLabel.AutoSize = true;
            ItemPartsLabel.Location = new Point(12, 287);
            ItemPartsLabel.Name = "ItemPartsLabel";
            ItemPartsLabel.Size = new Size(158, 17);
            ItemPartsLabel.TabIndex = 4;
            ItemPartsLabel.Text = "物品配件码（Item Parts）: ";
            // 
            // ItemParts
            // 
            ItemParts.Location = new Point(12, 307);
            ItemParts.Multiline = true;
            ItemParts.Name = "ItemParts";
            ItemParts.ScrollBars = ScrollBars.Vertical;
            ItemParts.Size = new Size(776, 230);
            ItemParts.TabIndex = 5;
            ItemParts.TextChanged += ItemParts_TextChanged;
            // 
            // ItemSerialCopyButton
            // 
            ItemSerialCopyButton.Font = new Font("Microsoft YaHei UI", 9F, FontStyle.Regular, GraphicsUnit.Point, 134);
            ItemSerialCopyButton.Location = new Point(698, 15);
            ItemSerialCopyButton.Name = "ItemSerialCopyButton";
            ItemSerialCopyButton.Size = new Size(90, 23);
            ItemSerialCopyButton.TabIndex = 6;
            ItemSerialCopyButton.Text = "复制 COPY";
            ItemSerialCopyButton.UseVisualStyleBackColor = true;
            ItemSerialCopyButton.Click += ItemSerialCopyButton_Click;
            // 
            // ItemSerialPasteButton
            // 
            ItemSerialPasteButton.Font = new Font("Microsoft YaHei UI", 9F, FontStyle.Regular, GraphicsUnit.Point, 134);
            ItemSerialPasteButton.Location = new Point(602, 15);
            ItemSerialPasteButton.Name = "ItemSerialPasteButton";
            ItemSerialPasteButton.Size = new Size(90, 23);
            ItemSerialPasteButton.TabIndex = 6;
            ItemSerialPasteButton.Text = "粘贴 PASTE";
            ItemSerialPasteButton.UseVisualStyleBackColor = true;
            ItemSerialPasteButton.Click += ItemSerialPasteButton_Click;
            // 
            // ItemPartsCopyButton
            // 
            ItemPartsCopyButton.Font = new Font("Microsoft YaHei UI", 9F, FontStyle.Regular, GraphicsUnit.Point, 134);
            ItemPartsCopyButton.Location = new Point(698, 284);
            ItemPartsCopyButton.Name = "ItemPartsCopyButton";
            ItemPartsCopyButton.Size = new Size(90, 23);
            ItemPartsCopyButton.TabIndex = 6;
            ItemPartsCopyButton.Text = "复制 COPY";
            ItemPartsCopyButton.UseVisualStyleBackColor = true;
            ItemPartsCopyButton.Click += ItemPartsCopyButton_Click;
            // 
            // ItemPartsPasteButton
            // 
            ItemPartsPasteButton.Font = new Font("Microsoft YaHei UI", 9F, FontStyle.Regular, GraphicsUnit.Point, 134);
            ItemPartsPasteButton.Location = new Point(602, 284);
            ItemPartsPasteButton.Name = "ItemPartsPasteButton";
            ItemPartsPasteButton.Size = new Size(90, 23);
            ItemPartsPasteButton.TabIndex = 6;
            ItemPartsPasteButton.Text = "粘贴 PASTE";
            ItemPartsPasteButton.UseVisualStyleBackColor = true;
            ItemPartsPasteButton.Click += ItemPartsPasteButton_Click;
            // 
            // label1
            // 
            label1.AutoSize = true;
            label1.ForeColor = Color.Gray;
            label1.Location = new Point(179, 21);
            label1.Name = "label1";
            label1.Size = new Size(107, 17);
            label1.TabIndex = 3;
            label1.Text = "@Ugy3L+2}TYg...";
            // 
            // label2
            // 
            label2.AutoSize = true;
            label2.ForeColor = Color.Gray;
            label2.Location = new Point(176, 287);
            label2.Name = "label2";
            label2.Size = new Size(129, 17);
            label2.TabIndex = 3;
            label2.Text = "24, 0, 1, 50| 2, 3379...";
            // 
            // StatusTip
            // 
            StatusTip.AutoSize = true;
            StatusTip.ForeColor = Color.Red;
            StatusTip.Location = new Point(12, 540);
            StatusTip.Name = "StatusTip";
            StatusTip.Size = new Size(61, 17);
            StatusTip.TabIndex = 7;
            StatusTip.Text = "StatusTip";
            StatusTip.Click += StatusTip_Click;
            // 
            // Form1
            // 
            AutoScaleDimensions = new SizeF(7F, 17F);
            AutoScaleMode = AutoScaleMode.Font;
            ClientSize = new Size(800, 568);
            Controls.Add(StatusTip);
            Controls.Add(ItemPartsPasteButton);
            Controls.Add(ItemSerialPasteButton);
            Controls.Add(ItemPartsCopyButton);
            Controls.Add(ItemSerialCopyButton);
            Controls.Add(ItemParts);
            Controls.Add(ItemPartsLabel);
            Controls.Add(label2);
            Controls.Add(label1);
            Controls.Add(ItemSerialLabel);
            Controls.Add(ItemSerial);
            FormBorderStyle = FormBorderStyle.FixedSingle;
            Icon = (Icon)resources.GetObject("$this.Icon");
            Name = "Form1";
            Text = "无主之地4物品序列号编解码器 - Borderlands4 Item Serials Codec by ZiGMa GaN";
            Load += Form1_Load;
            ResumeLayout(false);
            PerformLayout();
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
    }
}
