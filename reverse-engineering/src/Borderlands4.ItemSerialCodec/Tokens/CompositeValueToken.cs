namespace Borderlands4.ItemSerialCodec.Tokens;

public class CompositeValueToken(uint value, uint subValue) : Token(TokenType.CompositeValue)
{
    public uint Value { get; } = value;

    public uint SubValue { get; } = subValue;

    public override string ToString()
    {
        return $"{{{Value}:{SubValue}}}";
    }
}
