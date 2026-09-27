using BE;
using System;
using System.Collections.Generic;
using System.Windows.Forms;
using ReaLTaiizor.Forms;
using ReaLTaiizor.Manager;

namespace CAPAS
{
    public partial class frmMenu : MaterialForm, SeguridadYServicios.IObservadorIdioma
    {
        private readonly Dictionary<string, ToolStripItem> _menuItems = new Dictionary<string, ToolStripItem>();
        private readonly Dictionary<string, string> _menuDefaults = new Dictionary<string, string>();
        private bool _cargandoIdiomas;

        public frmMenu()
        {
            InitializeComponent();
        }

        private void frmMenu_Load(object sender, EventArgs e)
        {
            BE.USUARIO u = SeguridadYServicios.SessionManager.getInstance().getUsuario();
            this.Text = "Menu — " + u.Usuario;

            ActualizarVisibilidadMenu();

            GuardarMenuDefaults(menuStrip1.Items);
            CargarIdiomas();
            SeguridadYServicios.IdiomaManager.getInstance().Registrar(this);
            MaterialSkinManager.Instance.AddFormToManage(this);
            AppTheme.AplicarTema(this);
        }

        public void ActualizarVisibilidadMenu()
        {
            var sm = SeguridadYServicios.SessionManager.getInstance();
            bool puedeAdminUsuarios = sm.TienePermiso("Administrar usuarios");
            bool puedeGestionRoles = sm.TienePermiso("Gestión de roles");
            bool puedeGestionIdiomas = sm.TienePermiso("Gestión de idiomas");

            usuariosBloqueadosToolStripMenuItem.Visible = puedeAdminUsuarios;
            perfilesToolStripMenuItem.Visible = puedeGestionRoles;
            idiomasToolStripMenuItem.Visible = puedeGestionIdiomas;
            administracionToolStripMenuItem.Visible = puedeAdminUsuarios
                                                   || puedeGestionRoles
                                                   || puedeGestionIdiomas;
            bitacoraToolStripMenuItem.Visible = sm.TienePermiso("Ver bitácora");
        }

        private void frmMenu_FormClosed(object sender, FormClosedEventArgs e)
        {
            SeguridadYServicios.IdiomaManager.getInstance().Desregistrar(this);
        }

        public void ActualizarIdioma()
        {
            foreach (var kvp in _menuItems)
            {
                string texto = SeguridadYServicios.IdiomaManager.getInstance().Traducir(kvp.Key)
                               ?? _menuDefaults[kvp.Key];
                kvp.Value.Text = texto;
            }
        }

        private void GuardarMenuDefaults(ToolStripItemCollection items)
        {
            foreach (ToolStripItem item in items)
            {
                if (!string.IsNullOrEmpty(item.Name) && !string.IsNullOrEmpty(item.Text))
                {
                    _menuItems[item.Name] = item;
                    _menuDefaults[item.Name] = item.Text;
                }
                if (item is ToolStripMenuItem mi && mi.HasDropDownItems)
                    GuardarMenuDefaults(mi.DropDownItems);
            }
        }

        private void CargarIdiomas()
        {
            _cargandoIdiomas = true;
            cboIdiomaStatus.Items.Clear();
            foreach (IDIOMA idioma in new BLL.IdiomaBLL().ListarHabilitados())
                cboIdiomaStatus.Items.Add(idioma);
            cboIdiomaStatus.SelectedIndex = -1;
            _cargandoIdiomas = false;
        }

        private void cboIdiomaStatus_SelectedIndexChanged(object sender, EventArgs e)
        {
            if (_cargandoIdiomas) return;
            if (!(cboIdiomaStatus.SelectedItem is IDIOMA idioma)) return;

            var bll = new BLL.IdiomaBLL();

            foreach (var kvp in _menuDefaults)
                bll.RegistrarControl(kvp.Key, "[" + kvp.Value + "]");

            var traducciones = bll.CargarTraducciones(idioma.Id);
            SeguridadYServicios.IdiomaManager.getInstance().CambiarIdioma(idioma, traducciones);

            BE.USUARIO usuario = SeguridadYServicios.SessionManager.getInstance().getUsuario();
            if (usuario != null)
                new BLL.UsuarioBLL().ActualizarIdioma(usuario.Id, idioma.Id);
        }

        // ===== Usuario =====
        private void misNotificacionesToolStripMenuItem_Click(object sender, EventArgs e)
        {
            new frmMisNotificaciones().ShowDialog();
        }

        private void cerrarSesionToolStripMenuItem_Click(object sender, EventArgs e)
        {
            string usuario = SeguridadYServicios.SessionManager.getInstance().getUsuario().Usuario;
            new BLL.BitacoraBLL().RegistrarLogout(usuario);
            SeguridadYServicios.SessionManager.getInstance().cerrarSesion();

            new LogIn().Show();
            this.Close();
        }

        // ===== Horarios (Proceso 1) =====
        private void disponibilidadToolStripMenuItem_Click(object sender, EventArgs e)
        {
            new frmRegistrarDisponibilidad().ShowDialog();
        }

        private void generarGrillaToolStripMenuItem_Click(object sender, EventArgs e)
        {
            if (!SeguridadYServicios.SessionManager.getInstance().TienePermiso("Generar grilla de turnos"))
            {
                MsgBox.Show("No tenés permiso para acceder a esta función.", "Atención",
                    MsgBox.Botones.OK, MsgBox.Icono.Atencion);
                return;
            }
            new frmGenerarGrilla().ShowDialog();
        }

        private void ajustarPorAusenciaToolStripMenuItem_Click(object sender, EventArgs e)
        {
            new frmAjustarPorAusencia().ShowDialog();
        }

        private void gestionFranjasToolStripMenuItem_Click(object sender, EventArgs e)
        {
            new frmGestionFranjas().ShowDialog();
        }

        // ===== Mantenimiento (Proceso 2) =====
        private void registrarUsoToolStripMenuItem_Click(object sender, EventArgs e)
        {
            new frmRegistrarUsoMaquina().ShowDialog();
        }

        private void evaluarAutorizarToolStripMenuItem_Click(object sender, EventArgs e)
        {
            new frmEvaluarAutorizarVisita().ShowDialog();
        }

        private void coordinarVisitaToolStripMenuItem_Click(object sender, EventArgs e)
        {
            new frmCoordinarVisita().ShowDialog();
        }

        private void realizarRevisionToolStripMenuItem_Click(object sender, EventArgs e)
        {
            new frmRealizarRevision().ShowDialog();
        }

        private void confirmarCierreToolStripMenuItem_Click(object sender, EventArgs e)
        {
            new frmConfirmarCierreMantenimiento().ShowDialog();
        }

        // ===== Asistencia (Proceso 3) =====
        private void ficharAsistenciaToolStripMenuItem_Click(object sender, EventArgs e)
        {
            new frmFicharAsistencia().ShowDialog();
        }

        private void registrarAsistenciaSociosToolStripMenuItem_Click(object sender, EventArgs e)
        {
            new frmRegistrarAsistenciaSocios().ShowDialog();
        }

        private void evaluarCoberturaToolStripMenuItem_Click(object sender, EventArgs e)
        {
            new frmEvaluarCobertura().ShowDialog();
        }

        // ===== Configuración =====
        private void cambiarContraseñaToolStripMenuItem_Click(object sender, EventArgs e)
        {
            new frmContraseña().ShowDialog();
        }

        // ===== Bitácora =====
        private void bitacoraToolStripMenuItem_Click(object sender, EventArgs e)
        {
            new frmBitacora().ShowDialog();
        }

        // ===== Administración =====
        private void usuariosBloqueadosToolStripMenuItem_Click(object sender, EventArgs e)
        {
            new frmAdminUsuarios().ShowDialog();
            ActualizarVisibilidadMenu();
        }

        private void perfilesToolStripMenuItem_Click(object sender, EventArgs e)
        {
            new frmPerfiles().ShowDialog();
        }

        private void idiomasToolStripMenuItem_Click(object sender, EventArgs e)
        {
            new frmIdiomas().ShowDialog();
            CargarIdiomas();
        }
    }
}