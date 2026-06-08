using BE;
using System;
using System.Collections.Generic;
using System.Windows.Forms;
using ReaLTaiizor.Forms;
using ReaLTaiizor.Manager;

namespace CAPAS
{
    public partial class frmPerfiles : MaterialForm, SeguridadYServicios.IObservadorIdioma
    {
        private readonly BLL.PerfilBLL _bll = new BLL.PerfilBLL();
        private readonly Dictionary<string, Control> _controles = new Dictionary<string, Control>();
        private readonly Dictionary<string, string>  _defaults  = new Dictionary<string, string>();

        private List<Permiso> _permisosDisponibles = new List<Permiso>();

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
            _permisosDisponibles = _bll.ObtenerPermisosDisponibles();
            ActualizarIdioma();
            CargarArbol();
            IdiomaUIHelper.AgregarSelector(this);
            MaterialSkinManager.Instance.AddFormToManage(this);
            AppTheme.AplicarTema(this);
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
                : (mgr.Traducir("prefijo_Rol")     ?? "[Rol] ");
            return new TreeNode(prefijo + nodo.Nombre) { Tag = nodo };
        }

        private void btnAgregarRol_Click(object sender, EventArgs e)
        {
            string nombre = Microsoft.VisualBasic.Interaction.InputBox(
                "Nombre del rol:", "Agregar Rol");
            if (string.IsNullOrWhiteSpace(nombre)) return;

            int? padreId = null;
            if (treePermisos.SelectedNode?.Tag is Rol padre)
                padreId = padre.Id;

            try
            {
                _bll.AgregarRol(nombre, padreId);
            }
            catch (InvalidOperationException ex)
            {
                MessageBox.Show(ex.Message, "Error", MessageBoxButtons.OK, MessageBoxIcon.Error);
                return;
            }
            CargarArbol();
        }

        private void btnGuardarPermisos_Click(object sender, EventArgs e)
        {
            if (!(treePermisos.SelectedNode?.Tag is Rol rol)) return;

            List<int> seleccionados = new List<int>();
            for (int i = 0; i < chkPermisos.Items.Count; i++)
            {
                if (chkPermisos.GetItemChecked(i))
                    seleccionados.Add(((Permiso)chkPermisos.Items[i]).Id);
            }

            _bll.ActualizarPermisosDeRol(rol.Id, seleccionados);
            CargarArbol();

            foreach (TreeNode tn in treePermisos.Nodes)
            {
                TreeNode encontrado = BuscarNodo(tn, rol.Id);
                if (encontrado != null) { treePermisos.SelectedNode = encontrado; break; }
            }
        }

        private TreeNode BuscarNodo(TreeNode raiz, int id)
        {
            if (raiz.Tag is NodoPermiso n && n.Id == id) return raiz;
            foreach (TreeNode hijo in raiz.Nodes)
            {
                TreeNode resultado = BuscarNodo(hijo, id);
                if (resultado != null) return resultado;
            }
            return null;
        }

        private void btnEliminar_Click(object sender, EventArgs e)
        {
            if (!(treePermisos.SelectedNode?.Tag is NodoPermiso nodo)) return;

            if (nodo.EsHoja())
            {
                MessageBox.Show("Los permisos del catálogo no se pueden eliminar.\nDesasignalo usando los checkboxes.",
                    "Aviso", MessageBoxButtons.OK, MessageBoxIcon.Information);
                return;
            }

            if (MessageBox.Show($"¿Eliminar el rol '{nodo.Nombre}' y todos sus sub-roles?",
                "Confirmar", MessageBoxButtons.YesNo, MessageBoxIcon.Question) == DialogResult.Yes)
            {
                try
                {
                    _bll.Eliminar(nodo.Id);
                }
                catch (InvalidOperationException ex)
                {
                    MessageBox.Show(ex.Message, "Error", MessageBoxButtons.OK, MessageBoxIcon.Error);
                    return;
                }
                panelPermisos.Visible = false;
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

            if (nodo is Rol rol)
            {
                lblSeleccionado.Text  = $"Rol: {rol.Nombre}";
                btnEliminar.Enabled   = !rol.Protegido;
                panelPermisos.Visible = true;
                panelPadre.Visible    = true;
                PopularCheckPermisos(rol);
                PopularComboPadre(rol, e.Node);
            }
            else
            {
                lblSeleccionado.Text  = $"Permiso: {nodo.Nombre}";
                btnEliminar.Enabled   = false;
                panelPermisos.Visible = false;
                panelPadre.Visible    = false;
            }
        }

        private void PopularCheckPermisos(Rol rol)
        {
            HashSet<int> asignados = new HashSet<int>();
            foreach (NodoPermiso hijo in rol.ObtenerHijos())
            {
                if (hijo.EsHoja()) asignados.Add(hijo.Id);
            }

            chkPermisos.Items.Clear();
            foreach (Permiso p in _permisosDisponibles)
                chkPermisos.Items.Add(p, asignados.Contains(p.Id));
        }

        private void PopularComboPadre(Rol rol, TreeNode nodoActual)
        {
            HashSet<int> excluidos = new HashSet<int> { rol.Id };
            RecolectarDescendientes(nodoActual, excluidos);

            var candidatos = new List<ItemRol> { new ItemRol { Id = null, Nombre = "(ninguno)" } };
            foreach (TreeNode raiz in treePermisos.Nodes)
                RecolectarRolesCandidatos(raiz, excluidos, candidatos);

            cboPadre.DataSource = null;
            cboPadre.DataSource = candidatos;

            cboPadre.SelectedIndex = 0;
            if (rol.PadreId.HasValue)
            {
                for (int i = 1; i < candidatos.Count; i++)
                {
                    if (candidatos[i].Id == rol.PadreId)
                    {
                        cboPadre.SelectedIndex = i;
                        break;
                    }
                }
            }
        }

        private void RecolectarDescendientes(TreeNode nodo, HashSet<int> ids)
        {
            foreach (TreeNode hijo in nodo.Nodes)
            {
                if (hijo.Tag is Rol r)
                {
                    ids.Add(r.Id);
                    RecolectarDescendientes(hijo, ids);
                }
            }
        }

        private void RecolectarRolesCandidatos(TreeNode nodo, HashSet<int> excluidos, List<ItemRol> lista)
        {
            if (!(nodo.Tag is Rol r) || excluidos.Contains(r.Id)) return;
            lista.Add(new ItemRol { Id = r.Id, Nombre = r.Nombre });
            foreach (TreeNode hijo in nodo.Nodes)
                RecolectarRolesCandidatos(hijo, excluidos, lista);
        }

        private void btnAsignarPadre_Click(object sender, EventArgs e)
        {
            if (!(treePermisos.SelectedNode?.Tag is Rol rol)) return;
            if (!(cboPadre.SelectedItem is ItemRol seleccionado)) return;

            try
            {
                _bll.CambiarPadre(rol.Id, seleccionado.Id);
            }
            catch (InvalidOperationException ex)
            {
                MessageBox.Show(ex.Message, "Error", MessageBoxButtons.OK, MessageBoxIcon.Error);
                return;
            }

            CargarArbol();
            foreach (TreeNode tn in treePermisos.Nodes)
            {
                TreeNode encontrado = BuscarNodo(tn, rol.Id);
                if (encontrado != null) { treePermisos.SelectedNode = encontrado; break; }
            }
        }

        private class ItemRol
        {
            public int? Id { get; set; }
            public string Nombre { get; set; }
            public override string ToString() => Nombre;
        }

      
    }
}
