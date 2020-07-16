using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using System.Text;
using System.Threading;
using System.Threading.Tasks;

namespace DAN_XLVII_MilosPeric
{
    internal class Vehicle
    {
        public readonly string[] moveDirection = { "North", "South" };
        public Random random = new Random();
        private readonly object objLock = new object();
        public delegate void Del(string message);
        public Del messageHandler = DelegateMethod;
        List<string> orderList = new List<string>();
        Semaphore semaphore = new Semaphore(1, 15);
        Stopwatch stwch = new Stopwatch();
        int numberOfIterations;
        int counter = 0;
        

        public static void DelegateMethod(string message)
        {
            Console.WriteLine(message);
        }

        public void StartVehicle()
        {
            numberOfIterations = random.Next(1, 16);
            Console.WriteLine($"Total number of vehicles: {numberOfIterations}");
            for (int i = 0; i < numberOfIterations; i++)
            {
                Thread tTruck = new Thread(new ParameterizedThreadStart(Worker));
                tTruck.Name = $"{moveDirection[random.Next(0, 2)]}";
                orderList.Add(tTruck.Name);
                tTruck.Start(i);
            }
            orderList.Add(" ");
        }

        public void Worker(object obj)
        {        
            lock (objLock)
            {
                messageHandler($"Vehicle number {(int)obj + 1} started in direction {Thread.CurrentThread.Name}");               
            }
            Thread.Sleep(500);
            stwch.Start();
            CrossBridge((int)obj);                           
        }

        public void CrossBridge(int vehichleNumber)
        {
            semaphore.WaitOne();
            messageHandler($"Vehicle number {vehichleNumber + 1} has crossed the bridge - {Thread.CurrentThread.Name}.");
            if (orderList.ElementAt(vehichleNumber + 1) == Thread.CurrentThread.Name)
            {
                semaphore.Release();
                counter++;
            }
            else
            {
                Thread.Sleep(500);
                semaphore.Release();
                counter++;
            }
            if (counter == numberOfIterations)
            {
                Console.WriteLine("Program execution time: " + stwch.Elapsed.TotalSeconds + " seconds.");
            }
        }
    }
}
