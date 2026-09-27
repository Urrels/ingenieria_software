using System;
using System.Collections.Generic;
using System.Windows.Forms;
using ReaLTaiizor.Forms;
using ReaLTaiizor.Manager;

namespace CAPAS
{
    public partial class frmConfirmarCierreMantenimiento : MaterialForm, SeguridadYServicios.IObservadorIdioma
    {
        private readonly BLL.InformeMantenimientoBLL _informeBLL = new BLL.InformeMantenimientoBLL();
        private readonly BLL.EquipoBLL _equipoBLL = new BLL.EquipoBLL();

        private readonly Dictionary<string, Control> _controles = new Dictionary<string, Control>();
        private readonly Dictionary<string, string> _defaults = new Dictionary<string, string>();

        private List<BE.InformeMantenimiento> _informes;

        public frmConfirmarCierreMantenimiento()
        {
            InitializeComponent();
        }

        private void frmConfirmarCierreMantenimiento_Load(object sender, EventArgs e)
        {
            CargarInformes();

            GuardarDefaults(this.Controls);
            _controles[this.Name] = this;
            _defaults[this.Name] = this.Text;
            SeguridadYServicios.IdiomaManager.getInstance().Registrar(this);
            ActualizarIdioma();
            IdiomaUIHelper.AgregarSelector(this);
            MaterialSkinManager.Instance.AddFormToManage(this);
            AppTheme.AplicarTema(this);
        }

        private void CargarInformes()
        {
            _informes = _informeBLL.ListarPendientesCierre();
            dgvInformes.DataSource = null;
            dgvInformes.DataSource = _informes;

            if (dgvInformes.Columns.Count > 0)
            {
                if (dgvInformes.Columns["VisitaId"] != null) dgvInformes.Columns["VisitaId"].Visible = false;
                if (dgvInformes.Columns["AlertaId"] != null) dgvInformes.Columns["AlertaId"].Visible = false;
                if (dgvInformes.Columns["EquipoId"] != null) dgvInformes.Columns["EquipoId"].Visible = false;
                if (dgvInformes.Columns["Resultado"] != null) dgvInformes.Columns["Resultado"].HeaderText = "Resultado";
                if (dgvInformes.Columns["PendienteRepuesto"] != null) dgvInformes.Columns["PendienteRepuesto"].HeaderText = "¿Repuesto pendiente?";
                if (dgvInformes.Columns["FechaEmision"] != null) dgvInformes.Columns["FechaEmision"].HeaderText = "Fecha del informe";
            }
        }

        private void frmConfirmarCierreMantenimiento_FormClosed(object sender, FormClosedEventArgs e)
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

        private void dgvInformes_SelectionChanged(object sender, EventArgs e)
        {
            var informe = dgvInformes.CurrentRow?.DataBoundItem as BE.InformeMantenimiento;
            if (informe == null) { lblDetalle.Text = ""; return; }

            var equipo = _equipoBLL.ObtenerPorId(informe.EquipoId);
            lblDetalle.Text = equipo != null
                ? $"Equipo: {equipo.Nombre}  |  Estado actual: {equipo.Estado}  |  Uso acumulado: {equipo.UsoAcumulado}"
                : "";
        }

        private void btnConfirmarCierre_Click(object sender, EventArgs e)
        {
            if (!(dgvInformes.CurrentRow?.DataBoundItem is BE.InformeMantenimiento informe))
            {
                MsgBox.Show("Seleccioná un informe.", "Atención", MsgBox.Botones.OK, MsgBox.Icono.Atencion);
                return;
            }

            string mensajeConfirmacion = informe.PendienteRepuesto
                ? "Este informe indica que falta un repuesto. El equipo quedará marcado 'En mantenimiento' y NO se reiniciará el contador de uso. ¿Confirmar de todas formas?"
                : "¿Confirmar el cierre? Se reiniciará el contador de uso y el equipo quedará operativo.";

            if (MsgBox.Show(mensajeConfirmacion, "Confirmar", MsgBox.Botones.SiNo, MsgBox.Icono.Pregunta) != DialogResult.Yes)
                return;

            _informeBLL.ConfirmarCierre(informe);

            MsgBox.Show(
                informe.PendienteRepuesto
                    ? "Equipo marcado como 'En mantenimiento'."
                    : "Cierre confirmado. Equipo operativo con el contador reiniciado.",
                "Éxito", MsgBox.Botones.OK, MsgBox.Icono.Exito);

            CargarInformes();
        }

        private void btnCerrar_Click(object sender, EventArgs e)
        {
            this.Close();
        }
    }
}