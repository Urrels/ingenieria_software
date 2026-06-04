namespace CAPAS
{
    partial class frmIdiomas
    {
        private System.ComponentModel.IContainer components = null;

        protected override void Dispose(bool disposing)
        {
            if (disposing && (components != null)) components.Dispose();
            base.Dispose(disposing);
        }

        private void InitializeComponent()
        {
            this.lblIdiomas = new System.Windows.Forms.Label();
            this.dgvIdiomas = new System.Windows.Forms.DataGridView();
            this.btnAgregarIdioma = new System.Windows.Forms.Button();
            this.btnRenombrar = new System.Windows.Forms.Button();
            this.btnToggleHabilitado = new System.Windows.Forms.Button();
            this.btnEliminarIdioma = new System.Windows.Forms.Button();
            this.lblTraducciones = new System.Windows.Forms.Label();
            this.dgvTraducciones = new System.Windows.Forms.DataGridView();
            this.btnGuardarTraducciones = new System.Windows.Forms.Button();
            this.btnCerrar = new System.Windows.Forms.Button();
            ((System.ComponentModel.ISupportInitialize)(this.dgvIdiomas)).BeginInit();
            ((System.ComponentModel.ISupportInitialize)(this.dgvTraducciones)).BeginInit();
            this.SuspendLayout();
            //
            // lblIdiomas
            //
            this.lblIdiomas.AutoSize = true;
            this.lblIdiomas.Font = new System.Drawing.Font("Microsoft Sans Serif", 10F, System.Drawing.FontStyle.Bold);
            this.lblIdiomas.Location = new System.Drawing.Point(12, 12);
            this.lblIdiomas.Name = "lblIdiomas";
            this.lblIdiomas.Text = "Idiomas";
            //
            // dgvIdiomas
            //
            this.dgvIdiomas.AllowUserToAddRows = false;
            this.dgvIdiomas.AllowUserToDeleteRows = false;
            this.dgvIdiomas.ColumnHeadersHeightSizeMode = System.Windows.Forms.DataGridViewColumnHeadersHeightSizeMode.AutoSize;
            this.dgvIdiomas.Location = new System.Drawing.Point(12, 42);
            this.dgvIdiomas.MultiSelect = false;
            this.dgvIdiomas.Name = "dgvIdiomas";
            this.dgvIdiomas.ReadOnly = true;
            this.dgvIdiomas.SelectionMode = System.Windows.Forms.DataGridViewSelectionMode.FullRowSelect;
            this.dgvIdiomas.Size = new System.Drawing.Size(270, 400);
            this.dgvIdiomas.TabIndex = 0;
            this.dgvIdiomas.SelectionChanged += new System.EventHandler(this.dgvIdiomas_SelectionChanged);
            //
            // btnAgregarIdioma
            //
            this.btnAgregarIdioma.Location = new System.Drawing.Point(12, 455);
            this.btnAgregarIdioma.Name = "btnAgregarIdioma";
            this.btnAgregarIdioma.Size = new System.Drawing.Size(128, 36);
            this.btnAgregarIdioma.Text = "Agregar idioma";
            this.btnAgregarIdioma.Click += new System.EventHandler(this.btnAgregarIdioma_Click);
            //
            // btnEliminarIdioma
            //
            this.btnEliminarIdioma.Location = new System.Drawing.Point(12, 541);
            this.btnEliminarIdioma.Name = "btnEliminarIdioma";
            this.btnEliminarIdioma.Size = new System.Drawing.Size(270, 36);
            this.btnEliminarIdioma.Text = "Eliminar idioma";
            this.btnEliminarIdioma.Click += new System.EventHandler(this.btnEliminarIdioma_Click);
            //
            // btnRenombrar
            //
            this.btnRenombrar.Location = new System.Drawing.Point(12, 498);
            this.btnRenombrar.Name = "btnRenombrar";
            this.btnRenombrar.Size = new System.Drawing.Size(270, 36);
            this.btnRenombrar.Text = "Renombrar idioma";
            this.btnRenombrar.Click += new System.EventHandler(this.btnRenombrar_Click);
            //
            // btnToggleHabilitado
            //
            this.btnToggleHabilitado.Location = new System.Drawing.Point(152, 455);
            this.btnToggleHabilitado.Name = "btnToggleHabilitado";
            this.btnToggleHabilitado.Size = new System.Drawing.Size(130, 36);
            this.btnToggleHabilitado.Text = "Habilitar/Deshabilitar";
            this.btnToggleHabilitado.Click += new System.EventHandler(this.btnToggleHabilitado_Click);
            //
            // lblTraducciones
            //
            this.lblTraducciones.AutoSize = true;
            this.lblTraducciones.Font = new System.Drawing.Font("Microsoft Sans Serif", 10F, System.Drawing.FontStyle.Bold);
            this.lblTraducciones.Location = new System.Drawing.Point(300, 12);
            this.lblTraducciones.Name = "lblTraducciones";
            this.lblTraducciones.Text = "Traducciones del idioma seleccionado";
            //
            // dgvTraducciones
            //
            this.dgvTraducciones.ColumnHeadersHeightSizeMode = System.Windows.Forms.DataGridViewColumnHeadersHeightSizeMode.AutoSize;
            this.dgvTraducciones.Location = new System.Drawing.Point(300, 42);
            this.dgvTraducciones.Name = "dgvTraducciones";
            this.dgvTraducciones.SelectionMode = System.Windows.Forms.DataGridViewSelectionMode.FullRowSelect;
            this.dgvTraducciones.Size = new System.Drawing.Size(610, 400);
            this.dgvTraducciones.TabIndex = 1;
            //
            // btnGuardarTraducciones
            //
            this.btnGuardarTraducciones.Location = new System.Drawing.Point(300, 455);
            this.btnGuardarTraducciones.Name = "btnGuardarTraducciones";
            this.btnGuardarTraducciones.Size = new System.Drawing.Size(200, 36);
            this.btnGuardarTraducciones.Text = "Guardar traducciones";
            this.btnGuardarTraducciones.Click += new System.EventHandler(this.btnGuardarTraducciones_Click);
            //
            // btnCerrar
            //
            this.btnCerrar.Location = new System.Drawing.Point(710, 455);
            this.btnCerrar.Name = "btnCerrar";
            this.btnCerrar.Size = new System.Drawing.Size(200, 36);
            this.btnCerrar.Text = "Cerrar";
            this.btnCerrar.Click += new System.EventHandler(this.btnCerrar_Click);
            //
            // frmIdiomas
            //
            this.AutoScaleDimensions = new System.Drawing.SizeF(9F, 20F);
            this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
            this.ClientSize = new System.Drawing.Size(930, 592);
            this.Controls.Add(this.lblIdiomas);
            this.Controls.Add(this.dgvIdiomas);
            this.Controls.Add(this.btnAgregarIdioma);
            this.Controls.Add(this.btnRenombrar);
            this.Controls.Add(this.btnEliminarIdioma);
            this.Controls.Add(this.btnToggleHabilitado);
            this.Controls.Add(this.lblTraducciones);
            this.Controls.Add(this.dgvTraducciones);
            this.Controls.Add(this.btnGuardarTraducciones);
            this.Controls.Add(this.btnCerrar);
            this.Name = "frmIdiomas";
            this.StartPosition = System.Windows.Forms.FormStartPosition.CenterScreen;
            this.Text = "Gestión de idiomas";
            this.Load += new System.EventHandler(this.frmIdiomas_Load);
            this.FormClosed += new System.Windows.Forms.FormClosedEventHandler(this.frmIdiomas_FormClosed);
            ((System.ComponentModel.ISupportInitialize)(this.dgvIdiomas)).EndInit();
            ((System.ComponentModel.ISupportInitialize)(this.dgvTraducciones)).EndInit();
            this.ResumeLayout(false);
            this.PerformLayout();
        }

        private System.Windows.Forms.Label lblIdiomas;
        private System.Windows.Forms.DataGridView dgvIdiomas;
        private System.Windows.Forms.Button btnAgregarIdioma;
        private System.Windows.Forms.Button btnRenombrar;
        private System.Windows.Forms.Button btnEliminarIdioma;
        private System.Windows.Forms.Button btnToggleHabilitado;
        private System.Windows.Forms.Label lblTraducciones;
        private System.Windows.Forms.DataGridView dgvTraducciones;
        private System.Windows.Forms.Button btnGuardarTraducciones;
        private System.Windows.Forms.Button btnCerrar;
    }
}
