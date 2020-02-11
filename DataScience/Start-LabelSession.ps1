param(
    [ScriptBlock] $ScriptBlock
)

$reviewed = [Ordered] @{}
if(Test-Path labeledData.csv)
{
    Import-Csv labeledData.csv -Header Path,Label | % { $reviewed[$_.Path] = $_.Label }
}

Write-Progress "Preparing for dataset review of $pwd"
$allFiles = dir *.ps1,*.psm1 -rec | ? { -not $reviewed.Contains( (Resolve-Path $_.FullName -Relative).Substring(2) ) }

$processed = $reviewed.Count
$total = $allFiles.Count + $reviewed.Count

$allFiles | % {
    Write-Progress "Processing $processed of $total" -PercentComplete ($processed / $total * 100)

    if($ScriptBlock)
    {
        $label = & $ScriptBlock $_
    }
    else
    {
        notepad $_.FullName | Out-Null
        $label = Read-Host "$(Resolve-Path -Relative $_.FullName)`r`nEnter Label. Any input (followed by ENTER) = Obfuscated. ENTER alone = Clean"
    }
    
    if($label -ne '?')
    {
        if($label)
        {
            $label = "1"
        }
        else
        {
            $label = "0"
        }

        $newPath = (Resolve-Path $_.FullName -Relative).Substring(2)
        ([PSCustomObject] @{ Path = $newPath; Label = $label }) | Export-Csv labeledData.csv -Append -NoTypeInformation
    }

    $processed++
}