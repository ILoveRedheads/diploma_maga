$path = "~\Documents\crime-scene\publish\"
$app = "CSService.API.dll"
$logPath = "~\Documents\crime-scene\log.txt"

$env:URLS='http://+:5197'
Start-Process -FilePath "dotnet" -ArgumentList $app -WorkingDirectory $path -RedirectStandardOutput $logPath -WindowStyle Minimized
