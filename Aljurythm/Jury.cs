using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.IO;

namespace Aljurythm
{
    public class Jury<TResult> where TResult : struct, IEquatable<TResult>
    {
        private List<Level> _levels;
        private string _name;
        private int _startingLevel = int.MaxValue;

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

        public string ProblemLink { get; set; }

        public string SubmissionLink { get; set; }

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

        public Action<TestCase<TResult>, StreamReader> ReadInput { get; set; }

        public Func<TestCase<TResult>, TResult> Algorithm { get; set; }

        public void DisplayMenu(bool extraOptions = true)
        {
            while (true)
            {
                if (!string.IsNullOrEmpty(_name))
                {
                    Print($"{Name}:\n", ConsoleColor.Magenta);
                    Console.WriteLine(new string('─', Name.Length + 1));
                }

                for (var i = 0; i < Levels.Count; i++) Console.WriteLine($"[{i + 1}] {Levels[i].Name}");

                if (extraOptions)
                {
                    Console.WriteLine();
                    Print(ProblemLink != null ? "[p] Problem Description\n" : "", ConsoleColor.Blue);
                    Print(SubmissionLink != null ? "[s] Submit Solution\n" : "", ConsoleColor.Blue);
                    Print("[*] Contribute to Aljurythm\n", ConsoleColor.Blue);
                }

                Console.WriteLine();

                Console.Write("> ");
                var choice = Console.ReadKey().KeyChar.ToString();
                Console.Clear();

                switch (choice)
                {
                    case "p":
                        if (ProblemLink == null) break;
                        Process.Start(ProblemLink);
                        continue;
                    case "s":
                        if (SubmissionLink == null) break;
                        Process.Start(SubmissionLink);
                        continue;
                    case "*":
                        Print("Opening Aljurythm on GitHub\n", ConsoleColor.Blue);
                        Process.Start("https://github.com/YoussefRaafatNasry/aljurythm");
                        Console.Clear();
                        continue;
                    default:
                        var isNumeric = int.TryParse(choice, out var n);
                        if (!isNumeric || n < 0 || n > Levels.Count) continue;
                        _startingLevel = --n;
                        break;
                }

                break;
            }
        }

        public void Start()
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
                        // Inputs and Algorithm Processing
                        var testCase = new TestCase<TResult>(level.InputSeparator);
                        ReadInput(testCase, streamReader);
                        testCase.RunAlgorithm(() => Algorithm(testCase), level.RunMultiplier);
                        level.Statistics.UpdateTime(testCase);

                        // Log and Inputs Printing
                        var caseNumber = (i + 1).ToString().PadLeft(paddingLength, '0');
                        if (level.DisplayLog || level.DisplayInputs) Console.Write($"Case {caseNumber}: ");

                        if (testCase.Time > level.TimeLimit)
                        {
                            if (!level.DisplayLog) Console.Write($"Case {caseNumber}: ");
                            Print("TIME LIMIT EXCEEDED\n", ConsoleColor.Blue);
                            if (level.DisplayInputs) testCase.PrintInput();
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
                        if (level.DisplayInputs) testCase.PrintInput();
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