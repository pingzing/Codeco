# Get the API keys from env vars
$uwpApiKey = $env:UWP_API_KEY;
$androidApiKey = $env:ANDROID_API_KEY;

if ([string]::IsNullOrWhiteSpace($uwpApiKey)) {    
    Write-Error "No UWP key found under UWP_API_KEY environment variable.";
    Throw; 
}

if ([string]::IsNullOrWhiteSpace($androidApiKey)) {
    Write-Error "No Android API key found under ANDROID_API_KEY environment variable.";
    Throw;
}

$originalContent = Get-Content .\Codeco.CrossPlatform\AppCenterConfig.cs;
$uwpReplacedContent = $originalContent.Replace("<UwpReplaceMe>", $uwpApiKey);
$bothReplacedContent = $uwpReplacedContent.Replace("<AndroidReplaceMe>", $androidApiKey);

# Rewrite the file with our new file-string
Set-Content .\Codeco.CrossPlatform\AppCenterConfig.cs $bothReplacedContent;