using System;
using System.IO;
using System.Linq;
using System.Xml.Linq;

namespace ConfiguradorBD
{
    internal static class ConfiguracionApp
    {
        internal const string NombreCadena = "BDCAPAS";

        internal static string RutaConfig =>
            Path.Combine(AppDomain.CurrentDomain.BaseDirectory, "CAPAS.exe.config");

        internal static string LeerCadena()
        {
            if (!File.Exists(RutaConfig)) return null;
            XElement add = BuscarCadena(XDocument.Load(RutaConfig));
            return (string)add?.Attribute("connectionString");
        }

        internal static void GuardarCadena(string cadena)
        {
            if (!File.Exists(RutaConfig))
                throw new FileNotFoundException(
                    "No se encontró CAPAS.exe.config junto al configurador.", RutaConfig);

            XDocument doc = XDocument.Load(RutaConfig);
            XElement add = BuscarCadena(doc);
            if (add == null)
            {
                XElement cadenas = doc.Root.Element("connectionStrings");
                if (cadenas == null)
                {
                    cadenas = new XElement("connectionStrings");
                    doc.Root.Add(cadenas);
                }
                add = new XElement("add",
                    new XAttribute("name", NombreCadena),
                    new XAttribute("providerName", "System.Data.SqlClient"));
                cadenas.Add(add);
            }
            add.SetAttributeValue("connectionString", cadena);
            doc.Save(RutaConfig);
        }

        private static XElement BuscarCadena(XDocument doc)
        {
            return doc.Root?.Element("connectionStrings")?.Elements("add")
                .FirstOrDefault(e => (string)e.Attribute("name") == NombreCadena);
        }
    }
}
