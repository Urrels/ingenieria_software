using BE;
using System;
using System.Collections.Generic;

namespace BLL
{
    public class IdiomaBLL
    {
        private readonly DAL.IdiomaDAL _dal = new DAL.IdiomaDAL();

        public List<IDIOMA> ListarTodos() => _dal.ListarTodos();

        public List<IDIOMA> ListarHabilitados() => _dal.ListarHabilitados();

        public IDIOMA ObtenerPorId(int id) => _dal.ObtenerPorId(id);

        public bool EstaEnUso(int id) => _dal.EstaEnUso(id);

        public IDIOMA Crear(string nombre, bool habilitado)
        {
            int id = _dal.Insertar(nombre, habilitado);
            return new IDIOMA { Id = id, Nombre = nombre, Habilitado = habilitado };
        }

        public void Renombrar(int id, string nombre) =>
            _dal.Renombrar(id, nombre);

        public void ActualizarEstado(int id, bool habilitado)
        {
            if (!habilitado)
            {
                IDIOMA idioma = _dal.ObtenerPorId(id);
                if (idioma != null && idioma.Predeterminado)
                    throw new InvalidOperationException(
                        "No se puede deshabilitar el idioma predeterminado del sistema.");
                if (_dal.EstaEnUso(id))
                    throw new InvalidOperationException(
                        "No se puede deshabilitar un idioma que está en uso por algún usuario.");
            }
            _dal.ActualizarEstado(id, habilitado);
        }

        public void Eliminar(int id)
        {
            IDIOMA idioma = _dal.ObtenerPorId(id);
            if (idioma != null && idioma.Predeterminado)
                throw new InvalidOperationException(
                    "No se puede eliminar el idioma predeterminado del sistema.");
            if (_dal.EstaEnUso(id))
                throw new InvalidOperationException(
                    "No se puede eliminar un idioma que está en uso por algún usuario.");
            _dal.Eliminar(id);
        }

        public void RegistrarControl(string clave, string textoDefault) =>
            _dal.RegistrarControl(clave, textoDefault);

        public Dictionary<string, string> CargarTraducciones(int idiomaId) =>
            _dal.CargarTraducciones(idiomaId);

        public List<CONTROL_IDIOMA> ListarControlesConTraduccion(int idiomaId) =>
            _dal.ListarControlesConTraduccion(idiomaId);

        public void GuardarTraduccion(int idiomaId, int controlId, string texto) =>
            _dal.GuardarTraduccion(idiomaId, controlId, texto);
    }
}
