using System.Runtime.CompilerServices;

namespace Borderlands4.ItemSerialCodec;

public class BitStreamWriter
{
    private readonly List<byte> _data;
    private int _currentByte;
    private int _bitPosition;

    public BitStreamWriter()
    {
        _data = [];
        _currentByte = 0;
        _bitPosition = 0;
    }

    public int Position => _bitPosition;

    public void WriteBits(uint value, int numBits)
    {
        if (numBits < 0 || numBits > 32)
        {
            throw new ArgumentException("numBits must be between 0 and 32.");
        }

        for (var i = numBits - 1; i >= 0; i--)
        {
            var bit = value >> i & 1;
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

    public void WriteCompactNumber(uint value)
    {
        var varint16Len = CalculateVarint16Bits(value);
        var varbit32Len = CalculateVarbit32Bits(value);

        if (varint16Len > varbit32Len)
        {
            WriteBits(0x06, 3); // 110 - varbit32 标记
            WriteVarbit32(value);
        }
        else
        {
            WriteBits(0x04, 3); // 100 - varint16 标记
            WriteVarint16(value);
        }
    }

    public void WriteVarint16(uint value)
    {
        if (value > 0xFFFF)
        {
            throw new ArgumentException("varint16 can only encode values up to 65535");
        }

        var chunks = new List<uint>();

        do
        {
            var chunkValue = value & 0x0F; // 取低4位
            value >>= 4;

            var hasMore = value > 0;
            var chunk = chunkValue | (hasMore ? 0x10u : 0x00u); // 设置延续标记

            chunks.Add(chunk);
        }
        while (value > 0 && chunks.Count < 4); // 最多4个块

        // 写入块（注意：需要反转比特顺序）
        foreach (var chunk in chunks)
        {
            WriteBits(ReverseBits(chunk, 5), 5);
        }
    }

    public void WriteVarbit32(uint value)
    {
        if (value == 0)
        {
            // 长度为0
            WriteBits(ReverseBits(0, 5), 5);
            return;
        }

        // 计算需要的比特数
        int bitCount = 32 - LeadingZeroCount(value);

        // 写入长度前缀（反转）
        WriteBits(ReverseBits((uint)bitCount, 5), 5);

        // 写入payload（反转）
        WriteBits(ReverseBits(value, bitCount), bitCount);
    }

    public byte[] ToByteArray()
    {
        // 如果当前字节有未写入的比特，填充0并添加
        if (_bitPosition > 0)
        {
            _currentByte <<= 8 - _bitPosition; // 左对齐
            _data.Add((byte)_currentByte);
        }

        return [.. _data];
    }

    [MethodImpl(MethodImplOptions.AggressiveInlining)]
    private static int CalculateVarint16Bits(uint value)
    {
        // Varint16：每5比特一个块，其中最高位表示是否继续，所以每个块有4个数据比特。
        // 对于16位整数，最多需要4个块（因为4*4=16比特）
        if (value == 0)
        {
            return 5; // 一个块（5比特）
        }
        var bitsNeeded = 0;
        while (value > 0)
        {
            bitsNeeded += 5;
            value >>= 4;
        }

        return bitsNeeded;
    }

    [MethodImpl(MethodImplOptions.AggressiveInlining)]
    private static int CalculateVarbit32Bits(uint value)
    {
        // Varbit32：5比特的长度前缀 + 实际数据比特数
        if (value == 0)
        {
            return 5; // 长度前缀为0，然后没有数据比特？但我们的ReadVarbit32中，长度为0则返回0，所以这里应该是5比特（只是长度前缀）
        }

        // 计算表示该值所需的比特数
        var dataBits = 32 - LeadingZeroCount(value);

        return 5 + dataBits;
    }

    [MethodImpl(MethodImplOptions.AggressiveInlining)]
    private static uint ReverseBits(uint value, int bitCount)
    {
        var result = 0u;
        for (var i = 0; i < bitCount; i++)
        {
            result = result << 1 | value & 1;
            value >>= 1;
        }
        return result;
    }

    [MethodImpl(MethodImplOptions.AggressiveInlining)]
    private static int LeadingZeroCount(uint value)
    {
        if (value == 0)
        {
            return 32;
        }

        var count = 0;
        while ((value & 0x80000000) == 0)
        {
            count++;
            value <<= 1;
        }

        return count;
    }
}