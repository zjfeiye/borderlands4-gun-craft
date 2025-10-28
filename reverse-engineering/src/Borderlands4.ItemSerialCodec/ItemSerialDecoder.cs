using Borderlands4.ItemSerialCodec.Extensions;
using Borderlands4.ItemSerialCodec.Tokens;
using System.Text;

namespace Borderlands4.ItemSerialCodec;

public class ItemSerialDecoder
{
    private readonly Base85 _base85 = new();
    private readonly List<Token> _tokens = [];

    public string DecodeAsPartsString(string serial, bool debug = false)
    {
        var results = Decode(serial, debug);

        return FormatResults(results);
    }

    public Token[] Decode(string serial, bool debug = false)
    {
        var bitStream = _base85.DecodeSerial(serial, debug);
        return Decode(bitStream, debug);
    }

    public Token[] Decode(byte[] bitStream, bool debug = false)
    {
        _tokens.Clear();

        var reader = new BitStreamReader(bitStream);

        try
        {
            // 读取起始标志 (5 bits)
            var startMarker = reader.ReadBits(5);
            if (startMarker != CONSTS.ITEM_DATA_HEADER_MARKER) // 00100 = 4
            {
                throw new InvalidOperationException($"error: start marker is not 00100, but {Convert.ToString(startMarker, 2).PadLeft(5, '0')}.");
            }

            if (debug) Console.WriteLine($"读取起始标志: {Convert.ToString(startMarker, 2).PadLeft(5, '0')}");

            // 读取起始标志后 00
            var token = ReadNextToken(reader);
            if (token != 0b00)
            {
                throw new InvalidOperationException($"error: expected segment separator 00, but got {Convert.ToString(token, 2).PadLeft(2, '0')}.");
            }

            _tokens.Add(new SegmentSeparatorToken());

            if (debug) Console.WriteLine($"片段分割符: {Convert.ToString(token, 2).PadLeft(2, '0')}");

            // 解析数据片段
            while (reader.RemainingBits > 0)
            {
                // 检查剩余比特是否都是0
                if (reader.IsRemainingAllZeros())
                {
                    if (debug) Console.WriteLine("剩余比特全为0，解析结束");
                    break;
                }

                ReadNextSegment(reader, debug);
            }

            return _tokens.ToArray();
        }
        catch (Exception ex)
        {
            if (debug) Console.WriteLine($"解码过程中出错: {ex}");

            throw new InvalidOperationException("decoding error.", ex);
        }
    }

    private void ReadNextSegment(BitStreamReader reader, bool debug = false)
    {
        while (reader.RemainingBits > 0)
        {
            var token = ReadNextToken(reader);

            switch (token)
            {
                default:
                    throw new InvalidOperationException($"error: invalid token.");
                case CONSTS.TOKEN_SEGMENT_SEPARATOR:
                    // 片段分割符 00，结束本片段
                    {
                        _tokens.Add(new SegmentSeparatorToken());

                        if (debug) Console.WriteLine("片段分隔符: 00");

                        return;
                    }
                case CONSTS.TOKEN_INTRA_SEGMENT_SEPARATOR:
                    // 片段内分割符 01，读取下一个 token
                    {
                        _tokens.Add(new ValueSeparatorToken());

                        if (debug) Console.WriteLine("片段内分隔符: 01，继续读取");

                        continue;
                    }
                case CONSTS.TOKEN_VARINT16:
                    // Varint16 标记 100，读取数据
                    {
                        if (debug) Console.WriteLine($"检测到 Varint16 编码标记: {token}");

                        var value = reader.ReadVarint16();
                        //results.Add(value);
                        _tokens.Add(new NumberToken(value));

                        if (debug) Console.WriteLine($"读取 Varint16 值: {value}");
                        break;
                    }
                case CONSTS.TOKEN_VARBIT32:
                    // Varbit32 标记 110，读取数据
                    {
                        if (debug) Console.WriteLine($"检测到 Varbit32 编码标记: {token}");

                        var value = reader.ReadVarbit32();
                        //results.Add(value);
                        _tokens.Add(new NumberToken(value));

                        if (debug) Console.WriteLine($"读取 Varbit32 值: {value}");
                        break;
                    }
                case CONSTS.TOKEN_STRING:
                    // String 标记 111，读取数据
                    {
                        if (debug) Console.WriteLine($"检测到 String 编码标记: {token}");

                        var value = reader.ReadString();
                        //results.Add(value);
                        _tokens.Add(new StringToken(value));

                        if (debug) Console.WriteLine($"读取 String 值: {value}");
                        break;
                    }
                case CONSTS.TOKEN_PART:
                    // Part 标记 101，读取数据
                    {
                        ReadNextPart(reader, debug);
                        break;
                    }
            }
        }

        return;
    }

    private static uint ReadNextToken(BitStreamReader reader)
    {
        if (reader.RemainingBits < 2)
        {
            throw new InvalidOperationException($"error: not enough bits to read data token.");
        }

        var token = reader.ReadBits(2);

        if (token == CONSTS.TOKEN_SEGMENT_SEPARATOR || token == CONSTS.TOKEN_INTRA_SEGMENT_SEPARATOR)
        {
            return token;
        }

        //既不是 00 也不是 01，需要再多读取 1 个比特
        if (reader.RemainingBits > 0)
        {
            var nextBits = reader.ReadBits(1);
            token = token << 1 | nextBits;

            if (token == CONSTS.TOKEN_VARINT16
                || token == CONSTS.TOKEN_VARBIT32
                || token == CONSTS.TOKEN_PART
                || token == CONSTS.TOKEN_STRING)
            {
                return token;
            }

            throw new InvalidOperationException($"error: failed to read valid data token, got {Convert.ToString(token, 2).PadLeft(3, '0')} at position {reader.Position - 3}.");
        }
        else
        {
            throw new InvalidOperationException($"error: not enough bits to read data token.");
        }
    }

    private void ReadNextPart(BitStreamReader reader, bool debug = false)
    {
        // 读取配件类型值
        var partType = reader.ReadVarint16();

        if (debug) Console.WriteLine($"配件类型: {partType}");

        // 读取下一个比特决定配件格式
        var formatBit = reader.ReadBits(1);
        if (formatBit == CONSTS.TOKEN_PART_COMPLEX_FORMAT_FLAG) // 1
        {
            // 复合值配件
            if (debug) Console.WriteLine("配件格式: 复合值");

            var objValue = reader.ReadVarint16(); //这个值总是以 Varint16 编码

            _tokens.Add(new CompositeValueToken(partType, objValue));

            if (debug) Console.WriteLine($"复合值: {{{partType}:{objValue}}}");

            var endMarker = reader.ReadBits(3);
            if (endMarker != CONSTS.TOKEN_PART_COMPLEX_VALUE_END_MARKER) // 000 = 0
            {
                throw new InvalidOperationException($"error: expected data end marker 000, but got {Convert.ToString(endMarker, 2).PadLeft(3, '0')}.");
            }
        }
        else // 0
        {
            // 读取更多比特来确定格式
            var nextBits = reader.ReadBits(2);
            var combinedBits = formatBit << 2 | nextBits;

            if (combinedBits == CONSTS.TOKEN_PART_END_MARKER) // 010 = 2
            {
                // 简单值配件
                if (debug) Console.WriteLine("配件格式: 简单值");

                _tokens.Add(new SimpleValueToken(partType));

                if (debug) Console.WriteLine($"简单值: {{{partType}}}");
            }
            else if (combinedBits == CONSTS.TOKEN_PART_ARRAY_VALUE_FLAG) // 001 = 1
            {
                // 可能是数组开始
                var arrayStart = reader.ReadBits(2);
                if (arrayStart == CONSTS.TOKEN_PART_ARRAY_VALUE_START_MARKER) // 01 = 1
                {
                    if (debug) Console.WriteLine("配件格式: 数组值");

                    var arrayValues = new List<object>();

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
                        else if (nextMarker == CONSTS.TOKEN_STRING) // 111 = 7
                        {
                            reader.SkipBits(3);

                            var value = reader.ReadString();
                            arrayValues.Add(value);

                            if (debug) Console.WriteLine($"数组元素(String): {value}");

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

                    _tokens.Add(new ArrayValueToken(partType, [.. arrayValues]));

                    // 数组结束
                    if (reader.RemainingBits >= 2)
                    {
                        var endMarker = reader.ReadBits(2);
                        if (endMarker != CONSTS.TOKEN_PART_ARRAY_VALUE_END_MARKER) // 00 = 0
                        {
                            throw new InvalidOperationException($"error: expected array format end marker 00, but got {Convert.ToString(endMarker, 2).PadLeft(2, '0')}.");
                        }
                    }
                    else
                    {
                        //TODO: 长度不足，非法
                    }

                    if (debug) Console.WriteLine($"数组配件: {{{partType}:[{string.Join(" ", arrayValues)}]}}");
                }
                else
                {
                    throw new InvalidOperationException($"unknown array start marker: {Convert.ToString(arrayStart, 2).PadLeft(2, '0')}");
                }
            }
            else
            {
                throw new InvalidOperationException($"unknown part format: {Convert.ToString(combinedBits, 2).PadLeft(3, '0')}");
            }
        }
    }

    // 格式化输出结果
    public static string FormatResults(IEnumerable<Token> tokens)
    {
        var sb = new StringBuilder();

        foreach (var token in tokens)
        {
            if (token.Type != TokenType.SegmentSeparator && token.Type != TokenType.ValueSeparator)
            {
                sb.Append($" {token}");
            }
            else
            {
                sb.Append(token.ToString());
            }
        }

        return sb.ToString().TrimStart('|', ' ');
    }
}