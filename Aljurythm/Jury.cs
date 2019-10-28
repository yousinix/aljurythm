using System;
using System.Collections.Generic;
using System.IO;

namespace Aljurythm
{
    public class Jury
    {
        public List<Level> Levels { get; set; }

        public void Evaluate<T>(Func<Level, StreamReader, TestCase<T>> evaluateTestCase)
            where T : struct, IEquatable<T>
        {
            for (var index = 0; index < Levels.Count; index++)
            {
                var level = Levels[index];
                Print($"Running {level.Name}...\n\n", ConsoleColor.Yellow);

                using (var streamReader = new StreamReader(level.Path))
                {
                    level.Statistics.TotalCases = Convert.ToInt32(streamReader.ReadLine());
                    var paddingLength = level.Statistics.TotalCases.ToString().Length;

                    for (var i = 0; i < level.Statistics.TotalCases; i++)
                    {
                        Console.Write($"Case {(i + 1).ToString().PadLeft(paddingLength, '0')}: ");

                        var testCase = evaluateTestCase(level, streamReader);
                        level.Statistics.UpdateTime(testCase);

                        if (testCase.Time > level.TimeLimit)
                        {
                            Print("TIME LIMIT EXCEEDED\n", ConsoleColor.Blue);
                            return;
                        }

                        if (!testCase.Actual.Equals(testCase.Expected))
                        {
                            level.Statistics.FailedCases++;
                            Print($"FAILED [Actual = {testCase.Actual} :: Expected = {testCase.Expected}]\n",
                                ConsoleColor.Red);
                        }
                        else
                        {
                            Print($"COMPLETED [{testCase.Time} ms]\n", ConsoleColor.Green);
                        }
                    }

                }

                level.Statistics.Print();
                if (index == Levels.Count - 1) continue;
                Print($"Run {Levels[index + 1].Name}? (y/N) ", ConsoleColor.Yellow);
                var choice = Console.ReadKey().KeyChar;
                if (char.ToLower(choice) != 'y')
                {
                    Console.WriteLine();
                    break;
                }
                Console.SetCursorPosition(0, Console.CursorTop);
                Console.WriteLine(new string(' ', Console.WindowWidth));
            }
        }

        private static void Print(string message, ConsoleColor color)
        {
            Console.ForegroundColor = color;
            Console.Write(message);
            Console.ResetColor();
        }
    }
}