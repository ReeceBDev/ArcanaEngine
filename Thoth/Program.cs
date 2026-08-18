using System.Collections.Concurrent;
using System.Collections.Immutable;
using System.Text.RegularExpressions;
using System.Threading;
using Thoth.External;
using Thoth.External.InternalConcreteDependencies;
using Thoth.External.Types;
using Thoth.Managers;
using Thoth.Resources;

namespace Thoth
{
    internal class Program
    {
        static void Main(string[] args)
        {
            Console.WriteLine("Starting Thoth...");

            if (args.Length > 1)
            {
                Console.WriteLine("ERROR: Multiple args are not currently supported. Detected args: " + string.Join(' ', args));
                return;
            }
            
            if (args.Length > 0 && (args[0] == "--run-example" || args[0] == "--run-demo"))
            {
                Console.WriteLine($"Detected args: {args[0]}");

                ExampleUsage.Run();
            }

            Console.WriteLine("Exiting program...");
        }
    }
}