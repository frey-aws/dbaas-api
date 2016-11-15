using System;
using System.Threading;
using System.Threading.Tasks;
using OperationsApi.BusinessLogic.Command;

namespace OperationsApi.Harness
{
    class Program
    {
        static void Main(string[] args)
        {
            SimpleThreadLoggingTest();           
        }

        // Really simple test, yes has threading issues, but confirms that the logging will be properly threading, albeit not perfect yet
        private static void SimpleThreadLoggingTest()
        {
            AmazonRdsCommand cmd = new AmazonRdsCommand();

            for (int x = 0; x <= 100; x++)
            {
                Task.Run(() => { cmd.JustLog("Test " + Interlocked.Increment(ref x)); });
            }
        }
    }
}
