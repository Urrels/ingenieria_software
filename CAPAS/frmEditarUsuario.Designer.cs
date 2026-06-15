namespace CAPAS
{
    partial class frmEditarUsuario
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
            this.btnAceptar = new System.Windows.Forms.Button();
            this.btnCancelar = new System.Windows.Forms.Button();
            this.SuspendLayout();
            //
            // lblNombrePersona
            //
            this.lblNombrePersona.AutoSize = true;
            this.lblNombrePersona.Location = new System.Drawing.Point(20, 90);
            this.lblNombrePersona.Name = "lblNombrePersona";
            this.lblNombrePersona.Size = new System.Drawing.Size(68, 20);
            this.lblNombrePersona.TabIndex = 0;
            this.lblNombrePersona.Text = "Nombre:";
            //
            // txtNombrePersona
            //
            this.txtNombrePersona.Location = new System.Drawing.Point(130, 87);
            this.txtNombrePersona.Name = "txtNombrePersona";
            this.txtNombrePersona.Size = new System.Drawing.Size(200, 26);
            this.txtNombrePersona.TabIndex = 0;
            //
            // lblApellido
            //
            this.lblApellido.AutoSize = true;
            this.lblApellido.Location = new System.Drawing.Point(20, 130);
            this.lblApellido.Name = "lblApellido";
            this.lblApellido.Size = new System.Drawing.Size(68, 20);
            this.lblApellido.TabIndex = 1;
            this.lblApellido.Text = "Apellido:";
            //
            // txtApellido
            //
            this.txtApellido.Location = new System.Drawing.Point(130, 127);
            this.txtApellido.Name = "txtApellido";
            this.txtApellido.Size = new System.Drawing.Size(200, 26);
            this.txtApellido.TabIndex = 1;
            //
            // btnAceptar
            //
            this.btnAceptar.Location = new System.Drawing.Point(67, 190);
            this.btnAceptar.Name = "btnAceptar";
            this.btnAceptar.Size = new System.Drawing.Size(110, 48);
            this.btnAceptar.TabIndex = 2;
            this.btnAceptar.Text = "Crear";
            this.btnAceptar.Click += new System.EventHandler(this.btnAceptar_Click);
            //
            // btnCancelar
            //
            this.btnCancelar.Location = new System.Drawing.Point(195, 190);
            this.btnCancelar.Name = "btnCancelar";
            this.btnCancelar.Size = new System.Drawing.Size(110, 48);
            this.btnCancelar.TabIndex = 3;
            this.btnCancelar.Text = "Cancelar";
            this.btnCancelar.Click += new System.EventHandler(this.btnCancelar_Click);
            //
            // frmEditarUsuario
            //
            this.AutoScaleDimensions = new System.Drawing.SizeF(9F, 20F);
            this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
            this.BackColor = System.Drawing.SystemColors.ActiveCaption;
            this.ClientSize = new System.Drawing.Size(392, 258);
            this.Controls.Add(this.lblNombrePersona);
            this.Controls.Add(this.txtNombrePersona);
            this.Controls.Add(this.lblApellido);
            this.Controls.Add(this.txtApellido);
            this.Controls.Add(this.btnAceptar);
            this.Controls.Add(this.btnCancelar);
            this.FormBorderStyle = System.Windows.Forms.FormBorderStyle.FixedDialog;
            this.MaximizeBox = false;
            this.MinimizeBox = false;
            this.Name = "frmEditarUsuario";
            this.StartPosition = System.Windows.Forms.FormStartPosition.CenterParent;
            this.Text = "Editar datos";
            this.FormClosed += new System.Windows.Forms.FormClosedEventHandler(this.frmEditarUsuario_FormClosed);
            this.Load += new System.EventHandler(this.frmEditarUsuario_Load);
            this.ResumeLayout(false);
            this.PerformLayout();

        }

        private System.Windows.Forms.Label    lblNombrePersona;
        private System.Windows.Forms.TextBox  txtNombrePersona;
        private System.Windows.Forms.Label    lblApellido;
        private System.Windows.Forms.TextBox  txtApellido;
        private System.Windows.Forms.Button   btnAceptar;
        private System.Windows.Forms.Button   btnCancelar;
    }
}
