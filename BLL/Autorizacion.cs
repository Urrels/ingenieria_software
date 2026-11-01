using SeguridadYServicios;
using System;

namespace BLL
{
    // La interfaz oculta las opciones sin permiso; esta guarda lo exige también en la capa de negocio.
    internal static class Autorizacion
    {
        internal static void Exigir(string permiso)
        {
            if (!SessionManager.getInstance().TienePermiso(permiso))
                throw new UnauthorizedAccessException("No tenés el permiso \"" + permiso + "\" para realizar esta operación.");
        }
    }
}
