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
        private readonly string[] moveDirection = { "North", "South" };
        private Random random = new Random();
        private readonly object objLock = new object();
        private delegate void Del(string message);
        private Del messageHandler = DelegateMethod;
        private List<string> orderList = new List<string>();
        private Semaphore semaphore = new Semaphore(1, 15);
        private Stopwatch stwch = new Stopwatch();
        private int numberOfIterations;
        private int counter = 0;
        
        /// <summary>
        /// Designated method for delegate use.
        /// In this class all messages are written by assigning this method to delegate Del
        /// </summary>
        /// <param name="message">Message to print using Console.Writeline().</param>
        public static void DelegateMethod(string message)
        {
            Console.WriteLine(message);
        }

        /// <summary>
        /// Starts threads in for loop, using random number of iterations 1-15, each thread is assigned random name North or South.
        /// Thread start order is saved in orderList.
        /// </summary>
        public void StartVehicle()
        {
            numberOfIterations = random.Next(1, 16);
            messageHandler($"Total number of vehicles: {numberOfIterations}");
            for (int i = 0; i < numberOfIterations; i++)
            {
                Thread tTruck = new Thread(new ParameterizedThreadStart(Worker));
                tTruck.Name = $"{moveDirection[random.Next(0, 2)]}";
                orderList.Add(tTruck.Name);
                tTruck.Start(i);
            }
            orderList.Add(" ");
        }

        /// <summary>
        /// Method is used by threads created in for loop to print number of vehicle and direction moving.
        /// </summary>
        /// <param name="obj">Number of thread starting the method</param>
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
        /// <summary>
        /// After all threads start they call this method. Semaphore is used to synchronize thread execution. 
        /// If direction is same next treads enters critical section instantly, otherwise there is 500ms delay,
        /// to represent change of direction of wehicles crossing the bridge.
        /// </summary>
        /// <param name="vehichleNumber">Number of thread entering this method.</param>
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
                messageHandler("Program execution time: " + stwch.Elapsed.TotalSeconds + " seconds.");
            }
        }
    }
}
