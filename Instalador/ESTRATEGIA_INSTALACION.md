# Instalación: casos especiales y estrategia

El instalador (`GestionGimnasio-Setup.exe`) instala la aplicación y abre el **configurador de base de datos**
(`ConfiguradorBD.exe`), que se ocupa de que exista un SQL Server utilizable. Este documento lista cada caso
que puede aparecer en el equipo del cliente, cómo se detecta y qué hace el sistema.

| # | Caso | Cómo se detecta | Estrategia |
|---|------|-----------------|------------|
| 1 | **No hay ninguna instancia de SQL Server en el equipo** | Se leen `HKLM\SOFTWARE\Microsoft\Microsoft SQL Server\Instance Names\SQL` (vistas de 64 y 32 bits) y `Local DB\Installed Versions`. Si no aparece nada, la lista queda vacía. | Se muestra el panel "No se encontró ninguna instancia" con dos salidas: instalar LocalDB, o escribir a mano el nombre de un servidor remoto (por ejemplo `servidor\SQLEXPRESS` o `192.168.0.10,1433`). Después de instalar, "Buscar instancias" vuelve a detectar. |
| 2 | **No existe el motor de base de datos** | Igual que el caso 1 (no hay instancias ni LocalDB). | **Con `Prerequisitos\SqlLocalDB.msi` incluido:** se instala en modo silencioso (`IACCEPTSQLLOCALDBLICENSETERMS=YES`), se crea la instancia `MSSQLLocalDB` y se usa `(localdb)\MSSQLLocalDB`. **Sin el .msi:** se abre la página oficial de descarga de SQL Server Express/LocalDB y se pide volver a "Buscar instancias". Si la instalación silenciosa falla, se ofrece la descarga de SQL Server Express. El bundle de WiX también instala LocalDB antes de la aplicación cuando no hay ningún motor (`InstallCondition` con `util:RegistrySearch`). |
| 3 | **El servicio de SQL Server existe pero no está levantado** | `InstanciaSql.ServicioDe(servidor)` traduce el nombre de la instancia al servicio de Windows (`MSSQLSERVER`, `MSSQL$NOMBRE`) y `EstadoServicio` consulta si está en ejecución. | Se muestra el estado debajo del selector ("Servicio X: detenido"). Al probar la conexión o instalar, se pregunta "¿Querés iniciarlo ahora?" y se inicia con espera de hasta 60 segundos. Si no se puede (servicio deshabilitado o sin permisos), se indica revisar `services.msc`. El configurador pide privilegios de administrador (`requireAdministrator`). |
| 4 | LocalDB instalado pero sin instancia creada o detenida | Servidor `(localdb)\...` | `SqlLocalDB create` + `start` automáticos antes de conectar. |
| 5 | Servidor remoto sin acceso al puerto | `InstanciaSql.PuertoTcpRemoto` / `PuertoAbierto` (1433 por defecto o el puerto explícito). | Se avisa que el puerto TCP no responde y qué revisar (servidor encendido, TCP/IP habilitado, firewall). No se intenta crear nada. |
| 6 | Versión de SQL Server anterior a 2019 | Versión mayor < 15 al probar la conexión. | Se rechaza con mensaje claro; no se crea la base. |
| 7 | Usuario sin permiso para crear bases | `CREATE ANY DATABASE` / rol sysadmin. | Se avisa que use un usuario administrador del servidor. |
| 8 | Poco espacio en disco | Menos de 300 MB libres en la unidad del sistema (solo servidores locales). | Advertencia con opción de continuar o cancelar. |
| 9 | La base `BDCAPAS` ya existe | Consulta previa. | No se toca su estructura ni sus datos: solo se actualizan las traducciones y se configura la aplicación para usarla. |
| 10 | Falla un script a mitad de la creación | Excepción al ejecutar un lote. | Se elimina la base parcial (rollback) y se deja el detalle en el registro. |
| 11 | Base inaccesible al abrir la aplicación | `ConexionBLL.Disponible()` en el arranque. | La aplicación ofrece abrir el configurador. |
| 12 | Falta .NET Framework 4.8 | Condición de lanzamiento del MSI / `NetFx48Web` del bundle. | El bundle lo descarga e instala; el MSI muestra un mensaje con el enlace si se ejecuta solo. |

Todo queda registrado en `%ProgramData%\GestionGimnasio\install.log`.

## Sistema que no se ve vacío

Al terminar la instalación, el configurador explica que, al ingresar por primera vez con `admin`, la aplicación
ofrece **cargar datos de demostración** (también disponible en *Administración → Cargar datos de demostración*).
Los datos salen del archivo serializado `Scripts\datos_demo.json`, que instala el MSI junto a los scripts SQL:
empleados, franjas, historial de socios, grillas de las últimas semanas con sus fichajes, evaluaciones de
cobertura, valores hora, feriado y equipos con alertas. Solo se pueden cargar si el sistema no tiene datos de negocio.

## Para que el instalador incluya LocalDB

El `.msi` de LocalDB no se versiona (pesa decenas de MB). Para generar un instalador que funcione en un equipo sin
ningún SQL Server:

1. Descargar `SqlLocalDB.msi` desde la página oficial indicada en `Prerequisitos\LEEME.txt`.
2. Copiarlo en `Instalador\Prerequisitos\SqlLocalDB.msi`.
3. Ejecutar `Instalador\generar-instalador.ps1`.
