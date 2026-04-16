using DAL;

namespace BLL
{
    public static class AppInicializador
    {
        public static void Inicializar()
        {
            DBInicializador.InicializarSiNoExiste();
        }
    }
}