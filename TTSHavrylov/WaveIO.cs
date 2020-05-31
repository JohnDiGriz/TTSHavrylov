
using System;
using System.Collections.Generic;
using System.Text;
using System.IO;
using System.Runtime.InteropServices;
using System.Linq;

namespace TTSHavrylov
{
    namespace Concatenation_Waves
    {
        class WaveIO
        {
            public int length;
            public short channels;
            public int samplerate;
            public int DataLength;
            public short BitsPerSample;

            private void WaveHeaderIN(byte[] wave)
            {
                MemoryStream ms = new MemoryStream(wave);

                BinaryReader br = new BinaryReader(ms);
                length = (int)ms.Length - 8;
                ms.Position = 22;
                channels = br.ReadInt16();
                ms.Position = 24;
                samplerate = br.ReadInt32();
                ms.Position = 34;

                BitsPerSample = br.ReadInt16();
                DataLength = (int)ms.Length - 44;
                br.Close();
                ms.Close();

            }

            private byte[] WaveHeaderOUT()
            {
                byte[] wave = new byte[44];
                MemoryStream ms = new MemoryStream(wave);

                BinaryWriter bw = new BinaryWriter(ms);
                ms.Position = 0;
                bw.Write(new char[4] { 'R', 'I', 'F', 'F' });

                bw.Write(length);

                bw.Write(new char[8] { 'W', 'A', 'V', 'E', 'f', 'm', 't', ' ' });

                bw.Write((int)16);

                bw.Write((short)1);
                bw.Write(channels);

                bw.Write(samplerate);

                bw.Write((int)(samplerate * ((BitsPerSample * channels) / 8)));

                bw.Write((short)((BitsPerSample * channels) / 8));

                bw.Write(BitsPerSample);

                bw.Write(new char[4] { 'd', 'a', 't', 'a' });
                bw.Write(DataLength);
                bw.Close();
                ms.Close();
                return wave;
            }
            public static byte[] Merge(List<byte[]> files)
            {
                WaveIO wa_IN = new WaveIO();
                WaveIO wa_out = new WaveIO();

                wa_out.DataLength = 0;
                wa_out.length = 0;


                //Gather header data
                foreach (byte[] file in files)
                {
                    wa_IN.WaveHeaderIN(file);
                    wa_out.DataLength += wa_IN.DataLength;
                    wa_out.length += wa_IN.length;

                }
                byte[] outFile = new byte[wa_out.DataLength];
                //Recontruct new header
                wa_out.BitsPerSample = wa_IN.BitsPerSample;
                wa_out.channels = wa_IN.channels;
                wa_out.samplerate = wa_IN.samplerate;
                outFile=wa_out.WaveHeaderOUT().Concat(outFile).ToArray();
                var pos = 44;
                foreach (byte[] file in files)
                {
                    MemoryStream ms = new MemoryStream(file);
                    byte[] arrfile = new byte[ms.Length - 44];
                    ms.Position = 44;
                    ms.Read(arrfile, 0, arrfile.Length);
                    ms.Close();

                    ms = new MemoryStream(outFile);
                    ms.Position = pos;
                    BinaryWriter bw = new BinaryWriter(ms);
                    bw.Write(arrfile);
                    bw.Close();
                    pos += arrfile.Length;
                    ms.Close();
                }
                return outFile;
            }


        }
    }
}
