param($OriginalInput)

$startInfo = New-Object System.Diagnostics.ProcessStartInfo -Property @{ RedirectStandardOutput = $true; FileName = "$PSScriptRoot\ConvertToPowerShellExpression.exe"; Arguments = $OriginalInput; UseShellExecute = $false }
$process = [System.Diagnostics.Process]::Start($startInfo)
$process.StandardOutput.ReadToEnd().Trim()