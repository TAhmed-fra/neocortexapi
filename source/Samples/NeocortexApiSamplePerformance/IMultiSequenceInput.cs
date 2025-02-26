using System;
using System.Collections.Generic;

namespace NeocortexApiSamplePerformance
{
    /// <summary>
    /// 5️⃣ IMultiSequenceInput.cs - Interface for multi-sequence input configurations
    /// </summary>
    public interface IMultiSequenceInput
    {
        /// <summary>
        /// Gets or sets the ID of the experiment.
        /// </summary>
        int ExperimentId { get; set; }

        /// <summary>
        /// Gets or sets the name of the experiment.
        /// </summary>
        string ExperimentName { get; set; }

        /// <summary>
        /// Gets or sets the number of CPU cores used.
        /// </summary>
        int CPU { get; set; }

        /// <summary>
        /// Gets or sets the .NET version being used.
        /// </summary>
        string DotnetVersion { get; set; }

        /// <summary>
        /// Gets or sets the width of the input.
        /// </summary>
        int W { get; set; }

        /// <summary>
        /// Gets or sets the number of columns in the input space.
        /// </summary>
        int N { get; set; }

        /// <summary>
        /// Gets or sets the total number of columns.
        /// </summary>
        int numColumns { get; set; }

        /// <summary>
        /// Gets or sets the potential connection radius.
        /// </summary>
        double Radius { get; set; }

        /// <summary>
        /// Gets or sets whether the input is periodic.
        /// </summary>
        bool Periodic { get; set; }

        /// <summary>
        /// Gets or sets whether input values are clipped.
        /// </summary>
        bool ClipInput { get; set; }

        /// <summary>
        /// Gets or sets the name of the configuration.
        /// </summary>
        string Name { get; set; }

        /// <summary>
        /// Gets or sets the number of cells per column.
        /// </summary>
        int CellsPerColumn { get; set; }

        /// <summary>
        /// Gets or sets whether global inhibition is enabled.
        /// </summary>
        bool GlobalInhibition { get; set; }

        /// <summary>
        /// Gets or sets the density of local area inhibition.
        /// </summary>
        double LocalAreaDensity { get; set; }

        /// <summary>
        /// Gets or sets the number of active columns per inhibition area.
        /// </summary>
        double NumActiveColumnsPerInhArea { get; set; }

        /// <summary>
        /// Gets or sets the activation threshold.
        /// </summary>
        int ActivationThreshold { get; set; }

        /// <summary>
        /// Gets or sets the connected permanence threshold.
        /// </summary>
        double ConnectedPermanence { get; set; }

        /// <summary>
        /// Gets or sets the permanence decrement during learning.
        /// </summary>
        double PermanenceDecrement { get; set; }

        /// <summary>
        /// Gets or sets the permanence increment during learning.
        /// </summary>
        double PermanenceIncrement { get; set; }

        /// <summary>
        /// Gets or sets the predicted segment decrement.
        /// </summary>
        double PredictedSegmentDecrement { get; set; }

        /// <summary>
        /// Gets or sets the sequence length.
        /// </summary>
        int SequenceLength { get; set; }

        /// <summary>
        /// Gets or sets the first sequence input values.
        /// </summary>
        List<double> Sequence1 { get; set; }

        /// <summary>
        /// Gets or sets the second sequence input values.
        /// </summary>
        List<double> Sequence2 { get; set; }
    }
}
