param([string]$Salida)
$ErrorActionPreference = 'Stop'
Add-Type -AssemblyName System.Drawing
$scratch = Split-Path -Parent $MyInvocation.MyCommand.Path
$diag = 'D:\source\repos\ingenieria_software\DIAGRAMAS'

function Cargar($n) { $x = Get-Content (Join-Path $scratch $n) -Raw -Encoding UTF8 | ConvertFrom-Json; foreach ($i in $x) { $i } }
$seg = @(Cargar 'cus_seguridad.json')
$neg = @(Cargar "cus_negocio_a.json") + @(Cargar "cus_negocio_b.json")

$word = New-Object -ComObject Word.Application
$word.Visible = $false
$doc = $word.Documents.Add()
$sel = $word.Selection
$ps = $doc.PageSetup
$ps.PaperSize = 7  # A4
$ps.LeftMargin = 70.9; $ps.RightMargin = 70.9; $ps.TopMargin = 70.9; $ps.BottomMargin = 70.9
$AZUL = 0xB66F3B   # BGR de #3B6FB6
$CLARO = 0xFBF0E8  # BGR de #E8F0FB

$doc.Styles.Item(-1).Font.Name = 'Calibri'; $doc.Styles.Item(-1).Font.Size = 11
$doc.Styles.Item(-1).ParagraphFormat.SpaceAfter = 4
foreach ($i in -2,-3,-4) { $s = $doc.Styles.Item($i); $s.Font.Name = 'Calibri'; $s.Font.Color = $AZUL; $s.Font.Bold = $true }
$doc.Styles.Item(-2).Font.Size = 20; $doc.Styles.Item(-3).Font.Size = 15; $doc.Styles.Item(-4).Font.Size = 12

function Estilo($n) { try { $sel.Style = $doc.Styles.Item([int]$n) } catch { Write-Output "ESTILO FALLA: [$n] $($_.Exception.Message) en $((Get-PSCallStack)[1].Position.Text)"; throw } }
function Linea($texto, $estilo = -1) { Estilo $estilo; $sel.TypeText([string]$texto); $sel.TypeParagraph(); Estilo -1 }
function Negrita($texto) { Estilo -1; $sel.Font.Bold = 1; $sel.TypeText([string]$texto); $sel.Font.Bold = 0; $sel.TypeParagraph() }
function Salto { $sel.InsertBreak(7) }
function Lista($items) { foreach ($t in $items) { Linea $t -49 } }
function Numerada($items) { $n = 1; foreach ($t in $items) { Estilo -1; $sel.ParagraphFormat.LeftIndent = 18; $sel.ParagraphFormat.FirstLineIndent = -18; $sel.TypeText("$n. $t"); $sel.TypeParagraph(); $sel.ParagraphFormat.LeftIndent = 0; $sel.ParagraphFormat.FirstLineIndent = 0; $n++ } }

function Tabla($encabezados, $filas, $anchos) {
    Estilo -1
    $t = $doc.Tables.Add($sel.Range, $filas.Count + 1, $encabezados.Count)
    $t.Borders.Enable = $true
    $t.Range.Font.Size = 10
    $t.Range.ParagraphFormat.SpaceAfter = 0
    for ($c = 0; $c -lt $encabezados.Count; $c++) {
        $cel = $t.Cell(1, $c + 1); $cel.Range.Text = $encabezados[$c]; $cel.Range.Font.Bold = 1; $cel.Shading.BackgroundPatternColor = $CLARO
    }
    for ($r = 0; $r -lt $filas.Count; $r++) {
        for ($c = 0; $c -lt $encabezados.Count; $c++) { $t.Cell($r + 2, $c + 1).Range.Text = [string]$filas[$r][$c] }
    }
    if ($anchos) { for ($c = 0; $c -lt $anchos.Count; $c++) { $t.Columns.Item($c + 1).PreferredWidthType = 3; $t.Columns.Item($c + 1).PreferredWidth = $anchos[$c] } }
    $sel.EndKey(6) | Out-Null
    $sel.TypeParagraph()
}

function Imagen($ruta) {
    if (-not $ruta) { Write-Output "RUTA NULA"; Write-Output (Get-PSCallStack | Out-String); return }
    if (-not (Test-Path $ruta)) { Linea "(falta la imagen $([IO.Path]::GetFileName($ruta)))"; return }
    $img = [System.Drawing.Image]::FromFile($ruta)
    $w = $img.Width; $h = $img.Height; $img.Dispose()
    $maxW = 453.0; $maxH = 640.0
    $esc = [Math]::Min($maxW / $w, $maxH / $h)
    $sel.ParagraphFormat.Alignment = 1
    $pic = $doc.InlineShapes.AddPicture($ruta, $false, $true, $sel.Range)
    $pic.LockAspectRatio = -1
    $pic.Width = [single]($w * $esc); $pic.Height = [single]($h * $esc)
    $sel.EndKey(6) | Out-Null
    $sel.TypeParagraph()
    $sel.ParagraphFormat.Alignment = 0
}

function Opc($v, $d = '-') { if ($null -eq $v -or "$v" -eq '') { $d } else { "$v" } }

function Describir($cu, $rutasDiagramas) {
    Linea "$($cu.id) - $($cu.nombre)" -3
    Tabla @('Campo', 'Detalle') @(
        @('Actor primario', (Opc $cu.actor_primario)),
        @('Actor secundario', (Opc $cu.actor_secundario)),
        @('Frecuencia', (Opc $cu.frecuencia)),
        @('Prioridad', (Opc $cu.prioridad))) @(25, 75)
    Linea 'Propósito' -4; Linea $cu.proposito
    if ($cu.precondiciones) { Linea 'Precondiciones' -4; Lista $cu.precondiciones }
    if ($cu.postcondiciones_exito) {
        Linea 'Postcondiciones' -4
        Negrita 'En caso de éxito'; Lista $cu.postcondiciones_exito
        if ($cu.postcondiciones_fallo) { Negrita 'En caso de fallo'; Lista $cu.postcondiciones_fallo }
    }
    Linea 'Disparador' -4; Linea $cu.disparador
    Linea 'Flujo principal' -4; Numerada $cu.flujo_principal
    if ($cu.flujos_alternativos) {
        Linea 'Flujos alternativos' -4
        foreach ($f in $cu.flujos_alternativos) { Negrita "$($f.id). $($f.nombre)"; Lista $f.pasos }
    }
    if ($cu.excepciones) {
        Linea 'Excepciones' -4
        Tabla @('Código', 'Descripción', 'Manejo') @($cu.excepciones | ForEach-Object { , @($_.codigo, $_.descripcion, $_.manejo) }) @(12, 44, 44)
    }
    if ($cu.reglas_negocio) {
        Linea 'Reglas de negocio' -4
        Tabla @('Código', 'Regla') @($cu.reglas_negocio | ForEach-Object { , @($_.codigo, $_.regla) }) @(12, 88)
    }
    if ($cu.relaciones) {
        Linea 'Relaciones con otros casos de uso' -4
        Tabla @('Tipo', 'Destino', 'Condición') @($cu.relaciones | ForEach-Object { , @($_.tipo, $_.destino, $_.condicion) }) @(14, 36, 50)
    }
    if ($cu.observaciones) { Linea 'Observaciones' -4; Linea $cu.observaciones }
    Linea 'Diagrama de secuencia' -4
    foreach ($r in $rutasDiagramas) { Imagen $r }
}

# ---------- Portada ----------
$sel.ParagraphFormat.Alignment = 1
for ($i = 0; $i -lt 6; $i++) { $sel.TypeParagraph() }
$sel.Font.Size = 30; $sel.Font.Bold = 1; $sel.Font.Color = $AZUL
$sel.TypeText('Sistema de Gestión de Gimnasio'); $sel.TypeParagraph()
$sel.Font.Size = 18; $sel.Font.Bold = 0
$sel.TypeText('Trabajo Práctico - Ingeniería de Software'); $sel.TypeParagraph()
$sel.Font.Size = 14; $sel.Font.Color = 0
$sel.TypeParagraph()
$sel.TypeText('Casos de uso, diagramas de secuencia, instalador y serialización'); $sel.TypeParagraph()
$sel.TypeParagraph()
$sel.TypeText('Documento de entrega'); $sel.TypeParagraph()
$sel.Font.Size = 11; $sel.Font.Color = 0
$sel.ParagraphFormat.Alignment = 0
Salto

# ---------- Índice ----------
Linea 'Índice' -2
$doc.TablesOfContents.Add($sel.Range, $true, 1, 2) | Out-Null
$sel.EndKey(6) | Out-Null
Salto

# ---------- 1. Introducción ----------
Linea '1. Introducción y actores' -2
Linea 'El sistema gestiona un gimnasio: seguridad (usuarios, roles, idiomas, bitácora e integridad) y tres procesos de negocio: planificación de horarios, mantenimiento de equipamiento y horas trabajadas con su liquidación. Se desarrolla en capas (BE, DAL, BLL, SeguridadYServicios y la interfaz WinForms CAPAS) sobre SQL Server y se instala con un instalador propio.'
Tabla @('Actor', 'Descripción') @(
    @('Usuario', 'Persona registrada en el sistema. Inicia y cierra sesión, cambia su contraseña y el idioma de la interfaz.'),
    @('Administrador', 'Especialización de Usuario. Gestiona usuarios, roles, idiomas, la bitácora, la integridad y los procesos de negocio.'),
    @('Empleado', 'Personal del gimnasio (recepción o sala): carga su disponibilidad, ficha, toma o cancela turnos y registra el uso de equipos.'),
    @('Técnico', 'Realiza las revisiones técnicas de los equipos.'),
    @('Sensor IoT', 'Actor externo que informa el uso de los equipos por la API.')) @(22, 78)
Linea 'Convenciones de los diagramas de secuencia' -3
Lista @('El actor solo intercambia mensajes con el límite «:Sistema», que los reenvía al formulario.', 'Toda llamada tiene su retorno (flecha punteada), también en cada rama de las alternativas.', 'Alta, baja y modificación se modelan como casos de uso separados.', 'Los nombres de los procedimientos almacenados coinciden con los del código.')

# ---------- 2. Diagramas de casos de uso ----------
Salto
Linea '2. Diagramas de casos de uso' -2
Linea 'Seguridad (1)' -3; Imagen (Join-Path $diag 'DiagramaCasosDeUso_Seguridad1.png')
Linea 'Seguridad (2)' -3; Imagen (Join-Path $diag 'DiagramaCasosDeUso_Seguridad2.png')
Linea 'Procesos de negocio' -3; Imagen (Join-Path $diag 'DiagramaCasosDeUso_Negocio.png')

# ---------- Índice de CU ----------
Salto
Linea '3. Índice de casos de uso' -2
$todos = @($seg + $neg)
Tabla @('ID', 'Nombre', 'Actor primario') @($todos | ForEach-Object { , @($_.id, $_.nombre, $_.actor_primario) }) @(12, 58, 30)

# ---------- 4. CU de seguridad ----------
Salto
Linea '4. Casos de uso de seguridad (CU-01 a CU-20)' -2
$primero = $true
foreach ($cu in $seg) {
    if (-not $primero) { Salto }; $primero = $false
    $n = $cu.id.Substring(3)
    $png = Get-ChildItem $diag -Filter "DiagramaSecuencia_CU${n}_*.png" | Select-Object -First 1
    Describir $cu @($png.FullName)
}

# ---------- 5. CU de negocio ----------
Salto
Linea '5. Casos de uso de los procesos de negocio (CU-21 a CU-44)' -2
$primero = $true
foreach ($cu in $neg) {
    if (-not $primero) { Salto }; $primero = $false
    $rutas = @($cu.diagramas | ForEach-Object { Join-Path $diag "DiagramaSecuencia_$_.png" })
    Describir $cu $rutas
}

# ---------- 6. Instalador ----------
Salto
Linea '6. Instalador y casos especiales' -2
Linea 'El instalador (GestionGimnasio-Setup.exe, WiX 6) instala .NET Framework 4.8 si falta, LocalDB si no hay ningún motor, la aplicación y el configurador de base de datos (ConfiguradorBD.exe). El configurador se abre con el parámetro /auto, de modo que la base de datos queda configurada con un solo clic: elige la instancia, inicia el servicio si está detenido, instala LocalDB si hace falta, crea la base, guarda la cadena de conexión y deja registro en %ProgramData%\GestionGimnasio\install.log.'
Linea 'Casos especiales' -3
Tabla @('Caso', 'Cómo se identifica', 'Estrategia') @(
    @('No se encuentra ninguna instancia', 'Se leen las claves Instance Names\SQL del registro (vistas de 64 y 32 bits) y las versiones de LocalDB; la lista queda vacía.', 'Se muestra el aviso «No se encontró ninguna instancia» con dos salidas: instalar LocalDB o escribir a mano un servidor remoto (por ejemplo servidor\SQLEXPRESS o 192.168.0.10,1433). Después de instalar se vuelve a buscar.'),
    @('No existe el motor de base de datos', 'No hay instancias ni LocalDB.', 'Con Prerequisitos\SqlLocalDB.msi incluido se instala en modo silencioso, se crea la instancia MSSQLLocalDB y se usa (localdb)\MSSQLLocalDB. Sin el .msi se abre la página oficial de descarga y se pide volver a buscar. Si la instalación silenciosa falla, se ofrece descargar SQL Server Express.'),
    @('El servicio de SQL no está levantado', 'Se traduce la instancia a su servicio de Windows (MSSQLSERVER o MSSQL$NOMBRE) y se consulta su estado.', 'Se muestra el estado, se pregunta si se inicia (en modo /auto se inicia directamente) y se espera hasta 60 segundos. Si no se puede, se indica revisar services.msc.'),
    @('El motor recién iniciado todavía no responde', 'La primera conexión falla justo después de iniciar el servicio o LocalDB.', 'Se reintenta la conexión hasta 10 veces, cada 3 segundos (salvo errores de credenciales). Esto evita tener que hacer clic otra vez.'),
    @('LocalDB sin instancia creada o detenida', 'El servidor es (localdb)\...', 'Se ejecutan SqlLocalDB create y start antes de conectar.'),
    @('Servidor remoto sin acceso al puerto', 'Falla la prueba TCP al puerto (1433 o el indicado).', 'Se avisa qué revisar (servidor encendido, TCP/IP habilitado, firewall). No se crea nada.'),
    @('Versión anterior a SQL Server 2019, sin permiso para crear bases o poco espacio', 'Se verifican al probar la conexión.', 'Se rechaza con un mensaje claro o se advierte, según el caso.'),
    @('La base ya existe', 'Consulta previa.', 'No se toca: solo se actualizan las traducciones y se configura la aplicación.'),
    @('Falla un script a mitad de la creación', 'Excepción al ejecutar un lote.', 'Se elimina la base parcial (rollback) y se deja el detalle en el registro.')) @(22, 31, 47)
Linea 'Al abrir la aplicación se comprueba la conexión; si la base no responde, se ofrece abrir el configurador. El detalle completo está en Instalador\ESTRATEGIA_INSTALACION.md.'

# ---------- 7. Serialización ----------
Linea '7. Serialización' -2
Linea 'El sistema usa archivos serializados en JSON (Newtonsoft.Json) y los consume en el funcionamiento normal:'
Lista @(
    'datos_demo.json (carpeta Scripts, instalado por el MSI): contiene empleados, franjas, historial de socios, equipos y feriados de demostración. DatosDemoBLL lo lee, lo deserializa a BE.DatosDemo, lo valida y con él carga el sistema (CU-40). Así los datos de ejemplo no están escritos en el código.',
    'Exportación del informe de gestión: InformeGestionBLL.GuardarJson serializa el informe (BE.InformeGestion) a un archivo JSON (CU-41), además de la exportación a PDF.')
Linea 'Flujo de CU-40: archivo JSON -> deserializar a objetos -> validar -> crear usuarios, franjas, historial, equipos y feriados -> generar grillas y fichajes simulados -> evaluar la cobertura.'

$doc.TablesOfContents.Item(1).Update()
$doc.Repaginate()
$doc.SaveAs2([string]$Salida, 16)
$paginas = $doc.ComputeStatistics(2)
$doc.Close(0); $word.Quit()
Write-Output "OK $Salida paginas=$paginas casos=$($todos.Count)"
