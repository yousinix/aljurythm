using System;
using System.Collections.Generic;
using Aljurythm;

namespace Demo
{
    internal class Program
    {
        private static void Main(string[] args)
        {
            var jury = new Jury
            {
                Name = "Arithmetic Algorithm",
                DescriptionUri = @"..\..\Description.txt",
                SubmissionUri = "https://www.google.com/",
                Levels = new List<Level>
                {
                    new Level
                    {
                        Name = "Sample Cases",
                        Path = @"Tests\sample.txt",
                        TimeLimit = 35,
                        RunMultiplier = 1E5,
                        DisplayInputs = true,
                        InputSeparator = ", "
                    },
                    new Level
                    {
                        Name = "Complete Cases",
                        Path = @"Tests\complete.txt",
                        TimeLimit = 8,
                        DisplayLog = false
                    }
                },
                Parse = (testCase, streamReader) =>
                {
                    // Inputs
                    var operands = Convert.ToString(streamReader.ReadLine()).Split(' ');
                    testCase.Input["x"] = Convert.ToInt32(operands[0]);
                    testCase.Input["y"] = Convert.ToInt32(operands[1]);

                    // Expected Outputs
                    testCase.Expected["sum"] = Convert.ToInt32(streamReader.ReadLine());
                    testCase.Expected["mul"] = Convert.ToInt32(streamReader.ReadLine());
                },
                Algorithm = testCase =>
                {
                    // Pass Test Case's 'Inputs to Algorithm
                    // Note: The Algorithm is run Level's RunMultiplier times
                    Algorithm((int)testCase.Input["x"], (int)testCase.Input["y"], out var sum, out var mul);

                    // Save Actual Outputs
                    testCase.Actual["sum"] = sum;
                    testCase.Actual["mul"] = mul;
                }
            };

            jury.ShowMenu();
            jury.Start();
        }

        private static void Algorithm(int x, int y, out int sum, out int mul)
        {
            sum = x + y;
            mul = x * y;
        }
    }
}
