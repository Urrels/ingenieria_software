using BE;
using System;
using System.Collections.Generic;
using System.Drawing;
using System.Windows.Forms;
using System.Windows.Forms.VisualStyles;
using ReaLTaiizor.Forms;
using ReaLTaiizor.Manager;

namespace CAPAS
{
    public partial class frmPerfiles : MaterialForm, SeguridadYServicios.IObservadorIdioma
    {
        private readonly BLL.PerfilBLL _bll = new BLL.PerfilBLL();
        private readonly Dictionary<string, Control> _controles = new Dictionary<string, Control>();
        private readonly Dictionary<string, string> _defaults = new Dictionary<string, string>();

        private List<Permiso> _permisosDisponibles = new List<Permiso>();
        private readonly HashSet<int> _padresDeshabilitados = new HashSet<int>();
        private readonly HashSet<int> _permisosHeredados = new HashSet<int>();
        private int _cboPadreIndiceAnterior;

        public frmPerfiles()
        {
            InitializeComponent();
        }

        private void frmPerfiles_Load(object sender, EventArgs e)
        {
            GuardarDefaults(this.Controls);
            _controles[this.Name] = this;
            _defaults[this.Name] = this.Text;
            SeguridadYServicios.IdiomaManager.getInstance().Registrar(this);
            _permisosDisponibles = _bll.ObtenerPermisosDisponibles();
            cboPadre.DrawMode = DrawMode.OwnerDrawFixed;
            cboPadre.ItemHeight = 20;
            cboPadre.DrawItem += cboPadre_DrawItem;
            cboPadre.SelectedIndexChanged += cboPadre_SelectedIndexChanged;
            chkPermisos.DrawMode = DrawMode.OwnerDrawFixed;
            chkPermisos.DrawItem += chkPermisos_DrawItem;
            chkPermisos.ItemCheck += chkPermisos_ItemCheck;
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
                    _defaults[c.Name] = c.Text;
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
                : (mgr.Traducir("prefijo_Rol") ?? "[Rol] ");
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
                int permisoId = ((Permiso)chkPermisos.Items[i]).Id;
                if (_permisosHeredados.Contains(permisoId)) continue;
                seleccionados.Add(permisoId);
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
                MsgBox.Show("Los permisos del catálogo no se pueden eliminar.\nDesasignalo usando los checkboxes.",
                    "Aviso", MsgBox.Botones.OK, MsgBox.Icono.Exito);
                return;
            }

            if (MsgBox.Show($"¿Eliminar el rol '{nodo.Nombre}' y todos sus sub-roles?",
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
                lblSeleccionado.Text = $"Rol: {rol.Nombre}";
                btnEliminar.Enabled = !rol.Protegido;
                panelPermisos.Visible = true;
                panelPadre.Visible = true;
                PopularCheckPermisos(rol);
                PopularComboPadre(rol, e.Node);
            }
            else
            {
                lblSeleccionado.Text = $"Permiso: {nodo.Nombre}";
                btnEliminar.Enabled = false;
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

            _permisosHeredados.Clear();
            foreach (int id in _bll.ObtenerPermisosHeredados(rol.Id))
                _permisosHeredados.Add(id);

            chkPermisos.Items.Clear();
            foreach (Permiso p in _permisosDisponibles)
            {
                bool marcado = asignados.Contains(p.Id) && !_permisosHeredados.Contains(p.Id);
                chkPermisos.Items.Add(p, marcado);
            }
        }

        private void chkPermisos_ItemCheck(object sender, ItemCheckEventArgs e)
        {
            if (e.Index < 0 || e.Index >= chkPermisos.Items.Count) return;
            if (chkPermisos.Items[e.Index] is Permiso p && _permisosHeredados.Contains(p.Id))
                e.NewValue = CheckState.Unchecked;
        }

        private void chkPermisos_DrawItem(object sender, DrawItemEventArgs e)
        {
            if (e.Index < 0 || e.Index >= chkPermisos.Items.Count) return;

            bool heredado = chkPermisos.Items[e.Index] is Permiso p && _permisosHeredados.Contains(p.Id);
            Color colorTexto = heredado ? SystemColors.GrayText : chkPermisos.ForeColor;

            e.DrawBackground();

            CheckBoxState estadoCheck = chkPermisos.GetItemChecked(e.Index)
                ? (heredado ? CheckBoxState.CheckedDisabled : CheckBoxState.CheckedNormal)
                : (heredado ? CheckBoxState.UncheckedDisabled : CheckBoxState.UncheckedNormal);

            Size tamanioCheck = CheckBoxRenderer.GetGlyphSize(e.Graphics, estadoCheck);
            Point puntoCheck = new Point(e.Bounds.X + 2, e.Bounds.Y + (e.Bounds.Height - tamanioCheck.Height) / 2);
            CheckBoxRenderer.DrawCheckBox(e.Graphics, puntoCheck, estadoCheck);

            Rectangle rectTexto = new Rectangle(e.Bounds.X + tamanioCheck.Width + 4, e.Bounds.Y,
                e.Bounds.Width - tamanioCheck.Width - 4, e.Bounds.Height);
            using (Brush brush = new SolidBrush(colorTexto))
                e.Graphics.DrawString(chkPermisos.Items[e.Index].ToString(), e.Font, brush, rectTexto);

            e.DrawFocusRectangle();
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
