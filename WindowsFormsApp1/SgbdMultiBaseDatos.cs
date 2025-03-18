using System;
using System.Collections.Generic;
using System.Data;
using System.Data.SqlClient;
using System.Linq;
using System.Text;
using System.Windows.Forms;
using ConexionesSGBD;

namespace WindowsFormsApp1
{
    public partial class SgbdMultiBaseDatos : Form
    {
        private Dictionary<string, IBaseDatos> conexiones;
        private IBaseDatos conexionActual;
        private string nombreConexionActual;

        public SgbdMultiBaseDatos(Dictionary<string, IBaseDatos> conexiones)
        {
            InitializeComponent();
            this.conexiones = conexiones ?? new Dictionary<string, IBaseDatos>(); // Evitar nulos
        }

        private void SgbdMultiBaseDatos_Load(object sender, EventArgs e)
        {
            CargarListaConexiones();
            LlenarTreeView();
        }

        // 🔹 Cargar todas las conexiones en el TreeView
        private void CargarListaConexiones()
        {
            treeViewBD.Nodes.Clear();

            if (conexiones.Count == 0)
            {
                MessageBox.Show("No hay conexiones disponibles.", "Error", MessageBoxButtons.OK, MessageBoxIcon.Error);
                return;
            }

            foreach (var conexion in conexiones)
            {
                TreeNode conexionNode = new TreeNode(conexion.Key) { Tag = conexion.Key };
                treeViewBD.Nodes.Add(conexionNode);
            }

            // Seleccionar la primera conexión automáticamente
            if (treeViewBD.Nodes.Count > 0)
            {
                treeViewBD.SelectedNode = treeViewBD.Nodes[0];
                CambiarConexion(treeViewBD.Nodes[0].Tag.ToString());
            }
        }

        // 🔹 Evento cuando seleccionas una conexión en el TreeView
        private void TreeViewBD_AfterSelect_1(object sender, TreeViewEventArgs e)
        {
            // 🔹 Si el nodo seleccionado es una conexión, cambiar a esa conexión
            if (e.Node.Tag != null && e.Node.Tag.ToString() == "conexion")
            {
                CambiarConexion(e.Node.Text);
            }
            // 🔹 Si el nodo seleccionado es una base de datos, actualizar la conexión y el ComboBox
            else if (e.Node.Parent != null && e.Node.Parent.Tag != null && e.Node.Parent.Tag.ToString() == "conexion")
            {
                string baseDatosSeleccionada = e.Node.Text.Replace("Base de Datos: ", "").Trim();

                if (conexionActual != null)
                {
                    conexionActual.CerrarConexion(); // 🔹 Cierra la conexión anterior
                }

                string nombreConexion = e.Node.Parent.Text; // 🔹 Nombre del gestor de BD (ejemplo: "SQL Server - 12:30:01")
                if (!conexiones.ContainsKey(nombreConexion)) return; // 🔹 Validar que la conexión exista

                conexionActual = conexiones[nombreConexion]; // 🔹 Cambiar conexión activa
                nombreConexionActual = nombreConexion;
                conexionActual.AbrirConexion(); // 🔹 Abrir la nueva conexión con la base seleccionada

                // 🔹 Actualizar el ComboBox para reflejar la nueva base de datos seleccionada
                if (comboBoxBD.Items.Contains(baseDatosSeleccionada))
                {
                    comboBoxBD.SelectedItem = baseDatosSeleccionada;
                }
            }
        }


        // 🔹 Cambiar la conexión activa
        private void CambiarConexion(string nombreConexion)
        {
            if (!conexiones.ContainsKey(nombreConexion))
            {
                MessageBox.Show("La conexión seleccionada no existe.", "Error", MessageBoxButtons.OK, MessageBoxIcon.Error);
                return;
            }

            if (conexionActual != null)
            {
                conexionActual.CerrarConexion(); // 🔹 Cierra la conexión anterior
            }

            // 🔹 Activar la nueva conexión
            conexionActual = conexiones[nombreConexion];
            nombreConexionActual = nombreConexion;
            conexionActual.AbrirConexion(); // 🔹 Abre la nueva conexión

            // 🔹 Actualizar el ComboBox con las bases de datos de la nueva conexión
            CargarListaBasesDatos();

            // 🔹 Actualizar el TreeView para reflejar la nueva conexión
            LlenarTreeView();
        }

        // 🔹 Cargar bases de datos en el ComboBox
        private void CargarListaBasesDatos()
        {
            comboBoxBD.Items.Clear();

            if (conexionActual == null)
            {
                MessageBox.Show("No hay conexión activa.", "Error", MessageBoxButtons.OK, MessageBoxIcon.Error);
                return;
            }

            // 🔹 Obtener bases de datos disponibles
            List<string> basesDeDatos = conexionActual.ObtenerBasesDeDatos();

            if (basesDeDatos.Count == 0)
            {
                MessageBox.Show("No se encontraron bases de datos en este gestor.", "Información", MessageBoxButtons.OK, MessageBoxIcon.Warning);
                return;
            }

            comboBoxBD.Items.AddRange(basesDeDatos.ToArray());
            comboBoxBD.SelectedIndex = 0; // 🔹 Selecciona la primera base de datos automáticamente
        }

        private void comboBoxBD_SelectedIndexChanged(object sender, EventArgs e)
        {
            if (comboBoxBD.SelectedItem == null)
            {
                MessageBox.Show("Seleccione una base de datos.", "Error", MessageBoxButtons.OK, MessageBoxIcon.Error);
                return;
            }

            nombreConexionActual = comboBoxBD.SelectedItem.ToString();
            LlenarTreeView();
        }

        // 🔹 Llenar el TreeView con bases de datos y sus tablas
        private void LlenarTreeView()
        {
            if (conexionActual == null)
            {
                MessageBox.Show("No hay conexión activa.", "Error", MessageBoxButtons.OK, MessageBoxIcon.Error);
                return;
            }

            // 🔹 Buscar si la conexión ya está en TreeView
            TreeNode conexionNode = null;
            foreach (TreeNode node in treeViewBD.Nodes)
            {
                if (node.Text == nombreConexionActual)
                {
                    conexionNode = node;
                    break;
                }
            }

            // 🔹 Si la conexión NO está en el TreeView, la agregamos como nodo raíz
            if (conexionNode == null)
            {
                conexionNode = new TreeNode(nombreConexionActual) { Tag = "conexion" };
                treeViewBD.Nodes.Add(conexionNode);
            }
            else
            {
                // 🔹 Si la conexión ya existe, limpiar sus bases de datos antes de actualizar
                conexionNode.Nodes.Clear();
            }

            // 🔹 Obtener bases de datos de esta conexión
            List<string> basesDeDatos = conexionActual.ObtenerBasesDeDatos();
            if (basesDeDatos.Count == 0)
            {
                MessageBox.Show("No se encontraron bases de datos en este gestor.", "Información", MessageBoxButtons.OK, MessageBoxIcon.Warning);
                return;
            }

            foreach (var baseDatos in basesDeDatos)
            {
                TreeNode baseNode = new TreeNode($"Base de Datos: {baseDatos}") { Tag = "baseDatos" };

                // 🔹 Obtener todas las tablas de esta base de datos
                List<string> tablas = conexionActual.ObtenerTablas();
                if (tablas != null && tablas.Count > 0)
                {
                    TreeNode tablasNode = new TreeNode("Tablas");

                    foreach (var tabla in tablas)
                    {
                        TreeNode tablaNode = new TreeNode(tabla);

                        // 🔹 Obtener los atributos (columnas) de la tabla
                        Dictionary<string, string> atributos = conexionActual.ObtenerAtributos(tabla);
                        if (atributos != null && atributos.Count > 0)
                        {
                            foreach (var atributo in atributos)
                            {
                                tablaNode.Nodes.Add(new TreeNode($"{atributo.Key} ({atributo.Value})"));
                            }
                        }

                        tablasNode.Nodes.Add(tablaNode);
                    }

                    baseNode.Nodes.Add(tablasNode);
                }

                conexionNode.Nodes.Add(baseNode);
            }

            conexionNode.Expand(); // 🔹 Expande la conexión actual
        }






        // 🔹 Agregar nodos de tablas y atributos
        private void AgregarNodo(TreeNode parent, string nombre, List<string> elementos)
        {
            if (elementos == null || elementos.Count == 0)
            {
                return;
            }

            TreeNode nodoCategoria = new TreeNode(nombre);
            foreach (var item in elementos)
            {
                TreeNode tablaNode = new TreeNode(item);
                Dictionary<string, string> atributos = conexionActual.ObtenerAtributos(item);

                foreach (var atributo in atributos)
                {
                    tablaNode.Nodes.Add(new TreeNode($"{atributo.Key} ({atributo.Value})"));
                }

                nodoCategoria.Nodes.Add(tablaNode);
            }

            parent.Nodes.Add(nodoCategoria);
        }

        // 🔹 Agregar una nueva conexión con el botón +
        private void btnAgregarConexion_Click_1(object sender, EventArgs e)
        {
            using (var loginForm = new Form1())
            {
                if (loginForm.ShowDialog() == DialogResult.OK)
                {
                    string gestorSeleccionado = loginForm.GestorSeleccionado;

                    // 🔹 Verificar que `GestorSeleccionado` NO esté vacío o nulo
                    if (string.IsNullOrEmpty(gestorSeleccionado))
                    {
                        MessageBox.Show("Error: No se recibió un gestor de base de datos válido.", "Error", MessageBoxButtons.OK, MessageBoxIcon.Error);
                        return;
                    }

                    // 🔹 Agregar la marca de tiempo a la conexión
                    string nombreConexion = gestorSeleccionado;

                    // 🔹 Crear la conexión basada en `GestorSeleccionado`
                    IBaseDatos nuevaConexion = null;

                    switch (gestorSeleccionado.Split('-')[0].Trim()) // 🔹 Separa el nombre del gestor
                    {
                        case "Firebird":
                            nuevaConexion = loginForm.Conexion;
                            break;
                        case "SQL Server":
                            nuevaConexion = loginForm.Conexion;
                            break;
                        case "MySQL":
                            nuevaConexion = loginForm.Conexion;
                            break;
                        case "PostgreSQL":
                            nuevaConexion = loginForm.Conexion;
                            break;
                        case "Oracle":
                            nuevaConexion = loginForm.Conexion;
                            break;
                        default:
                            MessageBox.Show("Gestor de base de datos no soportado.", "Error", MessageBoxButtons.OK, MessageBoxIcon.Error);
                            return;
                    }

                    if (nuevaConexion != null)
                    {
                        // 🔹 Asegurar que la conexión no se duplique
                        if (!conexiones.ContainsKey(nombreConexion))
                        {
                            conexiones[nombreConexion] = nuevaConexion;

                            // 🔹 Agregar nodo raíz SOLO SI no existe
                            TreeNode conexionNode = new TreeNode(nombreConexion) { Tag = "conexion" };
                            treeViewBD.Nodes.Add(conexionNode);
                        }

                        // 🔹 Cambiar a la nueva conexión
                        CambiarConexion(nombreConexion);
                    }
                }
            }
        }




        private void btnEjecutar_Click(object sender, EventArgs e)
        {
            if (conexionActual == null)
            {
                MessageBox.Show("No hay conexión activa.", "Error", MessageBoxButtons.OK, MessageBoxIcon.Error);
                return;
            }

            if (comboBoxBD.SelectedItem == null)
            {
                MessageBox.Show("Seleccione una base de datos antes de ejecutar la consulta.", "Error", MessageBoxButtons.OK, MessageBoxIcon.Warning);
                return;
            }

            string baseDatosSeleccionada = comboBoxBD.SelectedItem.ToString().Trim();
            string consulta = txtQuery.Text.Trim();

            if (string.IsNullOrEmpty(consulta))
            {
                MessageBox.Show("Ingrese una consulta para ejecutar.", "Error", MessageBoxButtons.OK, MessageBoxIcon.Warning);
                return;
            }

            // 🔹 Verificar que la consulta no se está ejecutando en la base incorrecta
            if (conexionActual is ConexionSQLServer && consulta.Trim().ToUpper().Contains("CREATE TABLE") && consulta.Trim().ToUpper().Contains("FIREBIRD"))
            {
                MessageBox.Show("No puedes crear una tabla de Firebird en SQL Server.", "Error", MessageBoxButtons.OK, MessageBoxIcon.Error);
                return;
            }

            // 🔹 Agregar `USE <BaseDeDatos>` antes de ejecutar la consulta solo si es SQL Server o MySQL
            if (conexionActual is ConexionSQLServer || conexionActual is ConexionMySQL)
            {
                consulta = $"USE [{baseDatosSeleccionada}];\n" + consulta;
            }

            try
            {
                // 🔹 Ejecutar la consulta en la conexión actual
                List<string> resultados = conexionActual.EjecutarConsulta(consulta);

                // 🔹 Verificar si hubo error o si la consulta fue exitosa
                if (resultados.Count == 0)
                {
                    MessageBox.Show($"Consulta ejecutada con éxito en la base de datos: {baseDatosSeleccionada}.", "Éxito", MessageBoxButtons.OK, MessageBoxIcon.Information);
                }
                else if (resultados[0].StartsWith("Error:"))
                {
                    MessageBox.Show(resultados[0], "Error en la consulta", MessageBoxButtons.OK, MessageBoxIcon.Error);
                }
                else
                {
                    MessageBox.Show($"Consulta ejecutada con éxito en la base de datos: {baseDatosSeleccionada}.", "Éxito", MessageBoxButtons.OK, MessageBoxIcon.Information);
                }
            }
            catch (Exception ex)
            {
                MessageBox.Show($"Error al ejecutar la consulta: {ex.Message}", "Error", MessageBoxButtons.OK, MessageBoxIcon.Error);
            }
        }

    }
}
