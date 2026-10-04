using System.Collections.Generic;
using System.Windows.Forms;
using ReaLTaiizor.Forms;
using ReaLTaiizor.Manager;
using SeguridadYServicios;

namespace CAPAS
{
    public class FormBase : MaterialForm, IObservadorIdioma
    {
        protected readonly Dictionary<string, Control> _controles = new Dictionary<string, Control>();
        protected readonly Dictionary<string, string> _defaults = new Dictionary<string, string>();
        private bool _registrado;

        protected void InicializarFormulario()
        {
            GuardarDefaults(Controls);
            AjustarClaves();
            _controles[Name] = this;
            _defaults[Name] = Text;
            IdiomaManager.getInstance().Registrar(this);
            _registrado = true;
            ActualizarIdioma();
            IdiomaUIHelper.AgregarSelector(this);
            MaterialSkinManager.Instance.AddFormToManage(this);
            AppTheme.AplicarTema(this);
        }

        protected virtual void AjustarClaves() { }

        protected void ExcluirDeTraduccion(params Control[] controles)
        {
            foreach (Control c in controles)
            {
                _controles.Remove(Clave(c));
                _defaults.Remove(Clave(c));
            }
        }

        protected static void Encabezado(DataGridView grilla, string columna, string clave, string textoDefault)
        {
            if (grilla.Columns[columna] != null)
                grilla.Columns[columna].HeaderText = Textos.T(clave, textoDefault);
        }

        protected virtual void ActualizarTextosDinamicos() { }

        public void ActualizarIdioma()
        {
            IdiomaManager idiomas = IdiomaManager.getInstance();
            foreach (var kvp in _controles)
                kvp.Value.Text = idiomas.Traducir(kvp.Key) ?? _defaults[kvp.Key];
            ActualizarTextosDinamicos();
        }

        protected override void OnFormClosed(FormClosedEventArgs e)
        {
            if (_registrado)
                IdiomaManager.getInstance().Desregistrar(this);
            base.OnFormClosed(e);
        }

        private string Clave(Control c) => Name + "." + c.Name;

        private void GuardarDefaults(Control.ControlCollection controles)
        {
            foreach (Control c in controles)
            {
                if (!string.IsNullOrEmpty(c.Name) && !string.IsNullOrEmpty(c.Text))
                {
                    _controles[Clave(c)] = c;
                    _defaults[Clave(c)] = c.Text;
                }
                if (c.HasChildren) GuardarDefaults(c.Controls);
            }
        }
    }
}
