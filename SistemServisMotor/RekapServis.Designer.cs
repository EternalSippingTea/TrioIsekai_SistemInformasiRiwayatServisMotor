namespace SistemServisMotor
{
    partial class RekapServis
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
            this.lblTitle = new System.Windows.Forms.Label();
            this.lblCari = new System.Windows.Forms.Label();
            this.txtCari = new System.Windows.Forms.TextBox();
            this.btnLoad = new System.Windows.Forms.Button();
            this.lblFilterTgl = new System.Windows.Forms.Label();
            this.chkPakaiTanggal = new System.Windows.Forms.CheckBox();
            this.dtpDari = new System.Windows.Forms.DateTimePicker();
            this.dtpSampai = new System.Windows.Forms.DateTimePicker();
            this.lblDari = new System.Windows.Forms.Label();
            this.lblSampai = new System.Windows.Forms.Label();
            this.dgvPreview = new System.Windows.Forms.DataGridView();
            this.lblTotal = new System.Windows.Forms.Label();
            this.lblPreview = new System.Windows.Forms.Label();
            this.btnPrint = new System.Windows.Forms.Button();
            this.btnBack = new System.Windows.Forms.Button();
            ((System.ComponentModel.ISupportInitialize)(this.dgvPreview)).BeginInit();
            this.SuspendLayout();
            // 
            // lblTitle
            // 
            this.lblTitle.AutoSize = true;
            this.lblTitle.Font = new System.Drawing.Font("Microsoft Sans Serif", 15F, System.Drawing.FontStyle.Bold, System.Drawing.GraphicsUnit.Point, ((byte)(128)));
            this.lblTitle.Location = new System.Drawing.Point(298, 40);
            this.lblTitle.Name = "lblTitle";
            this.lblTitle.Size = new System.Drawing.Size(443, 29);
            this.lblTitle.TabIndex = 0;
            this.lblTitle.Text = "REKAP RIWAYAT SERVIS MOTOR";
            // 
            // lblCari
            // 
            this.lblCari.AutoSize = true;
            this.lblCari.Font = new System.Drawing.Font("Microsoft Sans Serif", 12F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(128)));
            this.lblCari.Location = new System.Drawing.Point(277, 101);
            this.lblCari.Name = "lblCari";
            this.lblCari.Size = new System.Drawing.Size(166, 25);
            this.lblCari.TabIndex = 1;
            this.lblCari.Text = "Cari Kendaraan : ";
            // 
            // txtCari
            // 
            this.txtCari.BackColor = System.Drawing.SystemColors.HighlightText;
            this.txtCari.ForeColor = System.Drawing.Color.Silver;
            this.txtCari.Location = new System.Drawing.Point(450, 103);
            this.txtCari.Name = "txtCari";
            this.txtCari.Size = new System.Drawing.Size(189, 22);
            this.txtCari.TabIndex = 2;
            this.txtCari.Text = "Input Plat Nomer";
            // 
            // btnLoad
            // 
            this.btnLoad.Anchor = ((System.Windows.Forms.AnchorStyles)((((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Bottom) 
            | System.Windows.Forms.AnchorStyles.Left) 
            | System.Windows.Forms.AnchorStyles.Right)));
            this.btnLoad.BackColor = System.Drawing.SystemColors.GradientActiveCaption;
            this.btnLoad.Location = new System.Drawing.Point(670, 102);
            this.btnLoad.Name = "btnLoad";
            this.btnLoad.Size = new System.Drawing.Size(98, 23);
            this.btnLoad.TabIndex = 3;
            this.btnLoad.Text = "Search";
            this.btnLoad.UseVisualStyleBackColor = false;
            this.btnLoad.Click += new System.EventHandler(this.btnLoad_Click);
            // 
            // lblFilterTgl
            // 
            this.lblFilterTgl.AutoSize = true;
            this.lblFilterTgl.Font = new System.Drawing.Font("Microsoft Sans Serif", 12F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(128)));
            this.lblFilterTgl.Location = new System.Drawing.Point(355, 152);
            this.lblFilterTgl.Name = "lblFilterTgl";
            this.lblFilterTgl.Size = new System.Drawing.Size(147, 25);
            this.lblFilterTgl.TabIndex = 4;
            this.lblFilterTgl.Text = "Filter Tanggal : ";
            // 
            // chkPakaiTanggal
            // 
            this.chkPakaiTanggal.AutoSize = true;
            this.chkPakaiTanggal.Location = new System.Drawing.Point(331, 155);
            this.chkPakaiTanggal.Name = "chkPakaiTanggal";
            this.chkPakaiTanggal.Size = new System.Drawing.Size(18, 17);
            this.chkPakaiTanggal.TabIndex = 5;
            this.chkPakaiTanggal.UseVisualStyleBackColor = true;
            this.chkPakaiTanggal.Click += new System.EventHandler(this.chkPakaiTanggal_CheckedChanged);
            // 
            // dtpDari
            // 
            this.dtpDari.Format = System.Windows.Forms.DateTimePickerFormat.Short;
            this.dtpDari.Location = new System.Drawing.Point(589, 155);
            this.dtpDari.Name = "dtpDari";
            this.dtpDari.Size = new System.Drawing.Size(116, 22);
            this.dtpDari.TabIndex = 6;
            // 
            // dtpSampai
            // 
            this.dtpSampai.Format = System.Windows.Forms.DateTimePickerFormat.Short;
            this.dtpSampai.Location = new System.Drawing.Point(589, 200);
            this.dtpSampai.Name = "dtpSampai";
            this.dtpSampai.Size = new System.Drawing.Size(116, 22);
            this.dtpSampai.TabIndex = 7;
            // 
            // lblDari
            // 
            this.lblDari.AutoSize = true;
            this.lblDari.Font = new System.Drawing.Font("Microsoft Sans Serif", 12F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(128)));
            this.lblDari.Location = new System.Drawing.Point(527, 152);
            this.lblDari.Name = "lblDari";
            this.lblDari.Size = new System.Drawing.Size(58, 25);
            this.lblDari.TabIndex = 8;
            this.lblDari.Text = "Dari: ";
            this.lblDari.Click += new System.EventHandler(this.lblDari_Click);
            // 
            // lblSampai
            // 
            this.lblSampai.AutoSize = true;
            this.lblSampai.Font = new System.Drawing.Font("Microsoft Sans Serif", 12F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(128)));
            this.lblSampai.Location = new System.Drawing.Point(495, 198);
            this.lblSampai.Name = "lblSampai";
            this.lblSampai.Size = new System.Drawing.Size(90, 25);
            this.lblSampai.TabIndex = 9;
            this.lblSampai.Text = "Sampai: ";
            // 
            // dgvPreview
            // 
            this.dgvPreview.ColumnHeadersHeightSizeMode = System.Windows.Forms.DataGridViewColumnHeadersHeightSizeMode.AutoSize;
            this.dgvPreview.Location = new System.Drawing.Point(12, 248);
            this.dgvPreview.Name = "dgvPreview";
            this.dgvPreview.RowHeadersWidth = 51;
            this.dgvPreview.RowTemplate.Height = 24;
            this.dgvPreview.Size = new System.Drawing.Size(996, 341);
            this.dgvPreview.TabIndex = 10;
            // 
            // lblTotal
            // 
            this.lblTotal.AutoSize = true;
            this.lblTotal.Font = new System.Drawing.Font("Microsoft Sans Serif", 12F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(128)));
            this.lblTotal.Location = new System.Drawing.Point(12, 615);
            this.lblTotal.Name = "lblTotal";
            this.lblTotal.Size = new System.Drawing.Size(143, 25);
            this.lblTotal.TabIndex = 11;
            this.lblTotal.Text = "Total : 0 Servis";
            // 
            // lblPreview
            // 
            this.lblPreview.AutoSize = true;
            this.lblPreview.Font = new System.Drawing.Font("Microsoft Sans Serif", 12F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(128)));
            this.lblPreview.Location = new System.Drawing.Point(12, 210);
            this.lblPreview.Name = "lblPreview";
            this.lblPreview.Size = new System.Drawing.Size(97, 25);
            this.lblPreview.TabIndex = 12;
            this.lblPreview.Text = "Preview : ";
            // 
            // btnPrint
            // 
            this.btnPrint.BackColor = System.Drawing.SystemColors.ControlLight;
            this.btnPrint.Location = new System.Drawing.Point(876, 619);
            this.btnPrint.Name = "btnPrint";
            this.btnPrint.Size = new System.Drawing.Size(98, 23);
            this.btnPrint.TabIndex = 13;
            this.btnPrint.Text = "Print";
            this.btnPrint.UseVisualStyleBackColor = false;
            this.btnPrint.Click += new System.EventHandler(this.btnPrint_Click);
            // 
            // btnBack
            // 
            this.btnBack.BackColor = System.Drawing.Color.MistyRose;
            this.btnBack.Location = new System.Drawing.Point(761, 619);
            this.btnBack.Name = "btnBack";
            this.btnBack.Size = new System.Drawing.Size(98, 23);
            this.btnBack.TabIndex = 14;
            this.btnBack.Text = "Back";
            this.btnBack.UseVisualStyleBackColor = false;
            this.btnBack.Click += new System.EventHandler(this.btnBack_Click);
            // 
            // RekapServis
            // 
            this.AutoScaleDimensions = new System.Drawing.SizeF(8F, 16F);
            this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
            this.ClientSize = new System.Drawing.Size(1020, 667);
            this.Controls.Add(this.btnBack);
            this.Controls.Add(this.btnPrint);
            this.Controls.Add(this.lblPreview);
            this.Controls.Add(this.lblTotal);
            this.Controls.Add(this.dgvPreview);
            this.Controls.Add(this.lblSampai);
            this.Controls.Add(this.lblDari);
            this.Controls.Add(this.dtpSampai);
            this.Controls.Add(this.dtpDari);
            this.Controls.Add(this.chkPakaiTanggal);
            this.Controls.Add(this.lblFilterTgl);
            this.Controls.Add(this.btnLoad);
            this.Controls.Add(this.txtCari);
            this.Controls.Add(this.lblCari);
            this.Controls.Add(this.lblTitle);
            this.Name = "RekapServis";
            this.StartPosition = System.Windows.Forms.FormStartPosition.CenterScreen;
            this.Text = "RekapServis";
            this.Load += new System.EventHandler(this.RekapServis_Load);
            ((System.ComponentModel.ISupportInitialize)(this.dgvPreview)).EndInit();
            this.ResumeLayout(false);
            this.PerformLayout();

        }

        #endregion

        private System.Windows.Forms.Label lblTitle;
        private System.Windows.Forms.Label lblCari;
        private System.Windows.Forms.TextBox txtCari;
        private System.Windows.Forms.Button btnLoad;
        private System.Windows.Forms.Label lblFilterTgl;
        private System.Windows.Forms.CheckBox chkPakaiTanggal;
        private System.Windows.Forms.DateTimePicker dtpDari;
        private System.Windows.Forms.DateTimePicker dtpSampai;
        private System.Windows.Forms.Label lblDari;
        private System.Windows.Forms.Label lblSampai;
        private System.Windows.Forms.DataGridView dgvPreview;
        private System.Windows.Forms.Label lblTotal;
        private System.Windows.Forms.Label lblPreview;
        private System.Windows.Forms.Button btnPrint;
        private System.Windows.Forms.Button btnBack;
    }
}