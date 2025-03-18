using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Data;
using MySql.Data.MySqlClient;

namespace ConexionesSGBD
{
    public class ConexionMySQL : IBaseDatos
    {
        private readonly MySqlConnection conexion;

        public ConexionMySQL(string servidor, string usuario, string contraseña)
        {
            string cadenaConexion = $"Server={servidor};Database=mysql;User={usuario};Password={contraseña};";
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

        public List<string> EjecutarConsulta(string consulta)
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

        public Dictionary<string, string> ObtenerAtributos(string tabla)
        {
            Dictionary<string, string> atributos = new Dictionary<string, string>();
            string consulta = $"DESCRIBE {tabla};";

            using ( conexion )
            {
                conexion.Open();
                using (MySqlCommand comando = new MySqlCommand(consulta, conexion))
                using (MySqlDataReader lector = comando.ExecuteReader())
                {
                    while (lector.Read())
                    {
                        atributos[lector["Field"].ToString()] = lector["Type"].ToString();
                    }
                }
            }
            return atributos;
        }

        public List<string> ObtenerTablas()
        {
            List<string> tablas = new List<string>();
            string consulta = "SELECT TABLE_SCHEMA, TABLE_NAME FROM INFORMATION_SCHEMA.TABLES WHERE TABLE_TYPE = 'BASE TABLE';";

            using (MySqlCommand comando = new MySqlCommand(consulta, conexion))
            using (MySqlDataReader lector = comando.ExecuteReader())
            {
                while (lector.Read())
                {
                    string baseDatos = lector["TABLE_SCHEMA"].ToString();
                    string nombreTabla = lector["TABLE_NAME"].ToString();
                    tablas.Add($"{baseDatos}.{nombreTabla}");
                }
            }

            return tablas;
        }


        public List<string> ObtenerBasesDeDatos()
        {
            return EjecutarConsulta("SHOW DATABASES;");
        }

        public List<string> ObtenerVistas()
        {
            return EjecutarConsulta("SELECT TABLE_NAME FROM information_schema.VIEWS WHERE TABLE_SCHEMA = DATABASE();");
        }

        public List<string> ObtenerProcedimientos()
        {
            return EjecutarConsulta("SELECT ROUTINE_NAME FROM information_schema.ROUTINES WHERE ROUTINE_TYPE='PROCEDURE' AND ROUTINE_SCHEMA = DATABASE();");
        }

        public List<string> ObtenerFunciones()
        {
            return EjecutarConsulta("SELECT ROUTINE_NAME FROM information_schema.ROUTINES WHERE ROUTINE_TYPE='FUNCTION' AND ROUTINE_SCHEMA = DATABASE();");
        }

        public List<string> ObtenerTriggers()
        {
            return EjecutarConsulta("SELECT TRIGGER_NAME FROM information_schema.TRIGGERS WHERE TRIGGER_SCHEMA = DATABASE();");
        }

        public List<string> ObtenerTiposDeDatos()
        {
            return EjecutarConsulta("SELECT DISTINCT DATA_TYPE FROM information_schema.COLUMNS WHERE TABLE_SCHEMA = DATABASE();");
        }
        public List<string> ObtenerIndices()
        {
            return EjecutarConsulta("SELECT DISTINCT INDEX_NAME FROM information_schema.statistics WHERE TABLE_SCHEMA = DATABASE();");
        }

        public List<string> ObtenerSecuencias()
        {
            return new List<string>(); // 🔹 MySQL no usa secuencias, utiliza AUTO_INCREMENT en su lugar.
        }
    }
}
