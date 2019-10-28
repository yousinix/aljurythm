using System;
using System.Collections.Generic;
using System.IO;

namespace Aljurythm
{
    public class Jury
    {
        private int _startingLevel;
        private string _name;
        private List<Level> _levels;

        public string Name
        {
            get => _name;
            set
            {
                _name = value;
                if (!string.IsNullOrEmpty(_name))
                    Console.Title = Name;
            }
        }

        public List<Level> Levels
        {
            get => _levels;
            set
            {
                _levels = value;
                for (var i = 0; i < _levels.Count; i++)
                {
                    var level = _levels[i];
                    if (string.IsNullOrEmpty(level.Name))
                        level.Name = $"Level {i + 1}";
                }
            }
        }

        public void DisplayMenu()
        {
            if (!string.IsNullOrEmpty(_name))
            {
                Print($"{Name}:\n", ConsoleColor.Magenta);
                Console.WriteLine(new string('─', Name.Length + 1));
            }

            for (var i = 0; i < Levels.Count; i++)
            {
                Console.WriteLine($"[{i + 1}] {Levels[i].Name}");
            }
            Console.WriteLine();

            Console.Write($"Enter your choice [1~{Levels.Count}]: ");
            var choice = Console.ReadKey().KeyChar.ToString();
            var isNumeric = int.TryParse(choice, out var n);
            _startingLevel = isNumeric ? n - 1 : Levels.Count;
            Console.Clear();
        }

        public void Evaluate<T>(Func<Level, StreamReader, TestCase<T>> evaluateTestCase)
            where T : struct, IEquatable<T>
        {
            for (var index = _startingLevel; index < Levels.Count; index++)
            {
                var level = Levels[index];
                Print($"Running {level.Name}...\n\n", ConsoleColor.Yellow);

                using (var streamReader = new StreamReader(level.Path))
                {
                    level.Statistics.TotalCases = Convert.ToInt32(streamReader.ReadLine());
                    var paddingLength = level.Statistics.TotalCases.ToString().Length;

                    for (var i = 0; i < level.Statistics.TotalCases; i++)
                    {
                        var caseNumber = (i + 1).ToString().PadLeft(paddingLength, '0');
                        if (level.DisplayLog || level.DisplayInputs) Console.Write($"Case {caseNumber}: ");

                        var testCase = evaluateTestCase(level, streamReader);
                        level.Statistics.UpdateTime(testCase);

                        if (testCase.Time > level.TimeLimit)
                        {
                            if (!level.DisplayLog) Console.Write($"Case {caseNumber}: ");
                            Print("TIME LIMIT EXCEEDED\n", ConsoleColor.Blue);
                            if (level.DisplayInputs) Console.WriteLine(testCase.Inputs);
                            return;
                        }

                        if (!testCase.Actual.Equals(testCase.Expected))
                        {
                            level.Statistics.FailedCases++;
                            if (!level.DisplayLog) Console.Write($"Case {caseNumber}: ");
                            Print($"FAILED [Actual = {testCase.Actual} :: Expected = {testCase.Expected}]\n",
                            ConsoleColor.Red);
                        }
                        else if (level.DisplayLog)
                        {
                            Print($"COMPLETED [{testCase.Time} ms]\n", ConsoleColor.Green);
                        }

                        if (!level.DisplayLog && level.DisplayInputs) Console.WriteLine();
                        if (level.DisplayInputs) Console.WriteLine(testCase.Inputs);

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