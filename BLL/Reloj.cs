using System;

namespace BLL
{
    public static class Reloj
    {
        private static Func<DateTime> _fuente = () => DateTime.Now;

        public static DateTime Ahora => _fuente();

        public static DateTime Hoy => _fuente().Date;

        public static void Usar(Func<DateTime> fuente)
        {
            _fuente = fuente ?? (() => DateTime.Now);
        }
    }
}
