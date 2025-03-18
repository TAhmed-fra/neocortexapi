using System;
using System.Collections.Generic;

namespace NeocortexApiSamplePerformance
{
    public interface IMultiSequenceInput
    {
        int ExperimentId { get; set; }
        string ExperimentName { get; set; }
        int CPU { get; set; }
        string DotnetVersion { get; set; }
        int W { get; set; }
        int N { get; set; }
        int numColumns { get; set; }
        double Radius { get; set; }
        bool Periodic { get; set; }
        bool ClipInput { get; set; }
        string Name { get; set; }
        int CellsPerColumn { get; set; }
        bool GlobalInhibition { get; set; }
        double LocalAreaDensity { get; set; }
        double NumActiveColumnsPerInhArea { get; set; }
        int ActivationThreshold { get; set; }
        double ConnectedPermanence { get; set; }
        double PermanenceDecrement { get; set; }
        double PermanenceIncrement { get; set; }
        double PredictedSegmentDecrement { get; set; }
        int SequenceLength { get; set; }

        //  Store multiple sequences dynamically
        Dictionary<string, List<double>> Sequences { get; set; }
    }
}
