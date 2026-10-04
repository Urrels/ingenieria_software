using System;
using System.Collections.Generic;
using System.Linq;
using System.Windows.Forms;
using ReaLTaiizor.Forms;
using ReaLTaiizor.Manager;

namespace CAPAS
{
    public partial class frmRegistrarDisponibilidad : MaterialForm, SeguridadYServicios.IObservadorIdioma
    {
        private readonly BLL.DisponibilidadBLL _bll = new BLL.DisponibilidadBLL();
        private readonly BLL.FranjaHorariaBLL _franjaBLL = new BLL.FranjaHorariaBLL();

        private readonly Dictionary<string, Control> _controles = new Dictionary<string, Control>();
        private readonly Dictionary<string, string> _defaults = new Dictionary<string, string>();

        private List<BE.FranjaHoraria> _franjasDisponibles;
        private DateTime _semana;

        public frmRegistrarDisponibilidad()
        {
            InitializeComponent();
        }

        private void frmRegistrarDisponibilidad_Load(object sender, EventArgs e)
        {
            _semana = ObtenerLunesDeLaSemanaSiguiente();
            BE.USUARIO usuario = SeguridadYServicios.SessionManager.getInstance().getUsuario();

            _franjasDisponibles = _franjaBLL.ListarTodas()
                .Where(f => f.RolRequerido == usuario.RolNombre)
                .ToList();

            clbFranjas.Items.Clear();
            foreach (var franja in _franjasDisponibles)
                clbFranjas.Items.Add(DescripcionFranja(franja));

            BE.Disponibilidad previa = _bll.ObtenerPrevia(usuario, _semana);
            if (previa != null)
            {
                for (int i = 0; i < _franjasDisponibles.Count; i++)
                {
                    bool yaSeleccionada = previa.Franjas.Any(f => f.Id == _franjasDisponibles[i].Id);
                    clbFranjas.SetItemChecked(i, yaSeleccionada);
                }
                lblAviso.Text = "Ya habías cargado disponibilidad para esta semana — podés modificarla.";
            }

            GuardarDefaults(this.Controls);
            _controles[this.Name] = this;
            _defaults[this.Name] = this.Text;
            SeguridadYServicios.IdiomaManager.getInstance().Registrar(this);
            ActualizarIdioma();
            IdiomaUIHelper.AgregarSelector(this);
            MaterialSkinManager.Instance.AddFormToManage(this);
            AppTheme.AplicarTema(this);
        }

        private DateTime ObtenerLunesDeLaSemanaSiguiente()
        {
            DateTime hoy = DateTime.Today;
            int diasHastaLunes = ((int)DayOfWeek.Monday - (int)hoy.DayOfWeek + 7) % 7;
            if (diasHastaLunes == 0) diasHastaLunes = 7;
            return hoy.AddDays(diasHastaLunes);
        }

        private static readonly Dictionary<string, DayOfWeek> DiasSemana = new Dictionary<string, DayOfWeek>
{
    { "Monday", DayOfWeek.Monday }, { "Tuesday", DayOfWeek.Tuesday }, { "Wednesday", DayOfWeek.Wednesday },
    { "Thursday", DayOfWeek.Thursday }, { "Friday", DayOfWeek.Friday }, { "Saturday", DayOfWeek.Saturday },
    { "Sunday", DayOfWeek.Sunday }
};

        private string DescripcionFranja(BE.FranjaHoraria f)
        {
            DateTime fecha = _semana;
            if (DiasSemana.TryGetValue(f.Dia, out var dia))
                fecha = _semana.AddDays(((int)dia - (int)DayOfWeek.Monday + 7) % 7);

            return $"{f.Dia} {fecha:dd/MM}  {f.HoraInicio:hh\\:mm} - {f.HoraFin:hh\\:mm}";
        }
        private void frmRegistrarDisponibilidad_FormClosed(object sender, FormClosedEventArgs e)
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

        private void btnAceptar_Click(object sender, EventArgs e)
        {
            var seleccionadas = new List<BE.FranjaHoraria>();
            for (int i = 0; i < clbFranjas.Items.Count; i++)
                if (clbFranjas.GetItemChecked(i))
                    seleccionadas.Add(_franjasDisponibles[i]);

            if (seleccionadas.Count == 0)
            {
                MsgBox.Show("Tenés que indicar al menos una franja.", "Atención",
                    MsgBox.Botones.OK, MsgBox.Icono.Atencion);
                return;
            }

            BE.USUARIO usuario = SeguridadYServicios.SessionManager.getInstance().getUsuario();
            BE.Disponibilidad resultado = _bll.RegistrarDisponibilidad(usuario, _semana, seleccionadas);

            if (resultado == null)
            {
                MsgBox.Show("Tenés que indicar al menos una franja.", "Atención",
                    MsgBox.Botones.OK, MsgBox.Icono.Atencion);
                return;
            }

            MsgBox.Show("Disponibilidad registrada correctamente.", "Éxito",
                MsgBox.Botones.OK, MsgBox.Icono.Exito);
            this.DialogResult = DialogResult.OK;
            this.Close();
        }

        private void btnCancelar_Click(object sender, EventArgs e)
        {
            this.DialogResult = DialogResult.Cancel;
            this.Close();
        }
    }
}