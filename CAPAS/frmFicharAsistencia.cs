using System;
using System.Windows.Forms;

namespace CAPAS
{
    public partial class frmFicharAsistencia : FormBase
    {
        private readonly BLL.AsistenciaPersonalBLL _asistenciaBLL = new BLL.AsistenciaPersonalBLL();


        public frmFicharAsistencia()
        {
            InitializeComponent();
        }

        private void frmFicharAsistencia_Load(object sender, EventArgs e)
        {
            ActualizarEstado();

            InicializarFormulario();
        }

        private void ActualizarEstado()
        {
            BE.USUARIO usuario = SeguridadYServicios.SessionManager.getInstance().getUsuario();
            bool tieneAbierto = _asistenciaBLL.TieneIngresoAbierto(usuario);

            lblEstado.Text = tieneAbierto
                ? "Tenés un ingreso registrado hoy, sin egreso."
                : "No tenés ningún ingreso abierto hoy.";

            btnIngreso.Enabled = !tieneAbierto;
            btnEgreso.Enabled = tieneAbierto;
        }

        private void btnIngreso_Click(object sender, EventArgs e)
        {
            BE.USUARIO usuario = SeguridadYServicios.SessionManager.getInstance().getUsuario();
            bool ok = _asistenciaBLL.RegistrarIngreso(usuario);

            if (!ok)
            {
                MsgBox.Show(Textos.T("msg_YaTenesUnIngresoRegistradoHoySinEgreso", "Ya tenés un ingreso registrado hoy sin egreso."), "Atención",
                    MsgBox.Botones.OK, MsgBox.Icono.Atencion);
            }
            else
            {
                MsgBox.Show(Textos.T("msg_IngresoRegistradoALas", "Ingreso registrado a las {0:HH:mm}.", DateTime.Now), "Éxito",
                    MsgBox.Botones.OK, MsgBox.Icono.Exito);
            }
            ActualizarEstado();
        }

        private void btnEgreso_Click(object sender, EventArgs e)
        {
            BE.USUARIO usuario = SeguridadYServicios.SessionManager.getInstance().getUsuario();
            bool ok = _asistenciaBLL.RegistrarEgreso(usuario);

            if (!ok)
            {
                MsgBox.Show(Textos.T("msg_NoTenesUnIngresoRegistradoHoyRegistraElIngresoPrimero", "No tenés un ingreso registrado hoy. Registrá el ingreso primero."),
                    "Atención", MsgBox.Botones.OK, MsgBox.Icono.Atencion);
            }
            else
            {
                MsgBox.Show(Textos.T("msg_EgresoRegistradoALas", "Egreso registrado a las {0:HH:mm}.", DateTime.Now), "Éxito",
                    MsgBox.Botones.OK, MsgBox.Icono.Exito);
            }
            ActualizarEstado();
        }

        private void btnCerrar_Click(object sender, EventArgs e)
        {
            this.Close();
        }
    }
}