$ErrorActionPreference = 'Stop'
$root = Split-Path -Parent $PSScriptRoot
$localConfig = Join-Path $root 'backend/.env.local'
foreach ($line in Get-Content -LiteralPath $localConfig) {
    if ($line -match '^([A-Za-z_][A-Za-z0-9_]*)=(.*)$') { [Environment]::SetEnvironmentVariable($Matches[1], $Matches[2], 'Process') }
}
$suffix = [Guid]::NewGuid().ToString('N').Substring(0, 8)
$e2ePassword = 'AutoMarket-E2E-' + [Guid]::NewGuid().ToString('N') + '!'
$env:BootstrapAdmin__Email = "e2e.admin+$suffix@example.invalid"
$env:BootstrapAdmin__Password = $e2ePassword
$jwtBytes = New-Object byte[] 48
$rng = [Security.Cryptography.RandomNumberGenerator]::Create()
$rng.GetBytes($jwtBytes)
$rng.Dispose()
$env:Jwt__Key = [Convert]::ToBase64String($jwtBytes)
$dotnet = Join-Path $root '.tools/dotnet/dotnet.exe'

function Invoke-Api($method, $path, $body = $null, $token = $null, $headers = @{}) {
    $allHeaders = @{ 'Content-Type' = 'application/json' }
    if ($token) { $allHeaders['Authorization'] = "Bearer $token" }
    foreach ($item in $headers.GetEnumerator()) { $allHeaders[$item.Key] = $item.Value }
    $parameters = @{ Method = $method; Uri = "http://localhost:5100/api/v1/$path"; Headers = $allHeaders }
    if ($null -ne $body) { $parameters.Body = ($body | ConvertTo-Json -Depth 8) }
    return Invoke-RestMethod @parameters
}

Push-Location $root
$backendProcess = $null
try {
    & $dotnet build backend/src/Api/Api.csproj --no-restore -m:1 -p:UseSharedCompilation=false --verbosity quiet
    if ($LASTEXITCODE -ne 0) { throw 'Backend build failed.' }
    & $dotnet build backend/tools/Database/Database.csproj --no-restore -m:1 -p:UseSharedCompilation=false --verbosity quiet
    if ($LASTEXITCODE -ne 0) { throw 'Database tool build failed.' }
    & $dotnet run --project backend/tools/Database --no-build -- bootstrap-admin
    if ($LASTEXITCODE -ne 0) { throw 'Administrator bootstrap failed.' }
    $backendProcess = Start-Process -FilePath $dotnet -ArgumentList @('run','--project','backend/src/Api','--no-build','--no-launch-profile') -WorkingDirectory $root -WindowStyle Hidden -PassThru
    $ready = $false
    for ($attempt = 0; $attempt -lt 30; $attempt++) {
        try { Invoke-RestMethod 'http://localhost:5100/health/ready' | Out-Null; $ready = $true; break } catch { Start-Sleep -Milliseconds 500 }
    }
    if (!$ready) { throw 'Backend did not become ready.' }

    $admin = (Invoke-Api 'POST' 'auth/login' @{ email = $env:BootstrapAdmin__Email; password = $e2ePassword }).data
    $city = (Invoke-Api 'POST' 'admin/cities' @{ name = "E2E City $suffix"; state = 'Test State'; latitude = 18.5204; longitude = 73.8567; serviceRadiusKm = 30 } $admin.accessToken).data
    $center = (Invoke-Api 'POST' 'admin/centers' @{ cityId = $city.id; name = "E2E Inspection $suffix"; address = 'Test fixture — not a public location'; kind = 'INSPECTION'; latitude = 18.5204; longitude = 73.8567 } $admin.accessToken).data

    $seller = (Invoke-Api 'POST' 'auth/register' @{ name = 'E2E Seller'; email = "e2e.seller+$suffix@example.invalid"; phone = ''; password = $e2ePassword; referralCode = $null }).data
    $valuation = (Invoke-Api 'POST' 'ai/valuation' @{ brand = 'AutoMarket Test'; model = 'Fixture'; registrationYear = 2023; kilometers = 12000; ownershipCount = 1 } $seller.accessToken).data
    if ($valuation.status -ne 'NOT_CONFIGURED') { throw 'AI valuation must fail closed when no approved service is configured.' }
    $submission = (Invoke-Api 'POST' 'seller/submissions' @{ brand = 'AutoMarket Test'; model = 'Fixture'; variant = 'E2E'; manufacturingYear = 2023; registrationYear = 2023; kilometers = 12000; ownershipCount = 1; fuelType = 'PETROL'; transmission = 'AUTOMATIC'; bodyType = 'SUV'; price = 900000; cityId = $city.id; description = 'Automated development test listing. Not real inventory.' } $seller.accessToken).data
    Invoke-Api 'POST' "seller/submissions/$($submission.id)/submit" @{} $seller.accessToken | Out-Null
    Invoke-Api 'POST' "admin/sellers/$($submission.id)/review" @{ status = 'UNDER_REVIEW'; notes = '' } $admin.accessToken | Out-Null
    Invoke-Api 'POST' "admin/sellers/$($submission.id)/review" @{ status = 'APPROVED'; notes = 'Approved by automated development smoke test.' } $admin.accessToken | Out-Null

    $search = Invoke-Api 'GET' "cars?q=Automrket%20Test&cityId=$($city.id)&pageSize=10"
    if ($search.data.Count -ne 1 -or $search.data[0].id -ne $submission.carId) { throw 'Typo-tolerant, city-scoped search failed.' }

    $buyer = (Invoke-Api 'POST' 'auth/register' @{ name = 'E2E Buyer'; email = "e2e.buyer+$suffix@example.invalid"; phone = ''; password = $e2ePassword; referralCode = $seller.user.referralCode }).data
    $preferences = (Invoke-Api 'GET' 'notifications/preferences' $null $buyer.accessToken).data
    if ($preferences.Count -ne 7) { throw 'Notification preference defaults failed.' }
    $updatedPreference = (Invoke-Api 'PATCH' 'notifications/preferences' @{ category = 'BOOKINGS'; inApp = $false; push = $true } $buyer.accessToken).data
    if ($updatedPreference.inApp -ne $false -or $updatedPreference.push -ne $true) { throw 'Notification preference update failed.' }
    $start = [DateTimeOffset]::UtcNow.AddHours(2)
    $start = [DateTimeOffset]::new($start.Year, $start.Month, $start.Day, $start.Hour, (($start.Minute -lt 30) ? 30 : 0), 0, [TimeSpan]::Zero)
    if ($start -le [DateTimeOffset]::UtcNow.AddHours(1)) { $start = $start.AddMinutes(30) }
    $appointment = (Invoke-Api 'POST' 'appointments' @{ carId = $submission.carId; serviceCenterId = $center.id; startsAt = $start.ToString('o'); appointmentType = 'VIEWING'; notes = 'Automated smoke test.' } $buyer.accessToken).data
    Invoke-Api 'PATCH' "admin/appointments/$($appointment.id)" @{ status = 'CONFIRMED'; notes = '' } $admin.accessToken | Out-Null
    $idempotency = "e2e-$suffix"
    $booking = (Invoke-Api 'POST' 'bookings' @{ carId = $submission.carId; appointmentId = $appointment.id } $buyer.accessToken @{ 'Idempotency-Key' = $idempotency }).data
    $bookingAgain = (Invoke-Api 'POST' 'bookings' @{ carId = $submission.carId; appointmentId = $appointment.id } $buyer.accessToken @{ 'Idempotency-Key' = $idempotency }).data
    if ($booking.id -ne $bookingAgain.id) { throw 'Booking idempotency failed.' }
    if ((Invoke-Api 'GET' 'bookings/me' $null $buyer.accessToken).data.Count -ne 1) { throw 'Buyer booking history failed.' }
    if ((Invoke-Api 'GET' 'notifications' $null $buyer.accessToken).data.Count -lt 2) { throw 'Notification persistence failed.' }
    Invoke-Api 'PATCH' "admin/bookings/$($booking.id)" @{ status = 'CONFIRMED'; notes = '' } $admin.accessToken | Out-Null
    Invoke-Api 'PATCH' "admin/bookings/$($booking.id)" @{ status = 'COMPLETED'; notes = '' } $admin.accessToken | Out-Null
    $sellerWallet = (Invoke-Api 'GET' 'rewards/wallet' $null $seller.accessToken).data
    $buyerWallet = (Invoke-Api 'GET' 'rewards/wallet' $null $buyer.accessToken).data
    if ($sellerWallet.balance -ne 500 -or $buyerWallet.balance -ne 500) { throw 'Referral reward qualification failed.' }
    $redeemed = (Invoke-Api 'POST' 'rewards/redeem' @{ points = 100; idempotencyKey = "redeem-$suffix" } $buyer.accessToken).data
    $redeemedAgain = (Invoke-Api 'POST' 'rewards/redeem' @{ points = 100; idempotencyKey = "redeem-$suffix" } $buyer.accessToken).data
    if ($redeemed.balance -ne 400 -or $redeemedAgain.balance -ne 400) { throw 'Wallet redemption idempotency failed.' }
    Write-Output "E2E PASS — city $($city.id), car $($submission.carId), appointment $($appointment.id), booking $($booking.bookingReference)"
}
finally {
    if ($backendProcess -and !$backendProcess.HasExited) { Stop-Process -Id $backendProcess.Id }
    Remove-Item Env:BootstrapAdmin__Email -ErrorAction SilentlyContinue
    Remove-Item Env:BootstrapAdmin__Password -ErrorAction SilentlyContinue
    Pop-Location
}
