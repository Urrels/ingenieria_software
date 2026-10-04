using BE;
using System.Windows.Forms;

namespace CAPAS
{
    internal static class IdiomaUIHelper
    {
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

            var fachada = new BLL.FachadaIdioma();
            IDIOMA activo = fachada.IdiomaActivo;
            foreach (IDIOMA idioma in fachada.ListarDisponibles())
            {
                cbo.Items.Add(idioma);
                if (activo != null && idioma.Id == activo.Id)
                    cbo.SelectedItem = idioma;
            }

            cbo.SelectedIndexChanged += (s, e) =>
            {
                if (cbo.SelectedItem is IDIOMA idioma)
                    fachada.Cambiar(idioma);
            };
        }
    }
}
