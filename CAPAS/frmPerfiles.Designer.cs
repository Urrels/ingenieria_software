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
            this.btnAgregarRol = new System.Windows.Forms.Button();
            this.panelPadre = new System.Windows.Forms.Panel();
            this.lblPadre = new System.Windows.Forms.Label();
            this.cboPadre = new System.Windows.Forms.ComboBox();
            this.btnAsignarPadre = new System.Windows.Forms.Button();
            this.panelPermisos = new System.Windows.Forms.Panel();
            this.lblPermisos = new System.Windows.Forms.Label();
            this.chkPermisos = new System.Windows.Forms.CheckedListBox();
            this.btnGuardarPermisos = new System.Windows.Forms.Button();
            this.btnEliminar = new System.Windows.Forms.Button();
            this.btnCerrar = new System.Windows.Forms.Button();
            this.panelDerecho.SuspendLayout();
            this.panelPadre.SuspendLayout();
            this.panelPermisos.SuspendLayout();
            this.SuspendLayout();
            this.treePermisos.Location = new System.Drawing.Point(12, 12);
            this.treePermisos.Name = "treePermisos";
            this.treePermisos.Size = new System.Drawing.Size(360, 462);
            this.treePermisos.TabIndex = 0;
            this.treePermisos.AfterSelect += new System.Windows.Forms.TreeViewEventHandler(this.treePermisos_AfterSelect);
            this.panelDerecho.Controls.Add(this.lblSeleccionado);
            this.panelDerecho.Controls.Add(this.btnAgregarRol);
            this.panelDerecho.Controls.Add(this.panelPadre);
            this.panelDerecho.Controls.Add(this.panelPermisos);
            this.panelDerecho.Controls.Add(this.btnEliminar);
            this.panelDerecho.Controls.Add(this.btnCerrar);
            this.panelDerecho.Location = new System.Drawing.Point(390, 12);
            this.panelDerecho.Name = "panelDerecho";
            this.panelDerecho.Size = new System.Drawing.Size(220, 508);
            this.panelDerecho.TabIndex = 1;
            this.lblSeleccionado.Location = new System.Drawing.Point(0, 10);
            this.lblSeleccionado.Name = "lblSeleccionado";
            this.lblSeleccionado.Size = new System.Drawing.Size(220, 40);
            this.lblSeleccionado.TabIndex = 0;
            this.lblSeleccionado.Text = "Seleccioná un nodo";
            this.lblSeleccionado.TextAlign = System.Drawing.ContentAlignment.MiddleCenter;
            this.btnAgregarRol.Location = new System.Drawing.Point(25, 60);
            this.btnAgregarRol.Name = "btnAgregarRol";
            this.btnAgregarRol.Size = new System.Drawing.Size(170, 35);
            this.btnAgregarRol.TabIndex = 1;
            this.btnAgregarRol.Text = "Agregar Rol";
            this.btnAgregarRol.Click += new System.EventHandler(this.btnAgregarRol_Click);
            this.panelPadre.Controls.Add(this.lblPadre);
            this.panelPadre.Controls.Add(this.cboPadre);
            this.panelPadre.Controls.Add(this.btnAsignarPadre);
            this.panelPadre.Location = new System.Drawing.Point(0, 100);
            this.panelPadre.Name = "panelPadre";
            this.panelPadre.Size = new System.Drawing.Size(220, 120);
            this.panelPadre.TabIndex = 2;
            this.panelPadre.Visible = false;
            this.lblPadre.Location = new System.Drawing.Point(5, 0);
            this.lblPadre.Name = "lblPadre";
            this.lblPadre.Size = new System.Drawing.Size(210, 20);
            this.lblPadre.TabIndex = 0;
            this.lblPadre.Text = "Rol padre:";
            this.cboPadre.DropDownStyle = System.Windows.Forms.ComboBoxStyle.DropDownList;
            this.cboPadre.Location = new System.Drawing.Point(4, 32);
            this.cboPadre.Name = "cboPadre";
            this.cboPadre.Size = new System.Drawing.Size(210, 28);
            this.cboPadre.TabIndex = 1;
            this.btnAsignarPadre.Location = new System.Drawing.Point(25, 66);
            this.btnAsignarPadre.Name = "btnAsignarPadre";
            this.btnAsignarPadre.Size = new System.Drawing.Size(170, 40);
            this.btnAsignarPadre.TabIndex = 2;
            this.btnAsignarPadre.Text = "Asignar padre";
            this.btnAsignarPadre.Click += new System.EventHandler(this.btnAsignarPadre_Click);
            this.panelPermisos.Controls.Add(this.lblPermisos);
            this.panelPermisos.Controls.Add(this.chkPermisos);
            this.panelPermisos.Controls.Add(this.btnGuardarPermisos);
            this.panelPermisos.Location = new System.Drawing.Point(0, 212);
            this.panelPermisos.Name = "panelPermisos";
            this.panelPermisos.Size = new System.Drawing.Size(220, 198);
            this.panelPermisos.TabIndex = 3;
            this.panelPermisos.Visible = false;
            this.lblPermisos.Location = new System.Drawing.Point(5, 11);
            this.lblPermisos.Name = "lblPermisos";
            this.lblPermisos.Size = new System.Drawing.Size(210, 22);
            this.lblPermisos.TabIndex = 0;
            this.lblPermisos.Text = "Permisos del rol:";
            this.chkPermisos.CheckOnClick = true;
            this.chkPermisos.FormattingEnabled = true;
            this.chkPermisos.Location = new System.Drawing.Point(7, 36);
            this.chkPermisos.Name = "chkPermisos";
            this.chkPermisos.Size = new System.Drawing.Size(210, 119);
            this.chkPermisos.TabIndex = 1;
            this.btnGuardarPermisos.Location = new System.Drawing.Point(25, 160);
            this.btnGuardarPermisos.Name = "btnGuardarPermisos";
            this.btnGuardarPermisos.Size = new System.Drawing.Size(170, 35);
            this.btnGuardarPermisos.TabIndex = 2;
            this.btnGuardarPermisos.Text = "Guardar permisos";
            this.btnGuardarPermisos.Click += new System.EventHandler(this.btnGuardarPermisos_Click);
            this.btnEliminar.Location = new System.Drawing.Point(25, 416);
            this.btnEliminar.Name = "btnEliminar";
            this.btnEliminar.Size = new System.Drawing.Size(170, 35);
            this.btnEliminar.TabIndex = 4;
            this.btnEliminar.Text = "Eliminar";
            this.btnEliminar.Click += new System.EventHandler(this.btnEliminar_Click);
            this.btnCerrar.Location = new System.Drawing.Point(25, 456);
            this.btnCerrar.Name = "btnCerrar";
            this.btnCerrar.Size = new System.Drawing.Size(170, 35);
            this.btnCerrar.TabIndex = 5;
            this.btnCerrar.Text = "Cerrar";
            this.btnCerrar.Click += new System.EventHandler(this.btnCerrar_Click);
            this.AutoScaleDimensions = new System.Drawing.SizeF(9F, 20F);
            this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
            this.BackColor = System.Drawing.SystemColors.ActiveCaption;
            this.ClientSize = new System.Drawing.Size(625, 532);
            this.Controls.Add(this.treePermisos);
            this.Controls.Add(this.panelDerecho);
            this.Name = "frmPerfiles";
            this.Text = "Gestión de Roles y Permisos";
            this.FormClosed += new System.Windows.Forms.FormClosedEventHandler(this.frmPerfiles_FormClosed);
            this.Load += new System.EventHandler(this.frmPerfiles_Load);
            this.panelDerecho.ResumeLayout(false);
            this.panelPadre.ResumeLayout(false);
            this.panelPermisos.ResumeLayout(false);
            this.ResumeLayout(false);

        }

        private System.Windows.Forms.TreeView treePermisos;
        private System.Windows.Forms.Panel panelDerecho;
        private System.Windows.Forms.Label lblSeleccionado;
        private System.Windows.Forms.Button btnAgregarRol;
        private System.Windows.Forms.Panel panelPadre;
        private System.Windows.Forms.Label lblPadre;
        private System.Windows.Forms.ComboBox cboPadre;
        private System.Windows.Forms.Button btnAsignarPadre;
        private System.Windows.Forms.Panel panelPermisos;
        private System.Windows.Forms.Label lblPermisos;
        private System.Windows.Forms.CheckedListBox chkPermisos;
        private System.Windows.Forms.Button btnGuardarPermisos;
        private System.Windows.Forms.Button btnEliminar;
        private System.Windows.Forms.Button btnCerrar;
    }
}
