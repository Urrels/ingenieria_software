using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Linq;
using System.Net.Sockets;
using System.ServiceProcess;
using Microsoft.Win32;

namespace ConfiguradorBD
{
    internal class InstanciaSql
    {
        internal string Servidor { get; set; }
        internal string NombreServicio { get; set; }
        internal bool EsLocalDb { get; set; }

        public override string ToString() => Servidor;

        internal static List<InstanciaSql> DetectarLocales()
        {
            var lista = new List<InstanciaSql>();
            foreach (RegistryView vista in new[] { RegistryView.Registry64, RegistryView.Registry32 })
            {
                using (RegistryKey raiz = RegistryKey.OpenBaseKey(RegistryHive.LocalMachine, vista))
                using (RegistryKey clave = raiz.OpenSubKey(@"SOFTWARE\Microsoft\Microsoft SQL Server\Instance Names\SQL"))
                {
                    if (clave == null) continue;
                    foreach (string nombre in clave.GetValueNames())
                    {
                        bool predeterminada = nombre.Equals("MSSQLSERVER", StringComparison.OrdinalIgnoreCase);
                        string servidor = predeterminada ? "." : @".\" + nombre;
                        if (lista.Any(i => i.Servidor.Equals(servidor, StringComparison.OrdinalIgnoreCase))) continue;
                        lista.Add(new InstanciaSql
                        {
                            Servidor = servidor,
                            NombreServicio = predeterminada ? "MSSQLSERVER" : "MSSQL$" + nombre
                        });
                    }
                }
            }
            if (LocalDb.EstaInstalado())
                lista.Add(new InstanciaSql { Servidor = LocalDb.Servidor, EsLocalDb = true });
            return lista;
        }

        internal static string ServicioDe(string servidor)
        {
            if (string.IsNullOrWhiteSpace(servidor)) return null;
            string s = servidor.Trim();
            if (s.StartsWith("(localdb)", StringComparison.OrdinalIgnoreCase)) return null;
            if (s.StartsWith("tcp:", StringComparison.OrdinalIgnoreCase)) s = s.Substring(4);
            int coma = s.IndexOf(',');
            if (coma >= 0) s = s.Substring(0, coma);

            string equipo = s, instancia = null;
            int barra = s.IndexOf('\\');
            if (barra >= 0)
            {
                equipo = s.Substring(0, barra);
                instancia = s.Substring(barra + 1);
            }
            if (!EsEquipoLocal(equipo)) return null;
            return string.IsNullOrEmpty(instancia) || instancia.Equals("MSSQLSERVER", StringComparison.OrdinalIgnoreCase)
                ? "MSSQLSERVER"
                : "MSSQL$" + instancia;
        }

        internal static ServiceControllerStatus? EstadoServicio(string nombreServicio)
        {
            if (nombreServicio == null) return null;
            try
            {
                using (var servicio = new ServiceController(nombreServicio))
                    return servicio.Status;
            }
            catch (InvalidOperationException)
            {
                return null;
            }
            catch (Win32Exception)
            {
                return null;
            }
        }

        internal static void IniciarServicio(string nombreServicio, TimeSpan espera)
        {
            using (var servicio = new ServiceController(nombreServicio))
            {
                if (servicio.Status == ServiceControllerStatus.Running) return;
                if (servicio.Status != ServiceControllerStatus.StartPending)
                    servicio.Start();
                servicio.WaitForStatus(ServiceControllerStatus.Running, espera);
            }
        }

        internal static bool PuertoTcpRemoto(string servidor, out string equipo, out int puerto)
        {
            equipo = null;
            puerto = 1433;
            if (string.IsNullOrWhiteSpace(servidor) || ServicioDe(servidor) != null) return false;
            string s = servidor.Trim();
            if (s.StartsWith("(localdb)", StringComparison.OrdinalIgnoreCase)) return false;
            if (s.StartsWith("tcp:", StringComparison.OrdinalIgnoreCase)) s = s.Substring(4);

            int coma = s.IndexOf(',');
            if (coma >= 0)
            {
                if (!int.TryParse(s.Substring(coma + 1).Trim(), out puerto)) return false;
                s = s.Substring(0, coma);
            }
            int barra = s.IndexOf('\\');
            if (barra >= 0)
            {
                if (coma < 0) return false;
                s = s.Substring(0, barra);
            }
            equipo = s.Trim();
            return equipo.Length > 0;
        }

        internal static bool PuertoAbierto(string equipo, int puerto, TimeSpan espera)
        {
            using (var cliente = new TcpClient())
            {
                try
                {
                    return cliente.ConnectAsync(equipo, puerto).Wait(espera) && cliente.Connected;
                }
                catch (AggregateException)
                {
                    return false;
                }
                catch (SocketException)
                {
                    return false;
                }
            }
        }

        private static bool EsEquipoLocal(string equipo)
        {
            return equipo == "." || equipo == "(local)"
                || equipo.Equals("localhost", StringComparison.OrdinalIgnoreCase)
                || equipo.Equals(Environment.MachineName, StringComparison.OrdinalIgnoreCase);
        }
    }
}
