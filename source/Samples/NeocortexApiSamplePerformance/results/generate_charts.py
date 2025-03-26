import pandas as pd
import matplotlib.pyplot as plt
import seaborn as sns

# Load CSV
df = pd.read_csv("Final_Result.csv")

# Clean column names (remove extra spaces)
df.columns = df.columns.str.strip()

# Set seaborn style
sns.set(style="whitegrid")

# === Plot 1: Learning Time vs. Sequence Length (Data Size) ===
plt.figure(figsize=(8, 5))
sns.scatterplot(data=df, x="Data Size", y="Learning Time (s)", hue="Experiment Name")
plt.title("Learning Time vs. Sequence Length")
plt.xlabel("Sequence Length")
plt.ylabel("Learning Time (s)")
plt.tight_layout()
plt.savefig("learning_time_vs_sequence_length.png")
plt.close()

# === Plot 2: Learning Time vs. CPU Cores ===
avg_cores_group = df.groupby("Cores Used")["Learning Time (s)"].mean().reset_index()

plt.figure(figsize=(8, 5))
sns.barplot(data=avg_cores_group, x="Cores Used", y="Learning Time (s)", palette="Blues")
plt.title("Avg. Learning Time vs. CPU Cores")
plt.ylabel("Avg. Learning Time (s)")
plt.xlabel("CPU Cores")
plt.tight_layout()
plt.savefig("learning_time_vs_cpu_cores.png")
plt.close()

# === Plot 3: Learning Time vs. CPU Speed ===
plt.figure(figsize=(8, 5))
sns.scatterplot(data=df, x="CPU Speed (GHz)", y="Learning Time (s)", hue="Experiment Name")
plt.title("Learning Time vs. CPU Speed")
plt.xlabel("CPU Speed (GHz)")
plt.ylabel("Learning Time (s)")
plt.tight_layout()
plt.savefig("learning_time_vs_cpu_speed.png")
plt.close()

# === Plot 4: Learning Time vs. .NET Version ===
avg_dotnet_group = df.groupby("DotNet Version")["Learning Time (s)"].mean().reset_index()

plt.figure(figsize=(8, 5))
sns.barplot(data=avg_dotnet_group, x="DotNet Version", y="Learning Time (s)", palette="Purples")
plt.title("Avg. Learning Time vs. .NET Version")
plt.ylabel("Avg. Learning Time (s)")
plt.xlabel(".NET Version")
plt.tight_layout()
plt.savefig("learning_time_vs_dotnet_version.png")
plt.close()

# === Plot 5: Accuracy vs. Sequence Length ===
plt.figure(figsize=(8, 5))
sns.scatterplot(data=df, x="Data Size", y="Training Accuracy (%)", label="Training", color="green")
plt.title("Training Accuracy vs. Sequence Length")
plt.xlabel("Sequence Length")
plt.ylabel("Accuracy (%)")
plt.legend()
plt.tight_layout()
plt.savefig("accuracy_vs_sequence_length.png")
plt.close()

print(" All charts generated successfully!")
