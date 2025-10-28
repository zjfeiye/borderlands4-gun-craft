namespace Borderlands4.ItemSerialCodec.Tokens;

public class SimpleValueToken(uint value) : Token(TokenType.SimpleValue)
{
    public uint Value { get; } = value;

    public override string ToString()
    {
        return $"{{{Value}}}";
    }
}
