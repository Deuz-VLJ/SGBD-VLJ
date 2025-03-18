using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Data;
using FirebirdSql.Data.FirebirdClient;

namespace ConexionesSGBD
{

    //J:\Documentos\TEC\ADMINISTRACION DE BASE DE DATOS\Aprendisaje firebird\Base de datos\BASE DE DATOS PRUEBA.GDB
    public class ConexionFirebird : IBaseDatos
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
        public List<string> EjecutarConsulta(string consulta)
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

        public Dictionary<string, string> ObtenerAtributos(string tabla)
        {
            Dictionary<string, string> atributos = new Dictionary<string, string>();

            try
            {
                if (conexion.State != ConnectionState.Open)
                {
                    conexion.Open();
                }

                // 🔹 Extraer solo el nombre de la tabla si viene en formato "BaseDeDatos.Tabla"
                string nombreTabla = tabla.Contains(".") ? tabla.Split('.')[1] : tabla;

                string consulta = @"
            SELECT TRIM(rf.RDB$FIELD_NAME) AS COLUMN_NAME, TRIM(dt.RDB$TYPE_NAME) AS DATA_TYPE
            FROM RDB$RELATION_FIELDS rf
            JOIN RDB$FIELDS f ON rf.RDB$FIELD_SOURCE = f.RDB$FIELD_NAME
            JOIN RDB$TYPES dt ON f.RDB$FIELD_TYPE = dt.RDB$TYPE
            WHERE rf.RDB$RELATION_NAME = @tabla
            AND dt.RDB$FIELD_NAME = 'RDB$FIELD_TYPE';
        ";

                using (FbCommand comando = new FbCommand(consulta, conexion))
                {
                    comando.Parameters.AddWithValue("@tabla", nombreTabla);

                    using (FbDataReader lector = comando.ExecuteReader())
                    {
                        while (lector.Read())
                        {
                            string columna = lector["COLUMN_NAME"].ToString().Trim();
                            string tipoDato = lector["DATA_TYPE"].ToString().Trim();
                            atributos[columna] = tipoDato;
                        }
                    }
                }
            }
            catch (Exception ex)
            {
                throw new Exception($"Error en ObtenerAtributos() Firebird para la tabla '{tabla}': {ex.Message}");
            }

            return atributos;
        }


        // Métodos para obtener la estructura de la base de datos
        public List<string> ObtenerBasesDeDatos()
        {
            List<string> basesDeDatos = new List<string>();

            try
            {
                if (conexion.State != ConnectionState.Open)
                {
                    conexion.Open();
                }

                // 🔹 Intentar obtener el nombre de la base de datos desde `MON$DATABASE`
                string consulta = "SELECT TRIM(MON$DATABASE_NAME) AS DATABASE_PATH FROM MON$DATABASE;";

                using (FbCommand comando = new FbCommand(consulta, conexion))
                using (FbDataReader lector = comando.ExecuteReader())
                {
                    if (lector.Read())
                    {
                        basesDeDatos.Add(lector["DATABASE_PATH"].ToString().Trim());
                    }
                }
            }
            catch (Exception ex)
            {
                throw new Exception($"Error en ObtenerBasesDeDatos() Firebird: {ex.Message}");
            }

            return basesDeDatos;
        }




        public List<string> ObtenerTablas()
        {
            List<string> tablas = new List<string>();

            try
            {
                if (conexion.State != ConnectionState.Open)
                {
                    conexion.Open();
                }

                // 🔹 Obtener solo las tablas de usuario en Firebird (evitar tablas del sistema)
                string consulta = @"
            SELECT TRIM(RDB$RELATION_NAME) AS TABLE_NAME 
            FROM RDB$RELATIONS 
            WHERE RDB$SYSTEM_FLAG = 0 OR RDB$SYSTEM_FLAG IS NULL
            ORDER BY RDB$RELATION_NAME;";

                using (FbCommand comando = new FbCommand(consulta, conexion))
                using (FbDataReader lector = comando.ExecuteReader())
                {
                    while (lector.Read())
                    {
                        string nombreTabla = lector["TABLE_NAME"].ToString().Trim();

                        // 🔹 Filtrar nombres incorrectos como "GBD"
                        if (!string.IsNullOrEmpty(nombreTabla) && !nombreTabla.StartsWith("RDB$"))
                        {
                            tablas.Add(nombreTabla);
                        }
                    }
                }
            }
            catch (Exception ex)
            {
                throw new Exception($"Error en ObtenerTablas() Firebird: {ex.Message}");
            }

            return tablas;
        }

        // 🔹 Método para obtener el nombre de la base de datos activa en Firebird
        private string ObtenerNombreBaseDeDatos()
        {
            string nombreBD = "FirebirdDB"; // 🔹 Valor por defecto si no se encuentra

            try
            {
                string consulta = "SELECT MON$DATABASE_NAME FROM MON$DATABASE;";

                using (FbCommand comando = new FbCommand(consulta, conexion))
                using (FbDataReader lector = comando.ExecuteReader())
                {
                    if (lector.Read())
                    {
                        nombreBD = lector["MON$DATABASE_NAME"].ToString().Trim();
                    }
                }
            }
            catch (Exception)
            {
                // 🔹 Si no se puede obtener, sigue con el nombre predeterminado
            }

            return nombreBD;
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
