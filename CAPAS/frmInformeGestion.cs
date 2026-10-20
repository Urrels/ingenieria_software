using System;
using System.Diagnostics;
using System.Globalization;
using System.Linq;
using System.Windows.Forms;

namespace CAPAS
{
    public class ConclusionFilaVM
    {
        public string Prioridad { get; set; }
        public string Area { get; set; }
        public string Conclusion { get; set; }
    }

    public class InformeEmpleadoFilaVM
    {
        public string Empleado { get; set; }
        public decimal Trabajadas { get; set; }
        public decimal Planificadas { get; set; }
        public decimal APagar { get; set; }
        public decimal ConRecargo { get; set; }
        public int Ausencias { get; set; }
        public decimal Importe { get; set; }
    }

    public class InformeCoberturaFilaVM
    {
        public string Franja { get; set; }
        public string Rol { get; set; }
        public int Semanas { get; set; }
        public int Falta { get; set; }
        public int Ajustado { get; set; }
        public int Exceso { get; set; }
        public int SociosPromedio { get; set; }
    }

    public class InformeEquipoFilaVM
    {
        public string Equipo { get; set; }
        public string Estado { get; set; }
        public int Uso { get; set; }
        public int NivelCritico { get; set; }
        public string UsoPct { get; set; }
        public string DiasHastaCritico { get; set; }
    }

    public partial class frmInformeGestion : FormBase
    {
        private static readonly CultureInfo Es = new CultureInfo("es-AR");

        private readonly BLL.InformeGestionBLL _informeBLL = new BLL.InformeGestionBLL();
        private BE.InformeGestion _informe;

        public frmInformeGestion()
        {
            InitializeComponent();
        }

        private void frmInformeGestion_Load(object sender, EventArgs e)
        {
            DateTime hoy = DateTime.Today;
            dtpHasta.Value = hoy;
            dtpDesde.Value = hoy.AddDays(-27);
            ActualizarBotones();

            InicializarFormulario();
        }

        protected override void AjustarClaves()
        {
            ExcluirDeTraduccion(dtpDesde, dtpHasta, lblResumen);
        }

        private void btnGenerar_Click(object sender, EventArgs e)
        {
            try
            {
                Cursor = Cursors.WaitCursor;
                _informe = _informeBLL.Generar(dtpDesde.Value.Date, dtpHasta.Value.Date);
            }
            catch (InvalidOperationException ex)
            {
                MsgBox.Show(ex.Message, "Atención", MsgBox.Botones.OK, MsgBox.Icono.Atencion);
                return;
            }
            finally
            {
                Cursor = Cursors.Default;
            }
            Mostrar();
        }

        private void Mostrar()
        {
            var i = _informe;
            lblResumen.Text = Textos.T("lbl_ResumenInforme",
                "Horas trabajadas: {0} de {1} planificadas ({2}%)  ·  Costo: $ {3}  ·  Con recargo: {4}%  ·  Ausentismo: {5}%  ·  Socios: {6}  ·  Costo por socio: $ {7}",
                i.HorasTrabajadas.ToString("0.##", Es), i.HorasAsignadas.ToString("0.##", Es), i.CumplimientoPct.ToString("0.#", Es),
                i.CostoTotal.ToString("N2", Es), i.ExtraPct.ToString("0.#", Es), i.AusentismoPct.ToString("0.#", Es),
                i.SociosAtendidos.ToString("N0", Es), i.CostoPorSocio.ToString("N2", Es));

            dgvConclusiones.DataSource = i.Conclusiones.Select(c => new ConclusionFilaVM
            {
                Prioridad = Textos.T("pdf_Prioridad_" + c.Prioridad, c.Prioridad),
                Area = Textos.T("pdf_Area_" + c.Area, c.Area),
                Conclusion = c.Texto
            }).ToList();

            dgvEmpleados.DataSource = i.Empleados.Select(x => new InformeEmpleadoFilaVM
            {
                Empleado = x.Empleado, Trabajadas = x.HorasTrabajadas, Planificadas = x.HorasAsignadas, APagar = x.HorasAPagar,
                ConRecargo = x.HorasExtra, Ausencias = x.Ausencias, Importe = x.Importe
            }).ToList();

            dgvCobertura.DataSource = i.Cobertura.Select(x => new InformeCoberturaFilaVM
            {
                Franja = x.Franja, Rol = x.Rol, Semanas = x.Semanas, Falta = x.Faltas, Ajustado = x.Ajustadas,
                Exceso = x.Excesos, SociosPromedio = x.SociosPromedio
            }).ToList();

            dgvEquipos.DataSource = i.Equipos.Select(x => new InformeEquipoFilaVM
            {
                Equipo = x.Nombre, Estado = x.Estado, Uso = x.Uso, NivelCritico = x.Critico,
                UsoPct = x.Critico > 0 ? x.UsoPct + " %" : "-",
                DiasHastaCritico = x.DiasHastaCritico.HasValue ? x.DiasHastaCritico.Value.ToString() : "-"
            }).ToList();

            ActualizarEncabezados();
            ActualizarBotones();
        }

        private void ActualizarBotones()
        {
            btnPdf.Enabled = _informe != null;
            btnJson.Enabled = _informe != null;
        }

        private void btnPdf_Click(object sender, EventArgs e)
        {
            if (_informe == null) return;
            using (var dialogo = new SaveFileDialog())
            {
                dialogo.Filter = "Documento PDF (*.pdf)|*.pdf";
                dialogo.FileName = $"InformeGestion_{_informe.Desde:yyyyMMdd}_{_informe.Hasta:yyyyMMdd}.pdf";
                if (dialogo.ShowDialog(this) != DialogResult.OK) return;

                try
                {
                    InformePdf.Guardar(_informe, dialogo.FileName);
                    _informeBLL.RegistrarExportacion("PDF");
                }
                catch (Exception ex) when (ex is System.IO.IOException || ex is UnauthorizedAccessException)
                {
                    MsgBox.Show(Textos.T("msg_NoSePudoExportar", "No se pudo exportar: {0}", ex.Message), "Error",
                        MsgBox.Botones.OK, MsgBox.Icono.Error);
                    return;
                }

                if (MsgBox.Show(Textos.T("msg_InformeExportadoAbrir", "Informe exportado. ¿Querés abrirlo ahora?"), "Éxito",
                        MsgBox.Botones.SiNo, MsgBox.Icono.Pregunta) == DialogResult.Yes)
                    Process.Start(new ProcessStartInfo(dialogo.FileName) { UseShellExecute = true });
            }
        }

        private void btnJson_Click(object sender, EventArgs e)
        {
            if (_informe == null) return;
            using (var dialogo = new SaveFileDialog())
            {
                dialogo.Filter = "Datos del informe (*.json)|*.json";
                dialogo.FileName = $"InformeGestion_{_informe.Desde:yyyyMMdd}_{_informe.Hasta:yyyyMMdd}.json";
                if (dialogo.ShowDialog(this) != DialogResult.OK) return;

                try
                {
                    BLL.InformeGestionBLL.GuardarJson(_informe, dialogo.FileName);
                    _informeBLL.RegistrarExportacion("JSON");
                    MsgBox.Show(Textos.T("msg_ExportacionCompletada", "Exportación completada."), "Éxito", MsgBox.Botones.OK, MsgBox.Icono.Exito);
                }
                catch (Exception ex) when (ex is System.IO.IOException || ex is UnauthorizedAccessException)
                {
                    MsgBox.Show(Textos.T("msg_NoSePudoExportar", "No se pudo exportar: {0}", ex.Message), "Error",
                        MsgBox.Botones.OK, MsgBox.Icono.Error);
                }
            }
        }

        private void btnCerrar_Click(object sender, EventArgs e)
        {
            this.Close();
        }

        private void ActualizarEncabezados()
        {
            Encabezado(dgvConclusiones, "Prioridad", "hdr_Prioridad", "Prioridad");
            Encabezado(dgvConclusiones, "Area", "hdr_Area", "Área");
            Encabezado(dgvConclusiones, "Conclusion", "hdr_Conclusion", "Conclusión");
            if (dgvConclusiones.Columns["Conclusion"] != null) dgvConclusiones.Columns["Conclusion"].FillWeight = 400;

            Encabezado(dgvEmpleados, "Empleado", "hdr_Empleado", "Empleado");
            Encabezado(dgvEmpleados, "Trabajadas", "hdr_HorasTrabajadas", "Trabajadas");
            Encabezado(dgvEmpleados, "Planificadas", "hdr_Planificadas", "Planificadas");
            Encabezado(dgvEmpleados, "APagar", "hdr_HorasAPagar", "A pagar");
            Encabezado(dgvEmpleados, "ConRecargo", "hdr_ConRecargo", "Con recargo");
            Encabezado(dgvEmpleados, "Ausencias", "hdr_Ausencias", "Ausencias");
            Encabezado(dgvEmpleados, "Importe", "hdr_Importe", "Importe");
            if (dgvEmpleados.Columns["Importe"] != null) dgvEmpleados.Columns["Importe"].DefaultCellStyle.Format = "N2";

            Encabezado(dgvCobertura, "Franja", "hdr_Franja", "Franja");
            Encabezado(dgvCobertura, "Rol", "hdr_Rol", "Rol");
            Encabezado(dgvCobertura, "Semanas", "hdr_Semanas", "Semanas");
            Encabezado(dgvCobertura, "Falta", "hdr_Falta", "Falta");
            Encabezado(dgvCobertura, "Ajustado", "hdr_Ajustado", "Ajustado");
            Encabezado(dgvCobertura, "Exceso", "hdr_Exceso", "Exceso");
            Encabezado(dgvCobertura, "SociosPromedio", "hdr_SociosPromedio", "Socios promedio");

            Encabezado(dgvEquipos, "Equipo", "hdr_Equipo", "Equipo");
            Encabezado(dgvEquipos, "Estado", "hdr_Estado", "Estado");
            Encabezado(dgvEquipos, "Uso", "hdr_Uso", "Uso");
            Encabezado(dgvEquipos, "NivelCritico", "hdr_NivelCritico", "Nivel crítico");
            Encabezado(dgvEquipos, "UsoPct", "hdr_UsoPct", "Uso / crítico");
            Encabezado(dgvEquipos, "DiasHastaCritico", "hdr_DiasHastaCritico", "Días hasta el crítico");
        }

        protected override void ActualizarTextosDinamicos()
        {
            ActualizarEncabezados();
            if (_informe != null) Mostrar();
        }
    }
}
