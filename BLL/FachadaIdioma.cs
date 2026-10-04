using BE;
using SeguridadYServicios;
using System.Collections.Generic;

namespace BLL
{
    public class FachadaIdioma
    {
        private readonly IdiomaBLL _idiomas = new IdiomaBLL();

        public IDIOMA IdiomaActivo => IdiomaManager.getInstance().IdiomaActivo;

        public List<IDIOMA> ListarDisponibles() => _idiomas.ListarHabilitados();

        public void Cambiar(IDIOMA idioma)
        {
            Aplicar(idioma);
            USUARIO usuario = SessionManager.getInstance().getUsuario();
            if (usuario != null)
                new UsuarioBLL().ActualizarIdioma(usuario.Id, idioma.Id);
        }

        public void AplicarPreferencia(USUARIO usuario)
        {
            if (!usuario.IdiomaId.HasValue) return;
            IDIOMA idioma = _idiomas.ObtenerPorId(usuario.IdiomaId.Value);
            if (idioma != null && idioma.Habilitado)
                Aplicar(idioma);
        }

        public void RecargarSiEstaActivo(int idiomaId)
        {
            IDIOMA activo = IdiomaActivo;
            if (activo != null && activo.Id == idiomaId)
                Aplicar(activo);
        }

        public void RegistrarClave(string clave, string textoDefault) =>
            _idiomas.RegistrarControl(clave, textoDefault);

        public string Traducir(string clave) => IdiomaManager.getInstance().Traducir(clave);

        private void Aplicar(IDIOMA idioma)
        {
            Dictionary<string, string> traducciones = _idiomas.CargarTraducciones(idioma.Id);
            IdiomaManager.getInstance().CambiarIdioma(idioma, traducciones);
        }
    }
}
