using Borderlands4.ItemSerialCodec;

namespace Deserializer;

class Program
{
    static void Main(string[] args)
    {
        TestBase85Decoder();

        TestItemSerialDecoder();

        TestItemSerialEncoder();

        Console.WriteLine("\n按任意键退出...");
        Console.ReadKey();
    }

    static void TestItemSerialEncoder()
    {
        Base85Decoder base85Decoder = new Base85Decoder();
        ItemSerialDecoder decoder = new ItemSerialDecoder();
        ItemSerialEncoder encoder = new ItemSerialEncoder();

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
                string bitstream = base85Decoder.DecodeToBitstream(testCase.Serial);
                var decodedResults = decoder.DecodeBitstream(bitstream);
                string decodedString = decoder.FormatResults(decodedResults);

                Console.WriteLine($"解码结果: {decodedString}");

                // 步骤2: 使用编码器将格式化数据编码回序列号
                string encodedSerial = encoder.EncodeToSerial(decodedString);
                Console.WriteLine($"编码结果: {encodedSerial}");

                // 步骤3: 再次解码编码后的序列号验证一致性
                string encodedBitstream = base85Decoder.DecodeToBitstream(encodedSerial);
                var redecodedResults = decoder.DecodeBitstream(encodedBitstream);
                string redecodedString = decoder.FormatResults(redecodedResults);

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

    static void TestIndividualComponents()
    {
        Console.WriteLine("组件测试");
        Console.WriteLine("========\n");

        // 测试 Base85Encoder
        TestBase85Encoder();

        // 测试 BitStreamWriter
        TestBitStreamWriter();

        // 测试 ItemSerialEncoder 的解析功能
        TestDataParsing();
    }

    static void TestBase85Encoder()
    {
        Console.WriteLine("Base85Encoder 测试:");

        Base85Encoder encoder = new Base85Encoder();
        byte[] testData = new byte[] { 0x84, 0xE4, 0x59, 0x5C, 0xCB, 0xD9, 0x00 };

        // 镜像字节
        byte[] mirrored = encoder.MirrorBytes(testData);
        Console.WriteLine($"原始数据: {BitConverter.ToString(testData).Replace("-", "")}");
        Console.WriteLine($"镜像数据: {BitConverter.ToString(mirrored).Replace("-", "")}");

        // Base85 编码
        string encoded = encoder.Encode(mirrored);
        Console.WriteLine($"Base85 编码: {encoded}");

        // 验证解码
        Base85Decoder decoder = new Base85Decoder();
        string bitstream = decoder.DecodeToBitstream("@U" + encoded);
        Console.WriteLine($"解码验证: {bitstream.Length} 位比特流");

        Console.WriteLine();
    }

    static void TestBitStreamWriter()
    {
        Console.WriteLine("BitStreamWriter 测试:");

        var writer = new BitStreamWriter();

        // 测试写入比特
        writer.WriteBits(0x04, 5); // 00100
        writer.WriteBits(0x00, 2); // 00

        // 测试 varint16
        writer.WriteVarint16(22);
        writer.WriteBits(0x01, 2); // 01

        writer.WriteVarint16(0);
        writer.WriteBits(0x01, 2); // 01

        writer.WriteVarint16(1);
        writer.WriteBits(0x01, 2); // 01

        writer.WriteVarint16(50);
        writer.WriteBits(0x00, 2); // 00

        // 获取字节数组
        byte[] data = writer.ToByteArray();
        Console.WriteLine($"写入的字节: {BitConverter.ToString(data).Replace("-", "")}");
        Console.WriteLine($"总比特数: {data.Length * 8}");

        Console.WriteLine();
    }

    static void TestDataParsing()
    {
        Console.WriteLine("数据解析测试:");

        ItemSerialEncoder encoder = new ItemSerialEncoder();

        // 测试各种配件格式的解析
        string testData1 = "22, 0, 1, 50| {67} {2:13} {5:[1 2 3]}|";

        try
        {
            var fragments = encoder.ParseFormattedData(testData1);
            Console.WriteLine($"解析测试数据: {testData1}");
            Console.WriteLine($"解析成功: {fragments.Count} 个片段");

            foreach (var fragment in fragments)
            {
                Console.WriteLine($"  片段: {fragment.Count} 个元素");
                foreach (var item in fragment)
                {
                    if (item is uint num)
                        Console.WriteLine($"    数值: {num}");
                    else if (item is SinglePart single)
                        Console.WriteLine($"    单值配件: {single.Type}");
                    else if (item is ObjectPart obj)
                        Console.WriteLine($"    对象配件: {obj.Type}:{obj.Value}");
                    else if (item is ArrayPart array)
                        Console.WriteLine($"    数组配件: {array.Type}:[{string.Join(" ", array.Values)}]");
                }
            }
        }
        catch (Exception e)
        {
            Console.WriteLine($"解析失败: {e.Message}");
        }

        Console.WriteLine();
    }

    static string NormalizeString(string str)
    {
        // 移除所有空格和制表符，转换为小写进行比较
        return new string(str.Where(c => !char.IsWhiteSpace(c)).ToArray()).ToLower();
    }

    static void TestItemSerialDecoder()
    {
        Base85Decoder base85Decoder = new Base85Decoder();
        ItemSerialDecoder itemDecoder = new ItemSerialDecoder();

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
                // Base85解码为bitstream
                string bitstream = base85Decoder.DecodeToBitstream(testCase.Serial);

                // 物品代码解码
                List<List<object>> results = itemDecoder.DecodeBitstream(bitstream, debug: true);

                // 格式化输出
                string formattedResult = itemDecoder.FormatResults(results);

                Console.WriteLine($"\n解码结果:");
                Console.WriteLine($"  实际: {formattedResult}");
                Console.WriteLine($"  期望: {testCase.Expected}");

                // 简单比较结果
                bool success = formattedResult.Replace(" ", "").Replace("\t", "") ==
                              testCase.Expected.Replace(" ", "").Replace("\t", "");
                Console.WriteLine($"  测试: {(success ? "√ 通过" : "× 失败")}");
            }
            catch (Exception e)
            {
                Console.WriteLine($"解码失败: {e.Message}");
                Console.WriteLine($"测试: × 失败");
            }
        }

        Console.WriteLine("\n所有测试完成！\n\n\n\n\n");
    }

    static void TestBase85Decoder()
    {
        Console.WriteLine("\nBase85解码器测试");
        Console.WriteLine("================\n");

        TestBase85DecoderBasicFunctionality();
        TestBase85DecoderExampleSequences();
        TestBase85DecoderEdgeCases();

        Console.WriteLine("\n所有测试完成！\n\n\n\n\n");
    }

    static void TestBase85DecoderBasicFunctionality()
    {
        Console.WriteLine("1. 基本功能测试");
        Console.WriteLine("----------------");

        Base85Decoder decoder = new Base85Decoder();

        // 测试1: 去除前导@U
        string test1 = "@UgydOV%h><";
        string stripped1 = decoder.StripLeadingU(test1);
        Console.WriteLine($"测试去除前导@U:");
        Console.WriteLine($"  输入: {test1}");
        Console.WriteLine($"  输出: {stripped1}");
        Console.WriteLine($"  结果: {(stripped1 == "gydOV%h><" ? "√ 通过" : "× 失败")}\n");

        // 测试2: 字节镜像
        byte testByte = 0b10000111; // 135
        byte mirrored = decoder.MirrorByte(testByte);
        Console.WriteLine($"测试字节镜像:");
        Console.WriteLine($"  输入: {Convert.ToString(testByte, 2).PadLeft(8, '0')} ({testByte})");
        Console.WriteLine($"  输出: {Convert.ToString(mirrored, 2).PadLeft(8, '0')} ({mirrored})");
        Console.WriteLine($"  期望: 11100001 (225)");
        Console.WriteLine($"  结果: {(mirrored == 225 ? "√ 通过" : "× 失败")}\n");

        // 测试3: 字节数组镜像
        byte[] testBytes = new byte[] { 0x84, 0xE4, 0x59, 0x5C, 0xCB, 0xD9, 0x00 };
        byte[] mirroredBytes = decoder.MirrorBytes(testBytes);
        string mirroredHex = BitConverter.ToString(mirroredBytes).Replace("-", "").ToLower();
        Console.WriteLine($"测试字节数组镜像:");
        Console.WriteLine($"  输入: {BitConverter.ToString(testBytes).Replace("-", "").ToLower()}");
        Console.WriteLine($"  输出: {mirroredHex}");
        Console.WriteLine($"  期望: 21279a3ad39b00");
        Console.WriteLine($"  结果: {(mirroredHex == "21279a3ad39b00" ? "√ 通过" : "× 失败")}\n");
    }

    static void TestBase85DecoderExampleSequences()
    {
        Console.WriteLine("2. 示例序列测试");
        Console.WriteLine("----------------");

        Base85Decoder decoder = new Base85Decoder();

        // 已知的测试用例
        var testCases = new[]
        {
            new {
                Serial = "@UgydOV%h><",
                ExpectedStripped = "gydOV%h><",
                ExpectedBytes = "84e4595ccbd900",
                ExpectedMirrored = "21279a3ad39b00",
                ExpectedBitstream = "00100001001001111001101000111010110100111001101100000000"
            },
            new {
                Serial = "@UgdhV<Fme!O0ud(G1Fb_9>QISFhpLIn)TZX3LZL>XUZH-WdLjS",
                ExpectedStripped = "gdhV<Fme!O0ud(G1Fb_9>QISFhpLIn)TZX3LZL>XUZH-WdLjS",
                ExpectedBytes = "", // 由于长度较长，我们只验证最终bitstream
                ExpectedMirrored = "",
                ExpectedBitstream = "001000010000010011000000001100100000110001001110000010001010011001000000100010000110010001101011110000001011010111000010101010000101011100001010100100010101001011100001010101011001000101010011001010110110010101100111000001010100001010000101011000101000010101111010100001010111111010000101010111100100010000000000" // 我们主要验证解码过程不报错
            }
        };

        foreach (var testCase in testCases)
        {
            Console.WriteLine($"测试序列: {testCase.Serial}");

            try
            {
                // 测试去除前导
                string stripped = decoder.StripLeadingU(testCase.Serial);
                Console.WriteLine($"  去除前导: {(stripped == testCase.ExpectedStripped ? "√" : "×")} {stripped}");

                // 测试完整解码流程
                string bitstream = decoder.DecodeToBitstream(testCase.Serial);

                if (!string.IsNullOrEmpty(testCase.ExpectedBitstream))
                {
                    bool bitstreamMatch = bitstream == testCase.ExpectedBitstream;
                    Console.WriteLine($"  比特流匹配: {(bitstreamMatch ? "√" : "×")}");

                    if (!bitstreamMatch)
                    {
                        Console.WriteLine($"    期望: {testCase.ExpectedBitstream}");
                        Console.WriteLine($"    实际: {bitstream}");
                    }
                }
                else
                {
                    Console.WriteLine($"  比特流生成: √ (长度: {bitstream.Length} 位)");
                }

                Console.WriteLine("  √ 解码成功\n");
            }
            catch (Exception ex)
            {
                Console.WriteLine($"  × 解码失败: {ex.Message}\n");
            }
        }
    }

    static void TestBase85DecoderEdgeCases()
    {
        Console.WriteLine("3. 边界情况测试");
        Console.WriteLine("----------------");

        Base85Decoder decoder = new Base85Decoder();

        // 测试1: 空字符串
        try
        {
            string result = decoder.DecodeToBitstream("");
            Console.WriteLine($"空字符串测试: {(string.IsNullOrEmpty(result) ? "√ 通过" : "× 失败")}");
        }
        catch
        {
            Console.WriteLine("空字符串测试: × 异常\n");
        }

        // 测试2: 只有@U
        try
        {
            string result = decoder.DecodeToBitstream("@U");
            Console.WriteLine($"只有@U测试: {(string.IsNullOrEmpty(result) ? "√ 通过" : "× 失败")}");
        }
        catch
        {
            Console.WriteLine("只有@U测试: × 异常\n");
        }

        // 测试3: 无效字符（应该会抛出异常）
        try
        {
            string result = decoder.DecodeToBitstream("@Uinvalid*chars");
            Console.WriteLine($"无效字符测试: × 应该抛出异常但通过了");
        }
        catch (Exception ex)
        {
            Console.WriteLine($"无效字符测试: √ 正确抛出异常 - {ex.Message}\n");
        }

        // 测试4: 带调试输出的版本
        Console.WriteLine("调试输出测试:");
        string debugResult = decoder.DecodeToBitstreamWithDebug("@UgydOV%h><");
        Console.WriteLine($"  调试版本输出: √ 完成 (比特流长度: {debugResult.Length})\n");
    }
}
