namespace CAPAS
{
    partial class frmDashboard
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
            this.pnlChartCobertura = new System.Windows.Forms.Panel();
            this.pnlChartEquipos = new System.Windows.Forms.Panel();
            this.lblFranjaAsistencia = new System.Windows.Forms.Label();
            this.cboFranjaAsistencia = new System.Windows.Forms.ComboBox();
            this.pnlChartAsistencia = new System.Windows.Forms.Panel();
            this.btnActualizar = new System.Windows.Forms.Button();
            this.btnCerrar = new System.Windows.Forms.Button();
            this.SuspendLayout();
            //
            // lblTitulo
            //
            this.lblTitulo.AutoSize = true;
            this.lblTitulo.Font = new System.Drawing.Font("Segoe UI", 12F, System.Drawing.FontStyle.Bold);
            this.lblTitulo.Location = new System.Drawing.Point(30, 20);
            this.lblTitulo.Name = "lblTitulo";
            this.lblTitulo.Size = new System.Drawing.Size(200, 21);
            this.lblTitulo.TabIndex = 0;
            this.lblTitulo.Text = "Panel de indicadores";
            //
            // pnlChartCobertura
            //
            this.pnlChartCobertura.BorderStyle = System.Windows.Forms.BorderStyle.FixedSingle;
            this.pnlChartCobertura.Location = new System.Drawing.Point(30, 55);
            this.pnlChartCobertura.Name = "pnlChartCobertura";
            this.pnlChartCobertura.Size = new System.Drawing.Size(860, 190);
            this.pnlChartCobertura.TabIndex = 1;
            //
            // pnlChartEquipos
            //
            this.pnlChartEquipos.BorderStyle = System.Windows.Forms.BorderStyle.FixedSingle;
            this.pnlChartEquipos.Location = new System.Drawing.Point(30, 255);
            this.pnlChartEquipos.Name = "pnlChartEquipos";
            this.pnlChartEquipos.Size = new System.Drawing.Size(860, 190);
            this.pnlChartEquipos.TabIndex = 2;
            //
            // lblFranjaAsistencia
            //
            this.lblFranjaAsistencia.AutoSize = true;
            this.lblFranjaAsistencia.Location = new System.Drawing.Point(30, 460);
            this.lblFranjaAsistencia.Name = "lblFranjaAsistencia";
            this.lblFranjaAsistencia.Size = new System.Drawing.Size(100, 15);
            this.lblFranjaAsistencia.TabIndex = 3;
            this.lblFranjaAsistencia.Text = "Franja a graficar:";
            //
            // cboFranjaAsistencia
            //
            this.cboFranjaAsistencia.DropDownStyle = System.Windows.Forms.ComboBoxStyle.DropDownList;
            this.cboFranjaAsistencia.FormattingEnabled = true;
            this.cboFranjaAsistencia.Location = new System.Drawing.Point(140, 456);
            this.cboFranjaAsistencia.Name = "cboFranjaAsistencia";
            this.cboFranjaAsistencia.Size = new System.Drawing.Size(300, 23);
            this.cboFranjaAsistencia.TabIndex = 4;
            this.cboFranjaAsistencia.SelectedIndexChanged += new System.EventHandler(this.cboFranjaAsistencia_SelectedIndexChanged);
            //
            // pnlChartAsistencia
            //
            this.pnlChartAsistencia.BorderStyle = System.Windows.Forms.BorderStyle.FixedSingle;
            this.pnlChartAsistencia.Location = new System.Drawing.Point(30, 490);
            this.pnlChartAsistencia.Name = "pnlChartAsistencia";
            this.pnlChartAsistencia.Size = new System.Drawing.Size(860, 190);
            this.pnlChartAsistencia.TabIndex = 5;
            //
            // btnActualizar
            //
            this.btnActualizar.Location = new System.Drawing.Point(30, 695);
            this.btnActualizar.Name = "btnActualizar";
            this.btnActualizar.Size = new System.Drawing.Size(150, 32);
            this.btnActualizar.TabIndex = 6;
            this.btnActualizar.Text = "Actualizar";
            this.btnActualizar.UseVisualStyleBackColor = true;
            this.btnActualizar.Click += new System.EventHandler(this.btnActualizar_Click);
            //
            // btnCerrar
            //
            this.btnCerrar.Location = new System.Drawing.Point(815, 695);
            this.btnCerrar.Name = "btnCerrar";
            this.btnCerrar.Size = new System.Drawing.Size(75, 32);
            this.btnCerrar.TabIndex = 7;
            this.btnCerrar.Text = "Cerrar";
            this.btnCerrar.UseVisualStyleBackColor = true;
            this.btnCerrar.Click += new System.EventHandler(this.btnCerrar_Click);
            //
            // frmDashboard
            //
            this.AutoScaleDimensions = new System.Drawing.SizeF(7F, 15F);
            this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
            this.ClientSize = new System.Drawing.Size(920, 750);
            this.Controls.Add(this.btnCerrar);
            this.Controls.Add(this.btnActualizar);
            this.Controls.Add(this.pnlChartAsistencia);
            this.Controls.Add(this.cboFranjaAsistencia);
            this.Controls.Add(this.lblFranjaAsistencia);
            this.Controls.Add(this.pnlChartEquipos);
            this.Controls.Add(this.pnlChartCobertura);
            this.Controls.Add(this.lblTitulo);
            this.MinimumSize = new System.Drawing.Size(936, 789);
            this.Name = "frmDashboard";
            this.StartPosition = System.Windows.Forms.FormStartPosition.CenterParent;
            this.Text = "Panel de indicadores";
            this.Load += new System.EventHandler(this.frmDashboard_Load);
            this.ResumeLayout(false);
            this.PerformLayout();
        }

        private System.Windows.Forms.Label lblTitulo;
        private System.Windows.Forms.Panel pnlChartCobertura;
        private System.Windows.Forms.Panel pnlChartEquipos;
        private System.Windows.Forms.Label lblFranjaAsistencia;
        private System.Windows.Forms.ComboBox cboFranjaAsistencia;
        private System.Windows.Forms.Panel pnlChartAsistencia;
        private System.Windows.Forms.Button btnActualizar;
        private System.Windows.Forms.Button btnCerrar;
    }
}