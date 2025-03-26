using System;
using System.Collections.Generic;

namespace NeocortexApiSamplePerformance
{
    /// <summary>
    /// Defines the structure for input parameters used in a multi-sequence HTM experiment.
    /// </summary>
    public interface IMultiSequenceInput
    {
        /// <summary>
        /// Unique identifier for the experiment run.
        /// </summary>
        int ExperimentId { get; set; }

        /// <summary>
        /// Descriptive name of the experiment configuration.
        /// </summary>
        string ExperimentName { get; set; }

        /// <summary>
        /// Number of CPU cores used during the experiment.
        /// </summary>
        int CPU { get; set; }

        /// <summary>
        /// Version of the .NET runtime in use.
        /// </summary>
        string DotnetVersion { get; set; }

        /// <summary>
        /// Width of the encoding input space.
        /// </summary>
        int W { get; set; }

        /// <summary>
        /// Total number of bits used by the encoder.
        /// </summary>
        int N { get; set; }

        /// <summary>
        /// Number of columns in the spatial pooler.
        /// </summary>
        int numColumns { get; set; }

        /// <summary>
        /// Inhibition radius used in spatial pooling.
        /// </summary>
        double Radius { get; set; }

        /// <summary>
        /// Indicates whether input should be treated as periodic (wraps around).
        /// </summary>
        bool Periodic { get; set; }

        /// <summary>
        /// Whether to clip input values during encoding.
        /// </summary>
        bool ClipInput { get; set; }

        /// <summary>
        /// Identifier for the input dataset or sequence group.
        /// </summary>
        string Name { get; set; }

        /// <summary>
        /// Number of cells allocated per column in temporal memory.
        /// </summary>
        int CellsPerColumn { get; set; }

        /// <summary>
        /// Enables or disables global inhibition in spatial pooling.
        /// </summary>
        bool GlobalInhibition { get; set; }

        /// <summary>
        /// Density of active columns in local inhibition area.
        /// </summary>
        double LocalAreaDensity { get; set; }

        /// <summary>
        /// Number of active columns within each inhibition area.
        /// </summary>
        double NumActiveColumnsPerInhArea { get; set; }

        /// <summary>
        /// Minimum number of active synapses required for a segment to be considered active.
        /// </summary>
        int ActivationThreshold { get; set; }

        /// <summary>
        /// Permanence value above which a synapse is considered connected.
        /// </summary>
        double ConnectedPermanence { get; set; }

        /// <summary>
        /// Value by which permanence is decreased during learning.
        /// </summary>
        double PermanenceDecrement { get; set; }

        /// <summary>
        /// Value by which permanence is increased during learning.
        /// </summary>
        double PermanenceIncrement { get; set; }

        /// <summary>
        /// Amount to decrement permanence on incorrectly predicted segments.
        /// </summary>
        double PredictedSegmentDecrement { get; set; }
        /// <summary>
        /// Length of the input sequence used during training.
        /// </summary>
        int SequenceLength { get; set; }

        /// <summary>
        /// Collection of named sequences. Each key maps to a sequence of input values.
        /// </summary>
        Dictionary<string, List<double>> Sequences { get; set; }
    }
}
