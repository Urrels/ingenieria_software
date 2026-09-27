using System;
using System.Collections.Generic;
using System.Windows.Forms;
using ReaLTaiizor.Forms;
using ReaLTaiizor.Manager;

namespace CAPAS
{
    public class EvaluacionFilaVM
    {
        public string Franja { get; set; }
        public string Rol { get; set; }
        public int PersonalAsignado { get; set; }
        public int SociosAsistidos { get; set; }
        public string Resultado { get; set; }
    }

    public partial class frmEvaluarCobertura : MaterialForm, SeguridadYServicios.IObservadorIdioma
    {
        private readonly BLL.EvaluacionCoberturaBLL _evaluacionBLL = new BLL.EvaluacionCoberturaBLL();

        private readonly Dictionary<string, Control> _controles = new Dictionary<string, Control>();
        private readonly Dictionary<string, string> _defaults = new Dictionary<string, string>();

        public frmEvaluarCobertura()
        {
            InitializeComponent();
        }

        private void frmEvaluarCobertura_Load(object sender, EventArgs e)
        {
            DateTime hoy = DateTime.Today;
            int diasDesdeLunes = ((int)hoy.DayOfWeek - (int)DayOfWeek.Monday + 7) % 7;
            dtpSemana.Value = hoy.AddDays(-diasDesdeLunes - 7);

            GuardarDefaults(this.Controls);
            _controles[this.Name] = this;
            _defaults[this.Name] = this.Text;
            SeguridadYServicios.IdiomaManager.getInstance().Registrar(this);
            ActualizarIdioma();
            IdiomaUIHelper.AgregarSelector(this);
            MaterialSkinManager.Instance.AddFormToManage(this);
            AppTheme.AplicarTema(this);
        }

        private void frmEvaluarCobertura_FormClosed(object sender, FormClosedEventArgs e)
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

        private void btnEvaluar_Click(object sender, EventArgs e)
        {
            DateTime fecha = dtpSemana.Value.Date;
            int diasDesdeLunes = ((int)fecha.DayOfWeek - (int)DayOfWeek.Monday + 7) % 7;
            DateTime lunes = fecha.AddDays(-diasDesdeLunes);

            var resultados = _evaluacionBLL.EvaluarSemana(lunes);

            if (resultados.Count == 0)
            {
                MsgBox.Show("No hay una grilla confirmada para esa semana, o no tiene turnos asignados.",
                    "Atención", MsgBox.Botones.OK, MsgBox.Icono.Atencion);
                dgvResultados.DataSource = null;
                return;
            }

            var filas = resultados.ConvertAll(r => new EvaluacionFilaVM
            {
                Franja = $"{r.Franja.Dia}  {r.Franja.HoraInicio:hh\\:mm}-{r.Franja.HoraFin:hh\\:mm}",
                Rol = r.Franja.RolRequerido,
                PersonalAsignado = r.PersonalAsignado,
                SociosAsistidos = r.SociosAsistidos,
                Resultado = r.Resultado
            });

            dgvResultados.DataSource = null;
            dgvResultados.DataSource = filas;

            if (dgvResultados.Columns.Count > 0)
            {
                dgvResultados.Columns["Franja"].HeaderText = "Franja";
                dgvResultados.Columns["Rol"].HeaderText = "Rol";
                dgvResultados.Columns["PersonalAsignado"].HeaderText = "Personal asignado";
                dgvResultados.Columns["SociosAsistidos"].HeaderText = "Socios asistidos";
                dgvResultados.Columns["Resultado"].HeaderText = "Resultado";
            }

            MsgBox.Show("Evaluación de cobertura registrada. Se usará para ajustar la próxima planificación.",
                "Éxito", MsgBox.Botones.OK, MsgBox.Icono.Exito);
        }

        private void btnExportar_Click(object sender, EventArgs e)
        {
            ExcelExportHelper.ExportarDataGridView(dgvResultados, "EvaluacionCobertura");
        }

        private void btnCerrar_Click(object sender, EventArgs e)
        {
            this.Close();
        }
    }
}