import os
import subprocess

os.chdir(os.path.dirname(__file__))
print(f"Current working directory is {os.getcwd()}")
subprocess.run(["dotnet", "build", "./Bulldog.sln", "--configuration=Release"], check=True)