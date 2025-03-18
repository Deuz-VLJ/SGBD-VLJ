namespace WindowsFormsApp1
{
    partial class SgbdMultiBaseDatos
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
            this.btnAgregarConexion = new System.Windows.Forms.Button();
            this.btnEjecutar = new System.Windows.Forms.Button();
            this.txtQuery = new System.Windows.Forms.TextBox();
            this.treeViewBD = new System.Windows.Forms.TreeView();
            this.comboBoxBD = new System.Windows.Forms.ComboBox();
            this.SuspendLayout();
            // 
            // btnAgregarConexion
            // 
            this.btnAgregarConexion.Location = new System.Drawing.Point(13, 7);
            this.btnAgregarConexion.Name = "btnAgregarConexion";
            this.btnAgregarConexion.Size = new System.Drawing.Size(16, 18);
            this.btnAgregarConexion.TabIndex = 9;
            this.btnAgregarConexion.Text = "+";
            this.btnAgregarConexion.UseVisualStyleBackColor = true;
            this.btnAgregarConexion.Click += new System.EventHandler(this.btnAgregarConexion_Click_1);
            // 
            // btnEjecutar
            // 
            this.btnEjecutar.Location = new System.Drawing.Point(217, 409);
            this.btnEjecutar.Name = "btnEjecutar";
            this.btnEjecutar.Size = new System.Drawing.Size(75, 23);
            this.btnEjecutar.TabIndex = 7;
            this.btnEjecutar.Text = "Compilar";
            this.btnEjecutar.UseVisualStyleBackColor = true;
            this.btnEjecutar.Click += new System.EventHandler(this.btnEjecutar_Click);
            // 
            // txtQuery
            // 
            this.txtQuery.Location = new System.Drawing.Point(199, 31);
            this.txtQuery.Multiline = true;
            this.txtQuery.Name = "txtQuery";
            this.txtQuery.Size = new System.Drawing.Size(474, 372);
            this.txtQuery.TabIndex = 6;
            // 
            // treeViewBD
            // 
            this.treeViewBD.Location = new System.Drawing.Point(13, 31);
            this.treeViewBD.Name = "treeViewBD";
            this.treeViewBD.Size = new System.Drawing.Size(158, 402);
            this.treeViewBD.TabIndex = 5;
            this.treeViewBD.AfterSelect += new System.Windows.Forms.TreeViewEventHandler(this.TreeViewBD_AfterSelect_1);
            // 
            // comboBoxBD
            // 
            this.comboBoxBD.FormattingEnabled = true;
            this.comboBoxBD.Location = new System.Drawing.Point(199, 7);
            this.comboBoxBD.Name = "comboBoxBD";
            this.comboBoxBD.Size = new System.Drawing.Size(121, 21);
            this.comboBoxBD.TabIndex = 10;
            // 
            // SgbdMultiBaseDatos
            // 
            this.AutoScaleDimensions = new System.Drawing.SizeF(6F, 13F);
            this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
            this.ClientSize = new System.Drawing.Size(800, 450);
            this.Controls.Add(this.comboBoxBD);
            this.Controls.Add(this.btnAgregarConexion);
            this.Controls.Add(this.btnEjecutar);
            this.Controls.Add(this.txtQuery);
            this.Controls.Add(this.treeViewBD);
            this.Name = "SgbdMultiBaseDatos";
            this.Text = "SgbdMultiBaseDatos";
            this.Load += new System.EventHandler(this.SgbdMultiBaseDatos_Load);
            this.ResumeLayout(false);
            this.PerformLayout();

        }

        #endregion

        private System.Windows.Forms.Button btnAgregarConexion;
        private System.Windows.Forms.Button btnEjecutar;
        private System.Windows.Forms.TextBox txtQuery;
        private System.Windows.Forms.TreeView treeViewBD;
        private System.Windows.Forms.ComboBox comboBoxBD;
    }
}