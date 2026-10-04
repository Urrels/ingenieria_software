namespace CAPAS
{
    partial class frmEvaluarAutorizarVisita
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
            this.dgvAlertas = new System.Windows.Forms.DataGridView();
            this.btnAutorizar = new System.Windows.Forms.Button();
            this.btnDescartar = new System.Windows.Forms.Button();
            this.btnExportar = new System.Windows.Forms.Button();
            this.btnCerrar = new System.Windows.Forms.Button();
            ((System.ComponentModel.ISupportInitialize)(this.dgvAlertas)).BeginInit();
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
            this.lblTitulo.Text = "Evaluar y autorizar visita técnica";
            //
            // dgvAlertas
            //
            this.dgvAlertas.AllowUserToAddRows = false;
            this.dgvAlertas.AllowUserToDeleteRows = false;
            this.dgvAlertas.ReadOnly = true;
            this.dgvAlertas.SelectionMode = System.Windows.Forms.DataGridViewSelectionMode.FullRowSelect;
            this.dgvAlertas.AutoSizeColumnsMode = System.Windows.Forms.DataGridViewAutoSizeColumnsMode.Fill;
            this.dgvAlertas.Location = new System.Drawing.Point(30, 70);
            this.dgvAlertas.Name = "dgvAlertas";
            this.dgvAlertas.RowTemplate.Height = 25;
            this.dgvAlertas.Size = new System.Drawing.Size(600, 340);
            this.dgvAlertas.TabIndex = 1;
            //
            // btnAutorizar
            //
            this.btnAutorizar.Location = new System.Drawing.Point(30, 425);
            this.btnAutorizar.Name = "btnAutorizar";
            this.btnAutorizar.Size = new System.Drawing.Size(150, 32);
            this.btnAutorizar.TabIndex = 2;
            this.btnAutorizar.Text = "Autorizar visita";
            this.btnAutorizar.UseVisualStyleBackColor = true;
            this.btnAutorizar.Click += new System.EventHandler(this.btnAutorizar_Click);
            //
            // btnDescartar
            //
            this.btnDescartar.Location = new System.Drawing.Point(190, 425);
            this.btnDescartar.Name = "btnDescartar";
            this.btnDescartar.Size = new System.Drawing.Size(150, 32);
            this.btnDescartar.TabIndex = 3;
            this.btnDescartar.Text = "Descartar alerta";
            this.btnDescartar.UseVisualStyleBackColor = true;
            this.btnDescartar.Click += new System.EventHandler(this.btnDescartar_Click);
            //
            // btnExportar
            //
            this.btnExportar.Location = new System.Drawing.Point(395, 425);
            this.btnExportar.Name = "btnExportar";
            this.btnExportar.Size = new System.Drawing.Size(150, 32);
            this.btnExportar.TabIndex = 4;
            this.btnExportar.Text = "Exportar a Excel";
            this.btnExportar.UseVisualStyleBackColor = true;
            this.btnExportar.Click += new System.EventHandler(this.btnExportar_Click);
            //
            // btnCerrar
            //
            this.btnCerrar.Location = new System.Drawing.Point(555, 425);
            this.btnCerrar.Name = "btnCerrar";
            this.btnCerrar.Size = new System.Drawing.Size(75, 32);
            this.btnCerrar.TabIndex = 5;
            this.btnCerrar.Text = "Cerrar";
            this.btnCerrar.UseVisualStyleBackColor = true;
            this.btnCerrar.Click += new System.EventHandler(this.btnCerrar_Click);
            //
            // frmEvaluarAutorizarVisita
            //
            this.AutoScaleDimensions = new System.Drawing.SizeF(7F, 15F);
            this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
            this.ClientSize = new System.Drawing.Size(660, 490);
            this.Controls.Add(this.btnCerrar);
            this.Controls.Add(this.btnExportar);
            this.Controls.Add(this.btnDescartar);
            this.Controls.Add(this.btnAutorizar);
            this.Controls.Add(this.dgvAlertas);
            this.Controls.Add(this.lblTitulo);
            this.MinimumSize = new System.Drawing.Size(676, 529);
            this.Name = "frmEvaluarAutorizarVisita";
            this.StartPosition = System.Windows.Forms.FormStartPosition.CenterParent;
            this.Text = "Evaluar y autorizar visita";
            this.Load += new System.EventHandler(this.frmEvaluarAutorizarVisita_Load);
            ((System.ComponentModel.ISupportInitialize)(this.dgvAlertas)).EndInit();
            this.ResumeLayout(false);
            this.PerformLayout();
        }

        private System.Windows.Forms.Label lblTitulo;
        private System.Windows.Forms.DataGridView dgvAlertas;
        private System.Windows.Forms.Button btnAutorizar;
        private System.Windows.Forms.Button btnDescartar;
        private System.Windows.Forms.Button btnExportar;
        private System.Windows.Forms.Button btnCerrar;
    }
}