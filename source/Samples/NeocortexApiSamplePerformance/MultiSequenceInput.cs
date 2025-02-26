using System;
using System.Collections.Generic;

namespace NeocortexApiSamplePerformance
{
    /// <summary>
    /// Holds sequence data for experiments
    /// </summary>
    public class MultiSequenceInput : IMultiSequenceInput
    {
        /// <summary>
        /// Unique identifier for the experiment
        /// </summary>
        public int ExperimentId { get; set; }

        /// <summary>
        /// Name of the experiment
        /// </summary>
        public string ExperimentName { get; set; } = string.Empty;

        /// <summary>
        /// Number of CPU cores used in the experiment
        /// </summary>
        public int CPU { get; set; }

        /// <summary>
        /// Version of the .NET framework being used
        /// </summary>
        public string DotnetVersion { get; set; } = string.Empty;

        /// <summary>
        /// Length of the sequence data used in the experiment
        /// </summary>
        public int SequenceLength { get; set; }

        /// <summary>
        /// The first sequence used for learning
        /// </summary>
        public List<double> Sequence1 { get; set; } = new List<double>();

        /// <summary>
        /// The second sequence used for learning
        /// </summary>
        public List<double> Sequence2 { get; set; } = new List<double>();

        /// <summary>
        /// Defines the width of the input space (Must be an odd number)
        /// </summary>
        public int W
        {
            get => _w;
            set => _w = (value % 2 == 0) ? value + 1 : value; 
        }
        private int _w;

        // 
        public double MaxBoost { get; set; } = 1.0;
        public int DutyCyclePeriod { get; set; } = 1000;
        public double MinPctOverlapDutyCycles { get; set; } = 0.001;
        public double MinVal { get; set; } = 0.0;
        public double MaxVal { get; set; } = 255.0;
        public double SynPermConnected { get; set; } = 0.10; // ✅ Replaces obsolete ConnectedPermanence
        public int N { get => throw new NotImplementedException(); set => throw new NotImplementedException(); }
        public int numColumns { get => throw new NotImplementedException(); set => throw new NotImplementedException(); }
        public double Radius { get => throw new NotImplementedException(); set => throw new NotImplementedException(); }
        public bool Periodic { get => throw new NotImplementedException(); set => throw new NotImplementedException(); }
        public bool ClipInput { get => throw new NotImplementedException(); set => throw new NotImplementedException(); }
        public string Name { get => throw new NotImplementedException(); set => throw new NotImplementedException(); }
        public int CellsPerColumn { get => throw new NotImplementedException(); set => throw new NotImplementedException(); }
        public bool GlobalInhibition { get => throw new NotImplementedException(); set => throw new NotImplementedException(); }
        public double LocalAreaDensity { get => throw new NotImplementedException(); set => throw new NotImplementedException(); }
        public double NumActiveColumnsPerInhArea { get => throw new NotImplementedException(); set => throw new NotImplementedException(); }
        public int ActivationThreshold { get => throw new NotImplementedException(); set => throw new NotImplementedException(); }
        public double ConnectedPermanence { get => throw new NotImplementedException(); set => throw new NotImplementedException(); }
        public double PermanenceDecrement { get => throw new NotImplementedException(); set => throw new NotImplementedException(); }
        public double PermanenceIncrement { get => throw new NotImplementedException(); set => throw new NotImplementedException(); }
        public double PredictedSegmentDecrement { get => throw new NotImplementedException(); set => throw new NotImplementedException(); }

        public MultiSequenceInput() { }
    }
}
