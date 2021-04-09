using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;

namespace IOMaker
{
    public partial class Mgrande : Form
    {
        public Mgrande()
        {
            InitializeComponent();
        }
        bool salir = false, optimo = false, Segfase = false, optimosegfase = false;
        string[,] Tabla = new string[100, 100];
        string Entra = "", Sale = "", EP = "";
        int EPi = 0, ITE = 0, ITE2 = 0, posx = 150, posdtgy = 145, W = 0, contador = Form1.m + 1;
        List<DGVS> tablas = new List<DGVS>();
        private void Mgrande_Load(object sender, EventArgs e)
        {
            creartabla();
            llenartabla();
            Letras();
        }
        void creartabla()
        {
            int vari = 1;
            for (int i = 0; i < Form1.n + 1; i++)
            {
                dataGridView1.Rows.Add();
                if (i == 0)
                    dataGridView1.Rows[0].Cells[0].Value = 1;
                else dataGridView1.Rows[i].Cells[0].Value = 0;
            }
            ///////////////////////////////////////////////Llenar la cantidad de Columnas
            for (int i = 1; i <= Form1.m; i++)   //Columnas de las Variables Reales
                dataGridView1.Columns.Add("X" + i.ToString(), "X" + (i).ToString());
            for (int i = 0; i <= Form1.n; i++)  //Parte Para las Variables de Holgura
            {
                if (Form1.tabla[i, (Form1.m + 1)] == "<=")
                {
                    dataGridView1.Columns.Add("X" + (Form1.m + vari).ToString(), "X" + (Form1.m + vari).ToString());
                    vari++;
                }
                else if (Form1.tabla[i, (Form1.m + 1)] == ">=")
                {
                    dataGridView1.Columns.Add("X" + (Form1.m + vari).ToString(), "X" + (Form1.m + vari).ToString());
                    vari++;
                }
            }
            vari = 1;
            for (int i = 0; i <= Form1.n; i++)
            {
                if (Form1.tabla[i, (Form1.m + 1)] == ">=" || Form1.tabla[i, (Form1.m + 1)] == "=")
                {
                    dataGridView1.Columns.Add("W" + (vari).ToString(), "W" + (vari).ToString()); vari++; W++;
                }
            }
            dataGridView1.Columns.Add("Z0", "Z0");
            dataGridView1.Columns.Add("VB", "Var.Básicas");
            for (int i = 0; i < dataGridView1.Columns.Count - 1; i++)
                dataGridView1.Columns[i].Width = 50;
            dataGridView1.Columns[dataGridView1.Columns.Count - 1].Width = 70;
        }
        void llenartabla()
        {
            //Función Objetivo 
            int ww = 1, xx = Form1.m;
            for (int i = 1; i <= Form1.n; i++) //llenado deZ0
                dataGridView1.Rows[i].Cells["Z0"].Value = Form1.tabla[i, Form1.m + 2]; dataGridView1.Rows[0].Cells["Z0"].Value = 0;
            for (int i = (dataGridView1.Columns.Count - 2 - W); i <= (dataGridView1.Columns.Count - 3); i++)
            {
                if (Form1.MaxMin == "Max Z")
                    dataGridView1.Rows[0].Cells[i].Value = "M";
                else if (Form1.MaxMin == "Min Z")
                    dataGridView1.Rows[0].Cells[i].Value = "-M";
            }
            for (int i = 0; i <= Form1.n; i++) //llenado del vector de valores (funciona)            
                for (int j = 1; j <= Form1.m; j++)
                    dataGridView1.Rows[i].Cells[j].Value = Form1.tabla[i, j];
            for (int i = 1; i <= Form1.n; i++) //Llenado de las Variables Básicas
            {
                if (Form1.tabla[i, Form1.m + 1] == "<=")
                {
                    dataGridView1.Rows[i].Cells["VB"].Value = "X" + (contador).ToString(); contador++;
                }
                else if (Form1.tabla[i, Form1.m + 1] == ">=")
                {
                    dataGridView1.Rows[i].Cells["VB"].Value = "W" + (ww).ToString(); ww++; contador++;
                }
                else if (Form1.tabla[i, Form1.m + 1] == "=")
                {
                    dataGridView1.Rows[i].Cells["VB"].Value = "W" + (ww).ToString(); ww++;
                }
            }
            for (int j = Form1.m + 1; j <= contador - 1; j++)//Parte de la fun Objetivo (ceros iniciales)
                dataGridView1.Rows[0].Cells[j].Value = 0;
            for (int i = 1; i <= Form1.n; i++)
                for (int j = (Form1.m + 1); j <= (W + contador - 1); j++) //LLenado de los vectores unitarios
                {
                    if (dataGridView1.Rows[i].Cells["VB"].Value.ToString() == dataGridView1.Columns[j].Name)
                        dataGridView1.Rows[i].Cells[j].Value = 1;
                    else dataGridView1.Rows[i].Cells[j].Value = 0;
                }
            for (int i = 1; i <= Form1.n; i++)
            {
                if (Form1.tabla[i, Form1.m + 1] == ">=" || Form1.tabla[i, Form1.m + 1] == "<=")
                    xx++;
                if (Form1.tabla[i, Form1.m + 1] == ">=")
                    dataGridView1.Rows[i].Cells[xx].Value = -1;
            }
        }
        void vectorunitario()
        {
            for (int i = 1; i <= Form1.n; i++)
            {
                if (dataGridView1.Rows[0].Cells[dataGridView1.Rows[i].Cells["VB"].Value.ToString()].Value.ToString() != "0")
                    for (int j = 1; j <= dataGridView1.Columns.Count - 2; j++)
                {

                }
            }
        }                      
        void Letras()
        {
            string aux = string.Empty;
            textBox2.Text = "";
            aux = Form1.MaxMin;
            for (int j = 1; j <= Form1.m; j++)
            {
                if (double.Parse(Form1.tabla[0, j]) < 0)
                    aux += Form1.tabla[0, j] + "X" + j.ToString();
                else
                    aux += "+" + Form1.tabla[0, j] + "X" + j.ToString();
            }
            textBox2.Text = aux += "=0" + Environment.NewLine;
            aux = string.Empty;
            for (int i = 1; i <= Form1.n; i++)
            {
                for (int j = 1; j <= (Form1.m + 2); j++)
                {
                    if (j <= Form1.m)
                    {
                        if (Convert.ToDouble(Form1.tabla[i, j]) < 0)
                            aux += Form1.tabla[i, j] + "X" + j.ToString();
                        else
                            aux += "+" + Form1.tabla[i, j] + "X" + j.ToString();
                    }
                    else aux += Form1.tabla[i, j];
                }
                textBox2.Text += aux + Environment.NewLine; aux = "";
            }
        }
    }
}
