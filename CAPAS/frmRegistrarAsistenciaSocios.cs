using System;
using System.Collections.Generic;
using System.Linq;
using System.Windows.Forms;
using ReaLTaiizor.Forms;
using ReaLTaiizor.Manager;

namespace CAPAS
{
    public partial class frmRegistrarAsistenciaSocios : MaterialForm, SeguridadYServicios.IObservadorIdioma
    {
        private readonly BLL.HistorialAsistenciaSociosBLL _historialBLL = new BLL.HistorialAsistenciaSociosBLL();
        private readonly BLL.FranjaHorariaBLL _franjaBLL = new BLL.FranjaHorariaBLL();

        private readonly Dictionary<string, Control> _controles = new Dictionary<string, Control>();
        private readonly Dictionary<string, string> _defaults = new Dictionary<string, string>();

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

            GuardarDefaults(this.Controls);
            _controles[this.Name] = this;
            _defaults[this.Name] = this.Text;
            SeguridadYServicios.IdiomaManager.getInstance().Registrar(this);
            ActualizarIdioma();
            IdiomaUIHelper.AgregarSelector(this);
            MaterialSkinManager.Instance.AddFormToManage(this);
            AppTheme.AplicarTema(this);
        }

        private void frmRegistrarAsistenciaSocios_FormClosed(object sender, FormClosedEventArgs e)
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

        private void btnRegistrar_Click(object sender, EventArgs e)
        {
            if (!(cboFranja.SelectedItem is BE.FranjaHoraria franja))
            {
                MsgBox.Show("Seleccioná una franja.", "Atención", MsgBox.Botones.OK, MsgBox.Icono.Atencion);
                return;
            }

            if (!int.TryParse(txtCantidad.Text, out int cantidad) || cantidad < 0)
            {
                MsgBox.Show("Ingresá una cantidad válida de socios.", "Atención",
                    MsgBox.Botones.OK, MsgBox.Icono.Atencion);
                return;
            }

            _historialBLL.RegistrarAsistencia(franja, dtpFecha.Value.Date, cantidad);

            MsgBox.Show("Asistencia de socios registrada.", "Éxito", MsgBox.Botones.OK, MsgBox.Icono.Exito);
            txtCantidad.Clear();
        }

        private void btnCerrar_Click(object sender, EventArgs e)
        {
            this.Close();
        }
    }
}