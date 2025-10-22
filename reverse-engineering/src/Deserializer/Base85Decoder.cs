namespace Deserializer;

public class Base85Decoder
{
    private readonly string _alphabet = "0123456789ABCDEFGHIJKLMNOPQRSTUVWXYZabcdefghijklmnopqrstuvwxyz!#$%&()*+-;<=>?@^_`{/}~";
    private readonly Dictionary<char, int> _charToValue;

    public Base85Decoder()
    {
        _charToValue = _alphabet.Select((c, i) => new { c, i }).ToDictionary(x => x.c, x => x.i);
    }

    public string StripLeadingU(string serial)
    {
        if (serial.StartsWith("@U"))
            return serial.Substring(2);
        return serial;
    }

    public byte MirrorByte(byte b)
    {
        byte mirrored = 0;
        for (int i = 0; i < 8; i++)
        {
            if ((b & (1 << i)) != 0)
                mirrored |= (byte)(1 << (7 - i));
        }
        return mirrored;
    }

    public byte[] MirrorBytes(byte[] data)
    {
        return data.Select(MirrorByte).ToArray();
    }

    public byte[] Base85ToBytes(string base85Str)
    {
        base85Str = base85Str.Trim();

        // 计算填充
        int padding = 0;
        while (base85Str.Length % 5 != 0)
        {
            base85Str += '~';
            padding++;
        }

        List<byte> result = new List<byte>();

        for (int i = 0; i < base85Str.Length; i += 5)
        {
            string chunk = base85Str.Substring(i, 5);

            // 将Base85字符转换为数值
            long value = 0;
            foreach (char c in chunk)
            {
                value = value * 85 + _charToValue[c];
            }

            // 将32位数值转换为4个字节
            byte[] bytesChunk = new byte[4];
            for (int j = 3; j >= 0; j--)
            {
                bytesChunk[j] = (byte)(value & 0xFF);
                value >>= 8;
            }

            result.AddRange(bytesChunk);
        }

        // 去除填充的字节
        if (padding > 0)
            result.RemoveRange(result.Count - padding, padding);

        return result.ToArray();
    }

    public string BytesToBitStream(byte[] data)
    {
        return string.Concat(data.Select(b => Convert.ToString(b, 2).PadLeft(8, '0')));
    }

    public string DecodeToBitstream(string serial)
    {
        // 步骤1: 去除前导@U
        string stripped = StripLeadingU(serial);

        // 步骤2: Base85转字节
        byte[] bytesData = Base85ToBytes(stripped);

        // 步骤3: 镜像字节
        byte[] mirrored = MirrorBytes(bytesData);

        // 步骤4: 生成比特流并返回
        return BytesToBitStream(mirrored);
    }

    // 可选：带调试信息的版本
    public string DecodeToBitstreamWithDebug(string serial)
    {
        // 步骤1: 去除前导@U
        string stripped = StripLeadingU(serial);
        Console.WriteLine($"1. 去除前导@U: {stripped}");

        // 步骤2: Base85转字节
        byte[] bytesData = Base85ToBytes(stripped);
        string hexStr = BitConverter.ToString(bytesData).Replace("-", "").ToLower();
        Console.WriteLine($"2. Base85转字节: {hexStr}");

        // 步骤3: 镜像字节
        byte[] mirrored = MirrorBytes(bytesData);
        string mirroredHex = BitConverter.ToString(mirrored).Replace("-", "").ToLower();
        Console.WriteLine($"3. 镜像字节: {mirroredHex}");

        // 步骤4: 生成比特流
        string bitstream = BytesToBitStream(mirrored);
        Console.WriteLine($"4. 比特流: {bitstream}");

        return bitstream;
    }
}