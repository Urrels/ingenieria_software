namespace CAPAS
{
    partial class frmConfirmarCierreMantenimiento
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
            this.dgvInformes = new System.Windows.Forms.DataGridView();
            this.lblDetalle = new System.Windows.Forms.Label();
            this.btnConfirmarCierre = new System.Windows.Forms.Button();
            this.btnCerrar = new System.Windows.Forms.Button();
            ((System.ComponentModel.ISupportInitialize)(this.dgvInformes)).BeginInit();
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
            this.lblTitulo.Text = "Registrar cierre de mantenimiento";
            //
            // dgvInformes
            //
            this.dgvInformes.AllowUserToAddRows = false;
            this.dgvInformes.AllowUserToDeleteRows = false;
            this.dgvInformes.ReadOnly = true;
            this.dgvInformes.SelectionMode = System.Windows.Forms.DataGridViewSelectionMode.FullRowSelect;
            this.dgvInformes.AutoSizeColumnsMode = System.Windows.Forms.DataGridViewAutoSizeColumnsMode.Fill;
            this.dgvInformes.Location = new System.Drawing.Point(30, 70);
            this.dgvInformes.Name = "dgvInformes";
            this.dgvInformes.RowTemplate.Height = 25;
            this.dgvInformes.Size = new System.Drawing.Size(600, 320);
            this.dgvInformes.TabIndex = 1;
            this.dgvInformes.SelectionChanged += new System.EventHandler(this.dgvInformes_SelectionChanged);
            //
            // lblDetalle
            //
            this.lblDetalle.AutoSize = true;
            this.lblDetalle.Location = new System.Drawing.Point(30, 405);
            this.lblDetalle.Name = "lblDetalle";
            this.lblDetalle.Size = new System.Drawing.Size(0, 15);
            this.lblDetalle.TabIndex = 2;
            //
            // btnConfirmarCierre
            //
            this.btnConfirmarCierre.Location = new System.Drawing.Point(30, 440);
            this.btnConfirmarCierre.Name = "btnConfirmarCierre";
            this.btnConfirmarCierre.Size = new System.Drawing.Size(180, 32);
            this.btnConfirmarCierre.TabIndex = 3;
            this.btnConfirmarCierre.Text = "Confirmar cierre";
            this.btnConfirmarCierre.UseVisualStyleBackColor = true;
            this.btnConfirmarCierre.Click += new System.EventHandler(this.btnConfirmarCierre_Click);
            //
            // btnCerrar
            //
            this.btnCerrar.Location = new System.Drawing.Point(555, 440);
            this.btnCerrar.Name = "btnCerrar";
            this.btnCerrar.Size = new System.Drawing.Size(75, 32);
            this.btnCerrar.TabIndex = 4;
            this.btnCerrar.Text = "Cerrar";
            this.btnCerrar.UseVisualStyleBackColor = true;
            this.btnCerrar.Click += new System.EventHandler(this.btnCerrar_Click);
            //
            // frmConfirmarCierreMantenimiento
            //
            this.AutoScaleDimensions = new System.Drawing.SizeF(7F, 15F);
            this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
            this.ClientSize = new System.Drawing.Size(660, 490);
            this.Controls.Add(this.btnCerrar);
            this.Controls.Add(this.btnConfirmarCierre);
            this.Controls.Add(this.lblDetalle);
            this.Controls.Add(this.dgvInformes);
            this.Controls.Add(this.lblTitulo);
            this.MinimumSize = new System.Drawing.Size(676, 529);
            this.Name = "frmConfirmarCierreMantenimiento";
            this.StartPosition = System.Windows.Forms.FormStartPosition.CenterParent;
            this.Text = "frmConfirmarCierreMantenimiento";
            this.Load += new System.EventHandler(this.frmConfirmarCierreMantenimiento_Load);
            ((System.ComponentModel.ISupportInitialize)(this.dgvInformes)).EndInit();
            this.ResumeLayout(false);
            this.PerformLayout();
        }

        private System.Windows.Forms.Label lblTitulo;
        private System.Windows.Forms.DataGridView dgvInformes;
        private System.Windows.Forms.Label lblDetalle;
        private System.Windows.Forms.Button btnConfirmarCierre;
        private System.Windows.Forms.Button btnCerrar;
    }
}