// Código completo actualizado del formulario SgbdMultiBaseDatos

using System;
using System.Collections.Generic;
using System.Data;
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
            this.conexiones = conexiones ?? new Dictionary<string, IBaseDatos>();
        }

        private void SgbdMultiBaseDatos_Load(object sender, EventArgs e)
        {
            CargarListaConexiones();
            LlenarTreeView();
        }

        private void CargarListaConexiones()
        {
            treeViewBD.Nodes.Clear();
            foreach (var conexion in conexiones)
            {
                TreeNode conexionNode = new TreeNode(conexion.Key) { Tag = "conexion" };
                treeViewBD.Nodes.Add(conexionNode);
            }
            if (treeViewBD.Nodes.Count > 0)
            {
                treeViewBD.SelectedNode = treeViewBD.Nodes[0];
                CambiarConexion(treeViewBD.SelectedNode.Text);
            }
        }

        private void TreeViewBD_AfterSelect_1(object sender, TreeViewEventArgs e)
        {
            if (e.Node.Tag?.ToString() == "conexion")
            {
                CambiarConexion(e.Node.Text);
            }
            else if (e.Node.Parent != null && e.Node.Parent.Tag?.ToString() == "conexion")
            {
                CambiarConexion(e.Node.Parent.Text);
                string bd = e.Node.Text.Replace("Base de Datos: ", "");
                if (comboBoxBD.Items.Contains(bd)) comboBoxBD.SelectedItem = bd;
            }
        }

        private void CambiarConexion(string nombreConexion)
        {
            if (!conexiones.ContainsKey(nombreConexion)) return;

            conexionActual?.CerrarConexion();

            conexionActual = conexiones[nombreConexion];
            nombreConexionActual = nombreConexion;
            conexionActual.AbrirConexion();
            CargarListaBasesDatos();
            LlenarTreeView();
        }

        private void CargarListaBasesDatos()
        {
            comboBoxBD.Items.Clear();
            if (conexionActual == null) return;
            var bases = conexionActual.ObtenerBasesDeDatos();
            comboBoxBD.Items.AddRange(bases.ToArray());
            if (bases.Count > 0) comboBoxBD.SelectedIndex = 0;
        }

        private void comboBoxBD_SelectedIndexChanged(object sender, EventArgs e)
        {
            if (comboBoxBD.SelectedItem == null) return;
            LlenarTreeView();
        }

        private void LlenarTreeView()
        {
            if (conexionActual == null || string.IsNullOrEmpty(nombreConexionActual)) return;

            TreeNode conexionNode = treeViewBD.Nodes.Cast<TreeNode>().FirstOrDefault(n => n.Text == nombreConexionActual);
            if (conexionNode == null)
            {
                conexionNode = new TreeNode(nombreConexionActual) { Tag = "conexion" };
                treeViewBD.Nodes.Add(conexionNode);
            }

            conexionNode.Nodes.Clear();

            foreach (string baseDatos in conexionActual.ObtenerBasesDeDatos())
            {
                TreeNode bdNode = new TreeNode("Base de Datos: " + baseDatos) { Tag = "baseDatos" };
                TreeNode tablasNode = new TreeNode("Tablas");

                var tablas = conexionActual.ObtenerTablas(baseDatos);
                foreach (var tabla in tablas)
                {
                    TreeNode tablaNode = new TreeNode(tabla);
                    var atributos = conexionActual.ObtenerAtributos(baseDatos, tabla);

                    foreach (var atributo in atributos)
                    {
                        tablaNode.Nodes.Add(new TreeNode($"{atributo.Key} ({atributo.Value})"));
                    }

                    tablasNode.Nodes.Add(tablaNode);
                }

                if (tablasNode.Nodes.Count > 0)
                    bdNode.Nodes.Add(tablasNode);

                conexionNode.Nodes.Add(bdNode);
            }

            conexionNode.Expand();
        }




        private string AdaptarConsulta(IBaseDatos conexion, string baseDatos, string consulta)
        {
            if (conexion is ConexionSQLServer)
                return $"USE [{baseDatos}];\n{consulta}";
            return consulta;
        }

        private void btnEjecutar_Click(object sender, EventArgs e)
        {
            if (conexionActual == null || comboBoxBD.SelectedItem == null) return;

            string bd = comboBoxBD.SelectedItem.ToString();
            string consulta = txtQuery.Text.Trim();
            if (string.IsNullOrWhiteSpace(consulta)) return;

            string consultaFinal = AdaptarConsulta(conexionActual, bd, consulta);

            if (conexionActual is ConexionMySQL mysql)
            {
                try { mysql.CambiarBaseDatos(bd); }
                catch (Exception ex)
                {
                    MessageBox.Show($"MySQL error: {ex.Message}"); return;
                }
            }

            List<string> resultados = conexionActual.EjecutarConsulta(consultaFinal);

            if (resultados.Count == 0)
                MessageBox.Show("Consulta ejecutada con éxito.");
            else if (resultados[0].StartsWith("Error:"))
                MessageBox.Show(resultados[0]);
            else
                MessageBox.Show("Consulta ejecutada con éxito y datos retornados.");
        }

        private void BttDesconexion_Click(object sender, EventArgs e)
        {
            if (treeViewBD.SelectedNode == null) return;
            TreeNode nodo = treeViewBD.SelectedNode;
            if (nodo.Parent != null && nodo.Parent.Tag?.ToString() == "conexion") nodo = nodo.Parent;
            if (nodo.Tag?.ToString() != "conexion") return;

            string nombreConexion = nodo.Text;
            if (conexiones.ContainsKey(nombreConexion))
            {
                conexiones[nombreConexion].CerrarConexion();
                conexiones.Remove(nombreConexion);
            }
            treeViewBD.Nodes.Remove(nodo);
            if (nombreConexionActual == nombreConexion)
            {
                conexionActual = null;
                nombreConexionActual = null;
                comboBoxBD.Items.Clear();
                comboBoxBD.Text = "";
            }
        }


        private void btnAgregarConexion_Click_1(object sender, EventArgs e)
        {
            using (var loginForm = new Form1())
            {
                if (loginForm.ShowDialog() == DialogResult.OK)
                {
                    string nombreConexion = loginForm.NombreConexion;

                    if (string.IsNullOrEmpty(nombreConexion))
                    {
                        MessageBox.Show("No se recibió un nombre de conexión válido.", "Error", MessageBoxButtons.OK, MessageBoxIcon.Error);
                        return;
                    }

                    IBaseDatos nuevaConexion = loginForm.Conexion;

                    if (nuevaConexion == null)
                    {
                        MessageBox.Show("La conexión no fue creada correctamente.", "Error", MessageBoxButtons.OK, MessageBoxIcon.Error);
                        return;
                    }

                    if (!conexiones.ContainsKey(nombreConexion))
                    {
                        conexiones[nombreConexion] = nuevaConexion;

                        TreeNode nodoConexion = new TreeNode(nombreConexion)
                        {
                            Tag = "conexion"
                        };
                        treeViewBD.Nodes.Add(nodoConexion);
                    }

                    CambiarConexion(nombreConexion);
                }
            }
        }



        private void BttActualizar_Click(object sender, EventArgs e)
        {
            if (treeViewBD.SelectedNode == null) return;

            TreeNode nodo = treeViewBD.SelectedNode;
            if (nodo.Parent != null && nodo.Parent.Tag?.ToString() == "conexion")
                nodo = nodo.Parent;

            if (nodo.Tag?.ToString() != "conexion") return;

            string nombreConexion = nodo.Text;
            if (!conexiones.ContainsKey(nombreConexion)) return;

            try
            {
                var conexion = conexiones[nombreConexion];
                conexion.CerrarConexion();
                conexion.AbrirConexion();

                if (nombreConexionActual == nombreConexion)
                {
                    conexionActual = conexion;
                    CargarListaBasesDatos();
                }

                nodo.Nodes.Clear();

                // 🔄 Recorremos cada base de datos individual
                foreach (var bd in conexion.ObtenerBasesDeDatos())
                {
                    TreeNode bdNode = new TreeNode($"Base de Datos: {bd}") { Tag = "baseDatos" };
                    TreeNode tablasNode = new TreeNode("Tablas");

                    // 🔄 Obtener solo las tablas de esa base de datos
                    var tablas = conexion.ObtenerTablas(bd);
                    foreach (var tabla in tablas)
                    {
                        TreeNode tablaNode = new TreeNode(tabla);

                        // 🔄 Obtener solo los atributos de esa tabla y base
                        var atributos = conexion.ObtenerAtributos(bd, tabla);
                        foreach (var atr in atributos)
                        {
                            tablaNode.Nodes.Add(new TreeNode($"{atr.Key} ({atr.Value})"));
                        }

                        tablasNode.Nodes.Add(tablaNode);
                    }

                    if (tablasNode.Nodes.Count > 0)
                        bdNode.Nodes.Add(tablasNode);

                    nodo.Nodes.Add(bdNode);
                }

                nodo.Expand();
                MessageBox.Show("Conexión actualizada correctamente.", "Actualizado", MessageBoxButtons.OK, MessageBoxIcon.Information);
            }
            catch (Exception ex)
            {
                MessageBox.Show($"Error al actualizar: {ex.Message}");
            }
        }

    }
}
