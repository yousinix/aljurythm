using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.IO;
using System.Linq;

namespace Aljurythm
{
    public class Jury
    {
        private const string PackageName = "Aljurythm";
        private const string PackageUri = "https://github.com/YoussefRaafatNasry/aljurythm";
        private List<Level> _levels;

        private string _name;
        private int _startingLevel;

        public string DescriptionUri { get; set; }
        public string SubmissionUri { get; set; }

        public Action<TestCase, StreamReader> Parse { get; set; }
        public Action<TestCase> Algorithm { get; set; }

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

        public void ShowMenu()
        {
            var menu = new Menu
            {
                Title = Name,
                MainItems = Levels.Select((lvl, index) =>
                    new MenuItem
                    {
                        HotKey = (index + 1).ToString(),
                        Text = lvl.Name,
                        Action = () => _startingLevel = index,
                        PostAction = MenuItem.PostActionMode.START
                    }).ToList(),
                ExtraItems = new List<MenuItem>
                {
                    new MenuItem
                    {
                        HotKey = "d",
                        Text = "Problem Description",
                        Action = () => Process.Start(DescriptionUri),
                        Condition = DescriptionUri != null
                    },
                    new MenuItem
                    {
                        HotKey = "s",
                        Text = "Submit Solution",
                        Action = () => Process.Start(SubmissionUri),
                        Condition = SubmissionUri != null
                    },
                    new MenuItem
                    {
                        HotKey = "*",
                        Text = $"Contribute to {PackageName}",
                        Action = () => Process.Start(PackageUri)
                    }
                }
            };

            menu.Show();
            menu.Prompt();
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
                        testCase.Run(Algorithm);
                        testCase.Evaluate();
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
                            Logger.LineBreak();
                        }
                        else
                        {
                            if (level.DisplayLog)
                                Logger.WriteLine($"COMPLETED [{testCase.Time} ms]", ConsoleColor.Green);
                            if (level.DisplayInputs) Logger.WriteLine($"{testCase.InputsLog}\n", ConsoleColor.Cyan);
                        }
                    }
                }

                level.Statistics.Print();

                if (index == Levels.Count - 1) continue;
                var nextLevel = Levels[index + 1];
                Logger.Write($"Run {nextLevel.Name}? (y/N) ", ConsoleColor.Yellow);
                var choice = Console.ReadLine()?.ToLower();
                Logger.ClearLast();
                Logger.LineBreak();
                if (choice != "y") break;
            }
        }
    }
}