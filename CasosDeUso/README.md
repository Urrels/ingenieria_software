# Catálogo de Casos de Uso

Sistema de Gestión de Usuarios — TP Ingeniería de Software.

## Actores

| Actor | Descripción |
|---|---|
| **Usuario** | Persona registrada en el sistema. Puede iniciar sesión, cerrar sesión, cambiar su contraseña y cambiar el idioma de la interfaz. |
| **Administrador** | Especialización de *Usuario*. Tiene a su cargo la gestión de usuarios, roles, idiomas y la consulta de la bitácora, además de poder restaurar la integridad del sistema. |

> El "Sistema" actúa como actor implícito en rutinas de inicialización (verificación de integridad al arrancar) — no se modela como actor humano.

---

## Catálogo

### Sesión y seguridad

| ID | Nombre | Actor primario | Estado |
|---|---|---|---|
| [CU-01](CU-01_IniciarSesion.md) | Iniciar Sesión | Usuario | Documentado |
| CU-02 | Restaurar Integridad del Sistema | Administrador | Pendiente |
| CU-03 | Cerrar Sesión | Usuario | Pendiente |
| CU-04 | Cambiar Contraseña | Usuario | Pendiente |
| CU-05 | Cambiar Idioma | Usuario | Pendiente |

### Gestión de usuarios

| ID | Nombre | Actor primario | Estado |
|---|---|---|---|
| CU-06 | Crear Usuario | Administrador | Pendiente |
| CU-07 | Eliminar Usuario | Administrador | Pendiente |
| CU-08 | Desbloquear Usuario | Administrador | Pendiente |
| CU-09 | Asignar Perfiles a Usuario | Administrador | Pendiente |
| CU-10 | Ver Historial de Usuario | Administrador | Pendiente |
| CU-11 | Restaurar Usuario desde Historial | Administrador | Pendiente |

### Gestión de roles y permisos

| ID | Nombre | Actor primario | Estado |
|---|---|---|---|
| CU-12 | Crear Rol | Administrador | Pendiente |
| CU-13 | Eliminar Rol | Administrador | Pendiente |
| CU-14 | Cambiar Padre de Rol | Administrador | Pendiente |
| CU-15 | Asignar Permisos a Rol | Administrador | Pendiente |

### Gestión de idiomas

| ID | Nombre | Actor primario | Estado |
|---|---|---|---|
| CU-16 | Crear Idioma | Administrador | Pendiente |
| CU-17 | Editar Idioma (renombrar / habilitar) | Administrador | Pendiente |
| CU-18 | Eliminar Idioma | Administrador | Pendiente |
| CU-19 | Cargar / Editar Traducciones | Administrador | Pendiente |

### Auditoría

| ID | Nombre | Actor primario | Estado |
|---|---|---|---|
| CU-20 | Ver Bitácora | Administrador | Pendiente |

---

## Relaciones entre casos de uso

| Origen | Tipo | Destino | Condición |
|---|---|---|---|
| CU-01 | «extend» | CU-02 | Integridad inválida y el usuario es Administrador con sus propios datos íntegros. |
| CU-11 | «include» | CU-10 | El rollback siempre parte de ver el historial. |
| CU-19 (cualquier alta o edición) | «include» | (rutina) `RegistrarControl` | Cada control nuevo registrado en BD para soportar traducción. |

---

## Permisos requeridos

Todos los CUs de Administrador requieren autenticación previa y el permiso correspondiente en el sistema de permisos. La UI oculta los ítems de menú cuyo permiso el usuario no tiene.

| Permiso | CUs que lo requieren |
|---|---|
| Administrar usuarios | CU-06, CU-07, CU-08, CU-09, CU-10, CU-11 |
| Gestión de roles | CU-12, CU-13, CU-14, CU-15 |
| Gestión de idiomas | CU-16, CU-17, CU-18, CU-19 |
| Ver bitácora | CU-20 |
| Cambiar contraseña | CU-04 |
| *(ninguno — todo usuario autenticado)* | CU-03, CU-05 |
| *(no autenticado)* | CU-01 |

---

## Reglas globales que aplican a todos los CUs de mutación

| Regla | Detalle |
|---|---|
| RG-01 | Toda mutación de `USUARIO` (incluyendo `USUARIO_PERFIL`) dispara recálculo de DVH/DVV. |
| RG-02 | Toda mutación de un usuario queda registrada como snapshot en `USUARIO_HISTORIAL` (excepto la contraseña, que nunca se guarda). |
| RG-03 | Toda acción significativa queda registrada en `BITACORA` con usuario, acción y fecha. |
| RG-04 | Los CUs administrativos requieren sesión activa y el permiso correspondiente; en caso contrario el ítem del menú ni siquiera está visible. |
| RG-05 | Los roles marcados como `PROTEGIDO=1` (actualmente solo *Administrador*) no pueden eliminarse ni dejarse sin permisos críticos. |
