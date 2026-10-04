using System;
using System.Collections.Generic;
using System.Windows.Forms;

namespace CAPAS
{
    public partial class frmAsignarEmpleado : FormBase
    {
        private readonly BE.Turno _turno;
        private readonly BE.FranjaHoraria _franja;
        private readonly BLL.TurnoBLL _turnoBLL = new BLL.TurnoBLL();

        public BE.USUARIO EmpleadoAsignado { get; private set; }
        public bool MarcadoSinCobertura { get; private set; }

        public frmAsignarEmpleado(BE.Turno turno, BE.FranjaHoraria franja)
        {
            InitializeComponent();
            _turno = turno;
            _franja = franja;
        }

        private void frmAsignarEmpleado_Load(object sender, EventArgs e)
        {
            lblFranja.Text = $"{_franja.Dia}  {_franja.HoraInicio:hh\\:mm}-{_franja.HoraFin:hh\\:mm}  ({_turno.RolRequerido})";

            List<BE.USUARIO> compatibles = _turnoBLL.BuscarEmpleadosCompatibles(_turno.RolRequerido, _franja, _turno.GrillaId);
            if (compatibles.Count == 0)
            {
                lblAviso.Text = Textos.T("lbl_SinEmpleadosParaFranja", "No hay empleados disponibles para esta franja.");
                lstEmpleados.Enabled = false;
                btnAsignar.Enabled = false;
            }
            else
            {
                lstEmpleados.DataSource = compatibles;
                lstEmpleados.DisplayMember = "Usuario";
                lstEmpleados.ValueMember = "Id";
            }

            InicializarFormulario();
        }

        private void btnAsignar_Click(object sender, EventArgs e)
        {
            if (!(lstEmpleados.SelectedItem is BE.USUARIO seleccionado))
            {
                MsgBox.Show(Textos.T("msg_SeleccionaUnEmpleado", "Seleccioná un empleado."), "Atención", MsgBox.Botones.OK, MsgBox.Icono.Atencion);
                return;
            }

            bool asignado = _turnoBLL.AsignarEmpleado(_turno, seleccionado);
            if (!asignado)
            {
                MsgBox.Show(
                    Textos.T("msg_SuperariaSuLimiteDeHorasSemanalesConEsteTurnoElegiOtroEmplea", "{0} superaría su límite de horas semanales con este turno. Elegí otro empleado.", seleccionado.Usuario),
                    "Límite de horas superado", MsgBox.Botones.OK, MsgBox.Icono.Atencion);
                return;
            }

            EmpleadoAsignado = seleccionado;
            this.DialogResult = DialogResult.OK;
            this.Close();
        }

        private void btnSinCobertura_Click(object sender, EventArgs e)
        {
            if (MsgBox.Show(Textos.T("msg_MarcarEstaFranjaComoDeficitDeCobertura", "¿Marcar esta franja como déficit de cobertura?"), "Confirmar",
                MsgBox.Botones.SiNo, MsgBox.Icono.Pregunta) != DialogResult.Yes)
                return;

            _turnoBLL.MarcarDeficitCobertura(_turno);
            MarcadoSinCobertura = true;
            this.DialogResult = DialogResult.OK;
            this.Close();
        }

        private void btnCancelar_Click(object sender, EventArgs e)
        {
            this.DialogResult = DialogResult.Cancel;
            this.Close();
        }
    }
}