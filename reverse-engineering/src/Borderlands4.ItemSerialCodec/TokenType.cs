namespace Borderlands4.ItemSerialCodec;

public enum TokenType
{
    SegmentSeparator,   // |
    ValueSeparator,     // ,
    NumberValue,
    StringValue,
    SimpleValue,
    CompositeValue,
    ArrayValue
}
