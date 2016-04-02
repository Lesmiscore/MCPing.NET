using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Net.Sockets;
using System.Text;
using System.Threading.Tasks;

namespace nao20010128nao.MCPing.PC
{
    public class PCQuery
    {
        private String host;
        private int port;

        public PCQuery(String host, int port)
        {
            this.host = host;
            this.port = port;
        }

        /* handshake->Request->statJson->ping */
        private void writeHandshake(Stream strm, String host, int port)
        {
            MemoryStream handshake_bytes = new MemoryStream();

            handshake_bytes.WriteByte(Utils.PACKET_HANDSHAKE);
            Utils.writeVarInt(handshake_bytes, Utils.PROTOCOL_VERSION);
            byte[] hostBytes = new ASCIIEncoding().GetBytes(host);
            Utils.writeVarInt(handshake_bytes, hostBytes.Length);
            handshake_bytes.Write(hostBytes, 0, hostBytes.Length);
            handshake_bytes.Write(BitConverter.GetBytes((short)port), 0, 2);
            Utils.writeVarInt(handshake_bytes, Utils.STATUS_HANDSHAKE);

            Utils.writeVarInt(strm, (int)handshake_bytes.Length);
            handshake_bytes.CopyTo(strm);
        }

        private void writeRequest(Stream strm)
        {
            strm.WriteByte(0x01); // Size of packet
            strm.WriteByte(Utils.PACKET_STATUSREQUEST);
        }

        private String getStatJson(Stream strm)
        {
            Utils.readVarInt(strm); // Size
            int id = Utils.readVarInt(strm);

            Utils.io(id == -1, "Server prematurely ended stream.");
            Utils.io(id != Utils.PACKET_STATUSREQUEST,
                    "Server returned invalid packet.");

            int length = Utils.readVarInt(strm);
            Utils.io(length == -1, "Server prematurely ended stream.");
            Utils.io(length == 0, "Server returned unexpected value.");

            byte[] data = new byte[length];
            strm.Read(data, 0, length);
            String json = new UTF8Encoding().GetString(data);
            return json;
        }

        private void doPing(Stream strmOut, Stream strmIn)
        {

            strmOut.WriteByte(0x09);
            strmOut.WriteByte(Utils.PACKET_PING);
            strmOut.Write(BitConverter.GetBytes(DateTime.Now.ToBinary()), 0, 8);

            Utils.readVarInt(strmIn); // Size
            int id = Utils.readVarInt(strmIn);
            Utils.io(id == -1, "Server prematurely ended stream.");
            Utils.io(id != Utils.PACKET_PING, "Server returned invalid packet.");
        }

        // ///////
        public PCQueryResult fetchReply()
        {
            TcpClient sock = null;
            try
            {
                sock = new TcpClient(host, port);
                Stream strm = sock.GetStream();
                writeHandshake(strm, host, port);
                writeRequest(strm);
                String s = getStatJson(strm);
                try
                {
                    return JsonConvert.DeserializeObject<Reply>(s);
                }
                catch (Exception)
                {
                    return JsonConvert.DeserializeObject<Reply19>(s);
                }
            }
            finally
            {
                if (sock != null)
                    sock.Close();
            }
        }

        public void doPingOnce()
        {
            TcpClient sock = null;
            try
            {
                sock = new TcpClient(host, port);
                doPing(sock.GetStream(), sock.GetStream());
            }
            finally
            {
                if (sock != null)
                    sock.Close();
            }
        }
    }
}
