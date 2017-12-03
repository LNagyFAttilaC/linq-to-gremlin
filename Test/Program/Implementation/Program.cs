using System;
using System.Diagnostics;
using System.Linq;
using System.Reflection;
using Test.TestCases;
using Test.TestCases.Meta;

namespace Test
{
    public class Program
    {
        #region Methods

        public static void Main(
            string[] args)
        {
            Start();

            var random = new Random();

            var testCases
                = Assembly
                    .GetExecutingAssembly()
                    .GetTypes()
                    .Where(
                        t => t.IsClass && !t.IsAbstract && typeof(ITestCase)
                            .IsAssignableFrom(
                                t))
                    .OrderBy(
                        t => random
                            .Next())
                    .ToArray();

            Write(
                testCases.Length + " test case(s) will be executed.\n");

            var disabled = 0;
            var failure = 0;
            var success = 0;
            var ticks = 0L;

            foreach (var testCase in testCases)
            {
                var instance
                    = (ITestCase)
                        Activator
                            .CreateInstance(
                                testCase);

                var stopwatch 
                    = Stopwatch
                        .StartNew();

                if (instance.TestCaseDescriptor.Enabled)
                {
                    Write(
                        "\n" + instance.TestCaseDescriptor.Name + "\n",
                        ConsoleColor.Cyan);

                    Write(
                        instance.TestCaseDescriptor.Goal + "\n");

                    try
                    {
                        Write(
                            "PREPARE...\n");

                        instance
                            .Prepare();

                        try
                        {
                            Write(
                                "EXECUTE...\n");

                            instance
                                .Execute();

                            success++;

                            Write(
                                "SUCCESS\n",
                                ConsoleColor.Green);
                        }
                        catch (TestCaseFailureException e)
                        {
                            failure++;

                            Write(
                                "FAILURE:\n" + e.Message + "\n",
                                ConsoleColor.Red);
                        }
                    }
                    catch (TestCaseFailureException e)
                    {
                        failure++;

                        Write(
                            "FAILURE:\n" + e.Message + "\n",
                            ConsoleColor.Red);
                    }
                    finally
                    {
                        try
                        {
                            Write(
                                "TEAR DOWN...\n");

                            instance
                                .TearDown();
                        }
                        catch (TestCaseFailureException e)
                        {
                            failure++;

                            Write(
                                "FAILURE:\n" + e.Message + "\n",
                                ConsoleColor.Red);
                        }
                    }
                }
                else
                {
                    disabled++;
                }

                stopwatch
                    .Stop();

                ticks += stopwatch.ElapsedMilliseconds;

                Write(
                    stopwatch.ElapsedMilliseconds + " ms\n");
            }

            Write(
                "\n" + testCases.Length + " test case(s) has been successfully executed.\n");

            Write(
                "Success: " + success,
                ConsoleColor.Green);

            Write(
                " | ");

            Write(
                "Failure: " + failure,
                ConsoleColor.Red);

            Write(
                " | ");

            Write(
                "Disabled: " + disabled);

            Write(
                " | ");

            Write(
               ticks + " ms\n");

            Write(
                "\nPress any key to quit...");

            End();

            Console
                .ReadKey();
        }

        private static void End()
        {

        }

        private static void Start()
        {

        }

        private static void Write(
            string text,
            ConsoleColor color = ConsoleColor.White)
        {
            var actualColor = Console.ForegroundColor;

            Console.ForegroundColor = color;

            Console
                .Write(
                    text);

            Console.ForegroundColor = actualColor;
        }

        #endregion
    }
}
