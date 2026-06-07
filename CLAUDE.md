# CLAUDE.md

This file provides guidance to Claude Code (claude.ai/code) when working with code in this repository.

## Build

```powershell
# Build the full solution
& "C:\Program Files\Microsoft Visual Studio\2022\Community\MSBuild\Current\Bin\MSBuild.exe" TP_IS.sln /p:Configuration=Debug /v:minimal

# Build a single project
& "C:\Program Files\Microsoft Visual Studio\2022\Community\MSBuild\Current\Bin\MSBuild.exe" BE\BE.csproj /p:Configuration=Debug
```

The entry point is `CAPAS` (outputs `CAPAS.exe`). There are no automated tests; verification is manual via the running application.

## Database setup

Run the full script against the local SQL Server instance (Windows Auth, database `BDCAPAS`):

```powershell
sqlcmd -S . -d BDCAPAS -E -i "DAL\script.sql"
```

The script is append-only and mostly idempotent: `CONTROL_REGISTRAR` and `TRADUCCION_GUARDAR` are upserts, but early `CREATE TABLE` / `CREATE PROCEDURE` blocks will error if objects already exist (harmless — the rest of the batch still runs). Later blocks use `IF OBJECT_ID ... DROP` before recreating, so they are fully idempotent.

**Resetting integrity for testing** — if you need to force the app to reinitialize DVH/DVV (e.g., after a direct DB edit):
```sql
DELETE FROM DIGITO_VERIFICADOR_VERTICAL WHERE TABLA = 'USUARIO';
```
On next startup `EstaInicializado()` returns false → `RecalcularIntegridad()` runs automatically.

## Architecture

Eight projects in the solution. Six are active; two are **legacy predecessors** that must not be confused with the active ones:

```
CAPAS (UI/WinForms)
  ├── BLL
  │     ├── DAL
  │     │     └── BE
  │     └── BE
  └── SeguridadYServicios
              └── BE

Seguridad/   ← LEGACY: superseded by SeguridadYServicios
Servicio/    ← LEGACY: superseded by SeguridadYServicios
```

**BE** — Plain entity classes (`USUARIO`, `BITACORA`, `IDIOMA`, `CONTROL_IDIOMA`, `NodoPermiso`, `Rol`, `Permiso`, `DigitoVerificadorVertical`, `UsuarioHistorial`, etc.) plus the `LoginResultado` enum. No logic. `USUARIO` has a `DVH int` property (the only integrity-protected entity). `NodoPermiso.ToString()` returns `Nombre` (used by `CheckedListBox` in `frmPerfiles`).

**DAL** — All DB access goes through `Acceso` (internal), which uses stored procedures exclusively — never inline SQL. `SqlParameter` objects are mandatory to prevent SQL injection. Connection string is in `Acceso.cs` targeting `BDCAPAS` on the local SQL Server instance.

**BLL** — Thin orchestration layer. Creates DAL instances directly (`new DAL.UsuarioDAL()`). No direct DB calls.

**SeguridadYServicios** — Cross-cutting concerns. Does NOT reference BLL or DAL; callers pass data in. Contains:
- `SessionManager` — Singleton, holds authenticated user and permissions (`List<string>`) for the session. `TienePermiso(string nombre)` checks against that list. `cerrarSesion()` nulls `_instance` to allow re-authentication.
- `IdiomaManager` — Singleton/Subject for the Observer pattern. Holds active-language translations as `Dictionary<string, string>`. Callers (CAPAS) fetch translations via BLL and pass them in via `CambiarIdioma()`.
- `IObservadorIdioma` — Observer interface (`ActualizarIdioma()`).
- `Hasher` — SHA-256 password hashing (one-way).
- `ValidadorContrasena` — Password rules (min 6 chars, uppercase, digit).
- `CalculadorDVH` — Static utility. `Calcular(string[])` for DVH; `CalcularVertical(List<string[]>, int)` for DVV. Formula: `Σ Unicode(char) × posAtributo × posChar` (1-based).

**CAPAS** — Windows Forms. References BLL and SeguridadYServicios. Every `Form` that should react to language changes implements `IObservadorIdioma`.

## Key design patterns

**Composite (roles and permissions tree)** — `NodoPermiso` is the abstract component; `Rol` (file: `PerfilPermiso.cs`) is a branch node stored as `TIPO='PERFIL'` in DB; `Permiso` is a leaf stored as `TIPO='PERMISO'`. The tree is built in-memory in `PerfilBLL.ObtenerArbol()`:
1. `PerfilDAL.ListarRoles()` fetches all `TIPO='PERFIL'` nodes and builds the rol hierarchy from `PADRE_ID`.
2. `PerfilDAL.ListarRolPermisos()` fetches all rows from `ROL_PERMISO` joined with `NODO_PERMISO`; each returned `Permiso` carries its rol's ID in the `PadreId` field as a carrier value, which `ObtenerArbol()` uses to attach the permiso to the correct rol branch.

**Roles and permissions data model** — Two distinct concepts live in `NODO_PERMISO`:
- **Roles** (`TIPO='PERFIL'`, `PADRE_ID` used for role hierarchy): created and deleted from the UI.
- **Catalog permissions** (`TIPO='PERMISO'`, `PADRE_ID=NULL`): pre-established, never created or deleted from the UI. Currently: *Ver bitácora*, *Administrar usuarios*, *Gestión de roles*, *Gestión de idiomas*, *Cambiar contraseña*.

The many-to-many link between roles and permissions lives in `ROL_PERMISO (ROL_ID, PERMISO_ID)`. `PERMISO_LISTAR_TODOS` filters `PADRE_ID IS NULL` so only catalog permisos appear; `PERFIL_LISTAR_TODOS` filters `TIPO='PERFIL'`.

`PERFIL_ELIMINAR` cascades via a recursive CTE: it collects the entire subtree of descendant role IDs and deletes their entries from `USUARIO_PERFIL` and `ROL_PERMISO` before removing them from `NODO_PERMISO`. `PerfilDAL.GuardarPermisosDeRol` runs inside a transaction (LIMPIAR + N × INSERTAR).

**Role parent assignment** — `PADRE_ID` on a `TIPO='PERFIL'` row establishes role hierarchy. `PerfilBLL.CambiarPadre` and `AgregarRol` both call `GenerariaCiclo` (walks the parent chain in-memory) before persisting, preventing circular references. The SP `PERFIL_CAMBIAR_PADRE` does a plain `UPDATE NODO_PERMISO SET PADRE_ID`.

**Role deletion guards** — `PerfilBLL.Eliminar` enforces two rules before calling the DAL, throwing `InvalidOperationException` for each:
1. `Rol.Protegido == true` → system role, cannot be deleted. Currently only *Administrador* (`PROTEGIDO=1`); all other roles default to `PROTEGIDO=0`.
2. `PerfilDAL.TieneUsuariosAsignados(id)` → the SP `PERFIL_TIENE_USUARIOS` uses a recursive CTE to check whether any role in the subtree has entries in `USUARIO_PERFIL`. Blocks deletion if so.

`frmPerfiles` shows three context-sensitive panels when a `Rol` node is selected: `panelPadre` (ComboBox to reassign parent, excludes the role's own subtree from candidates) and `panelPermisos` (CheckedListBox of catalog permissions). The Eliminar button is disabled in the UI when `rol.Protegido` is true.

**Permissions check flow** — `LoginBLL` calls `UsuarioPerfilBLL.ObtenerPermisos(usuarioId)` → `USUARIO_PERMISOS_LISTAR` SP → joins `USUARIO_PERFIL → ROL_PERMISO → NODO_PERMISO` to get distinct permission names → stored in `SessionManager._permisos`. UI checks via `SessionManager.TienePermiso("Ver bitácora")`.

**Observer (multi-language)** — `IdiomaManager` is the Subject. Forms are Observers. Each form has its own language selector added dynamically by `IdiomaUIHelper.AgregarSelector(form)` (called at the end of every form's `Load`), because `ShowDialog()` disables the parent form. On change: CAPAS calls `BLL.IdiomaBLL.CargarTraducciones(idiomaId)`, then passes the dictionary to `IdiomaManager.CambiarIdioma()`, which calls `Notificar()` → `ActualizarIdioma()` on every registered form. Forms fall back to their design-time text when a key has no translation.

Two categories of UI text are **not** captured by `GuardarDefaults(this.Controls)` and need explicit handling:
- **Form title bar**: register with `_controles[this.Name] = this; _defaults[this.Name] = this.Text;` after `GuardarDefaults`.
- **DataGridView column headers**: not in `Controls` collection; translate in a separate `ActualizarEncabezados()` method called from both `ActualizarIdioma()` and after any `DataSource` assignment. Use keys like `colhdr_Id`, `colhdr_Usuario`, etc.

**Key collision between forms** — if two forms have a control with the same `Name`, fix by removing the generic key and registering a form-specific one in `Load`:
```csharp
_controles.Remove("lblTitulo");
_defaults.Remove("lblTitulo");
_controles["lblTitulo_Bitacora"] = lblTitulo;
_defaults["lblTitulo_Bitacora"]  = lblTitulo.Text;
```

**Dynamic content (TreeView, generated text)** — controls whose text is built at runtime (e.g., node prefixes `"[Rol] "`, `"[Permiso] "`) are not in `_controles`. Call `IdiomaManager.getInstance().Traducir(key)` directly. Translation keys: `prefijo_Rol`, `prefijo_Permiso`.

**Singleton** — Both `SessionManager` and `IdiomaManager` use double-checked locking.

## Dígitos verificadores de integridad

The system protects **USUARIO** against unauthorized out-of-system DB modifications. **BITACORA**, **NODO_PERMISO**, and **ROL_PERMISO** are not integrity-protected.

**DVH (horizontal)** — one `int` per row, stored in the row's own `DVH` column. Formula: `Σᵢ Σⱼ Unicode(atrib[i][j]) × (i+1) × (j+1)`.

**DVV (vertical)** — one `int` per logical column, stored in `DIGITO_VERIFICADOR_VERTICAL (TABLA, COLUMNA, DVV)`. Formula: `Σₖ Σⱼ Unicode(fila[k][col][j]) × (k+1) × (j+1)` where `k` is row position ordered by `ID`.

**Multi-table state (USUARIO)** — the virtual attribute `PERFILES` (sorted comma-separated role IDs from `USUARIO_PERFIL`) is appended as the 7th attribute in USUARIO's DVH. Changes to `USUARIO_PERFIL` outside the system invalidate the affected user's DVH.

**Canonical attribute order** — fixed, must never change once data is stored:
- `USUARIO`: `ID, USUARIO, PASS, INTENTOS_FALLIDOS, BLOQUEADO("1"/"0"), ROL, PERFILES`

**Valid ROL values** — defined by `frmNuevoUsuario`'s ComboBox: `'admin'` and `'usuario'`. The `ROL` column is a display/classification field only; actual permissions are resolved via `USUARIO_PERFIL → ROL_PERMISO → NODO_PERMISO`.

**Startup and login flow** (`Program.cs` + `LogIn.cs`):

1. `Program.cs` calls `InicializarIntegridad()` before opening the login form:
   - If not initialized (no DVVs for USUARIO), calls `RecalcularIntegridad()` first.
   - Calls `VerificarIntegridad()` and stores the result in `Program.ResultadoIntegridad` (static).
   - **Does not block the app** — login form always opens.

2. After a successful login, `LogIn.cs` evaluates `Program.ResultadoIntegridad`:
   - If **valid**: `RecalcularIntegridadUsuarios()` is called (to update DVH after `ResetearIntentos` mutation), then `frmMenu` opens normally.
   - If **invalid**, two conditions determine who can restore:
     - **Admin whose own ID is NOT in `ResultadoIntegridad.IdsUsuariosAfectados`** → sees `frmRestaurarIntegridad` with two options (see below).
     - **Everyone else** (regular users, or admin whose own data is compromised) → sees a generic message and is blocked.

3. On **failed login** (wrong credentials or blocked user), `RecalcularIntegridadUsuarios()` is called **only if integrity is OK**. If integrity is compromised, no recalculation happens — corruption must not be silently healed by a login attempt.

**Critical invariant**: `LoginBLL.AutenticarUsuario()` never calls `RecalcularIntegridadUsuarios()`. All recalculation responsibility lives in `LogIn.cs` (CAPAS layer), conditioned on `Program.ResultadoIntegridad`.

**`ResultadoIntegridad`** has two fields:
- `Errores: List<string>` — human-readable descriptions of each failure.
- `IdsUsuariosAfectados: List<int>` — IDs of USUARIO rows with DVH mismatch (populated only for USUARIO, not DVV failures).

**`frmRestaurarIntegridad`** — shown to a trusted admin when integrity fails. Displays the error detail and offers:
- **Recalcular y continuar**: accepts the current DB state as legitimate, recalculates all DVH/DVV, and proceeds to `frmMenu`.
- **Restaurar desde historial**: opens `frmHistorialUsuario` for each affected user so the admin can roll back to a prior snapshot. Disabled if no affected user has any history records. After restoring, recalculates and proceeds to `frmMenu`.
- **Cancelar**: clears the session, returns to the login form.

**Integrity errors are logged** to `integridad_error.log` in the same folder as the `.exe` (via `Program.GuardarLogIntegridad`), with timestamp, each time `frmRestaurarIntegridad` loads.

**After every mutation of USUARIO**, the appropriate recalculation method must be called:

| Where | Trigger | Recalculation |
|---|---|---|
| `LogIn.cs` | Successful login, integrity OK | `RecalcularIntegridadUsuarios()` |
| `LogIn.cs` | Failed login, integrity OK | `RecalcularIntegridadUsuarios()` |
| `frmRestaurarIntegridad` | Admin chooses Recalcular or Restaurar | `RecalcularIntegridadUsuarios()` |
| `UsuarioBLL.*` (Crear, Eliminar, Desbloquear, CambiarContrasena) | Any user mutation | `RecalcularIntegridadUsuarios()` |
| `UsuarioPerfilBLL.GuardarAsignaciones()` | Profile assignment change | `RecalcularIntegridadUsuarios()` |
| `UsuarioHistorialBLL.Rollback()` | Rollback applied | `RecalcularIntegridadUsuarios()` |

`RecalcularIntegridad()` (no suffix) delegates to `RecalcularIntegridadUsuarios()` and is used only during initialization.

**Extending to a new entity** — add a `DVH int` column to the table, add rows to `DIGITO_VERIFICADOR_VERTICAL`, define the canonical attribute array and `ExtraerAtributos*` method in `IntegridadBLL`, then call `VerificarTabla` and `RecalcularTabla` (the generic internal helpers) following the existing USUARIO pattern.

## Control de cambios (USUARIO_HISTORIAL)

`USUARIO_HISTORIAL` is an append-only audit table: no UPDATEs, no DELETEs. Each row is a full snapshot of a user's state at a point in time. `PASS` is never stored.

**Change types**: `ALTA`, `BAJA`, `BLOQUEO`, `DESBLOQUEO`, `CAMBIO_CLAVE`, `ASIGNACION_PERFIL`, `ROLLBACK`.

**Snapshot timing** — `RegistrarCambio` must be called **after** the change, with one exception: `BAJA` is recorded **before** `_dal.Eliminar()` so the last known state is captured while the row still exists.

**Rollback** (`UsuarioHistorialBLL.Rollback`) — applies `ROL`, `BLOQUEADO`, `INTENTOS_FALLIDOS`, and `PERFILES` from the chosen snapshot. `PASS` is untouched. The rollback itself is recorded as a new `ROLLBACK` entry with `VERSION_ORIGEN` pointing to the restored snapshot ID. Calls `RecalcularIntegridadUsuarios()` internally.

**No FK to USUARIO** — intentional: the historial row survives the deletion of the user it describes.

**Limitation**: `frmRestaurarIntegridad` can only offer rollback for users who have at least one entry in `USUARIO_HISTORIAL`. Users created before the historial feature was implemented have no records and can only be resolved via "Recalcular y continuar".

## Database conventions

- All DB operations call stored procedures by name string (e.g., `"USUARIO_LOGIN"`).
- `Acceso.Leer()` returns a `DataTable`; `Acceso.Escribir()` returns affected row count.
- `Acceso` only has `CrearParametro` overloads for `string` and `int`. Pass `bool` values as `habilitado ? 1 : 0`.
- For **nullable int** parameters, `CrearParametro` cannot be used — construct manually: `new SqlParameter("@version_origen", SqlDbType.Int) { Value = (object)h.VersionOrigen ?? DBNull.Value }`.
- New tables and SPs are appended to `DAL/script.sql`.
- `EXEC` does not accept expression parameters — assign to a variable first.
- **PERSONA** table exists in the DB but is not used — legacy leftover from the initial scaffold.
- When a batch in `script.sql` uses `DECLARE @var` variables, a `GO` statement resets scope. Each batch after a `GO` must re-declare any variables it needs.
- To drop a column that has a DEFAULT constraint, first drop the constraint dynamically (its auto-generated name varies per instance), then drop the column.

## Adding a new form with language support

1. Implement `SeguridadYServicios.IObservadorIdioma`.
2. Add `Dictionary<string, Control> _controles` and `Dictionary<string, string> _defaults` fields.
3. In `Load`:
   - Call `GuardarDefaults(this.Controls)`.
   - Add `_controles[this.Name] = this; _defaults[this.Name] = this.Text;` (registers the title bar).
   - Call `IdiomaManager.getInstance().Registrar(this)`, then `ActualizarIdioma()`.
   - Call `IdiomaUIHelper.AgregarSelector(this)` last.
4. Wire `FormClosed` → `IdiomaManager.getInstance().Desregistrar(this)`.
5. `ActualizarIdioma()` iterates `_controles` and applies `Traducir(key) ?? _defaults[key]`. If the form has DataGridViews, also call `ActualizarEncabezados()`.
6. Add `Load` and `FormClosed` event wiring in the `.Designer.cs`.
7. Add the `.cs` and `.Designer.cs` entries to `CAPAS/UI.csproj`.
8. Append `EXEC CONTROL_REGISTRAR` calls to `DAL/script.sql` for all new control keys and form name, plus `EXEC TRADUCCION_GUARDAR` for each supported language, then re-run the script.

## Diagrams

`DER.puml` and `DiagramaClases.puml` in the repo root are PlantUML source files. To regenerate PNG images (requires Python and internet access):

```powershell
python generar_png.py DER.puml DER.png
python generar_png.py DiagramaClases.puml DiagramaClases.png
```

The script uses kroki.io as the rendering backend.
