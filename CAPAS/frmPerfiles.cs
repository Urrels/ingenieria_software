using BE;
using System;
using System.Collections.Generic;
using System.Windows.Forms;

namespace CAPAS
{
    public partial class frmPerfiles : Form, SeguridadYServicios.IObservadorIdioma
    {
        private readonly BLL.PerfilBLL _bll = new BLL.PerfilBLL();
        private readonly Dictionary<string, Control> _controles = new Dictionary<string, Control>();
        private readonly Dictionary<string, string>  _defaults  = new Dictionary<string, string>();

        public frmPerfiles()
        {
            InitializeComponent();
        }

        private void frmPerfiles_Load(object sender, EventArgs e)
        {
            GuardarDefaults(this.Controls);
            _controles[this.Name] = this;
            _defaults[this.Name]  = this.Text;
            SeguridadYServicios.IdiomaManager.getInstance().Registrar(this);
            ActualizarIdioma();
            CargarArbol();
            IdiomaUIHelper.AgregarSelector(this);
        }

        private void frmPerfiles_FormClosed(object sender, FormClosedEventArgs e)
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
            CargarArbol();
        }

        private void GuardarDefaults(Control.ControlCollection controles)
        {
            foreach (Control c in controles)
            {
                if (!string.IsNullOrEmpty(c.Name) && !string.IsNullOrEmpty(c.Text))
                {
                    _controles[c.Name] = c;
                    _defaults[c.Name]  = c.Text;
                }
                if (c.HasChildren) GuardarDefaults(c.Controls);
            }
        }

        private void CargarArbol()
        {
            treePermisos.Nodes.Clear();
            foreach (NodoPermiso raiz in _bll.ObtenerArbol())
            {
                TreeNode nodoTree = CrearNodoVisual(raiz);
                AgregarNodosRecursivo(nodoTree, raiz);
                treePermisos.Nodes.Add(nodoTree);
            }
            treePermisos.ExpandAll();
        }

        private void AgregarNodosRecursivo(TreeNode nodoTree, NodoPermiso nodo)
        {
            foreach (NodoPermiso hijo in nodo.ObtenerHijos())
            {
                TreeNode nodoHijo = CrearNodoVisual(hijo);
                AgregarNodosRecursivo(nodoHijo, hijo);
                nodoTree.Nodes.Add(nodoHijo);
            }
        }

        private TreeNode CrearNodoVisual(NodoPermiso nodo)
        {
            var mgr = SeguridadYServicios.IdiomaManager.getInstance();
            string prefijo = nodo.EsHoja()
                ? (mgr.Traducir("prefijo_Permiso") ?? "[Permiso] ")
                : (mgr.Traducir("prefijo_Perfil")  ?? "[Perfil] ");
            return new TreeNode(prefijo + nodo.Nombre) { Tag = nodo };
        }

        private void btnAgregarPerfil_Click(object sender, EventArgs e)
        {
            string nombre = Microsoft.VisualBasic.Interaction.InputBox(
                "Nombre del perfil:", "Agregar Perfil");
            if (string.IsNullOrWhiteSpace(nombre)) return;

            int? padreId = null;
            if (treePermisos.SelectedNode?.Tag is PerfilPermiso padre)
                padreId = padre.Id;

            _bll.AgregarPerfil(nombre, padreId);
            CargarArbol();
        }

        private void btnAgregarPermiso_Click(object sender, EventArgs e)
        {
            if (!(treePermisos.SelectedNode?.Tag is PerfilPermiso perfil))
            {
                MessageBox.Show("Seleccioná un perfil para agregar el permiso.",
                    "Aviso", MessageBoxButtons.OK, MessageBoxIcon.Warning);
                return;
            }

            string nombre = Microsoft.VisualBasic.Interaction.InputBox(
                "Nombre del permiso:", "Agregar Permiso");
            if (string.IsNullOrWhiteSpace(nombre)) return;

            _bll.AgregarPermiso(nombre, perfil.Id);
            CargarArbol();
        }

        private void btnEliminar_Click(object sender, EventArgs e)
        {
            if (!(treePermisos.SelectedNode?.Tag is NodoPermiso nodo)) return;

            if (MessageBox.Show($"¿Eliminar '{nodo.Nombre}' y todos sus hijos?",
                "Confirmar", MessageBoxButtons.YesNo, MessageBoxIcon.Question) == DialogResult.Yes)
            {
                _bll.Eliminar(nodo.Id);
                CargarArbol();
            }
        }

        private void btnCerrar_Click(object sender, EventArgs e)
        {
            this.Close();
        }

        private void treePermisos_AfterSelect(object sender, TreeViewEventArgs e)
        {
            if (!(e.Node?.Tag is NodoPermiso nodo)) return;
            lblSeleccionado.Text = nodo.EsHoja() ? $"Permiso: {nodo.Nombre}" : $"Perfil: {nodo.Nombre}";
            btnAgregarPermiso.Enabled = !nodo.EsHoja();
        }
    }
}
