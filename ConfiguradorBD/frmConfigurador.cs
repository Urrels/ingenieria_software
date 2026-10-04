using System;
using System.Collections.Generic;
using System.Data.SqlClient;
using System.Diagnostics;
using System.Drawing;
using System.IO;
using System.Linq;
using System.ServiceProcess;
using System.Threading.Tasks;
using System.Windows.Forms;

namespace ConfiguradorBD
{
    internal class frmConfigurador : Form
    {
        private const string UrlLocalDb = "https://learn.microsoft.com/sql/database-engine/configure-windows/sql-server-express-localdb";
        private const string UrlSqlExpress = "https://www.microsoft.com/sql-server/sql-server-downloads";
        private const long EspacioMinimoBytes = 300L * 1024 * 1024;

        private static readonly Color Acento = Color.FromArgb(59, 111, 182);
        private static readonly Color Texto = Color.FromArgb(43, 45, 66);

        private readonly RegistroInstalacion _log = new RegistroInstalacion();
        private List<InstanciaSql> _instancias = new List<InstanciaSql>();

        private readonly ComboBox cboServidor = new ComboBox();
        private readonly Label lblEstadoServicio = new Label();
        private readonly RadioButton rbWindows = new RadioButton();
        private readonly RadioButton rbSql = new RadioButton();
        private readonly TextBox txtUsuario = new TextBox();
        private readonly TextBox txtContrasena = new TextBox();
        private readonly Button btnBuscar = new Button();
        private readonly Button btnProbar = new Button();
        private readonly Button btnInstalar = new Button();
        private readonly Button btnCerrar = new Button();
        private readonly Panel pnlSinMotor = new Panel();
        private readonly Button btnInstalarMotor = new Button();
        private readonly ProgressBar prgProgreso = new ProgressBar();
        private readonly TextBox txtRegistro = new TextBox();

        internal frmConfigurador()
        {
            ArmarPantalla();
            _log.LineaAgregada += AgregarAlRegistro;
            Load += async (s, e) => await BuscarInstanciasAsync();
        }

        private void ArmarPantalla()
        {
            Text = "Configuración de la base de datos";
            Font = new Font("Segoe UI", 9.75f);
            BackColor = Color.White;
            ForeColor = Texto;
            FormBorderStyle = FormBorderStyle.FixedDialog;
            MaximizeBox = false;
            StartPosition = FormStartPosition.CenterScreen;
            ClientSize = new Size(640, 600);

            var lblTitulo = new Label
            {
                Text = "Configuración de la base de datos",
                Font = new Font("Segoe UI", 14f, FontStyle.Bold),
                ForeColor = Acento,
                Location = new Point(20, 15),
                AutoSize = true
            };
            var lblDescripcion = new Label
            {
                Text = "Elegí dónde crear la base de datos: una instancia de SQL Server de este equipo " +
                       "(se buscan solas) o un servidor de la red.",
                Location = new Point(20, 48),
                Size = new Size(600, 50)
            };

            var lblServidor = new Label { Text = "Servidor / instancia:", Location = new Point(20, 100), AutoSize = true };
            cboServidor.Location = new Point(20, 122);
            cboServidor.Size = new Size(440, 26);
            cboServidor.DropDownStyle = ComboBoxStyle.DropDown;
            cboServidor.TextChanged += (s, e) => ActualizarEstadoServicio();

            ConfigurarBoton(btnBuscar, "Buscar instancias", new Point(470, 120), new Size(150, 30), false);
            btnBuscar.Click += async (s, e) => await BuscarInstanciasAsync();

            lblEstadoServicio.Location = new Point(20, 152);
            lblEstadoServicio.Size = new Size(600, 20);

            pnlSinMotor.Location = new Point(20, 175);
            pnlSinMotor.Size = new Size(600, 70);
            pnlSinMotor.BackColor = Color.FromArgb(253, 243, 247);
            pnlSinMotor.Visible = false;
            var lblSinMotor = new Label
            {
                Text = "No se encontró ningún motor de SQL Server en este equipo. Podés instalar " +
                       "SQL Server Express LocalDB (gratuito) o escribir arriba un servidor de la red.",
                Location = new Point(10, 8),
                Size = new Size(380, 55)
            };
            ConfigurarBoton(btnInstalarMotor, "Instalar LocalDB", new Point(400, 18), new Size(190, 34), true);
            btnInstalarMotor.Click += async (s, e) => await InstalarMotorAsync();
            pnlSinMotor.Controls.Add(lblSinMotor);
            pnlSinMotor.Controls.Add(btnInstalarMotor);

            var grpAutenticacion = new GroupBox
            {
                Text = "Autenticación",
                Location = new Point(20, 250),
                Size = new Size(600, 115)
            };
            rbWindows.Text = "Autenticación de Windows (recomendada)";
            rbWindows.Location = new Point(15, 25);
            rbWindows.AutoSize = true;
            rbWindows.Checked = true;
            rbSql.Text = "Autenticación de SQL Server";
            rbSql.Location = new Point(15, 50);
            rbSql.AutoSize = true;
            rbSql.CheckedChanged += (s, e) => { txtUsuario.Enabled = txtContrasena.Enabled = rbSql.Checked; };
            var lblUsuario = new Label { Text = "Usuario:", Location = new Point(35, 80), AutoSize = true };
            txtUsuario.Location = new Point(100, 77);
            txtUsuario.Size = new Size(180, 26);
            txtUsuario.Enabled = false;
            var lblContrasena = new Label { Text = "Contraseña:", Location = new Point(300, 80), AutoSize = true };
            txtContrasena.Location = new Point(390, 77);
            txtContrasena.Size = new Size(190, 26);
            txtContrasena.UseSystemPasswordChar = true;
            txtContrasena.Enabled = false;
            grpAutenticacion.Controls.AddRange(new Control[] { rbWindows, rbSql, lblUsuario, txtUsuario, lblContrasena, txtContrasena });

            ConfigurarBoton(btnProbar, "Probar conexión", new Point(20, 378), new Size(160, 36), false);
            btnProbar.Click += async (s, e) => await ProbarConexionAsync();
            ConfigurarBoton(btnInstalar, "Crear base de datos", new Point(190, 378), new Size(190, 36), true);
            btnInstalar.Click += async (s, e) => await InstalarAsync();
            ConfigurarBoton(btnCerrar, "Cerrar", new Point(520, 378), new Size(100, 36), false);
            btnCerrar.Click += (s, e) => Close();

            prgProgreso.Location = new Point(20, 425);
            prgProgreso.Size = new Size(600, 8);
            prgProgreso.Style = ProgressBarStyle.Marquee;
            prgProgreso.Visible = false;

            txtRegistro.Location = new Point(20, 440);
            txtRegistro.Size = new Size(600, 145);
            txtRegistro.Multiline = true;
            txtRegistro.ReadOnly = true;
            txtRegistro.ScrollBars = ScrollBars.Vertical;
            txtRegistro.Font = new Font("Consolas", 8.5f);
            txtRegistro.BackColor = Color.FromArgb(232, 240, 251);

            Controls.AddRange(new Control[]
            {
                lblTitulo, lblDescripcion, lblServidor, cboServidor, btnBuscar, lblEstadoServicio,
                pnlSinMotor, grpAutenticacion, btnProbar, btnInstalar, btnCerrar, prgProgreso, txtRegistro
            });
        }

        private static void ConfigurarBoton(Button boton, string texto, Point ubicacion, Size tamanio, bool principal)
        {
            boton.Text = texto;
            boton.Location = ubicacion;
            boton.Size = tamanio;
            boton.FlatStyle = FlatStyle.Flat;
            boton.FlatAppearance.BorderColor = Acento;
            boton.BackColor = principal ? Acento : Color.White;
            boton.ForeColor = principal ? Color.White : Acento;
            boton.Cursor = Cursors.Hand;
        }

        private async Task BuscarInstanciasAsync()
        {
            await EjecutarAsync(() =>
            {
                _log.Escribir("Buscando instancias de SQL Server en este equipo...");
                _instancias = InstanciaSql.DetectarLocales();
                _log.Escribir(_instancias.Count == 0
                    ? "No se encontró ninguna instancia local."
                    : "Instancias encontradas: " + string.Join(", ", _instancias.Select(i => i.Servidor)));
            });

            pnlSinMotor.Visible = _instancias.Count == 0;
            string anterior = ServidorDeConfiguracion();
            cboServidor.Items.Clear();
            cboServidor.Items.AddRange(_instancias.Cast<object>().ToArray());
            if (!string.IsNullOrEmpty(anterior))
                cboServidor.Text = anterior;
            else if (_instancias.Count > 0)
                cboServidor.SelectedIndex = 0;
            ActualizarEstadoServicio();
        }

        private static string ServidorDeConfiguracion()
        {
            try
            {
                string cadena = ConfiguracionApp.LeerCadena();
                return string.IsNullOrEmpty(cadena) ? null : new SqlConnectionStringBuilder(cadena).DataSource;
            }
            catch (Exception ex) when (ex is ArgumentException || ex is FormatException || ex is System.Xml.XmlException)
            {
                return null;
            }
        }

        private void ActualizarEstadoServicio()
        {
            string servidor = cboServidor.Text.Trim();
            if (servidor.StartsWith("(localdb)", StringComparison.OrdinalIgnoreCase))
            {
                lblEstadoServicio.Text = "LocalDB: la instancia se crea e inicia automáticamente.";
                lblEstadoServicio.ForeColor = Texto;
                return;
            }
            string servicio = InstanciaSql.ServicioDe(servidor);
            ServiceControllerStatus? estado = InstanciaSql.EstadoServicio(servicio);
            if (servicio == null)
                lblEstadoServicio.Text = servidor.Length == 0 ? "" : "Servidor remoto: se verifica al probar la conexión.";
            else if (estado == null)
                lblEstadoServicio.Text = $"No existe el servicio {servicio} en este equipo.";
            else
                lblEstadoServicio.Text = $"Servicio {servicio}: " +
                    (estado == ServiceControllerStatus.Running ? "en ejecución" : "detenido");
            lblEstadoServicio.ForeColor = estado == ServiceControllerStatus.Running || servicio == null
                ? Texto : Color.FromArgb(194, 56, 107);
        }

        private async Task ProbarConexionAsync()
        {
            SqlConnectionStringBuilder cadena = ArmarCadena();
            if (cadena == null) return;
            if (!VerificarPuerto(cadena.DataSource)) return;
            if (!PrepararMotor(cadena.DataSource)) return;

            ResultadoVerificacion resultado = null;
            bool ok = await EjecutarAsync(() =>
            {
                _log.Escribir("Probando la conexión con " + cadena.DataSource + "...");
                resultado = new InstaladorBaseDatos(cadena, _log).Verificar();
            });
            if (!ok) return;

            _log.Escribir($"Conectado: SQL Server {resultado.Version} ({resultado.Edicion}).");
            string mensaje = $"Conexión exitosa.\n\nSQL Server {resultado.Version} ({resultado.Edicion})\n" +
                             (resultado.ExisteBase ? "La base de datos ya existe en este servidor." : "La base de datos todavía no existe.");
            if (!resultado.VersionCompatible)
                mensaje += "\n\nAtención: se necesita SQL Server 2019 o posterior.";
            MessageBox.Show(this, mensaje, "Probar conexión", MessageBoxButtons.OK,
                resultado.VersionCompatible ? MessageBoxIcon.Information : MessageBoxIcon.Warning);
        }

        private async Task InstalarAsync()
        {
            SqlConnectionStringBuilder cadena = ArmarCadena();
            if (cadena == null) return;

            string esquema = BuscarScript("script.sql");
            string traducciones = BuscarScript("traducciones.sql");
            if (esquema == null || traducciones == null)
            {
                MessageBox.Show(this, "No se encontraron los scripts script.sql y traducciones.sql en la carpeta Scripts.",
                    "Faltan archivos", MessageBoxButtons.OK, MessageBoxIcon.Error);
                return;
            }
            if (!VerificarEspacioEnDisco(cadena.DataSource)) return;
            if (!VerificarPuerto(cadena.DataSource)) return;
            if (!PrepararMotor(cadena.DataSource)) return;

            var instalador = new InstaladorBaseDatos(cadena, _log);
            ResultadoVerificacion verificacion = null;
            if (!await EjecutarAsync(() => verificacion = instalador.Verificar())) return;

            if (!verificacion.VersionCompatible)
            {
                MessageBox.Show(this, $"El servidor tiene SQL Server {verificacion.Version}. Se necesita SQL Server 2019 o posterior.",
                    "Versión no compatible", MessageBoxButtons.OK, MessageBoxIcon.Error);
                return;
            }

            bool soloTraducciones = false;
            if (verificacion.ExisteBase)
            {
                if (MessageBox.Show(this,
                        "La base de datos ya existe en este servidor y no se va a modificar su estructura ni sus datos.\n\n" +
                        "¿Querés actualizar las traducciones y configurar el sistema para usar esta base?",
                        "La base ya existe", MessageBoxButtons.YesNo, MessageBoxIcon.Question) != DialogResult.Yes)
                    return;
                soloTraducciones = true;
            }
            else if (!verificacion.PuedeCrearBases)
            {
                MessageBox.Show(this, "El usuario no tiene permiso para crear bases de datos en este servidor. " +
                                      "Usá un usuario administrador del SQL Server.",
                    "Permisos insuficientes", MessageBoxButtons.OK, MessageBoxIcon.Error);
                return;
            }

            bool ok = await EjecutarAsync(() =>
            {
                if (soloTraducciones)
                    instalador.ActualizarTraducciones(traducciones);
                else
                    instalador.CrearBase(esquema, traducciones);
                ConfiguracionApp.GuardarCadena(instalador.CadenaParaAplicacion());
                _log.Escribir("El sistema quedó configurado para usar " + cadena.DataSource + ".");
            });
            if (!ok) return;

            MessageBox.Show(this,
                "La base de datos quedó lista y el sistema configurado para usarla.\n\n" +
                "Usuario inicial: admin\nContraseña: 1234\n\nCambiá la contraseña después del primer ingreso.",
                "Instalación completa", MessageBoxButtons.OK, MessageBoxIcon.Information);
        }

        private async Task InstalarMotorAsync()
        {
            string msi = Path.Combine(AppDomain.CurrentDomain.BaseDirectory, "Prerequisitos", "SqlLocalDB.msi");
            if (!File.Exists(msi))
            {
                if (MessageBox.Show(this,
                        "Este instalador no incluye LocalDB. Se va a abrir la página oficial de Microsoft para descargarlo.\n\n" +
                        "Después de instalarlo, volvé a esta pantalla y tocá \"Buscar instancias\".",
                        "Descargar LocalDB", MessageBoxButtons.OKCancel, MessageBoxIcon.Information) == DialogResult.OK)
                    AbrirEnNavegador(UrlLocalDb);
                return;
            }

            bool instalado = false;
            if (await EjecutarAsync(() => instalado = LocalDb.InstalarSilencioso(msi, _log)) && instalado)
            {
                await BuscarInstanciasAsync();
                cboServidor.Text = LocalDb.Servidor;
            }
            else if (MessageBox.Show(this, "No se pudo instalar LocalDB automáticamente. ¿Abrir la página de descarga de SQL Server Express?",
                         "Instalación fallida", MessageBoxButtons.YesNo, MessageBoxIcon.Warning) == DialogResult.Yes)
            {
                AbrirEnNavegador(UrlSqlExpress);
            }
        }

        private bool PrepararMotor(string servidor)
        {
            if (servidor.StartsWith("(localdb)", StringComparison.OrdinalIgnoreCase))
                return EjecutarSincronico(() => LocalDb.AsegurarInstancia(_log));

            string servicio = InstanciaSql.ServicioDe(servidor);
            ServiceControllerStatus? estado = InstanciaSql.EstadoServicio(servicio);
            if (servicio == null || estado == null || estado == ServiceControllerStatus.Running) return true;

            if (MessageBox.Show(this, $"El servicio de SQL Server ({servicio}) está detenido.\n\n¿Querés iniciarlo ahora?",
                    "Servicio detenido", MessageBoxButtons.YesNo, MessageBoxIcon.Question) != DialogResult.Yes)
                return false;

            bool ok = EjecutarSincronico(() =>
            {
                _log.Escribir("Iniciando el servicio " + servicio + "...");
                InstanciaSql.IniciarServicio(servicio, TimeSpan.FromSeconds(60));
                _log.Escribir("Servicio " + servicio + " iniciado.");
            });
            ActualizarEstadoServicio();
            if (!ok)
                MessageBox.Show(this, "No se pudo iniciar el servicio. Verificá en services.msc que no esté deshabilitado " +
                                      "y que este programa se esté ejecutando como administrador.",
                    "Servicio detenido", MessageBoxButtons.OK, MessageBoxIcon.Warning);
            return ok;
        }

        private bool VerificarPuerto(string servidor)
        {
            if (!InstanciaSql.PuertoTcpRemoto(servidor, out string equipo, out int puerto)) return true;
            _log.Escribir($"Verificando el puerto TCP {puerto} de {equipo}...");
            if (InstanciaSql.PuertoAbierto(equipo, puerto, TimeSpan.FromSeconds(5))) return true;
            _log.Escribir($"El puerto TCP {puerto} de {equipo} no responde.");
            MessageBox.Show(this, $"No se pudo llegar al puerto TCP {puerto} de {equipo}.\n\n" +
                                  "Verificá que el servidor esté encendido, que SQL Server acepte conexiones TCP/IP " +
                                  "y que el firewall permita ese puerto.",
                "Puerto cerrado", MessageBoxButtons.OK, MessageBoxIcon.Warning);
            return false;
        }

        private bool VerificarEspacioEnDisco(string servidor)
        {
            if (!servidor.StartsWith("(localdb)", StringComparison.OrdinalIgnoreCase) && InstanciaSql.ServicioDe(servidor) == null)
                return true;
            var unidad = new DriveInfo(Path.GetPathRoot(Environment.SystemDirectory));
            if (unidad.AvailableFreeSpace >= EspacioMinimoBytes) return true;
            _log.Escribir($"Espacio libre insuficiente en {unidad.Name}: {unidad.AvailableFreeSpace / 1024 / 1024} MB.");
            return MessageBox.Show(this, $"Queda poco espacio libre en {unidad.Name} ({unidad.AvailableFreeSpace / 1024 / 1024} MB). " +
                                         "La creación de la base puede fallar. ¿Continuar igual?",
                "Poco espacio en disco", MessageBoxButtons.YesNo, MessageBoxIcon.Warning) == DialogResult.Yes;
        }

        private SqlConnectionStringBuilder ArmarCadena()
        {
            string servidor = cboServidor.Text.Trim();
            if (servidor.Length == 0)
            {
                MessageBox.Show(this, "Escribí o elegí un servidor.", "Falta el servidor", MessageBoxButtons.OK, MessageBoxIcon.Warning);
                return null;
            }
            var cadena = new SqlConnectionStringBuilder { DataSource = servidor, InitialCatalog = "master", ConnectTimeout = 15 };
            if (rbWindows.Checked)
            {
                cadena.IntegratedSecurity = true;
            }
            else
            {
                if (txtUsuario.Text.Trim().Length == 0)
                {
                    MessageBox.Show(this, "Escribí el usuario de SQL Server.", "Falta el usuario", MessageBoxButtons.OK, MessageBoxIcon.Warning);
                    return null;
                }
                cadena.UserID = txtUsuario.Text.Trim();
                cadena.Password = txtContrasena.Text;
            }
            return cadena;
        }

        private static string BuscarScript(string nombre)
        {
            var carpeta = new DirectoryInfo(AppDomain.CurrentDomain.BaseDirectory);
            string instalado = Path.Combine(carpeta.FullName, "Scripts", nombre);
            if (File.Exists(instalado)) return instalado;
            for (DirectoryInfo d = carpeta; d != null; d = d.Parent)
            {
                string desarrollo = Path.Combine(d.FullName, "DAL", nombre);
                if (File.Exists(desarrollo)) return desarrollo;
            }
            return null;
        }

        private async Task<bool> EjecutarAsync(Action trabajo)
        {
            HabilitarControles(false);
            try
            {
                await Task.Run(trabajo);
                return true;
            }
            catch (Exception ex)
            {
                MostrarError(ex);
                return false;
            }
            finally
            {
                HabilitarControles(true);
            }
        }

        private bool EjecutarSincronico(Action trabajo)
        {
            Cursor = Cursors.WaitCursor;
            try
            {
                trabajo();
                return true;
            }
            catch (Exception ex)
            {
                MostrarError(ex);
                return false;
            }
            finally
            {
                Cursor = Cursors.Default;
            }
        }

        private void MostrarError(Exception ex)
        {
            _log.Escribir("ERROR: " + ex);
            string detalle = ex is SqlException sql && (sql.Number == -1 || sql.Number == 2 || sql.Number == 53 || sql.Number == 26)
                ? "No se encontró el servidor o no responde. Verificá el nombre de la instancia y que el servicio esté iniciado."
                : ex.Message;
            MessageBox.Show(this, detalle + "\n\nEl detalle completo quedó en:\n" + _log.Ruta,
                "No se pudo completar la operación", MessageBoxButtons.OK, MessageBoxIcon.Error);
        }

        private void HabilitarControles(bool habilitar)
        {
            foreach (Control c in new Control[] { cboServidor, btnBuscar, btnProbar, btnInstalar, btnCerrar, btnInstalarMotor, rbWindows, rbSql })
                c.Enabled = habilitar;
            txtUsuario.Enabled = txtContrasena.Enabled = habilitar && rbSql.Checked;
            prgProgreso.Visible = !habilitar;
            Cursor = habilitar ? Cursors.Default : Cursors.WaitCursor;
        }

        private void AgregarAlRegistro(string linea)
        {
            if (txtRegistro.InvokeRequired)
            {
                txtRegistro.BeginInvoke(new Action<string>(AgregarAlRegistro), linea);
                return;
            }
            txtRegistro.AppendText(linea + Environment.NewLine);
        }

        private static void AbrirEnNavegador(string url)
        {
            Process.Start(new ProcessStartInfo(url) { UseShellExecute = true });
        }
    }
}
