using System;
using System.Collections.Generic;
using System.Windows.Forms;

namespace CAPAS
{
    public partial class frmRestore : Form
    {
        private readonly BLL.BackupBLL _bll = new BLL.BackupBLL();

        public List<string> Problemas { get; set; } = new List<string>();
        public string ArchivoBackup { get; set; } = null;

        public frmRestore()
        {
            InitializeComponent();
        }

        private void frmRestore_Load(object sender, EventArgs e)
        {
            string ultimo = _bll.ObtenerUltimoBackup();
            lblUltimoBackup.Text = ultimo != null
                ? "Último backup: " + System.IO.Path.GetFileName(ultimo)
                : "No hay backups disponibles.";

            if (Problemas != null && Problemas.Count > 0)
                lblProblemas.Text = "Problemas detectados:\n" + string.Join("\n", Problemas);
            else
                lblProblemas.Text = "";
        }

        private void btnRestaurar_Click(object sender, EventArgs e)
        {
            if (!_bll.VerificarPassword(txtPassword.Text))
            {
                MessageBox.Show("Contraseña maestra incorrecta.", "Error",
                    MessageBoxButtons.OK, MessageBoxIcon.Error);
                return;
            }

            string archivo = _bll.ObtenerUltimoBackup();
            if (archivo == null)
            {
                MessageBox.Show("No hay backups disponibles para restaurar.", "Error",
                    MessageBoxButtons.OK, MessageBoxIcon.Error);
                return;
            }

            if (MessageBox.Show(
                $"¿Restaurar desde:\n{System.IO.Path.GetFileName(archivo)}?\n\n" +
                "Esto reconstruirá los datos faltantes sin borrar los existentes.",
                "Confirmar restauración",
                MessageBoxButtons.YesNo, MessageBoxIcon.Warning) != DialogResult.Yes)
                return;

            try
            {
                _bll.RestaurarDesdeArchivo(archivo);
                MessageBox.Show("Restauración completada.", "Éxito",
                    MessageBoxButtons.OK, MessageBoxIcon.Information);
                this.DialogResult = DialogResult.OK;
            }
            catch (Exception ex)
            {
                MessageBox.Show("Error al restaurar: " + ex.Message, "Error",
                    MessageBoxButtons.OK, MessageBoxIcon.Error);
            }
        }

        private void btnCancelar_Click(object sender, EventArgs e)
        {
            this.DialogResult = DialogResult.Cancel;
        }
    }
}