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

The script is append-only and mostly idempotent: `CONTROL_REGISTRAR` and `TRADUCCION_GUARDAR` are upserts, but `CREATE TABLE` / `CREATE PROCEDURE` blocks will error if objects already exist (harmless — the rest of the batch still runs). The `ALTER TABLE ... ADD DVH` blocks use `IF COL_LENGTH(...) IS NULL` and are fully idempotent.

## Architecture

Six projects with a strict one-way dependency chain:

```
CAPAS (UI/WinForms)
  ├── BLL
  │     ├── DAL
  │     │     └── BE
  │     └── BE
  └── SeguridadYServicios
              └── BE
```

**BE** — Plain entity classes (`USUARIO`, `BITACORA`, `IDIOMA`, `CONTROL_IDIOMA`, `NodoPermiso`, `DigitoVerificadorVertical`, etc.). No logic. Every entity protected by the integrity system has a `DVH int` property.

**DAL** — All DB access goes through `Acceso` (internal), which uses stored procedures exclusively — never inline SQL. `SqlParameter` objects are mandatory to prevent SQL injection. Connection string is in `Acceso.cs` targeting `BDCAPAS` on the local SQL Server instance.

**BLL** — Thin orchestration layer. Creates DAL instances directly (`new DAL.UsuarioDAL()`). No direct DB calls.

**SeguridadYServicios** — Cross-cutting concerns. Does NOT reference BLL or DAL; callers pass data in. Contains:
- `SessionManager` — Singleton, holds authenticated user and permissions for the session.
- `IdiomaManager` — Singleton/Subject for the Observer pattern. Holds active-language translations as `Dictionary<string, string>`. Callers (CAPAS) fetch translations via BLL and pass them in via `CambiarIdioma()`.
- `IObservadorIdioma` — Observer interface (`ActualizarIdioma()`).
- `Hasher` — SHA-256 password hashing (one-way).
- `ValidadorContrasena` — Password rules (min 6 chars, uppercase, digit).
- `IEntidadVerificable` — Generic interface for entities that support DVH; `ObtenerAtributosParaDVH()` returns attribute values in fixed canonical order excluding DVH itself.
- `CalculadorDVH` — Static utility with two methods: `Calcular(string[])` for DVH and `CalcularVertical(List<string[]>, int)` for DVV. Both use the formula `Σ Unicode(char) × posAtributo × posChar` (1-based). All integrity math lives here.

**CAPAS** — Windows Forms. References BLL and SeguridadYServicios. Every `Form` that should react to language changes implements `IObservadorIdioma`: saves design-time control texts on `Load`, registers with `IdiomaManager`, and deregisters on `FormClosed`.

## Key design patterns

**Composite (permissions tree)** — `NodoPermiso` is the abstract component; `PerfilPermiso` is a branch (familia/grupo), `Permiso` is a leaf (patente atómica). The tree is built in-memory in `PerfilBLL.ObtenerArbol()` from a flat DB table (`NODO_PERMISO` with self-referencing `PADRE_ID`).

**Observer (multi-language)** — `IdiomaManager` is the Subject. Forms are Observers. Each form has its own language selector added dynamically by `IdiomaUIHelper.AgregarSelector(form)` (called at the end of every form's `Load`), because `ShowDialog()` disables the parent form. On change: CAPAS calls `BLL.IdiomaBLL.CargarTraducciones(idiomaId)`, then passes the dictionary to `IdiomaManager.CambiarIdioma()`, which calls `Notificar()` → `ActualizarIdioma()` on every registered form. Forms fall back to their design-time text when a key has no translation. Translatable controls are registered in `CONTROL_IDIOMA` (DB) via `CONTROL_REGISTRAR` SP (idempotent). ABM is in `frmIdiomas`.

Two categories of UI text are **not** captured by `GuardarDefaults(this.Controls)` and need explicit handling:
- **Form title bar**: register with `_controles[this.Name] = this; _defaults[this.Name] = this.Text;` after `GuardarDefaults`.
- **DataGridView column headers**: not in `Controls` collection; translate in a separate `ActualizarEncabezados()` method called from both `ActualizarIdioma()` and after any `DataSource` assignment. Use keys like `colhdr_Id`, `colhdr_Usuario`, etc.

**Key collision between forms** — if two forms have a control with the same `Name` (e.g., both `frmBitacora` and `frmAdminUsuarios` have `lblTitulo`), they'd share the same translation key and get the same text. Fix by removing the generic key and registering a form-specific one in `Load`, after `GuardarDefaults`:
```csharp
_controles.Remove("lblTitulo");
_defaults.Remove("lblTitulo");
_controles["lblTitulo_Bitacora"] = lblTitulo;
_defaults["lblTitulo_Bitacora"]  = lblTitulo.Text;
```
Then register the unique key with `CONTROL_REGISTRAR` in the SQL script.

**Dynamic content (TreeView, generated text)** — controls whose text is built at runtime (e.g., node prefixes `"[Perfil] "`) are not in `_controles`. Call `IdiomaManager.getInstance().Traducir(key)` directly inside the method that builds the content, and call that method from `ActualizarIdioma()` to rebuild when the language changes.

**UI text vs. business data** — only static interface strings go through the translation system. User-created content stored in the DB (profile names, permission names, usernames) is not translatable and should not be registered as `CONTROL_IDIOMA` keys.

**Singleton** — Both `SessionManager` and `IdiomaManager` use double-checked locking. `SessionManager.cerrarSesion()` nulls `_instance` to allow re-authentication.

## Dígitos verificadores de integridad

The system protects **USUARIO**, **BITACORA**, and **NODO_PERMISO** against unauthorized out-of-system DB modifications.

**DVH (horizontal)** — one `int` per row, stored in the row's own `DVH` column. Formula: `Σᵢ Σⱼ Unicode(atrib[i][j]) × (i+1) × (j+1)`. Detects any change to a single record's fields.

**DVV (vertical)** — one `int` per logical column, stored in `DIGITO_VERIFICADOR_VERTICAL (TABLA, COLUMNA, DVV)`. Formula: `Σₖ Σⱼ Unicode(fila[k][col][j]) × (k+1) × (j+1)` where `k` is row position ordered by `ID`. Detects insertions, deletions, and row swaps.

**Multi-table state (USUARIO)** — a user's full state spans `USUARIO` and `USUARIO_PERFIL`. The virtual attribute `PERFILES` (sorted comma-separated profile IDs, e.g. `"2,5"`) is appended as the 7th attribute in USUARIO's DVH calculation. Changes to `USUARIO_PERFIL` outside the system invalidate the affected user's DVH.

**Canonical attribute order** — fixed per entity, must never change once data is stored:
- `USUARIO`: `ID, USUARIO, PASS, INTENTOS_FALLIDOS, BLOQUEADO("1"/"0"), ROL, PERFILES`
- `BITACORA`: `ID, USUARIO, ACCION, FECHA` (FECHA as `CONVERT(VARCHAR,FECHA,120)` from the SP)
- `NODO_PERMISO`: `ID, NOMBRE, TIPO, PADRE_ID` (PADRE_ID as `"0"` when NULL)

**Startup flow** (`Program.cs`): before `Application.Run(new LogIn())`, `IntegridadBLL.EstaInicializado()` checks whether DVVs exist for all three tables. If not (first run after migration), `RecalcularIntegridad()` initializes everything. Then `VerificarIntegridad()` is called; on failure a `MessageBox` is shown and `Main()` returns without opening the login form.

**After every mutation**, the appropriate recalculation method must be called:

| BLL method | Recalculation called |
|---|---|
| `UsuarioBLL.*` (Crear, Eliminar, Desbloquear, CambiarContrasena) | `RecalcularIntegridadUsuarios()` |
| `UsuarioPerfilBLL.GuardarAsignaciones()` | `RecalcularIntegridadUsuarios()` |
| `LoginBLL.AutenticarUsuario()` (on any path that changes INTENTOS_FALLIDOS or BLOQUEADO) | `RecalcularIntegridadUsuarios()` |
| `BitacoraBLL.*` (RegistrarLogin, RegistrarLogout, RegistrarAccion) | `RecalcularIntegridadBitacora()` |
| `PerfilBLL.*` (AgregarPerfil, AgregarPermiso, Eliminar) | `RecalcularIntegridadNodos()` |

`RecalcularIntegridad()` (no suffix) recalculates all three tables at once and is used only during initialization.

**Extending to a new entity** — add a `DVH int` column to the table, add rows to `DIGITO_VERIFICADOR_VERTICAL`, define the canonical attribute array and `ExtraerAtributos*` method in `IntegridadBLL`, then call `VerificarTabla` and `RecalcularTabla` (the generic internal helpers) following the existing BITACORA/NODO_PERMISO pattern.

## Database conventions

- All DB operations call stored procedures by name string (e.g., `"USUARIO_LOGIN"`).
- `Acceso.Leer()` returns a `DataTable`; `Acceso.Escribir()` returns affected row count.
- `Acceso` only has `CrearParametro` overloads for `string` and `int`. Pass `bool` values as `habilitado ? 1 : 0`.
- New tables and SPs are appended to `DAL/script.sql` (idempotent — uses `IF NOT EXISTS` / `IF OBJECT_ID ... DROP`).
- `EXEC` does not accept expression parameters — assign to a variable first: `DECLARE @t NVARCHAR(500); SET @t = 'a' + CHAR(10) + 'b'; EXEC SP @t`.

## Adding a new form with language support

1. Implement `SeguridadYServicios.IObservadorIdioma`.
2. Add `Dictionary<string, Control> _controles` and `Dictionary<string, string> _defaults` fields.
3. In `Load`:
   - Call `GuardarDefaults(this.Controls)`.
   - Add `_controles[this.Name] = this; _defaults[this.Name] = this.Text;` (registers the title bar).
   - Call `IdiomaManager.getInstance().Registrar(this)`, then `ActualizarIdioma()`.
   - Call `IdiomaUIHelper.AgregarSelector(this)` last (adds language combo to StatusStrip).
4. Wire `FormClosed` → `IdiomaManager.getInstance().Desregistrar(this)`.
5. `ActualizarIdioma()` iterates `_controles` and applies `Traducir(key) ?? _defaults[key]`. If the form has DataGridViews, also call `ActualizarEncabezados()`.
6. If the form has DataGridViews, add `ActualizarEncabezados()` using `colhdr_*` keys, and call it from `ActualizarIdioma()` and after every `DataSource` assignment.
7. Add `Load` and `FormClosed` event wiring in the `.Designer.cs`.
8. Add the `.cs` and `.Designer.cs` entries to `CAPAS/UI.csproj`.
9. Append `EXEC CONTROL_REGISTRAR` calls to `DAL/script.sql` for all new control keys and form name, plus `EXEC TRADUCCION_GUARDAR` for each supported language, then re-run the script.
