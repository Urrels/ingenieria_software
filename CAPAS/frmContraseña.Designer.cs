namespace CAPAS
{
    partial class frmContraseña
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
            this.label1 = new System.Windows.Forms.Label();
            this.txtPassActual = new System.Windows.Forms.TextBox();
            this.txtNuevaPass = new System.Windows.Forms.TextBox();
            this.label2 = new System.Windows.Forms.Label();
            this.txtConfPass = new System.Windows.Forms.TextBox();
            this.label3 = new System.Windows.Forms.Label();
            this.btnContinuar = new System.Windows.Forms.Button();
            this.button1 = new System.Windows.Forms.Button();
            this.label4 = new System.Windows.Forms.Label();
            this.label5 = new System.Windows.Forms.Label();
            this.SuspendLayout();
            // 
            // label1
            // 
            this.label1.AutoSize = true;
            this.label1.Location = new System.Drawing.Point(42, 68);
            this.label1.Margin = new System.Windows.Forms.Padding(4, 0, 4, 0);
            this.label1.Name = "label1";
            this.label1.Size = new System.Drawing.Size(145, 20);
            this.label1.TabIndex = 0;
            this.label1.Text = "Contraseña Actual:";
            // 
            // txtPassActual
            // 
            this.txtPassActual.Location = new System.Drawing.Point(35, 106);
            this.txtPassActual.Margin = new System.Windows.Forms.Padding(4, 5, 4, 5);
            this.txtPassActual.Name = "txtPassActual";
            this.txtPassActual.PasswordChar = '*';
            this.txtPassActual.Size = new System.Drawing.Size(296, 26);
            this.txtPassActual.TabIndex = 1;
            // 
            // txtNuevaPass
            // 
            this.txtNuevaPass.Location = new System.Drawing.Point(35, 212);
            this.txtNuevaPass.Margin = new System.Windows.Forms.Padding(4, 5, 4, 5);
            this.txtNuevaPass.Name = "txtNuevaPass";
            this.txtNuevaPass.PasswordChar = '*';
            this.txtNuevaPass.Size = new System.Drawing.Size(296, 26);
            this.txtNuevaPass.TabIndex = 3;
            // 
            // label2
            // 
            this.label2.AutoSize = true;
            this.label2.Location = new System.Drawing.Point(42, 174);
            this.label2.Margin = new System.Windows.Forms.Padding(4, 0, 4, 0);
            this.label2.Name = "label2";
            this.label2.Size = new System.Drawing.Size(145, 20);
            this.label2.TabIndex = 2;
            this.label2.Text = "Nueva Contraseña:";
            // 
            // txtConfPass
            // 
            this.txtConfPass.Location = new System.Drawing.Point(35, 400);
            this.txtConfPass.Margin = new System.Windows.Forms.Padding(4, 5, 4, 5);
            this.txtConfPass.Name = "txtConfPass";
            this.txtConfPass.PasswordChar = '*';
            this.txtConfPass.Size = new System.Drawing.Size(296, 26);
            this.txtConfPass.TabIndex = 5;
            // 
            // label3
            // 
            this.label3.AutoSize = true;
            this.label3.Location = new System.Drawing.Point(42, 361);
            this.label3.Margin = new System.Windows.Forms.Padding(4, 0, 4, 0);
            this.label3.Name = "label3";
            this.label3.Size = new System.Drawing.Size(169, 20);
            this.label3.TabIndex = 4;
            this.label3.Text = "Confirmar Contraseña:";
            // 
            // btnContinuar
            // 
            this.btnContinuar.Location = new System.Drawing.Point(37, 476);
            this.btnContinuar.Margin = new System.Windows.Forms.Padding(4, 5, 4, 5);
            this.btnContinuar.Name = "btnContinuar";
            this.btnContinuar.Size = new System.Drawing.Size(110, 35);
            this.btnContinuar.TabIndex = 6;
            this.btnContinuar.Text = "Continuar";
            this.btnContinuar.UseVisualStyleBackColor = true;
            this.btnContinuar.Click += new System.EventHandler(this.btnContinuar_Click);
            // 
            // button1
            // 
            this.button1.Location = new System.Drawing.Point(211, 476);
            this.button1.Margin = new System.Windows.Forms.Padding(4, 5, 4, 5);
            this.button1.Name = "button1";
            this.button1.Size = new System.Drawing.Size(110, 35);
            this.button1.TabIndex = 7;
            this.button1.Text = "Cancelar";
            this.button1.UseVisualStyleBackColor = true;
            this.button1.Click += new System.EventHandler(this.button1_Click);
            // 
            // label4
            // 
            this.label4.AutoSize = true;
            this.label4.Location = new System.Drawing.Point(54, 268);
            this.label4.Margin = new System.Windows.Forms.Padding(4, 0, 4, 0);
            this.label4.Name = "label4";
            this.label4.Size = new System.Drawing.Size(239, 60);
            this.label4.TabIndex = 8;
            this.label4.Text = " - 6 o mas caracteres\r\n- 1 o mas Letras MAYÚSCULAS\r\n- 1 o mas NUMEROS";
            // 
            // label5
            // 
            this.label5.AutoSize = true;
            this.label5.Font = new System.Drawing.Font("Microsoft Sans Serif", 11F, System.Drawing.FontStyle.Bold);
            this.label5.Location = new System.Drawing.Point(41, 12);
            this.label5.Margin = new System.Windows.Forms.Padding(4, 0, 4, 0);
            this.label5.Name = "label5";
            this.label5.Size = new System.Drawing.Size(231, 26);
            this.label5.TabIndex = 9;
            this.label5.Text = "Cambiar Contraseña";
            // 
            // frmContraseña
            // 
            this.AutoScaleDimensions = new System.Drawing.SizeF(9F, 20F);
            this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
            this.ClientSize = new System.Drawing.Size(369, 542);
            this.Controls.Add(this.label5);
            this.Controls.Add(this.label4);
            this.Controls.Add(this.button1);
            this.Controls.Add(this.btnContinuar);
            this.Controls.Add(this.txtConfPass);
            this.Controls.Add(this.label3);
            this.Controls.Add(this.txtNuevaPass);
            this.Controls.Add(this.label2);
            this.Controls.Add(this.txtPassActual);
            this.Controls.Add(this.label1);
            this.Margin = new System.Windows.Forms.Padding(4, 5, 4, 5);
            this.Name = "frmContraseña";
            this.Text = "Cambiar Contraseña";
            this.Load += new System.EventHandler(this.frmContraseña_Load);
            this.ResumeLayout(false);
            this.PerformLayout();

        }

        private System.Windows.Forms.Label label1;
        private System.Windows.Forms.Label label2;
        private System.Windows.Forms.Label label3;
        private System.Windows.Forms.Label label4;
        private System.Windows.Forms.Label label5;
        private System.Windows.Forms.TextBox txtPassActual;
        private System.Windows.Forms.TextBox txtNuevaPass;
        private System.Windows.Forms.TextBox txtConfPass;
        private System.Windows.Forms.Button btnContinuar;
        private System.Windows.Forms.Button button1;
    }
}