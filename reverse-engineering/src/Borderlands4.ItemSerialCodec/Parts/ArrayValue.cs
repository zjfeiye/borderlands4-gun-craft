namespace Borderlands4.ItemSerialCodec.Parts;

public class ArrayValue : IPartValue
{
    public uint Type { get; set; }
    public required uint[] Values { get; set; }

    public override string ToString()
    {
        return $"{{{Type}:[{string.Join(" ", Values)}]}}";
    }
}