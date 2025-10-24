using Borderlands4.ItemSerialCodec;
using Borderlands4.ItemSerialCodec.Extensions;

namespace ItemSerialCodec.MSTests;

[TestClass]
public sealed class Base85Tests
{
    private class TestCase
    {
        public required string Serial { get; init; }
        public required string MirroredBytes { get; init; }
    }

    //test case from https://github.com/Nicnl/borderlands4-serials
    private readonly TestCase[] testCases =
        [
            new TestCase {
                Serial = "@Ugy3L+2}TYg%$yC%i7M2gZldO)@}cgb!l34$a-qf{00",
                MirroredBytes = "21070601906270443339b05391542b85764567854f857785430567054e85638400",
            },
            new TestCase {
                Serial = "@Ugy3L+2}Ta0Od!I{*`S=LLLKTRY91;d>K-Z#Y7QzFY8(O",
                MirroredBytes = "2107060190627045320443339b05391542b85764567854f857785430567054e8563840",
            },
            new TestCase {
                Serial = "@Ugy3L+2}TYgjMogxi7Hg07IhPq4>b?9sX3@zs9y*",
                MirroredBytes = "210706019062704431eb305391542a4568aec8a9f0acf0ad959c153a15fa10",
            },
            new TestCase {
                Serial = "@Ugy3L+2}Ta0Od!H/&7hp9LM3WZH&OXe^H7_bgUW^ag#Z",
                MirroredBytes = "21070601906270453204431eb305391542a4568aec8a9f0acf0ad959c153a15fa100",
            },
            new TestCase {
                Serial = "@Ugct)%FmVuJXn{hb3U#POJ!&6nQ*lsxP_0lm5d",
                MirroredBytes = "2118c0320c8e0886168142c550ae15a2bc56f856ca8e0ade0ab50aec88",
            },
            new TestCase {
                Serial = "@Ugct)%FmVuN0uhE5C^V{2hg#I5_MtWv2ek*)3Uw0!",
                MirroredBytes = "2118c0320c8e08a640886168142c550ae15a2bc56f856ca8e0ade0ab50aec880",
            },
            new TestCase {
                Serial = "@UgwSAs2}TYgOz#USjp~P5)S(jfsJ*DNsIaI@g+bLpr9!Pj^+J_H00",
                MirroredBytes = "210b06019062704432f7d054b157e15a2b8548af15bd154d150d15d2a182ab82a542b542af42a9c200",
            },
            new TestCase {
                Serial = "@UglGc",
                MirroredBytes = "2116c0",
            },
            new TestCase {
                Serial = "@Ugr$lGm/)}}!dNJvM-}RPG}?q38r1nh0{{",
                MirroredBytes = "21a5516019062704431a1405e2a8573e2c5b2158582b9f42c000",
            },
            new TestCase {
                Serial = "@Ugr$lGm/)}}!dNJvM-}RPG}?q38r1nh0{{",
                MirroredBytes = "21a5516019062704431a1405e2a8573e2c5b2158582b9f42c000",
            },
            new TestCase {
                Serial = "@UgydOV%h><",
                MirroredBytes = "21279a3ad39b00",
            },
        ];

    [TestMethod]
    public void TestCodec()
    {
        var codec = new Base85();
        foreach (var testCase in testCases)
        {
            var serial = codec.Encode(Convert.FromHexString(testCase.MirroredBytes).MirrorBytes());
            Assert.AreEqual(testCase.Serial, "@U" + serial);

            var bytes = codec.Decode(testCase.Serial[2..]);
            Assert.AreEqual(testCase.MirroredBytes, Convert.ToHexString(bytes.MirrorBytes()), true);
        }
    }

    [TestMethod]
    public void TestError()
    {
        var codec = new Base85();
        foreach (var testCase in testCases)
        {
            Assert.ThrowsException<InvalidOperationException>(() =>
            {
                _ = codec.Decode(testCase.Serial[2..] + "\"\"\"\"\",,,,,,,,,,,,");
            });
        }
    }

    [TestMethod]
    public void TestNoError()
    {
        //test case from https://github.com/Nicnl/borderlands4-serials
        string[] samples = [
            "@Ugy3L+2}TYg%$yC%i7M2gZldO)@}cgb!l34$a-qf{00",
            "@Ugy3L+2}TYg%$yC%i7M2gZldO)@}cgb!l34$a-qf`00",
            "@Ugy3L+2}TYg%$yC%i7M2gZldO)@}cgb!l34$av=Z",
            "@Ugy3L+2}TYg%$yC%i7M2gZldO)@}cgb!l34$G64",
            "@Ugy3L+2}TYg%$yC%i7M2gZldO)@}cgb!l33L00",
            "@Ugy3L+2}TYg%$yC%i7M2gZldO)@}cgb!XN+",
            "@Ugy3L+2}TYg%$yC%i7M2gZldO)@}ce_00",
            "@Ugy3L+2}TYg%$yC%i7M2gZldO)@}ce^00",
            "@Ugy3L+2}TYg%$yC%i7M2gZldO)@*w~",
            "@Ugy3L+2}TYg%$yC%i7M2gZldO)@*n^",
            "@Ugy3L+2}TYg%$yC%i7M2gZldNP00",
            "@Ugy3L+2}TYg%$yC%i7M2gZldNO00",
            "@Ugy3L+2}TYg%$yC%i7M2gZXy5",
            "@Ugy3L+2}TYg%$yC%i7M2gE&%",
            "@Ugy3L+2}TYg%$yC%i7M0~00",
            "@Ugy3L+2}TYg%$yC%i7Es",
            "@Ugy3L+2}TYg%$yC%i2w",
            "@Ugy3L+2}TY8",
            "@Ugy3L+2@aC}/NsC0/Nnmg",
            "@Ugy3L+2}S?",
            "@Ugy3L+2?hW",
            "@Ugx~-",
            "@Ugdh"
        ];

        var codec = new Base85();

        foreach (var serial in samples)
        {
            _ = codec.Decode(serial[2..]);
        }
    }

    [TestMethod]
    public void TestExtensions()
    {
        var codec = new Base85();
        foreach (var testCase in testCases)
        {
            var serial = codec.EncodeToSerial(Convert.FromHexString(testCase.MirroredBytes));
            Assert.AreEqual(testCase.Serial, serial);

            var mirroredBytes = codec.DecodeSerial(testCase.Serial);
            Assert.AreEqual(testCase.MirroredBytes, Convert.ToHexString(mirroredBytes), true);
        }
    }
}
