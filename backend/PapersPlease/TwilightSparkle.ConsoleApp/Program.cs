using System;
using System.Threading;

namespace TwilightSparkle.ConsoleApp
{
    public class ServerClass
    {
        private static int _count = 0;


        public void InstanceMethod()
        {
            Console.WriteLine("ServerClass.InstanceMethod is running on another thread.");

            var data = _count++;
            Thread.Sleep(3000);
            Console.WriteLine("The instance method called by the worker thread has ended. " + data);
        }
    }



    public class Simple
    {
        public static void Main()
        {
            var a = 0;
            var b = 0;
            while (true)
            {
                a++;
                Thread.Sleep(5000);
                if (a == 5)
                {
                    Console.WriteLine("Hello");
                    b++;
                    a = 0;
                }
            }



            //for (var i = 0; i < 10; i++)
            //{
            //    CreateThreads();
            //}
        }

        public static void CreateThreads()
        {
            var serverObject = new ServerClass();

            var instanceCaller = new Thread(serverObject.InstanceMethod);
            instanceCaller.Start();

            Console.WriteLine("The Main() thread calls this after starting the new InstanceCaller thread.");
        }
    }
}