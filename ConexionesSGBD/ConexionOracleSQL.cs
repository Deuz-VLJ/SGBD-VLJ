using System;
using System.Collections.Generic;
using System.Data;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Oracle.ManagedDataAccess.Client;

namespace ConexionesSGBD
{
    public class ConexionOracleSQL : IBaseDatos
    {
        private readonly OracleConnection conexion;
        //$"Data Source=(DESCRIPTION=(ADDRESS=(PROTOCOL=TCP)(HOST={servidor})(PORT=1521))(CONNECT_DATA=(SID={baseDatos})));User Id={usuario};Password={contraseña};"
        public ConexionOracleSQL(string servidor,  string usuario, string contraseña)
        {
            string cadenaConexion = $"Data Source=(DESCRIPTION=(ADDRESS=(PROTOCOL=TCP)(HOST={servidor})(PORT=1521))(CONNECT_DATA=()));User Id={usuario};Password={contraseña};"
;
            conexion = new OracleConnection(cadenaConexion);
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
                using (OracleCommand cmd = new OracleCommand(consulta, conexion))
                using (OracleDataReader reader = cmd.ExecuteReader())
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
            string consulta = $"SELECT COLUMN_NAME, DATA_TYPE FROM ALL_TAB_COLUMNS WHERE TABLE_NAME = '{tabla}' AND OWNER = (SELECT USER FROM dual);";

            using (conexion)
            {
                conexion.Open();
                using (OracleCommand comando = new OracleCommand(consulta, conexion))
                using (OracleDataReader lector = comando.ExecuteReader())
                {
                    while (lector.Read())
                    {
                        atributos[lector["COLUMN_NAME"].ToString()] = lector["DATA_TYPE"].ToString();
                    }
                }
            }
            return atributos;
        }
        public List<string> ObtenerBasesDeDatos()
        {
            return EjecutarConsulta("SELECT NAME FROM v$database");
        }
        public List<string> ObtenerTablas()
        {
            List<string> tablas = new List<string>();
            string consulta = "SELECT OWNER, TABLE_NAME FROM ALL_TABLES;";

            using (OracleCommand comando = new OracleCommand(consulta, conexion))
            using (OracleDataReader lector = comando.ExecuteReader())
            {
                while (lector.Read())
                {
                    string baseDatos = lector["OWNER"].ToString();
                    string nombreTabla = lector["TABLE_NAME"].ToString();
                    tablas.Add($"{baseDatos}.{nombreTabla}");
                }
            }

            return tablas;
        }

        public List<string> ObtenerVistas()
        {
            return EjecutarConsulta("SELECT view_name FROM all_views WHERE owner = USER;");
        }

        public List<string> ObtenerProcedimientos()
        {
            return EjecutarConsulta("SELECT object_name FROM all_procedures WHERE owner = USER;");
        }

        public List<string> ObtenerFunciones()
        {
            return EjecutarConsulta("SELECT object_name FROM all_objects WHERE object_type = 'FUNCTION' AND owner = USER;");
        }

        public List<string> ObtenerTriggers()
        {
            return EjecutarConsulta("SELECT trigger_name FROM all_triggers WHERE owner = USER;");
        }

        public List<string> ObtenerTiposDeDatos()
        {
            return EjecutarConsulta("SELECT DISTINCT data_type FROM all_tab_columns WHERE owner = USER;");
        }

        public List<string> ObtenerIndices()
        {
            return EjecutarConsulta("SELECT index_name FROM all_indexes WHERE owner = (SELECT USER FROM dual);");
        }

        public List<string> ObtenerSecuencias()
        {
            return EjecutarConsulta("SELECT sequence_name FROM all_sequences WHERE sequence_owner = (SELECT USER FROM dual);");
        }
    }
}
