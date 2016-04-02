using nao20010128nao.MCPing.PE;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace MCPing.NET.Test
{
    class Program
    {
        static void Main(string[] args)
        {
            Console.WriteLine(new PEQuery("127.0.0.1", 19132).FullStat().Data["hostname"]);
            Console.ReadKey();
        }
    }
}
