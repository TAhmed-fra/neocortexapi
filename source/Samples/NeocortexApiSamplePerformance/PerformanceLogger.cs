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
            double cpuUsage,
            double ramUsage,
            double cpuSpeedGHz,
            int cores,
            double trainingAccuracy)  // 🔹 Using only training accuracy
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
                        // 🔹 Removed testing accuracy, keeping only training accuracy
                        writer.WriteLine("Experiment Name,Sequence Name,Data Size,Learning Time (s),CPU Usage (%),RAM Usage (MB),CPU Speed (GHz),Cores Used,Training Accuracy (%)");
                    }

                    // 🔹 Log training accuracy instead of test accuracy
                    writer.WriteLine($"{experimentName},{sequenceName},{dataSize},{learningTimeSeconds},{cpuUsage},{ramUsage},{cpuSpeedGHz},{cores},{trainingAccuracy}");
                }

                Console.WriteLine("✅ CSV Write Successful!");
            }
            catch (Exception ex)
            {
                Console.WriteLine($"❌ ERROR: Could not write to CSV file. Exception: {ex.Message}");
            }
        }
    }
}


//using System;
//using System.IO;

//namespace NeocortexApiSamplePerformance
//{
//    public static class PerformanceLogger
//    {
//        public static void LogPerformance(
//            string experimentName, 
//            InputParameter input,
//            string sequenceName,
//            int dataSize,
//            double learningTimeSeconds,
//            long learningTimeMilliseconds,
//            double cpuUsage,
//            double ramUsage,
//            int cores,
//            double accuracy)
//        {
//            string outputPath = input.OutputCsvPath;

//            try
//            {
//                //Confirm the Output Path
//                Console.WriteLine($"Writing to CSV: {outputPath}");

//                bool fileExists = File.Exists(outputPath);
//                using (StreamWriter writer = new StreamWriter(outputPath, true))
//                {
//                    if (!fileExists)
//                    {
//                        writer.WriteLine("Experiment Name,Sequence Name,Data Size,Learning Time (s),Learning Time (ms),CPU Usage (%),RAM Usage (MB),Cores Used,Accuracy (%)");
//                    }

//                    writer.WriteLine($"{experimentName},{sequenceName},{dataSize},{learningTimeSeconds},{learningTimeMilliseconds},{cpuUsage},{ramUsage},{cores},{accuracy}");
//                }

//                // Confirm File Write
//                Console.WriteLine(" CSV Write Successful!");
//            }
//            catch (Exception ex)
//            {
//                Console.WriteLine($" ERROR: Could not write to CSV file. Exception: {ex.Message}");
//            }
//        }
//    }
//}
