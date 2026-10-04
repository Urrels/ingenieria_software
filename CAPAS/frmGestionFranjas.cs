using System;
using System.Windows.Forms;

namespace CAPAS
{
    public partial class frmGestionFranjas : FormBase
    {
        private readonly BLL.FranjaHorariaBLL _franjaBLL = new BLL.FranjaHorariaBLL();


        private static readonly string[] Dias =
            { "Monday", "Tuesday", "Wednesday", "Thursday", "Friday", "Saturday", "Sunday" };
        private static readonly string[] Roles = { "Sala", "Recepcion" };

        public frmGestionFranjas()
        {
            InitializeComponent();
        }

        private void frmGestionFranjas_Load(object sender, EventArgs e)
        {
            cboDia.DataSource = Dias;
            cboRol.DataSource = Roles;
            dtpHoraInicio.Value = DateTime.Today.AddHours(8);
            dtpHoraFin.Value = DateTime.Today.AddHours(10);

            CargarFranjas();

            InicializarFormulario();
        }

        private void CargarFranjas()
        {
            dgvFranjas.DataSource = null;
            dgvFranjas.DataSource = _franjaBLL.ListarTodas();

            if (dgvFranjas.Columns.Count > 0)
            {
                dgvFranjas.Columns["Dia"].HeaderText = "Día";
                dgvFranjas.Columns["HoraInicio"].HeaderText = "Hora inicio";
                dgvFranjas.Columns["HoraFin"].HeaderText = "Hora fin";
                dgvFranjas.Columns["RolRequerido"].HeaderText = "Rol";
            }
        }

        private void btnAgregar_Click(object sender, EventArgs e)
        {
            string dia = cboDia.SelectedItem as string;
            string rol = cboRol.SelectedItem as string;
            TimeSpan horaInicio = dtpHoraInicio.Value.TimeOfDay;
            TimeSpan horaFin = dtpHoraFin.Value.TimeOfDay;

            if (dia == null || rol == null)
            {
                MsgBox.Show(Textos.T("msg_SeleccionaDiaYRol", "Seleccioná día y rol."), "Atención", MsgBox.Botones.OK, MsgBox.Icono.Atencion);
                return;
            }

            try
            {
                _franjaBLL.Insertar(dia, horaInicio, horaFin, rol);
                MsgBox.Show(Textos.T("msg_FranjaAgregada", "Franja agregada."), "Éxito", MsgBox.Botones.OK, MsgBox.Icono.Exito);
                CargarFranjas();
            }
            catch (ArgumentException ex)
            {
                MsgBox.Show(ex.Message, "Atención", MsgBox.Botones.OK, MsgBox.Icono.Atencion);
            }
        }

        private void btnEliminar_Click(object sender, EventArgs e)
        {
            if (!(dgvFranjas.CurrentRow?.DataBoundItem is BE.FranjaHoraria franja))
            {
                MsgBox.Show(Textos.T("msg_SeleccionaUnaFranja", "Seleccioná una franja."), "Atención", MsgBox.Botones.OK, MsgBox.Icono.Atencion);
                return;
            }

            if (MsgBox.Show(Textos.T("msg_EliminarLaFranja", "¿Eliminar la franja '{0} {1:hh\\:mm}-{2:hh\\:mm} ({3})'?", franja.Dia, franja.HoraInicio, franja.HoraFin, franja.RolRequerido),
                "Confirmar", MsgBox.Botones.SiNo, MsgBox.Icono.Pregunta) != DialogResult.Yes)
                return;

            bool ok = _franjaBLL.Eliminar(franja.Id);
            if (!ok)
            {
                MsgBox.Show(Textos.T("msg_NoSePuedeEliminarYaTieneDisponibilidadTurnosOHistorialAsocia", "No se puede eliminar: ya tiene disponibilidad, turnos, o historial asociado."),
                    "Atención", MsgBox.Botones.OK, MsgBox.Icono.Atencion);
                return;
            }

            MsgBox.Show(Textos.T("msg_FranjaEliminada", "Franja eliminada."), "Éxito", MsgBox.Botones.OK, MsgBox.Icono.Exito);
            CargarFranjas();
        }

        private void btnExportar_Click(object sender, EventArgs e)
        {
            ExcelExportHelper.ExportarDataGridView(dgvFranjas, "Franjas");
        }

        private void btnCerrar_Click(object sender, EventArgs e)
        {
            this.Close();
        }
    }
}