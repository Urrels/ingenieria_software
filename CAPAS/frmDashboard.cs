using System;
using System.Collections.Generic;
using System.Windows.Forms;
using System.Windows.Forms.DataVisualization.Charting;
using ReaLTaiizor.Forms;
using ReaLTaiizor.Manager;

namespace CAPAS
{
    public partial class frmDashboard : MaterialForm, SeguridadYServicios.IObservadorIdioma
    {
        private readonly BLL.EquipoBLL _equipoBLL = new BLL.EquipoBLL();
        private readonly BLL.EvaluacionCoberturaBLL _evaluacionBLL = new BLL.EvaluacionCoberturaBLL();
        private readonly BLL.HistorialAsistenciaSociosBLL _historialBLL = new BLL.HistorialAsistenciaSociosBLL();
        private readonly BLL.FranjaHorariaBLL _franjaBLL = new BLL.FranjaHorariaBLL();

        private readonly Dictionary<string, Control> _controles = new Dictionary<string, Control>();
        private readonly Dictionary<string, string> _defaults = new Dictionary<string, string>();

        private Chart _chartCobertura;
        private Chart _chartEquipos;
        private Chart _chartAsistencia;

        private List<BE.FranjaHoraria> _franjas;

        public frmDashboard()
        {
            InitializeComponent();
        }

        private void frmDashboard_Load(object sender, EventArgs e)
        {
            ArmarCharts();
            CargarFranjasCombo();
            CargarTodo();

            GuardarDefaults(this.Controls);
            _controles[this.Name] = this;
            _defaults[this.Name] = this.Text;
            SeguridadYServicios.IdiomaManager.getInstance().Registrar(this);
            ActualizarIdioma();
            IdiomaUIHelper.AgregarSelector(this);
            MaterialSkinManager.Instance.AddFormToManage(this);
            AppTheme.AplicarTema(this);
        }

        private void ArmarCharts()
        {
            _chartCobertura = CrearChartBase();
            pnlChartCobertura.Controls.Add(_chartCobertura);

            _chartEquipos = CrearChartBase();
            pnlChartEquipos.Controls.Add(_chartEquipos);

            _chartAsistencia = CrearChartBase();
            pnlChartAsistencia.Controls.Add(_chartAsistencia);
        }

        private Chart CrearChartBase()
        {
            var chart = new Chart { Dock = DockStyle.Fill, BackColor = System.Drawing.Color.WhiteSmoke };
            var area = new ChartArea("area");
            chart.ChartAreas.Add(area);
            chart.Legends.Add(new Legend("leyenda"));
            return chart;
        }

        private void CargarFranjasCombo()
        {
            _franjas = _franjaBLL.ListarTodas();
            cboFranjaAsistencia.DataSource = _franjas;
            cboFranjaAsistencia.DisplayMember = "Dia";
            cboFranjaAsistencia.ValueMember = "Id";
            cboFranjaAsistencia.Format += (s, ev) =>
            {
                var f = (BE.FranjaHoraria)ev.ListItem;
                ev.Value = $"{f.Dia} {f.HoraInicio:hh\\:mm}-{f.HoraFin:hh\\:mm} ({f.RolRequerido})";
            };
        }

        private void CargarTodo()
        {
            CargarChartCobertura();
            CargarChartEquipos();
            CargarChartAsistencia();
        }

        private void CargarChartCobertura()
        {
            _chartCobertura.Series.Clear();
            _chartCobertura.Titles.Clear();
            _chartCobertura.Titles.Add("Evaluación de cobertura (Proceso 1 + 3)");

            var resumen = _evaluacionBLL.ObtenerResumen();

            var serie = new Series("Cantidad de franjas") { ChartType = SeriesChartType.Column };
            foreach (var kvp in resumen)
                serie.Points.AddXY(kvp.Key, kvp.Value);

            _chartCobertura.Series.Add(serie);
        }

        private void CargarChartEquipos()
        {
            _chartEquipos.Series.Clear();
            _chartEquipos.Titles.Clear();
            _chartEquipos.Titles.Add("Uso acumulado por equipo (Proceso 2)");

            var equipos = _equipoBLL.ListarTodos();

            var serieUso = new Series("Uso acumulado") { ChartType = SeriesChartType.Column };
            var serieUmbral = new Series("Umbral crítico")
            {
                ChartType = SeriesChartType.Line,
                BorderWidth = 3,
                Color = System.Drawing.Color.Red
            };

            foreach (var eq in equipos)
            {
                serieUso.Points.AddXY(eq.Nombre, eq.UsoAcumulado);
                serieUmbral.Points.AddXY(eq.Nombre, eq.NivelUsoCritico ?? 0);
            }

            _chartEquipos.Series.Add(serieUso);
            _chartEquipos.Series.Add(serieUmbral);
        }

        private void CargarChartAsistencia()
        {
            if (!(cboFranjaAsistencia.SelectedItem is BE.FranjaHoraria franja)) return;

            _chartAsistencia.Series.Clear();
            _chartAsistencia.Titles.Clear();
            _chartAsistencia.Titles.Add("Asistencia de socios en el tiempo (Proceso 3)");

            var serie = new Series("Socios")
            {
                ChartType = SeriesChartType.Line,
                BorderWidth = 3,
                MarkerStyle = MarkerStyle.Circle
            };

            var puntos = _historialBLL.ObtenerSerieCompleta(franja.Id);
            foreach (var punto in puntos)
                serie.Points.AddXY(punto.fecha.ToString("dd/MM"), punto.cantidad);

            _chartAsistencia.Series.Add(serie);
        }

        private void cboFranjaAsistencia_SelectedIndexChanged(object sender, EventArgs e)
        {
            CargarChartAsistencia();
        }

        private void frmDashboard_FormClosed(object sender, FormClosedEventArgs e)
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
            CargarTodo();
        }

        private void btnCerrar_Click(object sender, EventArgs e)
        {
            this.Close();
        }
    }
}