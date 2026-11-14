<#
.SYNOPSIS
    Downloads hourly OHLCV data from the Binance public API for the 11 instruments
    used by the SmarterSystems.ElliottWaves regression tests.

.DESCRIPTION
    Writes one JSON file per symbol into the testdata directory. The format matches
    what the test suite expects (camelCase property names, ISO-8601 timestampUtc).
    The Binance public klines endpoint requires no API key. The default fetch span is
    the fixed window from StartUtc to EndUtc that the regression snapshots were
    recorded on. Prices are parsed as decimal straight from the Binance strings.

    Override ELLIOTTWAVES_TESTDATA_PATH to write somewhere other than ./testdata.

.PARAMETER Symbols
    Symbols to fetch. Defaults to all 11 instruments used by the regression suite.

.PARAMETER Interval
    Kline interval. Defaults to 1h to match the analyzer input expectation.

.PARAMETER StartUtc
    Inclusive start of the fetch window (ISO-8601, UTC). Symbols listed later start
    at their first Binance candle.

.PARAMETER EndUtc
    Exclusive end of the fetch window (ISO-8601, UTC). The default matches the
    regression snapshots; a later end changes the detected in-progress waves.

.PARAMETER YearsBack
    Optional. When set, fetches a sliding window of this many years back from now
    instead of StartUtc to EndUtc. The regression snapshots do not hold on a
    sliding window.

.PARAMETER OutputDir
    Destination directory. Defaults to <repo-root>/testdata or the
    ELLIOTTWAVES_TESTDATA_PATH environment variable if set.

.EXAMPLE
    pwsh ./scripts/fetch-test-data.ps1

.EXAMPLE
    pwsh ./scripts/fetch-test-data.ps1 -Symbols BTCUSDT,ETHUSDT -YearsBack 3
#>
[CmdletBinding()]
param(
    [string[]]$Symbols = @(
        'BTCUSDT', 'ETHUSDT', 'SOLUSDT', 'AVAXUSDT', 'ADAUSDT',
        'LINKUSDT', 'MANAUSDT', 'HBARUSDT', 'DOTUSDT', 'XRPUSDT', 'SUIUSDT'
    ),
    [string]$Interval = '1h',
    [string]$StartUtc = '2017-01-01T00:00:00Z',
    [string]$EndUtc = '2026-03-20T00:00:00Z',
    [int]$YearsBack = 0,
    [string]$OutputDir = $null
)

$ErrorActionPreference = 'Stop'
$invariant = [Globalization.CultureInfo]::InvariantCulture

if (-not $OutputDir) {
    if ($env:ELLIOTTWAVES_TESTDATA_PATH) {
        $OutputDir = $env:ELLIOTTWAVES_TESTDATA_PATH
    } else {
        $repoRoot = (Resolve-Path (Join-Path $PSScriptRoot '..')).Path
        $OutputDir = Join-Path $repoRoot 'testdata'
    }
}

New-Item -ItemType Directory -Force -Path $OutputDir | Out-Null
Write-Host "Output directory: $OutputDir"

$baseUrl  = 'https://api.binance.com/api/v3/klines'
if ($YearsBack -gt 0) {
    $endTime   = [DateTimeOffset]::UtcNow.ToUnixTimeMilliseconds()
    $startTime = [DateTimeOffset]::UtcNow.AddYears(-$YearsBack).ToUnixTimeMilliseconds()
} else {
    $startTime = [DateTimeOffset]::Parse($StartUtc, $invariant, [Globalization.DateTimeStyles]::AssumeUniversal).ToUnixTimeMilliseconds()
    $endTime   = [DateTimeOffset]::Parse($EndUtc, $invariant, [Globalization.DateTimeStyles]::AssumeUniversal).ToUnixTimeMilliseconds()
}
$window = '{0:yyyy-MM-ddTHH:mm}Z to {1:yyyy-MM-ddTHH:mm}Z' -f [DateTimeOffset]::FromUnixTimeMilliseconds($startTime).UtcDateTime, [DateTimeOffset]::FromUnixTimeMilliseconds($endTime).UtcDateTime

foreach ($symbol in $Symbols) {
    $outFile = Join-Path $OutputDir "chartdata_$symbol.json"
    Write-Host "Fetching $symbol ($Interval, $window) ..." -ForegroundColor Cyan

    $candles = New-Object System.Collections.Generic.List[object]
    $cursor = $startTime
    $batchCount = 0

    while ($cursor -lt $endTime) {
        $url = "$baseUrl`?symbol=$symbol&interval=$Interval&startTime=$cursor&endTime=$($endTime - 1)&limit=1000"
        try {
            $response = Invoke-RestMethod -Uri $url -Method Get -TimeoutSec 30
        } catch {
            Write-Warning "  request failed at cursor $cursor : $($_.Exception.Message). Retrying in 5s..."
            Start-Sleep -Seconds 5
            continue
        }

        if (-not $response -or $response.Count -eq 0) { break }

        foreach ($k in $response) {
            $candles.Add([pscustomobject]@{
                timestampUtc = [DateTimeOffset]::FromUnixTimeMilliseconds([long]$k[0]).UtcDateTime.ToString('yyyy-MM-ddTHH:mm:ssZ')
                open   = [decimal]::Parse($k[1], $invariant)
                high   = [decimal]::Parse($k[2], $invariant)
                low    = [decimal]::Parse($k[3], $invariant)
                close  = [decimal]::Parse($k[4], $invariant)
                volume = [decimal]::Parse($k[5], $invariant)
            })
        }

        $batchCount++
        $cursor = [long]$response[-1][6] + 1

        # Rate limit: Binance allows 1200 weight per minute, each klines call is weight 1.
        # 200ms between calls keeps us well under the limit even with parallel symbols.
        Start-Sleep -Milliseconds 200
    }

    if ($candles.Count -eq 0) {
        Write-Warning "  no data returned for $symbol"
        continue
    }

    $candles | ConvertTo-Json -Depth 3 -Compress | Set-Content -Path $outFile -Encoding UTF8 -NoNewline
    Write-Host ("  wrote {0} candles ({1} batches) to {2}" -f $candles.Count, $batchCount, $outFile) -ForegroundColor Green
}

Write-Host "`nDone." -ForegroundColor Green
