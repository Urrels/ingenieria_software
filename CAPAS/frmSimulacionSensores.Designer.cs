namespace CAPAS
{
    partial class frmSimulacionSensores
    {
        private System.ComponentModel.IContainer components = new System.ComponentModel.Container();

        protected override void Dispose(bool disposing)
        {
            if (disposing && (components != null)) components.Dispose();
            base.Dispose(disposing);
        }

        private void InitializeComponent()
        {
            this.lblTitulo = new System.Windows.Forms.Label();
            this.btnIniciarApi = new System.Windows.Forms.Button();
            this.btnDetenerApi = new System.Windows.Forms.Button();
            this.lblIntervalo = new System.Windows.Forms.Label();
            this.numIntervalo = new System.Windows.Forms.NumericUpDown();
            this.btnIniciarSimulacion = new System.Windows.Forms.Button();
            this.btnDetenerSimulacion = new System.Windows.Forms.Button();
            this.lstLog = new System.Windows.Forms.ListBox();
            this.btnCerrar = new System.Windows.Forms.Button();
            this.timerSimulacion = new System.Windows.Forms.Timer(this.components);
            ((System.ComponentModel.ISupportInitialize)(this.numIntervalo)).BeginInit();
            this.SuspendLayout();
            //
            // lblTitulo
            //
            this.lblTitulo.AutoSize = true;
            this.lblTitulo.Font = new System.Drawing.Font("Segoe UI", 12F, System.Drawing.FontStyle.Bold);
            this.lblTitulo.Location = new System.Drawing.Point(30, 30);
            this.lblTitulo.Name = "lblTitulo";
            this.lblTitulo.Size = new System.Drawing.Size(260, 21);
            this.lblTitulo.TabIndex = 0;
            this.lblTitulo.Text = "Simulación de sensores IoT";
            //
            // btnIniciarApi
            //
            this.btnIniciarApi.Location = new System.Drawing.Point(30, 75);
            this.btnIniciarApi.Name = "btnIniciarApi";
            this.btnIniciarApi.Size = new System.Drawing.Size(150, 32);
            this.btnIniciarApi.TabIndex = 1;
            this.btnIniciarApi.Text = "Iniciar API";
            this.btnIniciarApi.UseVisualStyleBackColor = true;
            this.btnIniciarApi.Click += new System.EventHandler(this.btnIniciarApi_Click);
            //
            // btnDetenerApi
            //
            this.btnDetenerApi.Enabled = false;
            this.btnDetenerApi.Location = new System.Drawing.Point(190, 75);
            this.btnDetenerApi.Name = "btnDetenerApi";
            this.btnDetenerApi.Size = new System.Drawing.Size(150, 32);
            this.btnDetenerApi.TabIndex = 2;
            this.btnDetenerApi.Text = "Detener API";
            this.btnDetenerApi.UseVisualStyleBackColor = true;
            this.btnDetenerApi.Click += new System.EventHandler(this.btnDetenerApi_Click);
            //
            // lblIntervalo
            //
            this.lblIntervalo.AutoSize = true;
            this.lblIntervalo.Location = new System.Drawing.Point(30, 125);
            this.lblIntervalo.Name = "lblIntervalo";
            this.lblIntervalo.Size = new System.Drawing.Size(140, 15);
            this.lblIntervalo.TabIndex = 3;
            this.lblIntervalo.Text = "Intervalo entre eventos (s):";
            //
            // numIntervalo
            //
            this.numIntervalo.Location = new System.Drawing.Point(180, 121);
            this.numIntervalo.Minimum = new decimal(new int[] { 1, 0, 0, 0 });
            this.numIntervalo.Maximum = new decimal(new int[] { 60, 0, 0, 0 });
            this.numIntervalo.Value = new decimal(new int[] { 3, 0, 0, 0 });
            this.numIntervalo.Name = "numIntervalo";
            this.numIntervalo.Size = new System.Drawing.Size(60, 23);
            this.numIntervalo.TabIndex = 4;
            //
            // btnIniciarSimulacion
            //
            this.btnIniciarSimulacion.Enabled = false;
            this.btnIniciarSimulacion.Location = new System.Drawing.Point(30, 160);
            this.btnIniciarSimulacion.Name = "btnIniciarSimulacion";
            this.btnIniciarSimulacion.Size = new System.Drawing.Size(180, 32);
            this.btnIniciarSimulacion.TabIndex = 5;
            this.btnIniciarSimulacion.Text = "Iniciar simulación";
            this.btnIniciarSimulacion.UseVisualStyleBackColor = true;
            this.btnIniciarSimulacion.Click += new System.EventHandler(this.btnIniciarSimulacion_Click);
            //
            // btnDetenerSimulacion
            //
            this.btnDetenerSimulacion.Enabled = false;
            this.btnDetenerSimulacion.Location = new System.Drawing.Point(220, 160);
            this.btnDetenerSimulacion.Name = "btnDetenerSimulacion";
            this.btnDetenerSimulacion.Size = new System.Drawing.Size(180, 32);
            this.btnDetenerSimulacion.TabIndex = 6;
            this.btnDetenerSimulacion.Text = "Detener simulación";
            this.btnDetenerSimulacion.UseVisualStyleBackColor = true;
            this.btnDetenerSimulacion.Click += new System.EventHandler(this.btnDetenerSimulacion_Click);
            //
            // lstLog
            //
            this.lstLog.FormattingEnabled = true;
            this.lstLog.ItemHeight = 15;
            this.lstLog.Location = new System.Drawing.Point(30, 205);
            this.lstLog.Name = "lstLog";
            this.lstLog.Size = new System.Drawing.Size(600, 289);
            this.lstLog.TabIndex = 7;
            //
            // btnCerrar
            //
            this.btnCerrar.Location = new System.Drawing.Point(555, 505);
            this.btnCerrar.Name = "btnCerrar";
            this.btnCerrar.Size = new System.Drawing.Size(75, 32);
            this.btnCerrar.TabIndex = 8;
            this.btnCerrar.Text = "Cerrar";
            this.btnCerrar.UseVisualStyleBackColor = true;
            this.btnCerrar.Click += new System.EventHandler(this.btnCerrar_Click);
            //
            // timerSimulacion
            //
            this.timerSimulacion.Interval = 3000;
            this.timerSimulacion.Tick += new System.EventHandler(this.timerSimulacion_Tick);
            //
            // frmSimulacionSensores
            //
            this.AutoScaleDimensions = new System.Drawing.SizeF(7F, 15F);
            this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
            this.ClientSize = new System.Drawing.Size(660, 560);
            this.Controls.Add(this.btnCerrar);
            this.Controls.Add(this.lstLog);
            this.Controls.Add(this.btnDetenerSimulacion);
            this.Controls.Add(this.btnIniciarSimulacion);
            this.Controls.Add(this.numIntervalo);
            this.Controls.Add(this.lblIntervalo);
            this.Controls.Add(this.btnDetenerApi);
            this.Controls.Add(this.btnIniciarApi);
            this.Controls.Add(this.lblTitulo);
            this.MinimumSize = new System.Drawing.Size(676, 599);
            this.Name = "frmSimulacionSensores";
            this.StartPosition = System.Windows.Forms.FormStartPosition.CenterParent;
            this.Text = "frmSimulacionSensores";
            this.Load += new System.EventHandler(this.frmSimulacionSensores_Load);
            this.FormClosing += new System.Windows.Forms.FormClosingEventHandler(this.frmSimulacionSensores_FormClosing);
            ((System.ComponentModel.ISupportInitialize)(this.numIntervalo)).EndInit();
            this.ResumeLayout(false);
            this.PerformLayout();
        }

        private System.Windows.Forms.Label lblTitulo;
        private System.Windows.Forms.Button btnIniciarApi;
        private System.Windows.Forms.Button btnDetenerApi;
        private System.Windows.Forms.Label lblIntervalo;
        private System.Windows.Forms.NumericUpDown numIntervalo;
        private System.Windows.Forms.Button btnIniciarSimulacion;
        private System.Windows.Forms.Button btnDetenerSimulacion;
        private System.Windows.Forms.ListBox lstLog;
        private System.Windows.Forms.Button btnCerrar;
        private System.Windows.Forms.Timer timerSimulacion;
    }
}