$ErrorActionPreference = 'Stop'

$serverRoot = 'C:\Program Files\MySQL\MySQL Server 8.4'
$dataDirectory = Join-Path $env:LOCALAPPDATA 'CarRental\MySqlData-20261119'
$logDirectory = Join-Path $env:LOCALAPPDATA 'CarRental\logs'
$logFile = Join-Path $logDirectory 'mysql-server.err'
$serverExecutable = Join-Path $serverRoot 'bin\mysqld.exe'

if (-not (Test-Path $serverExecutable)) {
    throw "MySQL Server 8.4 was not found at $serverExecutable."
}
if (-not (Test-Path (Join-Path $dataDirectory 'mysql'))) {
    throw "The local MySQL data directory is not initialized: $dataDirectory"
}

$listener = Get-NetTCPConnection -LocalPort 3306 -State Listen -ErrorAction SilentlyContinue |
    Where-Object { $_.LocalAddress -eq '127.0.0.1' }
if ($listener) {
    Write-Output 'MySQL is already listening on 127.0.0.1:3306.'
    exit 0
}

New-Item -ItemType Directory -Force $logDirectory | Out-Null
$arguments = @(
    '--console',
    "--basedir=`"$serverRoot`"",
    "--datadir=`"$dataDirectory`"",
    '--bind-address=127.0.0.1',
    '--port=3306',
    '--mysqlx=0',
    "--log-error=`"$logFile`""
)
Start-Process -FilePath $serverExecutable -ArgumentList $arguments -WindowStyle Hidden | Out-Null
Start-Sleep -Seconds 2

$listener = Get-NetTCPConnection -LocalPort 3306 -State Listen -ErrorAction SilentlyContinue |
    Where-Object { $_.LocalAddress -eq '127.0.0.1' }
if (-not $listener) {
    if (Test-Path $logFile) { Get-Content $logFile -Tail 20 }
    throw 'MySQL did not start. Review the MySQL log above.'
}

Write-Output 'MySQL is listening on 127.0.0.1:3306.'
