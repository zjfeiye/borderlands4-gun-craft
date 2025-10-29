using Borderlands4.ItemSerialCodec;
using System.Text.RegularExpressions;

namespace ItemSerialCodec.ConsoleTests;
class Program
{
    static void Main(string[] args)
    {
        TestRegex();
        TestItemSerialDecoder();
        TestItemSerialEncoder();

        Console.WriteLine("\n按任意键退出...");
        Console.ReadKey();
    }

    static void TestRegex()
    {
        string combinedPattern = @"^((?:\d+|""[a-zA-Z0-9\._]+""|\\""[a-zA-Z0-9\._]+\\"")[ \t]*(?:,[ \t]*(?:\d+|""[a-zA-Z0-9\._]+""|\\""[a-zA-Z0-9\._]+\\""))*[ \t]*\|[ \t]*){2,}[ \t]*\|[ \t]*(""[a-zA-Z0-9\._]+""[ \t]*|\\""[a-zA-Z0-9\._]+\\""[ \t]*|\{[ \t]*\d+[ \t]*(?:[ \t]*:[ \t]*(?:\d+|\[[ \t]*(?:\d+|""[a-zA-Z0-9\._]+""|\\""[a-zA-Z0-9\._]+\\"")(?:[ \t]+(?:\d+|""[a-zA-Z0-9\._]+""|\\""[a-zA-Z0-9\._]+\\""))*[ \t]*\]))?[ \t]*\}[ \t]*)+[ \t]*(\|[ \t]*)?(((?:[ \t]*\d+|""[a-zA-Z0-9\._]+""|\\""[a-zA-Z0-9\._]+\\"")[ \t]*(?:,[ \t]*(?:\d+|""[a-zA-Z0-9\._]+""|\\""[a-zA-Z0-9\._]+\\""))*[ \t]*\|))?$";

        string[] testCases = {
            "303, 0, 1, 50| 2, 1733| | {8} {247:76} {1} {2} {247:[90 218 19]}|", // 原始示例
            "\"abc\", \"def\", 123| \"test\", 456| | {8} {247:76} {1} {2} {247:[\"xyz\" 218 \"test\"]}|", // 双引号字符串
            "\\\"abc\\\", \\\"def\\\", 123| \\\"test\\\", 456| | {8} {247:76} {1} {2} {247:[\\\"xyz\\\" 218 \\\"test\\\"]}|", // 斜杠双引号字符串
            "303, 0, \"test1\", 50| 2, \\\"test2\\\"| | {8} {247:76} {1} {2} {247:[90 \"test3\" 19]}|", // 混合双引号和斜杠双引号
            "303, 0, \"test1\", 50| 2, \\\"test2\\\"| | {8} {247:76} \"test3\" {2} {247:[90 \"test4\" 19]}|", // 混合双引号和斜杠双引号
            "303, 0, 1, 50| 2, 1733| | {8} {247:76} {1} {2} {247:[90 218 19]}|", // 纯数字
        };

        foreach (string test in testCases)
        {
            bool isMatch = Regex.IsMatch(test, combinedPattern);
            Console.WriteLine($"'{test}' -> {(isMatch ? "匹配" : "不匹配")}");

            if (isMatch)
            {
                // 提取匹配的部分进行验证
                Match match = Regex.Match(test, combinedPattern);
                Console.WriteLine($"  完整匹配: '{match.Value}'");

                // 提取花括号部分
                MatchCollection braceMatches = Regex.Matches(match.Value, @"\{\s*[^}]+\}");
                Console.WriteLine($"  找到 {braceMatches.Count} 个花括号单元:");
                foreach (Match braceMatch in braceMatches)
                {
                    Console.WriteLine($"    {braceMatch.Value}");
                }
            }
            Console.WriteLine();
        }
    }

    static void TestItemSerialDecoder()
    {
        var succeed = 0;
        var failed = 0;

        var itemDecoder = new ItemSerialDecoder();

        // 测试数据 - 示例序列及其期望结果
        var testCases = new[]
        {
            new {
                Serial = "@UgxFw!2}TYgjNc48i7M2hN}^_>Vxm5E1~mtj2XzXS3Y7~L4s{O!",
                Expected = "22, 0, 1, 50| 2, 3262|| {67} {2} {5} {66} {73} {72} {15} {19} {25} {28} {35} {36} {44} {48} {59}|"
            },
            new {
                Serial = "@UgdhV<Fme!O0ue@92CYLCDp8FZHk1xk6LqOi-9*hng+h%&y+VaU{X_r",
                Expected = "8, 0, 1, 50| 10, 1| 2, 3170|| {53} {2} {4} {6} {1:13} {52} {74} {11} {15} {75} {25} {32} {33} {39} {48} {79}|"
            },
            new {
                Serial = "@Ugr%1Tm/)}}!qhvUNWCv7Xi/fEAI%M^D+d4",
                Expected = "303, 0, 1, 50| 2, 885|| {8} {247:76} {9} {1} {247:[23 181 7]}|"
            },
            new {
                Serial = "@Ug!pHG2}TZ*Od!Hk{GfKIO!YFT-3FEGpl%;j?$j-VsvQj+$sw}<",
                Expected = "254, 0, 1, 50| 9, 1| 2, 3973|| {54} {12} {302} {365} {428} {491} {236} {299} {234:[48 25 83]}|"
            },
            new {
                Serial = "@UgdhV<Fme!K>Q&G>RG/`esC1~Bs7ih6CTb2U6lxUe73vr2C*omj7y", 
                Expected = "8, 0, 1, 50| 2, 2677|| {53} {2} {4} {3} {52} {74} {10} {15} {75} {25} {32} {33} {39} {47} {79}| \"c\", 12|"
            },
            new
            {
                Serial = "@Ugw$Yw2}TYg44elZMKj3!PMfaryAA)sYF~EwTy~Sg^8DAFmY45u`MN?a>QIduGTet/kolc9UGT7+{uPJi@-q7ilYPx;dHMd9uZxN5)E(3*R4LRg)H&1;4gd",
                Expected = "21, 0, 1, 50| 2, 1840|| \"MAL_SM.comp_05_legendary_firework\" {2} {5} {3} {6} {1:12} \"MAL_SM.part_barrel_02_firework\" {72} {14} {27} {35} {34} {43} {51} {1:48}|"
            }
        };

        Console.WriteLine("\n物品序列号解码器测试");
        Console.WriteLine("===================\n");

        for (int i = 0; i < testCases.Length; i++)
        {
            var testCase = testCases[i];
            Console.WriteLine(new string('=', 60));
            Console.WriteLine($"测试 #{i + 1}: {testCase.Serial}");
            Console.WriteLine(new string('-', 60));

            try
            {
                // 物品代码解码
                string formattedResult = itemDecoder.DecodeAsPartsString(testCase.Serial, debug: true);

                Console.WriteLine($"\n解码结果:");
                Console.WriteLine($"  实际: {formattedResult}");
                Console.WriteLine($"  期望: {testCase.Expected}");

                // 简单比较结果
                var success = formattedResult.Replace(" ", "").Replace("\t", "") ==
                              testCase.Expected.Replace(" ", "").Replace("\t", "");
                Console.WriteLine($"  测试: {(success ? "√ 通过" : "× 失败")}");

                succeed += success ? 1 : 0;
            }
            catch (Exception e)
            {
                Console.WriteLine($"解码失败: {e.Message}");
                Console.WriteLine($"测试: × 失败");

                failed++;
            }
        }

        Console.WriteLine($"\n所有测试完成！{succeed} 成功, {failed} 失败\n\n\n\n\n");
    }

    static void TestItemSerialEncoder()
    {
        var decoder = new ItemSerialDecoder();
        var encoder = new ItemSerialEncoder();

        // 示例数据 - 已知的序列号和对应的格式化数据
        var testCases = new[]
        {
            new {
                Serial = "@UgxFw!2}TYgjNc48i7M2hN}^_>Vxm5E1~mtj2XzXS3Y7~L4s{O!",
                Expected = "22, 0, 1, 50| 2, 3262|| {67} {2} {5} {66} {73} {72} {15} {19} {25} {28} {35} {36} {44} {48} {59}|"
            },
            new {
                Serial = "@UgdhV<Fme!O0ue@92CYLCDp8FZHk1xk6LqOi-9*hng+h%&y+VaU{X_r",
                Expected = "8, 0, 1, 50| 10, 1| 2, 3170|| {53} {2} {4} {6} {1:13} {52} {74} {11} {15} {75} {25} {32} {33} {39} {48} {79}|"
            },
            new {
                Serial = "@Ugr%1Tm/)}}!qhvUNWCv7Xi/fEAI%M^D+d4",
                Expected = "303, 0, 1, 50| 2, 885|| {8} {247:76} {9} {1} {247:[23 181 7]}|"
            },
            new {
                Serial = "@Ug!pHG2}TZ*Od!Hk{GfKIO!YFT-3FEGpl%;j?$j-VsvQj+$sw}<",
                Expected = "254, 0, 1, 50| 9, 1| 2, 3973|| {54} {12} {302} {365} {428} {491} {236} {299} {234:[48 25 83]}|"
            },
            new {
                Serial = "@UgdhV<Fme!K>Q&G>RG/`esC1~Bs7ih6CTb2U6lxUe73vr2C*omj7y",
                Expected = "8, 0, 1, 50| 2, 2677|| {53} {2} {4} {3} {52} {74} {10} {15} {75} {25} {32} {33} {39} {47} {79}| \"c\", 12|"
            },
            new
            {
                Serial = "@Ugw$Yw2}TYg44elZMKj3!PMfaryAA)sYF~EwTy~Sg^8DAFmY45u`MN?a>QIduGTet/kolc9UGT7+{uPJi@-q7ilYPx;dHMd9uZxN5)E(3*R4LRg)H&1;4gd",
                Expected = "21, 0, 1, 50| 2, 1840|| \"MAL_SM.comp_05_legendary_firework\" {2} {5} {3} {6} {1:12} \"MAL_SM.part_barrel_02_firework\" {72} {14} {27} {35} {34} {43} {51} {1:48}|"
            }
        };

        Console.WriteLine("物品序列号编码器测试");
        Console.WriteLine("==================================\n");

        int passedTests = 0;
        int totalTests = testCases.Length;

        for (int i = 0; i < testCases.Length; i++)
        {
            var testCase = testCases[i];
            Console.WriteLine(new string('=', 70));
            Console.WriteLine($"测试 #{i + 1}:");
            Console.WriteLine($"原始序列号: {testCase.Serial}");
            Console.WriteLine($"期望数据: {testCase.Expected}");
            Console.WriteLine(new string('-', 70));

            try
            {
                // 步骤1: 解码原始序列号获取格式化数据
                var decodedString = decoder.DecodeAsPartsString(testCase.Serial);

                Console.WriteLine($"解码结果: {decodedString}");

                // 步骤2: 使用编码器将格式化数据编码回序列号
                string encodedSerial = encoder.EncodeToSerial(decodedString);
                Console.WriteLine($"编码结果: {encodedSerial}");

                // 步骤3: 再次解码编码后的序列号验证一致性
                var redecodedString = decoder.DecodeAsPartsString(encodedSerial);

                Console.WriteLine($"再次解码: {redecodedString}");

                // 验证
                bool originalMatch = NormalizeString(decodedString) == NormalizeString(testCase.Expected);
                bool roundtripMatch = NormalizeString(redecodedString) == NormalizeString(decodedString);
                bool serialMatch = encodedSerial == testCase.Serial;

                Console.WriteLine($"原始数据匹配: {(originalMatch ? "√" : "×")}");
                Console.WriteLine($"往返编码匹配: {(roundtripMatch ? "√" : "×")}");
                Console.WriteLine($"序列号匹配: {(serialMatch ? "√" : "×")}");

                if (originalMatch && roundtripMatch)
                {
                    Console.WriteLine($"测试 #{i + 1}: √ 通过");
                    passedTests++;
                }
                else
                {
                    Console.WriteLine($"测试 #{i + 1}: × 失败");
                }
            }
            catch (Exception e)
            {
                Console.WriteLine($"测试失败: {e.Message}");
                Console.WriteLine($"测试 #{i + 1}: × 失败");
            }

            Console.WriteLine();
        }

        Console.WriteLine(new string('=', 70));
        Console.WriteLine($"测试总结: {passedTests}/{totalTests} 通过");
        Console.WriteLine(new string('=', 70));
    }

    static string NormalizeString(string str)
    {
        // 移除所有空格和制表符，转换为小写进行比较
        return new string(str.Where(c => !char.IsWhiteSpace(c)).ToArray()).ToLower();
    }
}