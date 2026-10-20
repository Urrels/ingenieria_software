using BLL;
using ClosedXML.Excel;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Windows.Forms;

namespace CAPAS
{
    public static class LiquidacionExcel
    {
        private const string FormatoHoras = "0.00";
        private const string FormatoImporte = "#,##0.00";

        public static void Exportar(List<BE.LiquidacionDia> dias, DateTime desde, DateTime hasta, string estado,
            BE.ParametrosLiquidacion parametros)
        {
            if (dias == null || dias.Count == 0)
            {
                MsgBox.Show(Textos.T("msg_NoHayDatosParaExportar", "No hay datos para exportar."), "Atención",
                    MsgBox.Botones.OK, MsgBox.Icono.Atencion);
                return;
            }

            using (SaveFileDialog dialogo = new SaveFileDialog())
            {
                dialogo.Filter = "Archivo de Excel (*.xlsx)|*.xlsx";
                dialogo.FileName = $"LiquidacionHoras_{desde:yyyyMMdd}_{hasta:yyyyMMdd}.xlsx";
                if (dialogo.ShowDialog() != DialogResult.OK) return;

                try
                {
                    using (var libro = new XLWorkbook())
                    {
                        HojaResumen(libro.Worksheets.Add(Textos.T("xl_Resumen", "Resumen")), dias, desde, hasta, estado);
                        HojaDetalle(libro.Worksheets.Add(Textos.T("xl_Detalle", "Detalle")), dias);
                        HojaCriterios(libro.Worksheets.Add(Textos.T("xl_Criterios", "Criterios")), dias, parametros);
                        libro.FullCalculationOnLoad = true;
                        libro.SaveAs(dialogo.FileName);
                    }
                    MsgBox.Show(Textos.T("msg_ExportacionCompletada", "Exportación completada."), "Éxito",
                        MsgBox.Botones.OK, MsgBox.Icono.Exito);
                }
                catch (Exception ex)
                {
                    MsgBox.Show(Textos.T("msg_NoSePudoExportar", "No se pudo exportar: {0}", ex.Message), "Error",
                        MsgBox.Botones.OK, MsgBox.Icono.Error);
                }
            }
        }

        private static void HojaResumen(IXLWorksheet hoja, List<BE.LiquidacionDia> dias, DateTime desde, DateTime hasta, string estado)
        {
            hoja.Cell(1, 1).Value = Textos.T("xl_TituloLiquidacion", "Liquidación de horas {0:dd/MM/yyyy} al {1:dd/MM/yyyy} ({2})",
                desde, hasta, estado);
            hoja.Cell(1, 1).Style.Font.Bold = true;
            hoja.Cell(1, 1).Style.Font.FontSize = 14;

            string[] encabezados =
            {
                Textos.T("xl_Empleado", "Empleado"),
                Textos.T("xl_HorasTrabajadas", "Horas trabajadas"),
                Textos.T("xl_HorasTrabajadasHhmm", "Trabajadas (hh:mm)"),
                Textos.T("xl_HorasAsignadas", "Horas asignadas"),
                Textos.T("xl_HorasAPagar", "Horas a pagar"),
                Textos.T("xl_HorasAPagarHhmm", "A pagar (hh:mm)"),
                Textos.T("xl_HorasNormales", "Normales"),
                Textos.T("xl_HorasExtra", "Extra"),
                Textos.T("xl_HorasFeriado", "Domingo/feriado"),
                Textos.T("xl_ExcedenteAAprobar", "Excedente a aprobar"),
                Textos.T("xl_Ausencias", "Ausencias"),
                Textos.T("xl_ImporteAPagar", "Importe a pagar")
            };
            for (int c = 0; c < encabezados.Length; c++)
                hoja.Cell(3, c + 1).Value = encabezados[c];
            Encabezado(hoja.Range(3, 1, 3, encabezados.Length));

            var resumen = LiquidacionCalculo.Resumir(dias);
            int fila = 4;
            foreach (var r in resumen)
            {
                hoja.Cell(fila, 1).Value = r.Empleado;
                hoja.Cell(fila, 2).Value = LiquidacionCalculo.Horas(r.MinTrabajados);
                hoja.Cell(fila, 3).Value = LiquidacionCalculo.Hhmm(r.MinTrabajados);
                hoja.Cell(fila, 4).Value = LiquidacionCalculo.Horas(r.MinAsignados);
                hoja.Cell(fila, 5).Value = LiquidacionCalculo.Horas(r.MinPagables);
                hoja.Cell(fila, 6).Value = LiquidacionCalculo.Hhmm(r.MinPagables);
                hoja.Cell(fila, 7).Value = LiquidacionCalculo.Horas(r.MinNormales);
                hoja.Cell(fila, 8).Value = LiquidacionCalculo.Horas(r.MinExtra);
                hoja.Cell(fila, 9).Value = LiquidacionCalculo.Horas(r.MinFeriado);
                hoja.Cell(fila, 10).Value = LiquidacionCalculo.Horas(r.MinExcedentePendiente);
                hoja.Cell(fila, 11).Value = r.Ausencias;
                hoja.Cell(fila, 12).Value = r.Importe;
                fila++;
            }

            int ultima = fila - 1;
            hoja.Cell(fila, 1).Value = Textos.T("xl_Total", "TOTAL");
            foreach (int col in new[] { 2, 4, 5, 7, 8, 9, 10, 11, 12 })
            {
                string letra = XLHelper.GetColumnLetterFromNumber(col);
                hoja.Cell(fila, col).FormulaA1 = $"SUM({letra}4:{letra}{ultima})";
            }
            hoja.Range(fila, 1, fila, encabezados.Length).Style.Font.Bold = true;
            hoja.Range(fila, 1, fila, encabezados.Length).Style.Border.TopBorder = XLBorderStyleValues.Thin;

            foreach (int col in new[] { 2, 4, 5, 7, 8, 9, 10 })
                hoja.Range(4, col, fila, col).Style.NumberFormat.Format = FormatoHoras;
            hoja.Range(4, 12, fila, 12).Style.NumberFormat.Format = FormatoImporte;

            hoja.SheetView.FreezeRows(3);
            hoja.Columns().AdjustToContents();
        }

        private static void HojaDetalle(IXLWorksheet hoja, List<BE.LiquidacionDia> dias)
        {
            string[] encabezados =
            {
                Textos.T("xl_Empleado", "Empleado"),
                Textos.T("xl_Fecha", "Fecha"),
                Textos.T("xl_Ingreso", "Ingreso"),
                Textos.T("xl_Egreso", "Egreso"),
                Textos.T("xl_HorasTrabajadas", "Horas trabajadas"),
                Textos.T("xl_HorasAsignadas", "Horas asignadas"),
                Textos.T("xl_HorasAPagar", "Horas a pagar"),
                Textos.T("xl_HorasNormales", "Normales"),
                Textos.T("xl_HorasExtra", "Extra"),
                Textos.T("xl_HorasFeriado", "Domingo/feriado"),
                Textos.T("xl_Excedente", "Excedente"),
                Textos.T("xl_ExcedenteAprobado", "Excedente aprobado"),
                Textos.T("xl_ValorHora", "Valor hora"),
                Textos.T("xl_Importe", "Importe"),
                Textos.T("xl_Observacion", "Observación")
            };
            for (int c = 0; c < encabezados.Length; c++)
                hoja.Cell(1, c + 1).Value = encabezados[c];
            Encabezado(hoja.Range(1, 1, 1, encabezados.Length));

            string si = Textos.T("xl_Si", "Sí");
            string no = Textos.T("xl_No", "No");
            int fila = 2;
            foreach (var d in dias)
            {
                hoja.Cell(fila, 1).Value = d.Empleado;
                hoja.Cell(fila, 2).Value = d.Fecha.Date;
                hoja.Cell(fila, 3).Value = d.Ingreso.HasValue ? d.Ingreso.Value.ToString(@"hh\:mm") : "";
                hoja.Cell(fila, 4).Value = d.Egreso.HasValue ? d.Egreso.Value.ToString(@"hh\:mm") : "";
                hoja.Cell(fila, 5).Value = LiquidacionCalculo.Horas(d.MinTrabajados);
                hoja.Cell(fila, 6).Value = LiquidacionCalculo.Horas(d.MinAsignados);
                hoja.Cell(fila, 7).Value = LiquidacionCalculo.Horas(d.MinPagables);
                hoja.Cell(fila, 8).Value = LiquidacionCalculo.Horas(d.MinNormales);
                hoja.Cell(fila, 9).Value = LiquidacionCalculo.Horas(d.MinExtra);
                hoja.Cell(fila, 10).Value = LiquidacionCalculo.Horas(d.MinFeriado);
                hoja.Cell(fila, 11).Value = LiquidacionCalculo.Horas(d.MinExcedente);
                hoja.Cell(fila, 12).Value = d.MinExcedente == 0 ? "" : (d.ExcedenteAprobado ? si : no);
                hoja.Cell(fila, 13).Value = d.ValorHora;
                hoja.Cell(fila, 14).Value = d.Importe;
                hoja.Cell(fila, 15).Value = TraducirObservacion(d.Observacion);
                fila++;
            }

            hoja.Range(2, 2, fila - 1, 2).Style.DateFormat.Format = "dd/MM/yyyy";
            hoja.Range(2, 5, fila - 1, 11).Style.NumberFormat.Format = FormatoHoras;
            hoja.Range(2, 13, fila - 1, 14).Style.NumberFormat.Format = FormatoImporte;
            hoja.RangeUsed().SetAutoFilter();
            hoja.SheetView.FreezeRows(1);
            hoja.Columns().AdjustToContents();
        }

        private static void HojaCriterios(IXLWorksheet hoja, List<BE.LiquidacionDia> dias, BE.ParametrosLiquidacion p)
        {
            var primero = dias.First();
            var filas = new List<string[]>
            {
                new[] { Textos.T("xl_RecargoExtra", "Recargo de horas extra (%)"), primero.RecargoExtraPct.ToString("0.##") },
                new[] { Textos.T("xl_RecargoFeriado", "Recargo de domingo/feriado (%)"), primero.RecargoFeriadoPct.ToString("0.##") }
            };
            if (p != null)
            {
                filas.Add(new[] { Textos.T("xl_Tolerancia", "Tolerancia (minutos)"), p.ToleranciaMinutos.ToString() });
                filas.Add(new[] { Textos.T("xl_Redondeo", "Redondeo (minutos)"), p.RedondeoMinutos.ToString() });
                filas.Add(new[] { Textos.T("xl_Jornada", "Jornada normal (horas)"), p.JornadaNormalHoras.ToString("0.##") });
            }
            hoja.Cell(1, 1).Value = Textos.T("xl_CriteriosAplicados", "Criterios aplicados");
            hoja.Cell(1, 1).Style.Font.Bold = true;
            int fila = 3;
            foreach (var f in filas)
            {
                hoja.Cell(fila, 1).Value = f[0];
                hoja.Cell(fila, 2).Value = f[1];
                fila++;
            }
            fila++;
            string[] reglas =
            {
                Textos.T("xl_Regla1", "Las horas a pagar nunca superan las asignadas en la grilla, salvo que el administrador apruebe el excedente."),
                Textos.T("xl_Regla2", "Una diferencia menor o igual a la tolerancia contra lo asignado se toma como exacta."),
                Textos.T("xl_Regla3", "Las horas de cada día se redondean al múltiplo de minutos indicado."),
                Textos.T("xl_Regla4", "Las horas que superan la jornada normal se pagan con recargo; domingos y feriados se pagan con su propio recargo."),
                Textos.T("xl_Regla5", "Los ingresos sin egreso no suman horas hasta que el administrador los corrige.")
            };
            foreach (string regla in reglas)
                hoja.Cell(fila++, 1).Value = regla;
            hoja.Column(1).Width = 60;
            hoja.Column(2).Width = 12;
        }

        private static void Encabezado(IXLRange rango)
        {
            rango.Style.Font.Bold = true;
            rango.Style.Fill.BackgroundColor = XLColor.FromHtml("#E8F0FB");
            rango.Style.Alignment.WrapText = true;
            rango.Style.Alignment.Vertical = XLAlignmentVerticalValues.Center;
        }

        internal static string TraducirObservacion(string observacion)
        {
            if (string.IsNullOrEmpty(observacion)) return observacion;
            var partes = observacion.Split(new[] { "; " }, StringSplitOptions.None).Select(TraducirParte);
            return string.Join("; ", partes);
        }

        private static string TraducirParte(string parte)
        {
            switch (parte)
            {
                case "Fichaje incompleto": return Textos.T("obs_FichajeIncompleto", "Fichaje incompleto");
                case "Ausente": return Textos.T("obs_Ausente", "Ausente");
                case "Sin turno asignado": return Textos.T("obs_SinTurnoAsignado", "Sin turno asignado");
                case "Domingo o feriado": return Textos.T("obs_DomingoOFeriado", "Domingo o feriado");
                case "Fichaje corregido": return Textos.T("obs_FichajeCorregido", "Fichaje corregido");
                case "Sin valor hora": return Textos.T("obs_SinValorHora", "Sin valor hora");
            }
            if (parte.StartsWith("Excedente aprobado "))
                return Textos.T("obs_ExcedenteAprobado", "Excedente aprobado {0}", parte.Substring("Excedente aprobado ".Length));
            if (parte.StartsWith("Excedente a aprobar "))
                return Textos.T("obs_ExcedenteAAprobar", "Excedente a aprobar {0}", parte.Substring("Excedente a aprobar ".Length));
            return parte;
        }
    }
}
