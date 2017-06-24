using System;
using System.Diagnostics;
using System.Linq;
using System.Net.Http;
using System.Threading.Tasks;

namespace TestAwaitApproaches.Client
{
    class Program
    {
        private static readonly HttpClient _httpClient = new HttpClient();

        private const int Iterations = 10;
        private const int RequestsPerIteration = 150;

        static void Main(string[] args)
        {
            Console.WriteLine("Start");
            var actions = new string[]
            {
                "await_service",
                "return_task_from_service",
                "wait_service",
            };

            foreach (var action in actions)
            {
                Console.WriteLine($"Warming {action} up...");
                ExecuteActions(1, action);
            }

            for (var i = 0; i < Iterations; i++)
            {
                Console.WriteLine();
                Console.WriteLine($"Iteration {i+1} of {Iterations}");
                foreach (var action in actions)
                {
                    Test(RequestsPerIteration, action);
                }
            }

            Console.WriteLine("Press ENTER to continue . . .");
            Console.ReadLine();
        }

        private static void Test(int count, string action)
        {
            var stopwatch = Stopwatch.StartNew();

            ExecuteActions(count, action);

            Console.WriteLine($"Took {stopwatch.Elapsed} to process {count} requests to {action}");
        }

        private static void ExecuteActions(int count, string action)
        {
            var tasks = Enumerable.Repeat(true, count).Select(x => DoRequest(action)).ToArray();
            try
            {
                Task.WaitAll(tasks, TimeSpan.FromMilliseconds(-1));
            }
            catch (AggregateException ae)
            {
                ae.Handle(e =>
                {
                    Console.WriteLine($"Exception: {e}.");
                    return true;
                });
            }
        }

        private static async Task<string> DoRequest(string action)
        {
            var response = await _httpClient.GetAsync($"http://localhost:50765/{action}");
            var content = await response.Content.ReadAsStringAsync();
            return content;
        }

    }
}