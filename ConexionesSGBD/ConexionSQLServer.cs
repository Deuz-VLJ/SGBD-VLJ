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


        public List<string> ObtenerTablas(string baseDatos)
        {
            List<string> tablas = new List<string>();
            string consulta = $"USE [{baseDatos}]; SELECT TABLE_NAME FROM INFORMATION_SCHEMA.TABLES WHERE TABLE_TYPE = 'BASE TABLE';";

            using (SqlCommand cmd = new SqlCommand(consulta, conexion))
            using (SqlDataReader reader = cmd.ExecuteReader())
            {
                while (reader.Read())
                {
                    tablas.Add(reader["TABLE_NAME"].ToString());
                }
            }

            return tablas;
        }

        public Dictionary<string, string> ObtenerAtributos(string baseDatos, string tabla)
        {
            Dictionary<string, string> atributos = new Dictionary<string, string>();

            // Obtener columnas y tipos
            string consulta = $"USE [{baseDatos}]; SELECT COLUMN_NAME, DATA_TYPE FROM INFORMATION_SCHEMA.COLUMNS WHERE TABLE_NAME = '{tabla}';";
            using (SqlCommand cmd = new SqlCommand(consulta, conexion))
            using (SqlDataReader reader = cmd.ExecuteReader())
            {
                while (reader.Read())
                {
                    atributos[reader["COLUMN_NAME"].ToString()] = reader["DATA_TYPE"].ToString();
                }
            }

            // Obtener claves primarias
            string pkQuery = $@"
        USE [{baseDatos}];
        SELECT COLUMN_NAME 
        FROM INFORMATION_SCHEMA.KEY_COLUMN_USAGE 
        WHERE OBJECTPROPERTY(OBJECT_ID(CONSTRAINT_SCHEMA + '.' + CONSTRAINT_NAME), 'IsPrimaryKey') = 1 
        AND TABLE_NAME = '{tabla}';";

            List<string> clavesPrimarias = new List<string>();
            using (SqlCommand cmd = new SqlCommand(pkQuery, conexion))
            using (SqlDataReader reader = cmd.ExecuteReader())
            {
                while (reader.Read())
                {
                    clavesPrimarias.Add(reader["COLUMN_NAME"].ToString());
                }
            }

            // Agregar "PK" si corresponde
            foreach (var pk in clavesPrimarias)
            {
                if (atributos.ContainsKey(pk))
                {
                    atributos[pk] += " (PK)";
                }
            }

            return atributos;
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
