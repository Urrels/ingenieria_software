using BE;
using System.Collections.Generic;

namespace BLL
{
    public class PerfilBLL
    {
        private readonly DAL.PerfilDAL _dal        = new DAL.PerfilDAL();
        private readonly IntegridadBLL _integridad = new IntegridadBLL();

        public List<NodoPermiso> ObtenerArbol()
        {
            List<NodoPermiso> todos = _dal.ListarTodos();
            Dictionary<int, NodoPermiso> mapa = new Dictionary<int, NodoPermiso>();

            foreach (NodoPermiso nodo in todos)
                mapa[nodo.Id] = nodo;

            List<NodoPermiso> raices = new List<NodoPermiso>();

            foreach (NodoPermiso nodo in todos)
            {
                if (nodo.PadreId == null)
                    raices.Add(nodo);
                else if (mapa.ContainsKey(nodo.PadreId.Value))
                    mapa[nodo.PadreId.Value].Agregar(nodo);
            }

            return raices;
        }

        public NodoPermiso AgregarPerfil(string nombre, int? padreId)
        {
            int id = _dal.Insertar(nombre, "PERFIL", padreId);
            _integridad.RecalcularIntegridadNodos();
            return new PerfilPermiso { Id = id, Nombre = nombre, PadreId = padreId };
        }

        public NodoPermiso AgregarPermiso(string nombre, int padreId)
        {
            int id = _dal.Insertar(nombre, "PERMISO", padreId);
            _integridad.RecalcularIntegridadNodos();
            return new Permiso { Id = id, Nombre = nombre, PadreId = padreId };
        }

        public void Eliminar(int id)
        {
            _dal.Eliminar(id);
            _integridad.RecalcularIntegridadNodos();
        }
    }
}
