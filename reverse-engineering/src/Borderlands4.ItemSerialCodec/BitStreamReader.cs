using System.Runtime.CompilerServices;

namespace Borderlands4.ItemSerialCodec;

public class BitStreamReader(byte[] data)
{
    private readonly byte[] _data = data;
    private int _bitPosition = 0;

    public int Position => _bitPosition;
    public int Length => _data.Length * 8;
    public int RemainingBits => Length - _bitPosition;

    public uint ReadBits(int numBits)
    {
        if (_bitPosition + numBits > Length)
        {
            throw new ArgumentException($"cannot read {numBits} bits at position {_bitPosition}, total length {Length}.");
        }

        var result = 0u;

        for (int i = 0; i < numBits; i++)
        {
            var byteIndex = _bitPosition / 8;
            var bitIndex = 7 - _bitPosition % 8; // 高位在前

            var bit = (uint)(_data[byteIndex] >> bitIndex & 1);
            result = result << 1 | bit;
            _bitPosition++;
        }

        return result;
    }

    public uint PeekBits(int numBits)
    {
        var savedPosition = _bitPosition;
        var result = ReadBits(numBits);
        _bitPosition = savedPosition;
        return result;
    }

    public void SkipBits(int numBits)
    {
        if (_bitPosition + numBits > Length)
        {
            throw new ArgumentException($"cannot skip {numBits} bits at position {_bitPosition}, total length {Length}.");
        }

        _bitPosition += numBits;
    }


    public void RewindBits(int numBits)
    {
        if (_bitPosition - numBits < 0)
        {
            throw new ArgumentException($"cannot rewind {numBits} bits at position {_bitPosition}.");
        }

        _bitPosition -= numBits;
    }

    public bool IsRemainingAllZeros()
    {
        if (RemainingBits <= 0)
        {
            return true;
        }

        // 检查剩余的所有比特是否都是0
        for (var i = _bitPosition; i < Length; i++)
        {
            var byteIndex = i / 8;
            var bitIndex = 7 - i % 8;

            if ((_data[byteIndex] >> bitIndex & 1) != 0)
            {
                return false;
            }
        }

        return true;
    }

    public uint ReadVarint16()
    {
        var value = 0u;
        var shift = 0;

        while (true)
        {
            var chunk = ReadBits(5);
            var reversedChunk = ReverseBits(chunk, 5); // 由于比特流已反转，我们需要反转这5个比特
            var continueFlag = (reversedChunk & 0x10) != 0; // 最高位是延续标记
            var data = reversedChunk & 0x0F; // 低4位是数据

            value |= data << shift;
            shift += 4;

            if (!continueFlag || shift >= 16)
            {
                break;
            }
        }

        return value;
    }

    public uint ReadVarbit32()
    {
        // 读取5比特的长度前缀
        var lengthBits = ReadBits(5);
        // 反转长度前缀
        var length = ReverseBits(lengthBits, 5);

        if (length == 0)
        {
            return 0;
        }

        // 读取指定长度的payload
        var payloadBits = ReadBits((int)length);

        // 反转payload
        var payload = ReverseBits(payloadBits, (int)length);

        return payload;
    }

    public string ReadString()
    {
        // 读取长度前缀（基于 varint16 编码）
        var length = ReadVarint16();

        if (length == 0)
        {
            return string.Empty;
        }

        var str = new char[length];
        for (var i = 0; i < length; i++)
        {
            // 每7个字节为1个字符
            var charBits = ReadBits(7);
            // 反转payload
            str[i] = (char)ReverseBits(charBits, 7);
        }

        return new string(str);
    }

    /// <summary>
    /// 反转指定数量的比特
    /// </summary>
    /// <param name="value"></param>
    /// <param name="bitCount"></param>
    /// <returns></returns>
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
}