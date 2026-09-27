namespace CAPAS
{
    partial class frmGestionFranjas
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
            this.dgvFranjas = new System.Windows.Forms.DataGridView();
            this.lblDia = new System.Windows.Forms.Label();
            this.cboDia = new System.Windows.Forms.ComboBox();
            this.lblHoraInicio = new System.Windows.Forms.Label();
            this.dtpHoraInicio = new System.Windows.Forms.DateTimePicker();
            this.lblHoraFin = new System.Windows.Forms.Label();
            this.dtpHoraFin = new System.Windows.Forms.DateTimePicker();
            this.lblRol = new System.Windows.Forms.Label();
            this.cboRol = new System.Windows.Forms.ComboBox();
            this.btnAgregar = new System.Windows.Forms.Button();
            this.btnEliminar = new System.Windows.Forms.Button();
            this.btnExportar = new System.Windows.Forms.Button();
            this.btnCerrar = new System.Windows.Forms.Button();
            ((System.ComponentModel.ISupportInitialize)(this.dgvFranjas)).BeginInit();
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
            this.lblTitulo.Text = "Gestión de franjas horarias";
            //
            // dgvFranjas
            //
            this.dgvFranjas.AllowUserToAddRows = false;
            this.dgvFranjas.AllowUserToDeleteRows = false;
            this.dgvFranjas.ReadOnly = true;
            this.dgvFranjas.SelectionMode = System.Windows.Forms.DataGridViewSelectionMode.FullRowSelect;
            this.dgvFranjas.AutoSizeColumnsMode = System.Windows.Forms.DataGridViewAutoSizeColumnsMode.Fill;
            this.dgvFranjas.Location = new System.Drawing.Point(30, 70);
            this.dgvFranjas.Name = "dgvFranjas";
            this.dgvFranjas.RowTemplate.Height = 25;
            this.dgvFranjas.Size = new System.Drawing.Size(500, 300);
            this.dgvFranjas.TabIndex = 1;
            //
            // lblDia
            //
            this.lblDia.AutoSize = true;
            this.lblDia.Location = new System.Drawing.Point(30, 390);
            this.lblDia.Name = "lblDia";
            this.lblDia.Size = new System.Drawing.Size(30, 15);
            this.lblDia.TabIndex = 2;
            this.lblDia.Text = "Día:";
            //
            // cboDia
            //
            this.cboDia.DropDownStyle = System.Windows.Forms.ComboBoxStyle.DropDownList;
            this.cboDia.FormattingEnabled = true;
            this.cboDia.Location = new System.Drawing.Point(90, 386);
            this.cboDia.Name = "cboDia";
            this.cboDia.Size = new System.Drawing.Size(120, 23);
            this.cboDia.TabIndex = 3;
            //
            // lblHoraInicio
            //
            this.lblHoraInicio.AutoSize = true;
            this.lblHoraInicio.Location = new System.Drawing.Point(230, 390);
            this.lblHoraInicio.Name = "lblHoraInicio";
            this.lblHoraInicio.Size = new System.Drawing.Size(30, 15);
            this.lblHoraInicio.TabIndex = 4;
            this.lblHoraInicio.Text = "De:";
            //
            // dtpHoraInicio
            //
            this.dtpHoraInicio.Format = System.Windows.Forms.DateTimePickerFormat.Time;
            this.dtpHoraInicio.ShowUpDown = true;
            this.dtpHoraInicio.Location = new System.Drawing.Point(265, 386);
            this.dtpHoraInicio.Name = "dtpHoraInicio";
            this.dtpHoraInicio.Size = new System.Drawing.Size(90, 23);
            this.dtpHoraInicio.TabIndex = 5;
            //
            // lblHoraFin
            //
            this.lblHoraFin.AutoSize = true;
            this.lblHoraFin.Location = new System.Drawing.Point(365, 390);
            this.lblHoraFin.Name = "lblHoraFin";
            this.lblHoraFin.Size = new System.Drawing.Size(30, 15);
            this.lblHoraFin.TabIndex = 6;
            this.lblHoraFin.Text = "A:";
            //
            // dtpHoraFin
            //
            this.dtpHoraFin.Format = System.Windows.Forms.DateTimePickerFormat.Time;
            this.dtpHoraFin.ShowUpDown = true;
            this.dtpHoraFin.Location = new System.Drawing.Point(395, 386);
            this.dtpHoraFin.Name = "dtpHoraFin";
            this.dtpHoraFin.Size = new System.Drawing.Size(90, 23);
            this.dtpHoraFin.TabIndex = 7;
            //
            // lblRol
            //
            this.lblRol.AutoSize = true;
            this.lblRol.Location = new System.Drawing.Point(30, 425);
            this.lblRol.Name = "lblRol";
            this.lblRol.Size = new System.Drawing.Size(30, 15);
            this.lblRol.TabIndex = 8;
            this.lblRol.Text = "Rol:";
            //
            // cboRol
            //
            this.cboRol.DropDownStyle = System.Windows.Forms.ComboBoxStyle.DropDownList;
            this.cboRol.FormattingEnabled = true;
            this.cboRol.Location = new System.Drawing.Point(90, 421);
            this.cboRol.Name = "cboRol";
            this.cboRol.Size = new System.Drawing.Size(120, 23);
            this.cboRol.TabIndex = 9;
            //
            // btnAgregar
            //
            this.btnAgregar.Location = new System.Drawing.Point(230, 421);
            this.btnAgregar.Name = "btnAgregar";
            this.btnAgregar.Size = new System.Drawing.Size(110, 30);
            this.btnAgregar.TabIndex = 10;
            this.btnAgregar.Text = "Agregar";
            this.btnAgregar.UseVisualStyleBackColor = true;
            this.btnAgregar.Click += new System.EventHandler(this.btnAgregar_Click);
            //
            // btnEliminar
            //
            this.btnEliminar.Location = new System.Drawing.Point(350, 421);
            this.btnEliminar.Name = "btnEliminar";
            this.btnEliminar.Size = new System.Drawing.Size(180, 30);
            this.btnEliminar.TabIndex = 11;
            this.btnEliminar.Text = "Eliminar seleccionada";
            this.btnEliminar.UseVisualStyleBackColor = true;
            this.btnEliminar.Click += new System.EventHandler(this.btnEliminar_Click);
            //
            // btnExportar
            //
            this.btnExportar.Location = new System.Drawing.Point(300, 465);
            this.btnExportar.Name = "btnExportar";
            this.btnExportar.Size = new System.Drawing.Size(150, 32);
            this.btnExportar.TabIndex = 12;
            this.btnExportar.Text = "Exportar a Excel";
            this.btnExportar.UseVisualStyleBackColor = true;
            this.btnExportar.Click += new System.EventHandler(this.btnExportar_Click);
            //
            // btnCerrar
            //
            this.btnCerrar.Location = new System.Drawing.Point(455, 465);
            this.btnCerrar.Name = "btnCerrar";
            this.btnCerrar.Size = new System.Drawing.Size(75, 32);
            this.btnCerrar.TabIndex = 13;
            this.btnCerrar.Text = "Cerrar";
            this.btnCerrar.UseVisualStyleBackColor = true;
            this.btnCerrar.Click += new System.EventHandler(this.btnCerrar_Click);
            //
            // frmGestionFranjas
            //
            this.AutoScaleDimensions = new System.Drawing.SizeF(7F, 15F);
            this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
            this.ClientSize = new System.Drawing.Size(560, 520);
            this.Controls.Add(this.btnCerrar);
            this.Controls.Add(this.btnExportar);
            this.Controls.Add(this.btnEliminar);
            this.Controls.Add(this.btnAgregar);
            this.Controls.Add(this.cboRol);
            this.Controls.Add(this.lblRol);
            this.Controls.Add(this.dtpHoraFin);
            this.Controls.Add(this.lblHoraFin);
            this.Controls.Add(this.dtpHoraInicio);
            this.Controls.Add(this.lblHoraInicio);
            this.Controls.Add(this.cboDia);
            this.Controls.Add(this.lblDia);
            this.Controls.Add(this.dgvFranjas);
            this.Controls.Add(this.lblTitulo);
            this.MinimumSize = new System.Drawing.Size(576, 559);
            this.Name = "frmGestionFranjas";
            this.StartPosition = System.Windows.Forms.FormStartPosition.CenterParent;
            this.Text = "frmGestionFranjas";
            this.Load += new System.EventHandler(this.frmGestionFranjas_Load);
            this.FormClosed += new System.Windows.Forms.FormClosedEventHandler(this.frmGestionFranjas_FormClosed);
            ((System.ComponentModel.ISupportInitialize)(this.dgvFranjas)).EndInit();
            this.ResumeLayout(false);
            this.PerformLayout();
        }

        private System.Windows.Forms.Label lblTitulo;
        private System.Windows.Forms.DataGridView dgvFranjas;
        private System.Windows.Forms.Label lblDia;
        private System.Windows.Forms.ComboBox cboDia;
        private System.Windows.Forms.Label lblHoraInicio;
        private System.Windows.Forms.DateTimePicker dtpHoraInicio;
        private System.Windows.Forms.Label lblHoraFin;
        private System.Windows.Forms.DateTimePicker dtpHoraFin;
        private System.Windows.Forms.Label lblRol;
        private System.Windows.Forms.ComboBox cboRol;
        private System.Windows.Forms.Button btnAgregar;
        private System.Windows.Forms.Button btnEliminar;
        private System.Windows.Forms.Button btnExportar;
        private System.Windows.Forms.Button btnCerrar;
    }
}