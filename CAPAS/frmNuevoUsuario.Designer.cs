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
            this.lblNombre     = new System.Windows.Forms.Label();
            this.txtNombre     = new System.Windows.Forms.TextBox();
            this.lblContrasena = new System.Windows.Forms.Label();
            this.txtContrasena = new System.Windows.Forms.TextBox();
            this.lblRol        = new System.Windows.Forms.Label();
            this.cboRol        = new System.Windows.Forms.ComboBox();
            this.btnAceptar    = new System.Windows.Forms.Button();
            this.btnCancelar   = new System.Windows.Forms.Button();
            this.SuspendLayout();
            this.lblNombre.AutoSize = true;
            this.lblNombre.Location = new System.Drawing.Point(20, 25);
            this.lblNombre.Text     = "Usuario:";
            this.txtNombre.Location = new System.Drawing.Point(130, 22);
            this.txtNombre.Size     = new System.Drawing.Size(200, 26);
            this.txtNombre.TabIndex = 0;
            this.lblContrasena.AutoSize = true;
            this.lblContrasena.Location = new System.Drawing.Point(20, 65);
            this.lblContrasena.Text     = "Contraseña:";
            this.txtContrasena.Location     = new System.Drawing.Point(130, 62);
            this.txtContrasena.Size         = new System.Drawing.Size(200, 26);
            this.txtContrasena.PasswordChar = '●';
            this.txtContrasena.TabIndex     = 1;
            this.lblRol.AutoSize = true;
            this.lblRol.Location = new System.Drawing.Point(20, 105);
            this.lblRol.Text     = "Rol:";
            this.cboRol.DropDownStyle = System.Windows.Forms.ComboBoxStyle.DropDownList;
            this.cboRol.Location      = new System.Drawing.Point(130, 102);
            this.cboRol.Size          = new System.Drawing.Size(200, 28);
            this.cboRol.TabIndex      = 2;
            this.btnAceptar.Location = new System.Drawing.Point(70, 155);
            this.btnAceptar.Size     = new System.Drawing.Size(110, 34);
            this.btnAceptar.TabIndex = 3;
            this.btnAceptar.Text     = "Crear";
            this.btnAceptar.Click   += new System.EventHandler(this.btnAceptar_Click);
            this.btnCancelar.Location = new System.Drawing.Point(195, 155);
            this.btnCancelar.Size     = new System.Drawing.Size(110, 34);
            this.btnCancelar.TabIndex = 4;
            this.btnCancelar.Text     = "Cancelar";
            this.btnCancelar.Click   += new System.EventHandler(this.btnCancelar_Click);
            this.AutoScaleDimensions = new System.Drawing.SizeF(9F, 20F);
            this.AutoScaleMode       = System.Windows.Forms.AutoScaleMode.Font;
            this.BackColor           = System.Drawing.SystemColors.ActiveCaption;
            this.ClientSize          = new System.Drawing.Size(370, 215);
            this.Controls.Add(this.lblNombre);
            this.Controls.Add(this.txtNombre);
            this.Controls.Add(this.lblContrasena);
            this.Controls.Add(this.txtContrasena);
            this.Controls.Add(this.lblRol);
            this.Controls.Add(this.cboRol);
            this.Controls.Add(this.btnAceptar);
            this.Controls.Add(this.btnCancelar);
            this.FormBorderStyle     = System.Windows.Forms.FormBorderStyle.FixedDialog;
            this.MaximizeBox         = false;
            this.MinimizeBox         = false;
            this.StartPosition       = System.Windows.Forms.FormStartPosition.CenterParent;
            this.Text                = "Nuevo usuario";
            this.Load               += new System.EventHandler(this.frmNuevoUsuario_Load);
            this.FormClosed         += new System.Windows.Forms.FormClosedEventHandler(this.frmNuevoUsuario_FormClosed);
            this.ResumeLayout(false);
            this.PerformLayout();
        }

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
