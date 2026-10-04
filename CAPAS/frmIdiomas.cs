using BE;
using System;
using System.Collections.Generic;
using System.Windows.Forms;

namespace CAPAS
{
    public partial class frmIdiomas : FormBase
    {
        private readonly BLL.IdiomaBLL _bll = new BLL.IdiomaBLL();
        private IDIOMA _idiomaSeleccionado;

        public frmIdiomas()
        {
            InitializeComponent();
        }

        private void frmIdiomas_Load(object sender, EventArgs e)
        {
            CargarIdiomas();
            InicializarFormulario();
        }

        protected override void ActualizarTextosDinamicos()
        {
            ActualizarEncabezados();
        }

        private void ActualizarEncabezados()
        {
            var mgr = SeguridadYServicios.IdiomaManager.getInstance();
            if (dgvIdiomas.Columns.Count > 0)
            {
                if (dgvIdiomas.Columns["Nombre"] != null)
                    dgvIdiomas.Columns["Nombre"].HeaderText =
                        mgr.Traducir("colhdr_Nombre") ?? "Idioma";
                if (dgvIdiomas.Columns["Habilitado"] != null)
                    dgvIdiomas.Columns["Habilitado"].HeaderText =
                        mgr.Traducir("colhdr_Habilitado") ?? "Habilitado";
            }
            if (dgvTraducciones.Columns.Count > 0)
            {
                if (dgvTraducciones.Columns["Clave"] != null)
                    dgvTraducciones.Columns["Clave"].HeaderText =
                        mgr.Traducir("colhdr_Clave") ?? "Clave";
                if (dgvTraducciones.Columns["TextoDefault"] != null)
                    dgvTraducciones.Columns["TextoDefault"].HeaderText =
                        mgr.Traducir("colhdr_TextoDefault") ?? "Texto por defecto";
                if (dgvTraducciones.Columns["TextoTraduccion"] != null)
                    dgvTraducciones.Columns["TextoTraduccion"].HeaderText =
                        mgr.Traducir("colhdr_TextoTraduccion") ?? "Traducción";
            }
        }

        private void CargarIdiomas()
        {
            var idiomas = _bll.ListarTodos();
            dgvIdiomas.DataSource = null;
            dgvIdiomas.DataSource = idiomas;

            if (dgvIdiomas.Columns.Count > 0)
            {
                dgvIdiomas.Columns["Id"].Visible = false;
                ActualizarEncabezados();
            }
        }

        private void dgvIdiomas_SelectionChanged(object sender, System.EventArgs e)
        {
            _idiomaSeleccionado = dgvIdiomas.CurrentRow?.DataBoundItem as IDIOMA;
            if (_idiomaSeleccionado != null)
                CargarTraducciones();
        }

        private void CargarTraducciones()
        {
            List<CONTROL_IDIOMA> controles = _bll.ListarControlesConTraduccion(_idiomaSeleccionado.Id);
            dgvTraducciones.DataSource = null;
            dgvTraducciones.DataSource = controles;

            if (dgvTraducciones.Columns.Count > 0)
            {
                dgvTraducciones.Columns["Id"].Visible = false;
                dgvTraducciones.Columns["Clave"].ReadOnly = true;
                dgvTraducciones.Columns["TextoDefault"].ReadOnly = true;
                ActualizarEncabezados();
                dgvTraducciones.Columns["TextoTraduccion"].ReadOnly = false;
            }
        }

        private void btnAgregarIdioma_Click(object sender, System.EventArgs e)
        {
            string nombre = Microsoft.VisualBasic.Interaction.InputBox(
                Textos.T("input_NombreDelNuevoIdioma", "Nombre del nuevo idioma:"), Textos.T("input_AgregarIdioma", "Agregar idioma"));
            if (string.IsNullOrWhiteSpace(nombre)) return;

            _bll.Crear(nombre, false);
            CargarIdiomas();
        }

        private void btnEliminarIdioma_Click(object sender, System.EventArgs e)
        {
            if (_idiomaSeleccionado == null)
            {
                MsgBox.Show(Textos.T("msg_SeleccionaUnIdioma", "Seleccioná un idioma."), "Aviso",
                    MsgBox.Botones.OK, MsgBox.Icono.Atencion);
                return;
            }

            if (_idiomaSeleccionado.Predeterminado)
            {
                MsgBox.Show(Textos.T("msg_NoSePuedeEliminarElIdiomaPredeterminadoDelSistema", "No se puede eliminar el idioma predeterminado del sistema."), "Aviso",
                    MsgBox.Botones.OK, MsgBox.Icono.Atencion);
                return;
            }

            if (_bll.EstaEnUso(_idiomaSeleccionado.Id))
            {
                MsgBox.Show(Textos.T("msg_NoSePuedeEliminarUnIdiomaQueEstaEnUsoPorAlgunUsuario", "No se puede eliminar un idioma que está en uso por algún usuario."), "Aviso",
                    MsgBox.Botones.OK, MsgBox.Icono.Atencion);
                return;
            }

            if (MsgBox.Show(
                    Textos.T("msg_EliminarElIdiomaSeBorraranTodasSusTraducciones", "¿Eliminar el idioma '{0}'? Se borrarán todas sus traducciones.", _idiomaSeleccionado.Nombre),
                    "Confirmar eliminación",
                    MsgBox.Botones.SiNo, MsgBox.Icono.Atencion) != DialogResult.Yes)
                return;

            try
            {
                _bll.Eliminar(_idiomaSeleccionado.Id);
            }
            catch (InvalidOperationException ex)
            {
                MsgBox.Show(ex.Message, "Aviso", MsgBox.Botones.OK, MsgBox.Icono.Atencion);
                return;
            }
            _idiomaSeleccionado = null;
            dgvTraducciones.DataSource = null;
            CargarIdiomas();
        }

        private void btnRenombrar_Click(object sender, System.EventArgs e)
        {
            if (_idiomaSeleccionado == null)
            {
                MsgBox.Show(Textos.T("msg_SeleccionaUnIdioma", "Seleccioná un idioma."), "Aviso",
                    MsgBox.Botones.OK, MsgBox.Icono.Atencion);
                return;
            }

            string nuevoNombre = Microsoft.VisualBasic.Interaction.InputBox(
                Textos.T("input_NuevoNombre", "Nuevo nombre:"), Textos.T("input_RenombrarIdioma", "Renombrar idioma"),
                _idiomaSeleccionado.Nombre);

            if (string.IsNullOrWhiteSpace(nuevoNombre)) return;
            if (nuevoNombre == _idiomaSeleccionado.Nombre) return;

            _bll.Renombrar(_idiomaSeleccionado.Id, nuevoNombre);
            CargarIdiomas();
        }

        private void btnToggleHabilitado_Click(object sender, System.EventArgs e)
        {
            if (_idiomaSeleccionado == null)
            {
                MsgBox.Show(Textos.T("msg_SeleccionaUnIdioma", "Seleccioná un idioma."), "Aviso",
                    MsgBox.Botones.OK, MsgBox.Icono.Atencion);
                return;
            }
            try
            {
                _bll.ActualizarEstado(_idiomaSeleccionado.Id, !_idiomaSeleccionado.Habilitado);
            }
            catch (InvalidOperationException ex)
            {
                MsgBox.Show(ex.Message, "Aviso", MsgBox.Botones.OK, MsgBox.Icono.Atencion);
                return;
            }
            CargarIdiomas();
        }

        private void btnGuardarTraducciones_Click(object sender, System.EventArgs e)
        {
            if (_idiomaSeleccionado == null) return;

            dgvTraducciones.CommitEdit(DataGridViewDataErrorContexts.Commit);

            foreach (DataGridViewRow fila in dgvTraducciones.Rows)
            {
                if (!(fila.DataBoundItem is CONTROL_IDIOMA control)) continue;
                if (string.IsNullOrWhiteSpace(control.TextoTraduccion)) continue;
                _bll.GuardarTraduccion(_idiomaSeleccionado.Id, control.Id, control.TextoTraduccion);
            }

            MsgBox.Show(Textos.T("msg_TraduccionesGuardadas", "Traducciones guardadas."), "Éxito",
                MsgBox.Botones.OK, MsgBox.Icono.Exito);

            new BLL.FachadaIdioma().RecargarSiEstaActivo(_idiomaSeleccionado.Id);
        }

        private void btnCerrar_Click(object sender, System.EventArgs e)
        {
            this.Close();
        }
    }
}
