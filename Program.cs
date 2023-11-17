namespace Threads
{
    public class SharedRecources
    {
        public static int SharedResouce { get; set; }

        public static readonly object lockObject = new();
    }

    class NumberUpCounter
    {
        public int Count { get; set; }

        public void CountUp(Action<long> callBack)
        {
            long sum = 0;

            try
            {
                Console.WriteLine("Count up started");
                Thread.Sleep(1000);

                for (int i = 1; i <= Count; i++)
                {
                    sum += i;

                    lock (SharedRecources.lockObject)
                    {
                        Console.WriteLine($"Shared resource is Count-Up: {SharedRecources.SharedResouce}, ");
                        SharedRecources.SharedResouce++;
                    }

                    Console.ForegroundColor = ConsoleColor.Green;
                    Console.WriteLine($"i = {i}, ");
                    Thread.Sleep(100);
                }
            }
            catch (Exception)
            {
                throw;
            }
            finally
            {
                callBack(sum);
            }
        }
    }

    class NumberDownCounter
    {
        public int Count { get; set; }

        public void CounDown(Action<long> callBack)
        {
            long sum = 0;

            try
            {
                Console.WriteLine("Count down started");
                Thread.Sleep(1000);

                for (int j = Count; j >= 1; j--)
                {
                    lock (SharedRecources.lockObject) 
                    {
                        Console.WriteLine($"Shared resource is Count-Down: {SharedRecources.SharedResouce}, ");
                        SharedRecources.SharedResouce--;
                    }
                    
                    Monitor.Exit(SharedRecources.lockObject);
                    Console.ForegroundColor = ConsoleColor.Red;
                    Console.WriteLine($"j = {j}, ");
                    Thread.Sleep(100);
                }
            }
            catch (Exception)
            {
                throw;
            }
            finally
            {
                callBack(sum);
            }
        }
    }

    internal class Program
    {
        static void Main(string[] args)
        {
            NumberDownCounter counterDown = new() { Count = 100 };
            NumberUpCounter counterUp = new() { Count = 100 };

            void callBack(long sum)
            {
                Console.WriteLine($"Return value from is sum {sum}");
            }

            ThreadStart thread = new(() => { counterDown.CounDown(callBack); });
            ThreadStart thread1 = new(() => { counterUp.CountUp(callBack); });
            Thread th1 = new(thread);
            Thread th2 = new(thread1);
            th2.Start();
            th1.Start();
        }
    }
}