using Microsoft.Win32;
using System;
using System.Data.SqlClient;
using System.IO;
using System.Windows.Forms;

namespace CAPAS
{
    // Carga de datos de demostración desde la interfaz: la ofrece cuando el sistema está vacío
    // y también está disponible en Administración → Cargar datos de demostración.
    internal static class DemoUI
    {
        private const string ClaveRegistro = @"Software\GestionGimnasio";
        private const string ValorPreguntada = "DemoPreguntada";

        internal static void OfrecerSiSistemaVacio(IWin32Window dueño)
        {
            var sesion = SeguridadYServicios.SessionManager.getInstance();
            if (!sesion.TienePermiso("Cargar datos de demostración") || YaSePregunto()) return;

            try
            {
                if (!new BLL.DatosDemoBLL().ContarDatos().SistemaVacio) return;
            }
            catch (SqlException)
            {
                return;
            }

            MarcarPreguntada();
            if (MsgBox.Show(Textos.T("msg_OfrecerDemo",
                    "El sistema todavía no tiene datos para probar los procesos. ¿Querés cargar datos de demostración " +
                    "(empleados, grillas, fichajes, equipos y evaluaciones)?"),
                    "Datos de demostración", MsgBox.Botones.SiNo, MsgBox.Icono.Pregunta) == DialogResult.Yes)
                Cargar();
        }

        internal static void CargarConConfirmacion(IWin32Window dueño)
        {
            if (MsgBox.Show(Textos.T("msg_ConfirmarCargaDemo",
                    "Se cargarán empleados, franjas, grillas, fichajes y equipos de ejemplo. " +
                    "Solo se puede si el sistema no tiene datos de negocio. ¿Continuar?"),
                    "Confirmar", MsgBox.Botones.SiNo, MsgBox.Icono.Pregunta) != DialogResult.Yes) return;
            Cargar();
        }

        private static void Cargar()
        {
            string archivo = BLL.DatosDemoBLL.BuscarArchivo();
            if (archivo == null)
            {
                MsgBox.Show(Textos.T("msg_FaltaArchivoDemo", "No se encontró el archivo datos_demo.json en la carpeta Scripts."),
                    "Error", MsgBox.Botones.OK, MsgBox.Icono.Error);
                return;
            }

            BE.ResultadoCargaDemo r;
            try
            {
                Cursor.Current = Cursors.WaitCursor;
                var bll = new BLL.DatosDemoBLL();
                r = bll.Cargar(BLL.DatosDemoBLL.Leer(archivo));
            }
            catch (Exception ex) when (ex is InvalidOperationException || ex is InvalidDataException || ex is FileNotFoundException
                                       || ex is Newtonsoft.Json.JsonException || ex is SqlException)
            {
                MsgBox.Show(Textos.T("msg_NoSePudoCargarDemo", "No se pudo cargar la demostración: {0}", ex.Message),
                    "Error", MsgBox.Botones.OK, MsgBox.Icono.Error);
                return;
            }
            finally
            {
                Cursor.Current = Cursors.Default;
            }

            string resumen = Textos.T("msg_DemoCargada",
                "Demostración cargada: {0} empleados (contraseña de todos: {1}), {2} franjas, {3} grillas, {4} fichajes, {5} evaluaciones y {6} equipos.",
                r.Empleados, r.ContrasenaEmpleados, r.Franjas, r.Grillas, r.Fichajes, r.Evaluaciones, r.Equipos);
            string nota = Textos.T("msg_DemoCargadaNota",
                "La próxima semana quedó lista para probar Generar grilla. Un empleado quedó con un ingreso sin egreso a propósito: " +
                "se corrige en Asistencia > Corregir fichajes.");
            MsgBox.Show(resumen + "\n\n" + nota, "Éxito", MsgBox.Botones.OK, MsgBox.Icono.Exito);
        }

        private static bool YaSePregunto()
        {
            try
            {
                using (RegistryKey clave = Registry.CurrentUser.OpenSubKey(ClaveRegistro))
                    return clave != null && Convert.ToInt32(clave.GetValue(ValorPreguntada, 0)) == 1;
            }
            catch (Exception ex) when (ex is System.Security.SecurityException || ex is UnauthorizedAccessException || ex is IOException)
            {
                return false;
            }
        }

        private static void MarcarPreguntada()
        {
            try
            {
                using (RegistryKey clave = Registry.CurrentUser.CreateSubKey(ClaveRegistro))
                    clave?.SetValue(ValorPreguntada, 1, RegistryValueKind.DWord);
            }
            catch (Exception ex) when (ex is System.Security.SecurityException || ex is UnauthorizedAccessException || ex is IOException)
            {
            }
        }
    }
}
