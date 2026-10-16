namespace CAPAS
{
    partial class frmCorregirFichajes
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
            this.chkSoloIncompletos = new System.Windows.Forms.CheckBox();
            this.btnBuscar = new System.Windows.Forms.Button();
            this.dgvFichajes = new System.Windows.Forms.DataGridView();
            this.lblIngreso = new System.Windows.Forms.Label();
            this.dtpIngreso = new System.Windows.Forms.DateTimePicker();
            this.lblEgreso = new System.Windows.Forms.Label();
            this.dtpEgreso = new System.Windows.Forms.DateTimePicker();
            this.lblMotivo = new System.Windows.Forms.Label();
            this.txtMotivo = new System.Windows.Forms.TextBox();
            this.btnCorregir = new System.Windows.Forms.Button();
            this.lblManual = new System.Windows.Forms.Label();
            this.cboEmpleado = new System.Windows.Forms.ComboBox();
            this.dtpFecha = new System.Windows.Forms.DateTimePicker();
            this.btnAgregarManual = new System.Windows.Forms.Button();
            this.btnCerrar = new System.Windows.Forms.Button();
            ((System.ComponentModel.ISupportInitialize)(this.dgvFichajes)).BeginInit();
            this.SuspendLayout();
            //
            // lblTitulo
            //
            this.lblTitulo.AutoSize = true;
            this.lblTitulo.Font = new System.Drawing.Font("Segoe UI", 12F, System.Drawing.FontStyle.Bold);
            this.lblTitulo.Location = new System.Drawing.Point(30, 75);
            this.lblTitulo.Name = "lblTitulo";
            this.lblTitulo.Size = new System.Drawing.Size(200, 21);
            this.lblTitulo.TabIndex = 0;
            this.lblTitulo.Text = "Corregir fichajes";
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
            // chkSoloIncompletos
            //
            this.chkSoloIncompletos.AutoSize = true;
            this.chkSoloIncompletos.Location = new System.Drawing.Point(435, 116);
            this.chkSoloIncompletos.Name = "chkSoloIncompletos";
            this.chkSoloIncompletos.Size = new System.Drawing.Size(200, 19);
            this.chkSoloIncompletos.TabIndex = 5;
            this.chkSoloIncompletos.Text = "Solo incompletos (sin egreso)";
            this.chkSoloIncompletos.UseVisualStyleBackColor = true;
            //
            // btnBuscar
            //
            this.btnBuscar.Location = new System.Drawing.Point(710, 111);
            this.btnBuscar.Name = "btnBuscar";
            this.btnBuscar.Size = new System.Drawing.Size(140, 30);
            this.btnBuscar.TabIndex = 6;
            this.btnBuscar.Text = "Buscar";
            this.btnBuscar.UseVisualStyleBackColor = true;
            this.btnBuscar.Click += new System.EventHandler(this.btnBuscar_Click);
            //
            // dgvFichajes
            //
            this.dgvFichajes.AllowUserToAddRows = false;
            this.dgvFichajes.AllowUserToDeleteRows = false;
            this.dgvFichajes.ReadOnly = true;
            this.dgvFichajes.MultiSelect = false;
            this.dgvFichajes.SelectionMode = System.Windows.Forms.DataGridViewSelectionMode.FullRowSelect;
            this.dgvFichajes.AutoSizeColumnsMode = System.Windows.Forms.DataGridViewAutoSizeColumnsMode.Fill;
            this.dgvFichajes.Location = new System.Drawing.Point(30, 155);
            this.dgvFichajes.Name = "dgvFichajes";
            this.dgvFichajes.RowTemplate.Height = 25;
            this.dgvFichajes.Size = new System.Drawing.Size(820, 260);
            this.dgvFichajes.TabIndex = 7;
            this.dgvFichajes.SelectionChanged += new System.EventHandler(this.dgvFichajes_SelectionChanged);
            //
            // lblIngreso
            //
            this.lblIngreso.AutoSize = true;
            this.lblIngreso.Location = new System.Drawing.Point(30, 438);
            this.lblIngreso.Name = "lblIngreso";
            this.lblIngreso.Size = new System.Drawing.Size(51, 15);
            this.lblIngreso.TabIndex = 8;
            this.lblIngreso.Text = "Ingreso:";
            //
            // dtpIngreso
            //
            this.dtpIngreso.Format = System.Windows.Forms.DateTimePickerFormat.Time;
            this.dtpIngreso.ShowUpDown = true;
            this.dtpIngreso.Location = new System.Drawing.Point(110, 434);
            this.dtpIngreso.Name = "dtpIngreso";
            this.dtpIngreso.Size = new System.Drawing.Size(100, 23);
            this.dtpIngreso.TabIndex = 9;
            //
            // lblEgreso
            //
            this.lblEgreso.AutoSize = true;
            this.lblEgreso.Location = new System.Drawing.Point(235, 438);
            this.lblEgreso.Name = "lblEgreso";
            this.lblEgreso.Size = new System.Drawing.Size(48, 15);
            this.lblEgreso.TabIndex = 10;
            this.lblEgreso.Text = "Egreso:";
            //
            // dtpEgreso
            //
            this.dtpEgreso.Format = System.Windows.Forms.DateTimePickerFormat.Time;
            this.dtpEgreso.ShowUpDown = true;
            this.dtpEgreso.Location = new System.Drawing.Point(300, 434);
            this.dtpEgreso.Name = "dtpEgreso";
            this.dtpEgreso.Size = new System.Drawing.Size(100, 23);
            this.dtpEgreso.TabIndex = 11;
            //
            // lblMotivo
            //
            this.lblMotivo.AutoSize = true;
            this.lblMotivo.Location = new System.Drawing.Point(30, 478);
            this.lblMotivo.Name = "lblMotivo";
            this.lblMotivo.Size = new System.Drawing.Size(48, 15);
            this.lblMotivo.TabIndex = 12;
            this.lblMotivo.Text = "Motivo:";
            //
            // txtMotivo
            //
            this.txtMotivo.Location = new System.Drawing.Point(110, 474);
            this.txtMotivo.MaxLength = 150;
            this.txtMotivo.Name = "txtMotivo";
            this.txtMotivo.Size = new System.Drawing.Size(740, 23);
            this.txtMotivo.TabIndex = 13;
            //
            // btnCorregir
            //
            this.btnCorregir.Location = new System.Drawing.Point(110, 512);
            this.btnCorregir.Name = "btnCorregir";
            this.btnCorregir.Size = new System.Drawing.Size(200, 30);
            this.btnCorregir.TabIndex = 14;
            this.btnCorregir.Text = "Corregir seleccionado";
            this.btnCorregir.UseVisualStyleBackColor = true;
            this.btnCorregir.Click += new System.EventHandler(this.btnCorregir_Click);
            //
            // lblManual
            //
            this.lblManual.AutoSize = true;
            this.lblManual.Location = new System.Drawing.Point(30, 566);
            this.lblManual.Name = "lblManual";
            this.lblManual.Size = new System.Drawing.Size(150, 15);
            this.lblManual.TabIndex = 15;
            this.lblManual.Text = "Fichaje que faltaba cargar:";
            //
            // cboEmpleado
            //
            this.cboEmpleado.DropDownStyle = System.Windows.Forms.ComboBoxStyle.DropDownList;
            this.cboEmpleado.FormattingEnabled = true;
            this.cboEmpleado.Location = new System.Drawing.Point(220, 562);
            this.cboEmpleado.Name = "cboEmpleado";
            this.cboEmpleado.Size = new System.Drawing.Size(220, 23);
            this.cboEmpleado.TabIndex = 16;
            //
            // dtpFecha
            //
            this.dtpFecha.Format = System.Windows.Forms.DateTimePickerFormat.Short;
            this.dtpFecha.Location = new System.Drawing.Point(455, 562);
            this.dtpFecha.Name = "dtpFecha";
            this.dtpFecha.Size = new System.Drawing.Size(120, 23);
            this.dtpFecha.TabIndex = 17;
            //
            // btnAgregarManual
            //
            this.btnAgregarManual.Location = new System.Drawing.Point(590, 559);
            this.btnAgregarManual.Name = "btnAgregarManual";
            this.btnAgregarManual.Size = new System.Drawing.Size(260, 30);
            this.btnAgregarManual.TabIndex = 18;
            this.btnAgregarManual.Text = "Agregar fichaje manual";
            this.btnAgregarManual.UseVisualStyleBackColor = true;
            this.btnAgregarManual.Click += new System.EventHandler(this.btnAgregarManual_Click);
            //
            // btnCerrar
            //
            this.btnCerrar.Location = new System.Drawing.Point(775, 612);
            this.btnCerrar.Name = "btnCerrar";
            this.btnCerrar.Size = new System.Drawing.Size(75, 30);
            this.btnCerrar.TabIndex = 19;
            this.btnCerrar.Text = "Cerrar";
            this.btnCerrar.UseVisualStyleBackColor = true;
            this.btnCerrar.Click += new System.EventHandler(this.btnCerrar_Click);
            //
            // frmCorregirFichajes
            //
            this.AutoScaleDimensions = new System.Drawing.SizeF(7F, 15F);
            this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
            this.ClientSize = new System.Drawing.Size(880, 665);
            this.Controls.Add(this.btnCerrar);
            this.Controls.Add(this.btnAgregarManual);
            this.Controls.Add(this.dtpFecha);
            this.Controls.Add(this.cboEmpleado);
            this.Controls.Add(this.lblManual);
            this.Controls.Add(this.btnCorregir);
            this.Controls.Add(this.txtMotivo);
            this.Controls.Add(this.lblMotivo);
            this.Controls.Add(this.dtpEgreso);
            this.Controls.Add(this.lblEgreso);
            this.Controls.Add(this.dtpIngreso);
            this.Controls.Add(this.lblIngreso);
            this.Controls.Add(this.dgvFichajes);
            this.Controls.Add(this.btnBuscar);
            this.Controls.Add(this.chkSoloIncompletos);
            this.Controls.Add(this.dtpHasta);
            this.Controls.Add(this.lblHasta);
            this.Controls.Add(this.dtpDesde);
            this.Controls.Add(this.lblDesde);
            this.Controls.Add(this.lblTitulo);
            this.MinimumSize = new System.Drawing.Size(896, 704);
            this.Name = "frmCorregirFichajes";
            this.StartPosition = System.Windows.Forms.FormStartPosition.CenterParent;
            this.Text = "Corregir fichajes";
            this.Load += new System.EventHandler(this.frmCorregirFichajes_Load);
            ((System.ComponentModel.ISupportInitialize)(this.dgvFichajes)).EndInit();
            this.ResumeLayout(false);
            this.PerformLayout();
        }

        private System.Windows.Forms.Label lblTitulo;
        private System.Windows.Forms.Label lblDesde;
        private System.Windows.Forms.DateTimePicker dtpDesde;
        private System.Windows.Forms.Label lblHasta;
        private System.Windows.Forms.DateTimePicker dtpHasta;
        private System.Windows.Forms.CheckBox chkSoloIncompletos;
        private System.Windows.Forms.Button btnBuscar;
        private System.Windows.Forms.DataGridView dgvFichajes;
        private System.Windows.Forms.Label lblIngreso;
        private System.Windows.Forms.DateTimePicker dtpIngreso;
        private System.Windows.Forms.Label lblEgreso;
        private System.Windows.Forms.DateTimePicker dtpEgreso;
        private System.Windows.Forms.Label lblMotivo;
        private System.Windows.Forms.TextBox txtMotivo;
        private System.Windows.Forms.Button btnCorregir;
        private System.Windows.Forms.Label lblManual;
        private System.Windows.Forms.ComboBox cboEmpleado;
        private System.Windows.Forms.DateTimePicker dtpFecha;
        private System.Windows.Forms.Button btnAgregarManual;
        private System.Windows.Forms.Button btnCerrar;
    }
}
