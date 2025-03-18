using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Data.SqlClient;
using System.Data;

namespace ConexionesSGBD
{
    public class ConexionSQLServer : IBaseDatos
    {

        private readonly SqlConnection conexion;

        public ConexionSQLServer(string servidor, string usuario, string contraseña)
        {
            string cadenaConexion = $"Server={servidor};Database=master;User Id={usuario};Password={contraseña};MultipleActiveResultSets=True;";
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
        public List<string> EjecutarConsulta(string consulta)
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

        public Dictionary<string, string> ObtenerAtributos(string tabla)
        {
            Dictionary<string, string> atributos = new Dictionary<string, string>();
            string consulta = $"SELECT COLUMN_NAME, DATA_TYPE FROM INFORMATION_SCHEMA.COLUMNS WHERE TABLE_NAME = '{tabla}';";

            try
            {
                if (conexion.State == System.Data.ConnectionState.Closed)
                {
                    conexion.Open();
                }

                using (SqlCommand comando = new SqlCommand(consulta, conexion))
                using (SqlDataReader lector = comando.ExecuteReader())
                {
                    while (lector.Read())
                    {
                        atributos[lector["COLUMN_NAME"].ToString()] = lector["DATA_TYPE"].ToString();
                    }
                }
            }
            catch (Exception ex)
            {
                throw new Exception($"Error en ObtenerAtributos() para la tabla '{tabla}': {ex.Message}");
            }

            return atributos;
        }

        // Obtener listas de objetos de la base de datos actual
        public List<string> ObtenerTablas()
        {
            List<string> tablas = new List<string>();
            string consultaBases = "SELECT name FROM sys.databases WHERE state_desc = 'ONLINE' AND name NOT IN ('master', 'tempdb', 'model', 'msdb');";

            List<string> basesDeDatos = new List<string>();

            // 🔹 PRIMERO: Obtener la lista de bases de datos
            using (SqlCommand comandoBases = new SqlCommand(consultaBases, conexion))
            using (SqlDataReader lectorBases = comandoBases.ExecuteReader())
            {
                while (lectorBases.Read())
                {
                    basesDeDatos.Add(lectorBases["name"].ToString());
                }
            } // 🔹 Cierra el lector DESPUÉS de terminar de leer todas las bases de datos

            // 🔹 SEGUNDO: Recorrer cada base de datos y obtener sus tablas
            foreach (var baseDatos in basesDeDatos)
            {
                string consultaTablas = $"USE [{baseDatos}]; SELECT '{baseDatos}' + '.' + TABLE_NAME FROM INFORMATION_SCHEMA.TABLES WHERE TABLE_TYPE = 'BASE TABLE';";

                using (SqlCommand comandoTablas = new SqlCommand(consultaTablas, conexion))
                using (SqlDataReader lectorTablas = comandoTablas.ExecuteReader())
                {
                    while (lectorTablas.Read())
                    {
                        tablas.Add(lectorTablas[0].ToString());
                    }
                }
            }

            return tablas;
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

        public List<string> ObtenerIndices()
        {
            return EjecutarConsulta("SELECT name FROM sys.indexes WHERE is_primary_key = 0 AND is_unique = 0;");
        }

        public List<string> ObtenerSecuencias()
        {
            return EjecutarConsulta("SELECT name FROM sys.sequences;");
        }

        // Obtener bases de datos en el servidor
        public List<string> ObtenerBasesDeDatos()
        {
            return EjecutarConsulta("SELECT name FROM sys.databases;");
        }

    }
}
