using System;
using System.Collections.Generic;
using System.Drawing;
using System.Globalization;
using System.IO;
using System.Text;

namespace CAPAS
{
    internal class PdfDocumento
    {
        internal const float Ancho = 595.28f;
        internal const float Alto = 841.89f;
        internal const float Margen = 40f;

        private static readonly Encoding WinAnsi = Encoding.GetEncoding(1252);
        private static readonly CultureInfo Inv = CultureInfo.InvariantCulture;

        private readonly List<MemoryStream> _paginas = new List<MemoryStream>();
        private MemoryStream _actual;

        internal float Y { get; set; }

        internal int CantidadPaginas => _paginas.Count;

        internal PdfDocumento()
        {
            NuevaPagina();
        }

        internal void NuevaPagina()
        {
            _actual = new MemoryStream();
            _paginas.Add(_actual);
            Y = Margen;
        }

        internal bool Asegurar(float alto)
        {
            if (Y + alto <= Alto - Margen - 14) return false;
            NuevaPagina();
            return true;
        }

        internal void Texto(float x, float yArriba, string texto, float tam = 10, bool negrita = false, Color? color = null)
        {
            if (string.IsNullOrEmpty(texto)) return;
            Color c = color ?? Color.Black;
            Escribir($"BT {Color3(c)} rg /{(negrita ? "F2" : "F1")} {F(tam)} Tf {F(x)} {F(Alto - yArriba - tam)} Td (");
            byte[] bytes = Codificar(texto);
            _actual.Write(bytes, 0, bytes.Length);
            Escribir(") Tj ET\n");
        }

        internal void TextoDerecha(float xDerecha, float yArriba, string texto, float tam = 10, bool negrita = false, Color? color = null)
        {
            Texto(xDerecha - Medir(texto, tam, negrita), yArriba, texto, tam, negrita, color);
        }

        internal void Rectangulo(float x, float yArriba, float ancho, float alto, Color? relleno, Color? borde = null, float grosor = 0.5f)
        {
            float y = Alto - yArriba - alto;
            if (relleno.HasValue) Escribir($"{Color3(relleno.Value)} rg {F(x)} {F(y)} {F(ancho)} {F(alto)} re f\n");
            if (borde.HasValue) Escribir($"{Color3(borde.Value, true)} RG {F(grosor)} w {F(x)} {F(y)} {F(ancho)} {F(alto)} re S\n");
        }

        internal void Linea(float x1, float y1Arriba, float x2, float y2Arriba, Color color, float grosor = 0.5f)
        {
            Escribir($"{Color3(color, true)} RG {F(grosor)} w {F(x1)} {F(Alto - y1Arriba)} m {F(x2)} {F(Alto - y2Arriba)} l S\n");
        }

        internal static float Medir(string texto, float tam, bool negrita = false)
        {
            if (string.IsNullOrEmpty(texto)) return 0;
            using (var bmp = new Bitmap(1, 1))
            using (var g = Graphics.FromImage(bmp))
            using (var fuente = new Font("Arial", tam, negrita ? FontStyle.Bold : FontStyle.Regular, GraphicsUnit.Point))
            {
                g.PageUnit = GraphicsUnit.Point;
                return g.MeasureString(texto, fuente, int.MaxValue, StringFormat.GenericTypographic).Width;
            }
        }

        internal static List<string> Envolver(string texto, float anchoMax, float tam, bool negrita = false)
        {
            var lineas = new List<string>();
            string actual = "";
            foreach (string palabra in (texto ?? "").Split(' '))
            {
                string prueba = actual.Length == 0 ? palabra : actual + " " + palabra;
                if (actual.Length > 0 && Medir(prueba, tam, negrita) > anchoMax)
                {
                    lineas.Add(actual);
                    actual = palabra;
                }
                else actual = prueba;
            }
            if (actual.Length > 0) lineas.Add(actual);
            return lineas;
        }

        internal static string Recortar(string texto, float anchoMax, float tam, bool negrita = false)
        {
            if (Medir(texto, tam, negrita) <= anchoMax) return texto;
            while (texto.Length > 1 && Medir(texto + "…", tam, negrita) > anchoMax)
                texto = texto.Substring(0, texto.Length - 1);
            return texto + "…";
        }

        internal void Guardar(string ruta)
        {
            File.WriteAllBytes(ruta, Generar());
        }

        internal byte[] Generar()
        {
            var salida = new MemoryStream();
            var posiciones = new List<long>();

            void Objeto(string cuerpo)
            {
                posiciones.Add(salida.Position);
                var b = WinAnsi.GetBytes($"{posiciones.Count} 0 obj\n{cuerpo}\nendobj\n");
                salida.Write(b, 0, b.Length);
            }

            var cabecera = Encoding.ASCII.GetBytes("%PDF-1.4\n%âãÏÓ\n");
            salida.Write(cabecera, 0, cabecera.Length);

            var hijos = new StringBuilder();
            for (int i = 0; i < _paginas.Count; i++)
                hijos.Append(5 + i * 2).Append(" 0 R ");

            Objeto("<< /Type /Catalog /Pages 2 0 R >>");
            Objeto($"<< /Type /Pages /Kids [ {hijos}] /Count {_paginas.Count} >>");
            Objeto("<< /Type /Font /Subtype /Type1 /BaseFont /Helvetica /Encoding /WinAnsiEncoding >>");
            Objeto("<< /Type /Font /Subtype /Type1 /BaseFont /Helvetica-Bold /Encoding /WinAnsiEncoding >>");

            for (int i = 0; i < _paginas.Count; i++)
            {
                int contenido = 6 + i * 2;
                Objeto($"<< /Type /Page /Parent 2 0 R /MediaBox [0 0 {F(Ancho)} {F(Alto)}] " +
                       $"/Resources << /Font << /F1 3 0 R /F2 4 0 R >> >> /Contents {contenido} 0 R >>");

                var pagina = new MemoryStream();
                byte[] cuerpo = _paginas[i].ToArray();
                pagina.Write(cuerpo, 0, cuerpo.Length);
                var pie = WinAnsi.GetBytes($"BT 0.4 0.4 0.4 rg /F1 8 Tf {F(Ancho / 2 - 20)} 20 Td (Pagina {i + 1} de {_paginas.Count}) Tj ET\n");
                pagina.Write(pie, 0, pie.Length);
                byte[] datos = pagina.ToArray();

                posiciones.Add(salida.Position);
                var inicio = WinAnsi.GetBytes($"{posiciones.Count} 0 obj\n<< /Length {datos.Length} >>\nstream\n");
                salida.Write(inicio, 0, inicio.Length);
                salida.Write(datos, 0, datos.Length);
                var fin = WinAnsi.GetBytes("\nendstream\nendobj\n");
                salida.Write(fin, 0, fin.Length);
            }

            long inicioXref = salida.Position;
            var xref = new StringBuilder();
            xref.Append($"xref\n0 {posiciones.Count + 1}\n0000000000 65535 f \n");
            foreach (long p in posiciones) xref.Append(p.ToString("0000000000", Inv)).Append(" 00000 n \n");
            xref.Append($"trailer\n<< /Size {posiciones.Count + 1} /Root 1 0 R >>\nstartxref\n{inicioXref}\n%%EOF\n");
            var bytesXref = Encoding.ASCII.GetBytes(xref.ToString());
            salida.Write(bytesXref, 0, bytesXref.Length);
            return salida.ToArray();
        }

        private void Escribir(string texto)
        {
            byte[] b = WinAnsi.GetBytes(texto);
            _actual.Write(b, 0, b.Length);
        }

        private static byte[] Codificar(string texto)
        {
            var sb = new StringBuilder();
            foreach (char ch in texto)
            {
                if (ch == '(' || ch == ')' || ch == '\\') sb.Append('\\');
                sb.Append(ch);
            }
            return WinAnsi.GetBytes(sb.ToString().Replace("→", "->").Replace("—", "-"));
        }

        private static string F(float valor) => valor.ToString("0.##", Inv);

        private static string Color3(Color c, bool trazo = false) =>
            $"{(c.R / 255f).ToString("0.###", Inv)} {(c.G / 255f).ToString("0.###", Inv)} {(c.B / 255f).ToString("0.###", Inv)}";
    }
}
