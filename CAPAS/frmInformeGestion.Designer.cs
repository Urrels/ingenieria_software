namespace CAPAS
{
    partial class frmInformeGestion
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
            this.btnGenerar = new System.Windows.Forms.Button();
            this.lblResumen = new System.Windows.Forms.Label();
            this.tabs = new System.Windows.Forms.TabControl();
            this.tabConclusiones = new System.Windows.Forms.TabPage();
            this.tabEmpleados = new System.Windows.Forms.TabPage();
            this.tabCobertura = new System.Windows.Forms.TabPage();
            this.tabEquipos = new System.Windows.Forms.TabPage();
            this.dgvConclusiones = new System.Windows.Forms.DataGridView();
            this.dgvEmpleados = new System.Windows.Forms.DataGridView();
            this.dgvCobertura = new System.Windows.Forms.DataGridView();
            this.dgvEquipos = new System.Windows.Forms.DataGridView();
            this.btnPdf = new System.Windows.Forms.Button();
            this.btnJson = new System.Windows.Forms.Button();
            this.btnCerrar = new System.Windows.Forms.Button();
            ((System.ComponentModel.ISupportInitialize)(this.dgvConclusiones)).BeginInit();
            ((System.ComponentModel.ISupportInitialize)(this.dgvEmpleados)).BeginInit();
            ((System.ComponentModel.ISupportInitialize)(this.dgvCobertura)).BeginInit();
            ((System.ComponentModel.ISupportInitialize)(this.dgvEquipos)).BeginInit();
            this.tabs.SuspendLayout();
            this.tabConclusiones.SuspendLayout();
            this.tabEmpleados.SuspendLayout();
            this.tabCobertura.SuspendLayout();
            this.tabEquipos.SuspendLayout();
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
            this.lblTitulo.Text = "Informe de gestión";
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
            // btnGenerar
            //
            this.btnGenerar.Location = new System.Drawing.Point(430, 111);
            this.btnGenerar.Name = "btnGenerar";
            this.btnGenerar.Size = new System.Drawing.Size(140, 30);
            this.btnGenerar.TabIndex = 5;
            this.btnGenerar.Text = "Generar informe";
            this.btnGenerar.UseVisualStyleBackColor = true;
            this.btnGenerar.Click += new System.EventHandler(this.btnGenerar_Click);
            //
            // lblResumen
            //
            this.lblResumen.AutoSize = false;
            this.lblResumen.Font = new System.Drawing.Font("Segoe UI", 9F, System.Drawing.FontStyle.Bold);
            this.lblResumen.Location = new System.Drawing.Point(30, 152);
            this.lblResumen.Name = "lblResumen";
            this.lblResumen.Size = new System.Drawing.Size(920, 40);
            this.lblResumen.TabIndex = 6;
            //
            // tabs
            //
            this.tabs.Controls.Add(this.tabConclusiones);
            this.tabs.Controls.Add(this.tabEmpleados);
            this.tabs.Controls.Add(this.tabCobertura);
            this.tabs.Controls.Add(this.tabEquipos);
            this.tabs.Location = new System.Drawing.Point(30, 198);
            this.tabs.Name = "tabs";
            this.tabs.SelectedIndex = 0;
            this.tabs.Size = new System.Drawing.Size(920, 380);
            this.tabs.TabIndex = 7;
            //
            // tabConclusiones
            //
            this.tabConclusiones.Controls.Add(this.dgvConclusiones);
            this.tabConclusiones.Location = new System.Drawing.Point(4, 24);
            this.tabConclusiones.Name = "tabConclusiones";
            this.tabConclusiones.Size = new System.Drawing.Size(912, 352);
            this.tabConclusiones.TabIndex = 0;
            this.tabConclusiones.Text = "Conclusiones";
            this.tabConclusiones.UseVisualStyleBackColor = true;
            //
            // tabEmpleados
            //
            this.tabEmpleados.Controls.Add(this.dgvEmpleados);
            this.tabEmpleados.Location = new System.Drawing.Point(4, 24);
            this.tabEmpleados.Name = "tabEmpleados";
            this.tabEmpleados.Size = new System.Drawing.Size(912, 352);
            this.tabEmpleados.TabIndex = 1;
            this.tabEmpleados.Text = "Horas y costo";
            this.tabEmpleados.UseVisualStyleBackColor = true;
            //
            // tabCobertura
            //
            this.tabCobertura.Controls.Add(this.dgvCobertura);
            this.tabCobertura.Location = new System.Drawing.Point(4, 24);
            this.tabCobertura.Name = "tabCobertura";
            this.tabCobertura.Size = new System.Drawing.Size(912, 352);
            this.tabCobertura.TabIndex = 2;
            this.tabCobertura.Text = "Cobertura";
            this.tabCobertura.UseVisualStyleBackColor = true;
            //
            // tabEquipos
            //
            this.tabEquipos.Controls.Add(this.dgvEquipos);
            this.tabEquipos.Location = new System.Drawing.Point(4, 24);
            this.tabEquipos.Name = "tabEquipos";
            this.tabEquipos.Size = new System.Drawing.Size(912, 352);
            this.tabEquipos.TabIndex = 3;
            this.tabEquipos.Text = "Mantenimiento";
            this.tabEquipos.UseVisualStyleBackColor = true;
            //
            // dgvConclusiones
            //
            this.dgvConclusiones.AllowUserToAddRows = false;
            this.dgvConclusiones.AllowUserToDeleteRows = false;
            this.dgvConclusiones.ReadOnly = true;
            this.dgvConclusiones.SelectionMode = System.Windows.Forms.DataGridViewSelectionMode.FullRowSelect;
            this.dgvConclusiones.AutoSizeColumnsMode = System.Windows.Forms.DataGridViewAutoSizeColumnsMode.Fill;
            this.dgvConclusiones.Dock = System.Windows.Forms.DockStyle.Fill;
            this.dgvConclusiones.Name = "dgvConclusiones";
            this.dgvConclusiones.RowTemplate.Height = 25;
            this.dgvConclusiones.TabIndex = 0;
            //
            // dgvEmpleados
            //
            this.dgvEmpleados.AllowUserToAddRows = false;
            this.dgvEmpleados.AllowUserToDeleteRows = false;
            this.dgvEmpleados.ReadOnly = true;
            this.dgvEmpleados.SelectionMode = System.Windows.Forms.DataGridViewSelectionMode.FullRowSelect;
            this.dgvEmpleados.AutoSizeColumnsMode = System.Windows.Forms.DataGridViewAutoSizeColumnsMode.Fill;
            this.dgvEmpleados.Dock = System.Windows.Forms.DockStyle.Fill;
            this.dgvEmpleados.Name = "dgvEmpleados";
            this.dgvEmpleados.RowTemplate.Height = 25;
            this.dgvEmpleados.TabIndex = 0;
            //
            // dgvCobertura
            //
            this.dgvCobertura.AllowUserToAddRows = false;
            this.dgvCobertura.AllowUserToDeleteRows = false;
            this.dgvCobertura.ReadOnly = true;
            this.dgvCobertura.SelectionMode = System.Windows.Forms.DataGridViewSelectionMode.FullRowSelect;
            this.dgvCobertura.AutoSizeColumnsMode = System.Windows.Forms.DataGridViewAutoSizeColumnsMode.Fill;
            this.dgvCobertura.Dock = System.Windows.Forms.DockStyle.Fill;
            this.dgvCobertura.Name = "dgvCobertura";
            this.dgvCobertura.RowTemplate.Height = 25;
            this.dgvCobertura.TabIndex = 0;
            //
            // dgvEquipos
            //
            this.dgvEquipos.AllowUserToAddRows = false;
            this.dgvEquipos.AllowUserToDeleteRows = false;
            this.dgvEquipos.ReadOnly = true;
            this.dgvEquipos.SelectionMode = System.Windows.Forms.DataGridViewSelectionMode.FullRowSelect;
            this.dgvEquipos.AutoSizeColumnsMode = System.Windows.Forms.DataGridViewAutoSizeColumnsMode.Fill;
            this.dgvEquipos.Dock = System.Windows.Forms.DockStyle.Fill;
            this.dgvEquipos.Name = "dgvEquipos";
            this.dgvEquipos.RowTemplate.Height = 25;
            this.dgvEquipos.TabIndex = 0;
            //
            // btnPdf
            //
            this.btnPdf.Location = new System.Drawing.Point(30, 596);
            this.btnPdf.Name = "btnPdf";
            this.btnPdf.Size = new System.Drawing.Size(170, 32);
            this.btnPdf.TabIndex = 8;
            this.btnPdf.Text = "Exportar a PDF";
            this.btnPdf.UseVisualStyleBackColor = true;
            this.btnPdf.Click += new System.EventHandler(this.btnPdf_Click);
            //
            // btnJson
            //
            this.btnJson.Location = new System.Drawing.Point(210, 596);
            this.btnJson.Name = "btnJson";
            this.btnJson.Size = new System.Drawing.Size(200, 32);
            this.btnJson.TabIndex = 9;
            this.btnJson.Text = "Exportar datos (JSON)";
            this.btnJson.UseVisualStyleBackColor = true;
            this.btnJson.Click += new System.EventHandler(this.btnJson_Click);
            //
            // btnCerrar
            //
            this.btnCerrar.Location = new System.Drawing.Point(875, 596);
            this.btnCerrar.Name = "btnCerrar";
            this.btnCerrar.Size = new System.Drawing.Size(75, 32);
            this.btnCerrar.TabIndex = 10;
            this.btnCerrar.Text = "Cerrar";
            this.btnCerrar.UseVisualStyleBackColor = true;
            this.btnCerrar.Click += new System.EventHandler(this.btnCerrar_Click);
            //
            // frmInformeGestion
            //
            this.AutoScaleDimensions = new System.Drawing.SizeF(7F, 15F);
            this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
            this.ClientSize = new System.Drawing.Size(980, 650);
            this.Controls.Add(this.btnCerrar);
            this.Controls.Add(this.btnJson);
            this.Controls.Add(this.btnPdf);
            this.Controls.Add(this.tabs);
            this.Controls.Add(this.lblResumen);
            this.Controls.Add(this.btnGenerar);
            this.Controls.Add(this.dtpHasta);
            this.Controls.Add(this.lblHasta);
            this.Controls.Add(this.dtpDesde);
            this.Controls.Add(this.lblDesde);
            this.Controls.Add(this.lblTitulo);
            this.MinimumSize = new System.Drawing.Size(996, 689);
            this.Name = "frmInformeGestion";
            this.StartPosition = System.Windows.Forms.FormStartPosition.CenterParent;
            this.Text = "Informe de gestión";
            this.Load += new System.EventHandler(this.frmInformeGestion_Load);
            this.tabs.ResumeLayout(false);
            this.tabConclusiones.ResumeLayout(false);
            this.tabEmpleados.ResumeLayout(false);
            this.tabCobertura.ResumeLayout(false);
            this.tabEquipos.ResumeLayout(false);
            ((System.ComponentModel.ISupportInitialize)(this.dgvConclusiones)).EndInit();
            ((System.ComponentModel.ISupportInitialize)(this.dgvEmpleados)).EndInit();
            ((System.ComponentModel.ISupportInitialize)(this.dgvCobertura)).EndInit();
            ((System.ComponentModel.ISupportInitialize)(this.dgvEquipos)).EndInit();
            this.ResumeLayout(false);
            this.PerformLayout();
        }

        private System.Windows.Forms.Label lblTitulo;
        private System.Windows.Forms.Label lblDesde;
        private System.Windows.Forms.DateTimePicker dtpDesde;
        private System.Windows.Forms.Label lblHasta;
        private System.Windows.Forms.DateTimePicker dtpHasta;
        private System.Windows.Forms.Button btnGenerar;
        private System.Windows.Forms.Label lblResumen;
        private System.Windows.Forms.TabControl tabs;
        private System.Windows.Forms.TabPage tabConclusiones;
        private System.Windows.Forms.TabPage tabEmpleados;
        private System.Windows.Forms.TabPage tabCobertura;
        private System.Windows.Forms.TabPage tabEquipos;
        private System.Windows.Forms.DataGridView dgvConclusiones;
        private System.Windows.Forms.DataGridView dgvEmpleados;
        private System.Windows.Forms.DataGridView dgvCobertura;
        private System.Windows.Forms.DataGridView dgvEquipos;
        private System.Windows.Forms.Button btnPdf;
        private System.Windows.Forms.Button btnJson;
        private System.Windows.Forms.Button btnCerrar;
    }
}
