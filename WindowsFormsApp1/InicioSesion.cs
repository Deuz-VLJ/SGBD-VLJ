using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;
using ConexionesSGBD;

namespace WindowsFormsApp1
{
    public partial class Form1 : Form
    {
        public Form1()
        {
            InitializeComponent();
        }



        private void BttConexionFB_Click(object sender, EventArgs e)
        {
            string servidor = txtServidor.Text; // Dirección IP del servidor Firebird
            string rutaBD = txtRuta.Text; // Ruta de la base de datos
            string usuario = txtUsuario.Text; // Usuario de la base de datos
            string contraseña = txtPassword.Text; // Respectiva contraseña del Usuario

            
            if (checkBox1.Checked == true) 
            {
                ConexionFirebird conexion = new ConexionFirebird(servidor, rutaBD, usuario, contraseña);
                if (conexion.ProbarConexion())
                {
                    MessageBox.Show("✅ Conexión exitosa a Firebird.");
                    SgbdFirebird form2 = new SgbdFirebird(conexion);
                    form2.Show();
                    this.Hide();
                }
                else
                {
                    MessageBox.Show("❌ Error de conexión.");
                }
            }
            if (checkBox2.Checked == true) 
            {
                 ConexionSQLServer conexion = new ConexionSQLServer(servidor, rutaBD, usuario, contraseña);
                if (conexion.ProbarConexion())
                {
                    MessageBox.Show("✅ Conexión exitosa a SQLServer.");
                    SgbdSQLServer form3 = new SgbdSQLServer(conexion);  
                    form3.Show();
                    this.Hide();
                }
                else
                {
                    MessageBox.Show("❌ Error de conexión.");
                }
            }
            if (checkBox3.Checked == true)
            {
                ConexionMySQL conexion = new ConexionMySQL(servidor, rutaBD, usuario, contraseña);
                if (conexion.ProbarConexion())
                {
                    MessageBox.Show("✅ Conexión exitosa a MySQL.");
                    SgbdMySQL form4 = new SgbdMySQL(conexion);
                    form4.Show();
                    this.Hide();
                }
                else
                {
                    MessageBox.Show("❌ Error de conexión.");
                }
            }
            if (checkBox4.Checked == true)
            {
                ConexionPostgresSQL conexion = new ConexionPostgresSQL(servidor, usuario, contraseña);
                if (conexion.ProbarConexion())
                {
                    MessageBox.Show("✅ Conexión exitosa a PosgreSQL.");
                    SgbdPostgresSQL form5 = new SgbdPostgresSQL(conexion);
                    form5.Show();
                    this.Hide();
                }
                else
                {
                    MessageBox.Show("❌ Error de conexión.");
                }
            }
            if (checkBox5.Checked == true)
            {
                ConexionOracleSQL conexion = new ConexionOracleSQL(servidor, rutaBD, usuario, contraseña);
                if (conexion.ProbarConexion())
                {
                    MessageBox.Show("✅ Conexión exitosa a ORACLESQL.");
                    SgbdOracleSQL form5 = new SgbdOracleSQL(conexion);
                    form5.Show();
                    this.Hide();
                }
                else
                {
                    MessageBox.Show("❌ Error de conexión.");
                }
            }


        }

        private void checkBox1_CheckedChanged(object sender, EventArgs e)
        {
            if (checkBox1.Checked)
            {
                // Desmarcar y deshabilitar los otros CheckBox
                checkBox2.Checked = false;
                checkBox3.Checked = false;
                checkBox4.Checked = false;
                checkBox5.Checked = false;
            }
        }

        private void checkBox2_CheckedChanged(object sender, EventArgs e)
        {
            if (checkBox2.Checked)
            {
                // Desmarcar y deshabilitar los otros CheckBox
                checkBox1.Checked = false;
                checkBox3.Checked = false;
                checkBox4.Checked = false;
                checkBox5.Checked = false;
            }
        }

        private void checkBox3_CheckedChanged(object sender, EventArgs e)
        {
            if (checkBox3.Checked)
            {
                // Desmarcar y deshabilitar los otros CheckBox
                checkBox1.Checked = false;
                checkBox2.Checked = false;
                checkBox4.Checked = false;
                checkBox5.Checked = false;

            }
        }

        private void checkBox4_CheckedChanged(object sender, EventArgs e)
        {
            if (checkBox4.Checked)
            {
                // Desmarcar y deshabilitar los otros CheckBox
                checkBox1.Checked = false;
                checkBox2.Checked = false;
                checkBox3.Checked = false;
                checkBox5.Checked = false;
            }
        }

        private void checkBox5_CheckedChanged(object sender, EventArgs e)
        {
            if (checkBox5.Checked)
            {
                // Desmarcar y deshabilitar los otros CheckBox
                checkBox1.Checked = false;
                checkBox2.Checked = false;
                checkBox3.Checked = false;
                checkBox4.Checked = false;
            }
        }

        private void Form1_Load(object sender, EventArgs e)
        {
            checkBox1.Checked = true;
        }
    }
}
