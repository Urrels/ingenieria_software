using System;
using System.Collections.Generic;
using System.Linq;
using System.Windows.Forms;

namespace CAPAS
{
    public partial class frmAjustarPorAusencia : FormBase
    {
        private readonly BLL.GrillaBLL _grillaBLL = new BLL.GrillaBLL();
        private readonly BLL.TurnoBLL _turnoBLL = new BLL.TurnoBLL();
        private readonly BLL.NotificacionBLL _notificacionBLL = new BLL.NotificacionBLL();
        private readonly BLL.FranjaHorariaBLL _franjaBLL = new BLL.FranjaHorariaBLL();
        private readonly BLL.UsuarioBLL _usuarioBLL = new BLL.UsuarioBLL();


        private BE.GrillaDeTurnos _grilla;
        private List<TurnoFilaVM> _filas;

        public frmAjustarPorAusencia()
        {
            InitializeComponent();
        }

        private void frmAjustarPorAusencia_Load(object sender, EventArgs e)
        {
            dtpSemana.Value = DateTime.Today;

            InicializarFormulario();
        }

        private void btnBuscar_Click(object sender, EventArgs e)
        {
            DateTime fecha = dtpSemana.Value.Date;
            int diasDesdeLunes = ((int)fecha.DayOfWeek - (int)DayOfWeek.Monday + 7) % 7;
            DateTime lunes = fecha.AddDays(-diasDesdeLunes);

            _grilla = _grillaBLL.ObtenerConTurnosPorSemana(lunes);

            if (_grilla == null)
            {
                MsgBox.Show(Textos.T("msg_NoHayNingunaGrillaGeneradaParaEsaSemana", "No hay ninguna grilla generada para esa semana."), "Atención",
                    MsgBox.Botones.OK, MsgBox.Icono.Atencion);
                dgvTurnos.DataSource = null;
                return;
            }

            if (_grilla.Estado != "Confirmada" && _grilla.Estado != "Comunicada")
            {
                MsgBox.Show(Textos.T("msg_LaGrillaDeEsaSemanaTodaviaNoEstaConfirmada", "La grilla de esa semana todavía no está confirmada."), "Atención",
                    MsgBox.Botones.OK, MsgBox.Icono.Atencion);
                dgvTurnos.DataSource = null;
                return;
            }

            CargarFilas();
        }

        private void CargarFilas()
        {
            var franjasPorId = _franjaBLL.ListarTodas().ToDictionary(f => f.Id);

            _filas = _grilla.Turnos.Select(t =>
            {
                string empleado = "Sin asignar";
                if (t.UsuarioId.HasValue)
                {
                    var u = _usuarioBLL.ObtenerPorId(t.UsuarioId.Value);
                    empleado = u != null ? $"{u.Nombre} {u.Apellido}" : "Sin asignar";
                }
                return new TurnoFilaVM(t, franjasPorId[t.FranjaId], empleado);
            }).ToList();

            dgvTurnos.DataSource = null;
            dgvTurnos.DataSource = _filas;

            if (dgvTurnos.Columns.Count > 0)
            {
                if (dgvTurnos.Columns["Turno"] != null) dgvTurnos.Columns["Turno"].Visible = false;
                if (dgvTurnos.Columns["Franja"] != null) dgvTurnos.Columns["Franja"].Visible = false;
                if (dgvTurnos.Columns["FranjaDescripcion"] != null) dgvTurnos.Columns["FranjaDescripcion"].HeaderText = "Franja";
                if (dgvTurnos.Columns["Rol"] != null) dgvTurnos.Columns["Rol"].HeaderText = "Rol";
                if (dgvTurnos.Columns["Empleado"] != null) dgvTurnos.Columns["Empleado"].HeaderText = "Empleado";
                if (dgvTurnos.Columns["Estado"] != null) dgvTurnos.Columns["Estado"].HeaderText = "Estado";
            }
        }

        private void dgvTurnos_CellDoubleClick(object sender, DataGridViewCellEventArgs e)
        {
            if (e.RowIndex < 0) return;
            var fila = (TurnoFilaVM)dgvTurnos.Rows[e.RowIndex].DataBoundItem;

            if (fila.Turno.Estado == "PendienteCobertura")
            {
                AsignarReemplazoPendiente(fila);
                return;
            }

            if (!fila.Turno.UsuarioId.HasValue)
            {
                MsgBox.Show(Textos.T("msg_EsteTurnoNoTieneUnEmpleadoAsignadoParaReemplazar", "Este turno no tiene un empleado asignado para reemplazar."), "Atención",
                    MsgBox.Botones.OK, MsgBox.Icono.Atencion);
                return;
            }

            BE.USUARIO empleadoAusente = _usuarioBLL.ObtenerPorId(fila.Turno.UsuarioId.Value);

            if (MsgBox.Show(Textos.T("msg_AvisoUnaAusenciaParaEsteTurno", "¿{0} {1} avisó una ausencia para este turno?", empleadoAusente.Nombre, empleadoAusente.Apellido),
                "Confirmar", MsgBox.Botones.SiNo, MsgBox.Icono.Pregunta) != DialogResult.Yes)
                return;

            List<BE.USUARIO> compatibles = _turnoBLL.BuscarEmpleadosCompatibles(fila.Turno.RolRequerido, fila.Franja, _grilla.Id)
                .Where(u => u.Id != empleadoAusente.Id)
                .ToList();

            if (compatibles.Count == 0)
            {
                MsgBox.Show(Textos.T("msg_SinCompatiblesTurnoMarcadoSinCobertura",
                            "No hay ningún empleado disponible compatible con el rol y la franja.\nEl turno queda marcado como sin cobertura."), "Sin cobertura",
                    MsgBox.Botones.OK, MsgBox.Icono.Atencion);
                _turnoBLL.MarcarDeficitCobertura(fila.Turno);
                CargarFilas();
                return;
            }

            using (var frmSeleccion = new frmSeleccionarReemplazo(compatibles))
            {
                if (frmSeleccion.ShowDialog() != DialogResult.OK) return;

                BE.USUARIO nuevoEmpleado = frmSeleccion.EmpleadoSeleccionado;
                bool ok = _turnoBLL.ReemplazarPorAusencia(fila.Turno, nuevoEmpleado);

                if (!ok)
                {
                    MsgBox.Show(Textos.T("msg_SuperariaSuLimiteDeHorasSemanalesElegiOtroEmpleado", "{0} superaría su límite de horas semanales. Elegí otro empleado.", nuevoEmpleado.Usuario),
                        "Límite de horas superado", MsgBox.Botones.OK, MsgBox.Icono.Atencion);
                    return;
                }

                string mensajeReemplazante = $"Se te asignó un turno el {fila.Franja.Dia} " +
                    $"{fila.Franja.HoraInicio:hh\\:mm}-{fila.Franja.HoraFin:hh\\:mm} por reemplazo de ausencia.";
                _notificacionBLL.Notificar(nuevoEmpleado, _grilla.Id, mensajeReemplazante);

                string mensajeReemplazado = $"Tu turno del {fila.Franja.Dia} " +
                    $"{fila.Franja.HoraInicio:hh\\:mm}-{fila.Franja.HoraFin:hh\\:mm} fue reasignado por tu ausencia.";
                _notificacionBLL.Notificar(empleadoAusente, _grilla.Id, mensajeReemplazado);

                MsgBox.Show(Textos.T("msg_TurnoReasignadoYAmbosEmpleadosNotificados", "Turno reasignado y ambos empleados notificados."), "Éxito",
                    MsgBox.Botones.OK, MsgBox.Icono.Exito);
                CargarFilas();
            }
        }

        private void AsignarReemplazoPendiente(TurnoFilaVM fila)
        {
            List<BE.USUARIO> compatibles = _turnoBLL.BuscarEmpleadosCompatibles(fila.Turno.RolRequerido, fila.Franja, _grilla.Id);

            if (compatibles.Count == 0)
            {
                MsgBox.Show(Textos.T("msg_SinCompatiblesTurnoSigueSinCobertura",
                            "No hay ningún empleado disponible compatible con el rol y la franja.\nEl turno sigue sin cobertura."), "Sin cobertura",
                    MsgBox.Botones.OK, MsgBox.Icono.Atencion);
                return;
            }

            using (var frmSeleccion = new frmSeleccionarReemplazo(compatibles))
            {
                if (frmSeleccion.ShowDialog() != DialogResult.OK) return;

                BE.USUARIO nuevoEmpleado = frmSeleccion.EmpleadoSeleccionado;
                bool ok = _turnoBLL.ReemplazarPorAusencia(fila.Turno, nuevoEmpleado);

                if (!ok)
                {
                    MsgBox.Show(Textos.T("msg_SuperariaSuLimiteDeHorasSemanalesElegiOtroEmpleado", "{0} superaría su límite de horas semanales. Elegí otro empleado.", nuevoEmpleado.Usuario),
                        "Límite de horas superado", MsgBox.Botones.OK, MsgBox.Icono.Atencion);
                    return;
                }

                string mensaje = $"Se te asignó el turno del {fila.Franja.Dia} " +
                    $"{fila.Franja.HoraInicio:hh\\:mm}-{fila.Franja.HoraFin:hh\\:mm} que habías confirmado que podías cubrir.";
                _notificacionBLL.Notificar(nuevoEmpleado, _grilla.Id, mensaje);

                MsgBox.Show(Textos.T("msg_TurnoCubiertoYEmpleadoNotificado", "Turno cubierto y empleado notificado."), "Éxito",
                    MsgBox.Botones.OK, MsgBox.Icono.Exito);
                CargarFilas();
            }
        }

        private void btnExportar_Click(object sender, EventArgs e)
        {
            ExcelExportHelper.ExportarDataGridView(dgvTurnos, "AjusteAusencia");
        }

        private void btnCerrar_Click(object sender, EventArgs e)
        {
            this.Close();
        }
    }
}