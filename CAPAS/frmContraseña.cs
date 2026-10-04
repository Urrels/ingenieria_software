using System;
using System.Windows.Forms;

namespace CAPAS
{
    public partial class frmContraseña : FormBase
    {

        public frmContraseña()
        {
            InitializeComponent();
        }

        private void frmContraseña_Load(object sender, EventArgs e)
        {
            txtPassActual.PasswordChar = '*';
            txtNuevaPass.PasswordChar = '*';
            txtConfPass.PasswordChar = '*';

            InicializarFormulario();
        }

        private void btnContinuar_Click(object sender, EventArgs e)
        {
            string passActual = txtPassActual.Text.Trim();
            string nuevaPass = txtNuevaPass.Text.Trim();
            string confPass = txtConfPass.Text.Trim();

            if (string.IsNullOrEmpty(passActual) || string.IsNullOrEmpty(nuevaPass) || string.IsNullOrEmpty(confPass))
            {
                MsgBox.Show(Textos.T("msg_CompletaTodosLosCampos", "Completá todos los campos."), "Atención",
                    MsgBox.Botones.OK, MsgBox.Icono.Atencion);
                return;
            }

            if (nuevaPass != confPass)
            {
                MsgBox.Show(Textos.T("msg_LasContrasenasNoCoinciden", "Las contraseñas no coinciden."), "Error",
                    MsgBox.Botones.OK, MsgBox.Icono.Error);
                return;
            }

            string errorValidacion = SeguridadYServicios.ValidadorContrasena.ObtenerError(nuevaPass);
            if (errorValidacion != null)
            {
                MsgBox.Show(errorValidacion, "Contraseña inválida",
                    MsgBox.Botones.OK, MsgBox.Icono.Atencion);
                return;
            }

            string usuario = SeguridadYServicios.SessionManager.getInstance().getUsuario().Usuario;
            BLL.UsuarioBLL bll = new BLL.UsuarioBLL();

            if (!bll.VerificarContrasena(usuario, passActual))
            {
                MsgBox.Show(Textos.T("msg_LaContrasenaActualEsIncorrecta", "La contraseña actual es incorrecta."), "Error",
                    MsgBox.Botones.OK, MsgBox.Icono.Error);
                return;
            }

            if (bll.CambiarContrasena(usuario, nuevaPass))
            {
                MsgBox.Show(Textos.T("msg_ContrasenaCambiadaExitosamente", "Contraseña cambiada exitosamente."), "Éxito",
                    MsgBox.Botones.OK, MsgBox.Icono.Exito);
                this.Close();
            }
            else
            {
                MsgBox.Show(Textos.T("msg_ErrorAlCambiarLaContrasena", "Error al cambiar la contraseña."), "Error",
                    MsgBox.Botones.OK, MsgBox.Icono.Error);
            }
        }

        private void button1_Click(object sender, EventArgs e)
        {
            this.Close();
        }
    }
}
