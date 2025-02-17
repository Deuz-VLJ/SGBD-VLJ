using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Data.SqlClient;
using System.Data;

namespace ConexionesSGBD
{
    public class ConexionSQLServer
    {

        private readonly SqlConnection conexion;

        public ConexionSQLServer(string servidor, string baseDatos, string usuario, string contraseña)
        {
            string cadenaConexion = $"Server={servidor};Database={baseDatos};User Id={usuario};Password={contraseña};TrustServerCertificate=True;";
            conexion = new SqlConnection(cadenaConexion);
        }

        // Abrir conexión si está cerrada
        public void AbrirConexion()
        {
            if (conexion.State == ConnectionState.Closed)
            {
                conexion.Open();
            }
        }

        // Cerrar conexión cuando termine la aplicación
        public void CerrarConexion()
        {
            if (conexion.State == ConnectionState.Open)
            {
                conexion.Close();
            }
        }

        // Probar si la conexión funciona
        public bool ProbarConexion()
        {
            try
            {
                AbrirConexion();
                return true;
            }
            catch (Exception)
            {
                return false;
            }
        }

        // Método genérico para ejecutar consultas SQL
        private List<string> EjecutarConsulta(string consulta)
        {
            List<string> resultados = new List<string>();

            try
            {
                AbrirConexion();
                using (SqlCommand cmd = new SqlCommand(consulta, conexion))
                using (SqlDataReader reader = cmd.ExecuteReader())
                {
                    while (reader.Read())
                    {
                        resultados.Add(reader.GetString(0).Trim());
                    }
                }
            }
            catch (Exception ex)
            {
                Console.WriteLine($"Error en consulta: {ex.Message}");
            }

            return resultados;
        }

        // Obtener listas de objetos de la base de datos
        public List<string> ObtenerTablas()
        {
            return EjecutarConsulta("SELECT TABLE_NAME FROM INFORMATION_SCHEMA.TABLES WHERE TABLE_TYPE = 'BASE TABLE';");
        }

        public List<string> ObtenerVistas()
        {
            return EjecutarConsulta("SELECT TABLE_NAME FROM INFORMATION_SCHEMA.VIEWS;");
        }

        public List<string> ObtenerProcedimientos()
        {
            return EjecutarConsulta("SELECT name FROM sys.procedures;");
        }

        public List<string> ObtenerFunciones()
        {
            return EjecutarConsulta("SELECT name FROM sys.objects WHERE type IN ('FN', 'TF');");
        }

        public List<string> ObtenerTriggers()
        {
            return EjecutarConsulta("SELECT name FROM sys.triggers;");
        }

        public List<string> ObtenerTiposDeDatos()
        {
            return EjecutarConsulta("SELECT name FROM sys.types;");
        }

    }
}
