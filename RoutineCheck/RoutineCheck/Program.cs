using System;
using System.Threading;

namespace RoutineCheck
{
    internal class Program
    {
        static void Main(string[] args)
        {
            while (true)
            {
                Routine.Initialize();
            }
        }
    }
}
