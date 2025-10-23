using System.Text.RegularExpressions;

namespace Borderlands4.ItemSerialCodec.WinApp
{
    public partial class Form1 : Form
    {
        private readonly Base85Decoder _base85Decoder = new();
        private readonly ItemSerialDecoder _itemDecoder = new();
        private readonly ItemSerialEncoder _itemEncoder = new();

        [GeneratedRegex(@"^[0-9A-Za-z!#\$%&\(\)\*\+\-;<=>\?@\^_`\{\}/~]+$", RegexOptions.Compiled)]
        private static partial Regex Base85Regex { get; }

        private bool _encoding = false;
        private bool _decoding = false;

        public Form1()
        {
            InitializeComponent();
        }

        private void Form1_Load(object sender, EventArgs e)
        {
            ClearTip();
        }

        private void ItemSerialPasteButton_Click(object sender, EventArgs e)
        {
            ItemSerial.Text = Clipboard.GetText();
        }

        private void ItemSerialCopyButton_Click(object sender, EventArgs e)
        {
            if (!string.IsNullOrEmpty(ItemSerial.Text))
            {
                Clipboard.SetText(ItemSerial.Text);
            }
        }

        private void ItemSerialCutButton_Click(object sender, EventArgs e)
        {
            if (!string.IsNullOrEmpty(ItemSerial.Text))
            {
                Clipboard.SetText(ItemSerial.Text);
                ItemSerial.Text = string.Empty;
            }
        }

        private void ItemPartsPasteButton_Click(object sender, EventArgs e)
        {
            ItemParts.Text = Clipboard.GetText();
        }

        private void ItemPartsCopyButton_Click(object sender, EventArgs e)
        {
            if (!string.IsNullOrEmpty(ItemParts.Text))
            {
                Clipboard.SetText(ItemParts.Text);
            }
        }

        private void ItemPartsCutButton_Click(object sender, EventArgs e)
        {
            if (!string.IsNullOrEmpty(ItemParts.Text))
            {
                Clipboard.SetText(ItemParts.Text);
                ItemParts.Text = string.Empty;
            }
        }

        private void ItemSerial_TextChanged(object sender, EventArgs e)
        {
            if (_encoding)
            {
                return;
            }

            _encoding = false;
            _decoding = true;

            try
            {
                if (!string.IsNullOrEmpty(ItemSerial.Text.Trim()))
                {
                    var serialCodes = ItemSerial.Text.Trim().Split('\r', '\n')
                    .Select(o => o.Trim())
                    .Where(o => !string.IsNullOrWhiteSpace(o));

                    if (serialCodes.Any())
                    {
                        ItemParts.Text = string.Empty;

                        foreach (var serial in serialCodes)
                        {
                            if (!serial.StartsWith("@U"))
                            {
                                ItemParts.Text = string.Empty;
                                ShowTip("序列号必须以 @U 开头！Serial must start with '@U'");
                                return;
                            }

                            if (!Base85Regex.IsMatch(serial))
                            {
                                ItemParts.Text = string.Empty;
                                ShowTip("无效的 Base85 格式！Invalid Base85 string");
                                return;
                            }

                            try
                            {
                                // Base85解码为bitstream
                                var bitstream = _base85Decoder.DecodeToBitStream(serial);
                                // 物品代码解码
                                var results = _itemDecoder.DecodeBitstream(bitstream, debug: false);

                                if (results.SelectMany(o => o).Any())
                                {
                                    // 格式化输出
                                    ItemParts.Text += _itemDecoder.FormatResults(results) + Environment.NewLine;
                                    ClearTip();
                                }
                                else
                                {
                                    ItemParts.Text = string.Empty;
                                    ShowTip($"序列号无效 Invalid serial！");
                                    return;
                                }
                            }
                            catch (Exception ex)
                            {
                                ItemParts.Text = string.Empty;
                                ShowTip(ex.Message, ex.ToString());
                                return;
                            }
                        }
                    }
                    else
                    {
                        ItemParts.Text = string.Empty;
                        ClearTip();
                    }
                }
                else
                {
                    ItemParts.Text = string.Empty;
                    ClearTip();
                }
            }
            finally
            {
                _encoding = false;
                _decoding = false;
            }
        }

        private void ItemParts_TextChanged(object sender, EventArgs e)
        {
            if (_decoding)
            {
                return;
            }

            _encoding = true;
            _decoding = false;

            try
            {
                if (!string.IsNullOrEmpty(ItemParts.Text.Trim()))
                {
                    var partsCodes = ItemParts.Text.Trim().Split('\r', '\n')
                    .Select(o => o.Trim())
                    .Where(o => !string.IsNullOrWhiteSpace(o));

                    if (partsCodes.Any())
                    {
                        ItemSerial.Text = string.Empty;

                        foreach (var parts in partsCodes)
                        {
                            if (!ItemPartsValidator.ValidateItemParts(parts))
                            {
                                ItemSerial.Text = string.Empty;
                                ShowTip("无效的配件码格式！Invalid parts string");
                                return;
                            }

                            try
                            {
                                var encodedSerial = _itemEncoder.EncodeToSerial(parts);

                                ItemSerial.Text += encodedSerial + Environment.NewLine;
                                ClearTip();
                            }
                            catch (Exception ex)
                            {
                                ItemSerial.Text = string.Empty;
                                ShowTip(ex.Message, ex.ToString());
                                return;
                            }
                        }
                    }
                    else
                    {
                        ItemSerial.Text = string.Empty;
                        ClearTip();
                    }
                }
                else
                {
                    ItemSerial.Text = string.Empty;
                    ClearTip();
                }
            }
            finally
            {
                _encoding = false;
                _decoding = false;
            }
        }

        private void ShowTip(string message, string? tooltip = null)
        {
            StatusTip.Text = message;
            StatusTip.Tag = tooltip;
        }

        private void ClearTip()
        {
            StatusTip.Text = string.Empty;
            StatusTip.Tag = null;
        }

        private void StatusTip_Click(object sender, EventArgs e)
        {
            if (!string.IsNullOrEmpty(StatusTip.Text))
            {
                if (StatusTip.Tag is not null)
                {
                    MessageBox.Show(StatusTip.Tag.ToString(), StatusTip.Text);
                }
                else
                {
                    MessageBox.Show(StatusTip.Text);
                }
            }
        }
    }
}
