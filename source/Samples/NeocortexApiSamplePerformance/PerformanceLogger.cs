using System;
using System.Collections.Generic;
using System.IO;

namespace NeocortexApiSamplePerformance
{
    public static class PerformanceLogger
    {
        public static void LogPerformance(InputParameter inputParameter, long executionTime)
        {
            bool fileExists = File.Exists(inputParameter.outputCsvPath);

            using (StreamWriter writer = new StreamWriter(inputParameter.outputCsvPath, true))
            {
                if (!fileExists)
                {
                    writer.WriteLine("ExperimentName,SequenceLength,CPU_Cores,CPU_Speed_GHz,DotnetVersion,ExecutionTime_ms");
                }

                foreach (var seq in inputParameter.sequences)
                {
                    writer.WriteLine($"{seq.Key},{seq.Value.Count},{inputParameter.cpuCores},{inputParameter.cpuSpeed},{inputParameter.dotnetVersion},{executionTime}");
                }
            }

            Console.WriteLine($"Performance data saved to {inputParameter.outputCsvPath}");
        }
    }
}
