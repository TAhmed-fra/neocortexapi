using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace NeocortexApiSamplePerformance
{
    public class InputParameter
    {
        public string sequenceFolder {  get; set; }
        public string outputCsvPath { get; set; }
        public string experimentClass { get; set; }
        public int cpuCores { get; set; }
        public double cpuSpeed { get; set; }
        public string dotnetVersion { get; set; }

        public Dictionary<string, List<double>> sequences {  get; set; }
    }
}
