$ErrorActionPreference = "Stop"

$profiles = [ordered]@{}

for (($i = 0); $i -le 25; $i++)
{
    $name = $i -eq 0 ? "Latest" : "Day$(([string] $i).PadLeft(2, '0'))"
    $cmdargs = $i -eq 0 ? "" : "$($i)"
  
    $profiles[$name] = [ordered]@{
        commandName      = "Project"
        commandLineArgs  = $cmdargs
        workingDirectory = "`$(SolutionDir)"
    }

}

@{profiles = $profiles; } | ConvertTo-Json | Out-File -FilePath (Join-Path $PSScriptRoot "launchSettings.json")