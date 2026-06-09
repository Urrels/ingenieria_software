using System;
using System.Collections.Generic;
using System.Text;
using System.Data.SqlClient;

namespace DAL
{
    public class ScriptDAL
    {
        public void EjecutarScript(string sql)
        {
            List<string> bloques = SepararPorGO(sql);

            Acceso db = new Acceso();
            db.Abrir();
            try
            {
                foreach (string bloque in bloques)
                {
                    string b = bloque.Trim();
                    if (string.IsNullOrEmpty(b) || b.StartsWith("USE "))
                        continue;
                    try
                    {
                        db.EjecutarDirecto(b);
                    }
                    catch (Exception ex)
                    {
                        System.Diagnostics.Debug.WriteLine("Error en bloque: " + ex.Message);
                        System.Diagnostics.Debug.WriteLine("Bloque: " + b);
                    }
                }
            }
            finally { db.Cerrar(); }
        }

        private List<string> SepararPorGO(string sql)
        {
            List<string> bloques = new List<string>();
            StringBuilder actual = new StringBuilder();

            string[] lineas = sql.Replace("\r\n", "\n").Split('\n');

            foreach (string linea in lineas)
            {
                string trimmed = linea.Trim();


                if (string.Equals(trimmed, "GO", StringComparison.OrdinalIgnoreCase))
                {
                    if (actual.Length > 0)
                    {
                        bloques.Add(actual.ToString());
                        actual.Clear();
                    }
                }
                else
                {
                    actual.AppendLine(linea);
                }
            }


            if (actual.Length > 0)
                bloques.Add(actual.ToString());

            return bloques;
        }

        public void EjecutarDirectoTexto(string sql)
        {
            Acceso db = new Acceso();
            db.Abrir();
            try { db.EjecutarDirecto(sql); }
            finally { db.Cerrar(); }
        }
    }
}