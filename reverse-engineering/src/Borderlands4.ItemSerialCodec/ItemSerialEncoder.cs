using Borderlands4.ItemSerialCodec.Extensions;
using Borderlands4.ItemSerialCodec.Tokens;
using System.Text.RegularExpressions;

namespace Borderlands4.ItemSerialCodec;

public partial class ItemSerialEncoder
{
    private readonly Base85 _base85 = new();

    public string EncodeToSerial(string partsStr)
    {
        // 解析格式化数据
        var tokens = ParsePartsString(partsStr);

        using var writer = new BitStreamWriter();

        // 生成比特流
        BuildBitStream(writer, tokens);

        // 获取字节数组
        var data = writer.ToByteArray();

        // 编码
        var serial = _base85.EncodeToSerial(data);

        return serial;
    }

    public static Token[] ParsePartsString(string partsStr)
    {
        var tokens = new List<Token>();

        var trimmedStr = "|" + WhiteSpaceRegex.Replace(partsStr, " ").Replace("\\", "").Trim().Trim('|') + "|";

        var buffer = new char[trimmedStr.Length];
        var count = 0;
        var chars = trimmedStr.ToCharArray();

        for (var i = 0; i < chars.Length; i++)
        {
            if (chars[i] == ' ')
            {
                continue;
            }
            else if (chars[i] == '|')
            {
                tokens.Add(new SegmentSeparatorToken());
            }
            else if (chars[i] == ',')
            {
                tokens.Add(new ValueSeparatorToken());
            }
            else if (chars[i] >= '0' && chars[i] <= '9')
            {
                buffer[count++] = chars[i];
                for (var j = i + 1; j < chars.Length; j++)
                {
                    if (chars[j] >= '0' && chars[j] <= '9')
                    {
                        buffer[count++] = chars[j];
                    }
                    else
                    {
                        i = j - 1;
                        break;
                    }
                }
                tokens.Add(new NumberToken(uint.Parse(new string(buffer, 0, count))));
                count = 0;
            }
            else if (chars[i] == '{')
            {
                buffer[count++] = chars[i];
                for (var j = i + 1; j < chars.Length; j++)
                {
                    if (chars[j] != '}')
                    {
                        buffer[count++] = chars[j];
                    }
                    else
                    {
                        i = j - 1;
                        break;
                    }
                }
                var content = new string(buffer, 1, count - 1);
                if (content.Contains(':') && content.Contains('['))
                {
                    // 数组格式 {type:[values]}
                    var value = uint.Parse(content[..content.IndexOf(':')].Trim());
                    var colonIndex1 = content.IndexOf('[');
                    var colonIndex2 = content.IndexOf(']');
                    var arrayContent = content.Substring(colonIndex1 + 1, colonIndex2 - colonIndex1 - 1);

                    var subValues = arrayContent.Split(' ', StringSplitOptions.RemoveEmptyEntries)
                        .Where(s => !string.IsNullOrWhiteSpace(s)).Select(s => s.Trim())
                        .Select(o => o.StartsWith('"') && o.EndsWith('"') ? o.Trim('"') : (object)uint.Parse(o))
                        .ToArray();
                    tokens.Add(new ArrayValueToken(value, subValues));
                }
                else if (content.Contains(':'))
                {
                    // 组合格式 {type:value}
                    var parts = content.Split(':', StringSplitOptions.RemoveEmptyEntries);
                    var value = uint.Parse(parts[0]);
                    var subValue = uint.Parse(parts[1]);
                    tokens.Add(new CompositeValueToken(value, subValue));
                }
                else
                {
                    // 简单格式 {type}
                    var value = uint.Parse(content);
                    tokens.Add(new SimpleValueToken(value));
                }
                count = 0;
            }
            else if (chars[i] == '"')
            {
                buffer[count++] = chars[i];
                for (var j = i + 1; j < chars.Length; j++)
                {
                    if (chars[j] != '"')
                    {
                        buffer[count++] = chars[j];
                    }
                    else
                    {
                        i = j;
                        break;
                    }
                }
                tokens.Add(new StringToken(new string(buffer, 1, count - 1)));
                count = 0;
            }
        }

        return [.. tokens];
    }

    private static void BuildBitStream(BitStreamWriter writer, Token[] tokens)
    {
        // 起始标志
        writer.WriteBits(CONSTS.ITEM_DATA_HEADER_MARKER, 5); // 00100

        //// 起始标志后的分隔符
        //writer.WriteBits(CONSTS.TOKEN_SEGMENT_START_MARKER, 2); // 00

        // 编码每个片段

        foreach (var token in tokens)
        {
            if (token is SegmentSeparatorToken segmentSeparatorToken)
            {
                writer.WriteBits(CONSTS.TOKEN_SEGMENT_SEPARATOR, 2);
            }
            else if (token is ValueSeparatorToken valueSeparatorToken)
            {
                writer.WriteBits(CONSTS.TOKEN_INTRA_SEGMENT_SEPARATOR, 2);
            }
            else if (token is NumberToken numberToken)
            {
                // 普通数值 - 通常使用 varint16 编码（最大值为0xFFFF）
                writer.WriteCompactNumber(numberToken.Value); //采用自动检测最终比特长度，使用更短的方式写入
            }
            else if (token is StringToken stringToken)
            {
                // 字符串
                writer.WriteBits(CONSTS.TOKEN_STRING, 3); // 111 - 字符串标记
                writer.WriteString(stringToken.Value);
            }
            else if (token is SimpleValueToken simpleValue)
            {
                // 简单格式
                writer.WriteBits(CONSTS.TOKEN_PART, 3); // 101 - 配件标记
                writer.WriteVarint16(simpleValue.Value);
                writer.WriteBits(CONSTS.TOKEN_PART_END_MARKER, 3); // 010 - 单个值格式
            }
            else if (token is CompositeValueToken compositeValue)
            {
                // 复合格式
                writer.WriteBits(CONSTS.TOKEN_PART, 3); // 101 - 配件标记
                writer.WriteVarint16(compositeValue.Value);
                writer.WriteBits(CONSTS.TOKEN_PART_COMPLEX_FORMAT_FLAG, 1); // 1 - 对象格式标记
                writer.WriteVarint16(compositeValue.SubValue);
                writer.WriteBits(CONSTS.TOKEN_PART_COMPLEX_VALUE_END_MARKER, 3); // 000 - 对象结束标记
            }
            else if (token is ArrayValueToken arrayValue)
            {
                // 数组格式
                writer.WriteBits(CONSTS.TOKEN_PART, 3); // 101 - 配件标记
                writer.WriteVarint16(arrayValue.Value);
                writer.WriteBits(CONSTS.TOKEN_PART_ARRAY_VALUE_FLAG, 3); // 001 - 数组类型标记
                writer.WriteBits(CONSTS.TOKEN_PART_ARRAY_VALUE_START_MARKER, 2); // 01 - 数组开始

                // 编码数组元素
                foreach (var subValue in arrayValue.SubValues)
                {
                    if (subValue is uint number)
                    {
                        writer.WriteCompactNumber(number);
                    }
                    else if (subValue is string str)
                    {
                        writer.WriteBits(CONSTS.TOKEN_STRING, 3); // 111 - 字符串标记
                        writer.WriteString(str);
                    }
                }

                writer.WriteBits(CONSTS.TOKEN_PART_ARRAY_VALUE_END_MARKER, 2); // 00 - 数组结束标记
            }
        }
    }

    [GeneratedRegex(@"\s+", RegexOptions.Compiled)]
    private static partial Regex WhiteSpaceRegex { get; }
}



