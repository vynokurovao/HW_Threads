using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading;
using System.Threading.Tasks;

namespace HW_Threads_task2
{
    class Program
    {
        static void Main(string[] args)
        {
            Object lockObject = new Object();
            for (int i = 0; i < Console.WindowWidth; i++)
            {
                ChainDisplayer cd = new ChainDisplayer(lockObject, i);
                Thread thread = new Thread(_ => cd.Display());
                thread.Start();
            }

            Console.ReadKey();
        }
    }
}
