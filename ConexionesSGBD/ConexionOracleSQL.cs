using System;
using System.Collections.Generic;
using System.Data;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Npgsql;
using Oracle.ManagedDataAccess.Client;

namespace ConexionesSGBD
{
    public class ConexionOracleSQL : IBaseDatos
    {
        private readonly OracleConnection conexion;
        //$"Data Source=(DESCRIPTION=(ADDRESS=(PROTOCOL=TCP)(HOST={servidor})(PORT=1521))(CONNECT_DATA=(SID={baseDatos})));User Id={usuario};Password={contraseña};"
        public ConexionOracleSQL(string servidor, string usuario, string contraseña)
        {
            string cadenaConexion = $"Data Source=(DESCRIPTION=(ADDRESS=(PROTOCOL=TCP)(HOST={servidor})(PORT=1521))(CONNECT_DATA=(SERVICE_NAME=XE)));User Id={usuario};Password={contraseña};";
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

        public Dictionary<string, string> ObtenerAtributos(string esquema, string tabla)
        {
            Dictionary<string, string> atributos = new Dictionary<string, string>();

            string consulta = $@"
        SELECT cols.COLUMN_NAME, cols.DATA_TYPE,
               CASE 
                   WHEN cons.CONSTRAINT_TYPE = 'P' THEN 'PK'
                   ELSE ''
               END AS KEY_TYPE
        FROM ALL_TAB_COLUMNS cols
        LEFT JOIN ALL_CONS_COLUMNS ccols 
            ON cols.OWNER = ccols.OWNER AND cols.TABLE_NAME = ccols.TABLE_NAME AND cols.COLUMN_NAME = ccols.COLUMN_NAME
        LEFT JOIN ALL_CONSTRAINTS cons 
            ON ccols.OWNER = cons.OWNER AND ccols.CONSTRAINT_NAME = cons.CONSTRAINT_NAME
        WHERE cols.OWNER = '{esquema.ToUpper()}' AND cols.TABLE_NAME = '{tabla.ToUpper()}'
        ORDER BY cols.COLUMN_ID";

            try
            {
                AbrirConexion();
                using (OracleCommand comando = new OracleCommand(consulta, conexion))
                using (OracleDataReader lector = comando.ExecuteReader())
                {
                    while (lector.Read())
                    {
                        string nombre = lector["COLUMN_NAME"].ToString();
                        string tipo = lector["DATA_TYPE"].ToString();
                        string key = lector["KEY_TYPE"].ToString();

                        atributos[nombre] = string.IsNullOrEmpty(key) ? tipo : $"{tipo} {key}";
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


        public List<string> ObtenerBasesDeDatos()
        {
            List<string> basesDeDatos = new List<string>();
            string consulta = "SELECT DISTINCT OWNER FROM ALL_TABLES ORDER BY OWNER";

            try
            {
                if (conexion.State == ConnectionState.Closed)
                {
                    conexion.Open();
                }

                using (OracleCommand comando = new OracleCommand(consulta, conexion))
                using (OracleDataReader lector = comando.ExecuteReader())
                {
                    while (lector.Read())
                    {
                        basesDeDatos.Add(lector.GetString(0));
                    }
                }
            }
            catch (Exception ex)
            {
                throw new Exception("Error al obtener bases de datos: " + ex.Message);
            }
            finally
            {
                if (conexion.State == ConnectionState.Open)
                {
                    conexion.Close();
                }
            }

            return basesDeDatos;
        }

        public List<string> ObtenerTablas(string esquema)
        {
            List<string> tablas = new List<string>();
            string consulta = $"SELECT TABLE_NAME FROM ALL_TABLES WHERE OWNER = '{esquema.ToUpper()}'";

            try
            {
                AbrirConexion();
                using (OracleCommand comando = new OracleCommand(consulta, conexion))
                using (OracleDataReader lector = comando.ExecuteReader())
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

        //public List<string> ObtenerLlavesPrimarias()
        //{
        //    List<string> claves = new List<string>();

        //    string query = @"
        //SELECT
        //    kcu.table_name,
        //    kcu.column_name
        //FROM
        //    information_schema.table_constraints tc
        //    JOIN information_schema.key_column_usage kcu
        //      ON tc.constraint_name = kcu.constraint_name
        //     AND tc.table_schema = kcu.table_schema
        //WHERE
        //    tc.constraint_type = 'PRIMARY KEY'
        //    AND tc.table_schema = 'public';";

        //    using (NpgsqlCommand cmd = new NpgsqlCommand(query, conexion))
        //    using (NpgsqlDataReader reader = cmd.ExecuteReader())
        //    {
        //        while (reader.Read())
        //        {
        //            string tabla = reader["table_name"].ToString();
        //            string columna = reader["column_name"].ToString();
        //            claves.Add($"{tabla}.{columna}");
        //        }
        //    }

        //    return claves;
        //}


        public List<string> ObtenerVistas(string baseDatos)
        {
            List<string> vistas = new List<string>();
            string consulta = $@"
        SELECT view_name 
        FROM all_views 
        WHERE owner = '{baseDatos.ToUpper()}';";

            AbrirConexion();
            using (OracleCommand cmd = new OracleCommand(consulta, conexion))
            using (OracleDataReader reader = cmd.ExecuteReader())
            {
                while (reader.Read())
                {
                    vistas.Add(reader.GetString(0));
                }
            }

            return vistas;
        }

        public List<string> ObtenerLlavesPrimarias(string baseDatos)
        {
            List<string> llaves = new List<string>();
            string consulta = $@"
        SELECT acc.table_name, acc.column_name
        FROM all_cons_columns acc
        JOIN all_constraints ac
          ON acc.constraint_name = ac.constraint_name
         AND acc.owner = ac.owner
        WHERE ac.constraint_type = 'P'
          AND ac.owner = '{baseDatos.ToUpper()}';";

            AbrirConexion();
            using (OracleCommand cmd = new OracleCommand(consulta, conexion))
            using (OracleDataReader reader = cmd.ExecuteReader())
            {
                while (reader.Read())
                {
                    llaves.Add($"{reader.GetString(0)}.{reader.GetString(1)}");
                }
            }

            return llaves;
        }

        public List<string> ObtenerLlavesForaneas(string baseDatos)
        {
            List<string> foraneas = new List<string>();
            string consulta = $@"
        SELECT a.table_name, a.column_name, c_pk.table_name AS ref_table, b.column_name AS ref_column
        FROM all_cons_columns a
        JOIN all_constraints c ON a.owner = c.owner AND a.constraint_name = c.constraint_name
        JOIN all_constraints c_pk ON c.r_owner = c_pk.owner AND c.r_constraint_name = c_pk.constraint_name
        JOIN all_cons_columns b ON b.owner = c_pk.owner AND b.constraint_name = c_pk.constraint_name AND b.position = a.position
        WHERE c.constraint_type = 'R' AND c.owner = '{baseDatos.ToUpper()}';";

            AbrirConexion();
            using (OracleCommand cmd = new OracleCommand(consulta, conexion))
            using (OracleDataReader reader = cmd.ExecuteReader())
            {
                while (reader.Read())
                {
                    string origen = reader.GetString(0);
                    string colOrigen = reader.GetString(1);
                    string destino = reader.GetString(2);
                    string colDestino = reader.GetString(3);
                    foraneas.Add($"FK: {origen}.{colOrigen} → {destino}.{colDestino}");
                }
            }

            return foraneas;
        }

        public List<string> ObtenerProcedimientos(string baseDatos)
        {
            List<string> procedimientos = new List<string>();
            string consulta = $@"
        SELECT object_name 
        FROM all_objects 
        WHERE object_type = 'PROCEDURE'
          AND owner = '{baseDatos.ToUpper()}';";

            AbrirConexion();
            using (OracleCommand cmd = new OracleCommand(consulta, conexion))
            using (OracleDataReader reader = cmd.ExecuteReader())
            {
                while (reader.Read())
                {
                    procedimientos.Add(reader.GetString(0));
                }
            }

            return procedimientos;
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
