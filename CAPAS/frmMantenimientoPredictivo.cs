using System;
using System.Collections.Generic;
using System.Linq;
using System.Windows.Forms;
using ReaLTaiizor.Forms;
using ReaLTaiizor.Manager;

namespace CAPAS
{
    public class PrediccionFilaVM
    {
        public string Equipo { get; set; }
        public int UsoAcumulado { get; set; }
        public int NivelUsoCritico { get; set; }
        public string DiasEstimados { get; set; }
        public string Urgencia { get; set; }
    }

    public partial class frmMantenimientoPredictivo : MaterialForm, SeguridadYServicios.IObservadorIdioma
    {
        private readonly BLL.EquipoBLL _equipoBLL = new BLL.EquipoBLL();

        private readonly Dictionary<string, Control> _controles = new Dictionary<string, Control>();
        private readonly Dictionary<string, string> _defaults = new Dictionary<string, string>();

        public frmMantenimientoPredictivo()
        {
            InitializeComponent();
        }

        private void frmMantenimientoPredictivo_Load(object sender, EventArgs e)
        {
            CargarPredicciones();

            GuardarDefaults(this.Controls);
            _controles[this.Name] = this;
            _defaults[this.Name] = this.Text;
            SeguridadYServicios.IdiomaManager.getInstance().Registrar(this);
            ActualizarIdioma();
            IdiomaUIHelper.AgregarSelector(this);
            MaterialSkinManager.Instance.AddFormToManage(this);
            AppTheme.AplicarTema(this);
        }

        private void CargarPredicciones()
        {
            var equiposConUmbral = _equipoBLL.ListarTodos().Where(eq => eq.NivelUsoCritico.HasValue).ToList();

            var filas = new List<(BE.Equipo equipo, int? dias)>();
            foreach (var equipo in equiposConUmbral)
                filas.Add((equipo, _equipoBLL.EstimarDiasHastaCritico(equipo)));

            var ordenadas = filas
                .OrderBy(f => f.dias.HasValue ? 0 : 1)
                .ThenBy(f => f.dias ?? int.MaxValue)
                .Select(f => new PrediccionFilaVM
                {
                    Equipo = f.equipo.Nombre,
                    UsoAcumulado = f.equipo.UsoAcumulado,
                    NivelUsoCritico = f.equipo.NivelUsoCritico.Value,
                    DiasEstimados = f.dias.HasValue ? f.dias.Value.ToString() : "Sin datos recientes",
                    Urgencia = !f.dias.HasValue ? "—" : f.dias.Value <= 3 ? "Alta" : f.dias.Value <= 10 ? "Media" : "Baja"
                })
                .ToList();

            dgvPredicciones.DataSource = null;
            dgvPredicciones.DataSource = ordenadas;

            if (dgvPredicciones.Columns.Count > 0)
            {
                dgvPredicciones.Columns["Equipo"].HeaderText = "Equipo";
                dgvPredicciones.Columns["UsoAcumulado"].HeaderText = "Uso acumulado";
                dgvPredicciones.Columns["NivelUsoCritico"].HeaderText = "Umbral crítico";
                dgvPredicciones.Columns["DiasEstimados"].HeaderText = "Días estimados restantes";
                dgvPredicciones.Columns["Urgencia"].HeaderText = "Urgencia";
            }
        }

        private void frmMantenimientoPredictivo_FormClosed(object sender, FormClosedEventArgs e)
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

        private void btnActualizar_Click(object sender, EventArgs e)
        {
            CargarPredicciones();
        }

        private void btnExportar_Click(object sender, EventArgs e)
        {
            ExcelExportHelper.ExportarDataGridView(dgvPredicciones, "MantenimientoPredictivo");
        }

        private void btnCerrar_Click(object sender, EventArgs e)
        {
            this.Close();
        }
    }
}