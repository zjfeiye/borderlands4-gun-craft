using Borderlands4.ItemSerialCodec;

namespace ItemSerialCodec.ConsoleTests;
class Program
{
    static void Main(string[] args)
    {
        TestItemSerialDecoder();
        TestItemSerialEncoder();

        Console.WriteLine("\n按任意键退出...");
        Console.ReadKey();
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
                Expected = "8, 0, 1, 50| 2, 2677|| {53} {2} {4} {3} {52} {74} {10} {15} {75} {25} {32} {33} {39} {47} {79}|"
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
                var decodedString = decoder.DecodeAsString(testCase.Serial);

                Console.WriteLine($"解码结果: {decodedString}");

                // 步骤2: 使用编码器将格式化数据编码回序列号
                string encodedSerial = encoder.EncodeToSerial(decodedString);
                Console.WriteLine($"编码结果: {encodedSerial}");

                // 步骤3: 再次解码编码后的序列号验证一致性
                var redecodedString = decoder.DecodeAsString(encodedSerial);

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
                Expected = "8, 0, 1, 50| 2, 2677|| {53} {2} {4} {3} {52} {74} {10} {15} {75} {25} {32} {33} {39} {47} {79}|"
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
                string formattedResult = itemDecoder.DecodeAsString(testCase.Serial, debug: true);

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
}