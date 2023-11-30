$template = Get-Content -Path (Join-Path $PSScriptRoot "Day.template")

for (($i = 1); $i -le 25; $i++) {
    $leftPadded = ([string] $i).PadLeft(2, '0')
    $dayFullName = "Day$($leftPadded)"
    $dir = Join-Path $PSScriptRoot $dayFullName

    if (-not (Test-Path $dir)) {
        New-Item -ItemType Directory -Path $dir
    }
    
    $template.Replace("#{FULLDAYNAME}#", $dayFullName).Replace("#{DAYNUMBER}#", $i) | Out-File -FilePath (Join-Path $dir "$($dayFullName).cs")
}