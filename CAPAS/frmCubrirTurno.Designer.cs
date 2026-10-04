namespace CAPAS
{
    partial class frmCubrirTurno
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
            this.btnTomarTurno = new System.Windows.Forms.Button();
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
            this.lblTitulo.Size = new System.Drawing.Size(280, 21);
            this.lblTitulo.TabIndex = 0;
            this.lblTitulo.Text = "Turnos disponibles para cubrir";
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
            // btnTomarTurno
            //
            this.btnTomarTurno.Location = new System.Drawing.Point(390, 440);
            this.btnTomarTurno.Name = "btnTomarTurno";
            this.btnTomarTurno.Size = new System.Drawing.Size(165, 32);
            this.btnTomarTurno.TabIndex = 2;
            this.btnTomarTurno.Text = "Tomar este turno";
            this.btnTomarTurno.UseVisualStyleBackColor = true;
            this.btnTomarTurno.Click += new System.EventHandler(this.btnTomarTurno_Click);
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
            // frmCubrirTurno
            //
            this.AutoScaleDimensions = new System.Drawing.SizeF(7F, 15F);
            this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
            this.ClientSize = new System.Drawing.Size(660, 500);
            this.Controls.Add(this.btnCerrar);
            this.Controls.Add(this.btnTomarTurno);
            this.Controls.Add(this.dgvTurnos);
            this.Controls.Add(this.lblTitulo);
            this.MinimumSize = new System.Drawing.Size(676, 539);
            this.Name = "frmCubrirTurno";
            this.StartPosition = System.Windows.Forms.FormStartPosition.CenterParent;
            this.Text = "Cubrir turno";
            this.Load += new System.EventHandler(this.frmCubrirTurno_Load);
            ((System.ComponentModel.ISupportInitialize)(this.dgvTurnos)).EndInit();
            this.ResumeLayout(false);
            this.PerformLayout();
        }

        private System.Windows.Forms.Label lblTitulo;
        private System.Windows.Forms.DataGridView dgvTurnos;
        private System.Windows.Forms.Button btnTomarTurno;
        private System.Windows.Forms.Button btnCerrar;
    }
}