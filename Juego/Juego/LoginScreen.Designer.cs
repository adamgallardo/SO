namespace Juego
{
    partial class LoginScreen
    {
        /// <summary>
        /// Required designer variable.
        /// </summary>
        private System.ComponentModel.IContainer components = null;

        /// <summary>
        /// Clean up any resources being used.
        /// </summary>
        /// <param name="disposing">true if managed resources should be disposed; otherwise, false.</param>
        protected override void Dispose(bool disposing)
        {
            if (disposing && (components != null))
            {
                components.Dispose();
            }
            base.Dispose(disposing);
        }

        #region Windows Form Designer generated code

        /// <summary>
        /// Required method for Designer support - do not modify
        /// the contents of this method with the code editor.
        /// </summary>
        private void InitializeComponent()
        {
            System.ComponentModel.ComponentResourceManager resources = new System.ComponentModel.ComponentResourceManager(typeof(LoginScreen));
            this.Drag_Pnl = new System.Windows.Forms.PictureBox();
            this.SR_Lbl = new System.Windows.Forms.Label();
            this.user_Tbx = new System.Windows.Forms.TextBox();
            this.pass_Tbx = new System.Windows.Forms.TextBox();
            this.CL_Lbl = new System.Windows.Forms.Label();
            this.eyeS_Pnl = new System.Windows.Forms.PictureBox();
            this.eyeH_Pnl = new System.Windows.Forms.PictureBox();
            this.min_Btn = new System.Windows.Forms.Button();
            this.exit_Btn = new System.Windows.Forms.Button();
            this.Enter_Pnl = new System.Windows.Forms.Panel();
            this.rapida = new System.Windows.Forms.RadioButton();
            this.ganadas = new System.Windows.Forms.RadioButton();
            this.ganador = new System.Windows.Forms.RadioButton();
            this.Lista = new System.Windows.Forms.ListBox();
            this.textBox1 = new System.Windows.Forms.TextBox();
            this.consultar = new System.Windows.Forms.Button();
            this.label1 = new System.Windows.Forms.Label();
            ((System.ComponentModel.ISupportInitialize)(this.Drag_Pnl)).BeginInit();
            ((System.ComponentModel.ISupportInitialize)(this.eyeS_Pnl)).BeginInit();
            ((System.ComponentModel.ISupportInitialize)(this.eyeH_Pnl)).BeginInit();
            this.SuspendLayout();
            // 
            // Drag_Pnl
            // 
            this.Drag_Pnl.BackColor = System.Drawing.Color.Transparent;
            this.Drag_Pnl.Location = new System.Drawing.Point(0, 0);
            this.Drag_Pnl.Name = "Drag_Pnl";
            this.Drag_Pnl.Size = new System.Drawing.Size(1280, 143);
            this.Drag_Pnl.TabIndex = 0;
            this.Drag_Pnl.TabStop = false;
            this.Drag_Pnl.MouseDown += new System.Windows.Forms.MouseEventHandler(this.Drag_Pnl_MouseDown);
            this.Drag_Pnl.MouseMove += new System.Windows.Forms.MouseEventHandler(this.Drag_Pnl_MouseMove);
            this.Drag_Pnl.MouseUp += new System.Windows.Forms.MouseEventHandler(this.Drag_Pnl_MouseUp);
            // 
            // SR_Lbl
            // 
            this.SR_Lbl.AutoSize = true;
            this.SR_Lbl.BackColor = System.Drawing.Color.Transparent;
            this.SR_Lbl.Font = new System.Drawing.Font("Microsoft YaHei UI", 14.25F, System.Drawing.FontStyle.Bold, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.SR_Lbl.ForeColor = System.Drawing.Color.FromArgb(((int)(((byte)(25)))), ((int)(((byte)(25)))), ((int)(((byte)(25)))));
            this.SR_Lbl.Location = new System.Drawing.Point(34, 232);
            this.SR_Lbl.Name = "SR_Lbl";
            this.SR_Lbl.Size = new System.Drawing.Size(136, 26);
            this.SR_Lbl.TabIndex = 1;
            this.SR_Lbl.Text = "Iniciar sesión";
            // 
            // user_Tbx
            // 
            this.user_Tbx.BackColor = System.Drawing.Color.FromArgb(((int)(((byte)(226)))), ((int)(((byte)(226)))), ((int)(((byte)(226)))));
            this.user_Tbx.BorderStyle = System.Windows.Forms.BorderStyle.None;
            this.user_Tbx.Font = new System.Drawing.Font("Microsoft YaHei UI", 9F, System.Drawing.FontStyle.Bold, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.user_Tbx.ForeColor = System.Drawing.Color.FromArgb(((int)(((byte)(166)))), ((int)(((byte)(166)))), ((int)(((byte)(166)))));
            this.user_Tbx.Location = new System.Drawing.Point(58, 294);
            this.user_Tbx.Name = "user_Tbx";
            this.user_Tbx.Size = new System.Drawing.Size(250, 16);
            this.user_Tbx.TabIndex = 2;
            this.user_Tbx.TabStop = false;
            this.user_Tbx.Text = "USUARIO";
            this.user_Tbx.Click += new System.EventHandler(this.user_Tbx_Click);
            // 
            // pass_Tbx
            // 
            this.pass_Tbx.BackColor = System.Drawing.Color.FromArgb(((int)(((byte)(226)))), ((int)(((byte)(226)))), ((int)(((byte)(226)))));
            this.pass_Tbx.BorderStyle = System.Windows.Forms.BorderStyle.None;
            this.pass_Tbx.Font = new System.Drawing.Font("Microsoft YaHei UI", 9F, System.Drawing.FontStyle.Bold, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.pass_Tbx.ForeColor = System.Drawing.Color.FromArgb(((int)(((byte)(166)))), ((int)(((byte)(166)))), ((int)(((byte)(166)))));
            this.pass_Tbx.Location = new System.Drawing.Point(58, 397);
            this.pass_Tbx.Name = "pass_Tbx";
            this.pass_Tbx.Size = new System.Drawing.Size(211, 16);
            this.pass_Tbx.TabIndex = 3;
            this.pass_Tbx.TabStop = false;
            this.pass_Tbx.Text = "CONTRASEÑA";
            this.pass_Tbx.Click += new System.EventHandler(this.pass_Tbx_Click);
            // 
            // CL_Lbl
            // 
            this.CL_Lbl.AutoSize = true;
            this.CL_Lbl.BackColor = System.Drawing.Color.Transparent;
            this.CL_Lbl.Font = new System.Drawing.Font("Microsoft YaHei UI", 9.75F, System.Drawing.FontStyle.Bold, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.CL_Lbl.ForeColor = System.Drawing.Color.FromArgb(((int)(((byte)(166)))), ((int)(((byte)(166)))), ((int)(((byte)(166)))));
            this.CL_Lbl.Location = new System.Drawing.Point(34, 686);
            this.CL_Lbl.Name = "CL_Lbl";
            this.CL_Lbl.Size = new System.Drawing.Size(147, 19);
            this.CL_Lbl.TabIndex = 4;
            this.CL_Lbl.Text = "CREAR UNA CUENTA";
            this.CL_Lbl.Click += new System.EventHandler(this.CL_Lbl_Click);
            this.CL_Lbl.MouseLeave += new System.EventHandler(this.CL_Lbl_MouseLeave);
            this.CL_Lbl.MouseMove += new System.Windows.Forms.MouseEventHandler(this.CL_Lbl_MouseMove);
            // 
            // eyeS_Pnl
            // 
            this.eyeS_Pnl.BackColor = System.Drawing.Color.FromArgb(((int)(((byte)(226)))), ((int)(((byte)(226)))), ((int)(((byte)(226)))));
            this.eyeS_Pnl.BackgroundImage = ((System.Drawing.Image)(resources.GetObject("eyeS_Pnl.BackgroundImage")));
            this.eyeS_Pnl.Location = new System.Drawing.Point(287, 395);
            this.eyeS_Pnl.Name = "eyeS_Pnl";
            this.eyeS_Pnl.Size = new System.Drawing.Size(20, 20);
            this.eyeS_Pnl.TabIndex = 5;
            this.eyeS_Pnl.TabStop = false;
            this.eyeS_Pnl.Visible = false;
            this.eyeS_Pnl.Click += new System.EventHandler(this.eyeS_Pnl_Click);
            // 
            // eyeH_Pnl
            // 
            this.eyeH_Pnl.BackColor = System.Drawing.Color.FromArgb(((int)(((byte)(226)))), ((int)(((byte)(226)))), ((int)(((byte)(226)))));
            this.eyeH_Pnl.BackgroundImage = ((System.Drawing.Image)(resources.GetObject("eyeH_Pnl.BackgroundImage")));
            this.eyeH_Pnl.Location = new System.Drawing.Point(287, 395);
            this.eyeH_Pnl.Name = "eyeH_Pnl";
            this.eyeH_Pnl.Size = new System.Drawing.Size(20, 20);
            this.eyeH_Pnl.TabIndex = 6;
            this.eyeH_Pnl.TabStop = false;
            this.eyeH_Pnl.Visible = false;
            this.eyeH_Pnl.Click += new System.EventHandler(this.eyeH_Pnl_Click);
            // 
            // min_Btn
            // 
            this.min_Btn.BackColor = System.Drawing.Color.FromArgb(((int)(((byte)(255)))), ((int)(((byte)(219)))), ((int)(((byte)(195)))));
            this.min_Btn.FlatAppearance.BorderSize = 0;
            this.min_Btn.FlatStyle = System.Windows.Forms.FlatStyle.Flat;
            this.min_Btn.ForeColor = System.Drawing.Color.Transparent;
            this.min_Btn.Image = ((System.Drawing.Image)(resources.GetObject("min_Btn.Image")));
            this.min_Btn.Location = new System.Drawing.Point(1154, 0);
            this.min_Btn.Name = "min_Btn";
            this.min_Btn.Size = new System.Drawing.Size(63, 40);
            this.min_Btn.TabIndex = 9;
            this.min_Btn.TabStop = false;
            this.min_Btn.UseVisualStyleBackColor = false;
            this.min_Btn.Click += new System.EventHandler(this.min_Btn_Click);
            this.min_Btn.MouseLeave += new System.EventHandler(this.min_Btn_MouseLeave);
            this.min_Btn.MouseMove += new System.Windows.Forms.MouseEventHandler(this.min_Btn_MouseMove);
            // 
            // exit_Btn
            // 
            this.exit_Btn.BackColor = System.Drawing.Color.FromArgb(((int)(((byte)(255)))), ((int)(((byte)(219)))), ((int)(((byte)(195)))));
            this.exit_Btn.FlatAppearance.BorderSize = 0;
            this.exit_Btn.FlatStyle = System.Windows.Forms.FlatStyle.Flat;
            this.exit_Btn.ForeColor = System.Drawing.Color.Transparent;
            this.exit_Btn.Image = ((System.Drawing.Image)(resources.GetObject("exit_Btn.Image")));
            this.exit_Btn.Location = new System.Drawing.Point(1217, 0);
            this.exit_Btn.Name = "exit_Btn";
            this.exit_Btn.Size = new System.Drawing.Size(63, 40);
            this.exit_Btn.TabIndex = 10;
            this.exit_Btn.TabStop = false;
            this.exit_Btn.UseVisualStyleBackColor = false;
            this.exit_Btn.Click += new System.EventHandler(this.exit_Btn_Click);
            this.exit_Btn.MouseLeave += new System.EventHandler(this.exit_Btn_MouseLeave);
            this.exit_Btn.MouseMove += new System.Windows.Forms.MouseEventHandler(this.exit_Btn_MouseMove);
            // 
            // Enter_Pnl
            // 
            this.Enter_Pnl.BackColor = System.Drawing.Color.Transparent;
            this.Enter_Pnl.BackgroundImage = ((System.Drawing.Image)(resources.GetObject("Enter_Pnl.BackgroundImage")));
            this.Enter_Pnl.BackgroundImageLayout = System.Windows.Forms.ImageLayout.Zoom;
            this.Enter_Pnl.Location = new System.Drawing.Point(151, 480);
            this.Enter_Pnl.Name = "Enter_Pnl";
            this.Enter_Pnl.Size = new System.Drawing.Size(63, 63);
            this.Enter_Pnl.TabIndex = 11;
            this.Enter_Pnl.Click += new System.EventHandler(this.Enter_Pnl_Click);
            // 
            // rapida
            // 
            this.rapida.AutoSize = true;
            this.rapida.BackColor = System.Drawing.Color.Transparent;
            this.rapida.ForeColor = System.Drawing.SystemColors.ControlLightLight;
            this.rapida.Location = new System.Drawing.Point(1030, 614);
            this.rapida.Name = "rapida";
            this.rapida.Size = new System.Drawing.Size(112, 17);
            this.rapida.TabIndex = 12;
            this.rapida.TabStop = true;
            this.rapida.Text = "Partida más rapida";
            this.rapida.UseVisualStyleBackColor = false;
            // 
            // ganadas
            // 
            this.ganadas.AutoSize = true;
            this.ganadas.BackColor = System.Drawing.Color.Transparent;
            this.ganadas.ForeColor = System.Drawing.SystemColors.ControlLightLight;
            this.ganadas.Location = new System.Drawing.Point(1030, 638);
            this.ganadas.Name = "ganadas";
            this.ganadas.Size = new System.Drawing.Size(107, 17);
            this.ganadas.TabIndex = 13;
            this.ganadas.TabStop = true;
            this.ganadas.Text = "Partidas ganadas";
            this.ganadas.UseVisualStyleBackColor = false;
            // 
            // ganador
            // 
            this.ganador.AutoSize = true;
            this.ganador.BackColor = System.Drawing.Color.Transparent;
            this.ganador.ForeColor = System.Drawing.SystemColors.ControlLightLight;
            this.ganador.Location = new System.Drawing.Point(1030, 662);
            this.ganador.Name = "ganador";
            this.ganador.Size = new System.Drawing.Size(102, 17);
            this.ganador.TabIndex = 14;
            this.ganador.TabStop = true;
            this.ganador.Text = "Dime el ganador";
            this.ganador.UseVisualStyleBackColor = false;
            // 
            // Lista
            // 
            this.Lista.FormattingEnabled = true;
            this.Lista.Location = new System.Drawing.Point(1148, 613);
            this.Lista.Name = "Lista";
            this.Lista.Size = new System.Drawing.Size(120, 95);
            this.Lista.TabIndex = 15;
            // 
            // textBox1
            // 
            this.textBox1.Location = new System.Drawing.Point(1030, 588);
            this.textBox1.Name = "textBox1";
            this.textBox1.Size = new System.Drawing.Size(112, 20);
            this.textBox1.TabIndex = 16;
            // 
            // consultar
            // 
            this.consultar.Location = new System.Drawing.Point(1030, 685);
            this.consultar.Name = "consultar";
            this.consultar.Size = new System.Drawing.Size(112, 23);
            this.consultar.TabIndex = 17;
            this.consultar.Text = "Consultar";
            this.consultar.UseVisualStyleBackColor = true;
            this.consultar.Click += new System.EventHandler(this.consultar_Click);
            // 
            // label1
            // 
            this.label1.AutoSize = true;
            this.label1.BackColor = System.Drawing.Color.Transparent;
            this.label1.ForeColor = System.Drawing.SystemColors.ControlLightLight;
            this.label1.Location = new System.Drawing.Point(1158, 591);
            this.label1.Name = "label1";
            this.label1.Size = new System.Drawing.Size(98, 13);
            this.label1.TabIndex = 18;
            this.label1.Text = "Lista de conectado";
            // 
            // LoginScreen
            // 
            this.AutoScaleDimensions = new System.Drawing.SizeF(6F, 13F);
            this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
            this.BackColor = System.Drawing.Color.White;
            this.BackgroundImage = ((System.Drawing.Image)(resources.GetObject("$this.BackgroundImage")));
            this.ClientSize = new System.Drawing.Size(1280, 720);
            this.Controls.Add(this.label1);
            this.Controls.Add(this.consultar);
            this.Controls.Add(this.textBox1);
            this.Controls.Add(this.Lista);
            this.Controls.Add(this.ganador);
            this.Controls.Add(this.ganadas);
            this.Controls.Add(this.rapida);
            this.Controls.Add(this.Enter_Pnl);
            this.Controls.Add(this.exit_Btn);
            this.Controls.Add(this.min_Btn);
            this.Controls.Add(this.eyeH_Pnl);
            this.Controls.Add(this.eyeS_Pnl);
            this.Controls.Add(this.CL_Lbl);
            this.Controls.Add(this.pass_Tbx);
            this.Controls.Add(this.user_Tbx);
            this.Controls.Add(this.SR_Lbl);
            this.Controls.Add(this.Drag_Pnl);
            this.DoubleBuffered = true;
            this.FormBorderStyle = System.Windows.Forms.FormBorderStyle.None;
            this.Icon = ((System.Drawing.Icon)(resources.GetObject("$this.Icon")));
            this.Name = "LoginScreen";
            this.StartPosition = System.Windows.Forms.FormStartPosition.CenterScreen;
            this.Text = "DAN";
            ((System.ComponentModel.ISupportInitialize)(this.Drag_Pnl)).EndInit();
            ((System.ComponentModel.ISupportInitialize)(this.eyeS_Pnl)).EndInit();
            ((System.ComponentModel.ISupportInitialize)(this.eyeH_Pnl)).EndInit();
            this.ResumeLayout(false);
            this.PerformLayout();

        }

        #endregion

        private System.Windows.Forms.PictureBox Drag_Pnl;
        private System.Windows.Forms.Label SR_Lbl;
        private System.Windows.Forms.TextBox user_Tbx;
        private System.Windows.Forms.TextBox pass_Tbx;
        private System.Windows.Forms.Label CL_Lbl;
        private System.Windows.Forms.PictureBox eyeS_Pnl;
        private System.Windows.Forms.PictureBox eyeH_Pnl;
        private System.Windows.Forms.Button min_Btn;
        private System.Windows.Forms.Button exit_Btn;
        private System.Windows.Forms.Panel Enter_Pnl;
        private System.Windows.Forms.RadioButton rapida;
        private System.Windows.Forms.RadioButton ganadas;
        private System.Windows.Forms.RadioButton ganador;
        private System.Windows.Forms.ListBox Lista;
        private System.Windows.Forms.TextBox textBox1;
        private System.Windows.Forms.Button consultar;
        private System.Windows.Forms.Label label1;
    }
}

