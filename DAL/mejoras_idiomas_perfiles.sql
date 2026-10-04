IF OBJECT_ID('dbo.IDIOMA_ELIMINAR', 'P') IS NOT NULL DROP PROCEDURE [dbo].[IDIOMA_ELIMINAR]
GO
CREATE PROCEDURE [dbo].[IDIOMA_ELIMINAR]
    @id INT
AS
BEGIN
    SET XACT_ABORT ON;

    IF EXISTS (SELECT 1 FROM IDIOMA WHERE ID = @id AND PREDETERMINADO = 1)
        THROW 50001, 'No se puede eliminar el idioma predeterminado del sistema.', 1;

    IF EXISTS (SELECT 1 FROM USUARIO WHERE IDIOMA_ID = @id)
        THROW 50002, 'No se puede eliminar un idioma que está en uso por algún usuario.', 1;

    BEGIN TRANSACTION;
        DELETE FROM TRADUCCION WHERE IDIOMA_ID = @id;
        DELETE FROM IDIOMA     WHERE ID        = @id;
    COMMIT TRANSACTION;
END
GO

IF OBJECT_ID('dbo.USUARIO_PERFIL_LISTAR_ACTIVOS', 'P') IS NOT NULL DROP PROCEDURE [dbo].[USUARIO_PERFIL_LISTAR_ACTIVOS]
GO
CREATE PROCEDURE [dbo].[USUARIO_PERFIL_LISTAR_ACTIVOS]
AS
    SELECT u.ID AS USUARIO_ID, up.PERFIL_ID
    FROM USUARIO u
    LEFT JOIN USUARIO_PERFIL up ON up.USUARIO_ID = u.ID
    WHERE u.BLOQUEADO = 0
GO

DECLARE @textos TABLE (CLAVE VARCHAR(100), ES NVARCHAR(400), EN NVARCHAR(400), PT NVARCHAR(400));
INSERT INTO @textos (CLAVE, ES, EN, PT) VALUES
    ('btnDuplicarRol', N'Duplicar rol', N'Duplicate role', N'Duplicar perfil'),
    ('btnmsg_Aceptar', N'Aceptar', N'OK', N'OK'),
    ('btnmsg_No', N'No', N'No', N'Não'),
    ('btnmsg_Si', N'Sí', N'Yes', N'Sim'),
    ('input_AgregarIdioma', N'Agregar idioma', N'Add language', N'Adicionar idioma'),
    ('input_AgregarRol', N'Agregar Rol', N'Add role', N'Adicionar perfil'),
    ('input_DuplicarRol', N'Duplicar rol', N'Duplicate role', N'Duplicar perfil'),
    ('input_NombreDelNuevoIdioma', N'Nombre del nuevo idioma:', N'Name of the new language:', N'Nome do novo idioma:'),
    ('input_NombreDelNuevoRol', N'Nombre del nuevo rol:', N'Name of the new role:', N'Nome do novo perfil:'),
    ('input_NombreDelRol', N'Nombre del rol:', N'Role name:', N'Nome do perfil:'),
    ('input_NuevoNombre', N'Nuevo nombre:', N'New name:', N'Novo nome:'),
    ('input_RenombrarIdioma', N'Renombrar idioma', N'Rename language', N'Renomear idioma'),
    ('input_SufijoCopia', N'{0} (copia)', N'{0} (copy)', N'{0} (cópia)'),
    ('msg_AlertaDescartada', N'Alerta descartada.', N'Alert dismissed.', N'Alerta descartado.'),
    ('msg_AsignacionesGuardadas', N'Asignaciones guardadas.', N'Assignments saved.', N'Atribuições salvas.'),
    ('msg_AsistenciaDeSociosRegistrada', N'Asistencia de socios registrada.', N'Member attendance recorded.', N'Presença de sócios registrada.'),
    ('msg_AutorizarLaVisitaTecnicaPara', N'¿Autorizar la visita técnica para ''{0}''?', N'Authorize the technical visit for ''{0}''?', N'Autorizar a visita técnica para ''{0}''?'),
    ('msg_AvisoUnaAusenciaParaEsteTurno', N'¿{0} {1} avisó una ausencia para este turno?', N'Did {0} {1} report an absence for this shift?', N'{0} {1} avisou uma ausência para este turno?'),
    ('msg_Bienvenido', N'Bienvenido, {0}!', N'Welcome, {0}!', N'Bem-vindo, {0}!'),
    ('msg_CierreConfirmadoEquipoOperativo', N'Cierre confirmado. Equipo operativo con el contador reiniciado.', N'Closure confirmed. Equipment operational with the counter reset.', N'Fechamento confirmado. Equipamento operacional com o contador reiniciado.'),
    ('msg_CompletaTodosLosCampos', N'Completá todos los campos.', N'Fill in all the fields.', N'Preencha todos os campos.'),
    ('msg_CompletaUsuarioYContrasena', N'Completá usuario y contraseña.', N'Enter username and password.', N'Preencha usuário e senha.'),
    ('msg_ConfirmarCancelarTurno', N'¿Seguro que querés cancelar el turno del {0} {1}?' + CHAR(10) + N'Se va a notificar automáticamente al administrador y a los empleados disponibles para cubrirlo.', N'Are you sure you want to cancel the shift on {0} {1}?' + CHAR(10) + N'The administrator and the employees available to cover it will be notified automatically.', N'Tem certeza de que deseja cancelar o turno de {0} {1}?' + CHAR(10) + N'O administrador e os funcionários disponíveis para cobri-lo serão notificados automaticamente.'),
    ('msg_ConfirmarCierre', N'¿Confirmar el cierre? Se reiniciará el contador de uso y el equipo quedará operativo.', N'Confirm the closure? The usage counter will be reset and the equipment will be operational.', N'Confirmar o fechamento? O contador de uso será reiniciado e o equipamento ficará operacional.'),
    ('msg_ConfirmarCierreConRepuestoPendiente', N'Este informe indica que falta un repuesto. El equipo quedará marcado ''En mantenimiento'' y NO se reiniciará el contador de uso. ¿Confirmar de todas formas?', N'This report indicates a missing spare part. The equipment will be marked ''Under maintenance'' and the usage counter will NOT be reset. Confirm anyway?', N'Este relatório indica que falta uma peça. O equipamento ficará marcado ''Em manutenção'' e o contador de uso NÃO será reiniciado. Confirmar mesmo assim?'),
    ('msg_ConfirmarRollback', N'¿Restaurar el estado del usuario ''{0}'' a la versión del {1:dd/MM/yyyy HH:mm}?' + CHAR(10) + N'' + CHAR(10) + N'Tipo de cambio registrado: {2}' + CHAR(10) + N'' + CHAR(10) + N'Nota: la contraseña NO se restaurará (se mantendrá la actual).', N'Restore user ''{0}'' to the version from {1:dd/MM/yyyy HH:mm}?' + CHAR(10) + N'' + CHAR(10) + N'Recorded change type: {2}' + CHAR(10) + N'' + CHAR(10) + N'Note: the password will NOT be restored (the current one is kept).', N'Restaurar o usuário ''{0}'' para a versão de {1:dd/MM/yyyy HH:mm}?' + CHAR(10) + N'' + CHAR(10) + N'Tipo de alteração registrado: {2}' + CHAR(10) + N'' + CHAR(10) + N'Observação: a senha NÃO será restaurada (a atual será mantida).'),
    ('msg_ContrasenaCambiadaExitosamente', N'Contraseña cambiada exitosamente.', N'Password changed successfully.', N'Senha alterada com sucesso.'),
    ('msg_DesbloquearAlUsuario', N'¿Desbloquear al usuario ''{0}''?', N'Unlock user ''{0}''?', N'Desbloquear o usuário ''{0}''?'),
    ('msg_DescartarEstaAlertaComoFalsoPositivo', N'¿Descartar esta alerta como falso positivo?', N'Dismiss this alert as a false positive?', N'Descartar este alerta como falso positivo?'),
    ('msg_DisponibilidadRegistradaCorrectamente', N'Disponibilidad registrada correctamente.', N'Availability recorded successfully.', N'Disponibilidade registrada com sucesso.'),
    ('msg_EgresoRegistradoALas', N'Egreso registrado a las {0:HH:mm}.', N'Clock-out recorded at {0:HH:mm}.', N'Saída registrada às {0:HH:mm}.'),
    ('msg_ElNombreDeUsuarioYaExiste', N'El nombre de usuario ya existe.', N'The username already exists.', N'O nome de usuário já existe.'),
    ('msg_ElSistemaNoPuedeIniciarse', N'El sistema no puede iniciarse debido a un problema interno.' + CHAR(10) + N'' + CHAR(10) + N'Comuníquese con el administrador del sistema.', N'The system cannot start due to an internal problem.' + CHAR(10) + N'' + CHAR(10) + N'Please contact the system administrator.', N'O sistema não pode ser iniciado devido a um problema interno.' + CHAR(10) + N'' + CHAR(10) + N'Entre em contato com o administrador do sistema.'),
    ('msg_ElUsuarioNoEstaBloqueado', N'El usuario no está bloqueado.', N'The user is not locked.', N'O usuário não está bloqueado.'),
    ('msg_ElegiUnTecnicoHabitualOAlternativoAntesDeConfirmar', N'Elegí un técnico (habitual o alternativo) antes de confirmar.', N'Choose a technician (usual or alternate) before confirming.', N'Escolha um técnico (habitual ou alternativo) antes de confirmar.'),
    ('msg_EliminarAlUsuarioEstaAccionNoSePuedeDeshacer', N'¿Eliminar al usuario ''{0}''? Esta acción no se puede deshacer.', N'Delete user ''{0}''? This action cannot be undone.', N'Excluir o usuário ''{0}''? Esta ação não pode ser desfeita.'),
    ('msg_EliminarElIdiomaSeBorraranTodasSusTraducciones', N'¿Eliminar el idioma ''{0}''? Se borrarán todas sus traducciones.', N'Delete language ''{0}''? All its translations will be deleted.', N'Excluir o idioma ''{0}''? Todas as suas traduções serão apagadas.'),
    ('msg_EliminarElRolYTodosSusSubroles', N'¿Eliminar el rol ''{0}'' y todos sus sub-roles?', N'Delete role ''{0}'' and all its sub-roles?', N'Excluir o perfil ''{0}'' e todos os seus subperfis?'),
    ('msg_EliminarLaFranja', N'¿Eliminar la franja ''{0} {1:hh\:mm}-{2:hh\:mm} ({3})''?', N'Delete time slot ''{0} {1:hh\:mm}-{2:hh\:mm} ({3})''?', N'Excluir a faixa ''{0} {1:hh\:mm}-{2:hh\:mm} ({3})''?'),
    ('msg_EquipoMarcadoEnMantenimiento', N'Equipo marcado como ''En mantenimiento''.', N'Equipment marked as ''Under maintenance''.', N'Equipamento marcado como ''Em manutenção''.'),
    ('msg_EquipoSinUmbralConfigurado', N'El equipo ''{0}'' no tiene un nivel de uso crítico configurado.' + CHAR(10) + N'Se notificará al Administrador para que lo defina.', N'Equipment ''{0}'' has no critical usage level configured.' + CHAR(10) + N'The Administrator will be notified to set it.', N'O equipamento ''{0}'' não tem um nível de uso crítico configurado.' + CHAR(10) + N'O Administrador será notificado para defini-lo.'),
    ('msg_ErrorAlCambiarLaContrasena', N'Error al cambiar la contraseña.', N'Error changing the password.', N'Erro ao alterar a senha.'),
    ('msg_EstadoRestauradoCorrectamente', N'Estado restaurado correctamente.', N'State restored successfully.', N'Estado restaurado com sucesso.'),
    ('msg_EsteTurnoNoTieneUnEmpleadoAsignadoParaReemplazar', N'Este turno no tiene un empleado asignado para reemplazar.', N'This shift has no assigned employee to replace.', N'Este turno não tem um funcionário atribuído para substituir.'),
    ('msg_EsteTurnoYaNoEstaAsignadoPuedeQueYaLoHayasCanceladoAntes', N'Este turno ya no está asignado (puede que ya lo hayas cancelado antes).', N'This shift is no longer assigned (you may have already cancelled it).', N'Este turno não está mais atribuído (talvez você já o tenha cancelado).'),
    ('msg_EvaluacionDeCoberturaRegistradaSeUsaraParaAjustarLaProximaPl', N'Evaluación de cobertura registrada. Se usará para ajustar la próxima planificación.', N'Coverage evaluation recorded. It will be used to adjust the next schedule.', N'Avaliação de cobertura registrada. Será usada para ajustar o próximo planejamento.'),
    ('msg_ExportacionCompletada', N'Exportación completada.', N'Export completed.', N'Exportação concluída.'),
    ('msg_FranjaAgregada', N'Franja agregada.', N'Time slot added.', N'Faixa adicionada.'),
    ('msg_FranjaEliminada', N'Franja eliminada.', N'Time slot deleted.', N'Faixa excluída.'),
    ('msg_GenerarLaGrillaParaLaSemanaDel', N'¿Generar la grilla para la semana del {0:dd/MM}?', N'Generate the schedule for the week of {0:dd/MM}?', N'Gerar a escala para a semana de {0:dd/MM}?'),
    ('msg_GrillaConfirmadaCorrectamente', N'Grilla confirmada correctamente.', N'Schedule confirmed successfully.', N'Escala confirmada com sucesso.'),
    ('msg_HorariosComunicadosAlEquipo', N'Horarios comunicados al equipo.', N'Schedule communicated to the team.', N'Horários comunicados à equipe.'),
    ('msg_InformeDeMantenimientoRegistradoSeNotificoAlAdministrador', N'Informe de mantenimiento registrado. Se notificó al Administrador.', N'Maintenance report recorded. The Administrator was notified.', N'Relatório de manutenção registrado. O Administrador foi notificado.'),
    ('msg_IngresaElResultadoDeLaRevision', N'Ingresá el resultado de la revisión.', N'Enter the inspection result.', N'Informe o resultado da revisão.'),
    ('msg_IngresaUnNombreDeUsuario', N'Ingresá un nombre de usuario.', N'Enter a username.', N'Informe um nome de usuário.'),
    ('msg_IngresaUnaCantidadValidaDeSocios', N'Ingresá una cantidad válida de socios.', N'Enter a valid number of members.', N'Informe uma quantidade válida de sócios.'),
    ('msg_IngresaUnaContrasena', N'Ingresá una contraseña.', N'Enter a password.', N'Informe uma senha.'),
    ('msg_IngresoRegistradoALas', N'Ingreso registrado a las {0:HH:mm}.', N'Clock-in recorded at {0:HH:mm}.', N'Entrada registrada às {0:HH:mm}.'),
    ('msg_LaContrasenaActualEsIncorrecta', N'La contraseña actual es incorrecta.', N'The current password is incorrect.', N'A senha atual está incorreta.'),
    ('msg_LaGrillaDeEsaSemanaTodaviaNoEstaConfirmada', N'La grilla de esa semana todavía no está confirmada.', N'That week''s schedule is not confirmed yet.', N'A escala dessa semana ainda não está confirmada.'),
    ('msg_LaGrillaYaFueConfirmadaNoSePuedeModificar', N'La grilla ya fue confirmada, no se puede modificar.', N'The schedule has already been confirmed and cannot be modified.', N'A escala já foi confirmada e não pode ser modificada.'),
    ('msg_LasContrasenasNoCoinciden', N'Las contraseñas no coinciden.', N'Passwords do not match.', N'As senhas não coincidem.'),
    ('msg_ListoElTurnoQuedoAsignadoAVos', N'¡Listo! El turno quedó asignado a vos.', N'Done! The shift is now assigned to you.', N'Pronto! O turno foi atribuído a você.'),
    ('msg_LosPermisosDelCatalogoNoSePuedenEliminarDesasignaloUsandoLos', N'Los permisos del catálogo no se pueden eliminar.' + CHAR(10) + N'Desasignalo usando los checkboxes.', N'Catalog permissions cannot be deleted.' + CHAR(10) + N'Unassign it using the checkboxes.', N'As permissões do catálogo não podem ser excluídas.' + CHAR(10) + N'Desatribua-a usando as caixas de seleção.'),
    ('msg_MarcarEstaFranjaComoDeficitDeCobertura', N'¿Marcar esta franja como déficit de cobertura?', N'Mark this time slot as a coverage shortfall?', N'Marcar esta faixa como déficit de cobertura?'),
    ('msg_NoHayDatosParaExportar', N'No hay datos para exportar.', N'There is no data to export.', N'Não há dados para exportar.'),
    ('msg_NoHayNingunaGrillaGeneradaParaEsaSemana', N'No hay ninguna grilla generada para esa semana.', N'No schedule has been generated for that week.', N'Nenhuma escala foi gerada para essa semana.'),
    ('msg_NoHayTecnicosAlternativosRegistradosEnElSistema', N'No hay técnicos alternativos registrados en el sistema.', N'There are no alternate technicians registered in the system.', N'Não há técnicos alternativos cadastrados no sistema.'),
    ('msg_NoHayUnaGrillaConfirmadaParaEsaSemanaONoTieneTurnosAsignados', N'No hay una grilla confirmada para esa semana, o no tiene turnos asignados.', N'There is no confirmed schedule for that week, or it has no assigned shifts.', N'Não há escala confirmada para essa semana, ou ela não tem turnos atribuídos.'),
    ('msg_NoPodesEliminarTuPropioUsuario', N'No podés eliminar tu propio usuario.', N'You cannot delete your own user.', N'Você não pode excluir seu próprio usuário.'),
    ('msg_NoSePudoExportar', N'No se pudo exportar: {0}', N'Could not export: {0}', N'Não foi possível exportar: {0}'),
    ('msg_NoSePudoIniciarLaApi', N'No se pudo iniciar la API: {0}', N'Could not start the API: {0}', N'Não foi possível iniciar a API: {0}'),
    ('msg_NoSePuedeEliminarElIdiomaPredeterminadoDelSistema', N'No se puede eliminar el idioma predeterminado del sistema.', N'The system''s default language cannot be deleted.', N'O idioma padrão do sistema não pode ser excluído.'),
    ('msg_NoSePuedeEliminarUnIdiomaQueEstaEnUsoPorAlgunUsuario', N'No se puede eliminar un idioma que está en uso por algún usuario.', N'A language that is in use by a user cannot be deleted.', N'Um idioma em uso por algum usuário não pode ser excluído.'),
    ('msg_NoSePuedeEliminarYaTieneDisponibilidadTurnosOHistorialAsocia', N'No se puede eliminar: ya tiene disponibilidad, turnos, o historial asociado.', N'Cannot delete: it already has associated availability, shifts or history.', N'Não é possível excluir: já possui disponibilidade, turnos ou histórico associados.'),
    ('msg_NoSePuedeRestaurarUnaVersionQueYaEsUnRollback', N'No se puede restaurar una versión que ya es un rollback.', N'A version that is already a rollback cannot be restored.', N'Não é possível restaurar uma versão que já é um rollback.'),
    ('msg_NoTenesPermisoParaAccederAEstaFuncion', N'No tenés permiso para acceder a esta función.', N'You do not have permission to access this feature.', N'Você não tem permissão para acessar esta função.'),
    ('msg_NoTenesUnIngresoRegistradoHoyRegistraElIngresoPrimero', N'No tenés un ingreso registrado hoy. Registrá el ingreso primero.', N'You have no clock-in recorded today. Record the clock-in first.', N'Você não tem uma entrada registrada hoje. Registre a entrada primeiro.'),
    ('msg_RegistroDeUsoGuardadoCorrectamente', N'Registro de uso guardado correctamente.', N'Usage record saved successfully.', N'Registro de uso salvo com sucesso.'),
    ('msg_SeleccionaDiaYRol', N'Seleccioná día y rol.', N'Select day and role.', N'Selecione dia e função.'),
    ('msg_SeleccionaUnEmpleado', N'Seleccioná un empleado.', N'Select an employee.', N'Selecione um funcionário.'),
    ('msg_SeleccionaUnEquipo', N'Seleccioná un equipo.', N'Select a piece of equipment.', N'Selecione um equipamento.'),
    ('msg_SeleccionaUnIdioma', N'Seleccioná un idioma.', N'Select a language.', N'Selecione um idioma.'),
    ('msg_SeleccionaUnInforme', N'Seleccioná un informe.', N'Select a report.', N'Selecione um relatório.'),
    ('msg_SeleccionaUnRol', N'Seleccioná un rol.', N'Select a role.', N'Selecione um perfil.'),
    ('msg_SeleccionaUnTurnoDeLaLista', N'Seleccioná un turno de la lista.', N'Select a shift from the list.', N'Selecione um turno da lista.'),
    ('msg_SeleccionaUnUsuario', N'Seleccioná un usuario.', N'Select a user.', N'Selecione um usuário.'),
    ('msg_SeleccionaUnaAlerta', N'Seleccioná una alerta.', N'Select an alert.', N'Selecione um alerta.'),
    ('msg_SeleccionaUnaFranja', N'Seleccioná una franja.', N'Select a time slot.', N'Selecione uma faixa.'),
    ('msg_SeleccionaUnaVisita', N'Seleccioná una visita.', N'Select a visit.', N'Selecione uma visita.'),
    ('msg_SinCompatiblesTurnoMarcadoSinCobertura', N'No hay ningún empleado disponible compatible con el rol y la franja.' + CHAR(10) + N'El turno queda marcado como sin cobertura.', N'No available employee matches the role and time slot.' + CHAR(10) + N'The shift is marked as uncovered.', N'Não há nenhum funcionário disponível compatível com a função e a faixa.' + CHAR(10) + N'O turno fica marcado como sem cobertura.'),
    ('msg_SinCompatiblesTurnoSigueSinCobertura', N'No hay ningún empleado disponible compatible con el rol y la franja.' + CHAR(10) + N'El turno sigue sin cobertura.', N'No available employee matches the role and time slot.' + CHAR(10) + N'The shift remains uncovered.', N'Não há nenhum funcionário disponível compatível com a função e a faixa.' + CHAR(10) + N'O turno continua sem cobertura.'),
    ('msg_SolicitudDeVisitaConfirmada', N'Solicitud de visita confirmada.', N'Visit request confirmed.', N'Solicitação de visita confirmada.'),
    ('msg_SuperariaSuLimiteDeHorasSemanalesConEsteTurnoElegiOtroEmplea', N'{0} superaría su límite de horas semanales con este turno. Elegí otro empleado.', N'{0} would exceed their weekly hour limit with this shift. Choose another employee.', N'{0} ultrapassaria o limite de horas semanais com este turno. Escolha outro funcionário.'),
    ('msg_SuperariaSuLimiteDeHorasSemanalesElegiOtroEmpleado', N'{0} superaría su límite de horas semanales. Elegí otro empleado.', N'{0} would exceed their weekly hour limit. Choose another employee.', N'{0} ultrapassaria o limite de horas semanais. Escolha outro funcionário.'),
    ('msg_TenesQueIndicarAlMenosUnaFranja', N'Tenés que indicar al menos una franja.', N'You must specify at least one time slot.', N'Você deve indicar pelo menos uma faixa.'),
    ('msg_TodaviaQuedanFranjasSinAsignarNiMarcarComoDeficitDeCobertura', N'Todavía quedan franjas sin asignar ni marcar como déficit de cobertura.', N'There are still time slots not assigned or marked as a coverage shortfall.', N'Ainda há faixas sem atribuir nem marcar como déficit de cobertura.'),
    ('msg_TraduccionesGuardadas', N'Traducciones guardadas.', N'Translations saved.', N'Traduções salvas.'),
    ('msg_TurnoCanceladoConNotificados', N'Turno cancelado. Se notificó al administrador y a {0} empleado(s) disponible(s).', N'Shift cancelled. The administrator and {0} available employee(s) were notified.', N'Turno cancelado. O administrador e {0} funcionário(s) disponível(is) foram notificados.'),
    ('msg_TurnoCanceladoSinNotificados', N'Turno cancelado. Se notificó al administrador (no se encontraron empleados disponibles para cubrirlo).', N'Shift cancelled. The administrator was notified (no available employees were found to cover it).', N'Turno cancelado. O administrador foi notificado (não foram encontrados funcionários disponíveis para cobri-lo).'),
    ('msg_TurnoCubiertoYEmpleadoNotificado', N'Turno cubierto y empleado notificado.', N'Shift covered and employee notified.', N'Turno coberto e funcionário notificado.'),
    ('msg_TurnoReasignadoYAmbosEmpleadosNotificados', N'Turno reasignado y ambos empleados notificados.', N'Shift reassigned and both employees notified.', N'Turno reatribuído e ambos os funcionários notificados.'),
    ('msg_TurnoYaCubierto', N'Este turno ya fue cubierto por otra persona (o superarías tu límite de horas).' + CHAR(10) + N'Se actualizó la lista.', N'This shift was already covered by someone else (or you would exceed your hour limit).' + CHAR(10) + N'The list was updated.', N'Este turno já foi coberto por outra pessoa (ou você ultrapassaria seu limite de horas).' + CHAR(10) + N'A lista foi atualizada.'),
    ('msg_UsuarioBloqueadoPorIntentosFallidosContactateConUnAdministra', N'Usuario bloqueado por intentos fallidos. Contactate con un administrador.', N'User locked due to failed attempts. Contact an administrator.', N'Usuário bloqueado por tentativas malsucedidas. Entre em contato com um administrador.'),
    ('msg_UsuarioCreadoCorrectamente', N'Usuario creado correctamente.', N'User created successfully.', N'Usuário criado com sucesso.'),
    ('msg_UsuarioDesbloqueado', N'Usuario desbloqueado.', N'User unlocked.', N'Usuário desbloqueado.'),
    ('msg_UsuarioOContrasenaIncorrectos', N'Usuario o contraseña incorrectos.', N'Incorrect username or password.', N'Usuário ou senha incorretos.'),
    ('msg_UsuarioSinHistorial', N'El usuario ''{0}'' no tiene historial de cambios registrado.' + CHAR(10) + N'' + CHAR(10) + N'No es posible restaurarlo desde historial. Use ''Recalcular y continuar''.', N'User ''{0}'' has no recorded change history.' + CHAR(10) + N'' + CHAR(10) + N'It cannot be restored from history. Use ''Recalculate and continue''.', N'O usuário ''{0}'' não tem histórico de alterações registrado.' + CHAR(10) + N'' + CHAR(10) + N'Não é possível restaurá-lo a partir do histórico. Use ''Recalcular e continuar''.'),
    ('msg_VisitaAutorizada', N'Visita autorizada.', N'Visit authorized.', N'Visita autorizada.'),
    ('msg_YaTenesUnIngresoRegistradoHoySinEgreso', N'Ya tenés un ingreso registrado hoy sin egreso.', N'You already have a clock-in today without a clock-out.', N'Você já tem uma entrada registrada hoje sem saída.'),
    ('tit_AccesoDenegado', N'Acceso denegado', N'Access denied', N'Acesso negado'),
    ('tit_Atencion', N'Atención', N'Warning', N'Atenção'),
    ('tit_Aviso', N'Aviso', N'Notice', N'Aviso'),
    ('tit_Confirmar', N'Confirmar', N'Confirm', N'Confirmar'),
    ('tit_ConfirmarCancelacion', N'Confirmar cancelación', N'Confirm cancellation', N'Confirmar cancelamento'),
    ('tit_ConfirmarEliminacion', N'Confirmar eliminación', N'Confirm deletion', N'Confirmar exclusão'),
    ('tit_ConfirmarRollback', N'Confirmar rollback', N'Confirm rollback', N'Confirmar rollback'),
    ('tit_ContrasenaInvalida', N'Contraseña inválida', N'Invalid password', N'Senha inválida'),
    ('tit_Error', N'Error', N'Error', N'Erro'),
    ('tit_ErrorDelSistema', N'Error del sistema', N'System error', N'Erro do sistema'),
    ('tit_Exito', N'Éxito', N'Success', N'Sucesso'),
    ('tit_LimiteDeHorasSuperado', N'Límite de horas superado', N'Hour limit exceeded', N'Limite de horas excedido'),
    ('tit_Listo', N'Listo', N'Done', N'Pronto'),
    ('tit_LoginExitoso', N'Login exitoso', N'Login successful', N'Login bem-sucedido'),
    ('tit_OperacionNoPermitida', N'Operación no permitida', N'Operation not allowed', N'Operação não permitida'),
    ('tit_SinCobertura', N'Sin cobertura', N'No coverage', N'Sem cobertura'),
    ('tit_SinHistorial', N'Sin historial', N'No history', N'Sem histórico'),
    ('tit_SinUmbralConfigurado', N'Sin umbral configurado', N'No threshold configured', N'Sem limite configurado'),
    ('tit_YaNoDisponible', N'Ya no disponible', N'No longer available', N'Não está mais disponível');

INSERT INTO CONTROL_IDIOMA (CLAVE, TEXTO_DEFAULT)
SELECT t.CLAVE, t.ES FROM @textos t
WHERE NOT EXISTS (SELECT 1 FROM CONTROL_IDIOMA c WHERE c.CLAVE = t.CLAVE);

DECLARE @ingId INT = (SELECT ID FROM IDIOMA WHERE NOMBRE = 'Inglés');
DECLARE @ptId  INT = (SELECT ID FROM IDIOMA WHERE NOMBRE = 'Portugues');

INSERT INTO TRADUCCION (IDIOMA_ID, CONTROL_ID, TEXTO)
SELECT @ingId, c.ID, t.EN
FROM @textos t JOIN CONTROL_IDIOMA c ON c.CLAVE = t.CLAVE
WHERE @ingId IS NOT NULL
  AND NOT EXISTS (SELECT 1 FROM TRADUCCION tr WHERE tr.IDIOMA_ID = @ingId AND tr.CONTROL_ID = c.ID);

INSERT INTO TRADUCCION (IDIOMA_ID, CONTROL_ID, TEXTO)
SELECT @ptId, c.ID, t.PT
FROM @textos t JOIN CONTROL_IDIOMA c ON c.CLAVE = t.CLAVE
WHERE @ptId IS NOT NULL
  AND NOT EXISTS (SELECT 1 FROM TRADUCCION tr WHERE tr.IDIOMA_ID = @ptId AND tr.CONTROL_ID = c.ID);
GO
