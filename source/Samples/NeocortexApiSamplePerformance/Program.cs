using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using System.Reflection;
using System.IO;
using NeocortexApiSamplePerformance;
using NeoCortexApi;

namespace NeocortexApiSamplePerformance
{
    class Program
    {
        static void Main(string[] args)
        {
            Console.WriteLine("Starting Experiment...");

            try
            {
                InputParameter inputParameter = ParseArguments(args);
                ValidateInputDirectory(inputParameter.sequenceFolder);
                inputParameter.sequences = LoadSequences(inputParameter.sequenceFolder);
                RunExperiment(inputParameter);
            }
            catch (Exception ex)
            {
                Console.WriteLine($"Error: {ex.Message}");
            }
        }

        private static InputParameter ParseArguments(string[] args)
        {
            if (args.Length == 0)
            {
                throw new ArgumentException("Usage: dotnet run --sequence-folder <path> --out <csv path> --cores <num> --cpu-speed <GHz> --dotnet <version> --experiment <class>");
            }

            return new InputParameter
            {
                sequenceFolder = GetArgumentValue(args, "--sequence-folder"),
                outputCsvPath = GetArgumentValue(args, "--out"),
                experimentClass = GetArgumentValue(args, "--experiment"),
                cpuCores = int.Parse(GetArgumentValue(args, "--cores")),
                cpuSpeed = double.Parse(GetArgumentValue(args, "--cpu-speed")),
                dotnetVersion = GetArgumentValue(args, "--dotnet")
            };
        }

        private static void ValidateInputDirectory(string folderPath)
        {
            if (!Directory.Exists(folderPath))
            {
                throw new DirectoryNotFoundException($"Sequence folder '{folderPath}' not found.");
            }
        }

        private static Dictionary<string, List<double>> LoadSequences(string folderPath)
        {
            List<MultiSequenceInput> inputs = ReadData.LoadSequences(folderPath);
            return inputs.ToDictionary(i => i.ExperimentName, i => i.Sequence1.Concat(i.Sequence2).ToList());
        }

        private static void RunExperiment(InputParameter inputParameter)
        {
            switch (inputParameter.experimentClass)
            {
                case "SP":
                    Stopwatch swSP = Stopwatch.StartNew();
                    swSP.Start();

                    SpatialPatternLearning experiment = new SpatialPatternLearning();
                    experiment.Run();

                    swSP.Stop();
                    Console.WriteLine($"SP Experiment Execution Time: {swSP.ElapsedMilliseconds} ms");

                    //Log SP experiment execution time
                    PerformanceLogger.LogPerformance(inputParameter, swSP.ElapsedMilliseconds);
                    break;

                case "MultiSequenceLearning":
                    foreach (var sequence in inputParameter.sequences)
                    {
                        Stopwatch swSequence = Stopwatch.StartNew();
                        swSequence.Start();

                        // Running experiment for each sequence
                        RunMultiSequenceLearningExperiment(new Dictionary<string, List<double>> { { sequence.Key, sequence.Value } });

                        swSequence.Stop();

                        Console.WriteLine($"MultiSequenceLearning Execution Time for {sequence.Key} (Length: {sequence.Value.Count}): {swSequence.ElapsedMilliseconds} ms");

                        // Log execution time for each sequence
                        PerformanceLogger.LogPerformance(inputParameter, swSequence.ElapsedMilliseconds);
                    }
                    break;

                case "3":
                    Console.WriteLine("Three");
                    break;

                default:
                    Console.WriteLine("Many");
                    break;
            }

            Console.WriteLine("Experiment Completed");
        }


        private static string GetArgumentValue(string[] args, string argName)
        {
            for (int i = 0; i < args.Length; i++)
            {
                if (args[i].Equals(argName, StringComparison.OrdinalIgnoreCase) && i + 1 < args.Length)
                {
                    return args[i + 1]; // Fetches the next value after the argument flag
                }
            }

            throw new ArgumentException($"Missing required argument: {argName}");
        }

        private static void RunMultiSequenceLearningExperiment(Dictionary<string, List<double>> inputs)
        {
            Dictionary<string, List<double>> sequences = new Dictionary<string, List<double>>();

            foreach (var kvp in inputs)
            {
                sequences.Add($"{kvp.Key}_S1", kvp.Value); // Ensuring unique keys
                sequences.Add($"{kvp.Key}_S2", kvp.Value); // Ensuring unique keys

                // Prototype for building the prediction engine.
                MultiSequenceLearning experiment = new MultiSequenceLearning();
                var predictor = experiment.Run(sequences);

                // These lists are used to see how the prediction works.
                var list1 = new double[] { 1.0, 2.0, 3.0, 4.0, 2.0, 5.0 };
                var list2 = new double[] { 2.0, 3.0, 4.0 };
                var list3 = new double[] { 8.0, 1.0, 2.0 };

                predictor.Reset();
                PredictNextElement(predictor, list1);

                predictor.Reset();
                PredictNextElement(predictor, list2);

                predictor.Reset();
                PredictNextElement(predictor, list3);
            }
        }


        private static void PredictNextElement(Predictor predictor, double[] list)
        {
            Debug.WriteLine("------------------------------");

            foreach (var item in list)
            {
                var res = predictor.Predict(item);

                if (res.Count > 0)
                {
                    foreach (var pred in res)
                    {
                        Debug.WriteLine($"{pred.PredictedInput} - {pred.Similarity}");
                    }

                    var tokens = res.First().PredictedInput.Split('_');
                    var tokens2 = res.First().PredictedInput.Split('-');
                    Debug.WriteLine($"Predicted Sequence: {tokens[0]}, predicted next element {tokens2.Last()}");
                }
                else
                    Debug.WriteLine("Nothing predicted :(");
            }

            Debug.WriteLine("------------------------------");
        }
    }
}

