﻿
namespace MCatalogos.Views.UserControls.Rotas
{
    partial class RotasEditUC
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

        #region Component Designer generated code

        /// <summary> 
        /// Required method for Designer support - do not modify 
        /// the contents of this method with the code editor.
        /// </summary>
        private void InitializeComponent()
        {
            this.cbNumeroRota = new System.Windows.Forms.ComboBox();
            this.cbLetraRota = new System.Windows.Forms.ComboBox();
            this.textNomeVendedora = new System.Windows.Forms.TextBox();
            this.label3 = new System.Windows.Forms.Label();
            this.label4 = new System.Windows.Forms.Label();
            this.label2 = new System.Windows.Forms.Label();
            this.panelContainer = new System.Windows.Forms.Panel();
            this.btnAddLetraRota = new System.Windows.Forms.Button();
            this.btnAddNumRota = new System.Windows.Forms.Button();
            this.panelContainer.SuspendLayout();
            this.SuspendLayout();
            // 
            // cbNumeroRota
            // 
            this.cbNumeroRota.FormattingEnabled = true;
            this.cbNumeroRota.Location = new System.Drawing.Point(221, 69);
            this.cbNumeroRota.Name = "cbNumeroRota";
            this.cbNumeroRota.Size = new System.Drawing.Size(61, 22);
            this.cbNumeroRota.TabIndex = 5;
            this.cbNumeroRota.SelectionChangeCommitted += new System.EventHandler(this.cbNumeroRota_SelectionChangeCommitted);
            // 
            // cbLetraRota
            // 
            this.cbLetraRota.FormattingEnabled = true;
            this.cbLetraRota.Location = new System.Drawing.Point(51, 69);
            this.cbLetraRota.Name = "cbLetraRota";
            this.cbLetraRota.Size = new System.Drawing.Size(61, 22);
            this.cbLetraRota.TabIndex = 5;
            this.cbLetraRota.SelectionChangeCommitted += new System.EventHandler(this.cbLetraRota_SelectionChangeCommitted);
            // 
            // textNomeVendedora
            // 
            this.textNomeVendedora.BackColor = System.Drawing.Color.DeepSkyBlue;
            this.textNomeVendedora.Location = new System.Drawing.Point(7, 28);
            this.textNomeVendedora.Name = "textNomeVendedora";
            this.textNomeVendedora.ReadOnly = true;
            this.textNomeVendedora.Size = new System.Drawing.Size(296, 22);
            this.textNomeVendedora.TabIndex = 4;
            // 
            // label3
            // 
            this.label3.AutoSize = true;
            this.label3.Location = new System.Drawing.Point(8, 73);
            this.label3.Name = "label3";
            this.label3.Size = new System.Drawing.Size(37, 14);
            this.label3.TabIndex = 2;
            this.label3.Text = "Letra:";
            // 
            // label4
            // 
            this.label4.AutoSize = true;
            this.label4.Location = new System.Drawing.Point(162, 73);
            this.label4.Name = "label4";
            this.label4.Size = new System.Drawing.Size(53, 14);
            this.label4.TabIndex = 3;
            this.label4.Text = "Número:";
            // 
            // label2
            // 
            this.label2.AutoSize = true;
            this.label2.Location = new System.Drawing.Point(4, 10);
            this.label2.Name = "label2";
            this.label2.Size = new System.Drawing.Size(69, 14);
            this.label2.TabIndex = 1;
            this.label2.Text = "Vendedora:";
            // 
            // panelContainer
            // 
            this.panelContainer.BackColor = System.Drawing.Color.FromArgb(((int)(((byte)(0)))), ((int)(((byte)(111)))), ((int)(((byte)(156)))));
            this.panelContainer.Controls.Add(this.textNomeVendedora);
            this.panelContainer.Controls.Add(this.btnAddLetraRota);
            this.panelContainer.Controls.Add(this.label4);
            this.panelContainer.Controls.Add(this.label2);
            this.panelContainer.Controls.Add(this.btnAddNumRota);
            this.panelContainer.Controls.Add(this.label3);
            this.panelContainer.Controls.Add(this.cbNumeroRota);
            this.panelContainer.Controls.Add(this.cbLetraRota);
            this.panelContainer.Dock = System.Windows.Forms.DockStyle.Fill;
            this.panelContainer.ForeColor = System.Drawing.Color.White;
            this.panelContainer.Location = new System.Drawing.Point(0, 0);
            this.panelContainer.Name = "panelContainer";
            this.panelContainer.Size = new System.Drawing.Size(323, 146);
            this.panelContainer.TabIndex = 39;
            // 
            // btnAddLetraRota
            // 
            this.btnAddLetraRota.BackColor = System.Drawing.Color.LightSteelBlue;
            this.btnAddLetraRota.FlatAppearance.BorderSize = 0;
            this.btnAddLetraRota.FlatStyle = System.Windows.Forms.FlatStyle.Flat;
            this.btnAddLetraRota.ForeColor = System.Drawing.Color.White;
            this.btnAddLetraRota.Image = global::MCatalogos.Properties.Resources.IconAdd10x10;
            this.btnAddLetraRota.Location = new System.Drawing.Point(113, 70);
            this.btnAddLetraRota.Name = "btnAddLetraRota";
            this.btnAddLetraRota.Size = new System.Drawing.Size(20, 20);
            this.btnAddLetraRota.TabIndex = 37;
            this.btnAddLetraRota.TabStop = false;
            this.btnAddLetraRota.Tag = "";
            this.btnAddLetraRota.TextAlign = System.Drawing.ContentAlignment.MiddleRight;
            this.btnAddLetraRota.UseVisualStyleBackColor = false;
            this.btnAddLetraRota.Click += new System.EventHandler(this.btnAddLetraRota_Click);
            // 
            // btnAddNumRota
            // 
            this.btnAddNumRota.BackColor = System.Drawing.Color.LightSteelBlue;
            this.btnAddNumRota.FlatAppearance.BorderSize = 0;
            this.btnAddNumRota.FlatStyle = System.Windows.Forms.FlatStyle.Flat;
            this.btnAddNumRota.ForeColor = System.Drawing.Color.White;
            this.btnAddNumRota.Image = global::MCatalogos.Properties.Resources.IconAdd10x10;
            this.btnAddNumRota.Location = new System.Drawing.Point(283, 70);
            this.btnAddNumRota.Name = "btnAddNumRota";
            this.btnAddNumRota.Size = new System.Drawing.Size(20, 20);
            this.btnAddNumRota.TabIndex = 37;
            this.btnAddNumRota.TabStop = false;
            this.btnAddNumRota.Tag = "";
            this.btnAddNumRota.TextAlign = System.Drawing.ContentAlignment.MiddleRight;
            this.btnAddNumRota.UseVisualStyleBackColor = false;
            this.btnAddNumRota.Click += new System.EventHandler(this.btnAddNumRota_Click);
            // 
            // RotasEditUC
            // 
            this.AutoScaleDimensions = new System.Drawing.SizeF(6F, 14F);
            this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
            this.Controls.Add(this.panelContainer);
            this.Font = new System.Drawing.Font("Calibri", 9F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.Name = "RotasEditUC";
            this.Size = new System.Drawing.Size(323, 146);
            this.Load += new System.EventHandler(this.RotasEditUC_Load);
            this.panelContainer.ResumeLayout(false);
            this.panelContainer.PerformLayout();
            this.ResumeLayout(false);

        }

        #endregion
        private System.Windows.Forms.Button btnAddLetraRota;
        private System.Windows.Forms.Button btnAddNumRota;
        private System.Windows.Forms.ComboBox cbNumeroRota;
        private System.Windows.Forms.ComboBox cbLetraRota;
        public System.Windows.Forms.TextBox textNomeVendedora;
        private System.Windows.Forms.Label label3;
        private System.Windows.Forms.Label label4;
        private System.Windows.Forms.Label label2;
        private System.Windows.Forms.Panel panelContainer;
    }
}
