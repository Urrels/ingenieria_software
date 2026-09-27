using System;
using System.Collections.Generic;
using System.Windows.Forms;
using ReaLTaiizor.Forms;
using ReaLTaiizor.Manager;

namespace CAPAS
{
    public partial class frmRealizarRevision : MaterialForm, SeguridadYServicios.IObservadorIdioma
    {
        private readonly BLL.VisitaTecnicaBLL _visitaBLL = new BLL.VisitaTecnicaBLL();
        private readonly BLL.AlertaRevisionBLL _alertaBLL = new BLL.AlertaRevisionBLL();
        private readonly BLL.EquipoBLL _equipoBLL = new BLL.EquipoBLL();
        private readonly BLL.InformeMantenimientoBLL _informeBLL = new BLL.InformeMantenimientoBLL();

        private readonly Dictionary<string, Control> _controles = new Dictionary<string, Control>();
        private readonly Dictionary<string, string> _defaults = new Dictionary<string, string>();

        private List<BE.VisitaTecnica> _visitas;

        public frmRealizarRevision()
        {
            InitializeComponent();
        }

        private void frmRealizarRevision_Load(object sender, EventArgs e)
        {
            CargarVisitas();

            GuardarDefaults(this.Controls);
            _controles[this.Name] = this;
            _defaults[this.Name] = this.Text;
            SeguridadYServicios.IdiomaManager.getInstance().Registrar(this);
            ActualizarIdioma();
            IdiomaUIHelper.AgregarSelector(this);
            MaterialSkinManager.Instance.AddFormToManage(this);
            AppTheme.AplicarTema(this);
        }

        private void CargarVisitas()
        {
            int tecnicoId = SeguridadYServicios.SessionManager.getInstance().getUsuario().Id;
            _visitas = _visitaBLL.ListarPendientesPorTecnico(tecnicoId);

            dgvVisitas.DataSource = null;
            dgvVisitas.DataSource = _visitas;

            if (dgvVisitas.Columns.Count > 0)
            {
                if (dgvVisitas.Columns["TecnicoId"] != null) dgvVisitas.Columns["TecnicoId"].Visible = false;
                if (dgvVisitas.Columns["AlertaId"] != null) dgvVisitas.Columns["AlertaId"].HeaderText = "Alerta";
                if (dgvVisitas.Columns["FechaCoordinada"] != null) dgvVisitas.Columns["FechaCoordinada"].HeaderText = "Fecha coordinada";
                if (dgvVisitas.Columns["Estado"] != null) dgvVisitas.Columns["Estado"].HeaderText = "Estado";
            }

            txtResultado.Clear();
            chkPendienteRepuesto.Checked = false;
        }

        private void frmRealizarRevision_FormClosed(object sender, FormClosedEventArgs e)
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

        private void dgvVisitas_SelectionChanged(object sender, EventArgs e)
        {
            var visita = dgvVisitas.CurrentRow?.DataBoundItem as BE.VisitaTecnica;
            if (visita == null) { lblEquipo.Text = "Equipo: —"; return; }

            BE.AlertaRevision alerta = _alertaBLL.ObtenerPorId(visita.AlertaId);
            BE.Equipo equipo = alerta != null ? _equipoBLL.ObtenerPorId(alerta.EquipoId) : null;

            lblEquipo.Text = equipo != null
                ? $"Equipo: {equipo.Nombre}  (uso acumulado: {equipo.UsoAcumulado})"
                : "Equipo: (no se pudo obtener el detalle)";
        }
        private void btnRegistrarInforme_Click(object sender, EventArgs e)
        {
            if (!(dgvVisitas.CurrentRow?.DataBoundItem is BE.VisitaTecnica visita))
            {
                MsgBox.Show("Seleccioná una visita.", "Atención", MsgBox.Botones.OK, MsgBox.Icono.Atencion);
                return;
            }

            if (string.IsNullOrWhiteSpace(txtResultado.Text))
            {
                MsgBox.Show("Ingresá el resultado de la revisión.", "Atención",
                    MsgBox.Botones.OK, MsgBox.Icono.Atencion);
                return;
            }

            _informeBLL.RegistrarInforme(visita, visita.AlertaId, txtResultado.Text.Trim(), chkPendienteRepuesto.Checked);

            MsgBox.Show("Informe de mantenimiento registrado. Se notificó al Administrador.", "Éxito",
                MsgBox.Botones.OK, MsgBox.Icono.Exito);
            CargarVisitas();
        }

        private void btnCerrar_Click(object sender, EventArgs e)
        {
            this.Close();
        }
    }
}