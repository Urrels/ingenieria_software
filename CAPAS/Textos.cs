using System;
using System.Collections.Generic;
using System.Data.SqlClient;
using System.Globalization;
using System.Linq;
using System.Text;

namespace CAPAS
{
    internal static class Textos
    {
        private const int LargoMaximoClave = 100;
        private static readonly HashSet<string> _registradas = new HashSet<string>();

        internal static string T(string clave, string textoDefault, params object[] args)
        {
            Registrar(clave, textoDefault);
            string texto = new BLL.FachadaIdioma().Traducir(clave) ?? textoDefault;
            if (args.Length == 0) return texto;
            try
            {
                return string.Format(texto, args);
            }
            catch (FormatException)
            {
                return string.Format(textoDefault, args);
            }
        }

        internal static string ClaveDesdeTexto(string prefijo, string texto)
        {
            string sinAcentos = new string(texto.Normalize(NormalizationForm.FormD)
                .Where(c => CharUnicodeInfo.GetUnicodeCategory(c) != UnicodeCategory.NonSpacingMark)
                .ToArray());
            var palabras = sinAcentos.Split(new[] { ' ' }, StringSplitOptions.RemoveEmptyEntries)
                .Select(p => new string(p.Where(char.IsLetterOrDigit).ToArray()))
                .Where(p => p.Length > 0)
                .Select(p => char.ToUpperInvariant(p[0]) + p.Substring(1));
            string clave = prefijo + string.Concat(palabras);
            return clave.Length <= LargoMaximoClave ? clave : clave.Substring(0, LargoMaximoClave);
        }

        private static void Registrar(string clave, string textoDefault)
        {
            if (!_registradas.Add(clave)) return;
            try
            {
                new BLL.FachadaIdioma().RegistrarClave(clave, textoDefault);
            }
            catch (SqlException)
            {
                _registradas.Remove(clave);
            }
        }
    }
}
