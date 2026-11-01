namespace CAPAS
{
    partial class frmParametrosLiquidacion
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
            this.tabs = new System.Windows.Forms.TabControl();
            this.tabParametros = new System.Windows.Forms.TabPage();
            this.tabValorHora = new System.Windows.Forms.TabPage();
            this.tabFeriados = new System.Windows.Forms.TabPage();
            this.lblTolerancia = new System.Windows.Forms.Label();
            this.nudTolerancia = new System.Windows.Forms.NumericUpDown();
            this.lblRedondeo = new System.Windows.Forms.Label();
            this.nudRedondeo = new System.Windows.Forms.NumericUpDown();
            this.lblJornada = new System.Windows.Forms.Label();
            this.nudJornada = new System.Windows.Forms.NumericUpDown();
            this.lblRecargoExtra = new System.Windows.Forms.Label();
            this.nudRecargoExtra = new System.Windows.Forms.NumericUpDown();
            this.lblRecargoFeriado = new System.Windows.Forms.Label();
            this.nudRecargoFeriado = new System.Windows.Forms.NumericUpDown();
            this.lblMaxHoras = new System.Windows.Forms.Label();
            this.nudMaxHoras = new System.Windows.Forms.NumericUpDown();
            this.btnGuardarParametros = new System.Windows.Forms.Button();
            this.lblEmpleadoValor = new System.Windows.Forms.Label();
            this.cboEmpleadoValor = new System.Windows.Forms.ComboBox();
            this.lblValor = new System.Windows.Forms.Label();
            this.nudValor = new System.Windows.Forms.NumericUpDown();
            this.lblVigente = new System.Windows.Forms.Label();
            this.dtpVigente = new System.Windows.Forms.DateTimePicker();
            this.btnGuardarValor = new System.Windows.Forms.Button();
            this.dgvValores = new System.Windows.Forms.DataGridView();
            this.lblFeriado = new System.Windows.Forms.Label();
            this.dtpFeriado = new System.Windows.Forms.DateTimePicker();
            this.lblDescripcion = new System.Windows.Forms.Label();
            this.txtDescripcion = new System.Windows.Forms.TextBox();
            this.btnGuardarFeriado = new System.Windows.Forms.Button();
            this.dgvFeriados = new System.Windows.Forms.DataGridView();
            this.btnEliminarFeriado = new System.Windows.Forms.Button();
            this.btnImportarFeriados = new System.Windows.Forms.Button();
            this.btnCerrar = new System.Windows.Forms.Button();
            this.tabs.SuspendLayout();
            this.tabParametros.SuspendLayout();
            this.tabValorHora.SuspendLayout();
            this.tabFeriados.SuspendLayout();
            ((System.ComponentModel.ISupportInitialize)(this.nudTolerancia)).BeginInit();
            ((System.ComponentModel.ISupportInitialize)(this.nudRedondeo)).BeginInit();
            ((System.ComponentModel.ISupportInitialize)(this.nudJornada)).BeginInit();
            ((System.ComponentModel.ISupportInitialize)(this.nudRecargoExtra)).BeginInit();
            ((System.ComponentModel.ISupportInitialize)(this.nudRecargoFeriado)).BeginInit();
            ((System.ComponentModel.ISupportInitialize)(this.nudMaxHoras)).BeginInit();
            ((System.ComponentModel.ISupportInitialize)(this.nudValor)).BeginInit();
            ((System.ComponentModel.ISupportInitialize)(this.dgvValores)).BeginInit();
            ((System.ComponentModel.ISupportInitialize)(this.dgvFeriados)).BeginInit();
            this.SuspendLayout();
            //
            // lblTitulo
            //
            this.lblTitulo.AutoSize = true;
            this.lblTitulo.Font = new System.Drawing.Font("Segoe UI", 12F, System.Drawing.FontStyle.Bold);
            this.lblTitulo.Location = new System.Drawing.Point(30, 75);
            this.lblTitulo.Name = "lblTitulo";
            this.lblTitulo.Size = new System.Drawing.Size(300, 21);
            this.lblTitulo.TabIndex = 0;
            this.lblTitulo.Text = "Parámetros de liquidación";
            //
            // tabs
            //
            this.tabs.Controls.Add(this.tabParametros);
            this.tabs.Controls.Add(this.tabValorHora);
            this.tabs.Controls.Add(this.tabFeriados);
            this.tabs.Location = new System.Drawing.Point(30, 115);
            this.tabs.Name = "tabs";
            this.tabs.SelectedIndex = 0;
            this.tabs.Size = new System.Drawing.Size(700, 390);
            this.tabs.TabIndex = 1;
            //
            // tabParametros
            //
            this.tabParametros.Controls.Add(this.btnGuardarParametros);
            this.tabParametros.Controls.Add(this.nudMaxHoras);
            this.tabParametros.Controls.Add(this.lblMaxHoras);
            this.tabParametros.Controls.Add(this.nudRecargoFeriado);
            this.tabParametros.Controls.Add(this.lblRecargoFeriado);
            this.tabParametros.Controls.Add(this.nudRecargoExtra);
            this.tabParametros.Controls.Add(this.lblRecargoExtra);
            this.tabParametros.Controls.Add(this.nudJornada);
            this.tabParametros.Controls.Add(this.lblJornada);
            this.tabParametros.Controls.Add(this.nudRedondeo);
            this.tabParametros.Controls.Add(this.lblRedondeo);
            this.tabParametros.Controls.Add(this.nudTolerancia);
            this.tabParametros.Controls.Add(this.lblTolerancia);
            this.tabParametros.Location = new System.Drawing.Point(4, 24);
            this.tabParametros.Name = "tabParametros";
            this.tabParametros.Padding = new System.Windows.Forms.Padding(3);
            this.tabParametros.Size = new System.Drawing.Size(692, 362);
            this.tabParametros.TabIndex = 0;
            this.tabParametros.Text = "Parámetros";
            this.tabParametros.UseVisualStyleBackColor = true;
            //
            // lblTolerancia
            //
            this.lblTolerancia.AutoSize = true;
            this.lblTolerancia.Location = new System.Drawing.Point(20, 28);
            this.lblTolerancia.Name = "lblTolerancia";
            this.lblTolerancia.Size = new System.Drawing.Size(140, 15);
            this.lblTolerancia.TabIndex = 0;
            this.lblTolerancia.Text = "Tolerancia (minutos):";
            //
            // nudTolerancia
            //
            this.nudTolerancia.Location = new System.Drawing.Point(360, 24);
            this.nudTolerancia.Maximum = new decimal(new int[] { 60, 0, 0, 0 });
            this.nudTolerancia.Name = "nudTolerancia";
            this.nudTolerancia.Size = new System.Drawing.Size(100, 23);
            this.nudTolerancia.TabIndex = 1;
            //
            // lblRedondeo
            //
            this.lblRedondeo.AutoSize = true;
            this.lblRedondeo.Location = new System.Drawing.Point(20, 68);
            this.lblRedondeo.Name = "lblRedondeo";
            this.lblRedondeo.Size = new System.Drawing.Size(220, 15);
            this.lblRedondeo.TabIndex = 2;
            this.lblRedondeo.Text = "Redondeo (minutos, 0 = sin redondeo):";
            //
            // nudRedondeo
            //
            this.nudRedondeo.Location = new System.Drawing.Point(360, 64);
            this.nudRedondeo.Maximum = new decimal(new int[] { 60, 0, 0, 0 });
            this.nudRedondeo.Name = "nudRedondeo";
            this.nudRedondeo.Size = new System.Drawing.Size(100, 23);
            this.nudRedondeo.TabIndex = 3;
            //
            // lblJornada
            //
            this.lblJornada.AutoSize = true;
            this.lblJornada.Location = new System.Drawing.Point(20, 108);
            this.lblJornada.Name = "lblJornada";
            this.lblJornada.Size = new System.Drawing.Size(220, 15);
            this.lblJornada.TabIndex = 4;
            this.lblJornada.Text = "Jornada normal (horas por día):";
            //
            // nudJornada
            //
            this.nudJornada.DecimalPlaces = 1;
            this.nudJornada.Increment = new decimal(new int[] { 5, 0, 0, 65536 });
            this.nudJornada.Location = new System.Drawing.Point(360, 104);
            this.nudJornada.Maximum = new decimal(new int[] { 24, 0, 0, 0 });
            this.nudJornada.Minimum = new decimal(new int[] { 1, 0, 0, 0 });
            this.nudJornada.Name = "nudJornada";
            this.nudJornada.Size = new System.Drawing.Size(100, 23);
            this.nudJornada.TabIndex = 5;
            this.nudJornada.Value = new decimal(new int[] { 8, 0, 0, 0 });
            //
            // lblRecargoExtra
            //
            this.lblRecargoExtra.AutoSize = true;
            this.lblRecargoExtra.Location = new System.Drawing.Point(20, 148);
            this.lblRecargoExtra.Name = "lblRecargoExtra";
            this.lblRecargoExtra.Size = new System.Drawing.Size(220, 15);
            this.lblRecargoExtra.TabIndex = 6;
            this.lblRecargoExtra.Text = "Recargo de horas extra (%):";
            //
            // nudRecargoExtra
            //
            this.nudRecargoExtra.Location = new System.Drawing.Point(360, 144);
            this.nudRecargoExtra.Maximum = new decimal(new int[] { 300, 0, 0, 0 });
            this.nudRecargoExtra.Name = "nudRecargoExtra";
            this.nudRecargoExtra.Size = new System.Drawing.Size(100, 23);
            this.nudRecargoExtra.TabIndex = 7;
            //
            // lblRecargoFeriado
            //
            this.lblRecargoFeriado.AutoSize = true;
            this.lblRecargoFeriado.Location = new System.Drawing.Point(20, 188);
            this.lblRecargoFeriado.Name = "lblRecargoFeriado";
            this.lblRecargoFeriado.Size = new System.Drawing.Size(220, 15);
            this.lblRecargoFeriado.TabIndex = 8;
            this.lblRecargoFeriado.Text = "Recargo de domingo o feriado (%):";
            //
            // nudRecargoFeriado
            //
            this.nudRecargoFeriado.Location = new System.Drawing.Point(360, 184);
            this.nudRecargoFeriado.Maximum = new decimal(new int[] { 300, 0, 0, 0 });
            this.nudRecargoFeriado.Name = "nudRecargoFeriado";
            this.nudRecargoFeriado.Size = new System.Drawing.Size(100, 23);
            this.nudRecargoFeriado.TabIndex = 9;
            //
            // lblMaxHoras
            //
            this.lblMaxHoras.AutoSize = true;
            this.lblMaxHoras.Location = new System.Drawing.Point(20, 228);
            this.lblMaxHoras.Name = "lblMaxHoras";
            this.lblMaxHoras.Size = new System.Drawing.Size(300, 15);
            this.lblMaxHoras.TabIndex = 10;
            this.lblMaxHoras.Text = "Horas máximas de un ingreso sin egreso:";
            //
            // nudMaxHoras
            //
            this.nudMaxHoras.Location = new System.Drawing.Point(360, 224);
            this.nudMaxHoras.Maximum = new decimal(new int[] { 24, 0, 0, 0 });
            this.nudMaxHoras.Minimum = new decimal(new int[] { 1, 0, 0, 0 });
            this.nudMaxHoras.Name = "nudMaxHoras";
            this.nudMaxHoras.Size = new System.Drawing.Size(100, 23);
            this.nudMaxHoras.TabIndex = 11;
            this.nudMaxHoras.Value = new decimal(new int[] { 16, 0, 0, 0 });
            //
            // btnGuardarParametros
            //
            this.btnGuardarParametros.Location = new System.Drawing.Point(360, 275);
            this.btnGuardarParametros.Name = "btnGuardarParametros";
            this.btnGuardarParametros.Size = new System.Drawing.Size(200, 32);
            this.btnGuardarParametros.TabIndex = 12;
            this.btnGuardarParametros.Text = "Guardar parámetros";
            this.btnGuardarParametros.UseVisualStyleBackColor = true;
            this.btnGuardarParametros.Click += new System.EventHandler(this.btnGuardarParametros_Click);
            //
            // tabValorHora
            //
            this.tabValorHora.Controls.Add(this.dgvValores);
            this.tabValorHora.Controls.Add(this.btnGuardarValor);
            this.tabValorHora.Controls.Add(this.dtpVigente);
            this.tabValorHora.Controls.Add(this.lblVigente);
            this.tabValorHora.Controls.Add(this.nudValor);
            this.tabValorHora.Controls.Add(this.lblValor);
            this.tabValorHora.Controls.Add(this.cboEmpleadoValor);
            this.tabValorHora.Controls.Add(this.lblEmpleadoValor);
            this.tabValorHora.Location = new System.Drawing.Point(4, 24);
            this.tabValorHora.Name = "tabValorHora";
            this.tabValorHora.Padding = new System.Windows.Forms.Padding(3);
            this.tabValorHora.Size = new System.Drawing.Size(692, 362);
            this.tabValorHora.TabIndex = 1;
            this.tabValorHora.Text = "Valor hora";
            this.tabValorHora.UseVisualStyleBackColor = true;
            //
            // lblEmpleadoValor
            //
            this.lblEmpleadoValor.AutoSize = true;
            this.lblEmpleadoValor.Location = new System.Drawing.Point(20, 25);
            this.lblEmpleadoValor.Name = "lblEmpleadoValor";
            this.lblEmpleadoValor.Size = new System.Drawing.Size(65, 15);
            this.lblEmpleadoValor.TabIndex = 0;
            this.lblEmpleadoValor.Text = "Empleado:";
            //
            // cboEmpleadoValor
            //
            this.cboEmpleadoValor.DropDownStyle = System.Windows.Forms.ComboBoxStyle.DropDownList;
            this.cboEmpleadoValor.FormattingEnabled = true;
            this.cboEmpleadoValor.Location = new System.Drawing.Point(110, 21);
            this.cboEmpleadoValor.Name = "cboEmpleadoValor";
            this.cboEmpleadoValor.Size = new System.Drawing.Size(240, 23);
            this.cboEmpleadoValor.TabIndex = 1;
            //
            // lblValor
            //
            this.lblValor.AutoSize = true;
            this.lblValor.Location = new System.Drawing.Point(375, 25);
            this.lblValor.Name = "lblValor";
            this.lblValor.Size = new System.Drawing.Size(70, 15);
            this.lblValor.TabIndex = 2;
            this.lblValor.Text = "Valor hora:";
            //
            // nudValor
            //
            this.nudValor.DecimalPlaces = 2;
            this.nudValor.Location = new System.Drawing.Point(460, 21);
            this.nudValor.Maximum = new decimal(new int[] { 10000000, 0, 0, 0 });
            this.nudValor.Name = "nudValor";
            this.nudValor.Size = new System.Drawing.Size(120, 23);
            this.nudValor.TabIndex = 3;
            this.nudValor.ThousandsSeparator = true;
            //
            // lblVigente
            //
            this.lblVigente.AutoSize = true;
            this.lblVigente.Location = new System.Drawing.Point(20, 68);
            this.lblVigente.Name = "lblVigente";
            this.lblVigente.Size = new System.Drawing.Size(85, 15);
            this.lblVigente.TabIndex = 4;
            this.lblVigente.Text = "Vigente desde:";
            //
            // dtpVigente
            //
            this.dtpVigente.Format = System.Windows.Forms.DateTimePickerFormat.Short;
            this.dtpVigente.Location = new System.Drawing.Point(110, 64);
            this.dtpVigente.Name = "dtpVigente";
            this.dtpVigente.Size = new System.Drawing.Size(130, 23);
            this.dtpVigente.TabIndex = 5;
            //
            // btnGuardarValor
            //
            this.btnGuardarValor.Location = new System.Drawing.Point(260, 60);
            this.btnGuardarValor.Name = "btnGuardarValor";
            this.btnGuardarValor.Size = new System.Drawing.Size(170, 30);
            this.btnGuardarValor.TabIndex = 6;
            this.btnGuardarValor.Text = "Guardar valor";
            this.btnGuardarValor.UseVisualStyleBackColor = true;
            this.btnGuardarValor.Click += new System.EventHandler(this.btnGuardarValor_Click);
            //
            // dgvValores
            //
            this.dgvValores.AllowUserToAddRows = false;
            this.dgvValores.AllowUserToDeleteRows = false;
            this.dgvValores.ReadOnly = true;
            this.dgvValores.SelectionMode = System.Windows.Forms.DataGridViewSelectionMode.FullRowSelect;
            this.dgvValores.AutoSizeColumnsMode = System.Windows.Forms.DataGridViewAutoSizeColumnsMode.Fill;
            this.dgvValores.Location = new System.Drawing.Point(20, 110);
            this.dgvValores.Name = "dgvValores";
            this.dgvValores.RowTemplate.Height = 25;
            this.dgvValores.Size = new System.Drawing.Size(650, 235);
            this.dgvValores.TabIndex = 7;
            //
            // tabFeriados
            //
            this.tabFeriados.Controls.Add(this.btnImportarFeriados);
            this.tabFeriados.Controls.Add(this.btnEliminarFeriado);
            this.tabFeriados.Controls.Add(this.dgvFeriados);
            this.tabFeriados.Controls.Add(this.btnGuardarFeriado);
            this.tabFeriados.Controls.Add(this.txtDescripcion);
            this.tabFeriados.Controls.Add(this.lblDescripcion);
            this.tabFeriados.Controls.Add(this.dtpFeriado);
            this.tabFeriados.Controls.Add(this.lblFeriado);
            this.tabFeriados.Location = new System.Drawing.Point(4, 24);
            this.tabFeriados.Name = "tabFeriados";
            this.tabFeriados.Padding = new System.Windows.Forms.Padding(3);
            this.tabFeriados.Size = new System.Drawing.Size(692, 362);
            this.tabFeriados.TabIndex = 2;
            this.tabFeriados.Text = "Feriados";
            this.tabFeriados.UseVisualStyleBackColor = true;
            //
            // lblFeriado
            //
            this.lblFeriado.AutoSize = true;
            this.lblFeriado.Location = new System.Drawing.Point(20, 25);
            this.lblFeriado.Name = "lblFeriado";
            this.lblFeriado.Size = new System.Drawing.Size(42, 15);
            this.lblFeriado.TabIndex = 0;
            this.lblFeriado.Text = "Fecha:";
            //
            // dtpFeriado
            //
            this.dtpFeriado.Format = System.Windows.Forms.DateTimePickerFormat.Short;
            this.dtpFeriado.Location = new System.Drawing.Point(75, 21);
            this.dtpFeriado.Name = "dtpFeriado";
            this.dtpFeriado.Size = new System.Drawing.Size(130, 23);
            this.dtpFeriado.TabIndex = 1;
            //
            // lblDescripcion
            //
            this.lblDescripcion.AutoSize = true;
            this.lblDescripcion.Location = new System.Drawing.Point(225, 25);
            this.lblDescripcion.Name = "lblDescripcion";
            this.lblDescripcion.Size = new System.Drawing.Size(75, 15);
            this.lblDescripcion.TabIndex = 2;
            this.lblDescripcion.Text = "Descripción:";
            //
            // txtDescripcion
            //
            this.txtDescripcion.Location = new System.Drawing.Point(310, 21);
            this.txtDescripcion.MaxLength = 100;
            this.txtDescripcion.Name = "txtDescripcion";
            this.txtDescripcion.Size = new System.Drawing.Size(190, 23);
            this.txtDescripcion.TabIndex = 3;
            //
            // btnGuardarFeriado
            //
            this.btnGuardarFeriado.Location = new System.Drawing.Point(515, 18);
            this.btnGuardarFeriado.Name = "btnGuardarFeriado";
            this.btnGuardarFeriado.Size = new System.Drawing.Size(155, 30);
            this.btnGuardarFeriado.TabIndex = 4;
            this.btnGuardarFeriado.Text = "Guardar feriado";
            this.btnGuardarFeriado.UseVisualStyleBackColor = true;
            this.btnGuardarFeriado.Click += new System.EventHandler(this.btnGuardarFeriado_Click);
            //
            // dgvFeriados
            //
            this.dgvFeriados.AllowUserToAddRows = false;
            this.dgvFeriados.AllowUserToDeleteRows = false;
            this.dgvFeriados.ReadOnly = true;
            this.dgvFeriados.MultiSelect = false;
            this.dgvFeriados.SelectionMode = System.Windows.Forms.DataGridViewSelectionMode.FullRowSelect;
            this.dgvFeriados.AutoSizeColumnsMode = System.Windows.Forms.DataGridViewAutoSizeColumnsMode.Fill;
            this.dgvFeriados.Location = new System.Drawing.Point(20, 65);
            this.dgvFeriados.Name = "dgvFeriados";
            this.dgvFeriados.RowTemplate.Height = 25;
            this.dgvFeriados.Size = new System.Drawing.Size(650, 240);
            this.dgvFeriados.TabIndex = 5;
            //
            // btnEliminarFeriado
            //
            this.btnEliminarFeriado.Location = new System.Drawing.Point(20, 315);
            this.btnEliminarFeriado.Name = "btnEliminarFeriado";
            this.btnEliminarFeriado.Size = new System.Drawing.Size(170, 30);
            this.btnEliminarFeriado.TabIndex = 6;
            this.btnEliminarFeriado.Text = "Eliminar feriado";
            this.btnEliminarFeriado.UseVisualStyleBackColor = true;
            this.btnEliminarFeriado.Click += new System.EventHandler(this.btnEliminarFeriado_Click);
            //
            // btnImportarFeriados
            //
            this.btnImportarFeriados.Location = new System.Drawing.Point(200, 315);
            this.btnImportarFeriados.Name = "btnImportarFeriados";
            this.btnImportarFeriados.Size = new System.Drawing.Size(260, 30);
            this.btnImportarFeriados.TabIndex = 7;
            this.btnImportarFeriados.Text = "Importar feriados del año";
            this.btnImportarFeriados.UseVisualStyleBackColor = true;
            this.btnImportarFeriados.Click += new System.EventHandler(this.btnImportarFeriados_Click);
            //
            // btnCerrar
            //
            this.btnCerrar.Location = new System.Drawing.Point(655, 515);
            this.btnCerrar.Name = "btnCerrar";
            this.btnCerrar.Size = new System.Drawing.Size(75, 30);
            this.btnCerrar.TabIndex = 2;
            this.btnCerrar.Text = "Cerrar";
            this.btnCerrar.UseVisualStyleBackColor = true;
            this.btnCerrar.Click += new System.EventHandler(this.btnCerrar_Click);
            //
            // frmParametrosLiquidacion
            //
            this.AutoScaleDimensions = new System.Drawing.SizeF(7F, 15F);
            this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
            this.ClientSize = new System.Drawing.Size(760, 565);
            this.Controls.Add(this.btnCerrar);
            this.Controls.Add(this.tabs);
            this.Controls.Add(this.lblTitulo);
            this.FormBorderStyle = System.Windows.Forms.FormBorderStyle.FixedDialog;
            this.MaximizeBox = false;
            this.MinimizeBox = false;
            this.Name = "frmParametrosLiquidacion";
            this.StartPosition = System.Windows.Forms.FormStartPosition.CenterParent;
            this.Text = "Parámetros de liquidación";
            this.Load += new System.EventHandler(this.frmParametrosLiquidacion_Load);
            this.tabs.ResumeLayout(false);
            this.tabParametros.ResumeLayout(false);
            this.tabParametros.PerformLayout();
            this.tabValorHora.ResumeLayout(false);
            this.tabValorHora.PerformLayout();
            this.tabFeriados.ResumeLayout(false);
            this.tabFeriados.PerformLayout();
            ((System.ComponentModel.ISupportInitialize)(this.nudTolerancia)).EndInit();
            ((System.ComponentModel.ISupportInitialize)(this.nudRedondeo)).EndInit();
            ((System.ComponentModel.ISupportInitialize)(this.nudJornada)).EndInit();
            ((System.ComponentModel.ISupportInitialize)(this.nudRecargoExtra)).EndInit();
            ((System.ComponentModel.ISupportInitialize)(this.nudRecargoFeriado)).EndInit();
            ((System.ComponentModel.ISupportInitialize)(this.nudMaxHoras)).EndInit();
            ((System.ComponentModel.ISupportInitialize)(this.nudValor)).EndInit();
            ((System.ComponentModel.ISupportInitialize)(this.dgvValores)).EndInit();
            ((System.ComponentModel.ISupportInitialize)(this.dgvFeriados)).EndInit();
            this.ResumeLayout(false);
            this.PerformLayout();
        }

        private System.Windows.Forms.Label lblTitulo;
        private System.Windows.Forms.TabControl tabs;
        private System.Windows.Forms.TabPage tabParametros;
        private System.Windows.Forms.TabPage tabValorHora;
        private System.Windows.Forms.TabPage tabFeriados;
        private System.Windows.Forms.Label lblTolerancia;
        private System.Windows.Forms.NumericUpDown nudTolerancia;
        private System.Windows.Forms.Label lblRedondeo;
        private System.Windows.Forms.NumericUpDown nudRedondeo;
        private System.Windows.Forms.Label lblJornada;
        private System.Windows.Forms.NumericUpDown nudJornada;
        private System.Windows.Forms.Label lblRecargoExtra;
        private System.Windows.Forms.NumericUpDown nudRecargoExtra;
        private System.Windows.Forms.Label lblRecargoFeriado;
        private System.Windows.Forms.NumericUpDown nudRecargoFeriado;
        private System.Windows.Forms.Label lblMaxHoras;
        private System.Windows.Forms.NumericUpDown nudMaxHoras;
        private System.Windows.Forms.Button btnGuardarParametros;
        private System.Windows.Forms.Label lblEmpleadoValor;
        private System.Windows.Forms.ComboBox cboEmpleadoValor;
        private System.Windows.Forms.Label lblValor;
        private System.Windows.Forms.NumericUpDown nudValor;
        private System.Windows.Forms.Label lblVigente;
        private System.Windows.Forms.DateTimePicker dtpVigente;
        private System.Windows.Forms.Button btnGuardarValor;
        private System.Windows.Forms.DataGridView dgvValores;
        private System.Windows.Forms.Label lblFeriado;
        private System.Windows.Forms.DateTimePicker dtpFeriado;
        private System.Windows.Forms.Label lblDescripcion;
        private System.Windows.Forms.TextBox txtDescripcion;
        private System.Windows.Forms.Button btnGuardarFeriado;
        private System.Windows.Forms.DataGridView dgvFeriados;
        private System.Windows.Forms.Button btnEliminarFeriado;
        private System.Windows.Forms.Button btnImportarFeriados;
        private System.Windows.Forms.Button btnCerrar;
    }
}
