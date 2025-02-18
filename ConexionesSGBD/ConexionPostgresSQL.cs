using System;
using System.Collections.Generic;
using System.Data;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Npgsql;

namespace ConexionesSGBD
{
    public class ConexionPostgresSQL
    {
        private readonly NpgsqlConnection conexion;

        public ConexionPostgresSQL(string servidor, string baseDatos, string usuario, string contraseña)
        {
            string cadenaConexion = $"host={servidor};Database={baseDatos};Username={usuario};Password={contraseña};Port=5432;";
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

        private List<string> EjecutarConsulta(string consulta)
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

        public List<string> ObtenerTablas()
        {
            return EjecutarConsulta("SELECT tablename FROM pg_tables WHERE schemaname = 'public';");
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
    }
}
