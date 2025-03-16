# This command sets the execution policy to bypass for only the current PowerShell session
# After the window is closed, the next PowerShell session will open running with the default execution policy.
Set-ExecutionPolicy -Scope Process -ExecutionPolicy Bypass

# Remove build and log directories
Get-ChildItem -inc bin,obj,log,logs,node_modules -rec | Remove-Item -rec -force

# Remove all Verify verification files (modify extensions as needed)
Get-ChildItem -Path . -Include *.verified.* -Recurse | Remove-Item -Force