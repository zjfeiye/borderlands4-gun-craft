namespace Borderlands4.ItemSerialCodec.Extensions;

public static class CommonExtensions
{
    public static byte[] MirrorBytes(this byte[] data)
    {
        return [.. data.Select(MirrorByte)];
    }

    public static byte MirrorByte(this byte b)
    {
        byte mirrored = 0;
        for (int i = 0; i < 8; i++)
        {
            if ((b & 1 << i) != 0)
                mirrored |= (byte)(1 << 7 - i);
        }
        return mirrored;
    }

    public static string ConvertToBitString(this byte[] data)
    {
        return string.Concat(data.Select(b => Convert.ToString(b, 2).PadLeft(8, '0')));
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
