public class InputParameter
{
    public string SequenceFolder { get; set; }
    public string OutputCsvPath { get; set; }
    public int CpuCores { get; set; }
    public double CpuSpeedGHz { get; set; }
    public string DotnetVersion { get; set; }
    public Dictionary<string, List<double>> Sequences { get; set; }
    public string ExperimentClass { get; set; }

    
    public int CpuAffinity { get; set; }
}
