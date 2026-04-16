using System.ComponentModel;

namespace Borderlands4.ItemSerialCodec.WinApp
{
    public partial class SlotSequenceInputDialog : Form
    {
        [DesignerSerializationVisibility(DesignerSerializationVisibility.Hidden)]
        public int SequenceNumber { get; set; }

        [DesignerSerializationVisibility(DesignerSerializationVisibility.Hidden)]
        public int TagId { get; set; }

        public SlotSequenceInputDialog()
        {
            InitializeComponent();
        }

        private void SlotSequenceInputDialog_Load(object sender, EventArgs e)
        {
            ItemTag.SelectedIndex = 0;
        }

        private void button1_Click(object sender, EventArgs e)
        {
            SequenceNumber = (int)SlotSequence.Value;

            TagId = ItemTag.SelectedIndex switch
            {
                0 => 1, //无
                1 => 3, //收藏
                2 => 5, //垃圾
                3 => 9, //银行
                4 => 17, //标签1
                5 => 33, //标签2
                6 => 65, //标签3
                7 => 129, //标签4
                _ => throw new InvalidOperationException("Invalid item tag index.")
            };

            DialogResult = DialogResult.OK;
            Close();
        }
    }
}
