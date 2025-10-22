namespace Deserializer; 

public class BitStreamReader
{
    private int _bitPosition;
    private readonly byte[] _data;

    public BitStreamReader(byte[] data)
    {
        _data = data;
        _bitPosition = 0;
    }

    public BitStreamReader(string bitString)
    {
        _data = BitStringToBytes(bitString);
        _bitPosition = 0;
    }

    public int Position => _bitPosition;
    public int Length => _data.Length * 8;
    public int RemainingBits => Length - _bitPosition;

    public uint ReadBits(int numBits)
    {
        if (_bitPosition + numBits > Length)
            throw new ArgumentException($"无法读取 {numBits} 比特，位置 {_bitPosition}，总长度 {Length}");

        uint result = 0;

        for (int i = 0; i < numBits; i++)
        {
            int byteIndex = _bitPosition / 8;
            int bitIndex = 7 - (_bitPosition % 8); // 高位在前

            uint bit = (uint)((_data[byteIndex] >> bitIndex) & 1);
            result = (result << 1) | bit;
            _bitPosition++;
        }

        return result;
    }

    public uint PeekBits(int numBits)
    {
        int savedPosition = _bitPosition;
        uint result = ReadBits(numBits);
        _bitPosition = savedPosition;
        return result;
    }

    public void SkipBits(int numBits)
    {
        if (_bitPosition + numBits > Length)
            throw new ArgumentException($"无法跳过 {numBits} 比特，位置 {_bitPosition}，总长度 {Length}");

        _bitPosition += numBits;
    }

    public bool IsRemainingAllZeros()
    {
        if (RemainingBits <= 0) return true;

        // 检查剩余的所有比特是否都是0
        for (int i = _bitPosition; i < Length; i++)
        {
            int byteIndex = i / 8;
            int bitIndex = 7 - (i % 8);

            if (((_data[byteIndex] >> bitIndex) & 1) != 0)
                return false;
        }

        return true;
    }

    public uint ReadVarint16()
    {
        uint value = 0;
        int shift = 0;

        while (true)
        {
            uint chunk = ReadBits(5);
            bool continueFlag = (chunk & 0x10) != 0; // 最高位是延续标记
            uint data = chunk & 0x0F; // 低4位是数据

            value |= data << shift;
            shift += 4;

            if (!continueFlag || shift >= 16)
                break;
        }

        return value;
    }

    public uint ReadVarbit32()
    {
        // 读取5比特的长度前缀
        uint length = ReadBits(5);

        if (length == 0)
            return 0;

        // 读取指定长度的payload
        uint payload = ReadBits((int)length);

        return payload;
    }

    private static byte[] BitStringToBytes(string bitString)
    {
        int numBytes = (bitString.Length + 7) / 8;
        byte[] bytes = new byte[numBytes];

        for (int i = 0; i < bitString.Length; i++)
        {
            if (bitString[i] == '1')
            {
                int byteIndex = i / 8;
                int bitIndex = 7 - (i % 8); // 高位在前
                bytes[byteIndex] |= (byte)(1 << bitIndex);
            }
        }

        return bytes;
    }

    public static byte[] BitStringToBytes(string bitString, int expectedLength)
    {
        // 确保比特字符串长度正确，不足则填充0
        if (bitString.Length < expectedLength)
        {
            bitString = bitString.PadRight(expectedLength, '0');
        }
        else if (bitString.Length > expectedLength)
        {
            bitString = bitString.Substring(0, expectedLength);
        }

        return BitStringToBytes(bitString);
    }
}