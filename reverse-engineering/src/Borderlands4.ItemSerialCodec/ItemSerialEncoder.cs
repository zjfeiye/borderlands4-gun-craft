using Borderlands4.ItemSerialCodec.Extensions;
using Borderlands4.ItemSerialCodec.Parts;

namespace Borderlands4.ItemSerialCodec;

public class ItemSerialEncoder
{
    private readonly Base85 _base85 = new();

    public string EncodeToSerial(string partsStr)
    {
        // 解析格式化数据
        var segments = ParsePartsString(partsStr);

        var writer = new BitStreamWriter();

        // 生成比特流
        BuildBitStream(writer, segments);

        // 获取字节数组
        var data = writer.ToByteArray();

        // 编码
        var serial = _base85.EncodeToSerial(data);

        return serial;
    }

    public List<List<object>> ParsePartsString(string partsStr)
    {
        var segments = new List<List<object>>();

        // 分割片段
        var segmentStrings = partsStr.Trim().Trim('|', ' ').Split('|').ToArray();

        foreach (string segmentString in segmentStrings)
        {
            var seggment = new List<object>();
            var items = segmentString.Trim().Replace("}", "},").TrimEnd(',').Split(',', StringSplitOptions.RemoveEmptyEntries)
                .Select(s => s.Trim()).Where(s => !string.IsNullOrWhiteSpace(s))
                .ToArray();

            foreach (var item in items)
            {
                if (item.StartsWith('{') && item.EndsWith('}'))
                {
                    // 配件数据
                    var partContent = item[1..^1];

                    if (partContent.Contains(":["))
                    {
                        // 数组格式 {type:[values]}
                        var colonIndex = partContent.IndexOf(":[");
                        var type = uint.Parse(partContent[..colonIndex]);
                        var arrayContent = partContent.Substring(colonIndex + 2, partContent.Length - colonIndex - 3);
                        var values = arrayContent.Split(' ', StringSplitOptions.RemoveEmptyEntries)
                            .Where(s => !string.IsNullOrWhiteSpace(s))
                            .Select(uint.Parse)
                            .ToArray();
                        seggment.Add(new ArrayValue { Type = type, Values = values });
                    }
                    else if (partContent.Contains(':'))
                    {
                        // 复合 {type:value}
                        var parts = partContent.Split(':', StringSplitOptions.RemoveEmptyEntries);
                        var type = uint.Parse(parts[0]);
                        var value = uint.Parse(parts[1]);
                        seggment.Add(new ComplexValue { Type = type, Value = value });
                    }
                    else
                    {
                        // 简单格式 {type}
                        var type = uint.Parse(partContent);
                        seggment.Add(new SimpleValue { Type = type });
                    }
                }
                else
                {
                    // 普通数值
                    seggment.Add(uint.Parse(item));
                }
            }

            segments.Add(seggment);
        }

        return segments;
    }

    private void BuildBitStream(BitStreamWriter writer, List<List<object>> segments)
    {
        // 起始标志
        writer.WriteBits(CONSTS.ITEM_DATA_HEADER_MARKER, 5); // 00100

        // 起始标志后的分隔符
        writer.WriteBits(CONSTS.TOKEN_SEGMENT_START_MARKER, 2); // 00

        // 编码每个片段
        for (var i = 0; i < segments.Count; i++)
        {
            var segment = segments[i];

            // 编码片段内的数据
            for (var j = 0; j < segment.Count; j++)
            {
                var item = segment[j];
                var isLastInSegment = j == segment.Count - 1;

                if (item is uint number)
                {
                    // 普通数值 - 通常使用 varint16 编码（最大值为0xFFFF）
                    writer.WriteCompactNumber(number); //采用自动检测最终比特长度，使用更短的方式写入

                    // 片段内分隔符
                    if (!isLastInSegment)
                    {
                        writer.WriteBits(CONSTS.TOKEN_INTRA_SEGMENT_SEPARATOR, 2);
                    }
                }
                else if (item is SimpleValue single)
                {
                    // 简单格式
                    writer.WriteBits(CONSTS.TOKEN_PART_START_MARKER, 3); // 101 - 配件标记
                    writer.WriteVarint16(single.Type);
                    writer.WriteBits(CONSTS.TOKEN_PART_END_MARKER, 3); // 010 - 单个值格式
                }
                else if (item is ComplexValue complex)
                {
                    // 复合格式
                    writer.WriteBits(CONSTS.TOKEN_PART_START_MARKER, 3); // 101 - 配件标记
                    writer.WriteVarint16(complex.Type);
                    writer.WriteBits(CONSTS.TOKEN_PART_COMPLEX_FORMAT_FLAG, 1); // 1 - 对象格式标记
                    writer.WriteVarint16(complex.Value);
                    writer.WriteBits(CONSTS.TOKEN_PART_COMPLEX_VALUE_END_MARKER, 3); // 000 - 对象结束标记
                }
                else if (item is ArrayValue array)
                {
                    // 数组格式
                    writer.WriteBits(CONSTS.TOKEN_PART_START_MARKER, 3); // 101 - 配件标记
                    writer.WriteVarint16(array.Type);
                    writer.WriteBits(CONSTS.TOKEN_PART_ARRAY_VALUE_FLAG, 3); // 001 - 数组类型标记
                    writer.WriteBits(CONSTS.TOKEN_PART_ARRAY_VALUE_START_MARKER, 2); // 01 - 数组开始

                    // 编码数组元素
                    for (var k = 0; k < array.Values.Length; k++)
                    {
                        writer.WriteCompactNumber(array.Values[k]);
                    }

                    writer.WriteBits(CONSTS.TOKEN_PART_ARRAY_VALUE_END_MARKER, 2); // 00 - 数组结束标记
                }
            }

            writer.WriteBits(CONSTS.TOKEN_SEGMENT_END_MARKER, 2); // 00 - 片段结束标记
        }
    }
}



