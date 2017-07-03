using System;
using System.Diagnostics;
using System.Linq;
using System.Net.Http;
using System.Threading;
using System.Threading.Tasks;

namespace TestAwaitApproaches.Client
{
    class Program
    {
        private static readonly HttpClient _httpClient = new HttpClient()
        {
            Timeout = Timeout.InfiniteTimeSpan
        };

        private const int Iterations = 10;
        private const int RequestsPerIteration = 150;

        private static readonly TimeSpan CooldownPeriod = TimeSpan.FromMinutes(2);

        static void Main(string[] args)
        {
            bool selectionIsValid = false;
            char selection = default(char);
            while (!selectionIsValid)
            {
                Console.WriteLine("1: Test single-await methods.");
                Console.WriteLine("2: Test double-await methods.");
                selection = Console.ReadKey().KeyChar;
                Console.WriteLine();

                selectionIsValid = (new[] {'1', '2'}).Contains(selection);
            }

            switch (selection)
            {
                case '1':
                    TestSingleAwaitMethods();
                    break;
                case '2':
                    TestDoubleAwaitMethods();
                    break;
            }

            Console.WriteLine("Press ENTER to continue . . .");
            Console.ReadLine();
        }

        private static void TestDoubleAwaitMethods()
        {
            TestActions(new[]
            {
                "double_await/await_service_twice",
                "double_await/wait_on_first_result",
                "double_await/wait_on_both_results",
                "double_await/use_result_then_await",
                "double_await/use_result_then_result",
            });
        }

        private static void TestSingleAwaitMethods()
        {
            TestActions(new[]
            {
                "single_await/await_service",
                "single_await/return_task_from_service",
                "single_await/wait_service",
            });
        }

        private static void TestActions(string[] actions)
        {
            Console.WriteLine("Start");
            foreach (var action in actions)
            {
                Console.WriteLine($"Warming {action} up...");
                ExecuteActions(1, action);
            }

            for (var i = 0; i < Iterations; i++)
            {
                Console.WriteLine();
                Console.WriteLine($"Iteration {i + 1} of {Iterations}");
                foreach (var action in actions)
                {
                    Test(RequestsPerIteration, action);
                }
            }
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
                var exceptionGroups = ae.InnerExceptions.GroupBy(ie => ie.GetBaseException().Message);
                foreach (var eg in exceptionGroups)
                {
                    Console.WriteLine($"{eg.Count():N0} exception(s): {eg.Key}");
                }

                Console.WriteLine($"Cooling down for {CooldownPeriod}");
                Thread.Sleep(CooldownPeriod);
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