namespace CAPAS
{
    partial class frmMisNotificaciones
    {
        private System.ComponentModel.IContainer components = new System.ComponentModel.Container();

        protected override void Dispose(bool disposing)
        {
            if (disposing && (components != null)) components.Dispose();
            base.Dispose(disposing);
        }

        private void InitializeComponent()
        {
            this.lblTitulo = new System.Windows.Forms.Label();
            this.dgvNotificaciones = new System.Windows.Forms.DataGridView();
            this.btnExportar = new System.Windows.Forms.Button();
            this.btnCerrar = new System.Windows.Forms.Button();
            this.timerRefresco = new System.Windows.Forms.Timer(this.components);
            ((System.ComponentModel.ISupportInitialize)(this.dgvNotificaciones)).BeginInit();
            this.SuspendLayout();
            //
            // lblTitulo
            //
            this.lblTitulo.AutoSize = true;
            this.lblTitulo.Font = new System.Drawing.Font("Segoe UI", 12F, System.Drawing.FontStyle.Bold);
            this.lblTitulo.Location = new System.Drawing.Point(30, 30);
            this.lblTitulo.Name = "lblTitulo";
            this.lblTitulo.Size = new System.Drawing.Size(180, 21);
            this.lblTitulo.TabIndex = 0;
            this.lblTitulo.Text = "Mis notificaciones";
            //
            // dgvNotificaciones
            //
            this.dgvNotificaciones.AllowUserToAddRows = false;
            this.dgvNotificaciones.AllowUserToDeleteRows = false;
            this.dgvNotificaciones.ReadOnly = true;
            this.dgvNotificaciones.SelectionMode = System.Windows.Forms.DataGridViewSelectionMode.FullRowSelect;
            this.dgvNotificaciones.AutoSizeColumnsMode = System.Windows.Forms.DataGridViewAutoSizeColumnsMode.Fill;
            this.dgvNotificaciones.Location = new System.Drawing.Point(30, 70);
            this.dgvNotificaciones.Name = "dgvNotificaciones";
            this.dgvNotificaciones.RowTemplate.Height = 25;
            this.dgvNotificaciones.Size = new System.Drawing.Size(600, 350);
            this.dgvNotificaciones.TabIndex = 1;
            //
            // btnExportar
            //
            this.btnExportar.Location = new System.Drawing.Point(395, 435);
            this.btnExportar.Name = "btnExportar";
            this.btnExportar.Size = new System.Drawing.Size(150, 32);
            this.btnExportar.TabIndex = 2;
            this.btnExportar.Text = "Exportar a Excel";
            this.btnExportar.UseVisualStyleBackColor = true;
            this.btnExportar.Click += new System.EventHandler(this.btnExportar_Click);
            //
            // btnCerrar
            //
            this.btnCerrar.Location = new System.Drawing.Point(555, 435);
            this.btnCerrar.Name = "btnCerrar";
            this.btnCerrar.Size = new System.Drawing.Size(75, 32);
            this.btnCerrar.TabIndex = 3;
            this.btnCerrar.Text = "Cerrar";
            this.btnCerrar.UseVisualStyleBackColor = true;
            this.btnCerrar.Click += new System.EventHandler(this.btnCerrar_Click);
            //
            // timerRefresco
            //
            this.timerRefresco.Interval = 4000;
            this.timerRefresco.Tick += new System.EventHandler(this.timerRefresco_Tick);
            //
            // frmMisNotificaciones
            //
            this.AutoScaleDimensions = new System.Drawing.SizeF(7F, 15F);
            this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
            this.ClientSize = new System.Drawing.Size(660, 490);
            this.Controls.Add(this.btnCerrar);
            this.Controls.Add(this.btnExportar);
            this.Controls.Add(this.dgvNotificaciones);
            this.Controls.Add(this.lblTitulo);
            this.MinimumSize = new System.Drawing.Size(676, 529);
            this.Name = "frmMisNotificaciones";
            this.StartPosition = System.Windows.Forms.FormStartPosition.CenterParent;
            this.Text = "frmMisNotificaciones";
            this.Load += new System.EventHandler(this.frmMisNotificaciones_Load);
            this.FormClosed += new System.Windows.Forms.FormClosedEventHandler(this.frmMisNotificaciones_FormClosed);
            ((System.ComponentModel.ISupportInitialize)(this.dgvNotificaciones)).EndInit();
            this.ResumeLayout(false);
            this.PerformLayout();
        }

        private System.Windows.Forms.Label lblTitulo;
        private System.Windows.Forms.DataGridView dgvNotificaciones;
        private System.Windows.Forms.Button btnExportar;
        private System.Windows.Forms.Button btnCerrar;
        private System.Windows.Forms.Timer timerRefresco;
    }
}