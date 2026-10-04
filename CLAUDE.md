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

## Installer (WiX)

`Instalador/` holds an SDK-style WiX Toolset **6.0.2** project (v7 requires accepting the OSMF EULA on every build machine, v6 does not). It is **not** part of `TP_IS.sln` (Visual Studio needs the HeatWave extension to load `.wixproj`); build it with:

```powershell
powershell -ExecutionPolicy Bypass -File Instalador\generar-instalador.ps1
```

The script finds MSBuild with `vswhere`, builds `TP_IS.sln` in Release (`/restore`) and then `Instalador\Bundle\Bundle.wixproj`, which builds `Instalador.wixproj` through a `ProjectReference` (WiX comes from NuGet; no separate install). Outputs: `Instalador\Bundle\bin\Release\GestionGimnasio-Setup.exe` (the distributable) and `Instalador\bin\Release\GestionGimnasio.msi`. Every user-visible installer text says "Gestión de gimnasio", never "CAPAS" (product/bundle name, install and Start-menu folder, shortcuts, license); `CAPAS` survives only as technical identifiers (`CAPAS.exe`, project/namespace, database `BDCAPAS`).

**Bundle** (`Instalador/Bundle/`, WiX Burn): `Bundle.wxs` uses `WixStandardBootstrapperApplication` (`rtfLicense` theme with a custom `ThemeFile` `Tema.xml` — a copy of WiX 6.0.2's `RtfTheme.xml` with a wider `LaunchButton` (170) and `EulaAcceptCheckbox` (300) so the Spanish texts fit; short `Licencia.rtf`; Spanish strings in `Tema.es-ES.wxl` — keep `EnableDefaultEmbeddedResourceItems=false` in `Bundle.wixproj` or the `.wxl` gets compiled as a localization instead of a theme file). Chain: `NetFx48Web` → optional `SqlLocalDB.msi` (same `IncluirLocalDB` define; `InstallCondition` skips it when `util:RegistrySearch` finds a SQL Server instance or LocalDB) → the application MSI (`MsiPackage Id="Aplicacion"`) with `INSTALLFOLDER=[InstallFolder]` (overridable from the Options page). `LaunchTarget` = `[InstallFolder]\ConfiguradorBD.exe` shows a "Configurar base de datos" button on the success page (the MSI's exit-dialog checkbox does not appear when the MSI runs inside the bundle). `Instalador.wixproj` excludes `Bundle\**` via `DefaultItemExcludes`. Bump `Bundle/@Version` together with `Package/@Version`; never change the bundle's `UpgradeCode`.

`Package.wxs`: per-machine, `es-ES` UI (`WixUI_InstallDir` + `Licencia.rtf`), `MajorUpgrade`, launch condition on `WIXNETFX4RELEASEINSTALLED >= #528040` (.NET Framework 4.8, Netfx extension), installs `CAPAS\bin\Release\**` (minus `*.pdb`, `*.xml`, `*.log`) via the `Files` wildcard element to `ProgramFiles6432Folder\Gestión de gimnasio`, the `DAL\*.sql` scripts to its `Scripts` subfolder, and Start-menu + desktop shortcuts. Bump `Package/@Version` for each release; never change `UpgradeCode`. The installer also adds a Start-menu shortcut to `ConfiguradorBD.exe` and, on the exit dialog, a checked-by-default checkbox that launches it (`WixShellExec` from the Util extension, impersonated, so Windows auth uses the real user — never run the DB setup as a deferred custom action: it would run as SYSTEM). If `Instalador\Prerequisitos\SqlLocalDB.msi` exists at build time (`IncluirLocalDB` define, git-ignored), it is packaged into the `Prerequisitos` subfolder.

**ConfiguradorBD** (`ConfiguradorBD/`, WinForms, .NET Framework 4.8, `requireAdministrator` manifest, in `TP_IS.sln`) builds into `CAPAS\bin\$(Configuration)\` so it sits next to `CAPAS.exe.config` and is harvested by the installer's `Files` wildcard. It: detects local instances from `HKLM\SOFTWARE\Microsoft\Microsoft SQL Server\Instance Names\SQL` (64/32-bit views) and LocalDB (`Local DB\Installed Versions`); maps the typed server to its Windows service (`InstanciaSql.ServicioDe`) and offers to start it if stopped; for `(localdb)\MSSQLLocalDB` runs `SqlLocalDB create/start`; with no engine, silently installs `Prerequisitos\SqlLocalDB.msi` (`IACCEPTSQLLOCALDBLICENSETERMS=YES`) or opens the download page; pre-flight checks (TCP port for remote servers — `InstanciaSql.PuertoTcpRemoto`/`PuertoAbierto`, default 1433 or the explicit `,port`; skipped for local servers, LocalDB and named instances without a port; connection, version ≥ 15 / SQL Server 2019, `CREATE ANY DATABASE`, disk space); runs `Scripts\script.sql` + `Scripts\traducciones.sql` batch by batch (`GO` splitter, falls back to `..\DAL\` in development); drops the database if any batch fails (rollback); if `BDCAPAS` already exists, only re-runs translations; writes the connection string to `CAPAS.exe.config`; logs to `%ProgramData%\GestionGimnasio\install.log`. `CAPAS/Program.cs` checks `BLL.ConexionBLL.Disponible()` at startup and offers to launch the configurator when the database is unreachable.

## Database setup

Two scripts live in `DAL/` (both UTF-8 with BOM, so `sqlcmd` reads accents correctly):

```powershell
# 1. Full schema (tables, FKs, all stored procedures) + idempotent seed data
sqlcmd -S . -E -i "DAL\script.sql"
# 2. Every UI text of the system and its Spanish/English/Portuguese translations
sqlcmd -S . -d BDCAPAS -E -i "DAL\traducciones.sql"
```

`script.sql` is the SSMS export of the real `BDCAPAS` database (it starts with `CREATE DATABASE`, so it is meant for a fresh install), made portable — `CREATE DATABASE [BDCAPAS]` without machine-specific file paths or `LEDGER`, and no `FILESTREAM` option (unsupported by LocalDB); requires SQL Server 2019+ — with `USUARIO_PERMISOS_LISTAR` in its non-inheriting version and a final idempotent seed block: languages (`Español` default, `Inglés`, `Portugues`), the `PERMISO` catalog, roles (`Administrador` protected with every permission; `Usuario`, `Sala`, `Recepcion`, `Tecnico` with *Cambiar contraseña*) and the `admin` user (password `1234`) assigned to `Administrador`. When the schema changes, update `script.sql` (re-export from SSMS or edit the affected object) instead of adding migration scripts.

`traducciones.sql` is the single source of translations and can be re-run at any time. It upserts the 429 keys of the system (`CONTROL_IDIOMA.TEXTO_DEFAULT` = Spanish, plus `TRADUCCION` rows for Español/Inglés/Portugues; it creates Inglés/Portugues if missing) inside a transaction, and **deletes keys that are no longer used** (with their translations). Because it overwrites translations, edits made from `frmIdiomas` must also be copied into the script to survive a re-run.

**Resetting integrity for testing** — if you need to force the app to reinitialize DVH/DVV (e.g., after a direct DB edit):
```sql
DELETE FROM DIGITO_VERIFICADOR_VERTICAL WHERE TABLA = 'USUARIO';
```
On next startup `EstaInicializado()` returns false → `RecalcularIntegridad()` runs automatically.

## Architecture

Six projects in the solution:

```
CAPAS (UI/WinForms)
  ├── BLL
  │     ├── DAL
  │     │     └── BE
  │     └── BE
  └── SeguridadYServicios
              └── BE

ConfiguradorBD (standalone setup tool, no project references)
```

`ConfiguradorBD` is the only code allowed to run SQL text directly (it executes the setup scripts batch by batch); the application itself goes through stored procedures only.

**BE** — Plain entity classes (`USUARIO`, `BITACORA`, `IDIOMA`, `CONTROL_IDIOMA`, `NodoPermiso`, `Rol`, `Permiso`, `UsuarioHistorial`, etc.) plus the `LoginResultado` enum. No logic. `USUARIO` has a `DVH int` property (the only integrity-protected entity). `NodoPermiso.ToString()` returns `Nombre` (used by `CheckedListBox` in `frmPerfiles`).

**DAL** — All DB access goes through `Acceso` (internal), which uses stored procedures exclusively — never inline SQL. `SqlParameter` objects are mandatory to prevent SQL injection. The connection string is read from `CAPAS/App.config` (`<connectionStrings>`, name `BDCAPAS`) via `ConfigurationManager`; `Acceso` throws a clear `InvalidOperationException` if it is missing. `Acceso.Cerrar()` and `DeshacerTX()` are null-safe so a failed `Abrir()` never masks the original exception.

**BLL** — Thin orchestration layer. Creates DAL instances directly (`new DAL.UsuarioDAL()`). No direct DB calls.

**SeguridadYServicios** — Cross-cutting concerns. Does NOT reference BLL or DAL; callers pass data in. Contains:
- `SessionManager` — Singleton, holds authenticated user and permissions (`List<string>`) for the session. `TienePermiso(string nombre)` checks against that list. `cerrarSesion()` nulls `_instance` to allow re-authentication.
- `IdiomaManager` — Singleton/Subject for the Observer pattern. Holds active-language translations as `Dictionary<string, string>`. Callers (CAPAS) fetch translations via BLL and pass them in via `CambiarIdioma()`.
- `IObservadorIdioma` — Observer interface (`ActualizarIdioma()`).
- `Hasher` — SHA-256 password hashing (one-way).
- `ValidadorContrasena` — Password rules (min 6 chars, uppercase, digit).
- `CalculadorDVH` — Static utility. `Calcular(string[])` for DVH; `CalcularVertical(List<string[]>, int)` for DVV. Formula: `Σ Unicode(char) × posAtributo × posChar` (1-based).

**CAPAS** — Windows Forms. References BLL and SeguridadYServicios. Every `Form` that should react to language changes implements `IObservadorIdioma`. All forms inherit from `ReaLTaiizor.Forms.MaterialForm` (not `System.Windows.Forms.Form`).

## UI theme (ReaLTaiizor)

The app uses **ReaLTaiizor 3.8.1.8** (NuGet, .NET Framework 4.8). Two layers cooperate:

**MaterialSkinManager** (global, `Program.cs`) — configured once before `Application.Run`:
```csharp
var skin = MaterialSkinManager.Instance;
skin.Theme = MaterialSkinManager.Themes.LIGHT;
skin.ColorScheme = new MaterialColorScheme(
    AppTheme.Acento, AppTheme.AcentoHover, AppTheme.Seleccion,
    AppTheme.AcentoHover, MaterialTextShade.WHITE);
```

**Per-form** — `FormBase.InicializarFormulario()` calls, as its last two steps, `MaterialSkinManager.Instance.AddFormToManage(this)` and `AppTheme.AplicarTema(this)` (see "Adding a new form"). `frmMenu` is the only form not derived from `FormBase` and calls them itself.

**`AppTheme.AplicarTema(form)`** (`CAPAS/AppTheme.cs`) — applies the corporate palette to Button, TextBox, Label, DataGridView, TreeView, ComboBox, Panel, GroupBox, MenuStrip, StatusStrip, and DateTimePicker controls recursively. White + blue/pink palette (gym branding): `FondoForm=#FFFFFF`, `FondoHeader=#E8F0FB` (soft blue), `FondoGrillaAlt=#FDF3F7` (soft pink), `FondoStatus=#E1EAF8`, accent `#3B6FB6` (blue: title bar, buttons, headers), hover `#C2386B` (pink, with white text), selection `#F8D7E3` (light pink), text `#2B2D42`. The MaterialForm title bar uses the same `AppTheme` colors (custom `Color` overload of `MaterialColorScheme`). All colors live in `AppTheme` — never hardcode colors elsewhere; labels always get `TextoPrincipal` (a white label would be invisible on the white background).

**Critical layout constraint** — `MaterialForm` renders its own title bar (~64 px) **inside** the client area at `y=0`. Controls in `.Designer.cs` must have `Location.Y ≥ ~70` or they will be hidden under the title bar. When designing a new form or adjusting an existing one, offset all content controls by at least 70 px from the top of the client area. The `ClientSize.Height` must be increased by the same amount relative to the visible content.

## Key design patterns

**Composite (roles and permissions tree)** — `NodoPermiso` is the abstract component; `Rol` (file: `PerfilPermiso.cs`) is a branch node stored in the `ROL` table; `Permiso` is a leaf stored in the `PERMISO` table. The tree is built in-memory in `PerfilBLL.ObtenerArbol()`:
1. `PerfilDAL.ListarRoles()` fetches all `ROL` rows and builds the rol hierarchy from `PADRE_ID`.
2. `PerfilDAL.ListarRolPermisos()` fetches all rows from `ROL_PERMISO` joined with `PERMISO`; each returned `Permiso` carries its rol's ID in the `PadreId` field as a carrier value, which `ObtenerArbol()` uses to attach the permiso to the correct rol branch.

**Roles and permissions data model** — Roles and permissions live in two separate tables (migrated off the old single-table `NODO_PERMISO` design; the legacy table and its `PERFIL_*` SPs still exist in `script.sql` but nothing uses them):
- **`ROL`** (`ID, NOMBRE, PADRE_ID, PROTEGIDO`): created and deleted from the UI, `PADRE_ID` used for role hierarchy.
- **`PERMISO`** (`ID, NOMBRE`): pre-established catalog, never created or deleted from the UI. Currently: *Ver bitácora*, *Administrar usuarios*, *Gestión de roles*, *Gestión de idiomas*, *Cambiar contraseña*.

The many-to-many link between roles and permissions lives in `ROL_PERMISO (ROL_ID, PERMISO_ID)`, with real FKs to both `ROL` and `PERMISO`. `PERMISO_LISTAR_TODOS` lists the `PERMISO` catalog; `ROL_LISTAR_TODOS` lists all roles.

`ROL_ELIMINAR` cascades via a recursive CTE: it collects the entire subtree of descendant role IDs and deletes their entries from `USUARIO_PERFIL` and `ROL_PERMISO` before removing them from `ROL`. `PerfilDAL.GuardarPermisosDeRol` runs inside a transaction (LIMPIAR + N × INSERTAR).

`NODO_PERMISO` is kept as an unused legacy remnant (not dropped, for safety) — its data was copied into `ROL`/`PERMISO` and no current SP references it.

**Role parent assignment** — `PADRE_ID` on a `ROL` row establishes role hierarchy. `PerfilBLL.CambiarPadre` calls two in-memory guards before persisting, each throwing `InvalidOperationException`:
1. `GenerariaCiclo` — walks the parent chain up from the candidate parent; rejects it if `rolId` appears (candidate is a descendant of the role, would create a cycle).
2. `EsAncestroDelPadreActual` — walks the parent chain up from the role's *current* parent (excluding it); rejects the candidate if it appears there (candidate is an ancestor above the current parent — only "lateral" moves or the no-op current parent are allowed).

`frmPerfiles.PopularComboPadre` mirrors rule 1 by excluding the role's subtree (`RecolectarDescendientes`) from the combo entirely. Rule 2's targets — ancestors above the current parent (`RecolectarAncestrosSuperiores`, stored in `_padresDeshabilitados`) — remain visible in the combo but are rendered grayed-out (`cboPadre_DrawItem`, `OwnerDrawFixed`) and cannot be selected (`cboPadre_SelectedIndexChanged` reverts to `_cboPadreIndiceAnterior`). The SP `ROL_CAMBIAR_PADRE` does a plain `UPDATE ROL SET PADRE_ID`.

**No permission inheritance** — `ROL.PADRE_ID` is purely organizational: it controls how roles are nested in the tree shown by `frmPerfiles` (via `PerfilBLL.ObtenerArbol()`), nothing else. A role's actual permissions are exactly its own rows in `ROL_PERMISO` — never a parent's. `USUARIO_PERMISOS_LISTAR` looks up permissions directly from `USUARIO_PERFIL → ROL_PERMISO → PERMISO` for the user's own assigned role(s), with no walk up `PADRE_ID`. This was previously CU-15 ("inherited permissions"), removed after it caused a privilege-escalation bug: a role accidentally parented under `Administrador` silently inherited all of its permissions. `chkPermisos` in `frmPerfiles` renders with default (non-owner-draw) checkboxes — checked state reflects only the role's own `ROL_PERMISO` rows.

**Role deletion guards** — `PerfilBLL.Eliminar` enforces two rules before calling the DAL, throwing `InvalidOperationException` for each:
1. `Rol.Protegido == true` → system role, cannot be deleted. Currently only *Administrador* (`PROTEGIDO=1`); all other roles default to `PROTEGIDO=0`.
2. `PerfilDAL.TieneUsuariosAsignados(id)` → the SP `ROL_TIENE_USUARIOS` uses a recursive CTE to check whether any role in the subtree has entries in `USUARIO_PERFIL`. Blocks deletion if so.

**Anti-lockout guard** — `PerfilBLL.ValidarQueQuedeAdministracion` rejects (with `InvalidOperationException`) any change that would leave the system without at least one active (non-blocked) user holding each administration permission (`Administrar usuarios`, `Gestión de roles`). It simulates the change in memory (data from `USUARIO_PERFIL_LISTAR_ACTIVOS` and `ROL_PERMISO_LISTAR_TODOS`) and only blocks if a permission that was covered before stops being covered. Called from `PerfilBLL.ActualizarPermisosDeRol`, `UsuarioPerfilBLL.GuardarAsignaciones`, `UsuarioBLL.Eliminar` and `UsuarioHistorialBLL.Rollback` (a snapshot with `Bloqueado=true` counts as losing all roles). The UI catches the exception and shows the message.

**Atomic role assignment** — `UsuarioPerfilDAL.ReemplazarAsignaciones` runs `USUARIO_PERFIL_BORRAR_TODOS` + N × `USUARIO_PERFIL_ASIGNAR` inside a transaction; used by both `GuardarAsignaciones` and `Rollback`.

**Bitácora for security changes** — `PerfilBLL` logs `ROL_CREADO:`, `ROL_ELIMINADO:`, `ROL_PADRE_CAMBIADO:` and `ROL_PERMISOS_MODIFICADOS:` (+ role name); `UsuarioPerfilBLL.GuardarAsignaciones` logs `PERFILES_ASIGNADOS:` (+ username). `BITACORA.ACCION` is `VARCHAR(50)`, so `BitacoraBLL.RegistrarAccion` truncates longer actions.

`frmPerfiles` shows three context-sensitive panels when a `Rol` node is selected: `panelPadre` (ComboBox to reassign parent, excludes the role's own subtree from candidates) and `panelPermisos` (CheckedListBox of catalog permissions). The Eliminar button is disabled in the UI when `rol.Protegido` is true.

**Permissions check flow** — `LoginBLL` calls `UsuarioPerfilBLL.ObtenerPermisos(usuarioId)` → `USUARIO_PERMISOS_LISTAR` SP → joins `USUARIO_PERFIL → ROL_PERMISO → PERMISO` for the user's own roles (no inheritance, see above) to get distinct permission names → stored in `SessionManager._permisos`. UI checks via `SessionManager.TienePermiso("Ver bitácora")`. **Never check `USUARIO.Rol` to control visibility** — it is a legacy field not aligned with the permission system (the old `SessionManager.EsAdmin()` helper that did this was removed).

**Observer (multi-language)** — `IdiomaManager` is the Subject. Forms are Observers (`FormBase` implements `IObservadorIdioma`). Each form has its own language selector added dynamically by `IdiomaUIHelper.AgregarSelector(form)` (called by `FormBase.InicializarFormulario()`), because `ShowDialog()` disables the parent form. On change the UI calls `FachadaIdioma.Cambiar(idioma)`, which loads the translations and passes them to `IdiomaManager.CambiarIdioma()`, which calls `Notificar()` → `ActualizarIdioma()` on every registered form. Forms fall back to their design-time text when a key has no translation.

**Facade (`BLL.FachadaIdioma`)** — single entry point for language operations, hiding `IdiomaBLL`, `IdiomaManager`, `SessionManager` and `UsuarioBLL`. `Cambiar(idioma)` applies a language and, if there is a session, saves it as the user's preference; `AplicarPreferencia(usuario)` (used by `LoginBLL`) applies the saved language only if it is still enabled; `RecargarSiEstaActivo(idiomaId)` (used by `frmIdiomas` after saving translations); `ListarDisponibles()`, `IdiomaActivo`, `Traducir(clave)`, `RegistrarClave(clave, textoDefault)`. UI code must not call `IdiomaManager.CambiarIdioma` or `IdiomaBLL.CargarTraducciones` directly.

**Template Method (`CAPAS.FormBase`)** — base class (derives from `MaterialForm`, implements `IObservadorIdioma`) for every form except `frmMenu`. Owns `_controles`/`_defaults`, `GuardarDefaults`, `ActualizarIdioma`, and unregisters from `IdiomaManager` in `OnFormClosed`. `InicializarFormulario()` runs the fixed sequence (GuardarDefaults → `AjustarClaves()` hook → title-bar key → `Registrar` → `ActualizarIdioma()` → `AgregarSelector` → skin → theme). Translation keys are **scoped by form**: `<FormName>.<ControlName>` for controls, `<FormName>` for the title bar — so two forms can reuse a control name (e.g. `lblTitulo`) without sharing a translation. Hooks: `AjustarClaves()` (call `ExcluirDeTraduccion(control)` for labels whose text is generated at runtime, e.g. `lblPagina`, so a language change does not overwrite them) and `ActualizarTextosDinamicos()` (grid headers, tree refresh — called at the end of every `ActualizarIdioma()`). Helper `Encabezado(grilla, columna, clave, texto)` sets a column header through `Textos.T` if the column exists. All 31 forms except `frmMenu` derive from `FormBase`; `frmMenu` scopes its menu/status-strip keys as `frmMenu.<ItemName>` and its title as `frmMenu_Titulo`.

**Translated messages (`CAPAS.Textos`)** — every `MsgBox.Show` message and `InputBox` prompt goes through `Textos.T("msg_Clave", "texto en español", args...)`: returns the active language's translation or the Spanish default, applying `string.Format` with `args` (falls back to the default if a translation has a broken placeholder). The first time a key is used in a session it is registered in `CONTROL_IDIOMA` (`CONTROL_REGISTRAR`, insert-only), so it appears in `frmIdiomas` ready to translate. `MsgBox` translates titles itself (key `tit_` + CamelCase of the Spanish title, via `Textos.ClaveDesdeTexto`) and its buttons (`btnmsg_Si`, `btnmsg_No`, `btnmsg_Aceptar`). Runtime texts use the same helper with prefixes `lbl_` (dynamic labels), `hdr_` (grid headers), `chart_` (dashboard charts) and `input_` (InputBox). English/Portuguese translations for every key are in `DAL/traducciones.sql`. Messages that come from BLL exceptions (`ex.Message`) or `ValidadorContrasena` are still Spanish-only.

**`IDIOMA.PREDETERMINADO`** — `BIT` column marking the system's default language (`Español`, seeded via `UPDATE IDIOMA SET PREDETERMINADO = 1 WHERE NOMBRE = 'Español'`). A language can be neither **deleted** nor **disabled** in two cases, each with its own message:
1. It is the default language (`Predeterminado == true`).
2. It is assigned to *any* user — `IdiomaBLL.EstaEnUso(id)` (SP `IDIOMA_ESTA_EN_USO`, `SELECT COUNT(*) FROM USUARIO WHERE IDIOMA_ID = @id`).

The rules are enforced in three layers: `frmIdiomas` pre-checks before asking for confirmation; `IdiomaBLL.Eliminar` / `IdiomaBLL.ActualizarEstado(id, false)` throw `InvalidOperationException` as a backstop (caught by the UI); and the SP `IDIOMA_ELIMINAR` re-checks both conditions (`THROW 50001/50002`) and deletes `TRADUCCION` + `IDIOMA` inside a transaction, so translations are never lost on a failed delete. `LoginBLL` only applies a user's saved language if it is still `Habilitado`.

**Per-user language preference (`USUARIO.IDIOMA_ID`)** — nullable FK to `IDIOMA(ID)` (`FK_USUARIO_IDIOMA`); `NULL` means "no preference saved yet". Both language selectors (`IdiomaUIHelper.AgregarSelector`'s combo and `frmMenu.cboIdiomaStatus`) call `FachadaIdioma.Cambiar(idioma)`, which persists the choice through `UsuarioBLL.ActualizarIdioma` (SP `USUARIO_ACTUALIZAR_IDIOMA`) only when there is a logged-in user — the pre-login `LogIn` selector does not persist. On successful login, `LoginBLL.AutenticarUsuario` calls `FachadaIdioma.AplicarPreferencia(u)` (`u.IdiomaId` is returned by `USUARIO_LOGIN`; the language is loaded with SP `IDIOMA_OBTENER_POR_ID`) — this happens *before* `frmMenu` is constructed, so the menu loads already translated. `BE.USUARIO.IdiomaId` (`int?`) is hidden from `dgvUsuarios` in `frmAdminUsuarios` (not user-relevant in that grid).

The form title bar is registered automatically by `FormBase` (key = form `Name`). **DataGridView column headers** are not in the `Controls` collection: set them in an `ActualizarEncabezados()` method using `Encabezado(...)` (keys `hdr_*`; older forms use `colhdr_*` through `Traducir`), call it after any `DataSource` assignment and from the `ActualizarTextosDinamicos()` override.

**Dynamic content (TreeView, generated text)** — controls whose text is built at runtime (e.g., node prefixes `"[Rol] "`, `"[Permiso] "`) are not in `_controles`. Call `IdiomaManager.getInstance().Traducir(key)` directly. Translation keys: `prefijo_Rol`, `prefijo_Permiso`.

**Singleton** — Both `SessionManager` and `IdiomaManager` use double-checked locking.

**Prototype (duplicate role)** — `NodoPermiso.Clonar()` is abstract; `Rol.Clonar()` deep-copies the role and recursively clones its children, `Permiso.Clonar()` copies the leaf. `PerfilBLL.DuplicarRol(rolId, nuevoNombre)` clones the role from the tree, renames it, clears `Protegido`, inserts it under the same parent and saves the clone's own permissions (sub-roles are not duplicated); logged as `ROL_DUPLICADO:`. UI: "Duplicar rol" button in `frmPerfiles` (enabled only when a role is selected).

## Dígitos verificadores de integridad

The system protects **USUARIO** against unauthorized out-of-system DB modifications. **BITACORA**, **ROL**, **PERMISO**, and **ROL_PERMISO** are not integrity-protected.

**DVH (horizontal)** — one `int` per row, stored in the row's own `DVH` column. Formula: `Σᵢ Σⱼ Unicode(atrib[i][j]) × (i+1) × (j+1)`.

**DVV (vertical)** — one `int` per logical column, stored in `DIGITO_VERIFICADOR_VERTICAL (TABLA, COLUMNA, DVV)`. Formula: `Σₖ Σⱼ Unicode(fila[k][col][j]) × (k+1) × (j+1)` where `k` is row position ordered by `ID`.

**Multi-table state (USUARIO)** — the virtual attribute `PERFILES` (sorted comma-separated role IDs from `USUARIO_PERFIL`) is appended as the 7th attribute in USUARIO's DVH. Changes to `USUARIO_PERFIL` outside the system invalidate the affected user's DVH.

**Canonical attribute order** — fixed, must never change once data is stored:
- `USUARIO`: `ID, USUARIO, PASS, INTENTOS_FALLIDOS, BLOQUEADO("1"/"0"), ROL, PERFILES`

**Valid ROL values** — defined by `frmNuevoUsuario`'s ComboBox: `'admin'` and `'usuario'`. The `ROL` string column is a legacy display/classification field, kept for compatibility and included in the DVH canonical attribute array (its position must never change). `USUARIO.ROL_ID` (nullable FK to `ROL(ID)`, backfilled from the legacy string) is the real source of truth: actual permissions are resolved via `USUARIO_PERFIL → ROL_PERMISO → PERMISO`.

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

**Integrity errors are logged** to `%LOCALAPPDATA%\GestionGimnasio\integridad_error.log` (via `Program.GuardarLogIntegridad`), with timestamp, each time `frmRestaurarIntegridad` loads. Not next to the `.exe`: once installed under Program Files that folder is read-only for standard users.

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

**Change types**: `ALTA`, `BAJA`, `BLOQUEO`, `DESBLOQUEO`, `CAMBIO_CLAVE`, `ASIGNACION_PERFIL`, `ROLLBACK`, `EDICION_DATOS`.

**Snapshot timing** — `RegistrarCambio` must be called **after** the change, with one exception: `BAJA` is recorded **before** `_dal.Eliminar()` so the last known state is captured while the row still exists.

**Rollback** (`UsuarioHistorialBLL.Rollback`) — applies `ROL`, `BLOQUEADO`, `INTENTOS_FALLIDOS`, `PERFILES`, `NOMBRE`, and `APELLIDO` from the chosen snapshot. `PASS` is untouched. The rollback itself is recorded as a new `ROLLBACK` entry with `VERSION_ORIGEN` pointing to the restored snapshot ID. Calls `RecalcularIntegridadUsuarios()` internally.

**`NOMBRE`/`APELLIDO` (non-critical fields)** — `USUARIO.NOMBRE` and `USUARIO.APELLIDO` are free-text personal data, optional and not part of any business rule. They are deliberately **excluded from the DVH/DVV canonical attribute array** (`IntegridadBLL.ColumnasUsuario`) — editing them never affects integrity digests. They ARE snapshotted in `USUARIO_HISTORIAL` and restored on rollback (see above), so history/rollback stays a faithful point-in-time copy, but `UsuarioBLL.ActualizarDatos` does **not** call `RecalcularIntegridadUsuarios()`. Edited via `frmEditarUsuario` ("Editar datos" button in `frmAdminUsuarios`), which records an `EDICION_DATOS` entry.

**Anti-recursion** — a `ROLLBACK` entry cannot itself be the target of another rollback. Enforced in two places: `frmHistorialUsuario` rejects the selection before asking for confirmation, and `UsuarioHistorialBLL.Rollback()` throws `InvalidOperationException` as a backstop if called anyway.

**No FK to USUARIO** — intentional: the historial row survives the deletion of the user it describes.

**Limitation**: `frmRestaurarIntegridad` can only offer rollback for users who have at least one entry in `USUARIO_HISTORIAL`. Users created before the historial feature was implemented have no records and can only be resolved via "Recalcular y continuar".

## Database conventions

- All DB operations call stored procedures by name string (e.g., `"USUARIO_LOGIN"`).
- `Acceso.Leer()` returns a `DataTable`; `Acceso.Escribir()` returns affected row count.
- `Acceso` only has `CrearParametro` overloads for `string` and `int`. Pass `bool` values as `habilitado ? 1 : 0`.
- For **nullable int** parameters, `CrearParametro` cannot be used — construct manually: `new SqlParameter("@version_origen", SqlDbType.Int) { Value = (object)h.VersionOrigen ?? DBNull.Value }`.
- New tables and SPs go into `DAL/script.sql` (keep it in sync with the real database); new UI texts go into `DAL/traducciones.sql`.
- `EXEC` does not accept expression parameters — assign to a variable first.
- **PERSONA** table exists in the DB but is not used — legacy leftover from the initial scaffold.
- When a batch in `script.sql` uses `DECLARE @var` variables, a `GO` statement resets scope. Each batch after a `GO` must re-declare any variables it needs.
- To drop a column that has a DEFAULT constraint, first drop the constraint dynamically (its auto-generated name varies per instance), then drop the column.

## Adding a new form with language support

1. Inherit from `CAPAS.FormBase` (which already derives from `MaterialForm` and implements `IObservadorIdioma`).
2. In the `Load` handler, put form-specific setup that must happen first (data that `ActualizarTextosDinamicos` needs, default values), then call `InicializarFormulario();`, then anything that should run after the theme (e.g., layout adjustments).
3. Override `ActualizarTextosDinamicos()` if the form has DataGridView headers or other runtime text, and `AjustarClaves()` to exclude labels whose text is generated at runtime. Control names may repeat across forms (keys are scoped by form).
4. Use `Textos.T("msg_...", "texto")` for every `MsgBox.Show` message and `InputBox` prompt.
5. In `.Designer.cs`: place all content controls at `Location.Y ≥ 70` to clear the ~64 px MaterialForm title bar. Set `ClientSize.Height` to accommodate the shifted layout. Wire only `Load` (unregistering on close is handled by `FormBase`).
6. Add the `.cs` and `.Designer.cs` entries to `CAPAS/UI.csproj`.
7. Add the new keys (`<FormName>` title, `<FormName>.<ControlName>` for each control with text, and any `Textos.T` keys) with their Spanish/English/Portuguese texts to `DAL/traducciones.sql` and re-run it. `Textos.T` keys also register themselves at runtime, so they appear in `frmIdiomas` even before the script is updated.

## Documentation artifacts

All diagram sources, generated images, and doc-generation scripts live under `DIAGRAMAS/`. PlantUML sources are rendered via `DIAGRAMAS/generar_png.py` (uses kroki.io, requires internet).

| File | Type | Purpose |
|---|---|---|
| `DIAGRAMAS/DER.puml` / `.png` | DER | Modelo entidad-relación de BDCAPAS |
| `DIAGRAMAS/DiagramaClases.puml` / `.png` | Diagrama de clases | Capas BE, BLL, DAL, SeguridadYServicios |
| `DIAGRAMAS/DiagramaClases_Composite.puml` / `.png` / `.svg` | Diagrama de clases (patrón) | Composite — árbol de roles/permisos (`NodoPermiso`/`Rol`/`Permiso`), con comentarios |
| `DIAGRAMAS/DiagramaClases_Observer.puml` / `.png` / `.svg` | Diagrama de clases (patrón) | Observer — multiidioma (`IdiomaManager`/`IObservadorIdioma`), con comentarios |
| `DIAGRAMAS/DiagramaClases_Singleton.puml` / `.png` / `.svg` | Diagrama de clases (patrón) | Singleton — `SessionManager`/`IdiomaManager`, con comentarios |
| `DIAGRAMAS/DiagramaClases_Facade.puml` / `.png` / `.svg` | Diagrama de clases (patrón) | Facade — `FachadaIdioma` y el subsistema de idiomas, con comentarios |
| `DIAGRAMAS/DiagramaClases_Prototype.puml` / `.png` / `.svg` | Diagrama de clases (patrón) | Prototype — `NodoPermiso.Clonar()` y "Duplicar rol", con comentarios |
| `DIAGRAMAS/DiagramaClases_TemplateMethod.puml` / `.png` / `.svg` | Diagrama de clases (patrón) | Template Method — `FormBase.InicializarFormulario()` y sus hooks, con comentarios |
| `DIAGRAMAS/DiagramaComponentes.puml` | Diagrama de componentes | Proyectos del .sln (BE, DAL, BLL, SeguridadYServicios, CAPAS), interfaces entre capas y BDCAPAS |
| `DIAGRAMAS/DiagramaComponentes/DiagramaComponentes - TP_IS.png` | Diagrama de componentes (render) | PNG renderizado del anterior |
| `DIAGRAMAS/DiagramaSecuencia_LoginIntegridad.puml` / `.png` | Secuencia (detallado) | Versión técnica del login + integridad, incluye Hasher, DALs, etc. |
| `DIAGRAMAS/DiagramaCasosDeUso_Seguridad1/2.puml`, `_Negocio.puml` / `.png` | Casos de uso | CU-01..CU-37 agrupados por subsistema (seguridad en dos diagramas, procesos de negocio en uno) |
| `DIAGRAMAS/DiagramaSecuencia_CU01..CU37_*.puml` / `.png` | Secuencia por CU | Nivel UI/BLL/DB, uno por caso de uso (CU-21 en dos partes: `CU21` propuesta y `CU21b` asignación/confirmación). Alineados con el código: los nombres de SP coinciden con los de `DAL/` |
| `DIAGRAMAS/CasosDeUso.docx` | Documento unificado | Versión anterior con los 20 CUs de seguridad (no incluye CU-21..CU-37) |
| `DIAGRAMAS/generar_casos_uso_docx.py` | Generador | Regenera `CasosDeUso.docx` desde el dict `CUS` definido en el script |

Sin acceso a kroki.io se puede renderizar localmente con el jar de PlantUML (requiere Java y Graphviz). Los diagramas grandes superan el límite por defecto de 4096 px, por eso se sube `PLANTUML_LIMIT_SIZE`:

```powershell
java -DPLANTUML_LIMIT_SIZE=16384 -jar plantuml.jar -tpng -pipe < DiagramaClases.puml > DiagramaClases.png
```

Regenerar un PNG individual (desde `DIAGRAMAS/`):

```powershell
python generar_png.py <archivo.puml> <archivo.png>
```

Regenerar en lote todos los diagramas de secuencia (con reintentos):

```powershell
python generar_pngs_lote.py
```

Regenerar el `.docx` de casos de uso (tras editar `CUS` en el script):

```powershell
python generar_casos_uso_docx.py
```

## Modeling conventions (use cases & sequence diagrams)

- **Actores:** solo dos — `Usuario` (base) y `Administrador` (especialización que generaliza a Usuario). Los permisos individuales (*Administrar usuarios*, *Gestión de roles*, *Gestión de idiomas*, *Ver bitácora*, *Cambiar contraseña*) son precondiciones del CU, no actores separados.
- **Diagramas de secuencia:** nivel UI / BLL / DB. `:DB` consolida todos los DALs como un `database`. Servicios transversales (`:SessionManager`, `:IntegridadBLL`) aparecen solo cuando son relevantes al flujo del actor.
- **Estilo PlantUML:** sin `box` agrupador; cada participante con color tenue por capa — CAPAS `#FEFECE`, BLL `#E8F4E8`, DAL `#E8E8F4`, Seguridad `#F4E8E8`. Skinparams comunes: `shadowing false`, `roundcorner 6`, `hide footbox`, `dpi 140`, `maxMessageSize 115` (envuelve los mensajes largos para que el diagrama entre legible en una página vertical; con más de 6 participantes conviene mover detalles a notas o partir el diagrama).
- **CU descriptions:** plantilla Cockburn — ID, actor primario, frecuencia, prioridad, propósito, precondiciones, postcondiciones (éxito y fallo), disparador, flujo principal numerado, flujos alternativos (`Xa`, `Xb`, etc.), excepciones, reglas de negocio y relaciones (`«include»` / `«extend»`).
