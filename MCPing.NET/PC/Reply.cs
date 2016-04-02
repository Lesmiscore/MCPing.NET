using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace nao20010128nao.MCPing.PC
{
    [JsonObject("reply")]
    public class Reply : PCQueryResult
    {
        [JsonProperty("description")]
        public String description;
        [JsonProperty("players")]
        public Players players;
        [JsonProperty("version")]
        public Version version;
        [JsonProperty("favicon")]
        public String favicon;

        [JsonObject("players")]
        public class Players
        {
            [JsonProperty("max")]
            public int max;
            [JsonProperty("online")]
            public int online;
            [JsonProperty("sample")]
            public List<Player> sample;
        }

        [JsonObject("player")]
        public class Player
        {
            [JsonProperty("name")]
            public String name;
            [JsonProperty("id")]
            public String id;
        }

        [JsonObject("version")]
        public class Version
        {
            [JsonProperty("name")]
            public String name;
            [JsonProperty("protocol")]
            public int protocol;
        }
    }
}
