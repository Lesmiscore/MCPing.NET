using System;
using System.IO;
using System.Net;
using System.Net.Sockets;
using System.Text;

namespace nao20010128nao.MCPing.PE
{
    public class UnconnectedPing
    {
        public const byte UCP_PID = 0x01;
        public const ulong MAGIC_1ST = 0x00ffff00fefefefeL;
        public const ulong MAGIC_2ND = 0xfdfdfdfd12345678L;

        public static UnconnectedPingResult DoPing(String ip, int port)
        {
            UdpClient ds = null;
            try
            {
                ds = new UdpClient();
                MemoryStream baos = new MemoryStream(25);
                baos.WriteByte(UCP_PID);
                baos.Write(BitConverter.GetBytes(DateTime.Now.ToBinary()), 0, 8);
                baos.Write(BitConverter.GetBytes(MAGIC_1ST), 0, 8);
                baos.Write(BitConverter.GetBytes(MAGIC_2ND), 0, 8);
                ds.Send(baos.ToArray(), (int)baos.Length, new IPEndPoint(IPAddress.Parse(ip), port));

                IPEndPoint ep = null;
                byte[] recv = ds.Receive(ref ep);

                MemoryStream dis = new MemoryStream(recv);
                if (dis.ReadByte() != 0x1c)
                {
                    throw new IOException("Server replied with invalid data");
                }
                byte[] longBuf = new byte[8];
                dis.Read(longBuf, 0, 8);// Ping ID
                dis.Read(longBuf, 0, 8);// Server ID
                dis.Read(longBuf, 0, 8);// MAGIC
                dis.Read(longBuf, 0, 8);// MAGIC
                dis.Read(longBuf, 0, 2);//Length
                short slen = BitConverter.ToInt16(longBuf, 0);
                byte[] readBuf = new byte[slen];
                dis.Read(readBuf, 0, slen);
                String s = new UTF8Encoding().GetString(readBuf);
                return new UnconnectedPingResult(s);
            }
            finally
            {
                if (ds != null)
                    ds.Close();
            }
        }

        public class UnconnectedPingResult : ServerPingResult
        {
            String[] serverInfos;
            String raw;


            public UnconnectedPingResult(String s)
            {
                serverInfos = (raw = s).Split(';');
            }

            public String ServerName
            {
                get
                {
                    return serverInfos[1];
                }
            }

            public String Raw
            {
                get
                {
                    return raw;
                }
            }
        }
    }
}
