using System;
using System.Collections.Generic;
using System.Linq;
using System.Windows.Forms;
using ReaLTaiizor.Forms;
using ReaLTaiizor.Manager;

namespace CAPAS
{
    public partial class frmMisTurnos : MaterialForm, SeguridadYServicios.IObservadorIdioma
    {
        private readonly BLL.TurnoBLL _turnoBLL = new BLL.TurnoBLL();
        private readonly BLL.FranjaHorariaBLL _franjaBLL = new BLL.FranjaHorariaBLL();

        private readonly Dictionary<string, Control> _controles = new Dictionary<string, Control>();
        private readonly Dictionary<string, string> _defaults = new Dictionary<string, string>();

        private List<TurnoEmpleadoVM> _filas;

        public frmMisTurnos()
        {
            InitializeComponent();
        }

        private void frmMisTurnos_Load(object sender, EventArgs e)
        {
            CargarFilas();

            GuardarDefaults(this.Controls);
            _controles[this.Name] = this;
            _defaults[this.Name] = this.Text;
            SeguridadYServicios.IdiomaManager.getInstance().Registrar(this);
            ActualizarIdioma();
            IdiomaUIHelper.AgregarSelector(this);
            MaterialSkinManager.Instance.AddFormToManage(this);
            AppTheme.AplicarTema(this);
        }

        private void frmMisTurnos_FormClosed(object sender, FormClosedEventArgs e)
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

        private void CargarFilas()
        {
            BE.USUARIO usuario = SeguridadYServicios.SessionManager.getInstance().getUsuario();
            var franjasPorId = _franjaBLL.ListarTodas().ToDictionary(f => f.Id);

            List<BE.Turno> turnos = _turnoBLL.ListarPorUsuario(usuario.Id);

            _filas = turnos.Select(t => new TurnoEmpleadoVM(t, franjasPorId[t.FranjaId])).ToList();

            dgvTurnos.DataSource = null;
            dgvTurnos.DataSource = _filas;

            if (dgvTurnos.Columns.Count > 0)
            {
                if (dgvTurnos.Columns["Turno"] != null) dgvTurnos.Columns["Turno"].Visible = false;
                if (dgvTurnos.Columns["Franja"] != null) dgvTurnos.Columns["Franja"].Visible = false;
                if (dgvTurnos.Columns["Dia"] != null) dgvTurnos.Columns["Dia"].HeaderText = "Día";
                if (dgvTurnos.Columns["Horario"] != null) dgvTurnos.Columns["Horario"].HeaderText = "Horario";
                if (dgvTurnos.Columns["Rol"] != null) dgvTurnos.Columns["Rol"].HeaderText = "Rol";
                if (dgvTurnos.Columns["Estado"] != null) dgvTurnos.Columns["Estado"].HeaderText = "Estado";
            }
        }

        private void btnCancelarTurno_Click(object sender, EventArgs e)
        {
            if (dgvTurnos.CurrentRow == null)
            {
                MsgBox.Show("Seleccioná un turno de la lista.", "Atención",
                    MsgBox.Botones.OK, MsgBox.Icono.Atencion);
                return;
            }

            var fila = (TurnoEmpleadoVM)dgvTurnos.CurrentRow.DataBoundItem;

            if (fila.Turno.Estado != "Asignado")
            {
                MsgBox.Show("Este turno ya no está asignado (puede que ya lo hayas cancelado antes).", "Atención",
                    MsgBox.Botones.OK, MsgBox.Icono.Atencion);
                return;
            }

            if (MsgBox.Show($"¿Seguro que querés cancelar el turno del {fila.Dia} {fila.Horario}?\n" +
                "Se va a notificar automáticamente al administrador y a los empleados disponibles para cubrirlo.",
                "Confirmar cancelación", MsgBox.Botones.SiNo, MsgBox.Icono.Pregunta) != DialogResult.Yes)
                return;

            BE.USUARIO usuario = SeguridadYServicios.SessionManager.getInstance().getUsuario();
            List<BE.USUARIO> notificados = _turnoBLL.CancelarPorEmpleado(fila.Turno, usuario);

            MsgBox.Show(notificados.Count > 0
                    ? $"Turno cancelado. Se notificó al administrador y a {notificados.Count} empleado(s) disponible(s)."
                    : "Turno cancelado. Se notificó al administrador (no se encontraron empleados disponibles para cubrirlo).",
                "Listo", MsgBox.Botones.OK, MsgBox.Icono.Exito);

            CargarFilas();
        }

        private void btnCerrar_Click(object sender, EventArgs e)
        {
            this.Close();
        }
    }

    public class TurnoEmpleadoVM
    {
        public BE.Turno Turno { get; }
        public BE.FranjaHoraria FranjaObj { get; }

        public string Dia => FranjaObj.Dia;
        public string Horario => $"{FranjaObj.HoraInicio:hh\\:mm} - {FranjaObj.HoraFin:hh\\:mm}";
        public string Rol => Turno.RolRequerido;
        public string Estado => Turno.Estado;

        public TurnoEmpleadoVM(BE.Turno turno, BE.FranjaHoraria franja)
        {
            Turno = turno;
            FranjaObj = franja;
        }
    }
}