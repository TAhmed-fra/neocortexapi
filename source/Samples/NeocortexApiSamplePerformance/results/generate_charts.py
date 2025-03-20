import pandas as pd
import matplotlib.pyplot as plt
import numpy as np

# Load CSV data
csv_file = "NewOuputCPUCORESPEED.csv"  # Replace with actual file
df = pd.read_csv(csv_file)

# Convert columns to numeric (if needed)
df["Training Accuracy (%)"] = pd.to_numeric(df["Training Accuracy (%)"], errors='coerce')
df["Learning Time (s)"] = pd.to_numeric(df["Learning Time (s)"], errors='coerce')
df["Data Size"] = pd.to_numeric(df["Data Size"], errors='coerce')
df["Cores Used"] = pd.to_numeric(df["Cores Used"], errors='coerce')
df["CPU Speed (GHz)"] = pd.to_numeric(df["CPU Speed (GHz)"], errors='coerce')

# Function to plot scatter with trend line
def plot_scatter_with_trend(x, y, xlabel, ylabel, title, filename, color):
    plt.figure(figsize=(8, 5))
    plt.scatter(x, y, c=color, label="Learning Time", s=50, alpha=0.75, edgecolors="black")
    
    #  Trend Line (Regression Line)
    if len(x) > 1:  # Ensure there's enough data for trend line
        z = np.polyfit(x, y, 1)  # Linear Fit
        p = np.poly1d(z)
        plt.plot(x, p(x), linestyle="--", color="black", label="Trend Line")

    plt.xlabel(xlabel, fontsize=12)
    plt.ylabel(ylabel, fontsize=12)
    plt.title(title, fontsize=14, fontweight="bold")
    plt.legend()
    plt.grid(True, linestyle="--", alpha=0.6)
    plt.savefig(filename)
    print(f" Chart saved: {filename}")

#  Plot Learning Time vs. Number of Elements
df_sorted = df.sort_values(by="Data Size")
plot_scatter_with_trend(df_sorted["Data Size"], df_sorted["Learning Time (s)"],
                         "Number of Elements in Sequence", "Learning Time (s)",
                         "Learning Time vs. Number of Elements", "learning_time_vs_elements.png", "blue")

#  Plot Learning Time vs. CPU Cores
df_sorted = df.sort_values(by="Cores Used")
plot_scatter_with_trend(df_sorted["Cores Used"], df_sorted["Learning Time (s)"],
                         "Number of CPU Cores Used", "Learning Time (s)",
                         "Learning Time vs. CPU Cores", "learning_time_vs_cores.png", "red")

#  Plot Learning Time vs. CPU Speed
df_sorted = df.sort_values(by="CPU Speed (GHz)")
plot_scatter_with_trend(df_sorted["CPU Speed (GHz)"], df_sorted["Learning Time (s)"],
                         "CPU Speed (GHz)", "Learning Time (s)",
                         "Learning Time vs. CPU Speed", "learning_time_vs_cpu_speed.png", "green")

print(" All Charts Generated Successfully!")
