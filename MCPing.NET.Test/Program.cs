using nao20010128nao.MCPing.PC;
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
            Console.WriteLine(((Reply)new PCQuery("hanageserver.com", 25565).fetchReply()).description);
            Console.ReadKey();
        }
    }
}
