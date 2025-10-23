namespace Borderlands4.ItemSerialCodec;

public class ItemSerialEncoder
{
    private readonly Base85Encoder _base85Encoder;

    public ItemSerialEncoder()
    {
        _base85Encoder = new Base85Encoder();
    }

    public string EncodeToSerial(string formattedData)
    {
        // 解析格式化数据
        var fragments = ParseFormattedData(formattedData);

        // 生成比特流
        var writer = new BitStreamWriter();
        GenerateBitstream(writer, fragments);

        // 获取字节数组
        byte[] data = writer.ToByteArray();

        // 镜像字节
        byte[] mirroredData = _base85Encoder.MirrorBytes(data);

        // Base85 编码
        string base85String = _base85Encoder.Encode(mirroredData);

        // 添加前导 @U
        return "@U" + base85String;
    }

    public List<List<object>> ParseFormattedData(string formattedData)
    {
        var fragments = new List<List<object>>();

        // 分割片段
        string[] fragmentStrings = formattedData.Trim().Trim('|').Replace("}", "},").Split('|')
            //.Where(s => !string.IsNullOrWhiteSpace(s))
            .ToArray();

        foreach (string fragmentString in fragmentStrings)
        {
            var fragment = new List<object>();
            string[] items = fragmentString.Trim().TrimEnd(',').Split(',')
                .Select(s => s.Trim())
                .Where(s => !string.IsNullOrWhiteSpace(s))
                .ToArray();

            foreach (string item in items)
            {
                if (item.StartsWith('{') && item.EndsWith('}'))
                {
                    // 配件数据
                    string attachmentContent = item.Substring(1, item.Length - 2);

                    if (attachmentContent.Contains(":["))
                    {
                        // 数组配件 {type:[values]}
                        int colonIndex = attachmentContent.IndexOf(":[");
                        uint type = uint.Parse(attachmentContent[..colonIndex]);
                        string arrayContent = attachmentContent.Substring(colonIndex + 2, attachmentContent.Length - colonIndex - 3);
                        uint[] values = arrayContent.Split(' ')
                            .Where(s => !string.IsNullOrWhiteSpace(s))
                            .Select(uint.Parse)
                            .ToArray();
                        fragment.Add(new ArrayPart { Type = type, Values = values });
                    }
                    else if (attachmentContent.Contains(':'))
                    {
                        // 对象配件 {type:value}
                        string[] parts = attachmentContent.Split(':');
                        uint type = uint.Parse(parts[0]);
                        uint value = uint.Parse(parts[1]);
                        fragment.Add(new ObjectPart { Type = type, Value = value });
                    }
                    else
                    {
                        // 单个值配件 {type}
                        uint type = uint.Parse(attachmentContent);
                        fragment.Add(new SinglePart { Type = type });
                    }
                }
                else
                {
                    // 普通数值
                    fragment.Add(uint.Parse(item));
                }
            }

            fragments.Add(fragment);
        }

        return fragments;
    }

    private void GenerateBitstream(BitStreamWriter writer, List<List<object>> fragments)
    {
        // 起始标志
        writer.WriteBits(0x04, 5); // 00100

        // 起始标志后的分隔符
        writer.WriteBits(0x00, 2); // 00

        // 编码每个片段
        for (int i = 0; i < fragments.Count; i++)
        {
            var fragment = fragments[i];

            // 编码片段内的数据
            for (int j = 0; j < fragment.Count; j++)
            {
                var item = fragment[j];
                bool isLastInFragment = j == fragment.Count - 1;

                if (item is uint number)
                {
                    // 普通数值 - 通常使用 varint16 编码（最大值为0xFFFF）
                    writer.WriteNumber(number);

                    // 片段内分隔符
                    if (!isLastInFragment)
                    {
                        writer.WriteBits(0x01u, 2);
                    }
                }
                else if (item is SinglePart single)
                {
                    // 单个值配件
                    writer.WriteBits(0x05, 3); // 101 - 配件标记
                    writer.WriteVarint16(single.Type);
                    writer.WriteBits(0x02, 3); // 010 - 单个值格式
                }
                else if (item is ObjectPart obj)
                {
                    // 对象值配件
                    writer.WriteBits(0x05, 3); // 101 - 配件标记
                    writer.WriteVarint16(obj.Type);
                    writer.WriteBits(0x01, 1); // 1 - 对象格式标记
                    writer.WriteVarint16(obj.Value);
                    writer.WriteBits(0x00, 3); // 000 - 对象结束标记
                }
                else if (item is ArrayPart array)
                {
                    // 数组配件
                    writer.WriteBits(0x05, 3); // 101 - 配件标记
                    writer.WriteVarint16(array.Type);
                    writer.WriteBits(0x01, 3); // 001 - 数组开始标记
                    writer.WriteBits(0x01, 2); // 01 - 数组开始

                    // 编码数组元素
                    for (int k = 0; k < array.Values.Length; k++)
                    {
                        writer.WriteNumber(array.Values[k]);
                    }

                    writer.WriteBits(0x00, 2); // 00 - 数组结束标记
                }
            }

            writer.WriteBits(0x00, 2); // 00 - 片段结束标记
        }
    }
}

// 配件数据类型
public class SinglePart
{
    public uint Type { get; set; }
}

public class ObjectPart
{
    public uint Type { get; set; }
    public uint Value { get; set; }
}

public class ArrayPart
{
    public uint Type { get; set; }
    public uint[] Values { get; set; }
}