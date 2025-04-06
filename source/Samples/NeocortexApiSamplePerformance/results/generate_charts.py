import pandas as pd
import matplotlib.pyplot as plt
import seaborn as sns

# Load CSV (make sure this file exists in your working directory)
df = pd.read_csv("Result.csv")
df.columns = df.columns.str.strip()  # Clean column names

# Set Seaborn style
sns.set(style="whitegrid")

# === Box Plot 1: Learning Time vs. CPU Cores ===
plt.figure(figsize=(8, 5))
sns.boxplot(data=df, x="Cores Used", y="Learning Time (s)", palette="Blues", showfliers=False)
plt.title("Learning Time vs. CPU Cores")
plt.xlabel("CPU Cores")
plt.ylabel("Learning Time (s)")
plt.tight_layout()
plt.savefig("learning_time_vs_cpu_cores.png")
plt.close()

# === Box Plot 2: Learning Time vs. CPU Speed ===
plt.figure(figsize=(8, 5))
sns.boxplot(data=df, x="CPU Speed (GHz)", y="Learning Time (s)", palette="Greens", showfliers=False)
plt.title("Learning Time vs. CPU Speed")
plt.xlabel("CPU Speed (GHz)")
plt.ylabel("Learning Time (s)")
plt.tight_layout()
plt.savefig("learning_time_vs_cpu_speed.png")
plt.close()

# === Box Plot 3: Learning Time vs. .NET Version ===
plt.figure(figsize=(8, 5))
sns.boxplot(data=df, x="DotNet Version", y="Learning Time (s)", palette="Purples", showfliers=False)
plt.title("Learning Time vs. .NET Version")
plt.xlabel(".NET Version")
plt.ylabel("Learning Time (s)")
plt.tight_layout()
plt.savefig("learning_time_vs_dotnet_version.png")
plt.close()

print(" All box plots generated and saved successfully!")
