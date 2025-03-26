using System;
using System.Collections.Generic;

namespace NeocortexApiSamplePerformance
{
    /// <summary>
    /// Provides a concrete implementation of the multi-sequence input configuration.
    /// Used to parameterize HTM experiments involving multiple input sequences.
    /// </summary>
    public class MultiSequenceInput : IMultiSequenceInput
    {
        /// <summary>
        /// Unique identifier for the experiment instance.
        /// </summary>
        public int ExperimentId { get; set; }

        /// <summary>
        /// Name of the experiment for identification or logging.
        /// </summary>
        public string ExperimentName { get; set; } = string.Empty;

        /// <summary>
        /// Number of CPU cores used during the experiment.
        /// </summary>
        public int CPU { get; set; }

        /// <summary>
        /// The .NET runtime version used during execution.
        /// </summary>
        public string DotnetVersion { get; set; } = string.Empty;

        /// <summary>
        /// Width of the encoder's active bits.
        /// </summary>
        public int W { get; set; }

        /// <summary>
        /// Total number of bits in the encoder output.
        /// </summary>
        public int N { get; set; } = 100;

        /// <summary>
        /// Number of columns in the spatial pooler.
        /// </summary>
        public int numColumns { get; set; } = 1024;

        /// <summary>
        /// Radius used for inhibition in the spatial pooler.
        /// </summary>
        public double Radius { get; set; } = -1.0;

        /// <summary>
        /// Indicates if the input is periodic (e.g., angles or dates).
        /// </summary>
        public bool Periodic { get; set; } = false;

        /// <summary>
        /// Whether to clip input values to stay within expected range.
        /// </summary>
        public bool ClipInput { get; set; } = false;

        /// <summary>
        /// Name for the input source or configuration.
        /// </summary>
        public string Name { get; set; } = "Default";

        /// <summary>
        /// Number of cells per column in the temporal memory.
        /// </summary>
        public int CellsPerColumn { get; set; } = 25;

        /// <summary>
        /// Enables or disables global inhibition across all columns.
        /// </summary>
        public bool GlobalInhibition { get; set; } = true;

        /// <summary>
        /// Local area density for inhibition when global inhibition is off.
        /// </summary>
        public double LocalAreaDensity { get; set; } = -1;

        /// <summary>
        /// Number of active columns per inhibition area.
        /// </summary>
        public double NumActiveColumnsPerInhArea { get; set; } = 0.02;

        /// <summary>
        /// Minimum number of active synapses required to activate a segment.
        /// </summary>
        public int ActivationThreshold { get; set; } = 15;

        /// <summary>
        /// Threshold above which a synapse is considered connected.
        /// </summary>
        public double ConnectedPermanence { get; set; } = 0.5;

        /// <summary>
        /// Amount by which permanence is decreased during learning.
        /// </summary>
        public double PermanenceDecrement { get; set; } = 0.25;

        /// <summary>
        /// Amount by which permanence is increased during learning.
        /// </summary>
        public double PermanenceIncrement { get; set; } = 0.15;

        /// <summary>
        /// Amount by which permanence is reduced for predictive segments that were incorrect.
        /// </summary>
        public double PredictedSegmentDecrement { get; set; } = 0.1;

        /// <summary>
        /// Number of elements in each sequence.
        /// </summary>
        public int SequenceLength { get; set; }

        /// <summary>
        /// Dictionary storing multiple named sequences for training/testing.
        /// KEY is the sequence name, VALUE is the list of elements in that sequence.
        /// </summary>
        public Dictionary<string, List<double>> Sequences { get; set; } = new Dictionary<string, List<double>>();
    }
}
