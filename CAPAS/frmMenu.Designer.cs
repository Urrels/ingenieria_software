namespace CAPAS
{
    partial class frmMenu
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
            this.menuStrip1 = new System.Windows.Forms.MenuStrip();
            this.usuarioToolStripMenuItem = new System.Windows.Forms.ToolStripMenuItem();
            this.misNotificacionesToolStripMenuItem = new System.Windows.Forms.ToolStripMenuItem();
            this.cerrarSesionToolStripMenuItem = new System.Windows.Forms.ToolStripMenuItem();
            this.horariosToolStripMenuItem = new System.Windows.Forms.ToolStripMenuItem();
            this.disponibilidadToolStripMenuItem = new System.Windows.Forms.ToolStripMenuItem();
            this.generarGrillaToolStripMenuItem = new System.Windows.Forms.ToolStripMenuItem();
            this.ajustarPorAusenciaToolStripMenuItem = new System.Windows.Forms.ToolStripMenuItem();
            this.gestionFranjasToolStripMenuItem = new System.Windows.Forms.ToolStripMenuItem();
            this.mantenimientoToolStripMenuItem = new System.Windows.Forms.ToolStripMenuItem();
            this.registrarUsoToolStripMenuItem = new System.Windows.Forms.ToolStripMenuItem();
            this.evaluarAutorizarToolStripMenuItem = new System.Windows.Forms.ToolStripMenuItem();
            this.coordinarVisitaToolStripMenuItem = new System.Windows.Forms.ToolStripMenuItem();
            this.realizarRevisionToolStripMenuItem = new System.Windows.Forms.ToolStripMenuItem();
            this.confirmarCierreToolStripMenuItem = new System.Windows.Forms.ToolStripMenuItem();
            this.asistenciaToolStripMenuItem = new System.Windows.Forms.ToolStripMenuItem();
            this.ficharAsistenciaToolStripMenuItem = new System.Windows.Forms.ToolStripMenuItem();
            this.registrarAsistenciaSociosToolStripMenuItem = new System.Windows.Forms.ToolStripMenuItem();
            this.evaluarCoberturaToolStripMenuItem = new System.Windows.Forms.ToolStripMenuItem();
            this.configuraciónToolStripMenuItem = new System.Windows.Forms.ToolStripMenuItem();
            this.cambiarContraseñaToolStripMenuItem = new System.Windows.Forms.ToolStripMenuItem();
            this.bitacoraToolStripMenuItem = new System.Windows.Forms.ToolStripMenuItem();
            this.administracionToolStripMenuItem = new System.Windows.Forms.ToolStripMenuItem();
            this.usuariosBloqueadosToolStripMenuItem = new System.Windows.Forms.ToolStripMenuItem();
            this.perfilesToolStripMenuItem = new System.Windows.Forms.ToolStripMenuItem();
            this.idiomasToolStripMenuItem = new System.Windows.Forms.ToolStripMenuItem();
            this.statusStrip1 = new System.Windows.Forms.StatusStrip();
            this.lblIdiomaStatus = new System.Windows.Forms.ToolStripLabel();
            this.cboIdiomaStatus = new System.Windows.Forms.ToolStripComboBox();
            this.menuStrip1.SuspendLayout();
            this.statusStrip1.SuspendLayout();
            this.SuspendLayout();
            //
            // menuStrip1
            //
            this.menuStrip1.Dock = System.Windows.Forms.DockStyle.Top;
            this.menuStrip1.GripMargin = new System.Windows.Forms.Padding(2, 2, 0, 2);
            this.menuStrip1.ImageScalingSize = new System.Drawing.Size(24, 24);
            this.menuStrip1.Items.AddRange(new System.Windows.Forms.ToolStripItem[] {
            this.usuarioToolStripMenuItem,
            this.horariosToolStripMenuItem,
            this.mantenimientoToolStripMenuItem,
            this.asistenciaToolStripMenuItem,
            this.configuraciónToolStripMenuItem,
            this.bitacoraToolStripMenuItem,
            this.administracionToolStripMenuItem});
            this.menuStrip1.Location = new System.Drawing.Point(0, 0);
            this.menuStrip1.Name = "menuStrip1";
            this.menuStrip1.Size = new System.Drawing.Size(852, 33);
            this.menuStrip1.TabIndex = 0;
            this.menuStrip1.Text = "menuStrip1";
            //
            // usuarioToolStripMenuItem
            //
            this.usuarioToolStripMenuItem.DropDownItems.AddRange(new System.Windows.Forms.ToolStripItem[] {
            this.misNotificacionesToolStripMenuItem,
            this.cerrarSesionToolStripMenuItem});
            this.usuarioToolStripMenuItem.Name = "usuarioToolStripMenuItem";
            this.usuarioToolStripMenuItem.Size = new System.Drawing.Size(88, 29);
            this.usuarioToolStripMenuItem.Text = "Usuario";
            //
            // misNotificacionesToolStripMenuItem
            //
            this.misNotificacionesToolStripMenuItem.Name = "misNotificacionesToolStripMenuItem";
            this.misNotificacionesToolStripMenuItem.Size = new System.Drawing.Size(270, 34);
            this.misNotificacionesToolStripMenuItem.Text = "Mis notificaciones";
            this.misNotificacionesToolStripMenuItem.Click += new System.EventHandler(this.misNotificacionesToolStripMenuItem_Click);
            //
            // cerrarSesionToolStripMenuItem
            //
            this.cerrarSesionToolStripMenuItem.Name = "cerrarSesionToolStripMenuItem";
            this.cerrarSesionToolStripMenuItem.Size = new System.Drawing.Size(270, 34);
            this.cerrarSesionToolStripMenuItem.Text = "Cerrar Sesion";
            this.cerrarSesionToolStripMenuItem.Click += new System.EventHandler(this.cerrarSesionToolStripMenuItem_Click);
            //
            // horariosToolStripMenuItem
            //
            this.horariosToolStripMenuItem.DropDownItems.AddRange(new System.Windows.Forms.ToolStripItem[] {
            this.disponibilidadToolStripMenuItem,
            this.generarGrillaToolStripMenuItem,
            this.ajustarPorAusenciaToolStripMenuItem,
            this.gestionFranjasToolStripMenuItem});
            this.horariosToolStripMenuItem.Name = "horariosToolStripMenuItem";
            this.horariosToolStripMenuItem.Size = new System.Drawing.Size(88, 29);
            this.horariosToolStripMenuItem.Text = "Horarios";
            //
            // disponibilidadToolStripMenuItem
            //
            this.disponibilidadToolStripMenuItem.Name = "disponibilidadToolStripMenuItem";
            this.disponibilidadToolStripMenuItem.Size = new System.Drawing.Size(280, 34);
            this.disponibilidadToolStripMenuItem.Text = "Registrar disponibilidad";
            this.disponibilidadToolStripMenuItem.Click += new System.EventHandler(this.disponibilidadToolStripMenuItem_Click);
            //
            // generarGrillaToolStripMenuItem
            //
            this.generarGrillaToolStripMenuItem.Name = "generarGrillaToolStripMenuItem";
            this.generarGrillaToolStripMenuItem.Size = new System.Drawing.Size(280, 34);
            this.generarGrillaToolStripMenuItem.Text = "Generar grilla de turnos";
            this.generarGrillaToolStripMenuItem.Click += new System.EventHandler(this.generarGrillaToolStripMenuItem_Click);
            //
            // ajustarPorAusenciaToolStripMenuItem
            //
            this.ajustarPorAusenciaToolStripMenuItem.Name = "ajustarPorAusenciaToolStripMenuItem";
            this.ajustarPorAusenciaToolStripMenuItem.Size = new System.Drawing.Size(280, 34);
            this.ajustarPorAusenciaToolStripMenuItem.Text = "Ajustar grilla por ausencia";
            this.ajustarPorAusenciaToolStripMenuItem.Click += new System.EventHandler(this.ajustarPorAusenciaToolStripMenuItem_Click);
            //
            // gestionFranjasToolStripMenuItem
            //
            this.gestionFranjasToolStripMenuItem.Name = "gestionFranjasToolStripMenuItem";
            this.gestionFranjasToolStripMenuItem.Size = new System.Drawing.Size(280, 34);
            this.gestionFranjasToolStripMenuItem.Text = "Gestión de franjas horarias";
            this.gestionFranjasToolStripMenuItem.Click += new System.EventHandler(this.gestionFranjasToolStripMenuItem_Click);
            //
            // mantenimientoToolStripMenuItem
            //
            this.mantenimientoToolStripMenuItem.DropDownItems.AddRange(new System.Windows.Forms.ToolStripItem[] {
            this.registrarUsoToolStripMenuItem,
            this.evaluarAutorizarToolStripMenuItem,
            this.coordinarVisitaToolStripMenuItem,
            this.realizarRevisionToolStripMenuItem,
            this.confirmarCierreToolStripMenuItem});
            this.mantenimientoToolStripMenuItem.Name = "mantenimientoToolStripMenuItem";
            this.mantenimientoToolStripMenuItem.Size = new System.Drawing.Size(120, 29);
            this.mantenimientoToolStripMenuItem.Text = "Mantenimiento";
            //
            // registrarUsoToolStripMenuItem
            //
            this.registrarUsoToolStripMenuItem.Name = "registrarUsoToolStripMenuItem";
            this.registrarUsoToolStripMenuItem.Size = new System.Drawing.Size(280, 34);
            this.registrarUsoToolStripMenuItem.Text = "Registrar uso de máquina";
            this.registrarUsoToolStripMenuItem.Click += new System.EventHandler(this.registrarUsoToolStripMenuItem_Click);
            //
            // evaluarAutorizarToolStripMenuItem
            //
            this.evaluarAutorizarToolStripMenuItem.Name = "evaluarAutorizarToolStripMenuItem";
            this.evaluarAutorizarToolStripMenuItem.Size = new System.Drawing.Size(280, 34);
            this.evaluarAutorizarToolStripMenuItem.Text = "Evaluar y autorizar visita";
            this.evaluarAutorizarToolStripMenuItem.Click += new System.EventHandler(this.evaluarAutorizarToolStripMenuItem_Click);
            //
            // coordinarVisitaToolStripMenuItem
            //
            this.coordinarVisitaToolStripMenuItem.Name = "coordinarVisitaToolStripMenuItem";
            this.coordinarVisitaToolStripMenuItem.Size = new System.Drawing.Size(280, 34);
            this.coordinarVisitaToolStripMenuItem.Text = "Coordinar visita técnica";
            this.coordinarVisitaToolStripMenuItem.Click += new System.EventHandler(this.coordinarVisitaToolStripMenuItem_Click);
            //
            // realizarRevisionToolStripMenuItem
            //
            this.realizarRevisionToolStripMenuItem.Name = "realizarRevisionToolStripMenuItem";
            this.realizarRevisionToolStripMenuItem.Size = new System.Drawing.Size(280, 34);
            this.realizarRevisionToolStripMenuItem.Text = "Realizar revisión técnica";
            this.realizarRevisionToolStripMenuItem.Click += new System.EventHandler(this.realizarRevisionToolStripMenuItem_Click);
            //
            // confirmarCierreToolStripMenuItem
            //
            this.confirmarCierreToolStripMenuItem.Name = "confirmarCierreToolStripMenuItem";
            this.confirmarCierreToolStripMenuItem.Size = new System.Drawing.Size(280, 34);
            this.confirmarCierreToolStripMenuItem.Text = "Confirmar cierre de mantenimiento";
            this.confirmarCierreToolStripMenuItem.Click += new System.EventHandler(this.confirmarCierreToolStripMenuItem_Click);
            //
            // asistenciaToolStripMenuItem
            //
            this.asistenciaToolStripMenuItem.DropDownItems.AddRange(new System.Windows.Forms.ToolStripItem[] {
            this.ficharAsistenciaToolStripMenuItem,
            this.registrarAsistenciaSociosToolStripMenuItem,
            this.evaluarCoberturaToolStripMenuItem});
            this.asistenciaToolStripMenuItem.Name = "asistenciaToolStripMenuItem";
            this.asistenciaToolStripMenuItem.Size = new System.Drawing.Size(100, 29);
            this.asistenciaToolStripMenuItem.Text = "Asistencia";
            //
            // ficharAsistenciaToolStripMenuItem
            //
            this.ficharAsistenciaToolStripMenuItem.Name = "ficharAsistenciaToolStripMenuItem";
            this.ficharAsistenciaToolStripMenuItem.Size = new System.Drawing.Size(280, 34);
            this.ficharAsistenciaToolStripMenuItem.Text = "Fichar asistencia";
            this.ficharAsistenciaToolStripMenuItem.Click += new System.EventHandler(this.ficharAsistenciaToolStripMenuItem_Click);
            //
            // registrarAsistenciaSociosToolStripMenuItem
            //
            this.registrarAsistenciaSociosToolStripMenuItem.Name = "registrarAsistenciaSociosToolStripMenuItem";
            this.registrarAsistenciaSociosToolStripMenuItem.Size = new System.Drawing.Size(280, 34);
            this.registrarAsistenciaSociosToolStripMenuItem.Text = "Registrar asistencia de socios";
            this.registrarAsistenciaSociosToolStripMenuItem.Click += new System.EventHandler(this.registrarAsistenciaSociosToolStripMenuItem_Click);
            //
            // evaluarCoberturaToolStripMenuItem
            //
            this.evaluarCoberturaToolStripMenuItem.Name = "evaluarCoberturaToolStripMenuItem";
            this.evaluarCoberturaToolStripMenuItem.Size = new System.Drawing.Size(280, 34);
            this.evaluarCoberturaToolStripMenuItem.Text = "Evaluar cobertura del servicio";
            this.evaluarCoberturaToolStripMenuItem.Click += new System.EventHandler(this.evaluarCoberturaToolStripMenuItem_Click);
            //
            // configuraciónToolStripMenuItem
            //
            this.configuraciónToolStripMenuItem.DropDownItems.AddRange(new System.Windows.Forms.ToolStripItem[] {
            this.cambiarContraseñaToolStripMenuItem});
            this.configuraciónToolStripMenuItem.Name = "configuraciónToolStripMenuItem";
            this.configuraciónToolStripMenuItem.Size = new System.Drawing.Size(139, 29);
            this.configuraciónToolStripMenuItem.Text = "Configuración";
            //
            // cambiarContraseñaToolStripMenuItem
            //
            this.cambiarContraseñaToolStripMenuItem.Name = "cambiarContraseñaToolStripMenuItem";
            this.cambiarContraseñaToolStripMenuItem.Size = new System.Drawing.Size(274, 34);
            this.cambiarContraseñaToolStripMenuItem.Text = "Cambiar Contraseña";
            this.cambiarContraseñaToolStripMenuItem.Click += new System.EventHandler(this.cambiarContraseñaToolStripMenuItem_Click);
            //
            // bitacoraToolStripMenuItem
            //
            this.bitacoraToolStripMenuItem.Name = "bitacoraToolStripMenuItem";
            this.bitacoraToolStripMenuItem.Size = new System.Drawing.Size(91, 29);
            this.bitacoraToolStripMenuItem.Text = "Bitácora";
            this.bitacoraToolStripMenuItem.Click += new System.EventHandler(this.bitacoraToolStripMenuItem_Click);
            //
            // administracionToolStripMenuItem
            //
            this.administracionToolStripMenuItem.DropDownItems.AddRange(new System.Windows.Forms.ToolStripItem[] {
            this.usuariosBloqueadosToolStripMenuItem,
            this.perfilesToolStripMenuItem,
            this.idiomasToolStripMenuItem});
            this.administracionToolStripMenuItem.Name = "administracionToolStripMenuItem";
            this.administracionToolStripMenuItem.Size = new System.Drawing.Size(140, 29);
            this.administracionToolStripMenuItem.Text = "Administración";
            //
            // usuariosBloqueadosToolStripMenuItem
            //
            this.usuariosBloqueadosToolStripMenuItem.Name = "usuariosBloqueadosToolStripMenuItem";
            this.usuariosBloqueadosToolStripMenuItem.Size = new System.Drawing.Size(280, 34);
            this.usuariosBloqueadosToolStripMenuItem.Text = "Gestión de usuarios";
            this.usuariosBloqueadosToolStripMenuItem.Click += new System.EventHandler(this.usuariosBloqueadosToolStripMenuItem_Click);
            //
            // perfilesToolStripMenuItem
            //
            this.perfilesToolStripMenuItem.Name = "perfilesToolStripMenuItem";
            this.perfilesToolStripMenuItem.Size = new System.Drawing.Size(280, 34);
            this.perfilesToolStripMenuItem.Text = "Roles y Permisos";
            this.perfilesToolStripMenuItem.Click += new System.EventHandler(this.perfilesToolStripMenuItem_Click);
            //
            // idiomasToolStripMenuItem
            //
            this.idiomasToolStripMenuItem.Name = "idiomasToolStripMenuItem";
            this.idiomasToolStripMenuItem.Size = new System.Drawing.Size(280, 34);
            this.idiomasToolStripMenuItem.Text = "Gestión de idiomas";
            this.idiomasToolStripMenuItem.Click += new System.EventHandler(this.idiomasToolStripMenuItem_Click);
            //
            // statusStrip1
            //
            this.statusStrip1.Dock = System.Windows.Forms.DockStyle.Bottom;
            this.statusStrip1.ImageScalingSize = new System.Drawing.Size(24, 24);
            this.statusStrip1.Items.AddRange(new System.Windows.Forms.ToolStripItem[] {
            this.lblIdiomaStatus,
            this.cboIdiomaStatus});
            this.statusStrip1.Location = new System.Drawing.Point(0, 416);
            this.statusStrip1.Name = "statusStrip1";
            this.statusStrip1.Size = new System.Drawing.Size(852, 33);
            this.statusStrip1.TabIndex = 1;
            //
            // lblIdiomaStatus
            //
            this.lblIdiomaStatus.Name = "lblIdiomaStatus";
            this.lblIdiomaStatus.Size = new System.Drawing.Size(70, 28);
            this.lblIdiomaStatus.Text = "Idioma:";
            //
            // cboIdiomaStatus
            //
            this.cboIdiomaStatus.DropDownStyle = System.Windows.Forms.ComboBoxStyle.DropDownList;
            this.cboIdiomaStatus.Name = "cboIdiomaStatus";
            this.cboIdiomaStatus.Size = new System.Drawing.Size(200, 33);
            this.cboIdiomaStatus.SelectedIndexChanged += new System.EventHandler(this.cboIdiomaStatus_SelectedIndexChanged);
            //
            // frmMenu
            //
            this.AutoScaleDimensions = new System.Drawing.SizeF(9F, 20F);
            this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
            this.BackColor = System.Drawing.SystemColors.ActiveCaption;
            this.ClientSize = new System.Drawing.Size(852, 449);
            this.MinimumSize = new System.Drawing.Size(560, 360);
            this.Controls.Add(this.menuStrip1);
            this.Controls.Add(this.statusStrip1);
            this.MainMenuStrip = this.menuStrip1;
            this.Name = "frmMenu";
            this.Text = "frmMenu";
            this.Load += new System.EventHandler(this.frmMenu_Load);
            this.FormClosed += new System.Windows.Forms.FormClosedEventHandler(this.frmMenu_FormClosed);
            this.menuStrip1.ResumeLayout(false);
            this.menuStrip1.PerformLayout();
            this.statusStrip1.ResumeLayout(false);
            this.statusStrip1.PerformLayout();
            this.ResumeLayout(false);
            this.PerformLayout();
        }

        private System.Windows.Forms.MenuStrip menuStrip1;
        private System.Windows.Forms.ToolStripMenuItem usuarioToolStripMenuItem;
        private System.Windows.Forms.ToolStripMenuItem misNotificacionesToolStripMenuItem;
        private System.Windows.Forms.ToolStripMenuItem cerrarSesionToolStripMenuItem;
        private System.Windows.Forms.ToolStripMenuItem horariosToolStripMenuItem;
        private System.Windows.Forms.ToolStripMenuItem disponibilidadToolStripMenuItem;
        private System.Windows.Forms.ToolStripMenuItem generarGrillaToolStripMenuItem;
        private System.Windows.Forms.ToolStripMenuItem ajustarPorAusenciaToolStripMenuItem;
        private System.Windows.Forms.ToolStripMenuItem gestionFranjasToolStripMenuItem;
        private System.Windows.Forms.ToolStripMenuItem mantenimientoToolStripMenuItem;
        private System.Windows.Forms.ToolStripMenuItem registrarUsoToolStripMenuItem;
        private System.Windows.Forms.ToolStripMenuItem evaluarAutorizarToolStripMenuItem;
        private System.Windows.Forms.ToolStripMenuItem coordinarVisitaToolStripMenuItem;
        private System.Windows.Forms.ToolStripMenuItem realizarRevisionToolStripMenuItem;
        private System.Windows.Forms.ToolStripMenuItem confirmarCierreToolStripMenuItem;
        private System.Windows.Forms.ToolStripMenuItem asistenciaToolStripMenuItem;
        private System.Windows.Forms.ToolStripMenuItem ficharAsistenciaToolStripMenuItem;
        private System.Windows.Forms.ToolStripMenuItem registrarAsistenciaSociosToolStripMenuItem;
        private System.Windows.Forms.ToolStripMenuItem evaluarCoberturaToolStripMenuItem;
        private System.Windows.Forms.ToolStripMenuItem configuraciónToolStripMenuItem;
        private System.Windows.Forms.ToolStripMenuItem cambiarContraseñaToolStripMenuItem;
        private System.Windows.Forms.ToolStripMenuItem bitacoraToolStripMenuItem;
        private System.Windows.Forms.ToolStripMenuItem administracionToolStripMenuItem;
        private System.Windows.Forms.ToolStripMenuItem usuariosBloqueadosToolStripMenuItem;
        private System.Windows.Forms.ToolStripMenuItem perfilesToolStripMenuItem;
        private System.Windows.Forms.ToolStripMenuItem idiomasToolStripMenuItem;
        private System.Windows.Forms.StatusStrip statusStrip1;
        private System.Windows.Forms.ToolStripLabel lblIdiomaStatus;
        private System.Windows.Forms.ToolStripComboBox cboIdiomaStatus;
    }
}