using Borderlands4.ItemSerialCodec;
using System.Text.RegularExpressions;

namespace ItemSerialCodec.MSTests;

[TestClass]
public sealed class SerialCodecTests
{
    [TestMethod]
    public void TestCodec()
    {
        string[] samples = [
            "@Ugy3L+2}TYg%$yC%i7M2gZldO)@}cgb!l34$a-qf{00",
            "@Ugr$WBm/$!m!X=5&qXq#",
            "@Ugr$WBm/$!m!X=5&qXxA;nj3Nj00",
            "@Ugr$WBm/$!m!X=5&qXxA;nj3OODgg",
            "@Ugr$WBm/$!m!X=5&qXxA;nj3OOD#<4R",
            "@Ugy3L+2}TYgOyvyviz?KiBDJYGs9dOW2m",
            "@Ugy3L+2}TMcjNb(cjVjck8WpL1s7>WTg+kRrl/uj",
            "@Ugy3L+2}TYg4BQJUjVjck61AvE^+Sb3b!rZ(7U~=V",
            "@Ugy3L+2}TYgOyvyviz?KiBDJYGs9dOW2m",
            "@Ugy3L+2}TYgjMogxi7Hg07IhPq4>b?9sX3@zs9y*",
            "@Ugy3L+2}TYg4BQJUjVjck61AvE^+Sb3b!rZ(7U~=V",
            "@Ugy3L+2}TYg%$yC%i7M2gZldO)@}cgb!l34$a-qf{00",
            "@Ugy3L+2}TYgT#^cvMir`2hg#I5@}cgb=Ak+@2XzZ/4gm",
            "@Ugy3L+2}TYgOyvyviz?KiBDJYKs9dOW2m",
            "@Ugy3L+2}TYgjMogxi7Hg07IhPq4>b?9sXeG%s9y*",
            "@Ugy3L+2}TYg4BQJUjVjck61AvE^+Sb3b!rc)7U~=V",
            "@Ugy3L+2}TYg%$yC%i7M2gZldO)@}cgb!l3q`a-qf{00",
            "@Ugy3L+2}TYgT#^cvMir`2hg#I5@}cgb=Ak-u2XzZ/4gm",
            "@Ug!pHG38o5YT`HzQ)h-nP",
            "@Uge8^+m/)}}!c178NkyuCbwKf>IWYh",
            "@Ug!pHG38o5YT`HzQ)h-nP",
            "@Ug!pHG38o5YZ7QZg)h-nP",
            "@Ug!pHG38o5YOe&^9)h-nP",
            "@Ug!pHG38o6@O)92A)h-nP",
            "@Ug!pHG38o5YPb#KC)h-nP",
            "@Ug!pHG38o5YMJlF2)h-nP",
            "@Ug!pHG38o4tO)92A)h-nP",
            "@Ug!pHG38o5Y4JxKV)h-nP",
            "@Ug!pHG38o5Y4JxKV)h-nP",
            "@Ug!pHG38o5YT`HzQ)k4)S6#x",
            "@Ug!pHG38o5YU8;7e00",
            "@Ug!pHG38o5YT`HzQ#Wbker2+r",
            "@Ug!pHG38o5YU20t_ra{#%6#x",
            "@Ug!pHG38o6DcBud",
            "@Ug!pHG38o6DP_;`100",
            "@Ug!pHG38o5YT>=",
            "@Ug!pHG38o5YT`HzQ)$V@)",
            "@Ugy3L+2}TYgOyvyviz?KiBDJYGs9dOW2m",
            "@Uge8^+m/)}}!c178NkyuCbwKf>IWYh",
            "@Uge8^+m/)}}!axR1DpKvM1BxF_41oav",
            "@Uge8aum/)}}!qkqSNDXRzG&iINder)8E{Op",
            "@Ugr$)Nm/)}}!YpV~ky;-O59uLV#F7vI",
            "@Ugr$!Lm/)}}!u<K5M>VQ_G&h6`+T9-j",
            "@Ugd_t@Fme!KdTvl?RG/_Tse7ors5+=wsFVl",
            "@Uge(J0Fme!Kux-$2RG}7is6<7oB&t$xP@zz<P`yy=5C",
            "@Uge8^+m/)}}!c178NkyuCbwKf>IWYh",
            "@Uge8^+m/)}}!axR1DpKvM1BxF_41oav",
            "@Ugr$WBm/$!m!X=5&qXq#",
            "@Ugr$WBm/$!m!X=5&qXxA;nj3Nj00",
            "@Ugr$WBm/$!m!X=5&qXxA;nj3OODgg",
            "@Ugr$WBm/$!m!X=5&qXxA;nj3OOD#<4R",
            "@Ugy3L+2}TYg%$yC%i7M2gZldO)@}cgb!l34$a-qf{00",
            "@Ugr$lG7-8sL(4z`<KALPY4GrpidjS",
            "@Ugr$rIm/)}}!q`oqNWCv7s8Ex7AI%h@D>DE",
            "@Uge8;)m/)}}!sxA_MZGU4Xi$ZEAI&bYFAo3"
        ];

        var decoder = new ItemSerialDecoder();
        var encoder = new ItemSerialEncoder();

        foreach (var serial in samples)
        {
            var partStr = decoder.DecodeAsPartsString(serial, debug: false);

            var reEncodedSerial = encoder.EncodeToSerial(partStr);

            Assert.AreEqual(serial, reEncodedSerial, true);
        }
    }

    [TestMethod]
    public void Test1()
    {
        var samples = new[]
        {
            new {
                Serial = "@Ugr%1Tm/)}}!qhvUNWCv7Xi/fEAI%M^D~HxV??W@l{7#!Lcvw#Vio<ewnf-;yzUH*Ne1FT=00",
                Expected = "303, 0, 1, 50| 2, 885|| {8} {247:76} {9} {1} {247:[23 181 7]} {21:[\"MAL_SM.part_barrel_02_firework\"]}|" //乱拼的
            },
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

        var decoder = new ItemSerialDecoder();
        var encoder = new ItemSerialEncoder();

        foreach (var sample in samples)
        {
            var partStr = decoder.DecodeAsPartsString(sample.Serial, debug: false);
            var reEncodedSerial = encoder.EncodeToSerial(partStr);

            Assert.AreEqual(sample.Serial, encoder.EncodeToSerial(partStr), true);

            Assert.IsTrue(Regex.IsMatch(sample.Expected, @"(((?:\d+|""[a-zA-Z0-9\._]+""|\\""[a-zA-Z0-9\._]+\\"")[ \t]*(?:,[ \t]*(?:\d+|""[a-zA-Z0-9\._]+""|\\""[a-zA-Z0-9\._]+\\""))*[ \t]*\|[ \t]*){2,}[ \t]*\|[ \t]*(""[a-zA-Z0-9\._]+""[ \t]*|\\""[a-zA-Z0-9\._]+\\""[ \t]*|\{[ \t]*\d+[ \t]*(?:[ \t]*:[ \t]*(?:\d+|\[[ \t]*(?:\d+|""[a-zA-Z0-9\._]+""|\\""[a-zA-Z0-9\._]+\\"")(?:[ \t]+(?:\d+|""[a-zA-Z0-9\._]+""|\\""[a-zA-Z0-9\._]+\\""))*[ \t]*\]))?[ \t]*\}[ \t]*)+[ \t]*(\|[ \t]*)?(((?:[ \t]*\d+|""[a-zA-Z0-9\._]+""|\\""[a-zA-Z0-9\._]+\\"")[ \t]*(?:,[ \t]*(?:\d+|""[a-zA-Z0-9\._]+""|\\""[a-zA-Z0-9\._]+\\""))*[ \t]*\|))?)"));
        }

        foreach (var sample in samples)
        {
            var reEncodedSerial = encoder.EncodeToSerial(sample.Expected);
            var partStr = decoder.DecodeAsPartsString(reEncodedSerial, debug: false);

            Assert.AreEqual(sample.Expected, partStr, true);
        }
    }

    [TestMethod]
    public void TestEncodeEdge()
    {
        var samples = new[]
        {
            new {
                Serial = "@Ugr%1Tm/)}}!qhvUNWCv7Xi/fEAI%N-p&4X;r%e/;ET@0PVY$4_{=#Hmb6Q@$zvb)Hl>-",
                Expected = "303, 0, 1, 50| 2, 885|| {8} {247:76} {9} {1} {247:[23 \"MAL_SM.part_barrel_02_firework\" 181 7]}|" //乱拼的
            },
            new {
                Serial = "@Ugr%1Tm/)}}!qhvUNWCv7Xi/fEAI%N-p&4X;r%e/;ET@0PVY$4_{=#Hmb6Q@$zvb)Hl>-",
                Expected = "303, 0, 1, 50| 2, 885|| {8} {247:76} {9} {1} {247:[23 \\\"MAL_SM.part_barrel_02_firework\\\" 181 7]}|" //乱拼的
            },
            new {
                Serial = "@UgxFw!2}TYgjNc48i7M2hN}^_>Vxm5E1~mtj2XzXS3Y7~L4s{O!",
                Expected = "    22, 0, 1, 50| 2, 3262||{67} {2} {5} {66} {73} {72} {15} {19} {25} {28}{35} {36} {44} {48}     {59}|     "
            },
            new {
                Serial = "@UgdhV<Fme!O0ue@92CYLCDp8FZHk1xk6LqOi-9*hng+h%&y+VaU{X_r",
                Expected = "8, 0, 1, 50| 10, 1| 2, 3170|| {53} {2} {4} {6} {1   :   13} {52} {74} {11} {15} {75} {25} {32} {33} {39} {48} {79}|"
            },
            new {
                Serial = "@Ugr%1Tm/)}}!qhvUNWCv7Xi/fEAI%M^D+d4",
                Expected = "303, 0, 1, 50| 2, 885|    | {8} {247:76} {9} {1} {247:   [   23 181 7    ]  } |"
            },
            new {
                Serial = "@Ug!pHG2}TZ*Od!Hk{GfKIO!YFT-3FEGpl%;j?$j-VsvQj+$sw}<",
                Expected = "254, 0, 1, 50| 9, 1| 2, 3973|| {54} {12} {302} {365} {428} {491} {236} {299} {    234  :  [48 25 83]   }|"
            },
            new {
                Serial = "@UgdhV<Fme!K>Q&G>RG/`esC1~Bs7ih6CTb2U6lxUe73vr2C*omj7y",
                Expected = "8, 0, 1, 50| 2, 2677|| {53} {2} {4} {3} {52} {74} {10} {15} {75} {25} {32} {33} {39} {47} {79}|\"c\", 12|"
            },
            new
            {
                Serial = "@Ugw$Yw2}TYg44elZMKj3!PMfaryAA)sYF~EwTy~Sg^8DAFmY45u`MN?a>QIduGTet/kolc9UGT7+{uPJi@-q7ilYPx;dHMd9uZxN5)E(3*R4LRg)H&1;4gd",
                Expected = "21, 0, 1, 50| 2, 1840|| \"MAL_SM.comp_05_legendary_firework\"{2} {5} {3} {6} {1:12} \"MAL_SM.part_barrel_02_firework\" {72} {14} {27} {35} {34} {43} {51} {1:48}|"
            }
        };

        var encoder = new ItemSerialEncoder();

        foreach (var sample in samples)
        {
            var reEncodedSerial = encoder.EncodeToSerial(sample.Expected);

            Assert.AreEqual(sample.Serial, reEncodedSerial, true);
            Assert.IsTrue(Regex.IsMatch(sample.Expected, @"(((?:\d+|""[a-zA-Z0-9\._]+""|\\""[a-zA-Z0-9\._]+\\"")[ \t]*(?:,[ \t]*(?:\d+|""[a-zA-Z0-9\._]+""|\\""[a-zA-Z0-9\._]+\\""))*[ \t]*\|[ \t]*){2,}[ \t]*\|[ \t]*(""[a-zA-Z0-9\._]+""[ \t]*|\\""[a-zA-Z0-9\._]+\\""[ \t]*|\{[ \t]*\d+[ \t]*(?:[ \t]*:[ \t]*(?:\d+|\[[ \t]*(?:\d+|""[a-zA-Z0-9\._]+""|\\""[a-zA-Z0-9\._]+\\"")(?:[ \t]+(?:\d+|""[a-zA-Z0-9\._]+""|\\""[a-zA-Z0-9\._]+\\""))*[ \t]*\]))?[ \t]*\}[ \t]*)+[ \t]*(\|[ \t]*)?(((?:[ \t]*\d+|""[a-zA-Z0-9\._]+""|\\""[a-zA-Z0-9\._]+\\"")[ \t]*(?:,[ \t]*(?:\d+|""[a-zA-Z0-9\._]+""|\\""[a-zA-Z0-9\._]+\\""))*[ \t]*\|))?)"));
        }
    }
}
