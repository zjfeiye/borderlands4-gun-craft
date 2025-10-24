namespace Borderlands4.ItemSerialCodec.Parts;

public class ComplexValue : IPartValue
{
    public uint Type { get; set; }
    public uint Value { get; set; }

    public override string ToString()
    {
        return $"{{{Type}:{Value}}}";
    }
}