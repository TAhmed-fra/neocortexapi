
namespace NeocortexApiSamplePerformance
{
    /// <summary>
    /// Represents a single named sequence used for training or testing the HTM model.
    /// </summary>
    public class Sequence
    {
        /// <summary>
        /// Gets or sets the name of the sequence.
        /// This is used to group or identify sequences during training and evaluation.
        /// </summary>
        public string name { get; set; }

        // <summary>
        /// Gets or sets the numerical values in the sequence.
        /// These values represent the ordered inputs that will be encoded and learned.
        /// </summary>
        public double[] Data { get; set; }

    }
}