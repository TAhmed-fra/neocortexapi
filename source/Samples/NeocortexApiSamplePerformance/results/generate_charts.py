import pandas as pd
import matplotlib.pyplot as plt
import seaborn as sns
import numpy as np

# Load CSV
df = pd.read_csv("Result.csv")

# Clean column names 
df.columns = df.columns.str.strip()

# Set seaborn style
sns.set(style="whitegrid")

# Learning Time vs. CPU Cores
avg_cores_group = df.groupby("Cores Used")["Learning Time (s)"].mean().reset_index()

plt.figure(figsize=(8, 5))
sns.barplot(data=avg_cores_group, x="Cores Used", y="Learning Time (s)", palette="Blues")
plt.title("Avg. Learning Time vs. CPU Cores")
plt.ylabel("Avg. Learning Time (s)")
plt.xlabel("CPU Cores")
plt.tight_layout()
plt.savefig("learning_time_vs_cpu_cores.png")
plt.close()

# Learning Time vs. CPU Speed 

min_speed = df["CPU Speed (GHz)"].min()
max_speed = df["CPU Speed (GHz)"].max()

# Expand the range a little to capture edges nicely
bin_edges = np.linspace(np.floor(min_speed * 10) / 10, np.ceil(max_speed * 10) / 10 + 0.05, num=6)

# Create labels
labels = [f"{bin_edges[i]:.2f}–{bin_edges[i+1]:.2f}" for i in range(len(bin_edges) - 1)]

# Assign bins
df["CPU Speed Range"] = pd.cut(df["CPU Speed (GHz)"], bins=bin_edges, labels=labels, include_lowest=True)

# Group and average
avg_speed_group = df.groupby("CPU Speed Range")["Learning Time (s)"].mean().reset_index()

# Plot
plt.figure(figsize=(8, 5))
sns.barplot(data=avg_speed_group, x="CPU Speed Range", y="Learning Time (s)", palette="Greens")
plt.title("Avg. Learning Time vs. CPU Speed")
plt.xlabel("CPU Speed (GHz)")
plt.ylabel("Avg. Learning Time (s)")
plt.tight_layout()
plt.savefig("learning_time_vs_cpu_speed.png")
plt.close()


# Learning Time vs. .NET Version 
avg_dotnet_group = df.groupby("DotNet Version")["Learning Time (s)"].mean().reset_index()

plt.figure(figsize=(8, 5))
sns.barplot(data=avg_dotnet_group, x="DotNet Version", y="Learning Time (s)", palette="Purples")
plt.title("Avg. Learning Time vs. .NET Version")
plt.ylabel("Avg. Learning Time (s)")
plt.xlabel(".NET Version")
plt.tight_layout()
plt.savefig("learning_time_vs_dotnet_version.png")
plt.close()


print(" All charts generated successfully!")
