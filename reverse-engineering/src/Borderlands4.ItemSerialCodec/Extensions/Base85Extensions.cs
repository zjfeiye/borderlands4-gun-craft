namespace Borderlands4.ItemSerialCodec.Extensions;

public static class Base85Extensions
{
    private const string SERIAL_PREFIX = "@U";

    public static string StripLeadingU(this string serial)
    {
        if (serial.StartsWith(SERIAL_PREFIX))
        {
            return serial[2..];
        }
        return serial;
    }

    public static byte[] DecodeSerial(this Base85 decoder, string serial, bool debug = false)
    {
        if (!serial.StartsWith(SERIAL_PREFIX))
        {
            throw new InvalidOperationException($"item serial must have the prefix '{SERIAL_PREFIX}'");
        }

        // 步骤1: 去除前导 '@U'
        var base85Str = serial.StripLeadingU();
        if (debug) Console.WriteLine($"1. 去除前导 '{SERIAL_PREFIX}': {base85Str}");

        // 步骤2: BASE85 解码
        var bitStream = decoder.Decode(base85Str);
        if (debug) Console.WriteLine($"2. BASE85 解码: {Convert.ToHexStringLower(bitStream)}");

        // 步骤3: 镜像字节
        var mirroredData = bitStream.MirrorBytes();
        if (debug) Console.WriteLine($"3. 镜像字节: {Convert.ToHexStringLower(mirroredData)}");

        // 步骤4: 生成比特流
        if (debug) Console.WriteLine($"4. 比特流: {mirroredData.ConvertToBitString()}");

        return mirroredData;
    }

    public static string EncodeToSerial(this Base85 encoder, byte[] data, bool debug = false)
    {
        if (data.Length == 0)
        {
            return SERIAL_PREFIX;
        }

        // 步骤1：镜像字节
        var mirroredData = data.MirrorBytes();
        if (debug) Console.WriteLine($"1. 镜像字节: {Convert.ToHexStringLower(mirroredData)}");

        // 步骤1：BASE85 编码
        string base85Str = encoder.Encode(mirroredData);
        if (debug) Console.WriteLine($"2. BASE85 编码: {base85Str}");

        // 添加前导 '@U'
        return SERIAL_PREFIX + base85Str;
    }
}
