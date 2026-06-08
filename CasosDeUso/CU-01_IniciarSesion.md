# CU-01 — Iniciar Sesión

| Campo | Valor |
|---|---|
| **Identificador** | CU-01 |
| **Nombre** | Iniciar Sesión |
| **Actor primario** | Usuario del sistema |
| **Actor secundario** | Administrador (solo en flujo alternativo de restauración de integridad) |
| **Frecuencia** | Alta — al inicio de cada uso de la aplicación |
| **Prioridad** | Crítica |

## Propósito

Permitir que un usuario registrado se autentique en el sistema para acceder a las funcionalidades correspondientes a su rol.

## Precondiciones

- La aplicación `CAPAS.exe` está iniciada.
- La base de datos `BDCAPAS` está accesible.
- El sistema ya ejecutó la verificación de integridad inicial y conoce su resultado.
- El usuario existe en la base de datos.

## Postcondiciones

### En caso de éxito
- Existe una sesión activa en `SessionManager` con el usuario autenticado y su lista de permisos.
- El contador de intentos fallidos del usuario queda en cero.
- El evento de login queda registrado en la bitácora.
- Los dígitos verificadores de la tabla `USUARIO` quedan recalculados.
- Se muestra el menú principal.

### En caso de fallo por credenciales inválidas
- No se crea sesión.
- El contador de intentos fallidos del usuario se incrementa.
- El evento `LOGIN_FALLIDO` queda en bitácora.
- Si el contador alcanzó el límite: el usuario queda bloqueado, se registran los eventos `USUARIO_BLOQUEADO` en bitácora y `BLOQUEO` en el historial del usuario.

### En caso de fallo por integridad comprometida
- No se crea sesión válida para el usuario común.
- El error queda registrado en `integridad_error.log`.
- Solo un administrador con datos íntegros puede acceder, exclusivamente para restaurar el sistema (CU-02).

## Disparador

El usuario presiona el botón **Ingresar** en el formulario de login.

## Flujo principal (curso normal)

1. El usuario ingresa su **nombre de usuario** y **contraseña**.
2. El usuario presiona **Ingresar**.
3. El sistema valida que ambos campos no estén vacíos.
4. El sistema verifica que el usuario no se encuentre bloqueado.
5. El sistema valida las credenciales contra la base de datos.
6. El sistema resetea el contador de intentos fallidos del usuario.
7. El sistema carga los permisos del usuario y los almacena en la sesión.
8. El sistema registra el evento de login en la bitácora.
9. El sistema evalúa el resultado de la verificación de integridad realizada al arranque.
10. El sistema recalcula los dígitos verificadores de la tabla `USUARIO`.
11. El sistema muestra un mensaje de bienvenida y abre el menú principal.

## Flujos alternativos

### 3a. Algún campo está vacío
- 3a.1. El sistema muestra el mensaje *"Completá usuario y contraseña"*.
- 3a.2. El caso de uso vuelve al paso 1.

### 4a. El usuario está bloqueado
- 4a.1. El sistema no valida las credenciales.
- 4a.2. Si la integridad es válida, el sistema recalcula los dígitos verificadores.
- 4a.3. El sistema muestra el mensaje *"Usuario bloqueado por intentos fallidos. Contactate con un administrador."*
- 4a.4. El caso de uso vuelve al paso 1.

### 5a. Credenciales inválidas — sin alcanzar el límite de intentos
- 5a.1. El sistema incrementa el contador de intentos fallidos.
- 5a.2. El sistema registra el evento `LOGIN_FALLIDO` en la bitácora.
- 5a.3. Si la integridad es válida, el sistema recalcula los dígitos verificadores.
- 5a.4. El sistema muestra el mensaje *"Usuario o contraseña incorrectos"*.
- 5a.5. El caso de uso vuelve al paso 1.

### 5b. Credenciales inválidas — se alcanzó el límite de intentos
- 5b.1. El sistema incrementa el contador y bloquea al usuario.
- 5b.2. El sistema registra `LOGIN_FALLIDO` y `USUARIO_BLOQUEADO` en la bitácora.
- 5b.3. El sistema registra `BLOQUEO` en el historial del usuario.
- 5b.4. Si la integridad es válida, el sistema recalcula los dígitos verificadores.
- 5b.5. El sistema muestra el mensaje *"Usuario bloqueado por intentos fallidos."*
- 5b.6. El caso de uso vuelve al paso 1.

### 9a. La integridad está comprometida y el usuario es administrador con sus propios datos íntegros
- 9a.1. El sistema invoca el caso de uso **CU-02 — Restaurar Integridad del Sistema** («extend»).
- 9a.2. Si la restauración finaliza con éxito, el caso de uso continúa en el paso 11.
- 9a.3. Si el administrador cancela, el sistema cierra la sesión y el caso de uso vuelve al paso 1.

### 9b. La integridad está comprometida y el usuario no puede restaurar
*(el usuario no es administrador, o sus propios datos están entre los afectados)*
- 9b.1. El sistema muestra el mensaje *"El sistema no puede iniciarse debido a un problema interno. Comuníquese con el administrador del sistema."*
- 9b.2. El sistema cierra la sesión.
- 9b.3. El caso de uso vuelve al paso 1.

## Excepciones

| Código | Descripción | Manejo |
|---|---|---|
| EX-01 | Error de conexión con la base de datos | Mensaje genérico al usuario, registro en log de la aplicación, vuelve al paso 1. |
| EX-02 | Error inesperado durante la verificación de integridad | El resultado se marca como inválido con el detalle del error y se sigue el flujo 9a / 9b. |

## Reglas de negocio asociadas

| Código | Regla |
|---|---|
| RN-01 | Las contraseñas se almacenan con hash SHA-256 unidireccional. La contraseña en claro nunca se guarda ni se loguea. |
| RN-02 | Un usuario queda bloqueado automáticamente al alcanzar el límite de intentos fallidos consecutivos. |
| RN-03 | La verificación de integridad se ejecuta una sola vez, al arrancar la aplicación. Su resultado no impide abrir el formulario de login pero sí condiciona el acceso al menú. |
| RN-04 | El recálculo de dígitos verificadores se realiza después de toda mutación de `USUARIO`, salvo cuando la integridad ya estaba comprometida (no se debe "sanar silenciosamente" datos corruptos). |
| RN-05 | Solo un administrador cuyos propios datos no estén entre los afectados por la corrupción puede acceder al sistema cuando la integridad está comprometida, y únicamente para restaurarla. |

## Relaciones con otros casos de uso

- **«extend» CU-02 — Restaurar Integridad del Sistema**: se activa desde el paso 9 cuando la integridad falló y el usuario tiene permiso para restaurar.
- **«include» Verificación de integridad**: ejecutada como paso automático del sistema al arrancar (no es un CU del actor humano, sino una rutina de inicialización).

## Observaciones

- El formulario de login siempre está disponible aunque la integridad falle; la decisión de permitir o no el acceso se toma *después* de validar las credenciales.
- El recálculo en los flujos 4a, 5a y 5b se realiza incluso ante un intento fallido porque la operación de incrementar intentos modifica la tabla `USUARIO` y, si los dígitos verificadores estaban válidos, deben mantenerse coherentes con el nuevo estado.
