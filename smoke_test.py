import os
import subprocess

os.chdir(os.path.dirname(__file__))
print(f"Current working directory is {os.getcwd()}")
subprocess.run(["dotnet", "run", "--project", "./tests/TestTool/TestTool.csproj", "--framework", "net5.0", "--", "--verbosity", "Debug"], check=True)
