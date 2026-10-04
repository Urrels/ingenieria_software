using System.Collections.Generic;
using System.Windows.Forms;

namespace CAPAS
{
    public partial class frmSeleccionarReemplazo : FormBase
    {
        private readonly List<BE.USUARIO> _candidatos;
        public BE.USUARIO EmpleadoSeleccionado { get; private set; }

        public frmSeleccionarReemplazo(List<BE.USUARIO> candidatos)
        {
            InitializeComponent();
            _candidatos = candidatos;
        }

        private void frmSeleccionarReemplazo_Load(object sender, System.EventArgs e)
        {
            lstEmpleados.DataSource = _candidatos;
            lstEmpleados.DisplayMember = "Usuario";
            lstEmpleados.ValueMember = "Id";
            InicializarFormulario();
        }

        private void btnAsignar_Click(object sender, System.EventArgs e)
        {
            if (!(lstEmpleados.SelectedItem is BE.USUARIO seleccionado))
            {
                MsgBox.Show(Textos.T("msg_SeleccionaUnEmpleado", "Seleccioná un empleado."), "Atención", MsgBox.Botones.OK, MsgBox.Icono.Atencion);
                return;
            }
            EmpleadoSeleccionado = seleccionado;
            this.DialogResult = DialogResult.OK;
            this.Close();
        }

        private void btnCancelar_Click(object sender, System.EventArgs e)
        {
            this.DialogResult = DialogResult.Cancel;
            this.Close();
        }
    }
}