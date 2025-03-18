using System;
using System.Collections.Generic;

namespace NeocortexApiSamplePerformance
{
    public class MultiSequenceInput : IMultiSequenceInput
    {
        public int ExperimentId { get; set; }
        public string ExperimentName { get; set; } = string.Empty;
        public int CPU { get; set; }
        public string DotnetVersion { get; set; } = string.Empty;
        public int W { get; set; }
        public int N { get; set; } = 100;
        public int numColumns { get; set; } = 1024;
        public double Radius { get; set; } = -1.0;
        public bool Periodic { get; set; } = false;
        public bool ClipInput { get; set; } = false;
        public string Name { get; set; } = "Default";
        public int CellsPerColumn { get; set; } = 25;
        public bool GlobalInhibition { get; set; } = true;
        public double LocalAreaDensity { get; set; } = -1;
        public double NumActiveColumnsPerInhArea { get; set; } = 0.02;
        public int ActivationThreshold { get; set; } = 15;
        public double ConnectedPermanence { get; set; } = 0.5;
        public double PermanenceDecrement { get; set; } = 0.25;
        public double PermanenceIncrement { get; set; } = 0.15;
        public double PredictedSegmentDecrement { get; set; } = 0.1;
        public int SequenceLength { get; set; }

        //  Store multiple sequences dynamically
        public Dictionary<string, List<double>> Sequences { get; set; } = new Dictionary<string, List<double>>();
    }
}
