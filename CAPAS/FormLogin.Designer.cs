namespace UI
{
    partial class FormLogin
    {
        private System.ComponentModel.IContainer components = null;
        private System.Windows.Forms.Label lblUsuario;
        private System.Windows.Forms.Label lblContrasena;
        private System.Windows.Forms.TextBox txtUsuario;
        private System.Windows.Forms.TextBox txtContrasena;
        private System.Windows.Forms.Button btnIngresar;
        private System.Windows.Forms.Button btnCancelar;

        protected override void Dispose(bool disposing)
        {
            if (disposing && (components != null)) components.Dispose();
            base.Dispose(disposing);
        }

        private void InitializeComponent()
        {
            this.lblUsuario = new System.Windows.Forms.Label();
            this.lblContrasena = new System.Windows.Forms.Label();
            this.txtUsuario = new System.Windows.Forms.TextBox();
            this.txtContrasena = new System.Windows.Forms.TextBox();
            this.btnIngresar = new System.Windows.Forms.Button();
            this.btnCancelar = new System.Windows.Forms.Button();
            this.SuspendLayout();

            this.lblUsuario.Location = new System.Drawing.Point(30, 40);
            this.lblUsuario.Size = new System.Drawing.Size(80, 20);
            this.lblUsuario.Text = "Usuario:";

            this.txtUsuario.Location = new System.Drawing.Point(120, 37);
            this.txtUsuario.Width = 200;
            this.txtUsuario.Name = "txtUsuario";

            this.lblContrasena.Location = new System.Drawing.Point(30, 80);
            this.lblContrasena.Size = new System.Drawing.Size(80, 20);
            this.lblContrasena.Text = "Contraseña:";

            this.txtContrasena.Location = new System.Drawing.Point(120, 77);
            this.txtContrasena.Width = 200;
            this.txtContrasena.Name = "txtContrasena";
            this.txtContrasena.PasswordChar = '*';

            this.btnIngresar.Location = new System.Drawing.Point(80, 130);
            this.btnIngresar.Size = new System.Drawing.Size(90, 30);
            this.btnIngresar.Text = "Ingresar";
            this.btnIngresar.Name = "btnIngresar";
            this.btnIngresar.Click += new System.EventHandler(this.btnIngresar_Click);

            this.btnCancelar.Location = new System.Drawing.Point(200, 130);
            this.btnCancelar.Size = new System.Drawing.Size(90, 30);
            this.btnCancelar.Text = "Cancelar";
            this.btnCancelar.Name = "btnCancelar";
            this.btnCancelar.Click += new System.EventHandler(this.btnCancelar_Click);

            this.ClientSize = new System.Drawing.Size(380, 210);
            this.Controls.AddRange(new System.Windows.Forms.Control[] {
                lblUsuario, txtUsuario,
                lblContrasena, txtContrasena,
                btnIngresar, btnCancelar
            });
            this.Name = "FormLogin";
            this.Text = "Iniciar sesión";
            this.StartPosition = System.Windows.Forms.FormStartPosition.CenterScreen;
            this.ResumeLayout(false);
            this.PerformLayout();
        }
    }
}