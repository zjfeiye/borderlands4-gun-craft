namespace Borderlands4.ItemSerialCodec.Tokens;

public abstract class Token(TokenType type)
{
    public TokenType Type { get; } = type;

    public override string ToString() => $"<{Type}>";
}
