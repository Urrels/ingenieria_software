using System;
using System.Collections.Generic;
using System.Linq;
using System.Windows.Forms;

namespace CAPAS
{
    public class FichajeFilaVM
    {
        public string Empleado { get; set; }
        public string Fecha { get; set; }
        public string Ingreso { get; set; }
        public string Egreso { get; set; }
        public string Estado { get; set; }
        public string Motivo { get; set; }
    }

    public partial class frmCorregirFichajes : FormBase
    {
        private readonly BLL.AsistenciaPersonalBLL _asistenciaBLL = new BLL.AsistenciaPersonalBLL();
        private readonly BLL.LiquidacionBLL _liquidacionBLL = new BLL.LiquidacionBLL();

        private List<BE.RegistroAsistenciaPersonal> _registros = new List<BE.RegistroAsistenciaPersonal>();

        public frmCorregirFichajes()
        {
            InitializeComponent();
        }

        private static BE.USUARIO Actor => SeguridadYServicios.SessionManager.getInstance().getUsuario();

        private void frmCorregirFichajes_Load(object sender, EventArgs e)
        {
            dtpDesde.Value = DateTime.Today.AddDays(-30);
            dtpHasta.Value = DateTime.Today;
            dtpIngreso.Value = DateTime.Today.AddHours(9);
            dtpEgreso.Value = DateTime.Today.AddHours(17);
            dtpFecha.Value = DateTime.Today;
            chkSoloIncompletos.Checked = true;

            cboEmpleado.DataSource = _liquidacionBLL.ListarEmpleados();
            cboEmpleado.SelectedIndex = -1;

            Buscar();
            InicializarFormulario();
        }

        protected override void AjustarClaves()
        {
            ExcluirDeTraduccion(dtpDesde, dtpHasta, dtpIngreso, dtpEgreso, dtpFecha, cboEmpleado);
        }

        private void btnBuscar_Click(object sender, EventArgs e)
        {
            Buscar();
        }

        private void Buscar()
        {
            _registros = _asistenciaBLL.Listar(dtpDesde.Value.Date, dtpHasta.Value.Date, chkSoloIncompletos.Checked);

            string sinEgreso = Textos.T("lbl_FichajeSinEgreso", "Sin egreso");
            string corregido = Textos.T("lbl_FichajeCorregido", "Corregido");
            string normal = Textos.T("lbl_FichajeNormal", "Normal");

            dgvFichajes.DataSource = null;
            dgvFichajes.DataSource = _registros.Select(r => new FichajeFilaVM
            {
                Empleado = r.Empleado,
                Fecha = r.Fecha.ToString("dd/MM/yyyy"),
                Ingreso = r.HoraIngreso.ToString(@"hh\:mm"),
                Egreso = r.HoraEgreso.HasValue ? r.HoraEgreso.Value.ToString(@"hh\:mm") : "",
                Estado = !r.HoraEgreso.HasValue ? sinEgreso : (r.Corregido ? corregido : normal),
                Motivo = r.MotivoCorreccion
            }).ToList();
            ActualizarEncabezados();
        }

        private BE.RegistroAsistenciaPersonal Seleccionado()
        {
            if (dgvFichajes.CurrentRow == null || dgvFichajes.CurrentRow.Index >= _registros.Count) return null;
            return _registros[dgvFichajes.CurrentRow.Index];
        }

        private void dgvFichajes_SelectionChanged(object sender, EventArgs e)
        {
            var r = Seleccionado();
            if (r == null) return;
            dtpIngreso.Value = DateTime.Today + r.HoraIngreso;
            if (r.HoraEgreso.HasValue) dtpEgreso.Value = DateTime.Today + r.HoraEgreso.Value;
        }

        private void btnCorregir_Click(object sender, EventArgs e)
        {
            var registro = Seleccionado();
            if (registro == null)
            {
                MsgBox.Show(Textos.T("msg_SeleccionaUnFichaje", "Seleccioná un fichaje de la lista."), "Atención",
                    MsgBox.Botones.OK, MsgBox.Icono.Atencion);
                return;
            }

            try
            {
                _asistenciaBLL.Corregir(registro.Id, dtpIngreso.Value.TimeOfDay, dtpEgreso.Value.TimeOfDay, txtMotivo.Text, Actor);
            }
            catch (InvalidOperationException ex)
            {
                MsgBox.Show(ex.Message, "Atención", MsgBox.Botones.OK, MsgBox.Icono.Atencion);
                return;
            }

            txtMotivo.Clear();
            Buscar();
            MsgBox.Show(Textos.T("msg_FichajeCorregido", "Fichaje corregido. Quedó registrado quién lo corrigió y por qué."), "Éxito",
                MsgBox.Botones.OK, MsgBox.Icono.Exito);
        }

        private void btnAgregarManual_Click(object sender, EventArgs e)
        {
            if (!(cboEmpleado.SelectedItem is BE.EmpleadoLiquidable empleado))
            {
                MsgBox.Show(Textos.T("msg_SeleccionaUnEmpleado", "Seleccioná un empleado."), "Atención",
                    MsgBox.Botones.OK, MsgBox.Icono.Atencion);
                return;
            }

            try
            {
                _asistenciaBLL.AgregarManual(empleado.Id, empleado.NombreCompleto, dtpFecha.Value.Date,
                    dtpIngreso.Value.TimeOfDay, dtpEgreso.Value.TimeOfDay, txtMotivo.Text, Actor);
            }
            catch (InvalidOperationException ex)
            {
                MsgBox.Show(ex.Message, "Atención", MsgBox.Botones.OK, MsgBox.Icono.Atencion);
                return;
            }

            txtMotivo.Clear();
            chkSoloIncompletos.Checked = false;
            Buscar();
            MsgBox.Show(Textos.T("msg_FichajeAgregado", "Fichaje agregado. Quedó marcado como cargado por el administrador."), "Éxito",
                MsgBox.Botones.OK, MsgBox.Icono.Exito);
        }

        private void btnCerrar_Click(object sender, EventArgs e)
        {
            this.Close();
        }

        private void ActualizarEncabezados()
        {
            Encabezado(dgvFichajes, "Empleado", "hdr_Empleado", "Empleado");
            Encabezado(dgvFichajes, "Fecha", "hdr_Fecha", "Fecha");
            Encabezado(dgvFichajes, "Ingreso", "hdr_Ingreso", "Ingreso");
            Encabezado(dgvFichajes, "Egreso", "hdr_Egreso", "Egreso");
            Encabezado(dgvFichajes, "Estado", "hdr_Estado", "Estado");
            Encabezado(dgvFichajes, "Motivo", "hdr_Motivo", "Motivo de la corrección");
        }

        protected override void ActualizarTextosDinamicos()
        {
            ActualizarEncabezados();
        }
    }
}
