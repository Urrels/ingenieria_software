using System;
using System.Collections.Generic;
using System.Windows.Forms;
using ReaLTaiizor.Forms;
using ReaLTaiizor.Manager;

namespace CAPAS
{
    public partial class frmEvaluarAutorizarVisita : MaterialForm, SeguridadYServicios.IObservadorIdioma
    {
        private readonly BLL.AlertaRevisionBLL _alertaBLL = new BLL.AlertaRevisionBLL();

        private readonly Dictionary<string, Control> _controles = new Dictionary<string, Control>();
        private readonly Dictionary<string, string> _defaults = new Dictionary<string, string>();

        private List<BE.AlertaRevision> _alertas;

        public frmEvaluarAutorizarVisita()
        {
            InitializeComponent();
        }

        private void frmEvaluarAutorizarVisita_Load(object sender, EventArgs e)
        {
            CargarAlertas();

            GuardarDefaults(this.Controls);
            _controles[this.Name] = this;
            _defaults[this.Name] = this.Text;
            SeguridadYServicios.IdiomaManager.getInstance().Registrar(this);
            ActualizarIdioma();
            IdiomaUIHelper.AgregarSelector(this);
            MaterialSkinManager.Instance.AddFormToManage(this);
            AppTheme.AplicarTema(this);
        }

        private void CargarAlertas()
        {
            _alertas = _alertaBLL.ListarPendientes();
            dgvAlertas.DataSource = null;
            dgvAlertas.DataSource = _alertas;

            if (dgvAlertas.Columns.Count > 0)
            {
                if (dgvAlertas.Columns["EquipoId"] != null) dgvAlertas.Columns["EquipoId"].Visible = false;
                if (dgvAlertas.Columns["EquipoNombre"] != null) dgvAlertas.Columns["EquipoNombre"].HeaderText = "Equipo";
                if (dgvAlertas.Columns["FechaGeneracion"] != null) dgvAlertas.Columns["FechaGeneracion"].HeaderText = "Fecha";
                if (dgvAlertas.Columns["Estado"] != null) dgvAlertas.Columns["Estado"].HeaderText = "Estado";
            }
        }

        private void frmEvaluarAutorizarVisita_FormClosed(object sender, FormClosedEventArgs e)
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

        private BE.AlertaRevision ObtenerSeleccionada()
        {
            if (dgvAlertas.CurrentRow?.DataBoundItem is BE.AlertaRevision alerta) return alerta;
            MsgBox.Show("Seleccioná una alerta.", "Atención", MsgBox.Botones.OK, MsgBox.Icono.Atencion);
            return null;
        }

        private void btnAutorizar_Click(object sender, EventArgs e)
        {
            var alerta = ObtenerSeleccionada();
            if (alerta == null) return;

            var equipo = _alertaBLL.ObtenerEquipoDeAlerta(alerta);
            if (MsgBox.Show($"¿Autorizar la visita técnica para '{equipo.Nombre}'?", "Confirmar",
                MsgBox.Botones.SiNo, MsgBox.Icono.Pregunta) != DialogResult.Yes)
                return;

            _alertaBLL.AutorizarVisita(alerta);
            MsgBox.Show("Visita autorizada.", "Éxito", MsgBox.Botones.OK, MsgBox.Icono.Exito);
            CargarAlertas();
        }

        private void btnDescartar_Click(object sender, EventArgs e)
        {
            var alerta = ObtenerSeleccionada();
            if (alerta == null) return;

            if (MsgBox.Show("¿Descartar esta alerta como falso positivo?", "Confirmar",
                MsgBox.Botones.SiNo, MsgBox.Icono.Pregunta) != DialogResult.Yes)
                return;

            _alertaBLL.DescartarAlerta(alerta);
            MsgBox.Show("Alerta descartada.", "Éxito", MsgBox.Botones.OK, MsgBox.Icono.Exito);
            CargarAlertas();
        }

        private void btnExportar_Click(object sender, EventArgs e)
        {
            ExcelExportHelper.ExportarDataGridView(dgvAlertas, "AlertasRevision");
        }

        private void btnCerrar_Click(object sender, EventArgs e)
        {
            this.Close();
        }
    }
}