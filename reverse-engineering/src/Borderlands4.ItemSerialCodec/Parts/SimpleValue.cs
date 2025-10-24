namespace Borderlands4.ItemSerialCodec.Parts;

// 配件数据类型
public class SimpleValue : IPartValue
{
    public uint Type { get; set; }

    public override string ToString()
    {
        return $"{{{Type}}}";
    }
}

