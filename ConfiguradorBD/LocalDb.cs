using System;
using System.Diagnostics;
using System.IO;
using System.Linq;
using Microsoft.Win32;

namespace ConfiguradorBD
{
    internal static class LocalDb
    {
        internal const string Instancia = "MSSQLLocalDB";
        internal static string Servidor => @"(localdb)\" + Instancia;

        private const string ClaveVersiones = @"SOFTWARE\Microsoft\Microsoft SQL Server Local DB\Installed Versions";

        internal static bool EstaInstalado() => RutaEjecutable() != null;

        internal static string RutaEjecutable()
        {
            foreach (RegistryView vista in new[] { RegistryView.Registry64, RegistryView.Registry32 })
            {
                using (RegistryKey raiz = RegistryKey.OpenBaseKey(RegistryHive.LocalMachine, vista))
                using (RegistryKey versiones = raiz.OpenSubKey(ClaveVersiones))
                {
                    if (versiones == null) continue;
                    foreach (string version in versiones.GetSubKeyNames().OrderByDescending(v => v))
                    {
                        using (RegistryKey clave = versiones.OpenSubKey(version))
                        {
                            string api = clave?.GetValue("InstanceAPIPath") as string;
                            if (string.IsNullOrEmpty(api)) continue;
                            DirectoryInfo carpetaVersion = Directory.GetParent(api)?.Parent?.Parent;
                            if (carpetaVersion == null) continue;
                            string exe = Path.Combine(carpetaVersion.FullName, "Tools", "Binn", "SqlLocalDB.exe");
                            if (File.Exists(exe)) return exe;
                        }
                    }
                }
            }
            return null;
        }

        internal static void AsegurarInstancia(RegistroInstalacion log)
        {
            string exe = RutaEjecutable() ?? "SqlLocalDB.exe";
            Ejecutar(exe, "create " + Instancia, log);
            int codigo = Ejecutar(exe, "start " + Instancia, log);
            if (codigo != 0)
                throw new InvalidOperationException(
                    "No se pudo iniciar la instancia de LocalDB (código " + codigo + "). Revisá el detalle en el registro.");
        }

        internal static bool InstalarSilencioso(string rutaMsi, RegistroInstalacion log)
        {
            log.Escribir("Instalando SQL Server Express LocalDB en modo silencioso: " + rutaMsi);
            int codigo = Ejecutar("msiexec.exe",
                "/i \"" + rutaMsi + "\" /qn IACCEPTSQLLOCALDBLICENSETERMS=YES", log);
            bool ok = codigo == 0 || codigo == 3010;
            log.Escribir(ok ? "LocalDB instalado correctamente." : "La instalación de LocalDB terminó con código " + codigo + ".");
            return ok;
        }

        private static int Ejecutar(string archivo, string argumentos, RegistroInstalacion log)
        {
            var inicio = new ProcessStartInfo(archivo, argumentos)
            {
                UseShellExecute = false,
                CreateNoWindow = true,
                RedirectStandardOutput = true,
                RedirectStandardError = true
            };
            using (Process proceso = Process.Start(inicio))
            {
                string salida = proceso.StandardOutput.ReadToEnd() + proceso.StandardError.ReadToEnd();
                proceso.WaitForExit();
                log.Escribir($"{Path.GetFileName(archivo)} {argumentos} -> código {proceso.ExitCode}. {salida.Trim()}");
                return proceso.ExitCode;
            }
        }
    }
}
