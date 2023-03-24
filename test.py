import os
import subprocess

os.chdir(os.path.dirname(__file__))
print(f"Current working directory is {os.getcwd()}")
subprocess.run(["dotnet", "test", "./Bulldog.sln", "--no-build", "--configuration=Release"], check=True)