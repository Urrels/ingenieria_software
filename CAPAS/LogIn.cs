using System;
using System.Windows.Forms;

namespace CAPAS
{
    public partial class LogIn : FormBase
    {

        public LogIn()
        {
            InitializeComponent();
        }

        private void LogIn_Load(object sender, EventArgs e)
        {
            InicializarFormulario();
        }

        private void btnIngresar_Click(object sender, EventArgs e)
        {
            if (string.IsNullOrWhiteSpace(txtUsuario.Text) ||
                string.IsNullOrWhiteSpace(txtContrasena.Text))
            {
                MsgBox.Show(Textos.T("msg_CompletaUsuarioYContrasena", "Completá usuario y contraseña."), "Atención",
                    MsgBox.Botones.OK, MsgBox.Icono.Atencion);
                return;
            }

            BLL.LoginBLL bll = new BLL.LoginBLL();
            BE.LoginResultado resultado = bll.AutenticarUsuario(
                txtUsuario.Text.Trim(), txtContrasena.Text.Trim());

            switch (resultado)
            {
                case BE.LoginResultado.Exito:
                    BE.USUARIO usuarioActual = SeguridadYServicios.SessionManager.getInstance().getUsuario();

                    if (Program.ResultadoIntegridad != null && !Program.ResultadoIntegridad.EsValido)
                    {
                        bool esAdmin = SeguridadYServicios.SessionManager.getInstance().TienePermiso("Administrar usuarios");
                        bool suDataEstaIntegra = !Program.ResultadoIntegridad.IdsUsuariosAfectados.Contains(usuarioActual.Id);

                        if (esAdmin && suDataEstaIntegra)
                        {
                            var frm = new frmRestaurarIntegridad(Program.ResultadoIntegridad);
                            if (frm.ShowDialog() != DialogResult.OK)
                            {
                                SeguridadYServicios.SessionManager.getInstance().cerrarSesion();
                                return;
                            }
                        }
                        else
                        {
                            MsgBox.Show(
                                Textos.T("msg_ElSistemaNoPuedeIniciarse",
                                    "El sistema no puede iniciarse debido a un problema interno.\n\nComuníquese con el administrador del sistema."),
                                "Error del sistema",
                                MsgBox.Botones.OK, MsgBox.Icono.Error);
                            SeguridadYServicios.SessionManager.getInstance().cerrarSesion();
                            return;
                        }
                    }
                    else
                    {
                        new BLL.IntegridadBLL().RecalcularIntegridadUsuarios();
                    }

                    MsgBox.Show(Textos.T("msg_Bienvenido", "Bienvenido, {0}!", usuarioActual.Usuario),
                        "Login exitoso", MsgBox.Botones.OK, MsgBox.Icono.Exito);
                    new frmMenu().Show();
                    this.Hide();
                    break;

                case BE.LoginResultado.UsuarioBloqueado:
                    if (Program.ResultadoIntegridad == null || Program.ResultadoIntegridad.EsValido)
                        new BLL.IntegridadBLL().RecalcularIntegridadUsuarios();
                    MsgBox.Show(
                        Textos.T("msg_UsuarioBloqueadoPorIntentosFallidosContactateConUnAdministra", "Usuario bloqueado por intentos fallidos. Contactate con un administrador."),
                        "Acceso denegado", MsgBox.Botones.OK, MsgBox.Icono.Error);
                    break;

                default:
                    if (Program.ResultadoIntegridad == null || Program.ResultadoIntegridad.EsValido)
                        new BLL.IntegridadBLL().RecalcularIntegridadUsuarios();
                    MsgBox.Show(Textos.T("msg_UsuarioOContrasenaIncorrectos", "Usuario o contraseña incorrectos."), "Error",
                        MsgBox.Botones.OK, MsgBox.Icono.Error);
                    break;
            }
        }
    }
}
