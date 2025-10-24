using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;

namespace Borderlands4.ItemSerialCodec.WinApp
{
    public partial class SlotSequenceInputDialog : Form
    {
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
