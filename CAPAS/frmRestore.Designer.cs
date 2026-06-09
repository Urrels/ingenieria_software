namespace CAPAS
{
    partial class frmRestore
    {
        private System.ComponentModel.IContainer components = null;

        protected override void Dispose(bool disposing)
        {
            if (disposing && (components != null))
                components.Dispose();
            base.Dispose(disposing);
        }

        private void InitializeComponent()
        {
            this.lblTitulo = new System.Windows.Forms.Label();
            this.lblUltimoBackup = new System.Windows.Forms.Label();
            this.lblProblemas = new System.Windows.Forms.Label();
            this.lblPassword = new System.Windows.Forms.Label();
            this.txtPassword = new System.Windows.Forms.TextBox();
            this.btnRestaurar = new System.Windows.Forms.Button();
            this.btnCancelar = new System.Windows.Forms.Button();
            this.SuspendLayout();

            // lblTitulo
            this.lblTitulo.AutoSize = false;
            this.lblTitulo.Font = new System.Drawing.Font("Segoe UI", 13F, System.Drawing.FontStyle.Bold);
            this.lblTitulo.ForeColor = System.Drawing.Color.DarkRed;
            this.lblTitulo.Location = new System.Drawing.Point(12, 20);
            this.lblTitulo.Size = new System.Drawing.Size(460, 30);
            this.lblTitulo.Text = "⚠ Restauración del Sistema";
            this.lblTitulo.TextAlign = System.Drawing.ContentAlignment.MiddleCenter;

            // lblUltimoBackup
            this.lblUltimoBackup.AutoSize = false;
            this.lblUltimoBackup.Font = new System.Drawing.Font("Segoe UI", 9F);
            this.lblUltimoBackup.ForeColor = System.Drawing.Color.DimGray;
            this.lblUltimoBackup.Location = new System.Drawing.Point(12, 60);
            this.lblUltimoBackup.Size = new System.Drawing.Size(460, 20);
            this.lblUltimoBackup.Text = "Último backup: (cargando...)";
            this.lblUltimoBackup.TextAlign = System.Drawing.ContentAlignment.MiddleCenter;

            // lblProblemas
            this.lblProblemas.AutoSize = false;
            this.lblProblemas.Font = new System.Drawing.Font("Segoe UI", 9F);
            this.lblProblemas.ForeColor = System.Drawing.Color.DarkRed;
            this.lblProblemas.Location = new System.Drawing.Point(12, 90);
            this.lblProblemas.Size = new System.Drawing.Size(460, 60);
            this.lblProblemas.Text = "";

            // lblPassword
            this.lblPassword.AutoSize = true;
            this.lblPassword.Font = new System.Drawing.Font("Segoe UI", 9F);
            this.lblPassword.Location = new System.Drawing.Point(12, 162);
            this.lblPassword.Text = "Contraseña maestra:";

            // txtPassword
            this.txtPassword.Location = new System.Drawing.Point(12, 182);
            this.txtPassword.Size = new System.Drawing.Size(460, 23);
            this.txtPassword.PasswordChar = '*';
            this.txtPassword.Font = new System.Drawing.Font("Segoe UI", 10F);

            // btnRestaurar
            this.btnRestaurar.Location = new System.Drawing.Point(12, 225);
            this.btnRestaurar.Size = new System.Drawing.Size(220, 40);
            this.btnRestaurar.Text = "Restaurar";
            this.btnRestaurar.Font = new System.Drawing.Font("Segoe UI", 10F, System.Drawing.FontStyle.Bold);
            this.btnRestaurar.BackColor = System.Drawing.Color.DarkRed;
            this.btnRestaurar.ForeColor = System.Drawing.Color.White;
            this.btnRestaurar.FlatStyle = System.Windows.Forms.FlatStyle.Flat;
            this.btnRestaurar.Click += new System.EventHandler(this.btnRestaurar_Click);

            // btnCancelar
            this.btnCancelar.Location = new System.Drawing.Point(252, 225);
            this.btnCancelar.Size = new System.Drawing.Size(220, 40);
            this.btnCancelar.Text = "Cancelar";
            this.btnCancelar.Font = new System.Drawing.Font("Segoe UI", 10F);
            this.btnCancelar.FlatStyle = System.Windows.Forms.FlatStyle.Flat;
            this.btnCancelar.Click += new System.EventHandler(this.btnCancelar_Click);

            // frmRestore
            this.AutoScaleDimensions = new System.Drawing.SizeF(7F, 15F);
            this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
            this.ClientSize = new System.Drawing.Size(484, 285);
            this.Controls.Add(this.lblTitulo);
            this.Controls.Add(this.lblUltimoBackup);
            this.Controls.Add(this.lblProblemas);
            this.Controls.Add(this.lblPassword);
            this.Controls.Add(this.txtPassword);
            this.Controls.Add(this.btnRestaurar);
            this.Controls.Add(this.btnCancelar);
            this.FormBorderStyle = System.Windows.Forms.FormBorderStyle.FixedDialog;
            this.MaximizeBox = false;
            this.MinimizeBox = false;
            this.Name = "frmRestore";
            this.StartPosition = System.Windows.Forms.FormStartPosition.CenterScreen;
            this.Text = "Restauración del Sistema";
            this.Load += new System.EventHandler(this.frmRestore_Load);
            this.ResumeLayout(false);
            this.PerformLayout();
        }

        private System.Windows.Forms.Label lblTitulo;
        private System.Windows.Forms.Label lblUltimoBackup;
        private System.Windows.Forms.Label lblProblemas;
        private System.Windows.Forms.Label lblPassword;
        private System.Windows.Forms.TextBox txtPassword;
        private System.Windows.Forms.Button btnRestaurar;
        private System.Windows.Forms.Button btnCancelar;
    }
}