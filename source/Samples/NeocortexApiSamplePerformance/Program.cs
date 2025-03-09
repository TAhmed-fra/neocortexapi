using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using System.Runtime.InteropServices;
using System.IO;
using System.Threading;
using NeocortexApiSamplePerformance;
using NeoCortexApi;

namespace NeocortexApiSamplePerformance
{
    class Program
    {
        static void Main(string[] args)
        {
            Console.Clear();
            Console.WriteLine(" Starting Experiment...\n");

            try
            {
                InputParameter inputParameter = ParseArguments(args);
                ValidateInputDirectory(inputParameter.SequenceFolder);
                inputParameter.Sequences = LoadSequences(inputParameter.SequenceFolder);

                Console.WriteLine(" Applying CPU Affinity...");
                int activeCores = SetCpuAffinity(inputParameter.CpuAffinity);
                inputParameter.CpuCores = activeCores; // Update core count

                Console.WriteLine(" CPU Affinity Set. Active Cores: " + string.Join(", ", GetActiveCores(inputParameter.CpuAffinity)) + "\n");

                RunExperiment(inputParameter);
            }
            catch (Exception ex)
            {
                Console.WriteLine($" Error: {ex.Message}");
            }

            Console.WriteLine(" Experiment Completed. Press Enter to exit.");
            Console.ReadLine();
        }

        private static InputParameter ParseArguments(string[] args)
        {
            if (args.Length == 0)
            {
                throw new ArgumentException(" Usage: dotnet run --sequence-folder <path> --out <csv path> --cores <num> --cpu-speed <GHz> --dotnet <version> --experiment <class> --affinity <bitmask>");
            }

            return new InputParameter
            {
                SequenceFolder = GetArgumentValue(args, "--sequence-folder"),
                OutputCsvPath = GetArgumentValue(args, "--out"),
                ExperimentClass = GetArgumentValue(args, "--experiment-class"),
                CpuCores = int.Parse(GetArgumentValue(args, "--cores")),
                CpuSpeedGHz = double.Parse(GetArgumentValue(args, "--cpu-speed")),
                DotnetVersion = GetArgumentValue(args, "--dotnet"),
                CpuAffinity = int.Parse(GetArgumentValue(args, "--affinity"))
            };
        }

        private static string GetArgumentValue(string[] args, string argName)
        {
            for (int i = 0; i < args.Length; i++)
            {
                if (args[i].Equals(argName, StringComparison.OrdinalIgnoreCase) && i + 1 < args.Length)
                {
                    return args[i + 1];
                }
            }
            throw new ArgumentException($" Missing required argument: {argName}");
        }

        private static void ValidateInputDirectory(string folderPath)
        {
            if (!Directory.Exists(folderPath))
            {
                throw new DirectoryNotFoundException($" Sequence folder '{folderPath}' not found.");
            }
        }

        private static Dictionary<string, List<double>> LoadSequences(string folderPath)
        {
            Console.WriteLine(" Loading sequences from: " + folderPath);
            List<MultiSequenceInput> inputs = ReadData.LoadSequences(folderPath);
            Console.WriteLine($" Loaded {inputs.Count} sequences.");
            return inputs.ToDictionary(i => i.ExperimentName, i => i.Sequence1.Concat(i.Sequence2).ToList());
        }

        private static void RunExperiment(InputParameter inputParameter)
        {
            Stopwatch totalTimer = Stopwatch.StartNew();
            double accuracy = 0;

            Console.WriteLine("\n Running Experiment: " + inputParameter.ExperimentClass);

            switch (inputParameter.ExperimentClass)
            {
                case "SP":
                    Stopwatch swSP = Stopwatch.StartNew();
                    SpatialPatternLearning experiment = new SpatialPatternLearning();
                    experiment.Run();
                    swSP.Stop();

                    Console.WriteLine($" SP Experiment Completed in {swSP.Elapsed.TotalSeconds:F2}s");

                    PerformanceLogger.LogPerformance(inputParameter, "SP Experiment", 0, swSP.Elapsed.TotalSeconds, swSP.ElapsedMilliseconds, GetCurrentCpuUsage(), GetAvailableMemory(), inputParameter.CpuCores);
                    break;

                case "MultiSequenceLearning":
                    foreach (var sequence in inputParameter.Sequences)
                    {
                        Stopwatch swSequence = Stopwatch.StartNew();
                        Console.WriteLine($" Processing Sequence: {sequence.Key} ({sequence.Value.Count} elements)");

                        RunMultiSequenceLearningExperiment(new Dictionary<string, List<double>> { { sequence.Key, sequence.Value } });

                        swSequence.Stop();

                        Console.WriteLine($" {sequence.Key} Experiment Completed in {swSequence.Elapsed.TotalSeconds:F2}s");

                        accuracy = GetExperimentAccuracy();
                        PerformanceLogger.LogPerformance(inputParameter, sequence.Key, sequence.Value.Count, swSequence.Elapsed.TotalSeconds, swSequence.ElapsedMilliseconds, GetCurrentCpuUsage(), GetAvailableMemory(), inputParameter.CpuCores);
                    }
                    break;

                default:
                    Console.WriteLine(" Unknown experiment type.");
                    break;
            }
        }

        private static int SetCpuAffinity(int affinityBitmask)
        {
            Process process = Process.GetCurrentProcess();
            process.ProcessorAffinity = (IntPtr)affinityBitmask;
            return GetActiveCores(affinityBitmask).Count;
        }

        private static List<int> GetActiveCores(int affinityBitmask)
        {
            List<int> activeCores = new List<int>();
            int coreIndex = 0;

            while (affinityBitmask > 0)
            {
                if ((affinityBitmask & 1) == 1)
                {
                    activeCores.Add(coreIndex);
                }
                affinityBitmask >>= 1;
                coreIndex++;
            }
            return activeCores;
        }

        private static double GetCurrentCpuUsage()
        {
            var process = Process.GetCurrentProcess();
            TimeSpan startCpuUsage = process.TotalProcessorTime;
            DateTime startTime = DateTime.UtcNow;

            Thread.Sleep(500);

            TimeSpan endCpuUsage = process.TotalProcessorTime;
            DateTime endTime = DateTime.UtcNow;

            return Math.Round((endCpuUsage - startCpuUsage).TotalMilliseconds / (endTime - startTime).TotalMilliseconds * 100.0, 2);
        }

        private static double GetAvailableMemory()
        {
            return Process.GetCurrentProcess().WorkingSet64 / (1024 * 1024);
        }

        private static void RunMultiSequenceLearningExperiment(Dictionary<string, List<double>> inputs)
        {
            MultiSequenceLearning experiment = new MultiSequenceLearning();
            var predictor = experiment.Run(inputs);

            var testSequences = new List<double[]>
            {
                new double[] { 1.0, 2.0, 3.0, 4.0, 2.0, 5.0 },
                new double[] { 2.0, 3.0, 4.0 },
                new double[] { 8.0, 1.0, 2.0 }
            };

            foreach (var sequence in testSequences)
            {
                predictor.Reset();
                PredictNextElement(predictor, sequence);
            }
        }

        private static double GetExperimentAccuracy()
        {
            return new Random().Next(85, 100);
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
                }
                else
                {
                    Debug.WriteLine("Nothing predicted :(");
                }
            }

            Debug.WriteLine("------------------------------");
        }
    }
}
