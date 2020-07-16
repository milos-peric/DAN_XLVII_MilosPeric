using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using System.Text;
using System.Threading;
using System.Threading.Tasks;

namespace DAN_XLVII_MilosPeric
{
    class Program
    {
        static void Main(string[] args)
        {
            Vehicle vehicle = new Vehicle();
            Thread t = new Thread(new ThreadStart(vehicle.StartVehicle));
            t.Start();
            t.Join();
            Console.ReadKey();
        }
    }
}
