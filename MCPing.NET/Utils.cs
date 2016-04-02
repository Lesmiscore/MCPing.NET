using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace nao20010128nao.MCPing
{
    public class Utils
    {
        public static readonly byte PACKET_HANDSHAKE = 0x00, PACKET_STATUSREQUEST = 0x00, PACKET_PING = 0x01;
        public static readonly int PROTOCOL_VERSION = 4;
        public static readonly int STATUS_HANDSHAKE = 1;

        public static void validate(object o, string m)
        {
            if (o == null)
            {
                throw new Exception(m);
            }
        }

        public static void io(bool b, string m)
        {
            if (b)
            {
                throw new IOException(m);
            }
        }

        public static int readVarInt(Stream strm)
        {

            int i = 0;
            int j = 0;
            while (true)
            {
                int k = strm.ReadByte();
                if (k == -1) continue;
                i |= (k & 0x7F) << j++ * 7;
                if (j > 5)
                    throw new Exception("VarInt too big");
                if ((k & 0x80) != 128)
                    break;
            }
            return i;
        }

        public static void writeVarInt(Stream strm, int paramInt)
        {
            while (true)
            {
                if ((paramInt & 0xFFFFFF80) == 0)
                {
                    strm.WriteByte((byte)(paramInt & 0xff));
                    return;
                }

                strm.WriteByte((byte)(paramInt & 0x7F | 0x80));
                paramInt = paramInt >> 7;
            }
        }

        public static byte[] subarray(byte[] buf, int a, int b)
        {
            if (b - a > buf.Length)
                return buf;// TODO better error checking

            byte[] result = new byte[b - a + 1];

            for (int i = a; i <= b; i++)
            {
                result[i - a] = buf[i];
            }
            return result;
        }

        public static byte[] trim(byte[] arr)
        {
            if (arr[0] != 0 && arr[arr.Length - 1] != 0)
                return arr; // return the input if it has no leading/trailing null
                            // bytes

            int begin = 0, end = arr.Length;
            for (int i = 0; i < arr.Length; i++) // find the first non-null byte
            {
                if (arr[i] != 0)
                {
                    begin = i;
                    break;
                }
            }
            for (int i = arr.Length - 1; i >= 0; i--) // find the last non-null byte
            {
                if (arr[i] != 0)
                {
                    end = i;
                    break;
                }
            }

            return subarray(arr, begin, end);
        }

        public static byte[][] split(byte[] input)
        {
            List<byte[]> temp = new List<byte[]>();

            int index_cache = 0;
            for (int i = 0; i < input.Length; i++)
            {
                if (input[i] == 0x00)
                {
                    // output[out_index++] = subarray(input, index_cache, i-1);
                    // //store the array from the last null byte to the current one
                    byte[] b = subarray(input, index_cache, i - 1);
                    temp.Add(b);
                    index_cache = i + 1;// note, this is the index *after* the null
                                        // byte
                }
            }
            // get the remaining part
            if (index_cache != 0) // prevent duplication if there are no null bytes
            {
                // output[out_index] = subarray(input, index_cache, input.length-1);
                byte[] b = subarray(input, index_cache, input.Length - 1);
                temp.Add(b);
            }
            return temp.ToArray();
        }

        public static byte[] padArrayEnd(byte[] arr, int amount)
        {
            byte[] arr2 = new byte[arr.Length + amount];
            for (int i = 0; i < arr.Length; i++)
            {
                arr2[i] = arr[i];
            }
            for (int i = arr.Length; i < arr2.Length; i++)
            {
                arr2[i] = 0;
            }
            return arr2;
        }

        public static short bytesToShort(byte[] b)
        {
            // Little endian
            return (short)((b[0] | b[1] << 8) & 0xffff);
        }

        public static byte[] intToBytes(int val)
        {
            return BitConverter.GetBytes(val);
        }

        public static int bytesToInt(byte[] val)
        {
            return val[0] << 24 | val[1] << 16 | val[2] << 8 | val[3] << 0;
        }
    }
}
