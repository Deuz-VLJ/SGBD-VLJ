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
    public partial class SgbdSQLServer : Form
    {
        private ConexionSQLServer conexionSQL;
        public SgbdSQLServer(ConexionSQLServer conexion)
        {
            InitializeComponent();
            this.conexionSQL = conexion;
        }

        private void SgbdSQLServer_Load(object sender, EventArgs e)
        {
            LlenarTreeView();
        }

        private void LlenarTreeView()
        {
            treeViewBD.Nodes.Clear();

            TreeNode rootNode = new TreeNode("BASE DE DATOS SQLSERVER");
            TreeNode basesDeDatosNode = new TreeNode("Bases de Datos");

            foreach (var bd in conexionSQL.ObtenerBasesDeDatos())
            {
                TreeNode bdNode = new TreeNode(bd);
                AgregarNodo(bdNode, "Tablas", conexionSQL.ObtenerTablas(bd));
                AgregarNodo(bdNode, "Vistas", conexionSQL.ObtenerVistas(bd));
                AgregarNodo(bdNode, "Procedimientos", conexionSQL.ObtenerProcedimientos(bd));
                AgregarNodo(bdNode, "Funciones", conexionSQL.ObtenerFunciones(bd));
                AgregarNodo(bdNode, "Triggers", conexionSQL.ObtenerTriggers(bd));
                AgregarNodo(bdNode, "Tipos de Datos", conexionSQL.ObtenerTiposDeDatos(bd));
                basesDeDatosNode.Nodes.Add(bdNode);
            }

            rootNode.Nodes.Add(basesDeDatosNode);
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
