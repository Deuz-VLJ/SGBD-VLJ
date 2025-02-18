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
    public partial class SgbdOracleSQL : Form
    {
        private ConexionOracleSQL conexionOracle;
        public SgbdOracleSQL(ConexionOracleSQL conexion)
        {
            InitializeComponent();
            this.conexionOracle = conexion;
        }

        private void SgbdOracleSQL_Load(object sender, EventArgs e)
        {
            LlenarTreeView();
        }

        private void LlenarTreeView()
        {
            treeViewBD.Nodes.Clear();

            TreeNode rootNode = new TreeNode("BaseDatosOracle");

            AgregarNodo(rootNode, "Tablas", conexionOracle.ObtenerTablas());
            AgregarNodo(rootNode, "Vistas", conexionOracle.ObtenerVistas());
            AgregarNodo(rootNode, "Procedimientos", conexionOracle.ObtenerProcedimientos());
            AgregarNodo(rootNode, "Funciones", conexionOracle.ObtenerFunciones());
            AgregarNodo(rootNode, "Triggers", conexionOracle.ObtenerTriggers());
            AgregarNodo(rootNode, "Tipos de Datos", conexionOracle.ObtenerTiposDeDatos());

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
