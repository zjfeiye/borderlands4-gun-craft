namespace Borderlands4.ItemSerialCodec.Tokens;

public class ArrayValueToken(uint value, object[] subValues) : Token(TokenType.ArrayValue)
{
    public uint Value { get; } = value;
    public object[] SubValues { get; } = subValues;

    public override string ToString()
    {
        var valueStr = SubValues.Select(o => o is uint num ? num.ToString() : $"\"{o}\"");
        return $"{{{Value}:[{string.Join(" ", valueStr)}]}}";
    }
}
