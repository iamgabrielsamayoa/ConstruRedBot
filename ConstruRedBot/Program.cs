using System;
using System.Threading.Tasks;

namespace ConstruRedBot
{
    class Program
    {
        public static async Task Main()
        {
            await new ClsBotCR().StartTelegram();
            
        }
            
       
    }
}
