using BE;
using System;
using System.Collections.Generic;
using System.Windows.Forms;

namespace CAPAS
{
    public partial class frmAsignarPerfiles : FormBase
    {
        private readonly BLL.PerfilBLL _perfilBll = new BLL.PerfilBLL();
        private readonly BLL.UsuarioPerfilBLL _asignacionBll = new BLL.UsuarioPerfilBLL();

        private readonly USUARIO _usuario;
        private List<NodoPermiso> _arbol;

        public frmAsignarPerfiles(USUARIO usuario)
        {
            InitializeComponent();
            _usuario = usuario;
        }

        private void frmAsignarPerfiles_Load(object sender, EventArgs e)
        {
            lblUsuario.Text = "Usuario: " + _usuario.Usuario;
            InicializarFormulario();
            _arbol = _perfilBll.ObtenerArbol();
            CargarArbolRoles();
            MarcarRolesAsignados();
        }

        private void CargarArbolRoles()
        {
            treeRoles.Nodes.Clear();
            foreach (NodoPermiso raiz in _arbol)
            {
                if (!raiz.EsHoja())
                {
                    TreeNode nodo = new TreeNode(raiz.Nombre) { Tag = raiz };
                    AgregarRolesRecursivo(nodo, raiz);
                    treeRoles.Nodes.Add(nodo);
                }
            }
            treeRoles.ExpandAll();
        }

        private void AgregarRolesRecursivo(TreeNode nodoTree, NodoPermiso nodo)
        {
            foreach (NodoPermiso hijo in nodo.ObtenerHijos())
            {
                if (!hijo.EsHoja())
                {
                    TreeNode nodoHijo = new TreeNode(hijo.Nombre) { Tag = hijo };
                    AgregarRolesRecursivo(nodoHijo, hijo);
                    nodoTree.Nodes.Add(nodoHijo);
                }
            }
        }

        private void MarcarRolesAsignados()
        {
            List<int> asignados = _asignacionBll.ObtenerPerfilesAsignados(_usuario.Id);
            MarcarNodos(treeRoles.Nodes, asignados);
        }

        private void MarcarNodos(TreeNodeCollection nodos, List<int> asignados)
        {
            foreach (TreeNode tn in nodos)
            {
                if (tn.Tag is NodoPermiso nodo)
                    tn.Checked = asignados.Contains(nodo.Id);
                MarcarNodos(tn.Nodes, asignados);
            }
        }

        private void btnGuardar_Click(object sender, EventArgs e)
        {
            List<int> seleccionados = new List<int>();
            ObtenerIdsCheckeados(treeRoles.Nodes, seleccionados);
            try
            {
                _asignacionBll.GuardarAsignaciones(_usuario.Id, seleccionados);
            }
            catch (InvalidOperationException ex)
            {
                MsgBox.Show(ex.Message, "Error", MsgBox.Botones.OK, MsgBox.Icono.Error);
                return;
            }
            MsgBox.Show(Textos.T("msg_AsignacionesGuardadas", "Asignaciones guardadas."), "Éxito",
                MsgBox.Botones.OK, MsgBox.Icono.Exito);
        }

        private void ObtenerIdsCheckeados(TreeNodeCollection nodos, List<int> ids)
        {
            foreach (TreeNode tn in nodos)
            {
                if (tn.Checked && tn.Tag is NodoPermiso nodo)
                    ids.Add(nodo.Id);
                ObtenerIdsCheckeados(tn.Nodes, ids);
            }
        }

        private void btnCerrar_Click(object sender, EventArgs e)
        {
            this.Close();
        }
    }
}
