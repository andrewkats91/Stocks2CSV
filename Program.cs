using System;

namespace Stocks2CSV
{
    class Program
    {
        static void Main(string[] args)
        {
            var tCore = new Core();

            if(args.Length > 0)
            {
                if(!string.IsNullOrEmpty(args[0]))
                {
                    if(args[0] == "-debug")
                    {
                        tCore.debugMode = true;
                        Console.WriteLine("Debug Enabled");
                    }
                }
            }

            Console.WriteLine("Enter input directory");
            tCore.inDir = Console.ReadLine();

            Console.WriteLine("Enter output directory");
            tCore.outDir = Console.ReadLine();

            Console.WriteLine("Processing. Please Wait...");
            tCore.DoWork();

            Console.WriteLine("Done");
        }
    }
}
