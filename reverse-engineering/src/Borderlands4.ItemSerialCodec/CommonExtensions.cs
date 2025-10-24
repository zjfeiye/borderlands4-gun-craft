namespace Borderlands4.ItemSerialCodec;

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
}
