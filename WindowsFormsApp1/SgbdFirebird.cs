using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;
using ConexionesSGBD;
using static System.Windows.Forms.VisualStyles.VisualStyleElement;

namespace WindowsFormsApp1
{
    public partial class SgbdFirebird : Form
    {
        private ConexionFirebird conexionFirebird;
        public SgbdFirebird(ConexionFirebird conexion)
        {
            InitializeComponent();
            this.conexionFirebird = conexion;
        }

        private void Form2_Load(object sender, EventArgs e)
        {
            LlenarTreeView();
        }

        private void LlenarTreeView()
        {
            treeViewBD.Nodes.Clear();

            TreeNode rootNode = new TreeNode("BASE DE DATOS Firebirt");

            AgregarNodo(rootNode, "Tablas", conexionFirebird.ObtenerTablas());
            AgregarNodo(rootNode, "Views", conexionFirebird.ObtenerVistas());
            AgregarNodo(rootNode, "Índices", conexionFirebird.ObtenerIndices());
            AgregarNodo(rootNode, "Procedimientos", conexionFirebird.ObtenerProcedimientos());
            AgregarNodo(rootNode, "Sequences", conexionFirebird.ObtenerSecuencias());
            AgregarNodo(rootNode, "Triggers", conexionFirebird.ObtenerTriggers());
            AgregarNodo(rootNode, "Data Types", conexionFirebird.ObtenerTiposDeDatos());

            treeViewBD.Nodes.Add(rootNode);
            treeViewBD.ExpandAll();
        }

        private void AgregarNodo(TreeNode parent, string nombre, List<string> elementos)
        {
            TreeNode nodo = new TreeNode(nombre);
            foreach (var item in elementos)
            {
                nodo.Nodes.Add(new TreeNode(item.Trim()));
            }
            parent.Nodes.Add(nodo);
        }
    }
}
