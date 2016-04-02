using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace nao20010128nao.MCPing.PE
{
    public class BasicStat : ServerPingResult
    {
        static byte NULL = 00;
        static byte SPACE = 20;

        // for simple stat
        private String motd, gameMode, mapName;
        private int onlinePlayers, maxPlayers;
        private short port;
        private String hostname;

        public BasicStat(byte[] data)
        {
            data = Utils.trim(data);
            byte[][] temp = Utils.split(data);

            motd = new UTF8Encoding().GetString(Utils.subarray(temp[0], 1, temp[0].Length - 1));
            gameMode = new UTF8Encoding().GetString(temp[1]);
            mapName = new UTF8Encoding().GetString(temp[2]);
            onlinePlayers = int.Parse(new UTF8Encoding().GetString(temp[3]));
            maxPlayers = int.Parse(new UTF8Encoding().GetString(temp[4]));
            port = Utils.bytesToShort(temp[5]);
            hostname = new UTF8Encoding().GetString(Utils.subarray(temp[5], 2, temp[5].Length - 1));

        }


        public override String ToString()
        {
            String delimiter = ", ";
            StringBuilder str = new StringBuilder();
            str.Append(motd);
            str.Append(delimiter);
            str.Append(gameMode);
            str.Append(delimiter);
            str.Append(mapName);
            str.Append(delimiter);
            str.Append(onlinePlayers);
            str.Append(delimiter);
            str.Append(maxPlayers);
            str.Append(delimiter);
            str.Append(port);
            str.Append(delimiter);
            str.Append(hostname);
            return str.ToString();
        }

        public String getMOTD()
        {
            return motd;
        }

        public String getGameMode()
        {
            return gameMode;
        }

        public String getMapName()
        {
            return mapName;
        }

        public int getOnlinePlayers()
        {
            return onlinePlayers;
        }

        public int getMaxPlayers()
        {
            return maxPlayers;
        }
    }
}
