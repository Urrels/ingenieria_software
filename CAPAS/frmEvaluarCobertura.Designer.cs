namespace CAPAS
{
    partial class frmEvaluarCobertura
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
            this.lblSemana = new System.Windows.Forms.Label();
            this.dtpSemana = new System.Windows.Forms.DateTimePicker();
            this.btnEvaluar = new System.Windows.Forms.Button();
            this.dgvResultados = new System.Windows.Forms.DataGridView();
            this.btnExportar = new System.Windows.Forms.Button();
            this.btnCerrar = new System.Windows.Forms.Button();
            ((System.ComponentModel.ISupportInitialize)(this.dgvResultados)).BeginInit();
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
            this.lblTitulo.Text = "Evaluar cobertura del servicio";
            //
            // lblSemana
            //
            this.lblSemana.AutoSize = true;
            this.lblSemana.Location = new System.Drawing.Point(30, 70);
            this.lblSemana.Name = "lblSemana";
            this.lblSemana.Size = new System.Drawing.Size(100, 15);
            this.lblSemana.TabIndex = 1;
            this.lblSemana.Text = "Semana a evaluar:";
            //
            // dtpSemana
            //
            this.dtpSemana.Format = System.Windows.Forms.DateTimePickerFormat.Short;
            this.dtpSemana.Location = new System.Drawing.Point(160, 66);
            this.dtpSemana.Name = "dtpSemana";
            this.dtpSemana.Size = new System.Drawing.Size(150, 23);
            this.dtpSemana.TabIndex = 2;
            //
            // btnEvaluar
            //
            this.btnEvaluar.Location = new System.Drawing.Point(330, 64);
            this.btnEvaluar.Name = "btnEvaluar";
            this.btnEvaluar.Size = new System.Drawing.Size(150, 30);
            this.btnEvaluar.TabIndex = 3;
            this.btnEvaluar.Text = "Evaluar cobertura";
            this.btnEvaluar.UseVisualStyleBackColor = true;
            this.btnEvaluar.Click += new System.EventHandler(this.btnEvaluar_Click);
            //
            // dgvResultados
            //
            this.dgvResultados.AllowUserToAddRows = false;
            this.dgvResultados.AllowUserToDeleteRows = false;
            this.dgvResultados.ReadOnly = true;
            this.dgvResultados.SelectionMode = System.Windows.Forms.DataGridViewSelectionMode.FullRowSelect;
            this.dgvResultados.AutoSizeColumnsMode = System.Windows.Forms.DataGridViewAutoSizeColumnsMode.Fill;
            this.dgvResultados.Location = new System.Drawing.Point(30, 110);
            this.dgvResultados.Name = "dgvResultados";
            this.dgvResultados.RowTemplate.Height = 25;
            this.dgvResultados.Size = new System.Drawing.Size(700, 380);
            this.dgvResultados.TabIndex = 4;
            //
            // btnExportar
            //
            this.btnExportar.Location = new System.Drawing.Point(490, 505);
            this.btnExportar.Name = "btnExportar";
            this.btnExportar.Size = new System.Drawing.Size(150, 32);
            this.btnExportar.TabIndex = 5;
            this.btnExportar.Text = "Exportar a Excel";
            this.btnExportar.UseVisualStyleBackColor = true;
            this.btnExportar.Click += new System.EventHandler(this.btnExportar_Click);
            //
            // btnCerrar
            //
            this.btnCerrar.Location = new System.Drawing.Point(655, 505);
            this.btnCerrar.Name = "btnCerrar";
            this.btnCerrar.Size = new System.Drawing.Size(75, 32);
            this.btnCerrar.TabIndex = 6;
            this.btnCerrar.Text = "Cerrar";
            this.btnCerrar.UseVisualStyleBackColor = true;
            this.btnCerrar.Click += new System.EventHandler(this.btnCerrar_Click);
            //
            // frmEvaluarCobertura
            //
            this.AutoScaleDimensions = new System.Drawing.SizeF(7F, 15F);
            this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
            this.ClientSize = new System.Drawing.Size(760, 560);
            this.Controls.Add(this.btnCerrar);
            this.Controls.Add(this.btnExportar);
            this.Controls.Add(this.dgvResultados);
            this.Controls.Add(this.btnEvaluar);
            this.Controls.Add(this.dtpSemana);
            this.Controls.Add(this.lblSemana);
            this.Controls.Add(this.lblTitulo);
            this.MinimumSize = new System.Drawing.Size(776, 599);
            this.Name = "frmEvaluarCobertura";
            this.StartPosition = System.Windows.Forms.FormStartPosition.CenterParent;
            this.Text = "frmEvaluarCobertura";
            this.Load += new System.EventHandler(this.frmEvaluarCobertura_Load);
            this.FormClosed += new System.Windows.Forms.FormClosedEventHandler(this.frmEvaluarCobertura_FormClosed);
            ((System.ComponentModel.ISupportInitialize)(this.dgvResultados)).EndInit();
            this.ResumeLayout(false);
            this.PerformLayout();
        }

        private System.Windows.Forms.Label lblTitulo;
        private System.Windows.Forms.Label lblSemana;
        private System.Windows.Forms.DateTimePicker dtpSemana;
        private System.Windows.Forms.Button btnEvaluar;
        private System.Windows.Forms.DataGridView dgvResultados;
        private System.Windows.Forms.Button btnExportar;
        private System.Windows.Forms.Button btnCerrar;
    }
}