namespace CAPAS
{
    partial class frmLiquidarHoras
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
            this.lblDesde = new System.Windows.Forms.Label();
            this.dtpDesde = new System.Windows.Forms.DateTimePicker();
            this.lblHasta = new System.Windows.Forms.Label();
            this.dtpHasta = new System.Windows.Forms.DateTimePicker();
            this.btnCalcular = new System.Windows.Forms.Button();
            this.lblLiquidaciones = new System.Windows.Forms.Label();
            this.cboLiquidaciones = new System.Windows.Forms.ComboBox();
            this.lblEstado = new System.Windows.Forms.Label();
            this.dgvResumen = new System.Windows.Forms.DataGridView();
            this.dgvDetalle = new System.Windows.Forms.DataGridView();
            this.lblAdvertencias = new System.Windows.Forms.Label();
            this.btnAprobarExcedente = new System.Windows.Forms.Button();
            this.btnRevocarExcedente = new System.Windows.Forms.Button();
            this.btnCorregirFichajes = new System.Windows.Forms.Button();
            this.btnParametros = new System.Windows.Forms.Button();
            this.btnCerrarPeriodo = new System.Windows.Forms.Button();
            this.btnMarcarPagada = new System.Windows.Forms.Button();
            this.btnExportar = new System.Windows.Forms.Button();
            this.btnRecibo = new System.Windows.Forms.Button();
            this.btnCerrar = new System.Windows.Forms.Button();
            ((System.ComponentModel.ISupportInitialize)(this.dgvResumen)).BeginInit();
            ((System.ComponentModel.ISupportInitialize)(this.dgvDetalle)).BeginInit();
            this.SuspendLayout();
            //
            // lblTitulo
            //
            this.lblTitulo.AutoSize = true;
            this.lblTitulo.Font = new System.Drawing.Font("Segoe UI", 12F, System.Drawing.FontStyle.Bold);
            this.lblTitulo.Location = new System.Drawing.Point(30, 75);
            this.lblTitulo.Name = "lblTitulo";
            this.lblTitulo.Size = new System.Drawing.Size(280, 21);
            this.lblTitulo.TabIndex = 0;
            this.lblTitulo.Text = "Liquidar horas de empleados";
            //
            // lblDesde
            //
            this.lblDesde.AutoSize = true;
            this.lblDesde.Location = new System.Drawing.Point(30, 118);
            this.lblDesde.Name = "lblDesde";
            this.lblDesde.Size = new System.Drawing.Size(45, 15);
            this.lblDesde.TabIndex = 1;
            this.lblDesde.Text = "Desde:";
            //
            // dtpDesde
            //
            this.dtpDesde.Format = System.Windows.Forms.DateTimePickerFormat.Short;
            this.dtpDesde.Location = new System.Drawing.Point(85, 114);
            this.dtpDesde.Name = "dtpDesde";
            this.dtpDesde.Size = new System.Drawing.Size(130, 23);
            this.dtpDesde.TabIndex = 2;
            //
            // lblHasta
            //
            this.lblHasta.AutoSize = true;
            this.lblHasta.Location = new System.Drawing.Point(235, 118);
            this.lblHasta.Name = "lblHasta";
            this.lblHasta.Size = new System.Drawing.Size(42, 15);
            this.lblHasta.TabIndex = 3;
            this.lblHasta.Text = "Hasta:";
            //
            // dtpHasta
            //
            this.dtpHasta.Format = System.Windows.Forms.DateTimePickerFormat.Short;
            this.dtpHasta.Location = new System.Drawing.Point(285, 114);
            this.dtpHasta.Name = "dtpHasta";
            this.dtpHasta.Size = new System.Drawing.Size(130, 23);
            this.dtpHasta.TabIndex = 4;
            //
            // btnCalcular
            //
            this.btnCalcular.Location = new System.Drawing.Point(430, 111);
            this.btnCalcular.Name = "btnCalcular";
            this.btnCalcular.Size = new System.Drawing.Size(110, 30);
            this.btnCalcular.TabIndex = 5;
            this.btnCalcular.Text = "Calcular";
            this.btnCalcular.UseVisualStyleBackColor = true;
            this.btnCalcular.Click += new System.EventHandler(this.btnCalcular_Click);
            //
            // lblLiquidaciones
            //
            this.lblLiquidaciones.AutoSize = true;
            this.lblLiquidaciones.Location = new System.Drawing.Point(580, 118);
            this.lblLiquidaciones.Name = "lblLiquidaciones";
            this.lblLiquidaciones.Size = new System.Drawing.Size(140, 15);
            this.lblLiquidaciones.TabIndex = 6;
            this.lblLiquidaciones.Text = "Liquidaciones cerradas:";
            //
            // cboLiquidaciones
            //
            this.cboLiquidaciones.DropDownStyle = System.Windows.Forms.ComboBoxStyle.DropDownList;
            this.cboLiquidaciones.FormattingEnabled = true;
            this.cboLiquidaciones.Location = new System.Drawing.Point(740, 114);
            this.cboLiquidaciones.Name = "cboLiquidaciones";
            this.cboLiquidaciones.Size = new System.Drawing.Size(290, 23);
            this.cboLiquidaciones.TabIndex = 7;
            this.cboLiquidaciones.SelectionChangeCommitted += new System.EventHandler(this.cboLiquidaciones_SelectionChangeCommitted);
            //
            // lblEstado
            //
            this.lblEstado.AutoSize = false;
            this.lblEstado.Font = new System.Drawing.Font("Segoe UI", 9F, System.Drawing.FontStyle.Bold);
            this.lblEstado.Location = new System.Drawing.Point(30, 152);
            this.lblEstado.Name = "lblEstado";
            this.lblEstado.Size = new System.Drawing.Size(1000, 20);
            this.lblEstado.TabIndex = 8;
            //
            // dgvResumen
            //
            this.dgvResumen.AllowUserToAddRows = false;
            this.dgvResumen.AllowUserToDeleteRows = false;
            this.dgvResumen.ReadOnly = true;
            this.dgvResumen.MultiSelect = false;
            this.dgvResumen.SelectionMode = System.Windows.Forms.DataGridViewSelectionMode.FullRowSelect;
            this.dgvResumen.AutoSizeColumnsMode = System.Windows.Forms.DataGridViewAutoSizeColumnsMode.Fill;
            this.dgvResumen.Location = new System.Drawing.Point(30, 178);
            this.dgvResumen.Name = "dgvResumen";
            this.dgvResumen.RowTemplate.Height = 25;
            this.dgvResumen.Size = new System.Drawing.Size(1000, 170);
            this.dgvResumen.TabIndex = 9;
            this.dgvResumen.SelectionChanged += new System.EventHandler(this.dgvResumen_SelectionChanged);
            //
            // dgvDetalle
            //
            this.dgvDetalle.AllowUserToAddRows = false;
            this.dgvDetalle.AllowUserToDeleteRows = false;
            this.dgvDetalle.ReadOnly = true;
            this.dgvDetalle.MultiSelect = false;
            this.dgvDetalle.SelectionMode = System.Windows.Forms.DataGridViewSelectionMode.FullRowSelect;
            this.dgvDetalle.AutoSizeColumnsMode = System.Windows.Forms.DataGridViewAutoSizeColumnsMode.Fill;
            this.dgvDetalle.Location = new System.Drawing.Point(30, 358);
            this.dgvDetalle.Name = "dgvDetalle";
            this.dgvDetalle.RowTemplate.Height = 25;
            this.dgvDetalle.Size = new System.Drawing.Size(1000, 230);
            this.dgvDetalle.TabIndex = 10;
            this.dgvDetalle.SelectionChanged += new System.EventHandler(this.dgvDetalle_SelectionChanged);
            //
            // lblAdvertencias
            //
            this.lblAdvertencias.AutoSize = false;
            this.lblAdvertencias.Location = new System.Drawing.Point(30, 594);
            this.lblAdvertencias.Name = "lblAdvertencias";
            this.lblAdvertencias.Size = new System.Drawing.Size(1000, 34);
            this.lblAdvertencias.TabIndex = 11;
            //
            // btnAprobarExcedente
            //
            this.btnAprobarExcedente.Location = new System.Drawing.Point(30, 634);
            this.btnAprobarExcedente.Name = "btnAprobarExcedente";
            this.btnAprobarExcedente.Size = new System.Drawing.Size(170, 30);
            this.btnAprobarExcedente.TabIndex = 12;
            this.btnAprobarExcedente.Text = "Aprobar excedente";
            this.btnAprobarExcedente.UseVisualStyleBackColor = true;
            this.btnAprobarExcedente.Click += new System.EventHandler(this.btnAprobarExcedente_Click);
            //
            // btnRevocarExcedente
            //
            this.btnRevocarExcedente.Location = new System.Drawing.Point(210, 634);
            this.btnRevocarExcedente.Name = "btnRevocarExcedente";
            this.btnRevocarExcedente.Size = new System.Drawing.Size(170, 30);
            this.btnRevocarExcedente.TabIndex = 13;
            this.btnRevocarExcedente.Text = "Revocar aprobación";
            this.btnRevocarExcedente.UseVisualStyleBackColor = true;
            this.btnRevocarExcedente.Click += new System.EventHandler(this.btnRevocarExcedente_Click);
            //
            // btnCorregirFichajes
            //
            this.btnCorregirFichajes.Location = new System.Drawing.Point(390, 634);
            this.btnCorregirFichajes.Name = "btnCorregirFichajes";
            this.btnCorregirFichajes.Size = new System.Drawing.Size(170, 30);
            this.btnCorregirFichajes.TabIndex = 14;
            this.btnCorregirFichajes.Text = "Corregir fichajes";
            this.btnCorregirFichajes.UseVisualStyleBackColor = true;
            this.btnCorregirFichajes.Click += new System.EventHandler(this.btnCorregirFichajes_Click);
            //
            // btnParametros
            //
            this.btnParametros.Location = new System.Drawing.Point(570, 634);
            this.btnParametros.Name = "btnParametros";
            this.btnParametros.Size = new System.Drawing.Size(170, 30);
            this.btnParametros.TabIndex = 15;
            this.btnParametros.Text = "Parámetros";
            this.btnParametros.UseVisualStyleBackColor = true;
            this.btnParametros.Click += new System.EventHandler(this.btnParametros_Click);
            //
            // btnCerrarPeriodo
            //
            this.btnCerrarPeriodo.Location = new System.Drawing.Point(30, 674);
            this.btnCerrarPeriodo.Name = "btnCerrarPeriodo";
            this.btnCerrarPeriodo.Size = new System.Drawing.Size(170, 30);
            this.btnCerrarPeriodo.TabIndex = 16;
            this.btnCerrarPeriodo.Text = "Cerrar período";
            this.btnCerrarPeriodo.UseVisualStyleBackColor = true;
            this.btnCerrarPeriodo.Click += new System.EventHandler(this.btnCerrarPeriodo_Click);
            //
            // btnMarcarPagada
            //
            this.btnMarcarPagada.Location = new System.Drawing.Point(210, 674);
            this.btnMarcarPagada.Name = "btnMarcarPagada";
            this.btnMarcarPagada.Size = new System.Drawing.Size(170, 30);
            this.btnMarcarPagada.TabIndex = 17;
            this.btnMarcarPagada.Text = "Marcar como pagada";
            this.btnMarcarPagada.UseVisualStyleBackColor = true;
            this.btnMarcarPagada.Click += new System.EventHandler(this.btnMarcarPagada_Click);
            //
            // btnExportar
            //
            this.btnExportar.Location = new System.Drawing.Point(390, 674);
            this.btnExportar.Name = "btnExportar";
            this.btnExportar.Size = new System.Drawing.Size(170, 30);
            this.btnExportar.TabIndex = 18;
            this.btnExportar.Text = "Exportar a Excel";
            this.btnExportar.UseVisualStyleBackColor = true;
            this.btnExportar.Click += new System.EventHandler(this.btnExportar_Click);
            //
            // btnRecibo
            //
            this.btnRecibo.Location = new System.Drawing.Point(570, 674);
            this.btnRecibo.Name = "btnRecibo";
            this.btnRecibo.Size = new System.Drawing.Size(170, 30);
            this.btnRecibo.TabIndex = 19;
            this.btnRecibo.Text = "Imprimir recibo";
            this.btnRecibo.UseVisualStyleBackColor = true;
            this.btnRecibo.Click += new System.EventHandler(this.btnRecibo_Click);
            //
            // btnCerrar
            //
            this.btnCerrar.Location = new System.Drawing.Point(955, 674);
            this.btnCerrar.Name = "btnCerrar";
            this.btnCerrar.Size = new System.Drawing.Size(75, 30);
            this.btnCerrar.TabIndex = 20;
            this.btnCerrar.Text = "Cerrar";
            this.btnCerrar.UseVisualStyleBackColor = true;
            this.btnCerrar.Click += new System.EventHandler(this.btnCerrar_Click);
            //
            // frmLiquidarHoras
            //
            this.AutoScaleDimensions = new System.Drawing.SizeF(7F, 15F);
            this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
            this.ClientSize = new System.Drawing.Size(1060, 725);
            this.Controls.Add(this.btnCerrar);
            this.Controls.Add(this.btnRecibo);
            this.Controls.Add(this.btnExportar);
            this.Controls.Add(this.btnMarcarPagada);
            this.Controls.Add(this.btnCerrarPeriodo);
            this.Controls.Add(this.btnParametros);
            this.Controls.Add(this.btnCorregirFichajes);
            this.Controls.Add(this.btnRevocarExcedente);
            this.Controls.Add(this.btnAprobarExcedente);
            this.Controls.Add(this.lblAdvertencias);
            this.Controls.Add(this.dgvDetalle);
            this.Controls.Add(this.dgvResumen);
            this.Controls.Add(this.lblEstado);
            this.Controls.Add(this.cboLiquidaciones);
            this.Controls.Add(this.lblLiquidaciones);
            this.Controls.Add(this.btnCalcular);
            this.Controls.Add(this.dtpHasta);
            this.Controls.Add(this.lblHasta);
            this.Controls.Add(this.dtpDesde);
            this.Controls.Add(this.lblDesde);
            this.Controls.Add(this.lblTitulo);
            this.MinimumSize = new System.Drawing.Size(1076, 764);
            this.Name = "frmLiquidarHoras";
            this.StartPosition = System.Windows.Forms.FormStartPosition.CenterParent;
            this.Text = "Liquidar horas";
            this.Load += new System.EventHandler(this.frmLiquidarHoras_Load);
            ((System.ComponentModel.ISupportInitialize)(this.dgvResumen)).EndInit();
            ((System.ComponentModel.ISupportInitialize)(this.dgvDetalle)).EndInit();
            this.ResumeLayout(false);
            this.PerformLayout();
        }

        private System.Windows.Forms.Label lblTitulo;
        private System.Windows.Forms.Label lblDesde;
        private System.Windows.Forms.DateTimePicker dtpDesde;
        private System.Windows.Forms.Label lblHasta;
        private System.Windows.Forms.DateTimePicker dtpHasta;
        private System.Windows.Forms.Button btnCalcular;
        private System.Windows.Forms.Label lblLiquidaciones;
        private System.Windows.Forms.ComboBox cboLiquidaciones;
        private System.Windows.Forms.Label lblEstado;
        private System.Windows.Forms.DataGridView dgvResumen;
        private System.Windows.Forms.DataGridView dgvDetalle;
        private System.Windows.Forms.Label lblAdvertencias;
        private System.Windows.Forms.Button btnAprobarExcedente;
        private System.Windows.Forms.Button btnRevocarExcedente;
        private System.Windows.Forms.Button btnCorregirFichajes;
        private System.Windows.Forms.Button btnParametros;
        private System.Windows.Forms.Button btnCerrarPeriodo;
        private System.Windows.Forms.Button btnMarcarPagada;
        private System.Windows.Forms.Button btnExportar;
        private System.Windows.Forms.Button btnRecibo;
        private System.Windows.Forms.Button btnCerrar;
    }
}
