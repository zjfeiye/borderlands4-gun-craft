using Borderlands4.ItemSerialCodec;

namespace ItemSerialCodec.MSTests;

[TestClass]
public sealed class SerialCodecTests
{
    [TestMethod]
    public void TestEncoder()
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
            var partStr = decoder.DecodeAsString(serial, debug: false);
            var reEncodedSerial = encoder.EncodeToSerial(partStr);

            Assert.AreEqual(serial, reEncodedSerial, true);
        }
    }
}
