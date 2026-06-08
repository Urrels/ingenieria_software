using System;
using System.Collections.Generic;
using System.Linq;
using System.Windows.Forms;
using ReaLTaiizor.Forms;
using ReaLTaiizor.Manager;

namespace CAPAS
{
    public partial class frmBitacora : MaterialForm, SeguridadYServicios.IObservadorIdioma
    {
        private const string OPCION_TODOS = "(Todos)";
        private List<BE.BITACORA> _todas = new List<BE.BITACORA>();
        private readonly Dictionary<string, Control> _controles = new Dictionary<string, Control>();
        private readonly Dictionary<string, string>  _defaults  = new Dictionary<string, string>();

        public frmBitacora()
        {
            InitializeComponent();
        }

        private void frmBitacora_Load(object sender, EventArgs e)
        {
            GuardarDefaults(this.Controls);
            _controles.Remove("lblTitulo");
            _defaults.Remove("lblTitulo");
            _controles["lblTitulo_Bitacora"] = lblTitulo;
            _defaults["lblTitulo_Bitacora"]  = lblTitulo.Text;
            _controles[this.Name] = this;
            _defaults[this.Name]  = this.Text;
            SeguridadYServicios.IdiomaManager.getInstance().Registrar(this);
            ActualizarIdioma();
            CargarBitacora();
            PoblarFiltros();
            Refrescar();
            IdiomaUIHelper.AgregarSelector(this);
            MaterialSkinManager.Instance.AddFormToManage(this);
            AppTheme.AplicarTema(this);
        }

        private void frmBitacora_FormClosed(object sender, FormClosedEventArgs e)
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
            ActualizarEncabezados();
        }

        private void ActualizarEncabezados()
        {
            if (dataGridView1.Columns.Count == 0) return;
            var mgr = SeguridadYServicios.IdiomaManager.getInstance();
            if (dataGridView1.Columns["Id"] != null)
                dataGridView1.Columns["Id"].HeaderText      = mgr.Traducir("colhdr_Id")      ?? "ID";
            if (dataGridView1.Columns["Usuario"] != null)
                dataGridView1.Columns["Usuario"].HeaderText = mgr.Traducir("colhdr_Usuario")  ?? "Usuario";
            if (dataGridView1.Columns["Accion"] != null)
                dataGridView1.Columns["Accion"].HeaderText  = mgr.Traducir("colhdr_Accion")   ?? "Acción";
            if (dataGridView1.Columns["Fecha"] != null)
                dataGridView1.Columns["Fecha"].HeaderText   = mgr.Traducir("colhdr_Fecha")    ?? "Fecha y Hora";
        }

        private void GuardarDefaults(Control.ControlCollection controles)
        {
            foreach (Control c in controles)
            {
                if (!string.IsNullOrEmpty(c.Name) && !string.IsNullOrEmpty(c.Text))
                {
                    _controles[c.Name] = c;
                    _defaults[c.Name]  = c.Text;
                }
                if (c.HasChildren) GuardarDefaults(c.Controls);
            }
        }

        private void CargarBitacora()
        {
            _todas = new BLL.BitacoraBLL().Listar();
        }

        private void PoblarFiltros()
        {
            cmbUsuario.Items.Clear();
            cmbUsuario.Items.Add(OPCION_TODOS);
            foreach (string u in _todas.Select(b => b.Usuario).Distinct().OrderBy(s => s))
                cmbUsuario.Items.Add(u);
            cmbUsuario.SelectedIndex = 0;

            cmbAccion.Items.Clear();
            cmbAccion.Items.Add(OPCION_TODOS);
            foreach (string a in _todas.Select(b => b.Accion).Distinct().OrderBy(s => s))
                cmbAccion.Items.Add(a);
            cmbAccion.SelectedIndex = 0;

            dtpDesde.Checked = false;
            dtpHasta.Checked = false;
        }

        private void Refrescar()
        {
            IEnumerable<BE.BITACORA> q = _todas;

            if (cmbUsuario.SelectedItem != null && (string)cmbUsuario.SelectedItem != OPCION_TODOS)
                q = q.Where(b => b.Usuario == (string)cmbUsuario.SelectedItem);

            if (cmbAccion.SelectedItem != null && (string)cmbAccion.SelectedItem != OPCION_TODOS)
                q = q.Where(b => b.Accion == (string)cmbAccion.SelectedItem);

            if (dtpDesde.Checked)
                q = q.Where(b => b.Fecha >= dtpDesde.Value.Date);

            if (dtpHasta.Checked)
                q = q.Where(b => b.Fecha < dtpHasta.Value.Date.AddDays(1));

            dataGridView1.DataSource = q.ToList();

            if (dataGridView1.Columns.Count > 0)
            {
                if (dataGridView1.Columns["Id"] != null)      dataGridView1.Columns["Id"].Width      = 50;
                if (dataGridView1.Columns["Usuario"] != null) dataGridView1.Columns["Usuario"].Width  = 150;
                if (dataGridView1.Columns["Accion"] != null)  dataGridView1.Columns["Accion"].Width   = 200;
                if (dataGridView1.Columns["Fecha"] != null)   dataGridView1.Columns["Fecha"].Width    = 200;
                ActualizarEncabezados();
            }
        }

        private void btnFiltrar_Click(object sender, EventArgs e) => Refrescar();

        private void btnLimpiar_Click(object sender, EventArgs e)
        {
            cmbUsuario.SelectedIndex = 0;
            cmbAccion.SelectedIndex  = 0;
            dtpDesde.Checked = false;
            dtpHasta.Checked = false;
            Refrescar();
        }
    }
}
