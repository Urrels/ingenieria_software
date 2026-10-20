using System;

namespace BLL
{
    // Fuente de la hora para fichajes y liquidación. Por defecto es el reloj del sistema;
    // el emulador de fichaje la reemplaza por un reloj acelerado.
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
