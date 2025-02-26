using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;

namespace NeocortexApiSamplePerformance
{
    public class ReadData
    {
        public static List<MultiSequenceInput> LoadSequences(string folderPath)
        {
            List<MultiSequenceInput> inputs = new List<MultiSequenceInput>();

            foreach (string file in Directory.GetFiles(folderPath, "*.csv"))
            {
                Console.WriteLine($"Loading sequences from {file}...");

                using (StreamReader reader = new StreamReader(file))
                {
                    string headerLine = reader.ReadLine();
                    while (!reader.EndOfStream)
                    {
                        var line = reader.ReadLine();
                        var values = line.Split(',');

                        var input = new MultiSequenceInput
                        {
                            ExperimentId = int.Parse(values[0]),
                            ExperimentName = values[1],
                            SequenceLength = int.Parse(values[2]),
                            Sequence1 = values[3].Split(';').Select(double.Parse).ToList()
                        };

                        inputs.Add(input);
                    }
                }
            }

            return inputs;
        }
    }
}
