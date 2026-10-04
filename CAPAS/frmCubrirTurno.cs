using System;
using System.Collections.Generic;
using System.Linq;
using System.Windows.Forms;

namespace CAPAS
{
    public partial class frmCubrirTurno : FormBase
    {
        private readonly BLL.TurnoBLL _turnoBLL = new BLL.TurnoBLL();
        private readonly BLL.FranjaHorariaBLL _franjaBLL = new BLL.FranjaHorariaBLL();


        private List<TurnoPendienteVM> _filas;

        public frmCubrirTurno()
        {
            InitializeComponent();
        }

        private void frmCubrirTurno_Load(object sender, EventArgs e)
        {
            CargarFilas();

            InicializarFormulario();
        }

        private void CargarFilas()
        {
            BE.USUARIO usuario = SeguridadYServicios.SessionManager.getInstance().getUsuario();
            var franjasPorId = _franjaBLL.ListarTodas().ToDictionary(f => f.Id);

            List<BE.Turno> turnos = _turnoBLL.ListarPendientesPorRol(usuario.RolNombre);

            _filas = turnos.Select(t => new TurnoPendienteVM(t, franjasPorId[t.FranjaId])).ToList();

            dgvTurnos.DataSource = null;
            dgvTurnos.DataSource = _filas;

            if (dgvTurnos.Columns.Count > 0)
            {
                if (dgvTurnos.Columns["Turno"] != null) dgvTurnos.Columns["Turno"].Visible = false;
                if (dgvTurnos.Columns["Franja"] != null) dgvTurnos.Columns["Franja"].Visible = false;
                ActualizarEncabezados();
            }
        }

        private void btnTomarTurno_Click(object sender, EventArgs e)
        {
            if (dgvTurnos.CurrentRow == null)
            {
                MsgBox.Show(Textos.T("msg_SeleccionaUnTurnoDeLaLista", "Seleccioná un turno de la lista."), "Atención",
                    MsgBox.Botones.OK, MsgBox.Icono.Atencion);
                return;
            }

            var fila = (TurnoPendienteVM)dgvTurnos.CurrentRow.DataBoundItem;
            BE.USUARIO usuario = SeguridadYServicios.SessionManager.getInstance().getUsuario();

            bool gano = _turnoBLL.TomarCobertura(fila.Turno, usuario);

            if (!gano)
            {
                MsgBox.Show(Textos.T("msg_TurnoYaCubierto",
                    "Este turno ya fue cubierto por otra persona (o superarías tu límite de horas).\nSe actualizó la lista."), "Ya no disponible",
                    MsgBox.Botones.OK, MsgBox.Icono.Atencion);
                CargarFilas();
                return;
            }

            MsgBox.Show(Textos.T("msg_ListoElTurnoQuedoAsignadoAVos", "¡Listo! El turno quedó asignado a vos."), "Éxito",
                MsgBox.Botones.OK, MsgBox.Icono.Exito);
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
        }

        protected override void ActualizarTextosDinamicos()
        {
            ActualizarEncabezados();
        }
    }

    public class TurnoPendienteVM
    {
        public BE.Turno Turno { get; }
        public BE.FranjaHoraria FranjaObj { get; }

        public string Dia => FranjaObj.Dia;
        public string Horario => $"{FranjaObj.HoraInicio:hh\\:mm} - {FranjaObj.HoraFin:hh\\:mm}";
        public string Rol => Turno.RolRequerido;

        public TurnoPendienteVM(BE.Turno turno, BE.FranjaHoraria franja)
        {
            Turno = turno;
            FranjaObj = franja;
        }
    }
}