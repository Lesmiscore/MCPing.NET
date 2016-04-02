using System;
using System.IO;

namespace nao20010128nao.MCPing.PE
{
    public class Request
    {
        MemoryStream byteStream;

        static byte[] MAGIC = { 0xFE, 0xFD };
        public byte type;
        public int sessionID;
        public byte[] payload;

        public Request()
        {
            int size = 1460;
            byteStream = new MemoryStream(size);
        }

        public Request(byte type)
        {
            this.type = type;
            // TODO move static type variables to Request
        }

        public byte[] ToBytes()
        {
            byteStream.SetLength(0);

            byteStream.Write(MAGIC, 0, 2);
            byteStream.WriteByte(type);
            byteStream.Write(BitConverter.GetBytes(sessionID), 0, 4);
            byte[] payload = payloadBytes();
            byteStream.Write(payload, 0, payload.Length);

            return byteStream.ToArray();
        }

        private byte[] payloadBytes()
        {
            if (type == PEQuery.HANDSHAKE)
            {
                return new byte[] { }; // return empty byte array
            }
            else // (type == MCQuery.STAT)
            {
                return payload;
            }
        }

        public void setPayload(int load)
        {
            this.payload = Utils.intToBytes(load);
        }
    }
}
