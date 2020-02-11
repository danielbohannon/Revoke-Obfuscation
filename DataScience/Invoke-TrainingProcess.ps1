<#
Accuracy / False Positive rate on training data = 0.9854 / 0.0035
Accuracy / False Positive rate on test data = 0.9115 / 0.0549
#>

## Process all the files in a directory, opening them up in notepad. When notepad closes:
## Push ENTER if it is clean
## Type any letter and then ENTER if it is obfuscated
## They will then be moved to "Obfuscated" or "Clean" subdirectories.
if(-not (Test-Path Obfuscated)) { mkdir Obfuscated }
if(-not (Test-Path Clean)) { mkdir Clean }
dir -af | % { notepad $_.Fullname | Out-Null; $d = Read-Host; if($d) { $_ | Move-Item -Destination Obfuscated } else { $_ | Move-Item -Destination Clean } }


## Process known obfuscated
dir .\InvokeCradleCrafter\ -af -rec | % { [PSCustomObject] @{ Path = (Resolve-Path -Relative $_.FullName).Substring(2); Label = "1" } } | Export-Csv InvokeCradleCrafter-obfuscation-labeledData.csv -NoTypeInformation

## Helpful intern version:
## Have Revoke-Obfuscation emit results for all items in the Clean or Obfuscated subdirectories. Requires an update to
## Revoke-Obfuscation ("-PassThru") to emit the objects directly rather than have them write to a CSV.
## Dump into one massive "AnalyzedCorpus.csv"
## 30 minutes for 11264 scripts
$labeledData = @{}
$bn = "c:\users\lee\corpus"
dir PowerShellCorpus\*.csv | % { Import-Csv $_.FullName | % { $labeledData[ (Join-Path $bn $_.path) ] = $_.Label } }
$analyzedCorpus = $labeledData.Keys | % {
    $scriptResult = ([PSCustomObject] (Get-RvoFeatureVector -Path $_)) | ConvertTo-CSV -NoTypeInformation | Select -Last 1
    $scriptResult += "," + $labeledData[$_]
    $scriptResult
}
$analyzedCorpus | Set-Content AnalyzedCorpus.csv

## Run the model traininer on that CSV. It will output a weight vector as one of its lines.
ModelTrainer\ModelTrainer.exe C:\Users\lee\OneDrive\Documents\Revoke-Obfuscation\AnalyzedCorpus.csv 0.01 | Tee-Object -Variable Output
$weights = ($output[-12] -split ' ' | ? { $_ } ) -join ', '


## "In the wild"" version:
$labeledData = @{}
$bn = "c:\users\lee\corpus"
$inTheWild = "PowerShellCorpus\InvokeCradleCrafter-obfuscation-labeledData.csv", "PowerShellCorpus\InvokeObfuscation-obfuscation-labeledData.csv", "PowerShellCorpus\IseSteroids-obfuscation-labeledData.csv", "PowerShellCorpus\UnderhandedPowerShell-obfuscation-labeledData.csv"
dir PowerShellCorpus\*.csv | % { Import-Csv $_.FullName | % { $labeledData[ (Join-Path $bn $_.path) ] = 0 } }
dir $inTheWild | % { Import-Csv $_.FullName | % { $labeledData[ (Join-Path $bn $_.path) ] = $_.Label } }

$analyzedCorpus = $labeledData.Keys | % {
    $scriptResult = ([PSCustomObject] (Get-RvoFeatureVector -Path $_)) | ConvertTo-CSV -NoTypeInformation | Select -Last 1
    $scriptResult += "," + $labeledData[$_]
    $scriptResult
}
$analyzedCorpus | Set-Content AnalyzedCorpusInTheWild.csv

## Run the model traininer on that CSV. It will output a weight vector as one of its lines.
ModelTrainer\ModelTrainer.exe C:\Users\lee\OneDrive\Documents\Revoke-Obfuscation\AnalyzedCorpusInTheWild.csv | Tee-Object -Variable Output
$weights = ($output[-12] -split ' ' | ? { $_ } ) -join ', '




## Seeing Measure-CharacterFrequency
<#

    Accuracy: 0.707203179334327
    Precision: 0.898895027624309
    Recall: 0.370530630835801
    F1Score: 0.524754071923883

    TruePositiveRate: 0.161649279682067
    FalsePositiveRate: 0.0181818181818182
    TrueNegativeRate: 0.54555389965226
    FalseNegativeRate: 0.274615002483855

#>
$labeledData = @{}
dir corpus\*.csv | % { $bn = $_.Directory; Import-Csv $_.FullName | % { if(($_.Path -notmatch '^(Invoke|Underhanded|IseSteroids)') -and ($_.Label -eq '1')) { Write-Host "Skipping $_" } else { $_ } } | % { $labeledData[ (Join-Path $bn $_.path) ] = $_.Label } }
$globalFrequency = Measure-CharacterFrequency -LiteralPath ($labeledData.GetEnumerator() | ? Value -eq "0" | % Name)

$truePositives = 0
$falsePositives = 0
$trueNegatives = 0
$falseNegatives = 0
$labeledData.GetEnumerator() | % {
    $scriptFrequency = Measure-CharacterFrequency.ps1 -LiteralPath $_.Name
    $sim = Measure-VectorSimilarity $globalFrequency $scriptFrequency -KeyProperty Name -ValueProperty Percent

    ## Evaluated as obfuscated
    if($sim -lt 0.8)
    {
        ## Is actually obfuscated
        if($_.Value -eq "1")
        {
            $truePositives++
        }
        else
        {
            $falsePositives++
        }
    }
    else
    {
        ## Evaluated as clean

        ## Is actually obfuscated
        if($_.Value -eq "1")
        {
            $falseNegatives++
        }
        else
        {
            $trueNegatives++
        }
    }
}

$totalCount = $truePositives + $falsePositives + $falseNegatives + $trueNegatives
$accuracy = ($truePositives * 1.0 + $trueNegatives) / $totalCount
$precision = ($truePositives * 1.0) / ($truePositives + $falsePositives)
$recall = ($truePositives * 1.0) / ($truePositives + $falseNegatives)
$f1Score = (2.0 * $precision * $recall) / ($precision + $recall)

"Accuracy: " + $accuracy
"Precision: " + $precision
"Recall: " + $recall
"F1Score: " + $f1Score

"TruePositiveRate: " + ($truePositives / $totalCount)
"FalsePositiveRate: " + ($falsePositives / $totalCount)
"TrueNegativeRate: " + ($trueNegatives / $totalCount)
"FalseNegativeRate: " + ($falseNegatives / $totalCount)



## Command lines
$all = $(ipcsv .\Lee_Clean.csv; ipcsv .\Lee_Obf.csv)
$analyzedCommandLines = $all | % {
    $scriptResult = ([PSCustomObject] (Get-RvoFeatureVector -ScriptExpression $_.ArgsCleaned)) | ConvertTo-CSV -NoTypeInformation | Select -Last 1
    if($_.DBO_Obfuscated -match 'clean')
    {
        $scriptResult += ",0"
    }
    else
    {
        $scriptResult += ",1"
    }

    $scriptResult
}
$analyzedCommandLines | Set-Content AnalyzedCommandLines.csv

$output = C:\users\lee\OneDrive\Documents\Revoke-Obfuscation\ModelTrainer\ModelTrainer.exe RandomAnalyzedCommandLines.csv 0.3 100