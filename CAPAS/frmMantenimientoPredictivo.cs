using System;
using System.Collections.Generic;
using System.Linq;
using System.Windows.Forms;

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

    public partial class frmMantenimientoPredictivo : FormBase
    {
        private readonly BLL.EquipoBLL _equipoBLL = new BLL.EquipoBLL();


        public frmMantenimientoPredictivo()
        {
            InitializeComponent();
        }

        private void frmMantenimientoPredictivo_Load(object sender, EventArgs e)
        {
            CargarPredicciones();

            InicializarFormulario();
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
                ActualizarEncabezados();
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

        private void ActualizarEncabezados()
        {
            Encabezado(dgvPredicciones, "Equipo", "hdr_Equipo", "Equipo");
            Encabezado(dgvPredicciones, "UsoAcumulado", "hdr_UsoAcumulado", "Uso acumulado");
            Encabezado(dgvPredicciones, "NivelUsoCritico", "hdr_UmbralCritico", "Umbral crítico");
            Encabezado(dgvPredicciones, "DiasEstimados", "hdr_DiasEstimadosRestantes", "Días estimados restantes");
            Encabezado(dgvPredicciones, "Urgencia", "hdr_Urgencia", "Urgencia");
        }

        protected override void ActualizarTextosDinamicos()
        {
            ActualizarEncabezados();
        }
    }
}