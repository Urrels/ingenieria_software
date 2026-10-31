# Instalación: casos especiales y estrategia

El instalador (`GestionGimnasio-Setup.exe`) instala la aplicación y abre el **configurador de base de datos**
(`ConfiguradorBD.exe`), que se ocupa de que exista un SQL Server utilizable. Este documento lista cada caso
que puede aparecer en el equipo del cliente, cómo se detecta y qué hace el sistema. El instalador es **guiado y poco invasivo**: no instala motores de base de datos; da instrucciones y links, y pide confirmación antes de iniciar un servicio.

| # | Caso | Cómo se detecta | Estrategia |
|---|------|-----------------|------------|
| 1 | **No se encuentra la instancia de SQL Server** | Se leen `HKLM\SOFTWARE\Microsoft\Microsoft SQL Server\Instance Names\SQL` (vistas de 64 y 32 bits) y `Local DB\Installed Versions`. La instancia escrita o elegida se traduce a su servicio de Windows (`InstanciaSql.ServicioDe`) y, si ese servicio no existe, o la conexión indica que el servidor no responde, la instancia no está. | **No se crea ni se instala nada.** Se muestra "No se encontró la instancia" con las instancias detectadas y los pasos para averiguar el nombre exacto (`services.msc` → *SQL Server (NOMBRE)*; `MSSQLSERVER` es la instancia predeterminada), se sugiere elegir una de la lista o tocar *Buscar instancias*, y, si el servidor está en otra computadora, escribir su nombre o IP. |
| 2 | **No existe el motor de base de datos** | No hay instancias ni LocalDB (la lista de instancias queda vacía). | **El instalador no instala SQL Server ni LocalDB.** Se muestra un panel y un cuadro con las instrucciones: descargar SQL Server Express (gratuito, instalación *Básica*) desde la página de Microsoft, terminar el asistente, volver y tocar *Buscar instancias*; o escribir un servidor de la red. El botón *Abrir la descarga* abre la página oficial en el navegador (solo si el usuario lo acepta). Se exige SQL Server 2019 o posterior. |
| 3 | **El servicio de SQL Server existe pero no está levantado** | `InstanciaSql.ServicioDe(servidor)` traduce el nombre de la instancia al servicio de Windows (`MSSQLSERVER`, `MSSQL$NOMBRE`) y `EstadoServicio` consulta si está en ejecución. | Se muestra el estado debajo del selector. Antes de tocar nada se **pregunta siempre** (también en modo automático) si se quiere iniciar el servicio, y se explica cómo hacerlo a mano (`services.msc` → clic derecho → *Iniciar*). Si se acepta, se inicia con espera de hasta 60 segundos; si no se puede (servicio deshabilitado o sin permisos), se indica revisar `services.msc`. El configurador pide privilegios de administrador (`requireAdministrator`). |
| 4 | LocalDB ya instalado pero sin instancia creada o detenida | Servidor `(localdb)\...` | `SqlLocalDB create` + `start` de la instancia `MSSQLLocalDB` antes de conectar (es una instancia del propio usuario; no instala nada). |
| 5 | Servidor remoto sin acceso al puerto | `InstanciaSql.PuertoTcpRemoto` / `PuertoAbierto` (1433 por defecto o el puerto explícito). | Se avisa que el puerto TCP no responde y qué revisar (servidor encendido, TCP/IP habilitado, firewall). No se intenta crear nada. |
| 6 | Versión de SQL Server anterior a 2019 | Versión mayor < 15 al probar la conexión. | Se rechaza con mensaje claro; no se crea la base. |
| 7 | Usuario sin permiso para crear bases | `CREATE ANY DATABASE` / rol sysadmin. | Se avisa que use un usuario administrador del servidor. |
| 8 | Poco espacio en disco | Menos de 300 MB libres en la unidad del sistema (solo servidores locales). | Advertencia con opción de continuar o cancelar. |
| 9 | La base `BDCAPAS` ya existe | Consulta previa. | No se toca su estructura ni sus datos: solo se actualizan las traducciones y se configura la aplicación para usarla. |
| 10 | Falla un script a mitad de la creación | Excepción al ejecutar un lote. | Se elimina la base parcial (rollback) y se deja el detalle en el registro. |
| 11 | Base inaccesible al abrir la aplicación | `ConexionBLL.Disponible()` en el arranque. | La aplicación ofrece abrir el configurador. |
| 12 | Falta .NET Framework 4.8 | Condición de lanzamiento del MSI / `NetFx48Web` del bundle. | El bundle lo descarga e instala (es un requisito de la aplicación); el MSI muestra un mensaje con el enlace si se ejecuta solo. |

Todo queda registrado en `%ProgramData%\GestionGimnasio\install.log`.

## Modo automático (un solo clic)

El instalador abre el configurador con `ConfiguradorBD.exe /auto` (se abre solo al terminar la instalación; también está en el menú Inicio como "Configurar base de datos"). Si hay una instancia local de SQL Server, el configurador la elige, crea la base y guarda la cadena de conexión sin más clics. No instala ningún motor de base de datos: si no encuentra ninguno, muestra las instrucciones del caso 2 y queda abierto. Si el servicio está detenido, pregunta antes de iniciarlo (caso 3). También acepta `/servidor=<nombre>` para forzar una instancia.

**Reintentos tras arrancar el motor:** el servicio o LocalDB recién iniciado tarda unos segundos en aceptar conexiones, y esa era la causa de que la base no se configurara al primer intento. Si el configurador acaba de iniciar el motor, `InstaladorBaseDatos` reintenta la conexión hasta 10 veces (cada 3 s). No se reintentan los errores de credenciales (18456/18452/18451).

## Sistema que no se ve vacío

Al terminar la instalación, el configurador explica que, al ingresar por primera vez con `admin`, la aplicación
ofrece **cargar datos de demostración** (también disponible en *Administración → Cargar datos de demostración*).
Los datos salen del archivo serializado `Scripts\datos_demo.json`, que instala el MSI junto a los scripts SQL:
empleados, franjas, historial de socios, grillas de las últimas semanas con sus fichajes, evaluaciones de
cobertura, valores hora, feriado y equipos con alertas. Solo se pueden cargar si el sistema no tiene datos de negocio.
