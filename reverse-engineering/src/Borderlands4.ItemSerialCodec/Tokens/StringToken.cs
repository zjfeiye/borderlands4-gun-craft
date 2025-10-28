namespace Borderlands4.ItemSerialCodec.Tokens;

public class StringToken(string value) : Token(TokenType.StringValue)
{
    public string Value { get; } = value;

    public override string ToString()
    {
        return $"\"{Value}\"";
    }
}
