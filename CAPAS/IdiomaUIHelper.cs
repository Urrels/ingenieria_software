using BE;
using System.Windows.Forms;

namespace CAPAS
{
    internal static class IdiomaUIHelper
    {
        /// <summary>
        /// Agrega un StatusStrip con combo de idioma al form recibido.
        /// Llamar al final del Load, después de GuardarDefaults y ActualizarIdioma.
        /// </summary>
        internal static void AgregarSelector(Form form)
        {
            var strip = new StatusStrip { Dock = DockStyle.Bottom };
            strip.Items.Add(new ToolStripLabel("Idioma: "));

            var cbo = new ToolStripComboBox
            {
                DropDownStyle = ComboBoxStyle.DropDownList,
                AutoSize = false,
                Width = 160
            };
            strip.Items.Add(cbo);
            form.Controls.Add(strip);
            form.Height += strip.Height;

            // Poblar el combo con los idiomas habilitados
            IDIOMA activo = SeguridadYServicios.IdiomaManager.getInstance().IdiomaActivo;
            foreach (IDIOMA idioma in new BLL.IdiomaBLL().ListarHabilitados())
            {
                cbo.Items.Add(idioma);
                if (activo != null && idioma.Id == activo.Id)
                    cbo.SelectedItem = idioma;
            }

            // Conectar evento DESPUÉS de poblar para no disparar durante la carga
            cbo.SelectedIndexChanged += (s, e) =>
            {
                if (!(cbo.SelectedItem is IDIOMA idioma)) return;
                var bll = new BLL.IdiomaBLL();
                var traducciones = bll.CargarTraducciones(idioma.Id);
                SeguridadYServicios.IdiomaManager.getInstance().CambiarIdioma(idioma, traducciones);
            };
        }
    }
}
