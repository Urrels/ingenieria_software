using System;
using System.Collections.Generic;
using System.Linq;
using System.Windows.Forms;

namespace CAPAS
{
    public partial class frmRegistrarUsoMaquina : FormBase
    {
        private readonly BLL.EquipoBLL _equipoBLL = new BLL.EquipoBLL();


        private List<BE.Equipo> _equipos;

        public frmRegistrarUsoMaquina()
        {
            InitializeComponent();
        }

        private void frmRegistrarUsoMaquina_Load(object sender, EventArgs e)
        {
            CargarEquipos();

            InicializarFormulario();
        }

        private void CargarEquipos()
        {
            _equipos = _equipoBLL.ListarTodos().Where(eq => eq.Estado == "Operativo").ToList();
            cboEquipo.DataSource = _equipos;
            cboEquipo.DisplayMember = "Nombre";
            cboEquipo.ValueMember = "Id";
        }

        private void btnRegistrar_Click(object sender, EventArgs e)
        {
            if (!(cboEquipo.SelectedItem is BE.Equipo equipo))
            {
                MsgBox.Show(Textos.T("msg_SeleccionaUnEquipo", "Seleccioná un equipo."), "Atención", MsgBox.Botones.OK, MsgBox.Icono.Atencion);
                return;
            }

            bool tieneUmbral = _equipoBLL.RegistrarUso(equipo.Id);

            if (!tieneUmbral)
            {
                MsgBox.Show(
                    Textos.T("msg_EquipoSinUmbralConfigurado",
                        "El equipo '{0}' no tiene un nivel de uso crítico configurado.\nSe notificará al Administrador para que lo defina.",
                        equipo.Nombre),
                    "Sin umbral configurado", MsgBox.Botones.OK, MsgBox.Icono.Atencion);
                CargarEquipos();
                return;
            }

            MsgBox.Show(Textos.T("msg_RegistroDeUsoGuardadoCorrectamente", "Registro de uso guardado correctamente."), "Éxito",
                MsgBox.Botones.OK, MsgBox.Icono.Exito);
            CargarEquipos();
        }

        private void btnCerrar_Click(object sender, EventArgs e)
        {
            this.Close();
        }
    }
}