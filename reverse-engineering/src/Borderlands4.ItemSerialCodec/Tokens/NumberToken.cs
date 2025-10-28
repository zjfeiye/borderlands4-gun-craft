namespace Borderlands4.ItemSerialCodec.Tokens;

public class NumberToken(uint value) : Token(TokenType.NumberValue)
{
    public uint Value { get; } = value;

    public override string ToString()
    {
        return Value.ToString();
    }
}
