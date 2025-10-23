namespace Borderlands4.ItemSerialCodec;

public class BitStreamWriter
{
    private readonly List<byte> _data;
    private int _currentByte;
    private int _bitPosition;

    private BitStreamWriter? _tempWriter = null;

    public BitStreamWriter()
    {
        _data = [];
        _currentByte = 0;
        _bitPosition = 0;
    }

    public void WriteBits(uint value, int numBits)
    {
        if (numBits < 0 || numBits > 32)
            throw new ArgumentException("numBits must be between 0 and 32");

        for (int i = numBits - 1; i >= 0; i--)
        {
            uint bit = value >> i & 1;
            _currentByte = _currentByte << 1 | (int)bit;
            _bitPosition++;

            if (_bitPosition == 8)
            {
                _data.Add((byte)_currentByte);
                _currentByte = 0;
                _bitPosition = 0;
            }
        }
    }

    public void WriteNumber(uint value)
    {
        if(_tempWriter is null)
        {
            _tempWriter = new BitStreamWriter();
        }

        var intLen = _tempWriter.WriteVarint16(value);
        var bitLen = _tempWriter.WriteVarbit32(value);

        if(intLen > bitLen)
        {
            WriteBits(0x06, 3); // 110 - varbit32 标记
            _ = WriteVarbit32(value);
        }
        else
        {
            WriteBits(0x04, 3); // 100 - varint16 标记
            _ = WriteVarint16(value);
        }
    }

    public int WriteVarint16(uint value)
    {
        if (value > 0xFFFF)
            throw new ArgumentException("varint16 can only encode values up to 65535");

        var chunks = new List<uint>();

        do
        {
            uint chunkValue = value & 0x0F; // 取低4位
            value >>= 4;

            bool hasMore = value > 0;
            uint chunk = chunkValue | (hasMore ? 0x10u : 0x00u); // 设置延续标记

            chunks.Add(chunk);
        }
        while (value > 0 && chunks.Count < 4); // 最多4个块

        // 写入块（注意：需要反转比特顺序）
        foreach (uint chunk in chunks)
        {
            WriteBits(ReverseBits(chunk, 5), 5);
        }

        return chunks.Count * 5;
    }

    public int WriteVarbit32(uint value)
    {
        if (value == 0)
        {
            // 长度为0
            WriteBits(ReverseBits(0, 5), 5);
            return 5;
        }

        // 计算需要的比特数
        int bitCount = 32 - LeadingZeroCount(value);

        // 写入长度前缀（反转）
        WriteBits(ReverseBits((uint)bitCount, 5), 5);

        // 写入payload（反转）
        WriteBits(ReverseBits(value, bitCount), bitCount);

        return 5 + bitCount;
    }

    public byte[] ToByteArray()
    {
        // 如果当前字节有未写入的比特，填充0并添加
        if (_bitPosition > 0)
        {
            _currentByte <<= 8 - _bitPosition; // 左对齐
            _data.Add((byte)_currentByte);
        }

        return _data.ToArray();
    }

    private uint ReverseBits(uint value, int bitCount)
    {
        uint result = 0;
        for (int i = 0; i < bitCount; i++)
        {
            result = result << 1 | value & 1;
            value >>= 1;
        }
        return result;
    }

    private int LeadingZeroCount(uint value)
    {
        if (value == 0) return 32;

        int count = 0;
        while ((value & 0x80000000) == 0)
        {
            count++;
            value <<= 1;
        }
        return count;
    }
}