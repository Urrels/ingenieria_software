using System;
using System.Windows.Forms;

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

    public partial class frmEvaluarCobertura : FormBase
    {
        private readonly BLL.EvaluacionCoberturaBLL _evaluacionBLL = new BLL.EvaluacionCoberturaBLL();


        public frmEvaluarCobertura()
        {
            InitializeComponent();
        }

        private void frmEvaluarCobertura_Load(object sender, EventArgs e)
        {
            DateTime hoy = DateTime.Today;
            int diasDesdeLunes = ((int)hoy.DayOfWeek - (int)DayOfWeek.Monday + 7) % 7;
            dtpSemana.Value = hoy.AddDays(-diasDesdeLunes - 7);

            InicializarFormulario();
        }

        private void btnEvaluar_Click(object sender, EventArgs e)
        {
            DateTime fecha = dtpSemana.Value.Date;
            int diasDesdeLunes = ((int)fecha.DayOfWeek - (int)DayOfWeek.Monday + 7) % 7;
            DateTime lunes = fecha.AddDays(-diasDesdeLunes);

            var resultados = _evaluacionBLL.EvaluarSemana(lunes);

            if (resultados.Count == 0)
            {
                MsgBox.Show(Textos.T("msg_NoHayUnaGrillaConfirmadaParaEsaSemanaONoTieneTurnosAsignados", "No hay una grilla confirmada para esa semana, o no tiene turnos asignados."),
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
                ActualizarEncabezados();
            }

            MsgBox.Show(Textos.T("msg_EvaluacionDeCoberturaRegistradaSeUsaraParaAjustarLaProximaPl", "Evaluación de cobertura registrada. Se usará para ajustar la próxima planificación."),
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

        private void ActualizarEncabezados()
        {
            Encabezado(dgvResultados, "Franja", "hdr_Franja", "Franja");
            Encabezado(dgvResultados, "Rol", "hdr_Rol", "Rol");
            Encabezado(dgvResultados, "PersonalAsignado", "hdr_PersonalAsignado", "Personal asignado");
            Encabezado(dgvResultados, "SociosAsistidos", "hdr_SociosAsistidos", "Socios asistidos");
            Encabezado(dgvResultados, "Resultado", "hdr_Resultado", "Resultado");
        }

        protected override void ActualizarTextosDinamicos()
        {
            ActualizarEncabezados();
        }
    }
}