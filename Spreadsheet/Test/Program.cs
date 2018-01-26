using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Formulas;

namespace Test
{
    class Program
    {
        static void Main(string[] args)
        {
            Formula f = new Formula("(adam + davies) + l0ves + Anneke + 3");

            Console.WriteLine();
            Console.WriteLine("It worked!");
            Console.ReadKey(true);
        }
    }
}
