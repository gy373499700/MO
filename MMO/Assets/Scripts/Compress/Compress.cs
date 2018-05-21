using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.IO; 
using SevenZip; 

public class GD
{
    public static void Compress(Stream src, Stream dst)
    {
        /* 压缩 */
        CoderPropID[] propIDs = {
                                    CoderPropID.DictionarySize, 
                                    CoderPropID.PosStateBits, 
                                    CoderPropID.LitContextBits, 
                                    CoderPropID.LitPosBits, 
                                    CoderPropID.Algorithm, 
                                    CoderPropID.NumFastBytes, 
                                    CoderPropID.MatchFinder,
                                    CoderPropID.EndMarker }; 
        object[] encode_properties = { 
                                         1 << 23, 
                                         2, 
                                         3, 
                                         0, 
                                         2, 
                                         128, 
                                         "bt4",
                                         false };
        SevenZip.Compression.LZMA.Encoder encoder = new SevenZip.Compression.LZMA.Encoder(); 
        encoder.SetCoderProperties(propIDs, encode_properties);

        encoder.WriteCoderProperties(dst);

        for (int i = 0; i < 8; i++) { 
            dst.WriteByte((Byte)(src.Length >> (8 * i)));
        }
        encoder.Code(src, dst, -1, -1, /*p*/null); 

    }
    public static void Decompress(Stream src, Stream dst)
    {
        byte[] decode_properties = new byte[5];         
        int n = src.Read(decode_properties, 0, 5);        
        if (n != 5)         
        {             
            Console.WriteLine("read encode_properties error.");            
            return;         
        } 

        SevenZip.Compression.LZMA.Decoder decoder = new SevenZip.Compression.LZMA.Decoder();
        decoder.SetDecoderProperties(decode_properties);

        long outSize = 0; 
        for (int i = 0; i < 8; i++) 
        { 
            int v = src.ReadByte(); 
            if (v < 0) 
            { 
                Console.WriteLine("read outSize error."); 
                return; 
            } 
            outSize |= ((long)(byte)v) << (8 * i); 
        }
        long compressedSize = src.Length - src.Position;
        decoder.Code(src, dst, compressedSize, outSize, null); 
    }
}

