using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace NHiLo.SqlSequence.QuickTestExample
{
    /// <summary>
    /// This is just a simple console app used to test the SQL Server sequence implementation.
    /// </summary>
    class Program
    {
        static void Main(string[] args)
        {
            var factory = new HiLoGeneratorFactory();
            var generator = factory.GetKeyGenerator("myEntity");
            Console.WriteLine(generator.GetKey());
            Console.WriteLine(generator.GetKey());
            Console.WriteLine(generator.GetKey());
        }
    }
}
