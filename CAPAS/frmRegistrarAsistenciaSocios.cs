using System;
using System.Collections.Generic;
using System.Linq;
using System.Windows.Forms;

namespace CAPAS
{
    public partial class frmRegistrarAsistenciaSocios : FormBase
    {
        private readonly BLL.HistorialAsistenciaSociosBLL _historialBLL = new BLL.HistorialAsistenciaSociosBLL();
        private readonly BLL.FranjaHorariaBLL _franjaBLL = new BLL.FranjaHorariaBLL();


        private List<BE.FranjaHoraria> _franjas;

        public frmRegistrarAsistenciaSocios()
        {
            InitializeComponent();
        }

        private void frmRegistrarAsistenciaSocios_Load(object sender, EventArgs e)
        {
            dtpFecha.Value = DateTime.Today;

            _franjas = _franjaBLL.ListarTodas();
            cboFranja.DataSource = _franjas;
            cboFranja.DisplayMember = "Dia";
            cboFranja.ValueMember = "Id";
            cboFranja.Format += (s, ev) =>
            {
                var f = (BE.FranjaHoraria)ev.ListItem;
                ev.Value = $"{f.Dia} {f.HoraInicio:hh\\:mm}-{f.HoraFin:hh\\:mm} ({f.RolRequerido})";
            };

            InicializarFormulario();
        }

        private void btnRegistrar_Click(object sender, EventArgs e)
        {
            if (!(cboFranja.SelectedItem is BE.FranjaHoraria franja))
            {
                MsgBox.Show(Textos.T("msg_SeleccionaUnaFranja", "Seleccioná una franja."), "Atención", MsgBox.Botones.OK, MsgBox.Icono.Atencion);
                return;
            }

            if (!int.TryParse(txtCantidad.Text, out int cantidad) || cantidad < 0)
            {
                MsgBox.Show(Textos.T("msg_IngresaUnaCantidadValidaDeSocios", "Ingresá una cantidad válida de socios."), "Atención",
                    MsgBox.Botones.OK, MsgBox.Icono.Atencion);
                return;
            }

            _historialBLL.RegistrarAsistencia(franja, dtpFecha.Value.Date, cantidad);

            MsgBox.Show(Textos.T("msg_AsistenciaDeSociosRegistrada", "Asistencia de socios registrada."), "Éxito", MsgBox.Botones.OK, MsgBox.Icono.Exito);
            txtCantidad.Clear();
        }

        private void btnCerrar_Click(object sender, EventArgs e)
        {
            this.Close();
        }
    }
}