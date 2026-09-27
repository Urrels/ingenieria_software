using System;
using System.Collections.Generic;
using System.Windows.Forms;
using ReaLTaiizor.Forms;
using ReaLTaiizor.Manager;

namespace CAPAS
{
    public partial class frmGestionFranjas : MaterialForm, SeguridadYServicios.IObservadorIdioma
    {
        private readonly BLL.FranjaHorariaBLL _franjaBLL = new BLL.FranjaHorariaBLL();

        private readonly Dictionary<string, Control> _controles = new Dictionary<string, Control>();
        private readonly Dictionary<string, string> _defaults = new Dictionary<string, string>();

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

            GuardarDefaults(this.Controls);
            _controles[this.Name] = this;
            _defaults[this.Name] = this.Text;
            SeguridadYServicios.IdiomaManager.getInstance().Registrar(this);
            ActualizarIdioma();
            IdiomaUIHelper.AgregarSelector(this);
            MaterialSkinManager.Instance.AddFormToManage(this);
            AppTheme.AplicarTema(this);
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

        private void frmGestionFranjas_FormClosed(object sender, FormClosedEventArgs e)
        {
            SeguridadYServicios.IdiomaManager.getInstance().Desregistrar(this);
        }

        public void ActualizarIdioma()
        {
            foreach (var kvp in _controles)
            {
                string t = SeguridadYServicios.IdiomaManager.getInstance().Traducir(kvp.Key)
                           ?? _defaults[kvp.Key];
                kvp.Value.Text = t;
            }
        }

        private void GuardarDefaults(Control.ControlCollection controles)
        {
            foreach (Control c in controles)
            {
                if (!string.IsNullOrEmpty(c.Name) && !string.IsNullOrEmpty(c.Text))
                {
                    _controles[c.Name] = c;
                    _defaults[c.Name] = c.Text;
                }
                if (c.HasChildren) GuardarDefaults(c.Controls);
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
                MsgBox.Show("Seleccioná día y rol.", "Atención", MsgBox.Botones.OK, MsgBox.Icono.Atencion);
                return;
            }

            try
            {
                _franjaBLL.Insertar(dia, horaInicio, horaFin, rol);
                MsgBox.Show("Franja agregada.", "Éxito", MsgBox.Botones.OK, MsgBox.Icono.Exito);
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
                MsgBox.Show("Seleccioná una franja.", "Atención", MsgBox.Botones.OK, MsgBox.Icono.Atencion);
                return;
            }

            if (MsgBox.Show($"¿Eliminar la franja '{franja.Dia} {franja.HoraInicio:hh\\:mm}-{franja.HoraFin:hh\\:mm} ({franja.RolRequerido})'?",
                "Confirmar", MsgBox.Botones.SiNo, MsgBox.Icono.Pregunta) != DialogResult.Yes)
                return;

            bool ok = _franjaBLL.Eliminar(franja.Id);
            if (!ok)
            {
                MsgBox.Show("No se puede eliminar: ya tiene disponibilidad, turnos, o historial asociado.",
                    "Atención", MsgBox.Botones.OK, MsgBox.Icono.Atencion);
                return;
            }

            MsgBox.Show("Franja eliminada.", "Éxito", MsgBox.Botones.OK, MsgBox.Icono.Exito);
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