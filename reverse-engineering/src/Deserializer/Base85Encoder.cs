using System.Text;

namespace Deserializer;

public class Base85Encoder
{
    private readonly string _alphabet = "0123456789ABCDEFGHIJKLMNOPQRSTUVWXYZabcdefghijklmnopqrstuvwxyz!#$%&()*+-;<=>?@^_`{/}~";
    private readonly Dictionary<int, char> _valueToChar;

    public Base85Encoder()
    {
        _valueToChar = _alphabet.Select((c, i) => new { c, i }).ToDictionary(x => x.i, x => x.c);
    }

    public string Encode(byte[] data)
    {
        if (data == null || data.Length == 0)
            return string.Empty;

        // 确保数据长度是4的倍数
        int padding = (4 - (data.Length % 4)) % 4;
        byte[] paddedData = new byte[data.Length + padding];
        Array.Copy(data, paddedData, data.Length);

        StringBuilder result = new StringBuilder();

        for (int i = 0; i < paddedData.Length; i += 4)
        {
            // 将4个字节转换为32位整数
            uint value = 0;
            for (int j = 0; j < 4; j++)
            {
                value = (value << 8) | paddedData[i + j];
            }

            // 转换为Base85
            char[] chunk = new char[5];
            for (int j = 4; j >= 0; j--)
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

    public byte[] MirrorBytes(byte[] data)
    {
        return data.Select(MirrorByte).ToArray();
    }

    private byte MirrorByte(byte b)
    {
        byte mirrored = 0;
        for (int i = 0; i < 8; i++)
        {
            if ((b & (1 << i)) != 0)
                mirrored |= (byte)(1 << (7 - i));
        }
        return mirrored;
    }
}