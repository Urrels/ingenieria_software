namespace CAPAS
{
    partial class frmAsignarEmpleado
    {
        private System.ComponentModel.IContainer components = null;

        protected override void Dispose(bool disposing)
        {
            if (disposing && (components != null)) components.Dispose();
            base.Dispose(disposing);
        }

        private void InitializeComponent()
        {
            this.lblFranja = new System.Windows.Forms.Label();
            this.lblAviso = new System.Windows.Forms.Label();
            this.lstEmpleados = new System.Windows.Forms.ListBox();
            this.btnAsignar = new System.Windows.Forms.Button();
            this.btnSinCobertura = new System.Windows.Forms.Button();
            this.btnCancelar = new System.Windows.Forms.Button();
            this.SuspendLayout();

            this.lblFranja.AutoSize = true;
            this.lblFranja.Font = new System.Drawing.Font("Segoe UI", 11F, System.Drawing.FontStyle.Bold);
            this.lblFranja.Location = new System.Drawing.Point(25, 40);
            this.lblFranja.Name = "lblFranja";
            this.lblFranja.Size = new System.Drawing.Size(100, 20);
            this.lblFranja.TabIndex = 0;

            this.lblAviso.AutoSize = true;
            this.lblAviso.ForeColor = System.Drawing.Color.DarkOrange;
            this.lblAviso.Location = new System.Drawing.Point(25, 70);
            this.lblAviso.Name = "lblAviso";
            this.lblAviso.Size = new System.Drawing.Size(0, 15);
            this.lblAviso.TabIndex = 1;

            this.lstEmpleados.FormattingEnabled = true;
            this.lstEmpleados.ItemHeight = 20;
            this.lstEmpleados.Location = new System.Drawing.Point(25, 95);
            this.lstEmpleados.Name = "lstEmpleados";
            this.lstEmpleados.Size = new System.Drawing.Size(330, 224);
            this.lstEmpleados.TabIndex = 2;

            this.btnAsignar.Location = new System.Drawing.Point(25, 335);
            this.btnAsignar.Name = "btnAsignar";
            this.btnAsignar.Size = new System.Drawing.Size(105, 32);
            this.btnAsignar.TabIndex = 3;
            this.btnAsignar.Text = "Asignar";
            this.btnAsignar.UseVisualStyleBackColor = true;
            this.btnAsignar.Click += new System.EventHandler(this.btnAsignar_Click);

            this.btnSinCobertura.Location = new System.Drawing.Point(140, 335);
            this.btnSinCobertura.Name = "btnSinCobertura";
            this.btnSinCobertura.Size = new System.Drawing.Size(135, 32);
            this.btnSinCobertura.TabIndex = 4;
            this.btnSinCobertura.Text = "Sin cobertura";
            this.btnSinCobertura.UseVisualStyleBackColor = true;
            this.btnSinCobertura.Click += new System.EventHandler(this.btnSinCobertura_Click);

            this.btnCancelar.Location = new System.Drawing.Point(280, 335);
            this.btnCancelar.Name = "btnCancelar";
            this.btnCancelar.Size = new System.Drawing.Size(75, 32);
            this.btnCancelar.TabIndex = 5;
            this.btnCancelar.Text = "Cancelar";
            this.btnCancelar.UseVisualStyleBackColor = true;
            this.btnCancelar.Click += new System.EventHandler(this.btnCancelar_Click);

            this.AutoScaleDimensions = new System.Drawing.SizeF(7F, 15F);
            this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
            this.ClientSize = new System.Drawing.Size(384, 391);
            this.Controls.Add(this.btnCancelar);
            this.Controls.Add(this.btnSinCobertura);
            this.Controls.Add(this.btnAsignar);
            this.Controls.Add(this.lstEmpleados);
            this.Controls.Add(this.lblAviso);
            this.Controls.Add(this.lblFranja);
            this.FormBorderStyle = System.Windows.Forms.FormBorderStyle.FixedDialog;
            this.MaximizeBox = false;
            this.MinimizeBox = false;
            this.Name = "frmAsignarEmpleado";
            this.StartPosition = System.Windows.Forms.FormStartPosition.CenterParent;
            this.Text = "Asignar empleado";
            this.Load += new System.EventHandler(this.frmAsignarEmpleado_Load);
            this.ResumeLayout(false);
            this.PerformLayout();
        }

        private System.Windows.Forms.Label lblFranja;
        private System.Windows.Forms.Label lblAviso;
        private System.Windows.Forms.ListBox lstEmpleados;
        private System.Windows.Forms.Button btnAsignar;
        private System.Windows.Forms.Button btnSinCobertura;
        private System.Windows.Forms.Button btnCancelar;
    }
}