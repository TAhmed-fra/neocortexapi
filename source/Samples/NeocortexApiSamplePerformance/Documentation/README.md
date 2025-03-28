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
## Key Methods
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
## How To Run The Application

Select `NeocortexApiSamplePerformance` from [NeoCortexApi.sln](https://github.com/TAhmed-fra/neocortexapi/blob/PerfEX/source/NeoCortexApi.All.sln)

Open the command prompt in [Project folder](https://github.com/TAhmed-fra/neocortexapi/tree/PerfEX/source/Samples/NeocortexApiSamplePerformance)

Run the below command in the opened terminal

```csharp
dotnet run --sequence-folder "<path_to_dataset>" --out "<output_csv_path>" --cores <number_of_cores> --affinity <cpu_affinity_bitmask> --experiment <experiment_class_name>

```

Example

```csharp
dotnet run --sequence-folder "C:\Users\shiha\SE-2024\neocortexapi\source\Samples\NeocortexApiSamplePerformance\Dataset" --out "C:\Users\shiha\SE-2024\neocortexapi\source\Samples\NeocortexApiSamplePerformance\results\Final_Result.csv" --cores 4 --affinity 15 --experiment MultiSequenceLearning
```
### CPU Core Affinity Reference

When specifying the `--affinity` argument, you pass a **bitmask** to define which CPU cores should be utilized. The following table illustrates common affinity values and their corresponding active cores:

| Decimal Affinity Value | Number of Cores Used | Corresponding Cores |
|------------------------|----------------------|---------------------|
| 1                      | 1 Core               | 0                   |
| 3                      | 2 Cores              | 0, 1                |
| 7                      | 3 Cores              | 0, 1, 2             |
| 15                     | 4 Cores              | 0, 1, 2, 3          |
| 31                     | 5 Cores              | 0, 1, 2, 3, 4       |
| 63                     | 6 Cores              | 0, 1, 2, 3, 4, 5    |
| 127                    | 7 Cores              | 0, 1, 2, 3, 4, 5, 6 |
| 255                    | 8 Cores              | 0, 1, 2, 3, 4, 5, 6, 7 |

> **Note:** Bitmask values are binary representations of enabled cores. For example, `15` is `00001111` in binary, which activates cores 0 through 3.

### Command Line Argument Breakdown

| Argument Flag        | Description                                                                                   | Example Value                                                                 |
|----------------------|-----------------------------------------------------------------------------------------------|--------------------------------------------------------------------------------|
| `--sequence-folder`  | Path to the folder containing input sequence and test JSON files.                             | `C:\...\NeocortexApiSamplePerformance\Dataset`                                 |
| `--out`              | Path to save the output CSV file where performance results will be logged.                   | `C:\...\NeocortexApiSamplePerformance\results\Final_Result.csv`               |
| `--cores`            | Number of CPU cores intended to use for the experiment.                                       | `4`                                                                            |
| `--affinity`         | Bitmask value used to control which CPU cores are active during the experiment.              | `15` (enables cores 0, 1, 2, and 3)                                            |
| `--experiment`       | Class name of the experiment that implements the `IExperiment` interface.                    | `MultiSequenceLearning`                                                       |
>  **.NET Version Switching Tip:**  
> Change the target .NET runtime version for the experiment by editing the `<TargetFramework>` tag inside the `NeocortexApiSamplePerformance.csproj` file.  
>  
> For example, to switch from `.NET 8.0` to `.NET 9.0`, update the line:
>
> ```xml
> <TargetFramework>net8.0</TargetFramework>
> ```
> to:
> ```xml
> <TargetFramework>net9.0</TargetFramework>
> ```
>  
> After saving the file, rebuild the project to run the experiment under the selected runtime environment.


## Result Visualization
After running the experiment with various configurations, the performance data is saved in a CSV file (e.g., Final_Result.csv or Result.csv). To analyze and visualize the performance trends, a Python script named `generate_charts.py` is provided inside the [results/](https://github.com/TAhmed-fra/neocortexapi/tree/PerfEX/source/Samples/NeocortexApiSamplePerformance/results) directory.

>  Tip: Ensure Python packages are installed before running `generate_charts.py`

```bash
pip install pandas matplotlib seaborn

```
> **Note**
The Python script generate_charts.py expects a specific CSV file to read.
You must ensure that the filename inside the script matches the one provided in the --out argument.

Inside generate_charts.py, this line:

```bash
df = pd.read_csv("Final_Result.csv")
```
It should be updated if you used a different output filename. For example:


```bash
df = pd.read_csv("myresults.csv")
```
This script performs the following:

Reads the CSV output file containing learning time, CPU configuration, and system specs.

Cleans and processes the data using pandas and seaborn.

Generates bar plots and saves them as PNGs in the same folder.

## Result

## Console Output


[![Console output 1](https://github.com/user-attachments/assets/4a84af70-7ef3-456d-840c-d432cb476a6b)](https://github.com/TAhmed-fra/neocortexapi/blob/PerfEX/source/Samples/NeocortexApiSamplePerformance/results/Console%20output%201.png)

## Sample Output CSV (Result.csv)

| Experiment Name     | Sequence Name | Sequence Length | Learning Time (s) | CPU Speed (GHz) | CPU Cores | .NET Version   |
|---------------------|----------------|------------------|-------------------|------------------|------------|----------------|
| MultiSequenceLearning | testSeq1      | 25               | 5.23              | 2.40             | 4          | .NET 9.0.3     |


## Diagram generated from output CSV

Learning Time vs CPU Cores

[![learning_time_vs_cpu_cores](https://github.com/user-attachments/assets/365e0755-1d79-416d-8269-d2126468a9ac)](https://github.com/TAhmed-fra/neocortexapi/blob/PerfEX/source/Samples/NeocortexApiSamplePerformance/results/learning_time_vs_cpu_cores.png)

Learning Time vs .NET Version

[![learning_time_vs_dotnet_version](https://github.com/user-attachments/assets/58f896f5-33f5-449c-8d16-019e81fd4189)](https://github.com/TAhmed-fra/neocortexapi/blob/PerfEX/source/Samples/NeocortexApiSamplePerformance/results/learning_time_vs_dotnet_version.png)

Learning Time vs CPU Speed

[![learning_time_vs_cpu_speed](https://github.com/user-attachments/assets/3c07b247-ae35-40cf-a719-d287734fb753)](https://github.com/TAhmed-fra/neocortexapi/blob/PerfEX/source/Samples/NeocortexApiSamplePerformance/results/learning_time_vs_cpu_speed.png)







## Conclusion

In this project, a modular and extendable experimental framework for assessing the performance of Hierarchical Temporal Memory (HTM) based learning, especially the MultiSequence Learning model, is successfully implemented. Dynamic control over CPU affinity, core selection, and .NET runtime versions were made possible during execution by including a console-based runner with variable parameters.
Key performance metrics such as learning time, RAM usage, CPU speed, and runtime versions were logged in CSV outputs. The experiments results showed a relationship between CPU core count and learning efficiency, confirming the advantage of parallelization. Also, the comparison among dotnet versions showed significant runtime optimizations, where .NET 9.0.3 outperformed .NET 8.0.14 in most configurations. IExperiment interface which uses reflection makes this application flexible to reuse for wider HTM research and testing.


## NuGet Packages Used

| Package | Description |
|--------|-------------|
| [`NeoCortexApi`](https://www.nuget.org/packages/NeoCortexApi/) | Core HTM library implementing Spatial Pooler and Temporal Memory. |
| [`Newtonsoft.Json`](https://www.nuget.org/packages/Newtonsoft.Json/) | Handles JSON serialization/deserialization. |


# References

Forked from [ddobric/neocortexapi](https://github.com/ddobric/neocortexapi)

[Hawkins, J., & Blakeslee, S. (2004). On Intelligence. Times Books](https://doi.org/10.2514/1.18111) 






