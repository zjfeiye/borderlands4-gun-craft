namespace Borderlands4.ItemSerialCodec;

public class CONSTS
{
    /// <summary>
    /// 物品数据头标识符。
    /// </summary>
    public const uint ITEM_DATA_HEADER_MARKER = 0x04u; // 00100 = 4

    /// <summary>
    /// Varint16 编码的开始标识符。
    /// </summary>
    public const uint TOKEN_VARINT16 = 0x04u; // 100 = 4

    /// <summary>
    /// Varbit32 编码的开始标识符。
    /// </summary>
    public const uint TOKEN_VARBIT32 = 0x06u; // 110 = 6

    /// <summary>
    /// 不受支持的标识符（DLC物品和皮肤数据）。
    /// </summary>
    public const uint TOKEN_STRING = 0x07u; // 111 = 7

    /// <summary>
    /// 片段分割符。
    /// </summary>
    public const uint TOKEN_SEGMENT_SEPARATOR = 0x00u; // 00 = 0

    /// <summary>
    /// 片段内数据分割符（片段是指两个'00'之间的数据）。
    /// </summary>
    public const uint TOKEN_VALUE_SEPARATOR = 0x01u; // 01 = 1

    /// <summary>
    /// 配件数据头标志，它的数据总是以 Varint16 编码。
    /// </summary>
    public const uint TOKEN_PART = 0x05u; // 101 = 5

    /// <summary>
    /// 配件数据结尾标志。
    /// </summary>
    public const uint TOKEN_PART_END_MARKER = 0x02u; // 010 = 2

    /// <summary>
    /// 配件数据格式：复合数据。
    /// </summary>
    public const uint TOKEN_PART_COMPOSITE_FORMAT_FLAG = 0x01u; // 1

    /// <summary>
    /// 配件数据中，复合格式数据的结束标识符。
    /// </summary>
    public const uint TOKEN_PART_COMPOSITE_VALUE_END_MARKER = 0x00u; // 000 = 0

    /// <summary>
    /// 配件数据格式：数组数据。
    /// </summary>
    public const uint TOKEN_PART_ARRAY_FORMAT_FLAG = 0x01u; // 001 = 1

    /// <summary>
    /// 配件数据中，数组格式数据的开始标识符。
    /// </summary>
    public const uint TOKEN_PART_ARRAY_VALUES_START_MARKER = 0x01u; // 01 = 1
    /// <summary>
    /// 配件数据中，数组格式数据的结束标识符。
    /// </summary>
    public const uint TOKEN_PART_ARRAY_VALUES_END_MARKER = 0x00u; // 00 = 0
}
