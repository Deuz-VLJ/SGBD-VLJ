using System;
using System.Collections.Generic;
using System.Data;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Npgsql;

namespace ConexionesSGBD
{
    public class ConexionPostgresSQL : IBaseDatos
    {
        private readonly NpgsqlConnection conexion;

        public ConexionPostgresSQL(string servidor, string usuario, string contraseña)
        {
            string cadenaConexion = $"host={servidor};Database=postgres;Username={usuario};Password={contraseña};Port=5432;";
            conexion = new NpgsqlConnection(cadenaConexion);
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
            using (NpgsqlCommand cmd = new NpgsqlCommand(consulta, conexion))
            using (NpgsqlDataReader reader = cmd.ExecuteReader())
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
        SELECT 
            cols.column_name, 
            cols.data_type,
            CASE 
                WHEN pk.column_name IS NOT NULL THEN 'PK'
                ELSE ''
            END AS key_type
        FROM information_schema.columns cols
        LEFT JOIN (
            SELECT 
                kcu.column_name
            FROM information_schema.table_constraints tc
            JOIN information_schema.key_column_usage kcu 
                ON tc.constraint_name = kcu.constraint_name
            WHERE tc.constraint_type = 'PRIMARY KEY' AND tc.table_name = '{tabla}'
        ) pk ON cols.column_name = pk.column_name
        WHERE cols.table_name = '{tabla}' AND cols.table_schema = 'public';";

            try
            {
                AbrirConexion();
                using (NpgsqlCommand comando = new NpgsqlCommand(consulta, conexion))
                using (NpgsqlDataReader lector = comando.ExecuteReader())
                {
                    while (lector.Read())
                    {
                        string nombre = lector["column_name"].ToString();
                        string tipo = lector["data_type"].ToString();
                        string key = lector["key_type"].ToString();

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
            return EjecutarConsulta("SELECT datname FROM pg_database WHERE datistemplate = false;");
        }



        public List<string> ObtenerTablas(string baseDatos)
        {
            List<string> tablas = new List<string>();
            string consulta = "SELECT table_name FROM information_schema.tables WHERE table_schema = 'public' AND table_type = 'BASE TABLE';";

            try
            {
                AbrirConexion();
                using (NpgsqlCommand comando = new NpgsqlCommand(consulta, conexion))
                using (NpgsqlDataReader lector = comando.ExecuteReader())
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



        public List<string> ObtenerVistas()
        {
            return EjecutarConsulta("SELECT viewname FROM pg_views WHERE schemaname = 'public';");
        }

        public List<string> ObtenerProcedimientos()
        {
            return EjecutarConsulta("SELECT proname FROM pg_proc JOIN pg_namespace ON pg_proc.pronamespace = pg_namespace.oid WHERE nspname = 'public';");
        }

        public List<string> ObtenerFunciones()
        {
            return EjecutarConsulta("SELECT routine_name FROM information_schema.routines WHERE routine_schema = 'public';");
        }

        public List<string> ObtenerTriggers()
        {
            return EjecutarConsulta("SELECT trigger_name FROM information_schema.triggers WHERE trigger_schema = 'public';");
        }

        public List<string> ObtenerTiposDeDatos()
        {
            return EjecutarConsulta("SELECT DISTINCT data_type FROM information_schema.columns WHERE table_schema = 'public';");
        }

        public List<string> ObtenerIndices()
        {
            return EjecutarConsulta("SELECT indexname FROM pg_indexes WHERE schemaname = 'public';");
        }

        public List<string> ObtenerSecuencias()
        {
            return EjecutarConsulta("SELECT sequence_name FROM information_schema.sequences WHERE sequence_schema = 'public';");
        }
    }
}
