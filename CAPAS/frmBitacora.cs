using System;
using System.Collections.Generic;
using System.Linq;
using System.Windows.Forms;

namespace CAPAS
{
    public partial class frmBitacora : Form
    {
        private const string OPCION_TODOS = "(Todos)";
        private List<BE.BITACORA> _todas = new List<BE.BITACORA>();

        public frmBitacora()
        {
            InitializeComponent();
        }

        private void frmBitacora_Load(object sender, EventArgs e)
        {
            CargarBitacora();
            PoblarFiltros();
            Refrescar();
        }

        private void CargarBitacora()
        {
            BLL.BitacoraBLL bll = new BLL.BitacoraBLL();
            _todas = bll.Listar();
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
                dataGridView1.Columns["Id"].HeaderText = "ID";
                dataGridView1.Columns["Usuario"].HeaderText = "Usuario";
                dataGridView1.Columns["Accion"].HeaderText = "Acción";
                dataGridView1.Columns["Fecha"].HeaderText = "Fecha y Hora";

                dataGridView1.Columns["Id"].Width = 50;
                dataGridView1.Columns["Usuario"].Width = 150;
                dataGridView1.Columns["Accion"].Width = 200;
                dataGridView1.Columns["Fecha"].Width = 200;
            }
        }

        private void btnFiltrar_Click(object sender, EventArgs e)
        {
            Refrescar();
        }

        private void btnLimpiar_Click(object sender, EventArgs e)
        {
            cmbUsuario.SelectedIndex = 0;
            cmbAccion.SelectedIndex = 0;
            dtpDesde.Checked = false;
            dtpHasta.Checked = false;
            Refrescar();
        }
    }
}
