using System.Runtime.CompilerServices;

namespace Borderlands4.ItemSerialCodec; 

public class BitStreamReader
{
    private int _bitPosition;
    private readonly byte[] _data;

    public BitStreamReader(byte[] data)
    {
        _data = data;
        _bitPosition = 0;
    }

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


    //private static byte[] BitStringToBytes(string bitString)
    //{
    //    int numBytes = (bitString.Length + 7) / 8;
    //    byte[] bytes = new byte[numBytes];

    //    for (int i = 0; i < bitString.Length; i++)
    //    {
    //        if (bitString[i] == '1')
    //        {
    //            int byteIndex = i / 8;
    //            int bitIndex = 7 - i % 8; // 高位在前
    //            bytes[byteIndex] |= (byte)(1 << bitIndex);
    //        }
    //    }

    //    return bytes;
    //}

    //public static byte[] BitStringToBytes(string bitString, int expectedLength)
    //{
    //    // 确保比特字符串长度正确，不足则填充0
    //    if (bitString.Length < expectedLength)
    //    {
    //        bitString = bitString.PadRight(expectedLength, '0');
    //    }
    //    else if (bitString.Length > expectedLength)
    //    {
    //        bitString = bitString.Substring(0, expectedLength);
    //    }

    //    return BitStringToBytes(bitString);
    //}
}