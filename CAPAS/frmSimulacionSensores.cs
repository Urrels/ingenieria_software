using Newtonsoft.Json;
using ReaLTaiizor.Forms;
using ReaLTaiizor.Manager;
using System;
using System.Collections.Generic;
using System.Net.Http;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;

namespace CAPAS
{
    public partial class frmSimulacionSensores : MaterialForm
    {
        private readonly ApiSensoresEquipos _api = new ApiSensoresEquipos();
        private readonly HttpClient _http = new HttpClient();
        private readonly Random _random = new Random();
        private bool _apiIniciada;

        private static readonly string[] NombresEquipos =
            { "Cinta 1", "Bicicleta fija 1", "Prensa de piernas", "Remo 1" };

        public frmSimulacionSensores()
        {
            InitializeComponent();
            _api.OnEvento += MostrarEvento;
        }

        private void frmSimulacionSensores_Load(object sender, EventArgs e)
        {
            MaterialSkinManager.Instance.AddFormToManage(this);
            AppTheme.AplicarTema(this);
        }

        private void MostrarEvento(string mensaje)
        {
            if (lstLog.InvokeRequired)
            {
                lstLog.Invoke(new Action(() => MostrarEvento(mensaje)));
                return;
            }
            lstLog.Items.Insert(0, mensaje);
        }

        private void btnIniciarApi_Click(object sender, EventArgs e)
        {
            if (_apiIniciada) return;
            try
            {
                _api.Iniciar(5050);
                _apiIniciada = true;
                btnIniciarApi.Enabled = false;
                btnDetenerApi.Enabled = true;
                btnIniciarSimulacion.Enabled = true;
            }
            catch (Exception ex)
            {
                MsgBox.Show("No se pudo iniciar la API: " + ex.Message, "Error",
                    MsgBox.Botones.OK, MsgBox.Icono.Error);
            }
        }

        private void btnDetenerApi_Click(object sender, EventArgs e)
        {
            timerSimulacion.Stop();
            _api.Detener();
            _apiIniciada = false;
            btnIniciarApi.Enabled = true;
            btnDetenerApi.Enabled = false;
            btnIniciarSimulacion.Enabled = false;
            btnDetenerSimulacion.Enabled = false;
        }

        private void btnIniciarSimulacion_Click(object sender, EventArgs e)
        {
            timerSimulacion.Interval = (int)numIntervalo.Value * 1000;
            timerSimulacion.Start();
            btnIniciarSimulacion.Enabled = false;
            btnDetenerSimulacion.Enabled = true;
            MostrarEvento($"[{DateTime.Now:HH:mm:ss}] Simulación de sensores iniciada (cada {numIntervalo.Value}s).");
        }

        private void btnDetenerSimulacion_Click(object sender, EventArgs e)
        {
            timerSimulacion.Stop();
            btnIniciarSimulacion.Enabled = true;
            btnDetenerSimulacion.Enabled = false;
            MostrarEvento($"[{DateTime.Now:HH:mm:ss}] Simulación de sensores detenida.");
        }

        private async void timerSimulacion_Tick(object sender, EventArgs e)
        {
            string equipoElegido = NombresEquipos[_random.Next(NombresEquipos.Length)];
            await EnviarUsoAsync(equipoElegido);
        }

        private async Task EnviarUsoAsync(string nombreEquipo)
        {
            try
            {
                var payload = new UsoEquipoRequest { NombreEquipo = nombreEquipo, Incremento = 1 };
                string json = JsonConvert.SerializeObject(payload);
                var contenido = new StringContent(json, Encoding.UTF8, "application/json");

                await _http.PostAsync("http://localhost:5050/api/equipos/uso", contenido);
            }
            catch (Exception ex)
            {
                MostrarEvento($"[{DateTime.Now:HH:mm:ss}] Error de red simulando sensor: {ex.Message}");
            }
        }

        private void frmSimulacionSensores_FormClosing(object sender, FormClosingEventArgs e)
        {
            timerSimulacion.Stop();
            if (_apiIniciada) _api.Detener();
        }

        private void btnCerrar_Click(object sender, EventArgs e)
        {
            this.Close();
        }
    }
}