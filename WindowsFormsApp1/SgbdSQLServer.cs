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

            AgregarNodo(rootNode, "Tablas", conexionSQL.ObtenerTablas());
            AgregarNodo(rootNode, "Vistas", conexionSQL.ObtenerVistas());
            AgregarNodo(rootNode, "Procedimientos", conexionSQL.ObtenerProcedimientos());
            AgregarNodo(rootNode, "Funciones", conexionSQL.ObtenerFunciones());
            AgregarNodo(rootNode, "Triggers", conexionSQL.ObtenerTriggers());
            AgregarNodo(rootNode, "Tipos de Datos", conexionSQL.ObtenerTiposDeDatos());

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
