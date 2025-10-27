using System.Text.RegularExpressions;

namespace Borderlands4.ItemSerialCodec.WinApp;

public partial class ItemPartsValidator
{
    // 验证整个物品序列号格式
    public static bool ValidateItemParts(string input)
    {
        if (string.IsNullOrWhiteSpace(input))
            return false;

        // 基本格式检查：包含片段分隔符 |
        if (!input.Contains("|"))
            return false;

        // 分割片段
        string[] fragments = input.Replace("}", "},").Split('|', StringSplitOptions.RemoveEmptyEntries);

        foreach (string fragment in fragments)
        {
            if (!ValidateFragment(fragment.Trim()))
                return false;
        }

        return true;
    }

    // 验证单个片段
    private static bool ValidateFragment(string fragment)
    {
        if (string.IsNullOrWhiteSpace(fragment))
            return true; // 空片段是允许的

        string[] elements = fragment.TrimEnd(',').Split(',');

        foreach (string element in elements)
        {
            if (!ValidateElement(element.Trim()))
                return false;
        }

        return true;
    }

    // 验证单个元素
    private static bool ValidateElement(string element)
    {
        if (string.IsNullOrWhiteSpace(element))
            return false;

        // 普通数字
        if (NumberRegex.IsMatch(element))
            return true;

        // 单个值配件 {数字}
        if (SImpleValueRegex.IsMatch(element))
            return true;

        // 对象值配件 {数字:数字}
        if (ComplexValueRegex.IsMatch(element))
            return true;

        // 数组值配件 {数字:[数字 数字 ...]}
        if (ArrayValueRegex.IsMatch(element))
            return true;

        return false;
    }

    [GeneratedRegex(@"^\d+$", RegexOptions.Compiled)]
    private static partial Regex NumberRegex { get; }

    [GeneratedRegex(@"^\{\d+\}$", RegexOptions.Compiled)]
    private static partial Regex SImpleValueRegex { get; }

    [GeneratedRegex(@"^\{\d+:\d+\}$", RegexOptions.Compiled)]
    private static partial Regex ComplexValueRegex { get; }

    [GeneratedRegex(@"^\{\d+:\s*\[\s*\d+(\s+\d+)*\s*\]\}$", RegexOptions.Compiled)]
    private static partial Regex ArrayValueRegex { get; }
}