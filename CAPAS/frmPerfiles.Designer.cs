namespace CAPAS
{
    partial class frmPerfiles
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
            this.treePermisos = new System.Windows.Forms.TreeView();
            this.panelDerecho = new System.Windows.Forms.Panel();
            this.lblSeleccionado = new System.Windows.Forms.Label();
            this.btnAgregarPerfil = new System.Windows.Forms.Button();
            this.btnAgregarPermiso = new System.Windows.Forms.Button();
            this.btnEliminar = new System.Windows.Forms.Button();
            this.btnCerrar = new System.Windows.Forms.Button();
            this.panelDerecho.SuspendLayout();
            this.SuspendLayout();
            //
            // treePermisos
            //
            this.treePermisos.Location = new System.Drawing.Point(12, 12);
            this.treePermisos.Name = "treePermisos";
            this.treePermisos.Size = new System.Drawing.Size(360, 426);
            this.treePermisos.TabIndex = 0;
            this.treePermisos.AfterSelect += new System.Windows.Forms.TreeViewEventHandler(this.treePermisos_AfterSelect);
            //
            // panelDerecho
            //
            this.panelDerecho.Controls.Add(this.lblSeleccionado);
            this.panelDerecho.Controls.Add(this.btnAgregarPerfil);
            this.panelDerecho.Controls.Add(this.btnAgregarPermiso);
            this.panelDerecho.Controls.Add(this.btnEliminar);
            this.panelDerecho.Controls.Add(this.btnCerrar);
            this.panelDerecho.Location = new System.Drawing.Point(390, 12);
            this.panelDerecho.Name = "panelDerecho";
            this.panelDerecho.Size = new System.Drawing.Size(200, 426);
            this.panelDerecho.TabIndex = 1;
            //
            // lblSeleccionado
            //
            this.lblSeleccionado.AutoSize = false;
            this.lblSeleccionado.Location = new System.Drawing.Point(0, 10);
            this.lblSeleccionado.Name = "lblSeleccionado";
            this.lblSeleccionado.Size = new System.Drawing.Size(200, 50);
            this.lblSeleccionado.TabIndex = 0;
            this.lblSeleccionado.Text = "Seleccioná un nodo";
            this.lblSeleccionado.TextAlign = System.Drawing.ContentAlignment.MiddleCenter;
            //
            // btnAgregarPerfil
            //
            this.btnAgregarPerfil.Location = new System.Drawing.Point(25, 80);
            this.btnAgregarPerfil.Name = "btnAgregarPerfil";
            this.btnAgregarPerfil.Size = new System.Drawing.Size(150, 40);
            this.btnAgregarPerfil.TabIndex = 1;
            this.btnAgregarPerfil.Text = "Agregar Perfil";
            this.btnAgregarPerfil.Click += new System.EventHandler(this.btnAgregarPerfil_Click);
            //
            // btnAgregarPermiso
            //
            this.btnAgregarPermiso.Enabled = false;
            this.btnAgregarPermiso.Location = new System.Drawing.Point(25, 140);
            this.btnAgregarPermiso.Name = "btnAgregarPermiso";
            this.btnAgregarPermiso.Size = new System.Drawing.Size(150, 40);
            this.btnAgregarPermiso.TabIndex = 2;
            this.btnAgregarPermiso.Text = "Agregar Permiso";
            this.btnAgregarPermiso.Click += new System.EventHandler(this.btnAgregarPermiso_Click);
            //
            // btnEliminar
            //
            this.btnEliminar.Location = new System.Drawing.Point(25, 200);
            this.btnEliminar.Name = "btnEliminar";
            this.btnEliminar.Size = new System.Drawing.Size(150, 40);
            this.btnEliminar.TabIndex = 3;
            this.btnEliminar.Text = "Eliminar";
            this.btnEliminar.Click += new System.EventHandler(this.btnEliminar_Click);
            //
            // btnCerrar
            //
            this.btnCerrar.Location = new System.Drawing.Point(25, 375);
            this.btnCerrar.Name = "btnCerrar";
            this.btnCerrar.Size = new System.Drawing.Size(150, 40);
            this.btnCerrar.TabIndex = 4;
            this.btnCerrar.Text = "Cerrar";
            this.btnCerrar.Click += new System.EventHandler(this.btnCerrar_Click);
            //
            // frmPerfiles
            //
            this.AutoScaleDimensions = new System.Drawing.SizeF(9F, 20F);
            this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
            this.BackColor = System.Drawing.SystemColors.ActiveCaption;
            this.ClientSize = new System.Drawing.Size(606, 450);
            this.Controls.Add(this.treePermisos);
            this.Controls.Add(this.panelDerecho);
            this.Name = "frmPerfiles";
            this.Text = "Gestión de Perfiles y Permisos";
            this.Load += new System.EventHandler(this.frmPerfiles_Load);
            this.FormClosed += new System.Windows.Forms.FormClosedEventHandler(this.frmPerfiles_FormClosed);
            this.panelDerecho.ResumeLayout(false);
            this.ResumeLayout(false);
        }

        private System.Windows.Forms.TreeView treePermisos;
        private System.Windows.Forms.Panel panelDerecho;
        private System.Windows.Forms.Label lblSeleccionado;
        private System.Windows.Forms.Button btnAgregarPerfil;
        private System.Windows.Forms.Button btnAgregarPermiso;
        private System.Windows.Forms.Button btnEliminar;
        private System.Windows.Forms.Button btnCerrar;
    }
}
