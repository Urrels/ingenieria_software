$ErrorActionPreference = 'Stop'
$raiz = Split-Path -Parent $PSScriptRoot

$vswhere = Join-Path ${env:ProgramFiles(x86)} 'Microsoft Visual Studio\Installer\vswhere.exe'
$msbuild = & $vswhere -latest -requires Microsoft.Component.MSBuild -find 'MSBuild\**\Bin\MSBuild.exe' | Select-Object -First 1
if (-not $msbuild) { throw 'No se encontró MSBuild. Instalá Visual Studio 2022 con la carga de trabajo de escritorio .NET.' }

& $msbuild (Join-Path $raiz 'TP_IS.sln') /restore /p:Configuration=Release /v:minimal
if ($LASTEXITCODE -ne 0) { throw 'Falló la compilación de la solución.' }

& $msbuild (Join-Path $PSScriptRoot 'Bundle\Bundle.wixproj') /restore /p:Configuration=Release /v:minimal
if ($LASTEXITCODE -ne 0) { throw 'Falló la generación del instalador.' }

$msi = Get-ChildItem (Join-Path $PSScriptRoot 'bin\Release') -Recurse -Filter *.msi | Select-Object -First 1
$exe = Get-ChildItem (Join-Path $PSScriptRoot 'Bundle\bin\Release') -Recurse -Filter *.exe | Select-Object -First 1
Write-Host "Instalador .exe: $($exe.FullName)" -ForegroundColor Green
Write-Host "Instalador .msi: $($msi.FullName)" -ForegroundColor Green
