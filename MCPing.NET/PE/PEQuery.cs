using System;
using System.Text;
using System.Net.Sockets;
using System.IO;
using System.Net;

namespace nao20010128nao.MCPing.PE
{
    public class PEQuery
    {
        public const byte HANDSHAKE = 9;
        public const byte STAT = 0;
        String serverAddress = "localhost";
        int queryPort = 25565;
        int localPort = 25566;
        private int token;
        private UdpClient socket;

        public PEQuery(String address, int port)
        {
            serverAddress = address;
            queryPort = port;
        }

        private void Handshake()
        {
            Request req = new Request();
            req.type = HANDSHAKE;
            req.sessionID = GenerateSessionID();

            int val = 11 - req.ToBytes().Length; // should be 11 bytes total
            byte[] input = Utils.padArrayEnd(req.ToBytes(), val);
            byte[] result = SendUDP(input);

            token = int.Parse(new UTF8Encoding().GetString(result).Trim());
        }

        public BasicStat BasicStat()
        {
            Handshake(); // get the session token first

            Request req = new Request(); // create a request
            req.type = STAT;
            req.sessionID = GenerateSessionID();
            req.setPayload(token);
            byte[] send = req.ToBytes();

            byte[] result = SendUDP(send);

            BasicStat res = new BasicStat(result);
            return res;
        }

        public FullStat FullStat()
        {
            // basicStat() calls handshake()
            // QueryResponse basicResp = this.basicStat();
            // int numPlayers = basicResp.onlinePlayers; //TODO use to determine max
            // length of full stat

            Handshake();

            Request req = new Request();
            req.type = STAT;
            req.sessionID = GenerateSessionID();
            req.setPayload(token);
            req.payload = Utils.padArrayEnd(req.payload, 4);

            byte[] send = req.ToBytes();

            byte[] result = SendUDP(send);

            /*
             * note: buffer size = base + #players(online) * 16(max username length)
             */

            FullStat res = new FullStat(result);
            return res;
        }

        private byte[] SendUDP(byte[] input)
        {
            try
            {
                while (socket == null)
                {
                    try
                    {
                        socket = new UdpClient(localPort);
                    }
                    catch (IOException)
                    {
                        ++localPort;
                    }
                }

                socket.Send(input, input.Length, new IPEndPoint(IPAddress.Parse(serverAddress), queryPort));

                IPEndPoint ep = null;
                return socket.Receive(ref ep);
            }
            catch (SocketException)
            {

            }
            catch (Exception)
            {

            }

            return null;
        }

        private int GenerateSessionID()
        {
            return 1;
        }

        ~PEQuery()
        {
            socket.Close();
        }
    }
}
