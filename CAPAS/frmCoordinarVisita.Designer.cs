namespace CAPAS
{
    partial class frmCoordinarVisita
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
            this.dgvAlertas = new System.Windows.Forms.DataGridView();
            this.lblTecnico = new System.Windows.Forms.Label();
            this.btnBuscarAlternativo = new System.Windows.Forms.Button();
            this.lblFecha = new System.Windows.Forms.Label();
            this.dtpFecha = new System.Windows.Forms.DateTimePicker();
            this.btnConfirmar = new System.Windows.Forms.Button();
            this.btnCerrar = new System.Windows.Forms.Button();
            ((System.ComponentModel.ISupportInitialize)(this.dgvAlertas)).BeginInit();
            this.SuspendLayout();
            //
            // lblTitulo
            //
            this.lblTitulo.AutoSize = true;
            this.lblTitulo.Font = new System.Drawing.Font("Segoe UI", 12F, System.Drawing.FontStyle.Bold);
            this.lblTitulo.Location = new System.Drawing.Point(30, 30);
            this.lblTitulo.Name = "lblTitulo";
            this.lblTitulo.Size = new System.Drawing.Size(200, 21);
            this.lblTitulo.TabIndex = 0;
            this.lblTitulo.Text = "Coordinar visita técnica";
            //
            // dgvAlertas
            //
            this.dgvAlertas.AllowUserToAddRows = false;
            this.dgvAlertas.AllowUserToDeleteRows = false;
            this.dgvAlertas.ReadOnly = true;
            this.dgvAlertas.SelectionMode = System.Windows.Forms.DataGridViewSelectionMode.FullRowSelect;
            this.dgvAlertas.AutoSizeColumnsMode = System.Windows.Forms.DataGridViewAutoSizeColumnsMode.Fill;
            this.dgvAlertas.Location = new System.Drawing.Point(30, 70);
            this.dgvAlertas.Name = "dgvAlertas";
            this.dgvAlertas.RowTemplate.Height = 25;
            this.dgvAlertas.Size = new System.Drawing.Size(600, 250);
            this.dgvAlertas.TabIndex = 1;
            this.dgvAlertas.SelectionChanged += new System.EventHandler(this.dgvAlertas_SelectionChanged);
            //
            // lblTecnico
            //
            this.lblTecnico.AutoSize = true;
            this.lblTecnico.Location = new System.Drawing.Point(30, 335);
            this.lblTecnico.Name = "lblTecnico";
            this.lblTecnico.Size = new System.Drawing.Size(230, 15);
            this.lblTecnico.TabIndex = 2;
            this.lblTecnico.Text = "Técnico: (seleccioná una alerta)";
            //
            // btnBuscarAlternativo
            //
            this.btnBuscarAlternativo.Location = new System.Drawing.Point(430, 330);
            this.btnBuscarAlternativo.Name = "btnBuscarAlternativo";
            this.btnBuscarAlternativo.Size = new System.Drawing.Size(200, 30);
            this.btnBuscarAlternativo.TabIndex = 3;
            this.btnBuscarAlternativo.Text = "Buscar alternativo";
            this.btnBuscarAlternativo.UseVisualStyleBackColor = true;
            this.btnBuscarAlternativo.Click += new System.EventHandler(this.btnBuscarAlternativo_Click);
            //
            // lblFecha
            //
            this.lblFecha.AutoSize = true;
            this.lblFecha.Location = new System.Drawing.Point(30, 380);
            this.lblFecha.Name = "lblFecha";
            this.lblFecha.Size = new System.Drawing.Size(120, 15);
            this.lblFecha.TabIndex = 4;
            this.lblFecha.Text = "Fecha coordinada:";
            //
            // dtpFecha
            //
            this.dtpFecha.Format = System.Windows.Forms.DateTimePickerFormat.Short;
            this.dtpFecha.Location = new System.Drawing.Point(160, 376);
            this.dtpFecha.Name = "dtpFecha";
            this.dtpFecha.Size = new System.Drawing.Size(150, 23);
            this.dtpFecha.TabIndex = 5;
            //
            // btnConfirmar
            //
            this.btnConfirmar.Location = new System.Drawing.Point(30, 420);
            this.btnConfirmar.Name = "btnConfirmar";
            this.btnConfirmar.Size = new System.Drawing.Size(200, 32);
            this.btnConfirmar.TabIndex = 6;
            this.btnConfirmar.Text = "Confirmar solicitud de visita";
            this.btnConfirmar.UseVisualStyleBackColor = true;
            this.btnConfirmar.Click += new System.EventHandler(this.btnConfirmar_Click);
            //
            // btnCerrar
            //
            this.btnCerrar.Location = new System.Drawing.Point(555, 420);
            this.btnCerrar.Name = "btnCerrar";
            this.btnCerrar.Size = new System.Drawing.Size(75, 32);
            this.btnCerrar.TabIndex = 7;
            this.btnCerrar.Text = "Cerrar";
            this.btnCerrar.UseVisualStyleBackColor = true;
            this.btnCerrar.Click += new System.EventHandler(this.btnCerrar_Click);
            //
            // frmCoordinarVisita
            //
            this.AutoScaleDimensions = new System.Drawing.SizeF(7F, 15F);
            this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
            this.ClientSize = new System.Drawing.Size(660, 470);
            this.Controls.Add(this.btnCerrar);
            this.Controls.Add(this.btnConfirmar);
            this.Controls.Add(this.dtpFecha);
            this.Controls.Add(this.lblFecha);
            this.Controls.Add(this.btnBuscarAlternativo);
            this.Controls.Add(this.lblTecnico);
            this.Controls.Add(this.dgvAlertas);
            this.Controls.Add(this.lblTitulo);
            this.MinimumSize = new System.Drawing.Size(676, 509);
            this.Name = "frmCoordinarVisita";
            this.StartPosition = System.Windows.Forms.FormStartPosition.CenterParent;
            this.Text = "frmCoordinarVisita";
            this.Load += new System.EventHandler(this.frmCoordinarVisita_Load);
            this.FormClosed += new System.Windows.Forms.FormClosedEventHandler(this.frmCoordinarVisita_FormClosed);
            ((System.ComponentModel.ISupportInitialize)(this.dgvAlertas)).EndInit();
            this.ResumeLayout(false);
            this.PerformLayout();
        }

        private System.Windows.Forms.Label lblTitulo;
        private System.Windows.Forms.DataGridView dgvAlertas;
        private System.Windows.Forms.Label lblTecnico;
        private System.Windows.Forms.Button btnBuscarAlternativo;
        private System.Windows.Forms.Label lblFecha;
        private System.Windows.Forms.DateTimePicker dtpFecha;
        private System.Windows.Forms.Button btnConfirmar;
        private System.Windows.Forms.Button btnCerrar;
    }
}