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
        private readonly BLL.FachadaIdioma _fachadaIdioma = new BLL.FachadaIdioma();

        public frmMenu()
        {
            InitializeComponent();
        }

        private void frmMenu_Load(object sender, EventArgs e)
        {
            ActualizarTitulo();

            ActualizarVisibilidadMenu();

            GuardarMenuDefaults(menuStrip1.Items);
            GuardarMenuDefaults(statusStrip1.Items);
            CargarIdiomas();
            SeguridadYServicios.IdiomaManager.getInstance().Registrar(this);
            MaterialSkinManager.Instance.AddFormToManage(this);
            AppTheme.AplicarTema(this);
        }

        public void ActualizarVisibilidadMenu()
        {
            var sm = SeguridadYServicios.SessionManager.getInstance();
            var permisoPorOpcion = new Dictionary<ToolStripItem, string>
            {
                { misTurnosToolStripMenuItem, "Gestionar mis turnos" },
                { cubrirTurnoToolStripMenuItem, "Gestionar mis turnos" },
                { disponibilidadToolStripMenuItem, "Gestionar mis turnos" },
                { generarGrillaToolStripMenuItem, "Generar grilla de turnos" },
                { ajustarPorAusenciaToolStripMenuItem, "Ajustar grilla de turnos" },
                { gestionFranjasToolStripMenuItem, "Gestionar franjas horarias" },
                { registrarUsoToolStripMenuItem, "Registrar uso de equipos" },
                { simulacionSensoresToolStripMenuItem, "Administrar mantenimiento" },
                { mantenimientoPredictivoToolStripMenuItem, "Administrar mantenimiento" },
                { evaluarAutorizarToolStripMenuItem, "Administrar mantenimiento" },
                { coordinarVisitaToolStripMenuItem, "Administrar mantenimiento" },
                { confirmarCierreToolStripMenuItem, "Administrar mantenimiento" },
                { realizarRevisionToolStripMenuItem, "Realizar revisión técnica" },
                { ficharAsistenciaToolStripMenuItem, "Fichar asistencia" },
                { registrarAsistenciaSociosToolStripMenuItem, "Registrar asistencia de socios" },
                { evaluarCoberturaToolStripMenuItem, "Evaluar cobertura" },
                { panelToolStripMenuItem, "Ver panel de indicadores" },
                { cambiarContraseñaToolStripMenuItem, "Cambiar contraseña" },
                { bitacoraToolStripMenuItem, "Ver bitácora" },
                { usuariosBloqueadosToolStripMenuItem, "Administrar usuarios" },
                { perfilesToolStripMenuItem, "Gestión de roles" },
                { idiomasToolStripMenuItem, "Gestión de idiomas" }
            };
            foreach (var par in permisoPorOpcion)
                par.Key.Visible = sm.TienePermiso(par.Value);

            foreach (ToolStripMenuItem grupo in new[] { horariosToolStripMenuItem, mantenimientoToolStripMenuItem,
                         asistenciaToolStripMenuItem, configuraciónToolStripMenuItem, administracionToolStripMenuItem })
            {
                bool algunaVisible = false;
                foreach (ToolStripItem opcion in grupo.DropDownItems)
                    if (opcion.Available) algunaVisible = true;
                grupo.Visible = algunaVisible;
            }
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
            ActualizarTitulo();
        }

        private void ActualizarTitulo()
        {
            BE.USUARIO u = SeguridadYServicios.SessionManager.getInstance().getUsuario();
            if (u != null)
                this.Text = Textos.T("frmMenu_Titulo", "Menú — {0}", u.Usuario);
        }

        private void GuardarMenuDefaults(ToolStripItemCollection items)
        {
            foreach (ToolStripItem item in items)
            {
                if (!string.IsNullOrEmpty(item.Name) && !string.IsNullOrEmpty(item.Text))
                {
                    _menuItems["frmMenu." + item.Name] = item;
                    _menuDefaults["frmMenu." + item.Name] = item.Text;
                }
                if (item is ToolStripMenuItem mi && mi.HasDropDownItems)
                    GuardarMenuDefaults(mi.DropDownItems);
            }
        }

        private void CargarIdiomas()
        {
            _cargandoIdiomas = true;
            cboIdiomaStatus.Items.Clear();
            foreach (IDIOMA idioma in _fachadaIdioma.ListarDisponibles())
                cboIdiomaStatus.Items.Add(idioma);
            cboIdiomaStatus.SelectedIndex = -1;
            _cargandoIdiomas = false;
        }

        private void cboIdiomaStatus_SelectedIndexChanged(object sender, EventArgs e)
        {
            if (_cargandoIdiomas) return;
            if (!(cboIdiomaStatus.SelectedItem is IDIOMA idioma)) return;

            foreach (var kvp in _menuDefaults)
                _fachadaIdioma.RegistrarClave(kvp.Key, kvp.Value);

            _fachadaIdioma.Cambiar(idioma);
        }

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

        private void misTurnosToolStripMenuItem_Click(object sender, EventArgs e)
        {
            new frmMisTurnos().ShowDialog();
        }

        private void cubrirTurnoToolStripMenuItem_Click(object sender, EventArgs e)
        {
            new frmCubrirTurno().ShowDialog();
        }

        private void disponibilidadToolStripMenuItem_Click(object sender, EventArgs e)
        {
            new frmRegistrarDisponibilidad().ShowDialog();
        }

        private void generarGrillaToolStripMenuItem_Click(object sender, EventArgs e)
        {
            if (!SeguridadYServicios.SessionManager.getInstance().TienePermiso("Generar grilla de turnos"))
            {
                MsgBox.Show(Textos.T("msg_NoTenesPermisoParaAccederAEstaFuncion", "No tenés permiso para acceder a esta función."), "Atención",
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

        private void registrarUsoToolStripMenuItem_Click(object sender, EventArgs e)
        {
            new frmRegistrarUsoMaquina().ShowDialog();
        }

        private void simulacionSensoresToolStripMenuItem_Click(object sender, EventArgs e)
        {
            new frmSimulacionSensores().ShowDialog();
        }

        private void mantenimientoPredictivoToolStripMenuItem_Click(object sender, EventArgs e)
        {
            new frmMantenimientoPredictivo().ShowDialog();
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
        private void panelToolStripMenuItem_Click(object sender, EventArgs e)
        {
            new frmDashboard().ShowDialog();
        }

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

        private void cambiarContraseñaToolStripMenuItem_Click(object sender, EventArgs e)
        {
            new frmContraseña().ShowDialog();
        }

        private void bitacoraToolStripMenuItem_Click(object sender, EventArgs e)
        {
            new frmBitacora().ShowDialog();
        }

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