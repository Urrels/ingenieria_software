using System.Collections.Generic;

namespace BLL
{
    public class NotificacionBLL
    {
        private readonly DAL.NotificacionDAL _dal = new DAL.NotificacionDAL();

        public bool Notificar(BE.USUARIO usuario, int grillaId, string mensaje)
        {
            bool medioValido = !string.IsNullOrEmpty(usuario.Email) || !string.IsNullOrEmpty(usuario.Telefono);

            var notificacion = new BE.Notificacion
            {
                GrillaId = grillaId,
                UsuarioId = usuario.Id,
                Mensaje = mensaje,
                Estado = medioValido ? "Enviada" : "Fallida"
            };
            notificacion.Id = _dal.Guardar(notificacion);

            return medioValido;
        }

        public List<BE.Notificacion> ListarPorUsuario(BE.USUARIO usuario) => _dal.ListarPorUsuario(usuario.Id);
    }
}