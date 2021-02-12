/* Program 1: Lg Lg n
 * Peter Peng, cp47
 * CS-212-B, Professor Plantinga, 9/2020
 */
using System;

namespace program1
{
    class Program
    {
        static void Main(string[] args)
        {
            // title
            Console.WriteLine("Floor (lg lg n) Computation");
     
            while (true)
            {
                // prompt user to enter number
                Console.Write("\nEnter n: ");
                float n = float.Parse(Console.ReadLine());
                // output calculated result
                Console.WriteLine("Floor (lg lg " + n + ") = " + Lg(Lg(n)));
            }
        }
        static long Lg(float n)
        {
            int count = 0;
            // devide n by 2 until n is smaller than 2
            // count the divisions
            while (n >= 2)
            {
                n = n / 2;
                count++;
            }
            return count;
        }
    }
}
