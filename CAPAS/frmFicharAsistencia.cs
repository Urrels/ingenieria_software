using System;
using System.Collections.Generic;
using System.Windows.Forms;
using ReaLTaiizor.Forms;
using ReaLTaiizor.Manager;

namespace CAPAS
{
    public partial class frmFicharAsistencia : MaterialForm, SeguridadYServicios.IObservadorIdioma
    {
        private readonly BLL.AsistenciaPersonalBLL _asistenciaBLL = new BLL.AsistenciaPersonalBLL();

        private readonly Dictionary<string, Control> _controles = new Dictionary<string, Control>();
        private readonly Dictionary<string, string> _defaults = new Dictionary<string, string>();

        public frmFicharAsistencia()
        {
            InitializeComponent();
        }

        private void frmFicharAsistencia_Load(object sender, EventArgs e)
        {
            ActualizarEstado();

            GuardarDefaults(this.Controls);
            _controles[this.Name] = this;
            _defaults[this.Name] = this.Text;
            SeguridadYServicios.IdiomaManager.getInstance().Registrar(this);
            ActualizarIdioma();
            IdiomaUIHelper.AgregarSelector(this);
            MaterialSkinManager.Instance.AddFormToManage(this);
            AppTheme.AplicarTema(this);
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

        private void frmFicharAsistencia_FormClosed(object sender, FormClosedEventArgs e)
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
        }

        private void GuardarDefaults(Control.ControlCollection controles)
        {
            foreach (Control c in controles)
            {
                if (!string.IsNullOrEmpty(c.Name) && !string.IsNullOrEmpty(c.Text))
                {
                    _controles[c.Name] = c;
                    _defaults[c.Name] = c.Text;
                }
                if (c.HasChildren) GuardarDefaults(c.Controls);
            }
        }

        private void btnIngreso_Click(object sender, EventArgs e)
        {
            BE.USUARIO usuario = SeguridadYServicios.SessionManager.getInstance().getUsuario();
            bool ok = _asistenciaBLL.RegistrarIngreso(usuario);

            if (!ok)
            {
                MsgBox.Show("Ya tenés un ingreso registrado hoy sin egreso.", "Atención",
                    MsgBox.Botones.OK, MsgBox.Icono.Atencion);
            }
            else
            {
                MsgBox.Show($"Ingreso registrado a las {DateTime.Now:HH:mm}.", "Éxito",
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
                // flujo 1a
                MsgBox.Show("No tenés un ingreso registrado hoy. Registrá el ingreso primero.",
                    "Atención", MsgBox.Botones.OK, MsgBox.Icono.Atencion);
            }
            else
            {
                MsgBox.Show($"Egreso registrado a las {DateTime.Now:HH:mm}.", "Éxito",
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