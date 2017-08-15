<#
Get-SplunkPoSHScriptBlocks
Written by Troy Arwine & Kurt Falde
Utilizes the Splunk REST API to query PowerShell Script Block logs and extract the data to a local JSON file
This in turn can be utilized for further analysis

Instructions:
Modify the 'earliest=-15m@m latest=now' section of the $search variable to change the timespan of events to query
May need to modify the Splunk Query if your events are formatted different in your Splunk config
Modify the url to your Splunk instance
The REST API must be enabled on Splunk in order for this to function.  If you are seeing connection reset errors may be due to REST API
not being enabled. 
Try testing with a shorter / simpler query if not getting results
#>

$search = 'search sourcetype=XmlWinEventLog:Microsoft-Windows-Powershell/Operational EventCode=4104 earliest=-15m@m latest=now | sort 0 - _time | spath | spath Event.EventData.Data{@Name} | eval MessageNumber=mvindex(''Event.EventData.Data'',0) | eval MessageTotal=mvindex(''Event.EventData.Data'',1) | eval ScriptBlockText=mvindex(''Event.EventData.Data'',2) | eval ScriptBlockId=mvindex(''Event.EventData.Data'',3) | eval Path=mvindex(''Event.EventData.Data'',4) | table EventCode TimeCreated Level MessageNumber MessageTotal ScriptBlockText ScriptBlockId Path'
$url = 'https://yoursplunkurl/services/search/jobs/export'
$credential = get-credential
$outfile = 'C:\temp\SplunkPoshScriptblocks-'+$(get-date -f yyyy-MM-dd-hh-mm)+'.json'

# If you have a Splunk Server that is using a self-signed / untrusted cert you need this portion to ignore the cert error
add-type @"
    using System.Net;
    using System.Security.Cryptography.X509Certificates;
    public class TrustAllCertsPolicy : ICertificatePolicy {
        public bool CheckValidationResult(
            ServicePoint srvPoint, X509Certificate certificate,
            WebRequest request, int certificateProblem) {
            return true;
        }
    }
"@
[System.Net.ServicePointManager]::CertificatePolicy = New-Object TrustAllCertsPolicy
$Body = @{
    search = $search
    output_mode = 'json'
    exec_mode = 'oneshot'
    }
Invoke-RestMethod -Uri $url -Credential $credential -Method Post -OutFile $outfile -Body $Body
