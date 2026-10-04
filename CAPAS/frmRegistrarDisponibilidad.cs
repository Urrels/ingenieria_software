using System;
using System.Collections.Generic;
using System.Linq;
using System.Windows.Forms;

namespace CAPAS
{
    public partial class frmRegistrarDisponibilidad : FormBase
    {
        private readonly BLL.DisponibilidadBLL _bll = new BLL.DisponibilidadBLL();
        private readonly BLL.FranjaHorariaBLL _franjaBLL = new BLL.FranjaHorariaBLL();


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
                lblAviso.Text = Textos.T("lbl_DisponibilidadYaCargada", "Ya habías cargado disponibilidad para esta semana — podés modificarla.");
            }

            InicializarFormulario();
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

        private void btnAceptar_Click(object sender, EventArgs e)
        {
            var seleccionadas = new List<BE.FranjaHoraria>();
            for (int i = 0; i < clbFranjas.Items.Count; i++)
                if (clbFranjas.GetItemChecked(i))
                    seleccionadas.Add(_franjasDisponibles[i]);

            if (seleccionadas.Count == 0)
            {
                MsgBox.Show(Textos.T("msg_TenesQueIndicarAlMenosUnaFranja", "Tenés que indicar al menos una franja."), "Atención",
                    MsgBox.Botones.OK, MsgBox.Icono.Atencion);
                return;
            }

            BE.USUARIO usuario = SeguridadYServicios.SessionManager.getInstance().getUsuario();
            BE.Disponibilidad resultado = _bll.RegistrarDisponibilidad(usuario, _semana, seleccionadas);

            if (resultado == null)
            {
                MsgBox.Show(Textos.T("msg_TenesQueIndicarAlMenosUnaFranja", "Tenés que indicar al menos una franja."), "Atención",
                    MsgBox.Botones.OK, MsgBox.Icono.Atencion);
                return;
            }

            MsgBox.Show(Textos.T("msg_DisponibilidadRegistradaCorrectamente", "Disponibilidad registrada correctamente."), "Éxito",
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