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

        public Dictionary<string, string> ObtenerAtributos(string baseDatos, string tabla)
        {
            Dictionary<string, string> atributos = new Dictionary<string, string>();
            string consulta = $@"
        SELECT COLUMN_NAME, DATA_TYPE,
        CASE WHEN COLUMN_KEY = 'PRI' THEN 'PK' ELSE '' END AS KEY_TYPE
        FROM INFORMATION_SCHEMA.COLUMNS
        WHERE TABLE_SCHEMA = '{baseDatos}' AND TABLE_NAME = '{tabla}';";

            try
            {
                AbrirConexion();
                using (MySqlCommand comando = new MySqlCommand(consulta, conexion))
                using (MySqlDataReader lector = comando.ExecuteReader())
                {
                    while (lector.Read())
                    {
                        string columna = lector["COLUMN_NAME"].ToString();
                        string tipo = lector["DATA_TYPE"].ToString();
                        string key = lector["KEY_TYPE"].ToString();
                        atributos[columna] = string.IsNullOrEmpty(key) ? tipo : $"{tipo} {key}";
                    }
                }
            }
            catch (Exception ex)
            {
                atributos.Clear();
                atributos["Error"] = ex.Message;
            }

            return atributos;
        }


        public List<string> ObtenerTablas(string baseDatos)
        {
            List<string> tablas = new List<string>();
            string consulta = $"SELECT TABLE_NAME FROM INFORMATION_SCHEMA.TABLES WHERE TABLE_SCHEMA = '{baseDatos}' AND TABLE_TYPE = 'BASE TABLE';";

            try
            {
                AbrirConexion();
                using (MySqlCommand comando = new MySqlCommand(consulta, conexion))
                using (MySqlDataReader lector = comando.ExecuteReader())
                {
                    while (lector.Read())
                    {
                        tablas.Add(lector.GetString(0));
                    }
                }
            }
            catch (Exception ex)
            {
                tablas.Add($"Error: {ex.Message}");
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



        public void CambiarBaseDatos(string nuevaBD)
        {
            try
            {
                if (conexion.State != ConnectionState.Open)
                {
                    conexion.Open();
                }

                conexion.ChangeDatabase(nuevaBD);
            }
            catch (Exception ex)
            {
                throw new Exception($"Error al cambiar de base de datos en MySQL: {ex.Message}");
            }
        }



    }
}



