using System;
using System.Collections.Generic;
using Aljurythm;

namespace Example
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
                        Path = @"Tests/sample.txt",
                        TimeLimit = 35,
                        RunMultiplier = 1E5,
                        DisplayInputs = true,
                        InputSeparator = ", "
                    },
                    new Level
                    {
                        Name = "Complete Cases",
                        Path = @"Tests/complete.txt",
                        TimeLimit = 8,
                        DisplayLog = false
                    }
                },
                Parse = (testCase, streamReader) =>
                {
                    // Inputs
                    var operands = Convert.ToString(streamReader.ReadLine()).Split(' ');
                    testCase.Inputs["x"] = Convert.ToInt32(operands[0]);
                    testCase.Inputs["y"] = Convert.ToInt32(operands[1]);

                    // Expected Results
                    testCase.ExpectedOutputs["sum"] = Convert.ToInt32(streamReader.ReadLine());
                    testCase.ExpectedOutputs["mul"] = Convert.ToInt32(streamReader.ReadLine());
                },
                Algorithm = testCase =>
                {
                    // Pass Test Case's 'Inputs to Algorithm
                    // Note: The Algorithm is run Level's RunMultiplier times
                    Algorithm((int)testCase.Inputs["x"], (int)testCase.Inputs["y"], out var sum, out var mul);

                    // Save Actual Outputs
                    testCase.ActualOutputs["sum"] = sum;
                    testCase.ActualOutputs["mul"] = mul;
                }
            };

            jury.DisplayMenu();
            jury.Start();
        }

        private static void Algorithm(int x, int y, out int sum, out int mul)
        {
            sum = x + y;
            mul = x * y;
        }
    }
}
