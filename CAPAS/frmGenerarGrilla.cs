using System;
using System.Collections.Generic;
using System.Linq;
using System.Windows.Forms;

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

    public partial class frmGenerarGrilla : FormBase
    {
        private readonly BLL.GrillaBLL _grillaBLL = new BLL.GrillaBLL();
        private readonly BLL.FranjaHorariaBLL _franjaBLL = new BLL.FranjaHorariaBLL();
        private readonly BLL.UsuarioBLL _usuarioBLL = new BLL.UsuarioBLL();


        private BE.GrillaDeTurnos _grilla;
        private List<TurnoFilaVM> _filas;

        public frmGenerarGrilla()
        {
            InitializeComponent();
        }

        private void frmGenerarGrilla_Load(object sender, EventArgs e)
        {
            dtpSemana.Value = ObtenerLunesDeLaSemanaSiguiente();

            InicializarFormulario();

            ActualizarBotones();
        }

        private DateTime ObtenerLunesDeLaSemanaSiguiente()
        {
            DateTime hoy = DateTime.Today;
            int diasHastaLunes = ((int)DayOfWeek.Monday - (int)hoy.DayOfWeek + 7) % 7;
            if (diasHastaLunes == 0) diasHastaLunes = 7;
            return hoy.AddDays(diasHastaLunes);
        }

        private void btnGenerar_Click(object sender, EventArgs e)
        {
            if (MsgBox.Show(Textos.T("msg_GenerarLaGrillaParaLaSemanaDel", "¿Generar la grilla para la semana del {0:dd/MM}?", dtpSemana.Value),
                "Confirmar", MsgBox.Botones.SiNo, MsgBox.Icono.Pregunta) != DialogResult.Yes)
                return;

            _grilla = _grillaBLL.GenerarGrilla(dtpSemana.Value.Date);
            CargarFilas();
            ActualizarBotones();
            AvisarFranjasSinTurnos();
        }

        // Una franja sin las 4 semanas de asistencia de socios tiene demanda 0 y no genera turnos
        private void AvisarFranjasSinTurnos()
        {
            var franjas = _franjaBLL.ListarTodas();
            if (franjas.Count == 0)
            {
                MsgBox.Show(Textos.T("msg_NoHayFranjasHorariasCargadas",
                    "No hay franjas horarias cargadas. Crealas en Horarios → Gestión de franjas horarias."),
                    "Atención", MsgBox.Botones.OK, MsgBox.Icono.Atencion);
                return;
            }

            var sinTurnos = franjas.Where(f => !_grilla.Turnos.Any(t => t.FranjaId == f.Id))
                .Select(f => $"{f.Dia} {f.HoraInicio:hh\\:mm}-{f.HoraFin:hh\\:mm} ({f.RolRequerido})")
                .ToList();
            if (sinTurnos.Count == 0) return;

            MsgBox.Show(Textos.T("msg_FranjasSinHistorialDeAsistencia",
                "Estas franjas no generaron turnos porque no tienen 4 semanas de asistencia de socios registradas:\n\n{0}\n\nCargalas en Asistencia → Registrar asistencia de socios y volvé a generar la grilla.",
                string.Join("\n", sinTurnos)),
                "Atención", MsgBox.Botones.OK, MsgBox.Icono.Atencion);
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
                ActualizarEncabezados();
            }
        }

        private void dgvTurnos_CellDoubleClick(object sender, DataGridViewCellEventArgs e)
        {
            if (_grilla.Estado != "Propuesta")
            {
                MsgBox.Show(Textos.T("msg_LaGrillaYaFueConfirmadaNoSePuedeModificar", "La grilla ya fue confirmada, no se puede modificar."), "Atención",
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
                MsgBox.Show(Textos.T("msg_TodaviaQuedanFranjasSinAsignarNiMarcarComoDeficitDeCobertura", "Todavía quedan franjas sin asignar ni marcar como déficit de cobertura."),
                    "Atención", MsgBox.Botones.OK, MsgBox.Icono.Atencion);
                return;
            }

            MsgBox.Show(Textos.T("msg_GrillaConfirmadaCorrectamente", "Grilla confirmada correctamente."), "Éxito", MsgBox.Botones.OK, MsgBox.Icono.Exito);
            ActualizarBotones();
        }

        private void btnComunicar_Click(object sender, EventArgs e)
        {
            _grillaBLL.ComunicarHorarios(_grilla);
            MsgBox.Show(Textos.T("msg_HorariosComunicadosAlEquipo", "Horarios comunicados al equipo."), "Éxito", MsgBox.Botones.OK, MsgBox.Icono.Exito);
            ActualizarBotones();
        }

        private void btnExportar_Click(object sender, EventArgs e)
        {
            ExcelExportHelper.ExportarDataGridView(dgvTurnos, "GrillaTurnos");
        }

        private void ActualizarBotones()
        {
            bool hayGrilla = _grilla != null;
            btnConfirmar.Enabled = hayGrilla && _grilla.Estado == "Propuesta" && _grilla.Turnos.Count > 0;
            btnComunicar.Enabled = hayGrilla && _grilla.Estado == "Confirmada";
            btnGenerar.Enabled = !hayGrilla || _grilla.Estado == "Comunicada" || _grilla.Estado == "Propuesta";
        }

        private void btnCerrar_Click(object sender, EventArgs e)
        {
            this.Close();
        }

        private void ActualizarEncabezados()
        {
            Encabezado(dgvTurnos, "FranjaDescripcion", "hdr_Franja", "Franja");
            Encabezado(dgvTurnos, "Rol", "hdr_Rol", "Rol");
            Encabezado(dgvTurnos, "Empleado", "hdr_Empleado", "Empleado");
            Encabezado(dgvTurnos, "Estado", "hdr_Estado", "Estado");
        }

        protected override void ActualizarTextosDinamicos()
        {
            ActualizarEncabezados();
        }
    }
}