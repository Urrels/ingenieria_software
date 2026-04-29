namespace CAPAS
{
    partial class LogIn
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
            this.lblUsuario = new System.Windows.Forms.Label();
            this.lblContrasena = new System.Windows.Forms.Label();
            this.txtUsuario = new System.Windows.Forms.TextBox();
            this.txtContrasena = new System.Windows.Forms.TextBox();
            this.btnIngresar = new System.Windows.Forms.Button();
            this.SuspendLayout();

            this.lblUsuario.Text = "Usuario";
            this.lblUsuario.Location = new System.Drawing.Point(50, 80);
            this.lblUsuario.AutoSize = true;

            this.lblContrasena.Text = "Contraseña";
            this.lblContrasena.Location = new System.Drawing.Point(50, 130);
            this.lblContrasena.AutoSize = true;

            this.txtUsuario.Location = new System.Drawing.Point(160, 75);
            this.txtUsuario.Size = new System.Drawing.Size(220, 26);
            this.txtUsuario.Name = "txtUsuario";

            this.txtContrasena.Location = new System.Drawing.Point(160, 125);
            this.txtContrasena.Size = new System.Drawing.Size(220, 26);
            this.txtContrasena.Name = "txtContrasena";
            this.txtContrasena.PasswordChar = '*';

            this.btnIngresar.Text = "Ingresar";
            this.btnIngresar.Location = new System.Drawing.Point(160, 180);
            this.btnIngresar.Size = new System.Drawing.Size(120, 40);
            this.btnIngresar.Click += new System.EventHandler(this.btnIngresar_Click);

            this.ClientSize = new System.Drawing.Size(450, 280);
            this.Controls.AddRange(new System.Windows.Forms.Control[] {
                this.lblUsuario, this.lblContrasena,
                this.txtUsuario, this.txtContrasena,
                this.btnIngresar
            });
            this.Text = "Iniciar Sesión";
            this.Name = "LogIn";
            this.StartPosition = System.Windows.Forms.FormStartPosition.CenterScreen;
            this.AcceptButton = this.btnIngresar; 
            this.ResumeLayout(false);
            this.PerformLayout();
        }

        private System.Windows.Forms.Label lblUsuario;
        private System.Windows.Forms.Label lblContrasena;
        private System.Windows.Forms.TextBox txtUsuario;
        private System.Windows.Forms.TextBox txtContrasena;
        private System.Windows.Forms.Button btnIngresar;
    }
}