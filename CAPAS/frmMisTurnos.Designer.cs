namespace CAPAS
{
    partial class frmMisTurnos
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
            this.dgvTurnos = new System.Windows.Forms.DataGridView();
            this.btnCancelarTurno = new System.Windows.Forms.Button();
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
            this.lblTitulo.Size = new System.Drawing.Size(150, 21);
            this.lblTitulo.TabIndex = 0;
            this.lblTitulo.Text = "Mis turnos";
            //
            // dgvTurnos
            //
            this.dgvTurnos.AllowUserToAddRows = false;
            this.dgvTurnos.AllowUserToDeleteRows = false;
            this.dgvTurnos.ReadOnly = true;
            this.dgvTurnos.SelectionMode = System.Windows.Forms.DataGridViewSelectionMode.FullRowSelect;
            this.dgvTurnos.AutoSizeColumnsMode = System.Windows.Forms.DataGridViewAutoSizeColumnsMode.Fill;
            this.dgvTurnos.Location = new System.Drawing.Point(30, 70);
            this.dgvTurnos.Name = "dgvTurnos";
            this.dgvTurnos.RowTemplate.Height = 25;
            this.dgvTurnos.Size = new System.Drawing.Size(600, 350);
            this.dgvTurnos.TabIndex = 1;
            //
            // btnCancelarTurno
            //
            this.btnCancelarTurno.Location = new System.Drawing.Point(415, 440);
            this.btnCancelarTurno.Name = "btnCancelarTurno";
            this.btnCancelarTurno.Size = new System.Drawing.Size(140, 32);
            this.btnCancelarTurno.TabIndex = 2;
            this.btnCancelarTurno.Text = "Cancelar turno";
            this.btnCancelarTurno.UseVisualStyleBackColor = true;
            this.btnCancelarTurno.Click += new System.EventHandler(this.btnCancelarTurno_Click);
            //
            // btnCerrar
            //
            this.btnCerrar.Location = new System.Drawing.Point(555, 440);
            this.btnCerrar.Name = "btnCerrar";
            this.btnCerrar.Size = new System.Drawing.Size(75, 32);
            this.btnCerrar.TabIndex = 3;
            this.btnCerrar.Text = "Cerrar";
            this.btnCerrar.UseVisualStyleBackColor = true;
            this.btnCerrar.Click += new System.EventHandler(this.btnCerrar_Click);
            //
            // frmMisTurnos
            //
            this.AutoScaleDimensions = new System.Drawing.SizeF(7F, 15F);
            this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
            this.ClientSize = new System.Drawing.Size(660, 500);
            this.Controls.Add(this.btnCerrar);
            this.Controls.Add(this.btnCancelarTurno);
            this.Controls.Add(this.dgvTurnos);
            this.Controls.Add(this.lblTitulo);
            this.MinimumSize = new System.Drawing.Size(676, 539);
            this.Name = "frmMisTurnos";
            this.StartPosition = System.Windows.Forms.FormStartPosition.CenterParent;
            this.Text = "frmMisTurnos";
            this.Load += new System.EventHandler(this.frmMisTurnos_Load);
            this.FormClosed += new System.Windows.Forms.FormClosedEventHandler(this.frmMisTurnos_FormClosed);
            ((System.ComponentModel.ISupportInitialize)(this.dgvTurnos)).EndInit();
            this.ResumeLayout(false);
            this.PerformLayout();
        }

        private System.Windows.Forms.Label lblTitulo;
        private System.Windows.Forms.DataGridView dgvTurnos;
        private System.Windows.Forms.Button btnCancelarTurno;
        private System.Windows.Forms.Button btnCerrar;
    }
}