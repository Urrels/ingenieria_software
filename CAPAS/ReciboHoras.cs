using BLL;
using System;
using System.Collections.Generic;
using System.Drawing;
using System.Drawing.Printing;
using System.Linq;
using System.Windows.Forms;

namespace CAPAS
{
    // Recibo de horas de un empleado: vista previa e impresión (también sirve para guardar en PDF con la impresora de Windows).
    public class ReciboHoras
    {
        private readonly List<BE.LiquidacionDia> _dias;
        private readonly DateTime _desde;
        private readonly DateTime _hasta;
        private readonly string _estado;
        private int _siguienteFila;

        private ReciboHoras(List<BE.LiquidacionDia> dias, DateTime desde, DateTime hasta, string estado)
        {
            _dias = dias.OrderBy(d => d.Fecha).ToList();
            _desde = desde;
            _hasta = hasta;
            _estado = estado;
        }

        public static void Mostrar(IWin32Window padre, List<BE.LiquidacionDia> diasEmpleado, DateTime desde, DateTime hasta, string estado)
        {
            if (diasEmpleado == null || diasEmpleado.Count == 0) return;
            var recibo = new ReciboHoras(diasEmpleado, desde, hasta, estado);
            using (var documento = new PrintDocument())
            using (var vista = new PrintPreviewDialog())
            {
                documento.DocumentName = Textos.T("lbl_ReciboDeHoras", "Recibo de horas");
                documento.BeginPrint += (s, e) => recibo._siguienteFila = 0;
                documento.PrintPage += recibo.ImprimirPagina;
                vista.Document = documento;
                vista.Width = 900;
                vista.Height = 700;
                vista.ShowDialog(padre);
            }
        }

        private void ImprimirPagina(object sender, PrintPageEventArgs e)
        {
            Graphics g = e.Graphics;
            Rectangle m = e.MarginBounds;
            using (Font titulo = new Font("Segoe UI", 16, FontStyle.Bold))
            using (Font normal = new Font("Segoe UI", 9))
            using (Font negrita = new Font("Segoe UI", 9, FontStyle.Bold))
            {
                int y = m.Top;
                if (_siguienteFila == 0)
                {
                    g.DrawString(Textos.T("lbl_ReciboDeHoras", "Recibo de horas"), titulo, Brushes.Black, m.Left, y);
                    y += 40;
                    g.DrawString(Textos.T("lbl_ReciboEmpleado", "Empleado: {0}", _dias[0].Empleado), negrita, Brushes.Black, m.Left, y);
                    y += 20;
                    g.DrawString(Textos.T("lbl_ReciboPeriodo", "Período: {0:dd/MM/yyyy} al {1:dd/MM/yyyy} ({2})", _desde, _hasta, _estado),
                        normal, Brushes.Black, m.Left, y);
                    y += 35;
                }

                int[] x = { m.Left, m.Left + 110, m.Left + 175, m.Left + 240, m.Left + 305, m.Left + 370, m.Left + 440, m.Left + 530 };
                string[] cabecera =
                {
                    Textos.T("hdr_Fecha", "Fecha"), Textos.T("lbl_ReciboTrabajadas", "Trab."), Textos.T("lbl_ReciboPagables", "A pagar"),
                    Textos.T("lbl_ReciboNormales", "Normal"), Textos.T("lbl_ReciboExtra", "Extra"), Textos.T("lbl_ReciboFeriado", "Dom/fer."),
                    Textos.T("lbl_ReciboValorHora", "Valor hora"), Textos.T("lbl_ReciboImporte", "Importe")
                };
                for (int i = 0; i < cabecera.Length; i++)
                    g.DrawString(cabecera[i], negrita, Brushes.Black, x[i], y);
                y += 20;
                g.DrawLine(Pens.Black, m.Left, y - 3, m.Right, y - 3);

                int filasPorPagina = (m.Bottom - 150 - y) / 18;
                int impresas = 0;
                while (_siguienteFila < _dias.Count && impresas < filasPorPagina)
                {
                    var d = _dias[_siguienteFila];
                    string[] celdas =
                    {
                        d.Fecha.ToString("ddd dd/MM"), LiquidacionCalculo.Hhmm(d.MinTrabajados), LiquidacionCalculo.Hhmm(d.MinPagables),
                        LiquidacionCalculo.Hhmm(d.MinNormales), LiquidacionCalculo.Hhmm(d.MinExtra), LiquidacionCalculo.Hhmm(d.MinFeriado),
                        d.ValorHora.ToString("N2"), d.Importe.ToString("N2")
                    };
                    for (int i = 0; i < celdas.Length; i++)
                        g.DrawString(celdas[i], normal, Brushes.Black, x[i], y);
                    y += 18;
                    _siguienteFila++;
                    impresas++;
                }

                if (_siguienteFila < _dias.Count)
                {
                    e.HasMorePages = true;
                    return;
                }

                g.DrawLine(Pens.Black, m.Left, y + 2, m.Right, y + 2);
                y += 8;
                var resumen = LiquidacionCalculo.Resumir(_dias).First();
                g.DrawString(Textos.T("lbl_ReciboTotalHoras", "Total de horas a pagar: {0}", LiquidacionCalculo.Hhmm(resumen.MinPagables)),
                    negrita, Brushes.Black, m.Left, y);
                y += 20;
                g.DrawString(Textos.T("lbl_ReciboTotalImporte", "Importe total: {0}", resumen.Importe.ToString("N2")),
                    negrita, Brushes.Black, m.Left, y);
                if (resumen.MinExcedentePendiente > 0)
                {
                    y += 20;
                    g.DrawString(Textos.T("lbl_ReciboExcedentePendiente", "Horas excedentes sin aprobar (no incluidas): {0}",
                        LiquidacionCalculo.Hhmm(resumen.MinExcedentePendiente)), normal, Brushes.Black, m.Left, y);
                }

                int yFirma = Math.Max(y + 70, m.Bottom - 60);
                g.DrawLine(Pens.Black, m.Left, yFirma, m.Left + 220, yFirma);
                g.DrawLine(Pens.Black, m.Right - 220, yFirma, m.Right, yFirma);
                g.DrawString(Textos.T("lbl_ReciboFirmaEmpleado", "Firma del empleado"), normal, Brushes.Black, m.Left, yFirma + 4);
                g.DrawString(Textos.T("lbl_ReciboFirmaResponsable", "Firma del responsable"), normal, Brushes.Black, m.Right - 220, yFirma + 4);

                e.HasMorePages = false;
                _siguienteFila = 0;
            }
        }
    }
}
