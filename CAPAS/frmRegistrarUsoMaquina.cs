using System;
using System.Collections.Generic;
using System.Linq;
using System.Windows.Forms;
using ReaLTaiizor.Forms;
using ReaLTaiizor.Manager;

namespace CAPAS
{
    public partial class frmRegistrarUsoMaquina : MaterialForm, SeguridadYServicios.IObservadorIdioma
    {
        private readonly BLL.EquipoBLL _equipoBLL = new BLL.EquipoBLL();

        private readonly Dictionary<string, Control> _controles = new Dictionary<string, Control>();
        private readonly Dictionary<string, string> _defaults = new Dictionary<string, string>();

        private List<BE.Equipo> _equipos;

        public frmRegistrarUsoMaquina()
        {
            InitializeComponent();
        }

        private void frmRegistrarUsoMaquina_Load(object sender, EventArgs e)
        {
            CargarEquipos();

            GuardarDefaults(this.Controls);
            _controles[this.Name] = this;
            _defaults[this.Name] = this.Text;
            SeguridadYServicios.IdiomaManager.getInstance().Registrar(this);
            ActualizarIdioma();
            IdiomaUIHelper.AgregarSelector(this);
            MaterialSkinManager.Instance.AddFormToManage(this);
            AppTheme.AplicarTema(this);
        }

        private void CargarEquipos()
        {
            _equipos = _equipoBLL.ListarTodos().Where(eq => eq.Estado == "Operativo").ToList();
            cboEquipo.DataSource = _equipos;
            cboEquipo.DisplayMember = "Nombre";
            cboEquipo.ValueMember = "Id";
        }

        private void frmRegistrarUsoMaquina_FormClosed(object sender, FormClosedEventArgs e)
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
            if (!(cboEquipo.SelectedItem is BE.Equipo equipo))
            {
                MsgBox.Show("Seleccioná un equipo.", "Atención", MsgBox.Botones.OK, MsgBox.Icono.Atencion);
                return;
            }

            bool tieneUmbral = _equipoBLL.RegistrarUso(equipo.Id);

            if (!tieneUmbral)
            {
                // flujo 3a
                MsgBox.Show(
                    $"El equipo '{equipo.Nombre}' no tiene un nivel de uso crítico configurado.\n" +
                    "Se notificará al Administrador para que lo defina.",
                    "Sin umbral configurado", MsgBox.Botones.OK, MsgBox.Icono.Atencion);
                CargarEquipos();
                return;
            }

            MsgBox.Show("Registro de uso guardado correctamente.", "Éxito",
                MsgBox.Botones.OK, MsgBox.Icono.Exito);
            CargarEquipos();
        }

        private void btnCerrar_Click(object sender, EventArgs e)
        {
            this.Close();
        }
    }
}