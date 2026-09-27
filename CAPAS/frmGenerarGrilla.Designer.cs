namespace CAPAS
{
    partial class frmGenerarGrilla
    {
        private System.ComponentModel.IContainer components = null;

        protected override void Dispose(bool disposing)
        {
            if (disposing && (components != null)) components.Dispose();
            base.Dispose(disposing);
        }

        private void InitializeComponent()
        {
            this.lblTitulo = new System.Windows.Forms.Label();
            this.lblSemana = new System.Windows.Forms.Label();
            this.dtpSemana = new System.Windows.Forms.DateTimePicker();
            this.btnGenerar = new System.Windows.Forms.Button();
            this.dgvTurnos = new System.Windows.Forms.DataGridView();
            this.btnConfirmar = new System.Windows.Forms.Button();
            this.btnComunicar = new System.Windows.Forms.Button();
            this.btnExportar = new System.Windows.Forms.Button();
            this.btnCerrar = new System.Windows.Forms.Button();
            ((System.ComponentModel.ISupportInitialize)(this.dgvTurnos)).BeginInit();
            this.SuspendLayout();
            //
            // lblTitulo
            //
            this.lblTitulo.AutoSize = true;
            this.lblTitulo.Font = new System.Drawing.Font("Segoe UI", 12F, System.Drawing.FontStyle.Bold);
            this.lblTitulo.Location = new System.Drawing.Point(30, 30);
            this.lblTitulo.Name = "lblTitulo";
            this.lblTitulo.Size = new System.Drawing.Size(220, 21);
            this.lblTitulo.TabIndex = 0;
            this.lblTitulo.Text = "Generar grilla de turnos";
            //
            // lblSemana
            //
            this.lblSemana.AutoSize = true;
            this.lblSemana.Location = new System.Drawing.Point(30, 70);
            this.lblSemana.Name = "lblSemana";
            this.lblSemana.Size = new System.Drawing.Size(100, 15);
            this.lblSemana.TabIndex = 1;
            this.lblSemana.Text = "Semana (lunes):";
            //
            // dtpSemana
            //
            this.dtpSemana.Format = System.Windows.Forms.DateTimePickerFormat.Short;
            this.dtpSemana.Location = new System.Drawing.Point(150, 66);
            this.dtpSemana.Name = "dtpSemana";
            this.dtpSemana.Size = new System.Drawing.Size(150, 23);
            this.dtpSemana.TabIndex = 2;
            //
            // btnGenerar
            //
            this.btnGenerar.Location = new System.Drawing.Point(320, 64);
            this.btnGenerar.Name = "btnGenerar";
            this.btnGenerar.Size = new System.Drawing.Size(150, 30);
            this.btnGenerar.TabIndex = 3;
            this.btnGenerar.Text = "Generar grilla";
            this.btnGenerar.UseVisualStyleBackColor = true;
            this.btnGenerar.Click += new System.EventHandler(this.btnGenerar_Click);
            //
            // dgvTurnos
            //
            this.dgvTurnos.AllowUserToAddRows = false;
            this.dgvTurnos.AllowUserToDeleteRows = false;
            this.dgvTurnos.ReadOnly = true;
            this.dgvTurnos.SelectionMode = System.Windows.Forms.DataGridViewSelectionMode.FullRowSelect;
            this.dgvTurnos.AutoSizeColumnsMode = System.Windows.Forms.DataGridViewAutoSizeColumnsMode.Fill;
            this.dgvTurnos.Location = new System.Drawing.Point(30, 110);
            this.dgvTurnos.Name = "dgvTurnos";
            this.dgvTurnos.RowTemplate.Height = 25;
            this.dgvTurnos.Size = new System.Drawing.Size(740, 380);
            this.dgvTurnos.TabIndex = 4;
            this.dgvTurnos.CellDoubleClick += new System.Windows.Forms.DataGridViewCellEventHandler(this.dgvTurnos_CellDoubleClick);
            //
            // btnConfirmar
            //
            this.btnConfirmar.Location = new System.Drawing.Point(30, 500);
            this.btnConfirmar.Name = "btnConfirmar";
            this.btnConfirmar.Size = new System.Drawing.Size(150, 32);
            this.btnConfirmar.TabIndex = 5;
            this.btnConfirmar.Text = "Confirmar grilla";
            this.btnConfirmar.UseVisualStyleBackColor = true;
            this.btnConfirmar.Click += new System.EventHandler(this.btnConfirmar_Click);
            //
            // btnComunicar
            //
            this.btnComunicar.Location = new System.Drawing.Point(190, 500);
            this.btnComunicar.Name = "btnComunicar";
            this.btnComunicar.Size = new System.Drawing.Size(180, 32);
            this.btnComunicar.TabIndex = 6;
            this.btnComunicar.Text = "Comunicar horarios";
            this.btnComunicar.UseVisualStyleBackColor = true;
            this.btnComunicar.Click += new System.EventHandler(this.btnComunicar_Click);
            //
            // btnExportar
            //
            this.btnExportar.Location = new System.Drawing.Point(470, 500);
            this.btnExportar.Name = "btnExportar";
            this.btnExportar.Size = new System.Drawing.Size(150, 32);
            this.btnExportar.TabIndex = 7;
            this.btnExportar.Text = "Exportar a Excel";
            this.btnExportar.UseVisualStyleBackColor = true;
            this.btnExportar.Click += new System.EventHandler(this.btnExportar_Click);
            //
            // btnCerrar
            //
            this.btnCerrar.Location = new System.Drawing.Point(695, 500);
            this.btnCerrar.Name = "btnCerrar";
            this.btnCerrar.Size = new System.Drawing.Size(75, 32);
            this.btnCerrar.TabIndex = 8;
            this.btnCerrar.Text = "Cerrar";
            this.btnCerrar.UseVisualStyleBackColor = true;
            this.btnCerrar.Click += new System.EventHandler(this.btnCerrar_Click);
            //
            // frmGenerarGrilla
            //
            this.AutoScaleDimensions = new System.Drawing.SizeF(7F, 15F);
            this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
            this.ClientSize = new System.Drawing.Size(800, 560);
            this.Controls.Add(this.btnCerrar);
            this.Controls.Add(this.btnExportar);
            this.Controls.Add(this.btnComunicar);
            this.Controls.Add(this.btnConfirmar);
            this.Controls.Add(this.dgvTurnos);
            this.Controls.Add(this.btnGenerar);
            this.Controls.Add(this.dtpSemana);
            this.Controls.Add(this.lblSemana);
            this.Controls.Add(this.lblTitulo);
            this.MinimumSize = new System.Drawing.Size(816, 599);
            this.Name = "frmGenerarGrilla";
            this.StartPosition = System.Windows.Forms.FormStartPosition.CenterParent;
            this.Text = "frmGenerarGrilla";
            this.Load += new System.EventHandler(this.frmGenerarGrilla_Load);
            this.FormClosed += new System.Windows.Forms.FormClosedEventHandler(this.frmGenerarGrilla_FormClosed);
            ((System.ComponentModel.ISupportInitialize)(this.dgvTurnos)).EndInit();
            this.ResumeLayout(false);
            this.PerformLayout();
        }

        private System.Windows.Forms.Label lblTitulo;
        private System.Windows.Forms.Label lblSemana;
        private System.Windows.Forms.DateTimePicker dtpSemana;
        private System.Windows.Forms.Button btnGenerar;
        private System.Windows.Forms.DataGridView dgvTurnos;
        private System.Windows.Forms.Button btnConfirmar;
        private System.Windows.Forms.Button btnComunicar;
        private System.Windows.Forms.Button btnExportar;
        private System.Windows.Forms.Button btnCerrar;
    }
}