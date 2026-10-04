using System;
using System.Collections.Generic;
using System.Linq;
using System.Windows.Forms;

namespace CAPAS
{
    public partial class frmMisTurnos : FormBase
    {
        private readonly BLL.TurnoBLL _turnoBLL = new BLL.TurnoBLL();
        private readonly BLL.FranjaHorariaBLL _franjaBLL = new BLL.FranjaHorariaBLL();


        private List<TurnoEmpleadoVM> _filas;

        public frmMisTurnos()
        {
            InitializeComponent();
        }

        private void frmMisTurnos_Load(object sender, EventArgs e)
        {
            CargarFilas();

            InicializarFormulario();
        }

        private void CargarFilas()
        {
            BE.USUARIO usuario = SeguridadYServicios.SessionManager.getInstance().getUsuario();
            var franjasPorId = _franjaBLL.ListarTodas().ToDictionary(f => f.Id);

            List<BE.Turno> turnos = _turnoBLL.ListarPorUsuario(usuario.Id);

            _filas = turnos.Select(t => new TurnoEmpleadoVM(t, franjasPorId[t.FranjaId])).ToList();

            dgvTurnos.DataSource = null;
            dgvTurnos.DataSource = _filas;

            if (dgvTurnos.Columns.Count > 0)
            {
                if (dgvTurnos.Columns["Turno"] != null) dgvTurnos.Columns["Turno"].Visible = false;
                if (dgvTurnos.Columns["Franja"] != null) dgvTurnos.Columns["Franja"].Visible = false;
                ActualizarEncabezados();
            }
        }

        private void btnCancelarTurno_Click(object sender, EventArgs e)
        {
            if (dgvTurnos.CurrentRow == null)
            {
                MsgBox.Show(Textos.T("msg_SeleccionaUnTurnoDeLaLista", "Seleccioná un turno de la lista."), "Atención",
                    MsgBox.Botones.OK, MsgBox.Icono.Atencion);
                return;
            }

            var fila = (TurnoEmpleadoVM)dgvTurnos.CurrentRow.DataBoundItem;

            if (fila.Turno.Estado != "Asignado")
            {
                MsgBox.Show(Textos.T("msg_EsteTurnoYaNoEstaAsignadoPuedeQueYaLoHayasCanceladoAntes", "Este turno ya no está asignado (puede que ya lo hayas cancelado antes)."), "Atención",
                    MsgBox.Botones.OK, MsgBox.Icono.Atencion);
                return;
            }

            if (MsgBox.Show(Textos.T("msg_ConfirmarCancelarTurno",
                "¿Seguro que querés cancelar el turno del {0} {1}?\nSe va a notificar automáticamente al administrador y a los empleados disponibles para cubrirlo.",
                fila.Dia, fila.Horario),
                "Confirmar cancelación", MsgBox.Botones.SiNo, MsgBox.Icono.Pregunta) != DialogResult.Yes)
                return;

            BE.USUARIO usuario = SeguridadYServicios.SessionManager.getInstance().getUsuario();
            List<BE.USUARIO> notificados = _turnoBLL.CancelarPorEmpleado(fila.Turno, usuario);

            MsgBox.Show(notificados.Count > 0
                    ? Textos.T("msg_TurnoCanceladoConNotificados", "Turno cancelado. Se notificó al administrador y a {0} empleado(s) disponible(s).", notificados.Count)
                    : Textos.T("msg_TurnoCanceladoSinNotificados", "Turno cancelado. Se notificó al administrador (no se encontraron empleados disponibles para cubrirlo)."),
                "Listo", MsgBox.Botones.OK, MsgBox.Icono.Exito);

            CargarFilas();
        }

        private void btnCerrar_Click(object sender, EventArgs e)
        {
            this.Close();
        }

        private void ActualizarEncabezados()
        {
            Encabezado(dgvTurnos, "Dia", "hdr_Dia", "Día");
            Encabezado(dgvTurnos, "Horario", "hdr_Horario", "Horario");
            Encabezado(dgvTurnos, "Rol", "hdr_Rol", "Rol");
            Encabezado(dgvTurnos, "Estado", "hdr_Estado", "Estado");
        }

        protected override void ActualizarTextosDinamicos()
        {
            ActualizarEncabezados();
        }
    }

    public class TurnoEmpleadoVM
    {
        public BE.Turno Turno { get; }
        public BE.FranjaHoraria FranjaObj { get; }

        public string Dia => FranjaObj.Dia;
        public string Horario => $"{FranjaObj.HoraInicio:hh\\:mm} - {FranjaObj.HoraFin:hh\\:mm}";
        public string Rol => Turno.RolRequerido;
        public string Estado => Turno.Estado;

        public TurnoEmpleadoVM(BE.Turno turno, BE.FranjaHoraria franja)
        {
            Turno = turno;
            FranjaObj = franja;
        }
    }
}