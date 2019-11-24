using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.IO;

namespace Aljurythm
{
    public class Jury
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

        public string DescriptionUri { get; set; }

        public string SubmissionUri { get; set; }

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

        public Action<TestCase, StreamReader> Parse { get; set; }

        public Action<TestCase> Algorithm { get; set; }

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
                    Logger.WriteLine(DescriptionUri != null ? "[d] Problem DescriptionURI" : "", ConsoleColor.Blue);
                    Logger.WriteLine(SubmissionUri != null ? "[s] Submit Solution" : "", ConsoleColor.Blue);
                    Logger.WriteLine("[*] Contribute to Aljurythm", ConsoleColor.Blue);
                }

                Logger.LineBreak();
                Logger.Write("> ");
                var choice = char.ToLower(Console.ReadKey().KeyChar);
                Logger.Clear();

                switch (choice)
                {
                    case 'd':
                        if (DescriptionUri == null) break;
                        Process.Start(DescriptionUri);
                        continue;
                    case 's':
                        if (SubmissionUri == null) break;
                        Process.Start(SubmissionUri);
                        continue;
                    case '*':
                        Logger.WriteLine("Opening Aljurythm on GitHub", ConsoleColor.Blue);
                        Process.Start("https://github.com/YoussefRaafatNasry/aljurythm");
                        Logger.Clear();
                        continue;
                    default:
                        var isNumeric = int.TryParse(choice.ToString(), out var n);
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
                    var padding = level.Statistics.TotalCases.ToString().Length;

                    for (var i = 0; i < level.Statistics.TotalCases; i++)
                    {
                        // Inputs and Algorithm Processing
                        var testCase = new TestCase(level, i, padding);
                        Parse(testCase, streamReader);
                        testCase.Test(Algorithm);
                        level.Statistics.UpdateTime(testCase);

                        // Log and Inputs Printing
                        if (level.DisplayLog || level.DisplayInputs) Logger.Write($"Case {testCase.Number}: ");

                        if (testCase.HasExceededTimeLimit)
                        {
                            if (!level.DisplayLog) Logger.Write($"Case {testCase.Number}: ");
                            Logger.WriteLine("TIME LIMIT EXCEEDED", ConsoleColor.Blue);
                            if (level.DisplayInputs) Logger.WriteLine($"{testCase.InputsLog}\n", ConsoleColor.Cyan);
                            return;
                        }

                        if (testCase.HasFailed)
                        {
                            level.Statistics.FailedCases++;
                            if (!level.DisplayLog) Logger.Write($"Case {testCase.Number}: ");
                            Logger.WriteLine("FAILED", ConsoleColor.Red);
                            if (level.DisplayInputs) Logger.WriteLine(testCase.InputsLog, ConsoleColor.Cyan);
                            Logger.WriteLine(testCase.FailureLog, ConsoleColor.Red);
                        }
                        else if (level.DisplayLog)
                        {
                            Logger.WriteLine($"COMPLETED [{testCase.Time} ms]", ConsoleColor.Green);
                            if (level.DisplayInputs) Logger.WriteLine($"{testCase.InputsLog}\n", ConsoleColor.Cyan);
                        }
                    }
                }

                level.Statistics.Print();

                if (index == Levels.Count - 1) continue;
                var nextLevel = Levels[index + 1];
                Logger.Write($"Run {nextLevel.Name}? (y/N) ", ConsoleColor.Yellow);
                var choice = char.ToLower(Console.ReadKey().KeyChar);
                Logger.ClearLast();
                if (choice != 'y') break;
            }
        }
    }
}