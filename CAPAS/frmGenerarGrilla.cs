using System;
using System.Collections.Generic;
using System.Linq;
using System.Windows.Forms;
using ReaLTaiizor.Forms;
using ReaLTaiizor.Manager;

namespace CAPAS
{
    public class TurnoFilaVM
    {
        public BE.Turno Turno { get; }
        public BE.FranjaHoraria Franja { get; }
        public string FranjaDescripcion => $"{Franja.Dia}  {Franja.HoraInicio:hh\\:mm}-{Franja.HoraFin:hh\\:mm}";
        public string Rol => Turno.RolRequerido;
        public string Empleado { get; set; }
        public string Estado => Turno.Estado;

        public TurnoFilaVM(BE.Turno turno, BE.FranjaHoraria franja, string empleado)
        {
            Turno = turno;
            Franja = franja;
            Empleado = empleado;
        }
    }

    public partial class frmGenerarGrilla : MaterialForm, SeguridadYServicios.IObservadorIdioma
    {
        private readonly BLL.GrillaBLL _grillaBLL = new BLL.GrillaBLL();
        private readonly BLL.FranjaHorariaBLL _franjaBLL = new BLL.FranjaHorariaBLL();
        private readonly BLL.UsuarioBLL _usuarioBLL = new BLL.UsuarioBLL();

        private readonly Dictionary<string, Control> _controles = new Dictionary<string, Control>();
        private readonly Dictionary<string, string> _defaults = new Dictionary<string, string>();

        private BE.GrillaDeTurnos _grilla;
        private List<TurnoFilaVM> _filas;

        public frmGenerarGrilla()
        {
            InitializeComponent();
        }

        private void frmGenerarGrilla_Load(object sender, EventArgs e)
        {
            dtpSemana.Value = ObtenerLunesDeLaSemanaSiguiente();

            GuardarDefaults(this.Controls);
            _controles[this.Name] = this;
            _defaults[this.Name] = this.Text;
            SeguridadYServicios.IdiomaManager.getInstance().Registrar(this);
            ActualizarIdioma();
            IdiomaUIHelper.AgregarSelector(this);
            MaterialSkinManager.Instance.AddFormToManage(this);
            AppTheme.AplicarTema(this);

            ActualizarBotones();
        }

        private DateTime ObtenerLunesDeLaSemanaSiguiente()
        {
            DateTime hoy = DateTime.Today;
            int diasHastaLunes = ((int)DayOfWeek.Monday - (int)hoy.DayOfWeek + 7) % 7;
            if (diasHastaLunes == 0) diasHastaLunes = 7;
            return hoy.AddDays(diasHastaLunes);
        }

        private void frmGenerarGrilla_FormClosed(object sender, FormClosedEventArgs e)
        {
            SeguridadYServicios.IdiomaManager.getInstance().Desregistrar(this);
        }

        public void ActualizarIdioma()
        {
            foreach (var kvp in _controles)
            {
                string t = SeguridadYServicios.IdiomaManager.getInstance().Traducir(kvp.Key)
                           ?? _defaults[kvp.Key];
                kvp.Value.Text = t;
            }
        }

        private void GuardarDefaults(Control.ControlCollection controles)
        {
            foreach (Control c in controles)
            {
                if (!string.IsNullOrEmpty(c.Name) && !string.IsNullOrEmpty(c.Text))
                {
                    _controles[c.Name] = c;
                    _defaults[c.Name] = c.Text;
                }
                if (c.HasChildren) GuardarDefaults(c.Controls);
            }
        }

        private void btnGenerar_Click(object sender, EventArgs e)
        {
            if (MsgBox.Show($"¿Generar la grilla para la semana del {dtpSemana.Value:dd/MM}?",
                "Confirmar", MsgBox.Botones.SiNo, MsgBox.Icono.Pregunta) != DialogResult.Yes)
                return;

            _grilla = _grillaBLL.GenerarGrilla(dtpSemana.Value.Date);
            CargarFilas();
            ActualizarBotones();
        }

        private void CargarFilas()
        {
            var franjasPorId = _franjaBLL.ListarTodas().ToDictionary(f => f.Id);

            _filas = _grilla.Turnos.Select(t =>
            {
                string empleado = "Sin asignar";
                if (t.UsuarioId.HasValue)
                {
                    var u = _usuarioBLL.ObtenerPorId(t.UsuarioId.Value);
                    empleado = u != null ? $"{u.Nombre} {u.Apellido}" : "Sin asignar";
                }
                else if (t.Estado == "DeficitCobertura")
                {
                    empleado = "— Déficit de cobertura —";
                }
                return new TurnoFilaVM(t, franjasPorId[t.FranjaId], empleado);
            }).ToList();

            dgvTurnos.DataSource = null;
            dgvTurnos.DataSource = _filas;

            if (dgvTurnos.Columns.Count > 0)
            {
                if (dgvTurnos.Columns["Turno"] != null) dgvTurnos.Columns["Turno"].Visible = false;
                if (dgvTurnos.Columns["Franja"] != null) dgvTurnos.Columns["Franja"].Visible = false;
                if (dgvTurnos.Columns["FranjaDescripcion"] != null) dgvTurnos.Columns["FranjaDescripcion"].HeaderText = "Franja";
                if (dgvTurnos.Columns["Rol"] != null) dgvTurnos.Columns["Rol"].HeaderText = "Rol";
                if (dgvTurnos.Columns["Empleado"] != null) dgvTurnos.Columns["Empleado"].HeaderText = "Empleado";
                if (dgvTurnos.Columns["Estado"] != null) dgvTurnos.Columns["Estado"].HeaderText = "Estado";
            }
        }

        private void dgvTurnos_CellDoubleClick(object sender, DataGridViewCellEventArgs e)
        {
            if (_grilla.Estado != "Propuesta")
            {
                MsgBox.Show("La grilla ya fue confirmada, no se puede modificar.", "Atención",
                    MsgBox.Botones.OK, MsgBox.Icono.Atencion);
                return;
            }
            if (e.RowIndex < 0) return;

            var fila = (TurnoFilaVM)dgvTurnos.Rows[e.RowIndex].DataBoundItem;

            using (var frm = new frmAsignarEmpleado(fila.Turno, fila.Franja))
            {
                frm.ShowDialog();
                CargarFilas();
            }
        }

        private void btnConfirmar_Click(object sender, EventArgs e)
        {
            bool ok = _grillaBLL.ConfirmarGrilla(_grilla);
            if (!ok)
            {
                MsgBox.Show("Todavía quedan franjas sin asignar ni marcar como déficit de cobertura.",
                    "Atención", MsgBox.Botones.OK, MsgBox.Icono.Atencion);
                return;
            }

            MsgBox.Show("Grilla confirmada correctamente.", "Éxito", MsgBox.Botones.OK, MsgBox.Icono.Exito);
            ActualizarBotones();
        }

        private void btnComunicar_Click(object sender, EventArgs e)
        {
            _grillaBLL.ComunicarHorarios(_grilla);
            MsgBox.Show("Horarios comunicados al equipo.", "Éxito", MsgBox.Botones.OK, MsgBox.Icono.Exito);
            ActualizarBotones();
        }

        private void btnExportar_Click(object sender, EventArgs e)
        {
            ExcelExportHelper.ExportarDataGridView(dgvTurnos, "GrillaTurnos");
        }

        private void ActualizarBotones()
        {
            bool hayGrilla = _grilla != null;
            btnConfirmar.Enabled = hayGrilla && _grilla.Estado == "Propuesta";
            btnComunicar.Enabled = hayGrilla && _grilla.Estado == "Confirmada";
            btnGenerar.Enabled = !hayGrilla || _grilla.Estado == "Comunicada";
        }

        private void btnCerrar_Click(object sender, EventArgs e)
        {
            this.Close();
        }
    }
}