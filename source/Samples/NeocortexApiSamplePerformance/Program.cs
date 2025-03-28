using System.Diagnostics;
using Newtonsoft.Json;


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
                
                // Dynamically load selected experiment
                IExperiment experiment = LoadExperiment(inputParameter.ExperimentClass);
                
                // Execute training and testing
                RunExperiment(inputParameter, trainingSequences, testSequences, experiment);
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

        /// <summary>
        /// Parses command-line arguments into structured parameters
        /// </summary>
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

        /// <summary>
        /// Extracts specific argument value from CLI args array
        /// </summary>
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

        /// <summary>
        /// Validates the existence of directory containing input JSONs
        /// </summary>
        private static void ValidateInputDirectory(string folderPath)
        {
            if (!Directory.Exists(folderPath))
            {
                throw new DirectoryNotFoundException($"Sequence folder '{folderPath}' not found.");
            }
        }

        /// <summary>
        /// Loads sequences from all matching JSON files
        /// </summary>
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

        /// <summary>
        /// Reads a dataset JSON file and deserializes into Sequence objects
        /// </summary>
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

        /// <summary>
        /// Gathers system performance metrics (RAM, CPU speed, .NET version)
        /// </summary>
        private static (double ramUsage, double cpuSpeedGHz, string dotNetVersion) GetSystemPerformanceMetrics()
        {
            //double cpuUsage = GetCpuUsage(); 
            double ramUsage = GetRamUsage();
            double cpuSpeedGHz = GetCpuSpeedGHz();
            string dotNetVersion = GetDotNetVersion();

            return (ramUsage, cpuSpeedGHz, dotNetVersion);
        }


        /// <summary>
        /// Returns current memory consumption of the process (in MB)
        /// </summary>
        private static double GetRamUsage()
        {
            using (Process process = Process.GetCurrentProcess())
            {
                return process.PrivateMemorySize64 / (1024.0 * 1024.0); // Convert bytes to MB
            }
        }

        /// <summary>
        /// Retrieves max CPU frequency from system (in GHz)
        /// </summary>
        private static double GetCpuSpeedGHz()
        {
            try
            {
                using (var searcher = new System.Management.ManagementObjectSearcher("SELECT MaxClockSpeed FROM Win32_Processor"))
                {
                    foreach (var item in searcher.Get())
                    {
                        if (item["MaxClockSpeed"] != null)
                        {
                            double maxClockSpeedMHz = Convert.ToDouble(item["MaxClockSpeed"]);
                            double maxClockSpeedGHz = maxClockSpeedMHz / 1000.0; // Convert MHz to GHz

                            Console.WriteLine($" CPU Speed Detected: {maxClockSpeedGHz:F2} GHz");
                            return maxClockSpeedGHz;
                        }
                    }
                }

                Console.WriteLine(" WARNING: Could not retrieve CPU speed. Returning default value.");
            }
            catch (Exception ex)
            {
                Console.WriteLine($" ERROR: Could not retrieve CPU speed. Exception: {ex.Message}");
            }

            return -1; 
        }


        /// <summary>
        /// Loads the experiment class dynamically using reflection from the command line.
        /// </summary>
        private static IExperiment LoadExperiment(string experimentClassName)
        {
            Type experimentType = Type.GetType($"NeocortexApiSamplePerformance.{experimentClassName}");

            if (experimentType == null || !typeof(IExperiment).IsAssignableFrom(experimentType))
                throw new ArgumentException($"Invalid experiment: {experimentClassName}. Make sure it implements IExperiment.");

            return (IExperiment)Activator.CreateInstance(experimentType);
        }

        /// <summary>
        /// Handles model training and testing, including logging performance
        /// </summary>
        private static void RunExperiment(InputParameter inputParameter, List<Sequence> trainingSequences, List<Sequence> testSequences, IExperiment experiment)
        {
            Console.WriteLine($" Running Experiment: {inputParameter.ExperimentClass}\n");

            //MultiSequenceLearning experiment = new MultiSequenceLearning();

            Console.WriteLine(" Starting Model Training...");
            Stopwatch trainingTimer = Stopwatch.StartNew();

            // Merge training sequences with the same name into one unified sequence
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
            var (ramUsage, cpuSpeedGHz, dotNetVersion) = GetSystemPerformanceMetrics();


            // Run Testing
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


                foreach (var seq in processedTrainingData)
                {
                    double trainAccuracy = experiment.GetAccuracy(seq.name);

                    Console.WriteLine($"Logging: {seq.name}, Training Accuracy: {trainAccuracy}, Testing Accuracy: {testAccuracy}, Output file: {inputParameter.OutputCsvPath}");

                    PerformanceLogger.LogPerformance(
                     inputParameter.ExperimentClass,
                     inputParameter,
                     seq.name,
                     seq.Data.Length,
                     learningTimeSeconds,
                     //cpuUsage,
                     ramUsage,
                     cpuSpeedGHz,
                     inputParameter.CpuCores,
                     trainAccuracy,
                     dotNetVersion
                        );




                    Console.WriteLine(" PerformanceLogger.LogPerformance() called successfully!");
                }
            }

            Console.WriteLine($" Model Testing Completed in {testingTimer.Elapsed.TotalSeconds:F2} seconds.");
        }

        /// <summary>
        /// Applies CPU affinity using a bitmask to control core usage
        /// </summary>
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

        /// <summary>
        /// Retrieves the current .NET runtime version
        /// </summary>
        private static string GetDotNetVersion()
        {
            return System.Runtime.InteropServices.RuntimeInformation.FrameworkDescription;
        }

        /// <summary>
        /// Extracts the list of enabled CPU cores from affinity bitmask
        /// </summary>
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

    }
}