using System.ComponentModel.Design.Serialization;
using System.Text;
using System.Text.RegularExpressions;

namespace Borderlands4.ItemSerialCodec.WinApp
{
    public partial class Form1 : Form
    {
        private readonly ItemSerialDecoder _itemDecoder = new();
        private readonly ItemSerialEncoder _itemEncoder = new();

        [GeneratedRegex(@"(@U[0-9A-Za-z!#\$%&\(\)\*\+\-;<=>\?@\^_`\{\}/~]+)", RegexOptions.Compiled)]
        private static partial Regex SerialRegex { get; }

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
            var input = ItemSerial.Text.Trim();
            if (_encoding)
            {
                return;
            }

            _encoding = false;
            _decoding = true;

            try
            {
                if (!string.IsNullOrEmpty(input))
                {
                    var serials = ExtractSerials(input);

                    if (serials.Length != 0)
                    {
                        ItemParts.Text = string.Empty;

                        foreach (var serial in serials)
                        {
                            try
                            {
                                // 物品代码解码
                                var results = _itemDecoder.DecodeAsString(serial, debug: false);

                                if (!string.IsNullOrWhiteSpace(results))
                                {
                                    // 格式化输出
                                    ItemParts.Text += results + Environment.NewLine;
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
                        ShowTip($"序列号无效 Invalid serial！");
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

        #region Encode & Decode

        private static string[] ExtractSerials(string inputData)
        {
            var serials = new List<string>();

            if (string.IsNullOrWhiteSpace(inputData))
                return [.. serials];

            var matches = SerialRegex.Matches(inputData);

            foreach (Match match in matches)
            {
                if (match.Success && match.Groups.Count > 1)
                {
                    serials.Add(match.Groups[1].Value);
                }
            }

            return [.. serials];
        }

        #endregion

        #region Status Tip

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

        #endregion

        private readonly SlotSequenceInputDialog _slotSequenceDialog = new();
        private readonly string _yamlSlotTemplate = @"
        slot_{0}: 
          serial: '{1}'
          state_flags: {2}";

        private void GenerateYamlButton_Click(object sender, EventArgs e)
        {
            var input = ItemSerial.Text.Trim();
            var serials = ExtractSerials(input);

            if (serials.Length > 0)
            {
                if (_slotSequenceDialog.ShowDialog() == DialogResult.OK)
                {
                    var seq = _slotSequenceDialog.SequenceNumber;
                    var sb = new StringBuilder();
                    sb.AppendLine();

                    foreach (var serial in serials)
                    {
                        sb.AppendLine(string.Format(_yamlSlotTemplate, seq++, serial, 1).Trim('\r', '\n'));
                    }

                    var result = sb.ToString().TrimEnd('\r', '\n');
                    Clipboard.SetText(result);
                }
            }
            else
            {
                MessageBox.Show("未找到有效的物品序列号 - No valid item serials found.");
            }
        }
    }
}
