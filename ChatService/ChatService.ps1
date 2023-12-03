# Test API with an X number of simultaneous chats. Pings every one second keep-alive each chat.

$chatService_hostAddress = 'http://localhost:5070'
$noOfSessions = 29

$sessionIds = New-Object -TypeName 'string[]' -ArgumentList $noOfSessions
for ($i = 0; $i -lt $noOfSessions; $i++) {
    $guid = Invoke-WebRequest -Uri "$chatService_hostAddress/CreateSession" -Method Post
    $sessionIds[$i] = $guid.Content -replace '"', ''
}

while (1 -eq 1) {
    foreach ($guid in $sessionIds) {
        $res = Invoke-WebRequest -Uri "$chatService_hostAddress/Ping" -Body @{ guid = $guid }
        if ($res.StatusCode -ne 200) {
            Write-Host $res.Content
        }
    }

    Start-Sleep -Seconds 1
}