$ErrorActionPreference = 'Stop'
$raiz = Split-Path -Parent $PSScriptRoot

$vswhere = Join-Path ${env:ProgramFiles(x86)} 'Microsoft Visual Studio\Installer\vswhere.exe'
$msbuild = & $vswhere -latest -requires Microsoft.Component.MSBuild -find 'MSBuild\**\Bin\MSBuild.exe' | Select-Object -First 1
if (-not $msbuild) { throw 'No se encontró MSBuild. Instalá Visual Studio 2022 con la carga de trabajo de escritorio .NET.' }

& $msbuild (Join-Path $raiz 'TP_IS.sln') /restore /p:Configuration=Release /v:minimal
if ($LASTEXITCODE -ne 0) { throw 'Falló la compilación de la solución.' }

& $msbuild (Join-Path $PSScriptRoot 'Instalador.wixproj') /restore /p:Configuration=Release /v:minimal
if ($LASTEXITCODE -ne 0) { throw 'Falló la generación del instalador.' }

$msi = Get-ChildItem (Join-Path $PSScriptRoot 'bin\Release') -Recurse -Filter *.msi | Select-Object -First 1
Write-Host "Instalador generado: $($msi.FullName)" -ForegroundColor Green
