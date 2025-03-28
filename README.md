[![license](https://img.shields.io/github/license/mashape/apistatus.svg?maxAge=2592000)](https://github.com/ddobric/htmdotnet/blob/master/LICENSE)
[![buildStatus](https://github.com/ddobric/neocortexapi/workflows/.NET%20Core/badge.svg)](https://github.com/ddobric/neocortexapi/actions?query=workflow%3A%22.NET+Core%22)

# ML 24/25-05 Implement Performance Measurement Experiment

## Abstract:
To optimize computational resources, machine learning applications require efficient performance assessment. This project aims to implement a performance measurement experiment focused on MultiSequence Learning using Hierarchical Temporal Memory, which is a version of a biologically inspired machine learning framework that processes sequential data over Sparse Distributed Representations (SDRs). To improve computational efficiency, SDRs assist in the effective encoding of the input sequences. As a means to evaluate the performance, the console application allows users to specify parameters such as sequence folder, output file location, number of CPU cores, and CPU affinity, which automatically runs the experiment using the IExperiment interface ( default experiment MultiSequence Learning experiment) and records key performance metrics in a CSV in the result folder and performance diagrams are generated with the metrics to analyze the relationship between the hardware configurations software environments, and computational efficiency

## Introduction:
To ensure optimal resource utilization, especially when processing large datasets, machine learning models need effective performance evaluation. MultiSequence Learning method is designed in a way to detect and predict patterns across multiple sequences which is particularly important for time-series analysis, anomaly detection, and forecasting tasks. In this project, we implemented a console-based application that allows users to run the experiment and at the same time can monitor the key performance metrics such as learning time, CPU usage, and memory consumption. This application can manage variable configurable parameters, including the sequence dataset location, number of CPU cores, and .NET runtime version giving the outlook of performance analysis.

To implement multi-sequence learning using HTM, the process starts with Sparse Distributed Representations (SDRs), where input is converted to binary with a small percentage of active bits and the encoding maintains semantic similarity which makes the model robust to noise letting it identify similar inputs efficiently. Then the Spatial Pooler processes the SDRs to produce stable and coherent representations. Multisequence learning using HTM is the most effective method for recognizing and predicting patterns throughout multiple input sequences.

This project is aimed at measuring the performance of an HTM-based multisequence learning implementation under various conditions. This experiment, which is implemented around multisequence learning, to quantify the learning time which is required for the HTM to learn sequences of different lengths, evaluate the CPU core utilization, and different runtime environments (.NET versions) on training speed and learning time affect the execution performance.

## Requirements:
To develop and run this project, we need.
- [Visual Studio 2022](https://visualstudio.microsoft.com/downloads/)
- [.NET 8.0 SDK](https://dotnet.microsoft.com/en-us/download/dotnet/8.0) and [.NET 9.0 SDK](https://dotnet.microsoft.com/en-us/download/dotnet/9.0)
- [Python on Windows](https://learn.microsoft.com/en-us/windows/python/beginners#install-python)

## Environment setup:
To set up the development platform, we have to.
- Install Visual Studio 2022 via [Visual Studio Installer](https://visualstudio.microsoft.com/downloads/)
- Download and install [.NET 9.0 SDK](https://dotnet.microsoft.com/en-us/download/dotnet/9.0). The latest version is recommended.
- For project purposes, required to install alongside [.NET 8.0 SDK](https://dotnet.microsoft.com/en-us/download/dotnet/8.0)
- Install Python on Windows via [Microsoft Store](https://apps.microsoft.com/detail/9pnrbtzxmb4z?hl=de-DE&gl=DE)

## Application setup:

## Code demonstration:
`Program.cs `is the main entry of the application that handles:

1. CLI argument parsing

2. CPU affinity setup

3. Dataset loading

4. Experiment execution

5. Performance logging

```csharp
namespace NeocortexApiSamplePerformance
{
    class Program
    {
        static void Main(string[] args)
        {
            Console.Clear();
            Console.WriteLine(" Starting Experiment...\n");

            try
            {
                //  Parse CLI Arguments
                InputParameter inputParameter = ParseArguments(args);
                ValidateInputDirectory(inputParameter.SequenceFolder);

                // Apply CPU Affinity
                int activeCores = SetCpuAffinity(inputParameter.CpuAffinity);
                inputParameter.CpuCores = activeCores;

                // Load Sequences
                List<Sequence> trainingSequences = LoadDatasets(inputParameter.SequenceFolder, "dataset_*.json");
                List<Sequence> testSequences = LoadDatasets(inputParameter.SequenceFolder, "test_*.json");

                // Load Experiment Class
                IExperiment experiment = LoadExperiment(inputParameter.ExperimentClass);

                // Run
                RunExperiment(inputParameter, trainingSequences, testSequences, experiment);
            }
            catch (Exception ex)
            {
                Console.WriteLine($" Fatal Error: {ex.Message}");
            }

            Console.WriteLine("\n Experiment Completed. Press Enter to exit.");
            Console.ReadLine();
        }
    }
}

```
### Key Methods
## `RunExperiment()` – Training, Testing, and Logging
This method runs the selected experiment, times the learning process, computes prediction accuracy, and logs key performance metrics:

```csharp

private static void RunExperiment(InputParameter inputParameter, List<Sequence> trainingSequences, List<Sequence> testSequences, IExperiment experiment)
{
    Stopwatch trainingTimer = Stopwatch.StartNew();
    var processedTrainingData = trainingSequences
        .GroupBy(seq => seq.name)
        .Select(g => new Sequence { name = g.Key, Data = g.SelectMany(seq => seq.Data).ToArray() })
        .ToList();

    var predictor = experiment.Run(processedTrainingData);
    trainingTimer.Stop();

    double learningTimeSeconds = trainingTimer.Elapsed.TotalSeconds;
    var (ramUsage, cpuSpeedGHz, dotNetVersion) = GetSystemPerformanceMetrics();

    // Test loop and performance log
    foreach (var testSeq in testSequences)
    {
        predictor.Reset();
        int matchCount = 0;

        for (int i = 1; i < testSeq.Data.Length; i++)
        {
            var res = predictor.Predict((int)testSeq.Data[i - 1]);
            if (res.Count > 0)
            {
                int predicted = int.Parse(res.First().PredictedInput.Split('-').Last());
                if (testSeq.Data[i] == predicted) matchCount++;
            }
        }

        double testAccuracy = (double)matchCount / (testSeq.Data.Length - 1) * 100.0;

        foreach (var seq in processedTrainingData)
        {
            double trainAccuracy = experiment.GetAccuracy(seq.name);
            PerformanceLogger.LogPerformance(
                inputParameter.ExperimentClass,
                inputParameter,
                seq.name,
                seq.Data.Length,
                learningTimeSeconds,
                ramUsage,
                cpuSpeedGHz,
                inputParameter.CpuCores,
                trainAccuracy,
                dotNetVersion
            );
        }
    }
}


```

## `LoadExperiment()` – Dynamic Class Loading via Reflection

```csharp

private static IExperiment LoadExperiment(string experimentClassName)
{
    Type experimentType = Type.GetType($"NeocortexApiSamplePerformance.{experimentClassName}");

    if (experimentType == null || !typeof(IExperiment).IsAssignableFrom(experimentType))
        throw new ArgumentException($"Invalid experiment: {experimentClassName}");

    return (IExperiment)Activator.CreateInstance(experimentType);
}

```
## `SetCpuAffinity()` – Control Which Cores Are Used

```csharp
private static int SetCpuAffinity(int affinityBitmask)
{
    Process process = Process.GetCurrentProcess();
    process.ProcessorAffinity = (IntPtr)affinityBitmask;

    long appliedAffinity = process.ProcessorAffinity.ToInt64();
    var activeCores = GetActiveCores((int)appliedAffinity);

    Console.WriteLine($" Active Cores: {string.Join(", ", activeCores)}");
    return activeCores.Count;
}

```

## `GetSystemPerformanceMetrics()` – Fetch RAM, CPU GHz, .NET Version

```csharp
private static (double ramUsage, double cpuSpeedGHz, string dotNetVersion) GetSystemPerformanceMetrics()
{
    double ramUsage = GetRamUsage();
    double cpuSpeedGHz = GetCpuSpeedGHz();
    string dotNetVersion = GetDotNetVersion();

    return (ramUsage, cpuSpeedGHz, dotNetVersion);
}

```
## Result
Learning Time vs CPU Cores

![learning_time_vs_cpu_cores](https://github.com/user-attachments/assets/365e0755-1d79-416d-8269-d2126468a9ac)

Learning Time vs 

![learning_time_vs_dotnet_version](https://github.com/user-attachments/assets/58f896f5-33f5-449c-8d16-019e81fd4189)


![learning_time_vs_cpu_speed](https://github.com/user-attachments/assets/3c07b247-ae35-40cf-a719-d287734fb753)







## Conclusion

In this project, a modular and extendable experimental framework for assessing the performance of Hierarchical Temporal Memory (HTM) based learning, especially the MultiSequence Learning model, is successfully implemented. Dynamic control over CPU affinity, core selection, and .NET runtime versions were made possible during execution by including a console-based runner with variable parameters.
Key performance metrics such as learning time, RAM usage, CPU speed, and runtime versions were logged in CSV outputs. The experiments results showed a relationship between CPU core count and learning efficiency, confirming the advantage of parallelization. Also, the comparison among dotnet versions showed significant runtime optimizations, where .NET 9.0.3 outperformed .NET 8.0.14 in most configurations. IExperiment interface which uses reflection makes this application flexible to reuse for wider HTM research and testing.

## Getting started
To get started, please see <a href="https://github.com/ddobric/neocortexapi/blob/master/source/Documentation/gettingStarted.md">this document.</a>

# References

HTM School:
https://www.youtube.com/playlist?list=PL3yXMgtrZmDqhsFQzwUC9V8MeeVOQ7eZ9&app=desktop

HTM Overview:
https://en.wikipedia.org/wiki/Hierarchical_temporal_memory

A Machine Learning Guide to HTM:
https://numenta.com/blog/2019/10/24/machine-learning-guide-to-htm

Numenta on Github:
https://github.com/numenta

HTM Community:
https://numenta.org/

A deep dive in HTM Temporal Memory algorithm:
https://numenta.com/assets/pdf/temporal-memory-algorithm/Temporal-Memory-Algorithm-Details.pdf

Continious Online Sequence Learning with HTM:
https://www.mitpressjournals.org/doi/full/10.1162/NECO_a_00893#.WMBBGBLytE6

# Papers and conference proceedings

#### International Journal of Artificial Intelligence and Applications
Scaling the HTM Spatial Pooler

Dobric, Pech, Ghita, Wennekers 2020. 2020 International Journal of Artificial Intelligence and Applications. Scaling the HTM Spatial Pooler. doi:10.5121/ijaia .2020.11407

#### AIS 2020 - 6th International Conference on Artificial Intelligence and Soft Computing (AIS 2020), Helsinki
The Parallel HTM Spatial Pooler with Actor Model

Dobric, Pech, Ghita, Wennekers 2020. 2020 AIS 2020 - 6th International Conference on Artificial Intelligence and Soft Computing, Helsinki. The Parallel HTM Spatial Pooler with Actor Model. https://aircconline.com/csit/csit1006.pdf, doi:10.5121/csit.2020.100606

#### Symposium on Pattern Recognition and Applications - Rome, Italy
On the Relationship Between Input Sparsity and Noise Robustness in Hierarchical Temporal Memory Spatial Pooler 

Dobric, Pech, Ghita, Wennekers 2020. 2020 Symposium on Pattern Recognition and Applications. On the Relationship Between Input Sparsity and Noise Robustness in Hierarchical Temporal Memory Spatial Pooler. https://dl.acm.org/doi/10.1145/3393822.3432317. doi:10.1145/3393822.3432317

#### International Conference on Pattern Recognition Applications and Methods - ICPRAM 2021
Improved HTM Spatial Pooler with Homeostatic Plasticity Control (Awarded with: *Best Industrial Paper*)

Dobric, Pech, Ghita, Wennekers 2021. ICPRAM Vienna Improved HTM Spatial Pooler with Homeostatic Plasticity control. doi:10.5220/0010314200980106

#### Springer Nature - Computer Sciences
On the Importance of the Newborn Stage When Learning Patterns with the Spatial Pooler

Dobric, Pech, Ghita, Wennekers 2022. Springer Nature Computer Science Journal
On the Importance of the Newborn Stage When Learning Patterns with the Spatial Pooler. https://rdcu.be/cIcoc. doi:10.1007/s42979-022-01066-4
