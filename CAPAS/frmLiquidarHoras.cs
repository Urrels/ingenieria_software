using BLL;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Windows.Forms;

namespace CAPAS
{
    public class LiquidacionResumenFilaVM
    {
        public string Empleado { get; set; }
        public string Trabajadas { get; set; }
        public string Asignadas { get; set; }
        public string Pagables { get; set; }
        public string Normales { get; set; }
        public string Extra { get; set; }
        public string Feriado { get; set; }
        public string ExcedenteAAprobar { get; set; }
        public int Ausencias { get; set; }
        public decimal Importe { get; set; }
    }

    public class LiquidacionDetalleFilaVM
    {
        public string Fecha { get; set; }
        public string Ingreso { get; set; }
        public string Egreso { get; set; }
        public string Trabajadas { get; set; }
        public string Asignadas { get; set; }
        public string Pagables { get; set; }
        public string Normales { get; set; }
        public string Extra { get; set; }
        public string Feriado { get; set; }
        public string Excedente { get; set; }
        public decimal ValorHora { get; set; }
        public decimal Importe { get; set; }
        public string Observacion { get; set; }
    }

    public partial class frmLiquidarHoras : FormBase
    {
        private readonly LiquidacionBLL _liquidacionBLL = new LiquidacionBLL();

        private List<BE.LiquidacionDia> _dias = new List<BE.LiquidacionDia>();
        private List<BE.LiquidacionResumenEmpleado> _resumen = new List<BE.LiquidacionResumenEmpleado>();
        private List<BE.LiquidacionDia> _detalle = new List<BE.LiquidacionDia>();
        private BE.Liquidacion _liquidacionCerrada;
        private bool _cargando;

        public frmLiquidarHoras()
        {
            InitializeComponent();
        }

        private static BE.USUARIO Actor => SeguridadYServicios.SessionManager.getInstance().getUsuario();

        private void frmLiquidarHoras_Load(object sender, EventArgs e)
        {
            DateTime primeroDeEsteMes = new DateTime(DateTime.Today.Year, DateTime.Today.Month, 1);
            dtpDesde.Value = primeroDeEsteMes.AddMonths(-1);
            dtpHasta.Value = primeroDeEsteMes.AddDays(-1);

            CargarLiquidaciones(null);
            ActualizarBotones();

            InicializarFormulario();
        }

        protected override void AjustarClaves()
        {
            ExcluirDeTraduccion(dtpDesde, dtpHasta, cboLiquidaciones, lblEstado, lblAdvertencias);
        }

        private void CargarLiquidaciones(int? seleccionarId)
        {
            _cargando = true;
            var lista = _liquidacionBLL.ListarLiquidaciones();
            cboLiquidaciones.DataSource = null;
            cboLiquidaciones.DataSource = lista;
            if (seleccionarId.HasValue)
                cboLiquidaciones.SelectedItem = lista.FirstOrDefault(l => l.Id == seleccionarId.Value);
            else
                cboLiquidaciones.SelectedIndex = -1;
            _cargando = false;
        }

        private void btnCalcular_Click(object sender, EventArgs e)
        {
            Calcular(null);
        }

        private void Calcular(string empleadoASeleccionar)
        {
            try
            {
                _dias = _liquidacionBLL.Calcular(dtpDesde.Value.Date, dtpHasta.Value.Date);
            }
            catch (InvalidOperationException ex)
            {
                MsgBox.Show(ex.Message, "Atención", MsgBox.Botones.OK, MsgBox.Icono.Atencion);
                return;
            }
            _liquidacionCerrada = null;
            cboLiquidaciones.SelectedIndex = -1;
            Mostrar(empleadoASeleccionar);

            if (_dias.Count == 0)
                MsgBox.Show(Textos.T("msg_NoHayFichajesNiTurnosAsignadosEnElPeriodo", "No hay fichajes ni turnos asignados en el período."),
                    "Atención", MsgBox.Botones.OK, MsgBox.Icono.Atencion);
        }

        private void cboLiquidaciones_SelectionChangeCommitted(object sender, EventArgs e)
        {
            if (_cargando || !(cboLiquidaciones.SelectedItem is BE.Liquidacion liquidacion)) return;

            _liquidacionCerrada = liquidacion;
            _dias = _liquidacionBLL.ObtenerDiasDeLiquidacion(liquidacion.Id);
            _cargando = true;
            dtpDesde.Value = liquidacion.Desde;
            dtpHasta.Value = liquidacion.Hasta;
            _cargando = false;
            Mostrar(null);
        }

        private void Mostrar(string empleadoASeleccionar)
        {
            _resumen = _liquidacionBLL.Resumir(_dias);

            dgvResumen.DataSource = null;
            dgvResumen.DataSource = _resumen.Select(r => new LiquidacionResumenFilaVM
            {
                Empleado = r.Empleado,
                Trabajadas = LiquidacionCalculo.Hhmm(r.MinTrabajados),
                Asignadas = LiquidacionCalculo.Hhmm(r.MinAsignados),
                Pagables = LiquidacionCalculo.Hhmm(r.MinPagables),
                Normales = LiquidacionCalculo.Hhmm(r.MinNormales),
                Extra = LiquidacionCalculo.Hhmm(r.MinExtra),
                Feriado = LiquidacionCalculo.Hhmm(r.MinFeriado),
                ExcedenteAAprobar = LiquidacionCalculo.Hhmm(r.MinExcedentePendiente),
                Ausencias = r.Ausencias,
                Importe = r.Importe
            }).ToList();
            ActualizarEncabezados();

            if (empleadoASeleccionar != null)
            {
                foreach (DataGridViewRow fila in dgvResumen.Rows)
                    if ((string)fila.Cells["Empleado"].Value == empleadoASeleccionar)
                        dgvResumen.CurrentCell = fila.Cells["Empleado"];
            }

            MostrarDetalle();
            ActualizarEstado();
            ActualizarBotones();
        }

        private void MostrarDetalle()
        {
            _detalle = new List<BE.LiquidacionDia>();
            if (dgvResumen.CurrentRow != null && dgvResumen.CurrentRow.Index < _resumen.Count)
            {
                int usuarioId = _resumen[dgvResumen.CurrentRow.Index].UsuarioId;
                _detalle = _dias.Where(d => d.UsuarioId == usuarioId).OrderBy(d => d.Fecha).ToList();
            }

            dgvDetalle.DataSource = null;
            dgvDetalle.DataSource = _detalle.Select(d => new LiquidacionDetalleFilaVM
            {
                Fecha = d.Fecha.ToString("ddd dd/MM/yyyy"),
                Ingreso = d.Ingreso.HasValue ? d.Ingreso.Value.ToString(@"hh\:mm") : "",
                Egreso = d.Egreso.HasValue ? d.Egreso.Value.ToString(@"hh\:mm") : "",
                Trabajadas = LiquidacionCalculo.Hhmm(d.MinTrabajados),
                Asignadas = LiquidacionCalculo.Hhmm(d.MinAsignados),
                Pagables = LiquidacionCalculo.Hhmm(d.MinPagables),
                Normales = LiquidacionCalculo.Hhmm(d.MinNormales),
                Extra = LiquidacionCalculo.Hhmm(d.MinExtra),
                Feriado = LiquidacionCalculo.Hhmm(d.MinFeriado),
                Excedente = LiquidacionCalculo.Hhmm(d.MinExcedente),
                ValorHora = d.ValorHora,
                Importe = d.Importe,
                Observacion = LiquidacionExcel.TraducirObservacion(d.Observacion)
            }).ToList();
            ActualizarEncabezados();
            ActualizarBotones();
        }

        private void dgvResumen_SelectionChanged(object sender, EventArgs e)
        {
            MostrarDetalle();
        }

        private void dgvDetalle_SelectionChanged(object sender, EventArgs e)
        {
            ActualizarBotones();
        }

        private string EstadoTexto()
        {
            if (_liquidacionCerrada == null) return Textos.T("lbl_EstadoVistaPrevia", "Vista previa");
            return _liquidacionCerrada.Estado == "Pagada"
                ? Textos.T("lbl_EstadoPagada", "Pagada")
                : Textos.T("lbl_EstadoCerrada", "Cerrada");
        }

        private void ActualizarEstado()
        {
            if (_dias.Count == 0)
            {
                lblEstado.Text = "";
                lblAdvertencias.Text = "";
                return;
            }

            decimal total = _resumen.Sum(r => r.Importe);
            lblEstado.Text = _liquidacionCerrada == null
                ? Textos.T("lbl_EstadoPreviaTotal", "Vista previa (sin cerrar) — total a pagar: {0}", total.ToString("N2"))
                : Textos.T("lbl_EstadoCerradaTotal", "Liquidación {0} — total a pagar: {1}", EstadoTexto(), total.ToString("N2"));

            var avisos = new List<string>();
            if (_liquidacionCerrada == null)
            {
                int incompletos = _liquidacionBLL.ObtenerIncompletos(dtpDesde.Value.Date, dtpHasta.Value.Date).Count;
                if (incompletos > 0)
                    avisos.Add(Textos.T("lbl_AvisoIncompletos", "{0} fichaje(s) incompleto(s): corregilos para poder cerrar.", incompletos));

                var sinValor = _dias.Where(d => d.MinPagables > 0 && d.ValorHora == 0m).Select(d => d.Empleado).Distinct().ToList();
                if (sinValor.Count > 0)
                    avisos.Add(Textos.T("lbl_AvisoSinValorHora", "Sin valor hora: {0}.", string.Join(", ", sinValor)));
            }
            int pendientes = _dias.Count(d => d.MinExcedentePendiente > 0);
            if (pendientes > 0)
                avisos.Add(Textos.T("lbl_AvisoExcedentes", "{0} día(s) con horas excedentes sin aprobar (no se pagan).", pendientes));

            lblAdvertencias.Text = string.Join("  ", avisos);
        }

        private BE.LiquidacionDia DiaSeleccionado()
        {
            if (dgvDetalle.CurrentRow == null || dgvDetalle.CurrentRow.Index >= _detalle.Count) return null;
            return _detalle[dgvDetalle.CurrentRow.Index];
        }

        private void ActualizarBotones()
        {
            bool hayDatos = _dias.Count > 0;
            bool vistaPrevia = _liquidacionCerrada == null;
            var dia = DiaSeleccionado();

            btnAprobarExcedente.Enabled = vistaPrevia && dia != null && dia.MinExcedente > 0 && !dia.ExcedenteAprobado;
            btnRevocarExcedente.Enabled = vistaPrevia && dia != null && dia.MinExcedente > 0 && dia.ExcedenteAprobado;
            btnCerrarPeriodo.Enabled = vistaPrevia && hayDatos;
            btnMarcarPagada.Enabled = _liquidacionCerrada != null && _liquidacionCerrada.Estado == "Cerrada";
            btnExportar.Enabled = hayDatos;
            btnRecibo.Enabled = hayDatos && dgvResumen.CurrentRow != null;
        }

        private void btnAprobarExcedente_Click(object sender, EventArgs e)
        {
            CambiarExcedente(true);
        }

        private void btnRevocarExcedente_Click(object sender, EventArgs e)
        {
            CambiarExcedente(false);
        }

        private void CambiarExcedente(bool aprobar)
        {
            var dia = DiaSeleccionado();
            if (dia == null) return;
            try
            {
                if (aprobar) _liquidacionBLL.AprobarExcedente(dia.UsuarioId, dia.Empleado, dia.Fecha, Actor);
                else _liquidacionBLL.RevocarExcedente(dia.UsuarioId, dia.Empleado, dia.Fecha, Actor);
            }
            catch (InvalidOperationException ex)
            {
                MsgBox.Show(ex.Message, "Atención", MsgBox.Botones.OK, MsgBox.Icono.Atencion);
                return;
            }
            Calcular(dia.Empleado);
        }

        private void btnCerrarPeriodo_Click(object sender, EventArgs e)
        {
            string pregunta = Textos.T("msg_CerrarPeriodoConfirmar",
                "¿Cerrar el período {0:dd/MM/yyyy} al {1:dd/MM/yyyy}? Los números quedan fijos y los fichajes de esas fechas ya no se podrán modificar. Total: {2}.",
                dtpDesde.Value, dtpHasta.Value, _resumen.Sum(r => r.Importe).ToString("N2"));
            if (MsgBox.Show(pregunta, "Confirmar", MsgBox.Botones.SiNo, MsgBox.Icono.Pregunta) != DialogResult.Yes) return;

            try
            {
                int id = _liquidacionBLL.Cerrar(dtpDesde.Value.Date, dtpHasta.Value.Date, Actor);
                CargarLiquidaciones(id);
                cboLiquidaciones_SelectionChangeCommitted(cboLiquidaciones, EventArgs.Empty);
                MsgBox.Show(Textos.T("msg_PeriodoLiquidadoYCerrado", "Período liquidado y cerrado."), "Éxito",
                    MsgBox.Botones.OK, MsgBox.Icono.Exito);
            }
            catch (InvalidOperationException ex)
            {
                MsgBox.Show(ex.Message, "Atención", MsgBox.Botones.OK, MsgBox.Icono.Atencion);
            }
        }

        private void btnMarcarPagada_Click(object sender, EventArgs e)
        {
            if (_liquidacionCerrada == null) return;
            if (MsgBox.Show(Textos.T("msg_MarcarComoPagadaConfirmar", "¿Marcar la liquidación como pagada?"), "Confirmar",
                    MsgBox.Botones.SiNo, MsgBox.Icono.Pregunta) != DialogResult.Yes) return;

            try
            {
                _liquidacionBLL.MarcarPagada(_liquidacionCerrada, Actor);
                CargarLiquidaciones(_liquidacionCerrada.Id);
                cboLiquidaciones_SelectionChangeCommitted(cboLiquidaciones, EventArgs.Empty);
            }
            catch (InvalidOperationException ex)
            {
                MsgBox.Show(ex.Message, "Atención", MsgBox.Botones.OK, MsgBox.Icono.Atencion);
            }
        }

        private void btnExportar_Click(object sender, EventArgs e)
        {
            BE.ParametrosLiquidacion parametros = _liquidacionCerrada == null ? _liquidacionBLL.ObtenerParametros() : null;
            LiquidacionExcel.Exportar(_dias, dtpDesde.Value.Date, dtpHasta.Value.Date, EstadoTexto(), parametros);
        }

        private void btnRecibo_Click(object sender, EventArgs e)
        {
            if (dgvResumen.CurrentRow == null || dgvResumen.CurrentRow.Index >= _resumen.Count) return;
            int usuarioId = _resumen[dgvResumen.CurrentRow.Index].UsuarioId;
            var diasEmpleado = _dias.Where(d => d.UsuarioId == usuarioId).ToList();
            ReciboHoras.Mostrar(this, diasEmpleado, dtpDesde.Value.Date, dtpHasta.Value.Date, EstadoTexto());
        }

        private void btnCorregirFichajes_Click(object sender, EventArgs e)
        {
            new frmCorregirFichajes().ShowDialog();
            if (_liquidacionCerrada == null && _dias.Count > 0) Calcular(null);
        }

        private void btnParametros_Click(object sender, EventArgs e)
        {
            new frmParametrosLiquidacion().ShowDialog();
            if (_liquidacionCerrada == null && _dias.Count > 0) Calcular(null);
        }

        private void btnCerrar_Click(object sender, EventArgs e)
        {
            this.Close();
        }

        private void ActualizarEncabezados()
        {
            Encabezado(dgvResumen, "Empleado", "hdr_Empleado", "Empleado");
            Encabezado(dgvResumen, "Trabajadas", "hdr_HorasTrabajadas", "Trabajadas");
            Encabezado(dgvResumen, "Asignadas", "hdr_HorasAsignadas", "Asignadas");
            Encabezado(dgvResumen, "Pagables", "hdr_HorasAPagar", "A pagar");
            Encabezado(dgvResumen, "Normales", "hdr_HorasNormales", "Normales");
            Encabezado(dgvResumen, "Extra", "hdr_HorasExtra", "Extra");
            Encabezado(dgvResumen, "Feriado", "hdr_HorasFeriado", "Dom./feriado");
            Encabezado(dgvResumen, "ExcedenteAAprobar", "hdr_ExcedenteAAprobar", "Excedente a aprobar");
            Encabezado(dgvResumen, "Ausencias", "hdr_Ausencias", "Ausencias");
            Encabezado(dgvResumen, "Importe", "hdr_Importe", "Importe");
            if (dgvResumen.Columns["Importe"] != null)
                dgvResumen.Columns["Importe"].DefaultCellStyle.Format = "N2";

            Encabezado(dgvDetalle, "Fecha", "hdr_Fecha", "Fecha");
            Encabezado(dgvDetalle, "Ingreso", "hdr_Ingreso", "Ingreso");
            Encabezado(dgvDetalle, "Egreso", "hdr_Egreso", "Egreso");
            Encabezado(dgvDetalle, "Trabajadas", "hdr_HorasTrabajadas", "Trabajadas");
            Encabezado(dgvDetalle, "Asignadas", "hdr_HorasAsignadas", "Asignadas");
            Encabezado(dgvDetalle, "Pagables", "hdr_HorasAPagar", "A pagar");
            Encabezado(dgvDetalle, "Normales", "hdr_HorasNormales", "Normales");
            Encabezado(dgvDetalle, "Extra", "hdr_HorasExtra", "Extra");
            Encabezado(dgvDetalle, "Feriado", "hdr_HorasFeriado", "Dom./feriado");
            Encabezado(dgvDetalle, "Excedente", "hdr_Excedente", "Excedente");
            Encabezado(dgvDetalle, "ValorHora", "hdr_ValorHora", "Valor hora");
            Encabezado(dgvDetalle, "Importe", "hdr_Importe", "Importe");
            Encabezado(dgvDetalle, "Observacion", "hdr_Observacion", "Observación");
            foreach (string columna in new[] { "ValorHora", "Importe" })
                if (dgvDetalle.Columns[columna] != null)
                    dgvDetalle.Columns[columna].DefaultCellStyle.Format = "N2";
            foreach (DataGridViewColumn columna in dgvDetalle.Columns)
                columna.SortMode = DataGridViewColumnSortMode.NotSortable;
            foreach (DataGridViewColumn columna in dgvResumen.Columns)
                columna.SortMode = DataGridViewColumnSortMode.NotSortable;
        }

        protected override void ActualizarTextosDinamicos()
        {
            ActualizarEncabezados();
            ActualizarEstado();
        }
    }
}
