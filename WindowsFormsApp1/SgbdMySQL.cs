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
    public partial class SgbdMySQL : Form
    {
        private ConexionMySQL conexionMySQL;
        public SgbdMySQL(ConexionMySQL conexion)
        {
            InitializeComponent();
            this.conexionMySQL = conexion;
        }

        private void SgbdMySQL_Load(object sender, EventArgs e)
        {
            LlenarTreeView();
        }

        private void LlenarTreeView()
        {
            treeViewBD.Nodes.Clear();

            TreeNode rootNode = new TreeNode("GESTIONPRODUCTOS");

            AgregarNodo(rootNode, "Tablas", conexionMySQL.ObtenerTablas());
            AgregarNodo(rootNode, "Vistas", conexionMySQL.ObtenerVistas());
            AgregarNodo(rootNode, "Procedimientos", conexionMySQL.ObtenerProcedimientos());
            AgregarNodo(rootNode, "Funciones", conexionMySQL.ObtenerFunciones());
            AgregarNodo(rootNode, "Triggers", conexionMySQL.ObtenerTriggers());
            AgregarNodo(rootNode, "Tipos de Datos", conexionMySQL.ObtenerTiposDeDatos());

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
