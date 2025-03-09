using System;
using System.IO;

namespace NeocortexApiSamplePerformance
{
    public static class PerformanceLogger
    {
        public static void LogPerformance(
            InputParameter inputParameter, string sequenceName,
            int sequenceLength, double executionTimeSeconds, long executionTimeMilliseconds,
            double cpuUsage, double ramUsage, int activeCores)
        {
            string filePath = inputParameter.OutputCsvPath;
            bool fileExists = File.Exists(filePath);

            using (StreamWriter writer = new StreamWriter(filePath, append: fileExists))
            {
                // If the file does not exist, write a header row
                if (!fileExists)
                {
                    writer.WriteLine("Timestamp,Experiment,SequenceName,SequenceLength,CPU_Cores,CPU_Speed_GHz,ExecutionTime_s,ExecutionTime_ms,CPU_Usage(%),RAM_Usage(MB),Active_Cores");
                }

                string timestamp = DateTime.Now.ToString("yyyy-MM-dd HH:mm:ss");

                // Write log entry with correct data types
                writer.WriteLine($"{timestamp},{inputParameter.ExperimentClass},{sequenceName},{sequenceLength},{inputParameter.CpuCores},{inputParameter.CpuSpeedGHz},{executionTimeSeconds:F2},{executionTimeMilliseconds},{cpuUsage:F2},{ramUsage:F2},{activeCores}");
            }

            Console.WriteLine($" Performance results logged to {filePath}");
        }
    }
}
