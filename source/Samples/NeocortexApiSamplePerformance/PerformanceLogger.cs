using System;
using System.IO;

namespace NeocortexApiSamplePerformance
{
    public static class PerformanceLogger
    {
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