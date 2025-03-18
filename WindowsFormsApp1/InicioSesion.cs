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
        public string GestorSeleccionado { get; private set; }
        public IBaseDatos Conexion { get; private set; } // Nueva propiedad para devolver la conexión

       public string servidor;
      public  string usuario;
       public string contrasena;
      public  string rutaBD;
        public Form1()
        {
            InitializeComponent();
        }

        private void btnConectar_Click(object sender, EventArgs e)
        {
            // Obtener los datos ingresados por el usuario
            servidor = txtServidor.Text.Trim();
            usuario = txtUsuario.Text.Trim();
            contrasena = txtContrasena.Text.Trim();
            rutaBD = txtRutaBD.Text.Trim();

            // 🔹 Asegurar que `GestorSeleccionado` tiene el valor correcto
            if (chkFirebird.Checked)
                GestorSeleccionado = "Firebird";
            else if (chkSqlServer.Checked)
                GestorSeleccionado = "SQL Server";
            else if (chkMySQL.Checked)
                GestorSeleccionado = "MySQL";
            else if (chkPostgreSQL.Checked)
                GestorSeleccionado = "PostgreSQL";
            else if (chkOracle.Checked)
                GestorSeleccionado = "Oracle";
            else
            {
                MessageBox.Show("Seleccione un gestor de base de datos.", "Error", MessageBoxButtons.OK, MessageBoxIcon.Warning);
                return;
            }

            // 🔹 Agregar la marca de tiempo a la conexión
            string nombreConexion = $"{GestorSeleccionado} - {DateTime.Now:HH:mm:ss}";

            // 🔹 Crear la conexión según el gestor seleccionado
            switch (GestorSeleccionado)
            {
                case "Firebird":
                    if (string.IsNullOrEmpty(rutaBD))
                    {
                        MessageBox.Show("Ingrese la ruta de la base de datos para Firebird.", "Error", MessageBoxButtons.OK, MessageBoxIcon.Warning);
                        return;
                    }
                    Conexion = new ConexionFirebird(servidor, rutaBD, usuario, contrasena);
                    break;

                case "SQL Server":
                    Conexion = new ConexionSQLServer(servidor, usuario, contrasena);
                    break;

                case "MySQL":
                    Conexion = new ConexionMySQL(servidor, usuario, contrasena);
                    break;

                case "PostgreSQL":
                    Conexion = new ConexionPostgresSQL(servidor, usuario, contrasena);
                    break;

                case "Oracle":
                    Conexion = new ConexionOracleSQL(servidor, usuario, contrasena);
                    break;

                default:
                    MessageBox.Show("Gestor de base de datos no soportado.", "Error", MessageBoxButtons.OK, MessageBoxIcon.Error);
                    return;
            }

            // 🔹 Verificar conexión antes de continuar
            if (!Conexion.ProbarConexion())
            {
                MessageBox.Show("No se pudo conectar a la base de datos. Verifique los datos ingresados.", "Error", MessageBoxButtons.OK, MessageBoxIcon.Error);
                return;
            }

            // 🔹 Guardar el nombre de la conexión con la hora correcta
            GestorSeleccionado = nombreConexion;

            // 🔹 Mostrar mensaje de éxito y cerrar formulario
            MessageBox.Show($"Conexión exitosa: {nombreConexion}", "Éxito", MessageBoxButtons.OK, MessageBoxIcon.Information);
            this.DialogResult = DialogResult.OK;
            this.Close();
        }

        private void btnCancelar_Click(object sender, EventArgs e)
        {
            this.DialogResult = DialogResult.Cancel;
            this.Close();
        }

        private void Form1_Load(object sender, EventArgs e)
        {

        }
    }
}
