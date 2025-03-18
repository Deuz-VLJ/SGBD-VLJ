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

            TreeNode rootNode = new TreeNode("BASE DE DATOS MySQL");
            TreeNode basesDeDatosNode = new TreeNode("Bases de Datos");

            foreach (var bd in conexionMySQL.ObtenerBasesDeDatos())
            {
                TreeNode bdNode = new TreeNode(bd);
                //AgregarNodo(bdNode, "Tablas", conexionMySQL.ObtenerTablas(bd));
                //AgregarNodo(bdNode, "Vistas", conexionMySQL.ObtenerVistas(bd));
                //AgregarNodo(bdNode, "Procedimientos", conexionMySQL.ObtenerProcedimientos(bd));
                //AgregarNodo(bdNode, "Funciones", conexionMySQL.ObtenerFunciones(bd));
                //AgregarNodo(bdNode, "Triggers", conexionMySQL.ObtenerTriggers(bd));
                //AgregarNodo(bdNode, "Tipos de Datos", conexionMySQL.ObtenerTiposDeDatos(bd));
                //basesDeDatosNode.Nodes.Add(bdNode);
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
