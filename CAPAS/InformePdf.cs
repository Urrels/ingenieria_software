using System;
using System.Collections.Generic;
using System.Drawing;
using System.Globalization;
using System.Linq;

namespace CAPAS
{
    // Dibuja el informe de gestión en un PDF A4: resumen, conclusiones, horas, cobertura y mantenimiento.
    internal static class InformePdf
    {
        private static readonly CultureInfo Es = new CultureInfo("es-AR");
        private const float ContenidoAncho = PdfDocumento.Ancho - 2 * PdfDocumento.Margen;

        internal static void Guardar(BE.InformeGestion i, string ruta)
        {
            var pdf = Crear(i);
            pdf.Guardar(ruta);
        }

        internal static PdfDocumento Crear(BE.InformeGestion i)
        {
            var pdf = new PdfDocumento();
            Encabezado(pdf, i);
            Resumen(pdf, i);
            Conclusiones(pdf, i);
            Horas(pdf, i);
            Cobertura(pdf, i);
            Mantenimiento(pdf, i);
            return pdf;
        }

        // ---------- Secciones ----------

        private static void Encabezado(PdfDocumento pdf, BE.InformeGestion i)
        {
            pdf.Rectangulo(0, 0, PdfDocumento.Ancho, 78, AppTheme.Acento);
            pdf.Texto(PdfDocumento.Margen, 16, Textos.T("pdf_Titulo", "Informe de gestión operativa"), 19, true, Color.White);
            pdf.Texto(PdfDocumento.Margen, 42, Textos.T("pdf_Periodo", "Período: {0:dd/MM/yyyy} al {1:dd/MM/yyyy}", i.Desde, i.Hasta), 10.5f, false, Color.White);
            pdf.Texto(PdfDocumento.Margen, 57, Textos.T("pdf_Generado", "Generado el {0:dd/MM/yyyy HH:mm} por {1}", i.GeneradoEn, i.GeneradoPor ?? "-"), 8.5f, false, Color.White);
            pdf.Y = 96;
        }

        private static void Resumen(PdfDocumento pdf, BE.InformeGestion i)
        {
            Seccion(pdf, Textos.T("pdf_ResumenEjecutivo", "Resumen ejecutivo"));
            var tarjetas = new[]
            {
                new[] { Textos.T("pdf_HorasTrabajadas", "Horas trabajadas"), N(i.HorasTrabajadas) + " h" },
                new[] { Textos.T("pdf_HorasAsignadas", "Horas planificadas"), N(i.HorasAsignadas) + " h" },
                new[] { Textos.T("pdf_Cumplimiento", "Cumplimiento"), N(i.CumplimientoPct) + " %" },
                new[] { Textos.T("pdf_CostoTotal", "Costo de horas"), "$ " + M(i.CostoTotal) },
                new[] { Textos.T("pdf_HorasConRecargo", "Horas con recargo"), N(i.ExtraPct) + " %" },
                new[] { Textos.T("pdf_Ausentismo", "Ausentismo"), N(i.AusentismoPct) + " %" },
                new[] { Textos.T("pdf_SociosAtendidos", "Socios atendidos"), i.SociosAtendidos.ToString("N0", Es) },
                new[] { Textos.T("pdf_CostoPorSocio", "Costo por socio"), "$ " + M(i.CostoPorSocio) }
            };
            float ancho = (ContenidoAncho - 3 * 8) / 4f;
            pdf.Asegurar(2 * 52);
            for (int k = 0; k < tarjetas.Length; k++)
            {
                float x = PdfDocumento.Margen + (k % 4) * (ancho + 8);
                float y = pdf.Y + (k / 4) * 58;
                pdf.Rectangulo(x, y, ancho, 50, AppTheme.FondoHeader, AppTheme.Borde);
                pdf.Texto(x + 8, y + 8, tarjetas[k][0], 8, false, AppTheme.TextoSecundario);
                pdf.Texto(x + 8, y + 25, tarjetas[k][1], 14, true, AppTheme.TextoEncabezado);
            }
            pdf.Y += 2 * 58 + 6;
        }

        private static void Conclusiones(PdfDocumento pdf, BE.InformeGestion i)
        {
            Seccion(pdf, Textos.T("pdf_Conclusiones", "Conclusiones y recomendaciones"));
            foreach (var c in i.Conclusiones.OrderBy(c => Orden(c.Prioridad)))
            {
                var lineas = PdfDocumento.Envolver(c.Texto, ContenidoAncho - 116, 9);
                float alto = Math.Max(14, lineas.Count * 12 + 4);
                pdf.Asegurar(alto);
                Color color = c.Prioridad == "Alta" ? AppTheme.ResultadoFalta : c.Prioridad == "Media" ? AppTheme.Acento : AppTheme.TextoSecundario;
                pdf.Rectangulo(PdfDocumento.Margen, pdf.Y, 104, 13, color);
                pdf.Texto(PdfDocumento.Margen + 4, pdf.Y + 2, PdfDocumento.Recortar(Textos.T("pdf_Prioridad_" + c.Prioridad, c.Prioridad) + " · " + Textos.T("pdf_Area_" + c.Area, c.Area), 98, 7, true), 7, true, Color.White);
                float y = pdf.Y;
                foreach (string linea in lineas)
                {
                    pdf.Texto(PdfDocumento.Margen + 112, y, linea, 9, false, AppTheme.TextoPrincipal);
                    y += 12;
                }
                pdf.Y += alto + 3;
            }
            pdf.Y += 4;
        }

        private static void Horas(PdfDocumento pdf, BE.InformeGestion i)
        {
            Seccion(pdf, Textos.T("pdf_HorasPorEmpleado", "Horas y costo por empleado"));
            Tabla(pdf,
                new[] { Textos.T("pdf_Empleado", "Empleado"), Textos.T("pdf_Trabajadas", "Trabajadas"), Textos.T("pdf_Planificadas", "Planificadas"),
                        Textos.T("pdf_APagar", "A pagar"), Textos.T("pdf_ConRecargo", "Con recargo"), Textos.T("pdf_Ausencias", "Ausencias"), Textos.T("pdf_Importe", "Importe") },
                new float[] { 150, 62, 70, 58, 62, 52, 61 },
                i.Empleados.Select(e => new[] { e.Empleado, N(e.HorasTrabajadas), N(e.HorasAsignadas), N(e.HorasAPagar), N(e.HorasExtra), e.Ausencias.ToString(), "$ " + M(e.Importe) }).ToList());

            if (i.Empleados.Count == 0) return;
            pdf.Y += 6;
            Leyenda(pdf, new[] { Tuple.Create(Textos.T("pdf_Planificadas", "Planificadas"), AppTheme.Borde), Tuple.Create(Textos.T("pdf_Trabajadas", "Trabajadas"), AppTheme.Acento) });
            decimal maximo = Math.Max(1, i.Empleados.Max(e => Math.Max(e.HorasAsignadas, e.HorasTrabajadas)));
            float zonaBarras = ContenidoAncho - 150;
            foreach (var e in i.Empleados)
            {
                pdf.Asegurar(22);
                pdf.Texto(PdfDocumento.Margen, pdf.Y + 2, PdfDocumento.Recortar(e.Empleado, 140, 8.5f), 8.5f, false, AppTheme.TextoPrincipal);
                pdf.Rectangulo(PdfDocumento.Margen + 148, pdf.Y, (float)(e.HorasAsignadas / maximo) * zonaBarras, 8, AppTheme.Borde);
                pdf.Rectangulo(PdfDocumento.Margen + 148, pdf.Y + 10, (float)(e.HorasTrabajadas / maximo) * zonaBarras, 8, AppTheme.Acento);
                pdf.Y += 22;
            }
            pdf.Y += 8;
        }

        private static void Cobertura(PdfDocumento pdf, BE.InformeGestion i)
        {
            Seccion(pdf, Textos.T("pdf_CoberturaPorFranja", "Cobertura del servicio por franja"));
            if (i.Cobertura.Count == 0)
            {
                Parrafo(pdf, Textos.T("pdf_SinEvaluaciones", "No hay evaluaciones de cobertura en el período. Evaluá las semanas desde Asistencia > Evaluar cobertura."));
                return;
            }
            Tabla(pdf,
                new[] { Textos.T("pdf_Franja", "Franja"), Textos.T("pdf_Rol", "Rol"), Textos.T("pdf_Semanas", "Semanas"), Textos.T("pdf_Falta", "Falta"),
                        Textos.T("pdf_Ajustado", "Ajustado"), Textos.T("pdf_Exceso", "Exceso"), Textos.T("pdf_SociosProm", "Socios prom.") },
                new float[] { 130, 70, 52, 48, 58, 52, 95 },
                i.Cobertura.Select(c => new[] { c.Franja, c.Rol, c.Semanas.ToString(), c.Faltas.ToString(), c.Ajustadas.ToString(), c.Excesos.ToString(), c.SociosPromedio.ToString() }).ToList());

            pdf.Y += 6;
            Leyenda(pdf, new[] { Tuple.Create(Textos.T("pdf_Falta", "Falta"), AppTheme.ResultadoFalta), Tuple.Create(Textos.T("pdf_Ajustado", "Ajustado"), AppTheme.ResultadoAjustado), Tuple.Create(Textos.T("pdf_Exceso", "Exceso"), AppTheme.ResultadoExceso) });
            float zona = ContenidoAncho - 150;
            foreach (var c in i.Cobertura)
            {
                pdf.Asegurar(16);
                pdf.Texto(PdfDocumento.Margen, pdf.Y + 1, PdfDocumento.Recortar(c.Franja, 145, 8.5f), 8.5f, false, AppTheme.TextoPrincipal);
                float x = PdfDocumento.Margen + 148;
                float unidad = c.Semanas == 0 ? 0 : zona / c.Semanas;
                foreach (var tramo in new[] { Tuple.Create(c.Faltas, AppTheme.ResultadoFalta), Tuple.Create(c.Ajustadas, AppTheme.ResultadoAjustado), Tuple.Create(c.Excesos, AppTheme.ResultadoExceso) })
                {
                    pdf.Rectangulo(x, pdf.Y, tramo.Item1 * unidad, 11, tramo.Item2);
                    x += tramo.Item1 * unidad;
                }
                pdf.Y += 16;
            }
            pdf.Y += 8;
        }

        private static void Mantenimiento(PdfDocumento pdf, BE.InformeGestion i)
        {
            Seccion(pdf, Textos.T("pdf_Mantenimiento", "Mantenimiento de equipamiento"));
            Parrafo(pdf, Textos.T("pdf_AlertasPendientes", "Alertas de revisión pendientes de autorizar: {0}.", i.AlertasPendientes));
            if (i.Equipos.Count == 0) return;
            Tabla(pdf,
                new[] { Textos.T("pdf_Equipo", "Equipo"), Textos.T("pdf_Estado", "Estado"), Textos.T("pdf_Uso", "Uso"), Textos.T("pdf_NivelCritico", "Nivel crítico"), Textos.T("pdf_DiasHastaCritico", "Días hasta el crítico") },
                new float[] { 150, 80, 60, 80, 105 },
                i.Equipos.Select(e => new[] { e.Nombre, e.Estado, e.Uso.ToString(), e.Critico.ToString(), e.DiasHastaCritico.HasValue ? e.DiasHastaCritico.Value.ToString() : "-" }).ToList());

            pdf.Y += 6;
            float zona = ContenidoAncho - 150;
            foreach (var e in i.Equipos)
            {
                pdf.Asegurar(16);
                pdf.Texto(PdfDocumento.Margen, pdf.Y + 1, PdfDocumento.Recortar(e.Nombre, 145, 8.5f), 8.5f, false, AppTheme.TextoPrincipal);
                float x = PdfDocumento.Margen + 148;
                pdf.Rectangulo(x, pdf.Y, zona, 11, AppTheme.FondoHeader);
                pdf.Rectangulo(x, pdf.Y, Math.Min(1f, e.UsoPct / 100f) * zona, 11, e.UsoPct >= 100 ? AppTheme.ResultadoFalta : e.UsoPct >= 90 ? AppTheme.Acento : AppTheme.ResultadoAjustado);
                pdf.TextoDerecha(x + zona - 4, pdf.Y + 1.5f, e.UsoPct + " %", 7.5f, true, AppTheme.TextoPrincipal);
                pdf.Y += 16;
            }
        }

        // ---------- Componentes ----------

        private static void Seccion(PdfDocumento pdf, string titulo)
        {
            pdf.Asegurar(40);
            pdf.Y += 6;
            pdf.Texto(PdfDocumento.Margen, pdf.Y, titulo, 12.5f, true, AppTheme.TextoEncabezado);
            pdf.Linea(PdfDocumento.Margen, pdf.Y + 18, PdfDocumento.Ancho - PdfDocumento.Margen, pdf.Y + 18, AppTheme.Acento, 1f);
            pdf.Y += 26;
        }

        private static void Parrafo(PdfDocumento pdf, string texto)
        {
            foreach (string linea in PdfDocumento.Envolver(texto, ContenidoAncho, 9))
            {
                pdf.Asegurar(13);
                pdf.Texto(PdfDocumento.Margen, pdf.Y, linea, 9, false, AppTheme.TextoPrincipal);
                pdf.Y += 13;
            }
            pdf.Y += 4;
        }

        private static void Tabla(PdfDocumento pdf, string[] encabezados, float[] anchos, List<string[]> filas)
        {
            float escala = ContenidoAncho / anchos.Sum();
            anchos = anchos.Select(a => a * escala).ToArray();

            void Cabecera()
            {
                pdf.Rectangulo(PdfDocumento.Margen, pdf.Y, ContenidoAncho, 16, AppTheme.FondoHeader);
                float xh = PdfDocumento.Margen;
                for (int c = 0; c < encabezados.Length; c++)
                {
                    if (c == 0) pdf.Texto(xh + 4, pdf.Y + 3, encabezados[c], 8, true, AppTheme.TextoEncabezado);
                    else pdf.TextoDerecha(xh + anchos[c] - 4, pdf.Y + 3, encabezados[c], 8, true, AppTheme.TextoEncabezado);
                    xh += anchos[c];
                }
                pdf.Y += 16;
            }

            pdf.Asegurar(16 + 15);
            Cabecera();
            bool alterna = false;
            foreach (var fila in filas)
            {
                if (pdf.Asegurar(15)) Cabecera();
                if (alterna) pdf.Rectangulo(PdfDocumento.Margen, pdf.Y, ContenidoAncho, 15, AppTheme.FondoGrillaAlt);
                alterna = !alterna;
                float x = PdfDocumento.Margen;
                for (int c = 0; c < fila.Length; c++)
                {
                    if (c == 0) pdf.Texto(x + 4, pdf.Y + 3, PdfDocumento.Recortar(fila[c], anchos[c] - 6, 8.5f), 8.5f, false, AppTheme.TextoPrincipal);
                    else pdf.TextoDerecha(x + anchos[c] - 4, pdf.Y + 3, fila[c], 8.5f, false, AppTheme.TextoPrincipal);
                    x += anchos[c];
                }
                pdf.Y += 15;
            }
            pdf.Linea(PdfDocumento.Margen, pdf.Y, PdfDocumento.Ancho - PdfDocumento.Margen, pdf.Y, AppTheme.Borde);
        }

        private static void Leyenda(PdfDocumento pdf, IEnumerable<Tuple<string, Color>> items)
        {
            pdf.Asegurar(18);
            float x = PdfDocumento.Margen;
            foreach (var item in items)
            {
                pdf.Rectangulo(x, pdf.Y + 1, 9, 9, item.Item2);
                pdf.Texto(x + 13, pdf.Y, item.Item1, 8, false, AppTheme.TextoSecundario);
                x += 13 + PdfDocumento.Medir(item.Item1, 8) + 16;
            }
            pdf.Y += 16;
        }

        private static int Orden(string prioridad) => prioridad == "Alta" ? 0 : prioridad == "Media" ? 1 : 2;

        private static string N(decimal valor) => valor.ToString("0.##", Es);

        private static string M(decimal valor) => valor.ToString("N2", Es);
    }
}
