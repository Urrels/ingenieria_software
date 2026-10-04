using BE;
using System;
using System.Collections.Generic;
using System.Drawing;
using System.Windows.Forms;
using System.Windows.Forms.VisualStyles;

namespace CAPAS
{
    public partial class frmPerfiles : FormBase
    {
        private readonly BLL.PerfilBLL _bll = new BLL.PerfilBLL();

        private List<Permiso> _permisosDisponibles = new List<Permiso>();
        private readonly HashSet<int> _padresDeshabilitados = new HashSet<int>();
        private int _cboPadreIndiceAnterior;

        public frmPerfiles()
        {
            InitializeComponent();
        }

        private void frmPerfiles_Load(object sender, EventArgs e)
        {
            _permisosDisponibles = _bll.ObtenerPermisosDisponibles();
            cboPadre.DrawMode = DrawMode.OwnerDrawFixed;
            cboPadre.ItemHeight = 20;
            cboPadre.DrawItem += cboPadre_DrawItem;
            cboPadre.SelectedIndexChanged += cboPadre_SelectedIndexChanged;
            InicializarFormulario();
        }

        protected override void ActualizarTextosDinamicos()
        {
            CargarArbol();
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
                : (mgr.Traducir("prefijo_Rol") ?? "[Rol] ");
            return new TreeNode(prefijo + nodo.Nombre) { Tag = nodo };
        }

        private void btnAgregarRol_Click(object sender, EventArgs e)
        {
            string nombre = Microsoft.VisualBasic.Interaction.InputBox(
                Textos.T("input_NombreDelRol", "Nombre del rol:"), Textos.T("input_AgregarRol", "Agregar Rol"));
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
                MsgBox.Show(ex.Message, "Error", MsgBox.Botones.OK, MsgBox.Icono.Error);
                return;
            }
            CargarArbol();
        }

        private void btnDuplicarRol_Click(object sender, EventArgs e)
        {
            if (!(treePermisos.SelectedNode?.Tag is Rol rol))
            {
                MsgBox.Show(Textos.T("msg_SeleccionaUnRol", "Seleccioná un rol."), "Aviso", MsgBox.Botones.OK, MsgBox.Icono.Atencion);
                return;
            }

            string nombre = Microsoft.VisualBasic.Interaction.InputBox(
                Textos.T("input_NombreDelNuevoRol", "Nombre del nuevo rol:"), Textos.T("input_DuplicarRol", "Duplicar rol"),
                Textos.T("input_SufijoCopia", "{0} (copia)", rol.Nombre));
            if (string.IsNullOrWhiteSpace(nombre)) return;

            try
            {
                _bll.DuplicarRol(rol.Id, nombre.Trim());
            }
            catch (InvalidOperationException ex)
            {
                MsgBox.Show(ex.Message, "Error", MsgBox.Botones.OK, MsgBox.Icono.Error);
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
                if (!chkPermisos.GetItemChecked(i)) continue;
                seleccionados.Add(((Permiso)chkPermisos.Items[i]).Id);
            }

            try
            {
                _bll.ActualizarPermisosDeRol(rol.Id, seleccionados);
            }
            catch (InvalidOperationException ex)
            {
                MsgBox.Show(ex.Message, "Error", MsgBox.Botones.OK, MsgBox.Icono.Error);
                return;
            }
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
                MsgBox.Show(Textos.T("msg_LosPermisosDelCatalogoNoSePuedenEliminarDesasignaloUsandoLos", "Los permisos del catálogo no se pueden eliminar.\nDesasignalo usando los checkboxes."),
                    "Aviso", MsgBox.Botones.OK, MsgBox.Icono.Exito);
                return;
            }

            if (MsgBox.Show(Textos.T("msg_EliminarElRolYTodosSusSubroles", "¿Eliminar el rol '{0}' y todos sus sub-roles?", nodo.Nombre),
                "Confirmar", MsgBox.Botones.SiNo, MsgBox.Icono.Pregunta) == DialogResult.Yes)
            {
                try
                {
                    _bll.Eliminar(nodo.Id);
                }
                catch (InvalidOperationException ex)
                {
                    MsgBox.Show(ex.Message, "Error", MsgBox.Botones.OK, MsgBox.Icono.Error);
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
                lblSeleccionado.Text = Textos.T("lbl_RolSeleccionado", "Rol: {0}", rol.Nombre);
                btnEliminar.Enabled = !rol.Protegido;
                btnDuplicarRol.Enabled = true;
                panelPermisos.Visible = true;
                panelPadre.Visible = true;
                PopularCheckPermisos(rol);
                PopularComboPadre(rol, e.Node);
            }
            else
            {
                lblSeleccionado.Text = Textos.T("lbl_PermisoSeleccionado", "Permiso: {0}", nodo.Nombre);
                btnEliminar.Enabled = false;
                btnDuplicarRol.Enabled = false;
                panelPermisos.Visible = false;
                panelPadre.Visible = false;
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

            _padresDeshabilitados.Clear();
            RecolectarAncestrosSuperiores(nodoActual, _padresDeshabilitados);

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
            _cboPadreIndiceAnterior = cboPadre.SelectedIndex;
            cboPadre.Invalidate();
        }

        private void cboPadre_DrawItem(object sender, DrawItemEventArgs e)
        {
            if (e.Index < 0 || !(cboPadre.Items[e.Index] is ItemRol item)) return;

            bool deshabilitado = item.Id.HasValue && _padresDeshabilitados.Contains(item.Id.Value);
            Color colorTexto = deshabilitado ? SystemColors.GrayText : e.ForeColor;

            e.DrawBackground();
            using (Brush brush = new SolidBrush(colorTexto))
                e.Graphics.DrawString(item.Nombre, e.Font, brush, e.Bounds);
            e.DrawFocusRectangle();
        }

        private void cboPadre_SelectedIndexChanged(object sender, EventArgs e)
        {
            if (cboPadre.SelectedItem is ItemRol item &&
                item.Id.HasValue && _padresDeshabilitados.Contains(item.Id.Value))
            {
                cboPadre.SelectedIndex = _cboPadreIndiceAnterior;
                return;
            }
            _cboPadreIndiceAnterior = cboPadre.SelectedIndex;
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

        private void RecolectarAncestrosSuperiores(TreeNode nodoActual, HashSet<int> ids)
        {
            TreeNode actual = nodoActual.Parent?.Parent;
            while (actual != null)
            {
                if (actual.Tag is Rol r) ids.Add(r.Id);
                actual = actual.Parent;
            }
        }

        private void RecolectarRolesCandidatos(TreeNode nodo, HashSet<int> excluidos, List<ItemRol> lista)
        {
            if (!(nodo.Tag is Rol r)) return;
            if (!excluidos.Contains(r.Id))
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
                MsgBox.Show(ex.Message, "Error", MsgBox.Botones.OK, MsgBox.Icono.Error);
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
