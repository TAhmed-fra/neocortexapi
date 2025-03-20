using NeoCortexApi;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace NeocortexApiSamplePerformance
{
    public interface IExperiment
    {
        /// <summary>
        /// Runs the experiment using a dictionary-based input.
        /// This method is provided for compatibility and will convert the input to List<Sequence>.
        /// </summary>
        Predictor Run(Dictionary<string, List<double>> sequences)
        {
            // Convert Dictionary to List<Sequence> before calling Run(List<Sequence>)
            List<Sequence> sequenceList = sequences.Select(kvp => new Sequence
            {
                name = kvp.Key,
                Data = kvp.Value.ToArray()
            }).ToList();

            return Run(sequenceList);
        }

        /// <summary>
        /// Runs the experiment with list-based input (used by MultiSequenceLearning).
        /// This must be implemented by any experiment class.
        /// </summary>
        Predictor Run(List<Sequence> sequences);

        /// <summary>
        /// Retrieves accuracy for a given sequence name.
        /// </summary>
        double GetAccuracy(string sequenceName);
    }


}
