using ClosedXML.Excel;
using System;
using System.Windows.Forms;

namespace CAPAS
{
    public static class ExcelExportHelper
    {
        // Exporta las columnas VISIBLES de cualquier DataGridView a un .xlsx elegido por el usuario.
        public static void ExportarDataGridView(DataGridView grilla, string nombreSugerido)
        {
            if (grilla.Rows.Count == 0)
            {
                MsgBox.Show("No hay datos para exportar.", "Atención", MsgBox.Botones.OK, MsgBox.Icono.Atencion);
                return;
            }

            using (SaveFileDialog dialogo = new SaveFileDialog())
            {
                dialogo.Filter = "Archivo de Excel (*.xlsx)|*.xlsx";
                dialogo.FileName = $"{nombreSugerido}_{DateTime.Now:yyyyMMdd_HHmm}.xlsx";

                if (dialogo.ShowDialog() != DialogResult.OK) return;

                try
                {
                    using (var libro = new XLWorkbook())
                    {
                        var hoja = libro.Worksheets.Add("Datos");

                        int col = 1;
                        foreach (DataGridViewColumn columna in grilla.Columns)
                        {
                            if (!columna.Visible) continue;
                            hoja.Cell(1, col).Value = columna.HeaderText;
                            hoja.Cell(1, col).Style.Font.Bold = true;
                            col++;
                        }

                        int fila = 2;
                        foreach (DataGridViewRow filaGrilla in grilla.Rows)
                        {
                            if (filaGrilla.IsNewRow) continue;
                            col = 1;
                            foreach (DataGridViewColumn columna in grilla.Columns)
                            {
                                if (!columna.Visible) continue;
                                object valor = filaGrilla.Cells[columna.Index].Value;
                                hoja.Cell(fila, col).Value = valor?.ToString() ?? "";
                                col++;
                            }
                            fila++;
                        }

                        hoja.Columns().AdjustToContents();
                        libro.SaveAs(dialogo.FileName);
                    }

                    MsgBox.Show("Exportación completada.", "Éxito", MsgBox.Botones.OK, MsgBox.Icono.Exito);
                }
                catch (Exception ex)
                {
                    MsgBox.Show("No se pudo exportar: " + ex.Message, "Error", MsgBox.Botones.OK, MsgBox.Icono.Error);
                }
            }
        }
    }
}