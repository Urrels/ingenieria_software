using System;
using System.Windows.Forms;

namespace CAPAS
{
    public partial class frmBitacora : Form
    {
        public frmBitacora()
        {
            InitializeComponent();
        }

        private void frmBitacora_Load(object sender, EventArgs e)
        {
            CargarBitacora();
        }

        private void btnRefrescar_Click(object sender, EventArgs e)
        {
            CargarBitacora();
        }

        private void CargarBitacora()
        {
            BLL.BitacoraBLL bll = new BLL.BitacoraBLL();
            dataGridView1.DataSource = bll.Listar();

            // Renombrar columnas para que se vean bien
            if (dataGridView1.Columns.Count > 0)
            {
                dataGridView1.Columns["Id"].HeaderText = "ID";
                dataGridView1.Columns["Usuario"].HeaderText = "Usuario";
                dataGridView1.Columns["Accion"].HeaderText = "Acción";
                dataGridView1.Columns["Fecha"].HeaderText = "Fecha y Hora";

                // Ajustar ancho de columnas
                dataGridView1.Columns["Id"].Width = 50;
                dataGridView1.Columns["Usuario"].Width = 150;
                dataGridView1.Columns["Accion"].Width = 150;
                dataGridView1.Columns["Fecha"].Width = 200;
            }
        }
    }
}