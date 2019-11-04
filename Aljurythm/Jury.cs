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
                    Logger.WriteLine($"{Name}:", ConsoleColor.Magenta);
                    Logger.WriteLine(new string('─', Name.Length + 1));
                }

                for (var i = 0; i < Levels.Count; i++) Logger.WriteLine($"[{i + 1}] {Levels[i].Name}");

                if (extraOptions)
                {
                    Logger.LineBreak();
                    Logger.WriteLine(ProblemLink != null ? "[p] Problem Description" : "", ConsoleColor.Blue);
                    Logger.WriteLine(SubmissionLink != null ? "[s] Submit Solution" : "", ConsoleColor.Blue);
                    Logger.WriteLine("[*] Contribute to Aljurythm", ConsoleColor.Blue);
                }

                Logger.LineBreak();
                Logger.Write("> ");
                var choice = Console.ReadKey().KeyChar.ToString();
                Logger.Clear();

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
                        Logger.WriteLine("Opening Aljurythm on GitHub", ConsoleColor.Blue);
                        Process.Start("https://github.com/YoussefRaafatNasry/aljurythm");
                        Logger.Clear();
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
                Logger.WriteLine($"Running {level.Name}...", ConsoleColor.Yellow);
                Logger.LineBreak();

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
                        if (level.DisplayLog || level.DisplayInputs) Logger.Write($"Case {caseNumber}: ");

                        if (testCase.Time > level.TimeLimit)
                        {
                            if (!level.DisplayLog) Logger.Write($"Case {caseNumber}: ");
                            Logger.WriteLine("TIME LIMIT EXCEEDED", ConsoleColor.Blue);
                            if (level.DisplayInputs) testCase.PrintInput();
                            return;
                        }

                        if (!testCase.Actual.Equals(testCase.Expected))
                        {
                            level.Statistics.FailedCases++;
                            if (!level.DisplayLog) Logger.Write($"Case {caseNumber}: ");
                            Logger.WriteLine($"FAILED [Actual = {testCase.Actual} :: Expected = {testCase.Expected}]",
                                ConsoleColor.Red);
                        }
                        else if (level.DisplayLog)
                        {
                            Logger.WriteLine($"COMPLETED [{testCase.Time} ms]", ConsoleColor.Green);
                        }

                        if (!level.DisplayLog && level.DisplayInputs) Logger.LineBreak();
                        if (level.DisplayInputs) testCase.PrintInput();
                    }
                }

                level.Statistics.Print();
                if (index == Levels.Count - 1) continue;
                var nextLevel = Levels[index + 1];
                Logger.Write($"Run {nextLevel.Name}? (y/N) ", ConsoleColor.Yellow);
                var choice = Console.ReadKey().KeyChar;
                Logger.ClearLast();
                if (char.ToLower(choice) != 'y') break;
            }
        }
    }
}