using Borderlands4.ItemSerialCodec.Parts;

namespace Borderlands4.ItemSerialCodec;

public class ItemSerialDecoder
{
    private readonly List<List<object>> _results = [];
    private List<object> _currentSegment = [];

    private readonly Base85 _base85 = new();

    public List<List<object>> Decode(string serial, bool debug = false)
    {
        var bitStream = _base85.DecodeSerial(serial, debug);
        return Decode(bitStream, debug);
    }

    public List<List<object>> Decode(byte[] bitStream, bool debug = false)
    {
        var reader = new BitStreamReader(bitStream);
        return Decode(reader, debug);
    }

    public List<List<object>> Decode(BitStreamReader reader, bool debug = false)
    {
        _results.Clear();
        _currentSegment = [];

        try
        {
            // 步骤1: 读取起始标志 (5 bits)
            var startMarker = reader.ReadBits(5);
            if (startMarker != CONSTS.ITEM_DATA_HEADER_MARKER) // 00100 = 4
            {
                throw new InvalidOperationException($"error: start marker is not 00100, but {Convert.ToString(startMarker, 2).PadLeft(5, '0')}.");
            }

            if (debug) Console.WriteLine($"读取起始标志: {Convert.ToString(startMarker, 2).PadLeft(5, '0')}");

            // 读取起始标志后的 00
            var initialSeparator = reader.ReadBits(2);
            if (initialSeparator != CONSTS.TOKEN_SEGMENT_START_MARKER)
            {
                throw new InvalidOperationException($"error: expected 00 after start marker, but got {Convert.ToString(initialSeparator, 2).PadLeft(2, '0')}.");
            }

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
                bool seggmentParsed = ParseSegment(reader, debug);

                // 将当前片段添加到结果中
                _results.Add([.. _currentSegment]);
                _currentSegment.Clear();

                // 如果不是最后一个片段，应该有片段分隔符00
                if (seggmentParsed && reader.RemainingBits >= 2 && !reader.IsRemainingAllZeros())
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

    private bool ParseSegment(BitStreamReader reader, bool debug = false)
    {
        var hasData = false;

        while (reader.RemainingBits >= 3)
        {
            // 窥视接下来的3个比特
            var nextBits = reader.PeekBits(3);

            if (nextBits == CONSTS.TOKEN_VARINT16 || nextBits == CONSTS.TOKEN_VARBIT32) // 100 = 4, 110 = 6
            {
                // varint16
                reader.SkipBits(3);

                if (debug) Console.WriteLine($"检测到 {(nextBits == CONSTS.TOKEN_VARINT16 ? "Varint16" : "Varbit32")} 编码标记: {nextBits}");

                var value = nextBits == CONSTS.TOKEN_VARINT16 ? reader.ReadVarint16() : reader.ReadVarbit32();
                _currentSegment.Add(value);

                if (debug) Console.WriteLine($"读取 {(nextBits == CONSTS.TOKEN_VARINT16 ? "Varint16" : "Varbit32")} 值: {value}");

                hasData = true;

                // 读取分隔符
                if (reader.RemainingBits >= 2)
                {
                    var separator = reader.ReadBits(2);
                    if (separator == CONSTS.TOKEN_INTRA_SEGMENT_SEPARATOR) // 01 = 1
                    {
                        if (debug) Console.WriteLine("片段内分隔符: 01，继续读取");
                        continue;
                    }
                    else if (separator == CONSTS.TOKEN_SEGMENT_END_MARKER) // 00 = 0
                    {
                        if (debug) Console.WriteLine("片段结束标记: 00");
                        return true;
                    }
                    else
                    {
                        throw new InvalidOperationException($"unknown separator: {Convert.ToString(separator, 2).PadLeft(2, '0')}.");
                    }
                }
                return true;
            }
            else if (nextBits == CONSTS.TOKEN_PART_START_MARKER) // 101 = 5
            {
                // 配件数据
                reader.SkipBits(3);

                if (debug) Console.WriteLine($"检测到配件数据标记: {nextBits}");

                ParseParts(reader, debug);
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
            else if (nextBits == 0x07) // 111 = 7
            {
                // 后续是皮肤或DLC数据，不做处理，忽略后续数据
                reader.SkipBits(reader.RemainingBits);
                if (debug) Console.WriteLine($"检测到意外标记：{Convert.ToString(nextBits, 2).PadLeft(3, '0')}，片段解析结束");
                return true;
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
                reader.SkipBits(reader.RemainingBits);
                if (debug) Console.WriteLine($"无有效标记: {Convert.ToString(nextBits, 2).PadLeft(3, '0')}，片段解析结束");
                return true;
            }
        }

        return hasData;
    }

    private void ParseParts(BitStreamReader reader, bool debug = false)
    {
        // 读取配件类型值
        var partType = reader.ReadVarint16();

        if (debug) Console.WriteLine($"配件类型: {partType}");

        // 读取下一个比特决定配件格式
        uint formatBit = reader.ReadBits(1);

        if (formatBit == CONSTS.TOKEN_PART_COMPLEX_FORMAT_FLAG) // 1
        {
            // 对象值配件
            if (debug) Console.WriteLine("配件格式: 复合值");

            var objValue = reader.ReadVarint16(); //这个值总是以 Varint16 编码

            var endMarker = reader.ReadBits(3);
            if (endMarker != CONSTS.TOKEN_PART_COMPLEX_VALUE_END_MARKER) // 000 = 0
            {
                throw new InvalidOperationException($"error: expected data end marker 000, but got {Convert.ToString(endMarker, 2).PadLeft(3, '0')}.");
            }

            _currentSegment.Add(new ComplexValue
            {
                Type = partType,
                Value = objValue
            });

            if (debug) Console.WriteLine($"复合值: {{{partType}:{objValue}}}");
        }
        else
        {
            // 读取更多比特来确定格式
            var nextBits = reader.ReadBits(2);
            var combinedBits = formatBit << 2 | nextBits;

            if (combinedBits == CONSTS.TOKEN_PART_END_MARKER) // 010 = 2
            {
                // 单个值配件
                if (debug) Console.WriteLine("配件格式: 简单值");

                _currentSegment.Add(new SimpleValue
                {
                    Type = partType
                });

                if (debug) Console.WriteLine($"简单值: {{{partType}}}");
            }
            else if (combinedBits == CONSTS.TOKEN_PART_ARRAY_VALUE_FLAG) // 001 = 1
            {
                // 可能是数组开始
                uint arrayStart = reader.ReadBits(2);
                if (arrayStart == CONSTS.TOKEN_PART_ARRAY_VALUE_START_MARKER) // 01 = 1
                {
                    if (debug) Console.WriteLine("配件格式: 数组值");

                    var arrayValues = new List<uint>();

                    // 解析数组元素
                    while (true)
                    {
                        // 检查标记
                        if (reader.RemainingBits < 3)
                        {
                            break;
                        }

                        var nextMarker = reader.PeekBits(3);
                        if (nextMarker == CONSTS.TOKEN_VARINT16 || nextMarker == CONSTS.TOKEN_VARBIT32) // 100 = 4, 110 = 6
                        {
                            reader.SkipBits(3);

                            var value = nextMarker == CONSTS.TOKEN_VARINT16 ? reader.ReadVarint16() : reader.ReadVarbit32();
                            arrayValues.Add(value);

                            if (debug) Console.WriteLine($"数组元素({(nextMarker == CONSTS.TOKEN_VARINT16 ? "Varint16" : "Varbit32")}): {value}");

                            // 检查结束符
                            if (reader.RemainingBits >= 2)
                            {
                                var sep = reader.PeekBits(2);
                                if (sep == CONSTS.TOKEN_PART_ARRAY_VALUE_END_MARKER) // 00 = 0
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
                        var endMarker = reader.ReadBits(2);
                        if (endMarker != CONSTS.TOKEN_PART_ARRAY_VALUE_END_MARKER) // 00 = 0
                        {
                            throw new InvalidOperationException($"error: expected array format end marker 00, but got {Convert.ToString(endMarker, 2).PadLeft(2, '0')}.");
                        }
                    }

                    _currentSegment.Add(new ArrayValue
                    {
                        Type = partType,
                        Values = [.. arrayValues]
                    });

                    if (debug) Console.WriteLine($"数组配件: {{{partType}:[{string.Join(" ", arrayValues)}]}}");
                }
                else
                {
                    throw new InvalidOperationException($"unknown array start marker:{Convert.ToString(arrayStart, 2).PadLeft(2, '0')}");
                }
            }
            else
            {
                throw new InvalidOperationException($"unknown part format:{Convert.ToString(combinedBits, 2).PadLeft(3, '0')}");
            }
        }
    }

    public string DecodeAsString(string serial, bool debug = false)
    {
        var results = Decode(serial, debug);

        return FormatResults(results);
    }

    // 格式化输出结果
    public static string FormatResults(List<List<object>> results)
    {
        var formattedParts = new List<string>();

        foreach (var segment in results)
        {
            var segmentParts = new List<string>();

            foreach (var item in segment)
            {
                if (item is uint number)
                {
                    segmentParts.Add(number.ToString());
                }
                else if (item is IPartValue val)
                {
                    segmentParts.Add(val.ToString()!);
                }
            }

            formattedParts.Add(string.Join(", ", segmentParts));
        }

        return string.Join("| ", formattedParts).Trim().Replace("| |", "||").Replace("},", "}").TrimEnd('|') + "|";
    }
}