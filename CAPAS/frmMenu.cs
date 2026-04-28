using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;

namespace CAPAS
{
    public partial class frmMenu : Form
    {
        public frmMenu()
        {
            InitializeComponent();
        }

        private void FrmPersonasToolStripMenuItem_Click(object sender, EventArgs e)
        {
            Form1 FrmPersonas = new Form1();
            FrmPersonas.ShowDialog();
        }

        private void cambiarContraseñaToolStripMenuItem_Click(object sender, EventArgs e)
        {
            frmContraseña frmContraseña1 = new frmContraseña();
            frmContraseña1.ShowDialog();
        }

        private void cerrarSesionToolStripMenuItem_Click(object sender, EventArgs e)
        {
            //ACA DEBERIA CERRAR LA SESION Y VOLVER AL LOGIN
             this.Close();
        }
    }
}
