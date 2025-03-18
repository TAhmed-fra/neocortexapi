using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.IO;
using System.Linq;
using Newtonsoft.Json;
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
                //  Parse Arguments
                InputParameter inputParameter = ParseArguments(args);
                ValidateInputDirectory(inputParameter.SequenceFolder);

                //  Apply CPU Affinity
                Console.WriteLine(" Applying CPU Affinity...");
                int activeCores = SetCpuAffinity(inputParameter.CpuAffinity);
                // Update core count
                inputParameter.CpuCores = activeCores;

                //  Load Training and Testing Sequences
                List<Sequence> trainingSequences = LoadDatasets(inputParameter.SequenceFolder, "dataset_*.json");
                List<Sequence> testSequences = LoadDatasets(inputParameter.SequenceFolder, "test_*.json");
                Console.WriteLine($" CPU Affinity Set. Active Cores: {string.Join(", ", GetActiveCores(inputParameter.CpuAffinity))}\n");
                Console.WriteLine($" Loaded {trainingSequences.Count} training sequences.");
                Console.WriteLine($" Loaded {testSequences.Count} test sequences.");

                // Run Experiment
                RunExperiment(inputParameter, trainingSequences, testSequences);
            }
            catch (Exception ex)
            {
                Console.WriteLine($" Fatal Error: {ex.Message}");
                Console.WriteLine($" Stack Trace: {ex.StackTrace}");
            }

            Console.WriteLine("\n Experiment Completed. Press Enter to exit.");
            // Keeps the console open
            Console.ReadLine();  
        }

        private static InputParameter ParseArguments(string[] args)
        {
            if (args.Length == 0)
            {
                throw new ArgumentException(" Missing required arguments. Use: dotnet run --sequence-folder <path> --out <csv path> --cores <num> --experiment <class>");
            }

            return new InputParameter
            {
                SequenceFolder = GetArgumentValue(args, "--sequence-folder") ?? throw new ArgumentException(" Missing required argument: --sequence-folder"),
                OutputCsvPath = GetArgumentValue(args, "--out") ?? "output.csv",
                ExperimentClass = GetArgumentValue(args, "--experiment") ?? throw new ArgumentException(" Missing required argument: --experiment"),
                CpuCores = int.Parse(GetArgumentValue(args, "--cores")),
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
                throw new DirectoryNotFoundException($"Sequence folder '{folderPath}' not found.");
            }
        }

        private static List<Sequence> LoadDatasets(string folderPath, string pattern)
        {
            List<Sequence> sequences = new List<Sequence>();
            string[] files = Directory.GetFiles(folderPath, pattern);

            foreach (var file in files)
            {
                var seqData = ReadDataset(file);
                sequences.AddRange(seqData);
            }

            return sequences;
        }

        private static List<Sequence> ReadDataset(string datasetPath)
        {
            try
            {
                Console.WriteLine($" Reading Dataset: {datasetPath}");
                return JsonConvert.DeserializeObject<List<Sequence>>(File.ReadAllText(datasetPath));
            }
            catch (Exception ex)
            {
                Console.WriteLine($" Error reading dataset: {ex.Message}");
                return new List<Sequence>();
            }
        }

        private static (double cpuUsage, double ramUsage, double cpuSpeedGHz) GetSystemPerformanceMetrics()
        {
            using (var process = Process.GetCurrentProcess())
            {
                double cpuUsage = process.TotalProcessorTime.TotalMilliseconds / Environment.ProcessorCount;
                double ramUsage = process.PrivateMemorySize64 / (1024.0 * 1024.0); // Convert to MB

                // Get CPU Speed (GHz)
                double cpuSpeedGHz = GetCpuSpeedGHz();

                return (cpuUsage, ramUsage, cpuSpeedGHz);
            }
        }

        // Function to Get CPU Speed
        private static double GetCpuSpeedGHz()
        {
            try
            {
                using (var searcher = new System.Management.ManagementObjectSearcher("SELECT MaxClockSpeed FROM Win32_Processor"))
                {
                    foreach (var item in searcher.Get())
                    {
                        return Convert.ToDouble(item["MaxClockSpeed"]) / 1000.0; // Convert MHz to GHz
                    }
                }
            }
            catch (Exception ex)
            {
                Console.WriteLine($"❌ ERROR: Could not retrieve CPU speed. Exception: {ex.Message}");
            }
            return 0; // Default to 0 if not found
        }

        //private static (double cpuUsage, double ramUsage) GetSystemPerformanceMetrics()
        //{
        //    using (var process = Process.GetCurrentProcess())
        //    {
        //        double cpuUsage = process.TotalProcessorTime.TotalMilliseconds / Environment.ProcessorCount;
        //        double ramUsage = process.PrivateMemorySize64 / (1024.0 * 1024.0);

        //        return (cpuUsage, ramUsage);
        //    }
        //}

        private static void RunExperiment(InputParameter inputParameter, List<Sequence> trainingSequences, List<Sequence> testSequences)
        {
            Console.WriteLine($" Running Experiment: {inputParameter.ExperimentClass}\n");

            MultiSequenceLearning experiment = new MultiSequenceLearning();

            Console.WriteLine(" Starting Model Training...");
            Stopwatch trainingTimer = Stopwatch.StartNew();

            var processedTrainingData = trainingSequences
                .GroupBy(seq => seq.name)
                .Select(g => new Sequence { name = g.Key, Data = g.SelectMany(seq => seq.Data).ToArray() })
                .ToList();

            // Train the model
            var predictor = experiment.Run(processedTrainingData);
            trainingTimer.Stop();

            double learningTimeSeconds = trainingTimer.Elapsed.TotalSeconds;
            long learningTimeMilliseconds = trainingTimer.ElapsedMilliseconds;

            Console.WriteLine($" Model Training Completed in {learningTimeSeconds:F2} seconds.");
            //var (cpuUsage, ramUsage) = GetSystemPerformanceMetrics();
            var (cpuUsage, ramUsage, cpuSpeedGHz) = GetSystemPerformanceMetrics();


            //  Now, Run Testing
            Console.WriteLine(" Starting Model Testing...");
            Stopwatch testingTimer = Stopwatch.StartNew();

            foreach (var testSeq in testSequences)
            {
                Console.WriteLine($" Testing Sequence: {testSeq.name} -> {string.Join("-", testSeq.Data)}");
                // Reset predictor before testing a new sequence
                predictor.Reset();  

                int matchCount = 0;
                int totalPredictions = testSeq.Data.Length - 1;
                List<string> predictionLog = new List<string>();

                int prev = -1;
                bool first = true;

                //  Run predictions on test data
                foreach (var next in testSeq.Data)
                {
                    if (first)
                    {
                        first = false;
                    }
                    else
                    {
                        var res = predictor.Predict(prev);
                        string log = $"Input: {prev}";

                        if (res.Count > 0)
                        {
                            var bestPrediction = res.First();
                            string[] predictionParts = bestPrediction.PredictedInput.Split('-');
                            int predictedValue = int.Parse(predictionParts.Last());

                            log += $", Predicted: {predictedValue}";

                            if (next == predictedValue)
                            {
                                matchCount++;
                            }
                        }
                        else
                        {
                            log += ", No prediction made.";
                        }

                        predictionLog.Add(log);
                    }

                    prev = (int)next;
                }

                // Calculate accuracy
                double testAccuracy = (double)matchCount / totalPredictions * 100.0;
                testingTimer.Stop();

                Console.WriteLine($" Accuracy for {testSeq.name}: {testAccuracy:F2}%");

                // Now log both training and testing results
                foreach (var seq in processedTrainingData)
                {
                    double trainAccuracy = experiment.GetAccuracy(seq.name);  //  Fetch actual training accuracy

                    Console.WriteLine($"Logging: {seq.name}, Training Accuracy: {trainAccuracy}, Testing Accuracy: {testAccuracy}, Output file: {inputParameter.OutputCsvPath}");

                    PerformanceLogger.LogPerformance(
                     inputParameter.ExperimentClass,
                     inputParameter,
                     seq.name,
                     seq.Data.Length,
                     learningTimeSeconds, 
                     cpuUsage,
                     ramUsage,
                     cpuSpeedGHz,
                     inputParameter.CpuCores,
                     trainAccuracy  
                        );


                    //PerformanceLogger.LogPerformance(
                    // inputParameter.ExperimentClass,
                    // inputParameter,
                    // seq.name,
                    // seq.Data.Length, // 🔹 Number of Elements in the Sequence
                    // learningTimeSeconds,
                    // cpuUsage,
                    // ramUsage,
                    // cpuSpeedGHz, // NEW: Track CPU Speed
                    // inputParameter.CpuCores
                    // );
                    //PerformanceLogger.LogPerformance(
                    //// Pass Experiment Name
                    //inputParameter.ExperimentClass,
                    //// Pass InputParameter object
                    //inputParameter,  
                    //seq.name,
                    //seq.Data.Length,
                    //// Log training time
                    //learningTimeSeconds,   
                    //learningTimeMilliseconds,
                    //// Log CPU usage
                    //cpuUsage,
                    //// Log RAM usage
                    //ramUsage,
                    //// Log active CPU cores
                    //inputParameter.CpuCores,
                    //// Log actual testing accuracy
                    //testAccuracy
                    //);


                    Console.WriteLine(" PerformanceLogger.LogPerformance() called successfully!");
                }
            }

            Console.WriteLine($" Model Testing Completed in {testingTimer.Elapsed.TotalSeconds:F2} seconds.");
        }


        private static int SetCpuAffinity(int affinityBitmask)
        {
            Process process = Process.GetCurrentProcess();
            process.ProcessorAffinity = (IntPtr)affinityBitmask;

            // Retrieve applied affinity
            long appliedAffinity = process.ProcessorAffinity.ToInt64();

            // Get the actual active cores
            var activeCores = GetActiveCores((int)appliedAffinity);

            // Debugging: Print confirmed affinity
            Console.WriteLine($" CPU Affinity Applied: {Convert.ToString(appliedAffinity, 2).PadLeft(Environment.ProcessorCount, '0')} (Binary Mask)");
            Console.WriteLine($" Active Cores Confirmed: {string.Join(", ", activeCores)}");

            return activeCores.Count;
        }

        private static List<int> GetActiveCores(int affinityBitmask)
        {
            List<int> activeCores = new List<int>();
            int coreIndex = 0;

            Console.WriteLine(" Checking active cores...");
            while (affinityBitmask > 0)
            {
                if ((affinityBitmask & 1) == 1)
                {
                    activeCores.Add(coreIndex);
                    Console.WriteLine($" Core {coreIndex} is ACTIVE");
                }
                affinityBitmask >>= 1;
                coreIndex++;
            }

            return activeCores;
        }

        //private static double GetApplicationCpuUsage()
        //{
        //    Process currentProcess = Process.GetCurrentProcess();
        //    TimeSpan prevTotalProcessorTime = currentProcess.TotalProcessorTime;
        //    DateTime prevTime = DateTime.UtcNow;

        //    // Short delay to capture CPU usage accurately
        //    System.Threading.Thread.Sleep(1000);

        //    TimeSpan newTotalProcessorTime = currentProcess.TotalProcessorTime;
        //    DateTime newTime = DateTime.UtcNow;

        //    // Get number of ACTIVE cores (used in affinity)
        //    int activeCores = currentProcess.ProcessorAffinity.ToInt64().ToString("X").Count(c => c == '1');

        //    // Prevent division by zero
        //    if (activeCores == 0) activeCores = 1;


        //    // Calculate CPU Usage relative to active cores
        //    double cpuUsage = (newTotalProcessorTime - prevTotalProcessorTime).TotalMilliseconds /
        //                      (newTime - prevTime).TotalMilliseconds * 100 / activeCores;

        //    return Math.Round(cpuUsage, 2);
        //}


        //private static double GetAvailableMemory()
        //{
        //    return Process.GetCurrentProcess().WorkingSet64 / (1024 * 1024);
        //}
    }
}