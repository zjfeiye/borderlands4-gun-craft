namespace Borderlands4.ItemSerialCodec.Tokens;

public class SegmentSeparatorToken() : Token(TokenType.SegmentSeparator)
{
    public override string ToString() => "|";
}
