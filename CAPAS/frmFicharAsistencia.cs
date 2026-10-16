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
            AjustarTamanioEstado();
            ActualizarEstado();

            InicializarFormulario();
        }

        protected override void AjustarClaves()
        {
            ExcluirDeTraduccion(lblEstado);
        }

        private void AjustarTamanioEstado()
        {
            lblEstado.AutoSize = false;
            lblEstado.Size = new System.Drawing.Size(340, 48);
        }

        private void ActualizarEstado()
        {
            BE.USUARIO usuario = SeguridadYServicios.SessionManager.getInstance().getUsuario();
            BE.EstadoFichaje estado = _asistenciaBLL.ObtenerEstado(usuario);

            if (estado.Abierto == null)
            {
                lblEstado.Text = Textos.T("lbl_SinIngresoAbierto", "No tenés ningún ingreso abierto.");
                btnIngreso.Enabled = true;
                btnEgreso.Enabled = false;
            }
            else if (estado.Vencido)
            {
                lblEstado.Text = Textos.T("lbl_IngresoVencido",
                    "Tenés un ingreso del {0:dd/MM} a las {1:hh\\:mm} sin egreso. Pedile al administrador que lo corrija.",
                    estado.Abierto.Fecha, estado.Abierto.HoraIngreso);
                btnIngreso.Enabled = false;
                btnEgreso.Enabled = false;
            }
            else
            {
                lblEstado.Text = Textos.T("lbl_IngresoAbierto", "Tenés un ingreso registrado el {0:dd/MM} a las {1:hh\\:mm}, sin egreso.",
                    estado.Abierto.Fecha, estado.Abierto.HoraIngreso);
                btnIngreso.Enabled = false;
                btnEgreso.Enabled = true;
            }
        }

        private void btnIngreso_Click(object sender, EventArgs e)
        {
            BE.USUARIO usuario = SeguridadYServicios.SessionManager.getInstance().getUsuario();
            BE.ResultadoFichaje resultado = _asistenciaBLL.RegistrarIngreso(usuario);

            switch (resultado)
            {
                case BE.ResultadoFichaje.Ok:
                    MsgBox.Show(Textos.T("msg_IngresoRegistradoALas", "Ingreso registrado a las {0:HH:mm}.", DateTime.Now), "Éxito",
                        MsgBox.Botones.OK, MsgBox.Icono.Exito);
                    break;
                case BE.ResultadoFichaje.IngresoVencido:
                    MsgBox.Show(Textos.T("msg_IngresoAnteriorSinEgreso", "Tenés un ingreso anterior sin egreso. Pedile al administrador que lo corrija antes de fichar de nuevo."),
                        "Atención", MsgBox.Botones.OK, MsgBox.Icono.Atencion);
                    break;
                default:
                    MsgBox.Show(Textos.T("msg_YaTenesUnIngresoRegistradoSinEgreso", "Ya tenés un ingreso registrado sin egreso."), "Atención",
                        MsgBox.Botones.OK, MsgBox.Icono.Atencion);
                    break;
            }
            ActualizarEstado();
        }

        private void btnEgreso_Click(object sender, EventArgs e)
        {
            BE.USUARIO usuario = SeguridadYServicios.SessionManager.getInstance().getUsuario();
            BE.ResultadoFichaje resultado = _asistenciaBLL.RegistrarEgreso(usuario);

            switch (resultado)
            {
                case BE.ResultadoFichaje.Ok:
                    MsgBox.Show(Textos.T("msg_EgresoRegistradoALas", "Egreso registrado a las {0:HH:mm}.", DateTime.Now), "Éxito",
                        MsgBox.Botones.OK, MsgBox.Icono.Exito);
                    break;
                case BE.ResultadoFichaje.IngresoVencido:
                    MsgBox.Show(Textos.T("msg_IngresoAnteriorSinEgreso", "Tenés un ingreso anterior sin egreso. Pedile al administrador que lo corrija antes de fichar de nuevo."),
                        "Atención", MsgBox.Botones.OK, MsgBox.Icono.Atencion);
                    break;
                default:
                    MsgBox.Show(Textos.T("msg_NoTenesUnIngresoRegistradoRegistraElIngresoPrimero", "No tenés un ingreso registrado. Registrá el ingreso primero."),
                        "Atención", MsgBox.Botones.OK, MsgBox.Icono.Atencion);
                    break;
            }
            ActualizarEstado();
        }

        private void btnCerrar_Click(object sender, EventArgs e)
        {
            this.Close();
        }
    }
}
