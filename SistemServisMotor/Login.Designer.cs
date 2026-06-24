namespace SistemServisMotor
{
    partial class LoginForm
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
            System.ComponentModel.ComponentResourceManager resources = new System.ComponentModel.ComponentResourceManager(typeof(LoginForm));
            this.lblTitle = new System.Windows.Forms.Label();
            this.lblStatus = new System.Windows.Forms.Label();
            this.lblusername = new System.Windows.Forms.Label();
            this.lbltele = new System.Windows.Forms.Label();
            this.txtusername = new System.Windows.Forms.TextBox();
            this.txttele = new System.Windows.Forms.TextBox();
            this.btnlogin = new System.Windows.Forms.Button();
            this.pictureBox1 = new System.Windows.Forms.PictureBox();
            ((System.ComponentModel.ISupportInitialize)(this.pictureBox1)).BeginInit();
            this.SuspendLayout();
            // 
            // lblTitle
            // 
            this.lblTitle.Anchor = ((System.Windows.Forms.AnchorStyles)((((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Bottom) 
            | System.Windows.Forms.AnchorStyles.Left) 
            | System.Windows.Forms.AnchorStyles.Right)));
            this.lblTitle.AutoSize = true;
            this.lblTitle.Font = new System.Drawing.Font("Dubai", 16.2F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.lblTitle.Location = new System.Drawing.Point(260, 315);
            this.lblTitle.Name = "lblTitle";
            this.lblTitle.Size = new System.Drawing.Size(360, 48);
            this.lblTitle.TabIndex = 0;
            this.lblTitle.Text = "Sistem Pencatat Servis Motor";
            // 
            // lblStatus
            // 
            this.lblStatus.AutoSize = true;
            this.lblStatus.Location = new System.Drawing.Point(428, 374);
            this.lblStatus.Name = "lblStatus";
            this.lblStatus.Size = new System.Drawing.Size(17, 16);
            this.lblStatus.TabIndex = 1;
            this.lblStatus.Text = "\"\"";
            // 
            // lblusername
            // 
            this.lblusername.AutoSize = true;
            this.lblusername.Font = new System.Drawing.Font("Dubai", 12F);
            this.lblusername.Location = new System.Drawing.Point(262, 401);
            this.lblusername.Name = "lblusername";
            this.lblusername.Size = new System.Drawing.Size(104, 34);
            this.lblusername.TabIndex = 2;
            this.lblusername.Text = "Username :";
            // 
            // lbltele
            // 
            this.lbltele.AutoSize = true;
            this.lbltele.Font = new System.Drawing.Font("Dubai", 12F);
            this.lbltele.Location = new System.Drawing.Point(262, 451);
            this.lbltele.Name = "lbltele";
            this.lbltele.Size = new System.Drawing.Size(132, 34);
            this.lbltele.TabIndex = 3;
            this.lbltele.Text = "No.Telphone : ";
            // 
            // txtusername
            // 
            this.txtusername.Font = new System.Drawing.Font("Dubai", 8.25F);
            this.txtusername.Location = new System.Drawing.Point(400, 401);
            this.txtusername.Name = "txtusername";
            this.txtusername.Size = new System.Drawing.Size(209, 31);
            this.txtusername.TabIndex = 4;
            // 
            // txttele
            // 
            this.txttele.Font = new System.Drawing.Font("Dubai", 8.25F);
            this.txttele.Location = new System.Drawing.Point(400, 451);
            this.txttele.Name = "txttele";
            this.txttele.Size = new System.Drawing.Size(209, 31);
            this.txttele.TabIndex = 5;
            // 
            // btnlogin
            // 
            this.btnlogin.BackColor = System.Drawing.SystemColors.ControlLight;
            this.btnlogin.Font = new System.Drawing.Font("Dubai", 8.25F);
            this.btnlogin.Location = new System.Drawing.Point(388, 505);
            this.btnlogin.Name = "btnlogin";
            this.btnlogin.Size = new System.Drawing.Size(98, 32);
            this.btnlogin.TabIndex = 6;
            this.btnlogin.Text = "Login";
            this.btnlogin.UseVisualStyleBackColor = false;
            this.btnlogin.Click += new System.EventHandler(this.btnlogin_Click);
            // 
            // pictureBox1
            // 
            this.pictureBox1.Image = ((System.Drawing.Image)(resources.GetObject("pictureBox1.Image")));
            this.pictureBox1.Location = new System.Drawing.Point(224, -2);
            this.pictureBox1.Name = "pictureBox1";
            this.pictureBox1.Size = new System.Drawing.Size(448, 294);
            this.pictureBox1.TabIndex = 7;
            this.pictureBox1.TabStop = false;
            // 
            // LoginForm
            // 
            this.AutoScaleDimensions = new System.Drawing.SizeF(8F, 16F);
            this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
            this.ClientSize = new System.Drawing.Size(861, 582);
            this.Controls.Add(this.pictureBox1);
            this.Controls.Add(this.btnlogin);
            this.Controls.Add(this.txttele);
            this.Controls.Add(this.txtusername);
            this.Controls.Add(this.lbltele);
            this.Controls.Add(this.lblusername);
            this.Controls.Add(this.lblStatus);
            this.Controls.Add(this.lblTitle);
            this.FormBorderStyle = System.Windows.Forms.FormBorderStyle.FixedSingle;
            this.MaximizeBox = false;
            this.Name = "LoginForm";
            this.StartPosition = System.Windows.Forms.FormStartPosition.CenterScreen;
            this.Text = "Login - Bengkel";
            this.Load += new System.EventHandler(this.LoginForm_Load);
            ((System.ComponentModel.ISupportInitialize)(this.pictureBox1)).EndInit();
            this.ResumeLayout(false);
            this.PerformLayout();

        }

        #endregion

        private System.Windows.Forms.Label lblTitle;
        private System.Windows.Forms.Label lblStatus;
        private System.Windows.Forms.Label lblusername;
        private System.Windows.Forms.Label lbltele;
        private System.Windows.Forms.TextBox txtusername;
        private System.Windows.Forms.TextBox txttele;
        private System.Windows.Forms.Button btnlogin;
        private System.Windows.Forms.PictureBox pictureBox1;
    }
}

