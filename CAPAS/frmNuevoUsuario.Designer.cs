namespace CAPAS
{
    partial class frmNuevoUsuario
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
            this.lblNombrePersona = new System.Windows.Forms.Label();
            this.txtNombrePersona = new System.Windows.Forms.TextBox();
            this.lblApellido = new System.Windows.Forms.Label();
            this.txtApellido = new System.Windows.Forms.TextBox();
            this.lblNombre = new System.Windows.Forms.Label();
            this.txtNombre = new System.Windows.Forms.TextBox();
            this.lblContrasena = new System.Windows.Forms.Label();
            this.txtContrasena = new System.Windows.Forms.TextBox();
            this.lblRol = new System.Windows.Forms.Label();
            this.cboRol = new System.Windows.Forms.ComboBox();
            this.btnAceptar = new System.Windows.Forms.Button();
            this.btnCancelar = new System.Windows.Forms.Button();
            this.SuspendLayout();
            //
            // lblNombrePersona
            //
            this.lblNombrePersona.AutoSize = true;
            this.lblNombrePersona.Location = new System.Drawing.Point(20, 107);
            this.lblNombrePersona.Name = "lblNombrePersona";
            this.lblNombrePersona.Size = new System.Drawing.Size(68, 20);
            this.lblNombrePersona.TabIndex = 0;
            this.lblNombrePersona.Text = "Nombre:";
            //
            // txtNombrePersona
            //
            this.txtNombrePersona.Location = new System.Drawing.Point(130, 104);
            this.txtNombrePersona.Name = "txtNombrePersona";
            this.txtNombrePersona.Size = new System.Drawing.Size(200, 26);
            this.txtNombrePersona.TabIndex = 0;
            //
            // lblApellido
            //
            this.lblApellido.AutoSize = true;
            this.lblApellido.Location = new System.Drawing.Point(20, 147);
            this.lblApellido.Name = "lblApellido";
            this.lblApellido.Size = new System.Drawing.Size(68, 20);
            this.lblApellido.TabIndex = 1;
            this.lblApellido.Text = "Apellido:";
            //
            // txtApellido
            //
            this.txtApellido.Location = new System.Drawing.Point(130, 144);
            this.txtApellido.Name = "txtApellido";
            this.txtApellido.Size = new System.Drawing.Size(200, 26);
            this.txtApellido.TabIndex = 1;
            //
            // lblNombre
            //
            this.lblNombre.AutoSize = true;
            this.lblNombre.Location = new System.Drawing.Point(20, 187);
            this.lblNombre.Name = "lblNombre";
            this.lblNombre.Size = new System.Drawing.Size(68, 20);
            this.lblNombre.TabIndex = 2;
            this.lblNombre.Text = "Usuario:";
            //
            // txtNombre
            //
            this.txtNombre.Location = new System.Drawing.Point(130, 184);
            this.txtNombre.Name = "txtNombre";
            this.txtNombre.Size = new System.Drawing.Size(200, 26);
            this.txtNombre.TabIndex = 2;
            //
            // lblContrasena
            //
            this.lblContrasena.AutoSize = true;
            this.lblContrasena.Location = new System.Drawing.Point(20, 227);
            this.lblContrasena.Name = "lblContrasena";
            this.lblContrasena.Size = new System.Drawing.Size(96, 20);
            this.lblContrasena.TabIndex = 3;
            this.lblContrasena.Text = "Contraseña:";
            //
            // txtContrasena
            //
            this.txtContrasena.Location = new System.Drawing.Point(130, 224);
            this.txtContrasena.Name = "txtContrasena";
            this.txtContrasena.PasswordChar = '●';
            this.txtContrasena.Size = new System.Drawing.Size(200, 26);
            this.txtContrasena.TabIndex = 3;
            //
            // lblRol
            //
            this.lblRol.AutoSize = true;
            this.lblRol.Location = new System.Drawing.Point(20, 267);
            this.lblRol.Name = "lblRol";
            this.lblRol.Size = new System.Drawing.Size(37, 20);
            this.lblRol.TabIndex = 4;
            this.lblRol.Text = "Rol:";
            //
            // cboRol
            //
            this.cboRol.DropDownStyle = System.Windows.Forms.ComboBoxStyle.DropDownList;
            this.cboRol.Location = new System.Drawing.Point(130, 264);
            this.cboRol.Name = "cboRol";
            this.cboRol.Size = new System.Drawing.Size(200, 28);
            this.cboRol.TabIndex = 4;
            //
            // btnAceptar
            //
            this.btnAceptar.Location = new System.Drawing.Point(67, 322);
            this.btnAceptar.Name = "btnAceptar";
            this.btnAceptar.Size = new System.Drawing.Size(110, 48);
            this.btnAceptar.TabIndex = 5;
            this.btnAceptar.Text = "Crear";
            this.btnAceptar.Click += new System.EventHandler(this.btnAceptar_Click);
            //
            // btnCancelar
            //
            this.btnCancelar.Location = new System.Drawing.Point(195, 322);
            this.btnCancelar.Name = "btnCancelar";
            this.btnCancelar.Size = new System.Drawing.Size(110, 48);
            this.btnCancelar.TabIndex = 6;
            this.btnCancelar.Text = "Cancelar";
            this.btnCancelar.Click += new System.EventHandler(this.btnCancelar_Click);
            //
            // frmNuevoUsuario
            //
            this.AutoScaleDimensions = new System.Drawing.SizeF(9F, 20F);
            this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
            this.BackColor = System.Drawing.SystemColors.ActiveCaption;
            this.ClientSize = new System.Drawing.Size(392, 415);
            this.Controls.Add(this.lblNombrePersona);
            this.Controls.Add(this.txtNombrePersona);
            this.Controls.Add(this.lblApellido);
            this.Controls.Add(this.txtApellido);
            this.Controls.Add(this.lblNombre);
            this.Controls.Add(this.txtNombre);
            this.Controls.Add(this.lblContrasena);
            this.Controls.Add(this.txtContrasena);
            this.Controls.Add(this.lblRol);
            this.Controls.Add(this.cboRol);
            this.Controls.Add(this.btnAceptar);
            this.Controls.Add(this.btnCancelar);
            this.FormBorderStyle = System.Windows.Forms.FormBorderStyle.FixedDialog;
            this.MaximizeBox = false;
            this.MinimizeBox = false;
            this.Name = "frmNuevoUsuario";
            this.StartPosition = System.Windows.Forms.FormStartPosition.CenterParent;
            this.Text = "Nuevo usuario";
            this.FormClosed += new System.Windows.Forms.FormClosedEventHandler(this.frmNuevoUsuario_FormClosed);
            this.Load += new System.EventHandler(this.frmNuevoUsuario_Load);
            this.ResumeLayout(false);
            this.PerformLayout();

        }

        private System.Windows.Forms.Label    lblNombrePersona;
        private System.Windows.Forms.TextBox  txtNombrePersona;
        private System.Windows.Forms.Label    lblApellido;
        private System.Windows.Forms.TextBox  txtApellido;
        private System.Windows.Forms.Label    lblNombre;
        private System.Windows.Forms.TextBox  txtNombre;
        private System.Windows.Forms.Label    lblContrasena;
        private System.Windows.Forms.TextBox  txtContrasena;
        private System.Windows.Forms.Label    lblRol;
        private System.Windows.Forms.ComboBox cboRol;
        private System.Windows.Forms.Button   btnAceptar;
        private System.Windows.Forms.Button   btnCancelar;
    }
}
