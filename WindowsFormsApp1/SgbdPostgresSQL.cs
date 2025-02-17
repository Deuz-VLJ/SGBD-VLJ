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

            TreeNode rootNode = new TreeNode("GESTIONPRODUCTOS");

            AgregarNodo(rootNode, "Tablas", conexionPostgres.ObtenerTablas());
            AgregarNodo(rootNode, "Vistas", conexionPostgres.ObtenerVistas());
            AgregarNodo(rootNode, "Procedimientos", conexionPostgres.ObtenerProcedimientos());
            AgregarNodo(rootNode, "Funciones", conexionPostgres.ObtenerFunciones());
            AgregarNodo(rootNode, "Triggers", conexionPostgres.ObtenerTriggers());
            AgregarNodo(rootNode, "Tipos de Datos", conexionPostgres.ObtenerTiposDeDatos());

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
