using System.ComponentModel;

namespace Borderlands4.ItemSerialCodec.WinApp
{
    public partial class SlotSequenceInputDialog : Form
    {
        [DesignerSerializationVisibility(DesignerSerializationVisibility.Hidden)]
        public int SequenceNumber { get; set; }

        public SlotSequenceInputDialog()
        {
            InitializeComponent();
        }

        private void button1_Click(object sender, EventArgs e)
        {
            SequenceNumber = (int)SlotSequence.Value;
            DialogResult = DialogResult.OK;

            Close();
        }
    }
}
