
/// <summary>
/// Represents input arguments and configuration passed to the experiment runner.
/// These values are typically parsed from the command line.
/// </summary>
public class InputParameter
{
    /// <summary>
    /// Path to the folder containing input sequence datasets (JSON files).
    /// </summary>
    public string SequenceFolder { get; set; }

    /// <summary>
    /// File path for writing output performance metrics (CSV format).
    /// </summary>
    public string OutputCsvPath { get; set; }

    /// <summary>
    /// Number of CPU cores assigned to the experiment.
    /// </summary>
    public int CpuCores { get; set; }

    /// <summary>
    /// Clock speed of the CPU used, in GHz.
    /// </summary>
    public double CpuSpeedGHz { get; set; }

    /// <summary>
    /// The version of the .NET runtime being used.
    /// </summary>
    public string DotnetVersion { get; set; }

    /// <summary>
    /// Dictionary of named sequences (optional preloaded sequences).
    /// </summary>
    public Dictionary<string, List<double>> Sequences { get; set; }

    /// <summary>
    /// The name of the experiment class to run (must implement IExperiment).
    /// </summary>
    public string ExperimentClass { get; set; }

    /// <summary>
    /// Bitmask value specifying CPU core affinity.
    /// Used to bind the process to specific logical processors.
    /// </summary>
    public int CpuAffinity { get; set; }
}
