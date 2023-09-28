# depends on the Xerris.OidcCli global tool being installed:
#   dotnet tool install -g Xerris.OidcCli
$clientId = "e4JmYLH5gWFqPnU9xaqUTqhdaIsbGZCB"
$authority = "https://spenses.us.auth0.com"
$port = 8082
$audience = "https://spenses-platform"

Write-Host("Requesting access token from $authority...")

$response = dotnet oidccli --authority $authority --clientId $clientId --port $port --audience $audience --scope "openid email profile" | ConvertFrom-Json

Set-Clipboard $response.accessToken

Write-Host("Access token copied to clipboard.")
