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
            this.SuspendLayout();
            // 
            // frmMisNotificaciones
            // 
            this.ClientSize = new System.Drawing.Size(300, 300);
            this.Name = "frmMisNotificaciones";
            this.Load += new System.EventHandler(this.frmMisNotificaciones_Load);
            this.ResumeLayout(false);

        }

        private System.Windows.Forms.Label lblTitulo;
        private System.Windows.Forms.DataGridView dgvNotificaciones;
        private System.Windows.Forms.Button btnExportar;
        private System.Windows.Forms.Button btnCerrar;
        private System.Windows.Forms.Timer timerRefresco;
    }
}