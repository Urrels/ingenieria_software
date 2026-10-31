# Auditoría de consistencia: documentación vs. código

Fuente de la documentación: los `.puml` de `DIAGRAMAS/` que generan las imágenes del Word (diagramas de clases de G06 y diagramas de secuencia de los 44 casos de uso). Fuente del código: `BE`, `BLL`, `DAL`, `SeguridadYServicios`, `CAPAS`, `ConfiguradorBD`, `EmuladorFichaje` y `DAL/script.sql`.
Criterio: un método "existe" si hay una declaración con ese nombre exacto en la clase indicada. No se deduce ni se sugiere nada que no esté en los archivos.

## A. Diagramas de clases (G06)

Métodos documentados en los diagramas de clases: **364**. Existen con el mismo nombre en la clase: **364**.

### A.1 Clases dibujadas que no existen en el código
- Ninguna.

### A.2 Métodos que aparecen en el diagrama de clases pero no están en la clase del código
- Ninguno.

### A.3 Métodos públicos del código que no figuran en el diagrama (solo de las clases que sí se dibujaron)
- Ninguno.

## B. Diagramas de secuencia (CU-01 a CU-44)

Llamadas a métodos de clases del código en los diagramas de secuencia: **188**. Coinciden con una declaración de la clase destino: **188** (incluye 7 usos de `ShowDialog/Show/Close` heredados de `Form`, que no se declaran en el proyecto).
Procedimientos almacenados referenciados en mensajes a `:DB`: **155**. Existen en `script.sql`: **155**.

- Sin inconsistencias.
## C. Métodos públicos de BLL y DAL que no aparecen en ningún diagrama de secuencia ni de clases


---

# Anexo: diferencias que había antes de corregir los diagramas

Estas diferencias se corrigieron en los `.puml` (y en el Word) para que coincidan con el código; el resultado de arriba es la verificación posterior.

## Resultado inicial

Fuente de la documentación: los `.puml` de `DIAGRAMAS/` que generan las imágenes del Word (diagramas de clases de G06 y diagramas de secuencia de los 44 casos de uso). Fuente del código: `BE`, `BLL`, `DAL`, `SeguridadYServicios`, `CAPAS`, `ConfiguradorBD`, `EmuladorFichaje` y `DAL/script.sql`.
Criterio: un método "existe" si hay una declaración con ese nombre exacto en la clase indicada. No se deduce ni se sugiere nada que no esté en los archivos.

### A. Diagramas de clases (G06)

Métodos documentados en los diagramas de clases: **296**. Existen con el mismo nombre en la clase: **291**.

#### A.1 Clases dibujadas que no existen en el código
- Ninguna.

#### A.2 Métodos que aparecen en el diagrama de clases pero no están en la clase del código
- `AsistenciaPersonalBLL.TieneIngresoAbierto()` — no existe en ninguna clase del proyecto
- `BitacoraBLL.Listar()` — existe con ese nombre en: `AsistenciaPersonalBLL`
- `BitacoraDAL.Listar()` — existe con ese nombre en: `AsistenciaPersonalBLL`
- `UsuarioBLL.ListarTodos()` — existe con ese nombre en: `EquipoBLL`, `EquipoDAL`, `IdiomaBLL`, `IdiomaDAL`
- `UsuarioDAL.ListarTodos()` — existe con ese nombre en: `EquipoBLL`, `EquipoDAL`, `IdiomaBLL`, `IdiomaDAL`

#### A.3 Métodos públicos del código que no figuran en el diagrama (solo de las clases que sí se dibujaron)
- `AlertaRevisionBLL`: `ObtenerEquipoDeAlerta()`, `ObtenerPorId()`
- `AlertaRevisionDAL`: `Obtener()`
- `AsistenciaPersonalDAL`: `Corregir()`, `InsertarManual()`, `ListarRango()`, `ObtenerAbierta()`, `ObtenerPorId()`, `RegistrarEgreso()`, `RegistrarIngreso()`
- `ConexionBLL`: `Disponible()`, `Identificador()`
- `ConexionDAL`: `Identificador()`, `Probar()`
- `DatosDemoBLL`: `BuscarArchivo()`
- `DemandaEstimadaDAL`: `ObtenerResumenCobertura()`
- `EquipoBLL`: `ObtenerPorId()`
- `EquipoDAL`: `ListarTodos()`, `ObtenerPorId()`, `ObtenerPorNombre()`
- `EvaluacionCoberturaBLL`: `Clasificar()`
- `FranjaHorariaDAL`: `Eliminar()`, `Insertar()`, `ListarTodas()`, `ObtenerPorId()`
- `GestionDAL`: `ContarDatosNegocio()`, `InsertarEquipo()`, `InsertarUsoConFecha()`, `ListarEvaluaciones()`, `ListarSocios()`
- `HistorialAsistenciaSociosDAL`: `Insertar()`, `ObtenerPorFranja()`, `ObtenerPorFranjaYSemana()`
- `InformeGestionBLL`: `ADatosJson()`
- `InformeMantenimientoDAL`: `Obtener()`
- `LiquidacionBLL`: `EliminarFeriado()`, `ListarEmpleados()`, `ListarFeriados()`, `ListarLiquidaciones()`, `ListarValoresHora()`, `ObtenerDiasDeLiquidacion()`, `ObtenerIncompletos()`, `ObtenerParametros()`, `ResumenReciente()`, `RevocarExcedente()`
- `LiquidacionCalculo`: `Hhmm()`, `Horas()`, `Redondear()`
- `LiquidacionDAL`: `AprobarExcedente()`, `EliminarFeriado()`, `FechaDeFranja()`, `GuardarFeriado()`, `GuardarLiquidacion()`, `GuardarParametros()`, `GuardarValorHora()`, `HayLiquidacionEnRango()`, `ListarDiasDeLiquidacion()`, `ListarEmpleados()`, `ListarFeriados()`, `ListarLiquidaciones()`, `ListarTurnosAsignados()`, `ListarValoresHora()`, `MarcarPagada()`, `ObtenerParametros()`, `RevocarExcedente()`
- `LiquidacionExcel`: `Exportar()`
- `ReciboHoras`: `Mostrar()`
- `TurnoDAL`: `ContarAsignadosPorFranja()`, `ListarPendientesPorRol()`, `ListarPorUsuario()`
- `UsuarioBLL`: `ListarAdmins()`
- `UsuarioDAL`: `ListarPorRol()`
- `frmMenu`: `ActualizarVisibilidadMenu()`

### B. Diagramas de secuencia (CU-01 a CU-44)

Llamadas a métodos de clases del código en los diagramas de secuencia: **187**. Coinciden con una declaración de la clase destino: **184** (incluye 7 usos de `ShowDialog/Show/Close` heredados de `Form`, que no se declaran en el proyecto).
Procedimientos almacenados referenciados en mensajes a `:DB`: **155**. Existen en `script.sql`: **154**.

#### DiagramaSecuencia_CU02_RestaurarIntegridad.puml
- `frmRestaurarIntegridad.GuardarLogIntegridad()` no está declarado en `frmRestaurarIntegridad` (existe con ese nombre en: `Program`)
- `LogIn.cerrarSesion()` no está declarado en `LogIn` (existe con ese nombre en: `SessionManager`)

#### DiagramaSecuencia_CU05_CambiarIdioma.puml
- `Pantallas abiertas.ActualizarIdioma()`: `Pantallas abiertas` es un participante del diagrama que no es una clase del código

#### DiagramaSecuencia_CU34_FicharAsistencia.puml
- `AsistenciaPersonalBLL.TieneIngresoAbierto()` no está declarado en `AsistenciaPersonalBLL` (no existe en ninguna clase del proyecto)

#### DiagramaSecuencia_CU41_InformeGestion.puml
- procedimiento `ALERTA_REVISION_LISTAR_PENDIENTES` (mensaje a `:DB`) no existe en `script.sql`

### C. Métodos públicos de BLL y DAL que no aparecen en ningún diagrama de secuencia ni de clases
- `AlertaRevisionDAL`: `Obtener()`
- `AsistenciaPersonalDAL`: `InsertarManual()`, `ListarRango()`, `ObtenerAbierta()`
- `ConexionBLL`: `Disponible()`, `Identificador()`
- `ConexionDAL`: `Identificador()`, `Probar()`
- `DatosDemoBLL`: `BuscarArchivo()`
- `DemandaEstimadaDAL`: `ObtenerResumenCobertura()`
- `EvaluacionCoberturaBLL`: `Clasificar()`
- `GestionDAL`: `ContarDatosNegocio()`, `InsertarEquipo()`, `InsertarUsoConFecha()`, `ListarEvaluaciones()`, `ListarSocios()`
- `HistorialAsistenciaSociosDAL`: `ObtenerPorFranja()`, `ObtenerPorFranjaYSemana()`
- `InformeGestionBLL`: `ADatosJson()`
- `InformeMantenimientoDAL`: `Obtener()`
- `LiquidacionBLL`: `EliminarFeriado()`, `ListarEmpleados()`, `ListarFeriados()`, `ListarLiquidaciones()`, `ListarValoresHora()`, `ObtenerDiasDeLiquidacion()`, `ObtenerIncompletos()`, `ObtenerParametros()`, `ResumenReciente()`, `RevocarExcedente()`
- `LiquidacionCalculo`: `Hhmm()`, `Horas()`, `Redondear()`
- `LiquidacionDAL`: `EliminarFeriado()`, `FechaDeFranja()`, `GuardarLiquidacion()`, `HayLiquidacionEnRango()`, `ListarDiasDeLiquidacion()`, `ListarEmpleados()`, `ListarFeriados()`, `ListarLiquidaciones()`, `ListarTurnosAsignados()`, `ListarValoresHora()`, `ObtenerParametros()`, `RevocarExcedente()`
- `TurnoDAL`: `ContarAsignadosPorFranja()`
- `UsuarioDAL`: `ListarPorRol()`
