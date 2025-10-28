namespace Borderlands4.ItemSerialCodec.Tokens;

public class ValueSeparatorToken() : Token(TokenType.SegmentSeparator)
{
    public override string ToString() => ",";
}
