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
    public partial class SgbdPostgresSQL : Form
    {
        private ConexionPostgresSQL conexionPostgres;

        public SgbdPostgresSQL(ConexionPostgresSQL conexion)
        {
            InitializeComponent();
            this.conexionPostgres = conexion;
        }

        private void SgbdPostgresSQL_Load(object sender, EventArgs e)
        {
            LlenarTreeView();
        }

        private void LlenarTreeView()
        {
            treeViewBD.Nodes.Clear();

            TreeNode rootNode = new TreeNode("BASE DE DATOS PostgreSQL");
            TreeNode basesDeDatosNode = new TreeNode("Bases de Datos");

            foreach (var bd in conexionPostgres.ObtenerBasesDeDatos())
            {
                TreeNode bdNode = new TreeNode(bd);
                AgregarNodo(bdNode, "Tablas", conexionPostgres.ObtenerTablas(bd));
                AgregarNodo(bdNode, "Vistas", conexionPostgres.ObtenerVistas(bd));
                AgregarNodo(bdNode, "Procedimientos", conexionPostgres.ObtenerProcedimientos(bd));
                AgregarNodo(bdNode, "Funciones", conexionPostgres.ObtenerFunciones(bd));
                AgregarNodo(bdNode, "Triggers", conexionPostgres.ObtenerTriggers(bd));
                AgregarNodo(bdNode, "Tipos de Datos", conexionPostgres.ObtenerTiposDeDatos(bd));
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
