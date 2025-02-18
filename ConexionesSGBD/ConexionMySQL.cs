using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Data;
using MySql.Data.MySqlClient;

namespace ConexionesSGBD
{
    public class ConexionMySQL
    {
        private readonly MySqlConnection conexion;

        public ConexionMySQL(string servidor, string baseDatos, string usuario, string contraseña)
        {
            string cadenaConexion = $"Server={servidor};Database={baseDatos};User Id={usuario};Password={contraseña};";
            conexion = new MySqlConnection(cadenaConexion);
        }

        public void AbrirConexion()
        {
            if (conexion.State == ConnectionState.Closed)
            {
                conexion.Open();
            }
        }

        public void CerrarConexion()
        {
            if (conexion.State == ConnectionState.Open)
            {
                conexion.Close();
            }
        }

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

        private List<string> EjecutarConsulta(string consulta)
        {
            List<string> resultados = new List<string>();

            try
            {
                AbrirConexion();
                using (MySqlCommand cmd = new MySqlCommand(consulta, conexion))
                using (MySqlDataReader reader = cmd.ExecuteReader())
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

        public List<string> ObtenerBasesDeDatos()
        {
            return EjecutarConsulta("SHOW DATABASES;");
        }

        public List<string> ObtenerTablas(string baseDatos)
        {
            return EjecutarConsulta($"SELECT TABLE_NAME FROM information_schema.TABLES WHERE TABLE_SCHEMA = '{baseDatos}';");
        }

        public List<string> ObtenerVistas(string baseDatos)
        {
            return EjecutarConsulta($"SELECT TABLE_NAME FROM information_schema.VIEWS WHERE TABLE_SCHEMA = '{baseDatos}';");
        }

        public List<string> ObtenerProcedimientos(string baseDatos)
        {
            return EjecutarConsulta($"SELECT ROUTINE_NAME FROM information_schema.ROUTINES WHERE ROUTINE_TYPE='PROCEDURE' AND ROUTINE_SCHEMA = '{baseDatos}';");
        }

        public List<string> ObtenerFunciones(string baseDatos)
        {
            return EjecutarConsulta($"SELECT ROUTINE_NAME FROM information_schema.ROUTINES WHERE ROUTINE_TYPE='FUNCTION' AND ROUTINE_SCHEMA = '{baseDatos}';");
        }

        public List<string> ObtenerTriggers(string baseDatos)
        {
            return EjecutarConsulta($"SELECT TRIGGER_NAME FROM information_schema.TRIGGERS WHERE TRIGGER_SCHEMA = '{baseDatos}';");
        }

        public List<string> ObtenerTiposDeDatos(string baseDatos)
        {
            return EjecutarConsulta($"SELECT DISTINCT DATA_TYPE FROM information_schema.COLUMNS WHERE TABLE_SCHEMA = '{baseDatos}';");
        }
    }
}
