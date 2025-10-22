namespace Deserializer;
using System;
using System.Collections.Generic;

public class ItemSerialDecoder
{
    private readonly List<List<object>> _results;
    private List<object> _currentFragment;

    public ItemSerialDecoder()
    {
        _results = new List<List<object>>();
        _currentFragment = new List<object>();
    }

    public List<List<object>> DecodeBitstream(string bitstream, bool debug = false)
    {
        var reader = new BitStreamReader(bitstream);
        return DecodeBitstream(reader, debug);
    }

    public List<List<object>> DecodeBitstream(BitStreamReader reader, bool debug = false)
    {
        _results.Clear();
        _currentFragment = new List<object>();

        try
        {
            // 步骤1: 读取起始标志 (5 bits)
            uint startMarker = reader.ReadBits(5);
            if (startMarker != 0x04) // 00100 = 4
            {
                if (debug) Console.WriteLine($"警告: 起始标志不是00100，而是 {Convert.ToString(startMarker, 2).PadLeft(5, '0')}");
            }

            if (debug) Console.WriteLine($"读取起始标志: {Convert.ToString(startMarker, 2).PadLeft(5, '0')}");

            // 读取起始标志后的00
            uint initialSeparator = reader.ReadBits(2);
            if (initialSeparator != 0x00 && debug)
                Console.WriteLine($"警告: 期望起始标志后的00，得到 {Convert.ToString(initialSeparator, 2).PadLeft(2, '0')}");

            if (debug) Console.WriteLine($"起始标志后分隔符: {Convert.ToString(initialSeparator, 2).PadLeft(2, '0')}");

            // 开始解析片段
            while (reader.RemainingBits > 0)
            {
                // 检查剩余比特是否都是0
                if (reader.IsRemainingAllZeros())
                {
                    if (debug) Console.WriteLine("剩余比特全为0，解析结束");
                    break;
                }

                // 解析当前片段
                bool fragmentParsed = ParseFragment(reader, debug);

                // 将当前片段添加到结果中
                _results.Add([.. _currentFragment]);
                _currentFragment.Clear();

                // 如果不是最后一个片段，应该有片段分隔符00
                if (fragmentParsed && reader.RemainingBits >= 2 && !reader.IsRemainingAllZeros())
                {
                    uint separator = reader.PeekBits(2);
                    if (separator == 0x00) // 00 = 0
                    {
                        reader.SkipBits(2);
                        _results.Add([]);
                        if (debug) Console.WriteLine("片段分隔符: 00");
                    }
                }
            }

            return _results;
        }
        catch (Exception e)
        {
            if (debug) Console.WriteLine($"解码过程中出错: {e}");
            return _results;
        }
    }

    private bool ParseFragment(BitStreamReader reader, bool debug = false)
    {
        bool hasData = false;

        while (reader.RemainingBits >= 3)
        {
            // 窥视接下来的3个比特
            uint nextBits = reader.PeekBits(3);

            if (nextBits == 0x04) // 100 = 4
            {
                // varint16编码
                reader.SkipBits(3);
                if (debug) Console.WriteLine("检测到varint16编码标记: 100");
                uint value = ReadVarint16(reader);
                _currentFragment.Add(value);
                if (debug) Console.WriteLine($"读取varint16值: {value}");
                hasData = true;

                // 读取分隔符
                if (reader.RemainingBits >= 2)
                {
                    uint separator = reader.ReadBits(2);
                    if (separator == 0x01) // 01 = 1
                    {
                        if (debug) Console.WriteLine("片段内分隔符: 01，继续读取");
                        continue;
                    }
                    else if (separator == 0x00) // 00 = 0
                    {
                        if (debug) Console.WriteLine("片段结束标记: 00");
                        return true;
                    }
                    else
                    {
                        if (debug) Console.WriteLine($"未知分隔符: {Convert.ToString(separator, 2).PadLeft(2, '0')}");
                        return true;
                    }
                }
                return true;
            }
            else if (nextBits == 0x06) // 110 = 6
            {
                // varbit32编码
                reader.SkipBits(3);
                if (debug) Console.WriteLine("检测到varbit32编码标记: 110");
                uint value = ReadVarbit32(reader);
                _currentFragment.Add(value);
                if (debug) Console.WriteLine($"读取varbit32值: {value}");
                hasData = true;

                // 读取分隔符
                if (reader.RemainingBits >= 2)
                {
                    uint separator = reader.ReadBits(2);
                    if (separator == 0x01) // 01 = 1
                    {
                        if (debug) Console.WriteLine("片段内分隔符: 01，继续读取");
                        continue;
                    }
                    else if (separator == 0x00) // 00 = 0
                    {
                        if (debug) Console.WriteLine("片段结束标记: 00");
                        return true;
                    }
                    else
                    {
                        if (debug) Console.WriteLine($"未知分隔符: {Convert.ToString(separator, 2).PadLeft(2, '0')}");
                        return true;
                    }
                }
                return true;
            }
            else if (nextBits == 0x05) // 101 = 5
            {
                // 配件数据
                reader.SkipBits(3);
                if (debug) Console.WriteLine("检测到配件数据标记: 101");
                ParseAttachment(reader, debug);
                hasData = true;

                // 配件数据后可能有分隔符
                if (reader.RemainingBits >= 2)
                {
                    uint separator = reader.PeekBits(2);
                    if (separator == 0x01) // 01 = 1
                    {
                        reader.SkipBits(2);
                        if (debug) Console.WriteLine("配件后分隔符: 01，继续读取");
                        continue;
                    }
                    else if (separator == 0x00) // 00 = 0
                    {
                        if (debug) Console.WriteLine("配件后片段结束");
                        return true;
                    }
                }
                continue;
            }
            else if (nextBits == 0x00 && hasData) // 00 = 0
            {
                // 片段分隔符，结束当前片段
                if (debug) Console.WriteLine("检测到片段分隔符，结束当前片段");
                return true;
            }
            else
            {
                // 没有有效标记，可能结束
                if (debug) Console.WriteLine($"无有效标记: {Convert.ToString(nextBits, 2).PadLeft(3, '0')}，片段解析结束");
                return true;
            }
        }

        return hasData;
    }

    private void ParseAttachment(BitStreamReader reader, bool debug = false)
    {
        // 读取配件类型值
        uint attachmentType = ReadVarint16(reader);
        if (debug) Console.WriteLine($"配件类型: {attachmentType}");

        // 读取下一个比特决定配件格式
        uint formatBit = reader.ReadBits(1);

        if (formatBit == 0x01) // 1
        {
            // 对象值配件
            if (debug) Console.WriteLine("配件格式: 对象值");
            uint objValue = ReadVarint16(reader);
            uint endMarker = reader.ReadBits(3);
            if (endMarker != 0x00 && debug) // 000 = 0
                Console.WriteLine($"警告: 期望对象结束标记000，得到 {Convert.ToString(endMarker, 2).PadLeft(3, '0')}");

            string attachment = $"{{{attachmentType}:{objValue}}}";
            _currentFragment.Add(attachment);
            if (debug) Console.WriteLine($"对象配件: {attachment}");
        }
        else
        {
            // 读取更多比特来确定格式
            uint nextBits = reader.ReadBits(2);
            uint combinedBits = (formatBit << 2) | nextBits;

            if (combinedBits == 0x02) // 010 = 2
            {
                // 单个值配件
                if (debug) Console.WriteLine("配件格式: 单个值");
                string attachment = $"{{{attachmentType}}}";
                _currentFragment.Add(attachment);
                if (debug) Console.WriteLine($"单个值配件: {attachment}");
            }
            else if (combinedBits == 0x01) // 001 = 1
            {
                // 可能是数组开始
                uint arrayStart = reader.ReadBits(2);
                if (arrayStart == 0x01) // 01 = 1
                {
                    if (debug) Console.WriteLine("配件格式: 数组");
                    List<uint> arrayValues = new List<uint>();

                    // 解析数组元素
                    while (true)
                    {
                        // 检查标记
                        if (reader.RemainingBits < 3)
                            break;

                        uint nextMarker = reader.PeekBits(3);
                        if (nextMarker == 0x04) // 100 = 4
                        {
                            reader.SkipBits(3);
                            uint value = ReadVarint16(reader);
                            arrayValues.Add(value);
                            if (debug) Console.WriteLine($"数组元素(varint16): {value}");

                            // 检查分隔符
                            if (reader.RemainingBits >= 2)
                            {
                                uint sep = reader.PeekBits(2);
                                if (sep == 0x00) // 00 = 0
                                {
                                    break;
                                }
                            }
                            continue;
                        }
                        else if (nextMarker == 0x06) // 110 = 6
                        {
                            reader.SkipBits(3);
                            uint value = ReadVarbit32(reader);
                            arrayValues.Add(value);
                            if (debug) Console.WriteLine($"数组元素(varbit32): {value}");

                            // 检查分隔符
                            if (reader.RemainingBits >= 2)
                            {
                                uint sep = reader.PeekBits(2);
                                if (sep == 0x00) // 00 = 0
                                {
                                    break;
                                }
                            }
                            continue;
                        }
                        else
                        {
                            break;
                        }
                    }

                    // 数组结束
                    if (reader.RemainingBits >= 2)
                    {
                        uint endMarker = reader.ReadBits(2);
                        if (endMarker != 0x00 && debug) // 00 = 0
                            Console.WriteLine($"警告: 期望数组结束00，得到 {Convert.ToString(endMarker, 2).PadLeft(2, '0')}");
                    }

                    string arrayStr = string.Join(" ", arrayValues);
                    string attachment = $"{{{attachmentType}:[{arrayStr}]}}";
                    _currentFragment.Add(attachment);
                    if (debug) Console.WriteLine($"数组配件: {attachment}");
                }
                else if (debug)
                {
                    Console.WriteLine($"未知的数组开始标记: {Convert.ToString(arrayStart, 2).PadLeft(2, '0')}");
                }
            }
            else if (debug)
            {
                Console.WriteLine($"未知的配件格式: {Convert.ToString(combinedBits, 2).PadLeft(3, '0')}");
            }
        }
    }

    // 修正的varint16解码方法，考虑比特流已反转
    private uint ReadVarint16(BitStreamReader reader)
    {
        uint value = 0;
        int shift = 0;

        while (true)
        {
            uint chunk = reader.ReadBits(5);

            // 由于比特流已反转，我们需要反转这5个比特
            uint reversedChunk = ReverseBits(chunk, 5);

            bool continueFlag = (reversedChunk & 0x10) != 0; // 最高位是延续标记
            uint data = reversedChunk & 0x0F; // 低4位是数据

            value |= data << shift;
            shift += 4;

            if (!continueFlag || shift >= 16)
                break;
        }

        return value;
    }

    // 修正的varbit32解码方法，考虑比特流已反转
    private uint ReadVarbit32(BitStreamReader reader)
    {
        // 读取5比特的长度前缀
        uint lengthBits = reader.ReadBits(5);

        // 反转长度前缀
        uint length = ReverseBits(lengthBits, 5);

        if (length == 0)
            return 0;

        // 读取指定长度的payload
        uint payloadBits = reader.ReadBits((int)length);

        // 反转payload
        uint payload = ReverseBits(payloadBits, (int)length);

        return payload;
    }

    // 反转指定数量的比特
    private uint ReverseBits(uint value, int bitCount)
    {
        uint result = 0;
        for (int i = 0; i < bitCount; i++)
        {
            result = (result << 1) | (value & 1);
            value >>= 1;
        }
        return result;
    }

    // 格式化输出结果
    public string FormatResults(List<List<object>> results)
    {
        var formattedParts = new List<string>();

        foreach (var fragment in results)
        {
            var fragmentParts = new List<string>();

            foreach (var item in fragment)
            {
                if (item is uint number)
                {
                    fragmentParts.Add(number.ToString());
                }
                else if (item is string str)
                {
                    fragmentParts.Add(str);
                }
            }

            formattedParts.Add(string.Join(", ", fragmentParts));
        }

        return (string.Join("| ", formattedParts).Trim() + "|").Replace("| |", "||").Replace("},", "}");
    }
}