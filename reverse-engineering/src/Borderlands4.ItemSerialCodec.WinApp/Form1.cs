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

        [GeneratedRegex(@"(((?:\d+|""[a-zA-Z0-9\._]+""|\\""[a-zA-Z0-9\._]+\\"")[ \t]*(?:,[ \t]*(?:\d+|""[a-zA-Z0-9\._]+""|\\""[a-zA-Z0-9\._]+\\""))*[ \t]*\|[ \t]*){2,}[ \t]*\|[ \t]*(""[a-zA-Z0-9\._]+""[ \t]*|\\""[a-zA-Z0-9\._]+\\""[ \t]*|\{[ \t]*\d+[ \t]*(?:[ \t]*:[ \t]*(?:\d+|\[[ \t]*(?:\d+|""[a-zA-Z0-9\._]+""|\\""[a-zA-Z0-9\._]+\\"")(?:[ \t]+(?:\d+|""[a-zA-Z0-9\._]+""|\\""[a-zA-Z0-9\._]+\\""))*[ \t]*\]))?[ \t]*\}[ \t]*)+[ \t]*(\|[ \t]*)?(((?:[ \t]*\d+|""[a-zA-Z0-9\._]+""|\\""[a-zA-Z0-9\._]+\\"")[ \t]*(?:,[ \t]*(?:\d+|""[a-zA-Z0-9\._]+""|\\""[a-zA-Z0-9\._]+\\""))*[ \t]*\|))?)", RegexOptions.Compiled)]
        private static partial Regex PartsCodeRegex { get; }

        private bool _encoding = false;
        private bool _decoding = false;

        public Form1()
        {
            InitializeComponent();
        }

        private void Form1_Load(object sender, EventArgs e)
        {
            Text += $" - v{GetAssemblyVersion()}";
            ClearTip();
        }

        /// <summary>
        /// 读取程序集的版本。
        /// </summary>
        /// <param name="assembly"></param>
        /// <returns></returns>
        public static string GetAssemblyVersion()
        {
            var assembly = typeof(Form1).Assembly;
            var ver = assembly.GetName().Version?.ToString();

            return !string.IsNullOrEmpty(ver) ? ver : "0.0.0.0";
        }

        #region Copy & Paste & Cut

        private void ItemSerialPasteButton_Click(object sender, EventArgs e)
        {
            ItemSerial.Text = Clipboard.GetText();
        }

        private void ItemSerialCopyButton_Click(object sender, EventArgs e)
        {
            if (!string.IsNullOrEmpty(ItemSerial.Text))
            {
                Clipboard.SetText(ItemSerial.Text.Trim('\r', '\n'));
            }
        }

        private void ItemSerialCutButton_Click(object sender, EventArgs e)
        {
            if (!string.IsNullOrEmpty(ItemSerial.Text))
            {
                Clipboard.SetText(ItemSerial.Text.Trim('\r', '\n'));
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
                Clipboard.SetText(ItemParts.Text.Trim('\r', '\n'));
            }
        }

        private void ItemPartsCutButton_Click(object sender, EventArgs e)
        {
            if (!string.IsNullOrEmpty(ItemParts.Text))
            {
                Clipboard.SetText(ItemParts.Text.Trim('\r', '\n'));
                ItemParts.Text = string.Empty;
            }
        }

        #endregion

        #region Encode & Decode

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
                                var results = _itemDecoder.DecodeAsPartsString(serial, debug: false);

                                if (!string.IsNullOrWhiteSpace(results))
                                {
                                    // 格式化输出
                                    ItemParts.Text += results + Environment.NewLine;
                                    ClearTip();
                                }
                                else
                                {
                                    ItemParts.Text += $"!!! ERROR: {serial} !!!" + Environment.NewLine;
                                    ShowTip($"物品序列号无效 Invalid serial！");
                                }
                            }
                            catch (Exception ex)
                            {
                                ItemParts.Text += $"!!! {ItemSerialDecoder.FormatResults(_itemDecoder.GetDecodedTokens())}... !!!" + Environment.NewLine;
                                ShowTip(ex.Message, ex.ToString());
                            }
                        }
                    }
                    else
                    {
                        ItemParts.Text = string.Empty;
                        ShowTip($"物品序列号无效 Invalid serial！");
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
            var input = ItemParts.Text.Trim();
            if (_decoding)
            {
                return;
            }

            _encoding = true;
            _decoding = false;

            try
            {
                if (!string.IsNullOrEmpty(input))
                {
                    var partsCodes = ExtractPartsString(input);

                    if (partsCodes.Length != 0)
                    {
                        ItemSerial.Text = string.Empty;

                        foreach (var partsCode in partsCodes)
                        {
                            //if (!ItemPartsValidator.ValidateItemParts(partsCode))
                            //{
                            //    ItemSerial.Text += $"!!! INVALID PARTS STRING: {partsCode} !!!" + Environment.NewLine;
                            //    ShowTip("物品配件码无效！Invalid parts string！");
                            //    return;
                            //}

                            try
                            {
                                var encodedSerial = _itemEncoder.EncodeToSerial(partsCode);

                                ItemSerial.Text += encodedSerial + Environment.NewLine;
                                ClearTip();
                            }
                            catch (Exception ex)
                            {
                                ItemSerial.Text += $"!!! ERROR: {partsCode} !!!" + Environment.NewLine;
                                ShowTip(ex.Message, ex.ToString());
                            }
                        }
                    }
                    else
                    {
                        ItemSerial.Text = string.Empty;
                        ShowTip("物品配件码无效！Invalid parts string！");
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

        private static string[] ExtractSerials(string inputData)
        {
            var serials = new List<string>();

            if (string.IsNullOrWhiteSpace(inputData))
                return [];

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

        private static string[] ExtractPartsString(string inputData)
        {
            var partsStrings = new List<string>();

            if (string.IsNullOrWhiteSpace(inputData))
                return [];

            //foreach (var line in inputData.Split(['\r', '\n'], StringSplitOptions.RemoveEmptyEntries))
            //{
            //    var matches = PartsCodeRegex.Matches(line);
            //    if (matches.Count > 0)
            //    {
            //        var match = matches[0];
            //        if (match.Success && match.Groups.Count > 1)
            //        {
            //            partsStrings.Add(match.Groups[1].Value);
            //        }
            //    }
            //}
            var matches = PartsCodeRegex.Matches(inputData);

            foreach (Match match in matches)
            {
                if (match.Success && match.Groups.Count > 1)
                {
                    partsStrings.Add(match.Groups[1].Value);
                }
            }

            return [.. partsStrings];
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

        #region YAML

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
                    var tag = _slotSequenceDialog.TagId;
                    var sb = new StringBuilder();
                    sb.AppendLine();

                    foreach (var serial in serials)
                    {
                        sb.AppendLine(string.Format(_yamlSlotTemplate, seq++, serial, tag).Trim('\r', '\n'));
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

        #endregion
    }
}
