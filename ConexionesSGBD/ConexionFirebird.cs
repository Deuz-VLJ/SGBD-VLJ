using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Data;
using FirebirdSql.Data.FirebirdClient;

namespace ConexionesSGBD
{
    public class ConexionFirebird
    {

        private readonly FbConnection conexion;

        public ConexionFirebird(string servidor, string rutaBD, string usuario, string contraseña)
        {
            string cadenaConexion = $"User={usuario};Password={contraseña};Database={rutaBD};DataSource={servidor};Port=3050;Dialect=3;Charset=UTF8;";
            conexion = new FbConnection(cadenaConexion);
        }

        // Abrir conexión (si está cerrada)
        public void AbrirConexion()
        {
            if (conexion.State == ConnectionState.Closed)
            {
                conexion.Open();
            }
        }

        // Cerrar conexión (cuando la aplicación termine)
        public void CerrarConexion()
        {
            if (conexion.State == ConnectionState.Open)
            {
                conexion.Close();
            }
        }

        // Método para probar la conexión
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

        // Método genérico para ejecutar consultas usando la misma conexión
        //Aun no se usa
        private List<string> EjecutarConsulta(string consulta)
        {
            List<string> resultados = new List<string>();

            try
            {
                AbrirConexion();  // Asegurar que la conexión esté abierta
                using (FbCommand cmd = new FbCommand(consulta, conexion))
                using (FbDataReader reader = cmd.ExecuteReader())
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

        // Métodos para obtener la estructura de la base de datos
        public List<string> ObtenerTablas()
        {
            return EjecutarConsulta("SELECT RDB$RELATION_NAME FROM RDB$RELATIONS WHERE RDB$SYSTEM_FLAG = 0;");
        }

        public List<string> ObtenerVistas()
        {
            return EjecutarConsulta("SELECT RDB$VIEW_NAME FROM RDB$RELATIONS WHERE RDB$VIEW_BLR IS NOT NULL;");
        }

        public List<string> ObtenerIndices()
        {
            return EjecutarConsulta("SELECT RDB$INDEX_NAME FROM RDB$INDICES;");
        }

        public List<string> ObtenerProcedimientos()
        {
            return EjecutarConsulta("SELECT RDB$PROCEDURE_NAME FROM RDB$PROCEDURES;");
        }

        public List<string> ObtenerSecuencias()
        {
            return EjecutarConsulta("SELECT RDB$GENERATOR_NAME FROM RDB$GENERATORS;");
        }

        public List<string> ObtenerTriggers()
        {
            return EjecutarConsulta("SELECT RDB$TRIGGER_NAME FROM RDB$TRIGGERS;");
        }

        public List<string> ObtenerTiposDeDatos()
        {
            return EjecutarConsulta("SELECT DISTINCT RDB$FIELD_NAME FROM RDB$FIELDS;");
        }


    }
}
