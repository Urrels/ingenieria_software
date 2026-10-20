using System;
using System.Collections.Generic;
using System.Linq;
using System.Windows.Forms;

namespace CAPAS
{
    public class ValorHoraFilaVM
    {
        public string Empleado { get; set; }
        public decimal Valor { get; set; }
        public string VigenteDesde { get; set; }
    }

    public class FeriadoFilaVM
    {
        public string Fecha { get; set; }
        public string Descripcion { get; set; }
    }

    public partial class frmParametrosLiquidacion : FormBase
    {
        private readonly BLL.LiquidacionBLL _liquidacionBLL = new BLL.LiquidacionBLL();

        private List<BE.Feriado> _feriados = new List<BE.Feriado>();

        public frmParametrosLiquidacion()
        {
            InitializeComponent();
        }

        private static BE.USUARIO Actor => SeguridadYServicios.SessionManager.getInstance().getUsuario();

        private void frmParametrosLiquidacion_Load(object sender, EventArgs e)
        {
            CargarParametros();
            cboEmpleadoValor.DataSource = _liquidacionBLL.ListarEmpleados();
            cboEmpleadoValor.SelectedIndex = -1;
            dtpVigente.Value = DateTime.Today;
            dtpFeriado.Value = DateTime.Today;
            CargarValores();
            CargarFeriados();

            InicializarFormulario();
        }

        protected override void AjustarClaves()
        {
            ExcluirDeTraduccion(nudTolerancia, nudRedondeo, nudJornada, nudRecargoExtra, nudRecargoFeriado, nudMaxHoras,
                nudValor, dtpVigente, dtpFeriado, cboEmpleadoValor);
        }

        // ---------- Parámetros ----------

        private void CargarParametros()
        {
            var p = _liquidacionBLL.ObtenerParametros();
            nudTolerancia.Value = p.ToleranciaMinutos;
            nudRedondeo.Value = p.RedondeoMinutos;
            nudJornada.Value = p.JornadaNormalHoras;
            nudRecargoExtra.Value = p.RecargoExtraPct;
            nudRecargoFeriado.Value = p.RecargoFeriadoPct;
            nudMaxHoras.Value = p.MaxHorasFichajeAbierto;
        }

        private void btnGuardarParametros_Click(object sender, EventArgs e)
        {
            var p = new BE.ParametrosLiquidacion
            {
                ToleranciaMinutos = (int)nudTolerancia.Value,
                RedondeoMinutos = (int)nudRedondeo.Value,
                JornadaNormalHoras = nudJornada.Value,
                RecargoExtraPct = nudRecargoExtra.Value,
                RecargoFeriadoPct = nudRecargoFeriado.Value,
                MaxHorasFichajeAbierto = (int)nudMaxHoras.Value
            };
            try
            {
                _liquidacionBLL.GuardarParametros(p, Actor);
            }
            catch (InvalidOperationException ex)
            {
                MsgBox.Show(ex.Message, "Atención", MsgBox.Botones.OK, MsgBox.Icono.Atencion);
                return;
            }
            MsgBox.Show(Textos.T("msg_ParametrosGuardados", "Parámetros guardados. Se aplican a los cálculos nuevos; las liquidaciones cerradas no cambian."),
                "Éxito", MsgBox.Botones.OK, MsgBox.Icono.Exito);
        }

        // ---------- Valor hora ----------

        private void CargarValores()
        {
            dgvValores.DataSource = null;
            dgvValores.DataSource = _liquidacionBLL.ListarValoresHora().Select(v => new ValorHoraFilaVM
            {
                Empleado = v.Empleado,
                Valor = v.Valor,
                VigenteDesde = v.VigenteDesde.ToString("dd/MM/yyyy")
            }).ToList();
            ActualizarEncabezados();
        }

        private void btnGuardarValor_Click(object sender, EventArgs e)
        {
            if (!(cboEmpleadoValor.SelectedItem is BE.EmpleadoLiquidable empleado))
            {
                MsgBox.Show(Textos.T("msg_SeleccionaUnEmpleado", "Seleccioná un empleado."), "Atención",
                    MsgBox.Botones.OK, MsgBox.Icono.Atencion);
                return;
            }
            try
            {
                _liquidacionBLL.GuardarValorHora(empleado, nudValor.Value, dtpVigente.Value.Date, Actor);
            }
            catch (InvalidOperationException ex)
            {
                MsgBox.Show(ex.Message, "Atención", MsgBox.Botones.OK, MsgBox.Icono.Atencion);
                return;
            }
            CargarValores();
        }

        // ---------- Feriados ----------

        private void CargarFeriados()
        {
            _feriados = _liquidacionBLL.ListarFeriados(DateTime.Today.AddYears(-1), DateTime.Today.AddYears(2));
            dgvFeriados.DataSource = null;
            dgvFeriados.DataSource = _feriados.Select(f => new FeriadoFilaVM
            {
                Fecha = f.Fecha.ToString("dd/MM/yyyy"),
                Descripcion = f.Descripcion
            }).ToList();
            ActualizarEncabezados();
        }

        private void btnGuardarFeriado_Click(object sender, EventArgs e)
        {
            _liquidacionBLL.GuardarFeriado(dtpFeriado.Value.Date, txtDescripcion.Text, Actor);
            txtDescripcion.Clear();
            CargarFeriados();
        }

        private void btnEliminarFeriado_Click(object sender, EventArgs e)
        {
            if (dgvFeriados.CurrentRow == null || dgvFeriados.CurrentRow.Index >= _feriados.Count) return;
            var feriado = _feriados[dgvFeriados.CurrentRow.Index];
            if (MsgBox.Show(Textos.T("msg_EliminarElFeriado", "¿Eliminar el feriado del {0:dd/MM/yyyy}?", feriado.Fecha),
                    "Confirmar", MsgBox.Botones.SiNo, MsgBox.Icono.Pregunta) != DialogResult.Yes) return;
            _liquidacionBLL.EliminarFeriado(feriado.Fecha, Actor);
            CargarFeriados();
        }

        private void btnCerrar_Click(object sender, EventArgs e)
        {
            this.Close();
        }

        private void ActualizarEncabezados()
        {
            Encabezado(dgvValores, "Empleado", "hdr_Empleado", "Empleado");
            Encabezado(dgvValores, "Valor", "hdr_ValorHora", "Valor hora");
            Encabezado(dgvValores, "VigenteDesde", "hdr_VigenteDesde", "Vigente desde");
            if (dgvValores.Columns["Valor"] != null)
                dgvValores.Columns["Valor"].DefaultCellStyle.Format = "N2";
            Encabezado(dgvFeriados, "Fecha", "hdr_Fecha", "Fecha");
            Encabezado(dgvFeriados, "Descripcion", "hdr_Descripcion", "Descripción");
        }

        protected override void ActualizarTextosDinamicos()
        {
            ActualizarEncabezados();
        }
    }
}
