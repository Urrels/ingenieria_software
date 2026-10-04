namespace CAPAS
{
    partial class frmRealizarRevision
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
            this.dgvVisitas = new System.Windows.Forms.DataGridView();
            this.lblEquipo = new System.Windows.Forms.Label();
            this.lblResultado = new System.Windows.Forms.Label();
            this.txtResultado = new System.Windows.Forms.TextBox();
            this.chkPendienteRepuesto = new System.Windows.Forms.CheckBox();
            this.btnRegistrarInforme = new System.Windows.Forms.Button();
            this.btnCerrar = new System.Windows.Forms.Button();
            ((System.ComponentModel.ISupportInitialize)(this.dgvVisitas)).BeginInit();
            this.SuspendLayout();
            //
            // lblTitulo
            //
            this.lblTitulo.AutoSize = true;
            this.lblTitulo.Font = new System.Drawing.Font("Segoe UI", 12F, System.Drawing.FontStyle.Bold);
            this.lblTitulo.Location = new System.Drawing.Point(30, 30);
            this.lblTitulo.Name = "lblTitulo";
            this.lblTitulo.Size = new System.Drawing.Size(300, 21);
            this.lblTitulo.TabIndex = 0;
            this.lblTitulo.Text = "Realizar revisión y emitir informe";
            //
            // dgvVisitas
            //
            this.dgvVisitas.AllowUserToAddRows = false;
            this.dgvVisitas.AllowUserToDeleteRows = false;
            this.dgvVisitas.ReadOnly = true;
            this.dgvVisitas.SelectionMode = System.Windows.Forms.DataGridViewSelectionMode.FullRowSelect;
            this.dgvVisitas.AutoSizeColumnsMode = System.Windows.Forms.DataGridViewAutoSizeColumnsMode.Fill;
            this.dgvVisitas.Location = new System.Drawing.Point(30, 70);
            this.dgvVisitas.Name = "dgvVisitas";
            this.dgvVisitas.RowTemplate.Height = 25;
            this.dgvVisitas.Size = new System.Drawing.Size(600, 200);
            this.dgvVisitas.TabIndex = 1;
            this.dgvVisitas.SelectionChanged += new System.EventHandler(this.dgvVisitas_SelectionChanged);
            //
            // lblEquipo
            //
            this.lblEquipo.AutoSize = true;
            this.lblEquipo.Location = new System.Drawing.Point(30, 285);
            this.lblEquipo.Name = "lblEquipo";
            this.lblEquipo.Size = new System.Drawing.Size(60, 15);
            this.lblEquipo.TabIndex = 2;
            this.lblEquipo.Text = "Equipo: —";
            //
            // lblResultado
            //
            this.lblResultado.AutoSize = true;
            this.lblResultado.Location = new System.Drawing.Point(30, 320);
            this.lblResultado.Name = "lblResultado";
            this.lblResultado.Size = new System.Drawing.Size(120, 15);
            this.lblResultado.TabIndex = 3;
            this.lblResultado.Text = "Resultado de la revisión:";
            //
            // txtResultado
            //
            this.txtResultado.Location = new System.Drawing.Point(30, 340);
            this.txtResultado.Multiline = true;
            this.txtResultado.Name = "txtResultado";
            this.txtResultado.Size = new System.Drawing.Size(600, 80);
            this.txtResultado.TabIndex = 4;
            //
            // chkPendienteRepuesto
            //
            this.chkPendienteRepuesto.AutoSize = true;
            this.chkPendienteRepuesto.Location = new System.Drawing.Point(30, 430);
            this.chkPendienteRepuesto.Name = "chkPendienteRepuesto";
            this.chkPendienteRepuesto.Size = new System.Drawing.Size(220, 19);
            this.chkPendienteRepuesto.TabIndex = 5;
            this.chkPendienteRepuesto.Text = "Requiere repuesto no disponible";
            this.chkPendienteRepuesto.UseVisualStyleBackColor = true;
            //
            // btnRegistrarInforme
            //
            this.btnRegistrarInforme.Location = new System.Drawing.Point(30, 465);
            this.btnRegistrarInforme.Name = "btnRegistrarInforme";
            this.btnRegistrarInforme.Size = new System.Drawing.Size(180, 32);
            this.btnRegistrarInforme.TabIndex = 6;
            this.btnRegistrarInforme.Text = "Registrar informe";
            this.btnRegistrarInforme.UseVisualStyleBackColor = true;
            this.btnRegistrarInforme.Click += new System.EventHandler(this.btnRegistrarInforme_Click);
            //
            // btnCerrar
            //
            this.btnCerrar.Location = new System.Drawing.Point(555, 465);
            this.btnCerrar.Name = "btnCerrar";
            this.btnCerrar.Size = new System.Drawing.Size(75, 32);
            this.btnCerrar.TabIndex = 7;
            this.btnCerrar.Text = "Cerrar";
            this.btnCerrar.UseVisualStyleBackColor = true;
            this.btnCerrar.Click += new System.EventHandler(this.btnCerrar_Click);
            //
            // frmRealizarRevision
            //
            this.AutoScaleDimensions = new System.Drawing.SizeF(7F, 15F);
            this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
            this.ClientSize = new System.Drawing.Size(660, 520);
            this.Controls.Add(this.btnCerrar);
            this.Controls.Add(this.btnRegistrarInforme);
            this.Controls.Add(this.chkPendienteRepuesto);
            this.Controls.Add(this.txtResultado);
            this.Controls.Add(this.lblResultado);
            this.Controls.Add(this.lblEquipo);
            this.Controls.Add(this.dgvVisitas);
            this.Controls.Add(this.lblTitulo);
            this.MinimumSize = new System.Drawing.Size(676, 559);
            this.Name = "frmRealizarRevision";
            this.StartPosition = System.Windows.Forms.FormStartPosition.CenterParent;
            this.Text = "Revisión técnica";
            this.Load += new System.EventHandler(this.frmRealizarRevision_Load);
            ((System.ComponentModel.ISupportInitialize)(this.dgvVisitas)).EndInit();
            this.ResumeLayout(false);
            this.PerformLayout();
        }

        private System.Windows.Forms.Label lblTitulo;
        private System.Windows.Forms.DataGridView dgvVisitas;
        private System.Windows.Forms.Label lblEquipo;
        private System.Windows.Forms.Label lblResultado;
        private System.Windows.Forms.TextBox txtResultado;
        private System.Windows.Forms.CheckBox chkPendienteRepuesto;
        private System.Windows.Forms.Button btnRegistrarInforme;
        private System.Windows.Forms.Button btnCerrar;
    }
}