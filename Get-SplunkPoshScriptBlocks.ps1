<#
Get-SplunkPoSHScriptBlocks
Written by Troy Arwine & Kurt Falde
Utilizes the Splunk REST API to query PowerShell Script Block logs and extract the data from them to a local JSON file
This in turn can be utilized for further forensic analysis

Help:
    Date/Time
     Modify the 'earliest=-15m@m latest=now' section of the $search variable to change the timespan of events to query
     Can also use something like 'earliest=10/19/2009:0:0:0 latest=10/27/2009:0:0:0' to control a time/date range for the query

     Index
     IF your PowerShell Logs are in their own index then specify an index name first to speed up the query

     ScriptBlockTextLenght is what it sounds like. If you are having too many script blocks that are extremely short in nature you can try trimming based on length to increase performance
     Or you could possibly whitelist them as well in Revoke-Obfuscation
     ScriptBlockTextLength>100

     Change the $url value to your local splunk instance

#>

$search = 'search index=PowerShellLogs sourcetype=XmlWinEventLog:Microsoft-Windows-Powershell/Operational EventCode=4104 earliest=-1m@m latest=now | spath output=Data path=Event.EventData.Data | fields EventCode TimeCreated Level Data | eval MessageNumber=mvindex(''Data'',0) | eval MessageTotal=mvindex(''Data'',1) | eval ScriptBlockText=mvindex(''Data'',2) | eval ScriptBlockId=mvindex(''Data'',3) | eval Path=mvindex(''Data'',3) | eval ScriptBlockTextLength=len(ScriptBlockText) | where ScriptBlockTextLength>100 | table EventCode TimeCreated Level MessageNumber MessageTotal ScriptBlockText ScriptBlockId Path | sort 0 - _time'
$url = 'https://changetosplunkurl/services/search/jobs/export'
$credential = get-credential
$outfile = 'C:\temp\SplunkPoshScriptblocks-'+$(get-date -f yyyy-MM-dd-hh-mm)+'.json'

# This will allow for self-signed SSL certs to work
[System.Net.ServicePointManager]::ServerCertificateValidationCallback = {$true}
[Net.ServicePointManager]::SecurityProtocol = [Net.SecurityProtocolType]::Tls 


$Body = @{
    search = $search
    output_mode = 'json'
    exec_mode = 'oneshot'
    }
Invoke-RestMethod -Uri $url -Credential $credential -Method Post -OutFile $outfile -Body $Body
