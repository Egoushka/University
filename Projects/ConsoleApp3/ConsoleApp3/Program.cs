using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ConsoleApp3
{
    internal class Person : IDisposable
    {
        private bool _isDispose = false;

        public void Dispose()
        {
            if (!_isDispose)
            {
                _isDispose = true;
                Dispose(_isDispose);
            }
            GC.SuppressFinalize(this);
        }

        protected virtual void Dispose(bool disposing)
        {
            if (disposing)
            {
                return;
            }
            /*
             *
             *
             */
        }
    }

    internal class Program
    {
        private static void Main(string[] args)
        {
            byte[] bytes = new byte[0];
            for (int i = 10000; i < 1000000; i += 10000)
            {
                bytes = new byte[i];
                if (GC.GetGeneration(bytes).ToString() == "2")
                {
                    Console.ReadKey();
                    return;
                }
            }
            Console.WriteLine();
        }
    }
}