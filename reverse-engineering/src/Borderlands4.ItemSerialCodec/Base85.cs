using System.Collections.ObjectModel;
using System.Text;
using System.Text.RegularExpressions;

namespace Borderlands4.ItemSerialCodec;

public partial class Base85
{
    private readonly static string _alphabet = "0123456789ABCDEFGHIJKLMNOPQRSTUVWXYZabcdefghijklmnopqrstuvwxyz!#$%&()*+-;<=>?@^_`{/}~";
    private readonly static ReadOnlyDictionary<char, int> _charToValue;
    private readonly static ReadOnlyDictionary<int, char> _valueToChar;

    [GeneratedRegex(@"^[0-9A-Za-z!#\$%&\(\)\*\+\-;<=>\?@\^_`\{\}/~]+$", RegexOptions.Compiled)]
    private static partial Regex Base85Regex { get; }

    static Base85()
    {
        _charToValue = _alphabet.Select((c, i) => new { c, i }).ToDictionary(x => x.c, x => x.i).AsReadOnly();
        _valueToChar = _alphabet.Select((c, i) => new { c, i }).ToDictionary(x => x.i, x => x.c).AsReadOnly();
    }

    public string Encode(byte[] data)
    {
        if (data == null || data.Length == 0)
        {
            return string.Empty;
        }

        // 确保数据长度是4的倍数
        var padding = (4 - data.Length % 4) % 4;
        var paddedData = new byte[data.Length + padding];
        Array.Copy(data, paddedData, data.Length);

        var result = new StringBuilder();

        for (var i = 0; i < paddedData.Length; i += 4)
        {
            // 将4个字节转换为32位整数
            var value = 0u;
            for (var j = 0; j < 4; j++)
            {
                value = value << 8 | paddedData[i + j];
            }

            // 转换为Base85
            var chunk = new char[5];
            for (var j = 4; j >= 0; j--)
            {
                chunk[j] = _valueToChar[(int)(value % 85)];
                value /= 85;
            }

            result.Append(chunk);
        }

        // 去除填充字符
        if (padding > 0)
        {
            result.Length -= padding;
        }

        return result.ToString();
    }

    public byte[] Decode(string base85Str)
    {
        if (!Base85Regex.IsMatch(base85Str))
        {
            throw new InvalidOperationException("invalid base85 string.");
        }

        // 计算填充
        var padding = 0;
        while (base85Str.Length % 5 != 0)
        {
            base85Str += '~';
            padding++;
        }

        var result = new List<byte>();

        for (int i = 0; i < base85Str.Length; i += 5)
        {
            var chunk = base85Str.Substring(i, 5);

            // 将Base85字符转换为数值
            var value = 0L;
            foreach (var c in chunk)
            {
                value = value * 85 + _charToValue[c];
            }

            // 将32位数值转换为4个字节
            var bytesChunk = new byte[4];
            for (int j = 3; j >= 0; j--)
            {
                bytesChunk[j] = (byte)(value & 0xFF);
                value >>= 8;
            }

            result.AddRange(bytesChunk);
        }

        // 去除填充的字节
        if (padding > 0)
        {
            result.RemoveRange(result.Count - padding, padding);
        }

        return [.. result];
    }

}
