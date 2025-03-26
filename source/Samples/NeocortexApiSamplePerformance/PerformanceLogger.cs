using System;
using System.IO;

namespace NeocortexApiSamplePerformance
{
    /// <summary>
    /// Handles logging of experiment performance metrics to a CSV file.
    /// </summary>
    public static class PerformanceLogger
    {
        /// <summary>
        /// Logs experiment metadata and performance statistics into a CSV file.
        /// Appends a new row or creates the file with headers if it doesn't exist.
        /// </summary>
        /// <param name="experimentName">Name of the experiment class used.</param>
        /// <param name="input">Input parameters including output path and core info.</param>
        /// <param name="sequenceName">The name of the training sequence.</param>
        /// <param name="dataSize">Total number of data points in the sequence.</param>
        /// <param name="learningTimeSeconds">Total training time in seconds.</param>
        /// <param name="ramUsage">RAM usage during training (in MB).</param>
        /// <param name="cpuSpeedGHz">CPU clock speed (GHz).</param>
        /// <param name="cores">Number of CPU cores used (derived from affinity).</param>
        /// <param name="trainingAccuracy">Accuracy achieved during training (percentage).</param>
        /// <param name="dotNetVersion">.NET version running the experiment.</param>
        public static void LogPerformance(
            string experimentName,
            InputParameter input,
            string sequenceName,
            int dataSize,
            double learningTimeSeconds,
            //double cpuUsage,
            double ramUsage,
            double cpuSpeedGHz,
            int cores,
            double trainingAccuracy,
            string dotNetVersion)
        {
            string outputPath = input.OutputCsvPath;

            try
            {
                Console.WriteLine($"Writing to CSV: {outputPath}");

                bool fileExists = File.Exists(outputPath);
                using (StreamWriter writer = new StreamWriter(outputPath, true))
                {
                    if (!fileExists)
                    {

                        writer.WriteLine("Experiment Name,Sequence Name,Data Size,Learning Time (s),RAM Usage (MB),CPU Speed (GHz),Cores Used,Training Accuracy (%), DotNet Version");
                    }


                    writer.WriteLine($"{experimentName},{sequenceName},{dataSize},{learningTimeSeconds},{ramUsage},{cpuSpeedGHz},{cores},{trainingAccuracy},{dotNetVersion}");
                }

                Console.WriteLine(" CSV Write Successful!");
            }
            catch (Exception ex)
            {
                Console.WriteLine($" ERROR: Could not write to CSV file. Exception: {ex.Message}");
            }
        }
    }
}