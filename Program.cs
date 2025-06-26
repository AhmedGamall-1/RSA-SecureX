using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace BigIntegerProject
{
    internal class FileName
    {
        public static void Main(string[] args)
        {
        
            /*int num = Convert.ToInt32(Console.ReadLine());

            for (int i = 0; i < num; i++)
            {
                try
                {
                    string strA = Console.ReadLine()?.Trim();
                    string strB = Console.ReadLine()?.Trim();

                    if (string.IsNullOrWhiteSpace(strA) || string.IsNullOrWhiteSpace(strB))
                    {
                        Console.WriteLine($"Test Case {i + 1}: Invalid input. Skipping...");
                        Console.WriteLine("---------------------");
                        continue;
                    }

                    BigInteger a = new BigInteger(strA);
                    BigInteger b = new BigInteger(strB);

                    Stopwatch stopwatch = new Stopwatch();

                    stopwatch.Start();
                    BigInteger result = BigInteger.Add(a, b); 
                    stopwatch.Stop();

                    Console.WriteLine($"Test Case {i + 1} Output: {result}");
                    Console.WriteLine($"Time Taken: {stopwatch.Elapsed.TotalMilliseconds:F4} ms");
                    Console.WriteLine("---------------------");
                }
                catch (Exception ex)
                {
                    Console.WriteLine($"Test Case {i + 1}: Error occurred - {ex.Message}");
                    Console.WriteLine("---------------------");
                }

            } */

            int num;
            num = Convert.ToInt32(Console.ReadLine());
            for (int i = 0; i < num; i++)
            {
                Stopwatch stopwatch = new Stopwatch();
                string n, key, func;
                int option; BigInteger r;
                n = Console.ReadLine();
                key = Console.ReadLine();
                func = Console.ReadLine();
                option = Convert.ToInt32(Console.ReadLine());
                //int time_before = System.Environment.TickCount;
                stopwatch.Start();
                if (option == 0) { r = RSA.EncryptDecrypt(new BigInteger(func), new BigInteger(key), new BigInteger(n)); }
                else { r = RSA.EncryptDecrypt(new BigInteger(func), new BigInteger(key), new BigInteger(n)); }
                stopwatch.Stop();
                //int time_after = System.Environment.TickCount;
                Console.Write("Output : ");
                r.ToOriginalString();
                Console.WriteLine(r);
                Console.Write("Time Taken : " + (stopwatch.Elapsed.TotalMilliseconds) + " ms");
                Console.WriteLine();
                Console.WriteLine("---------------------");


            } 


        }
    }
}