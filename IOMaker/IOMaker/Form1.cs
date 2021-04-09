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
    public partial class Form1 : Form
    {
        public Form1()
        {
            InitializeComponent();
        }
        public static string[,] tabla= new string[100,100];
        public static string[] punto = new string[3];
        public static int n = 0, m = 0;
        public static string MaxMin = "Max Z", procedimiento="";
        public static string ResFinal = "";
        bool corregir = false;
        private void button1_Click(object sender, EventArgs e) //función que crea la tabla de llenado(Y)
        {
            try {
                n = Convert.ToInt32(textBox2.Text); m = Convert.ToInt32(textBox1.Text); dataGridView1.Visible = true; corregir = true;
                //////////////////////////////////////////////////// Parte para agregar las columnas
                if (corregir == false)
                {
                    dataGridView1.Columns.Add("0", "Var.Nombre"); dataGridView1.Columns["0"].ReadOnly = true;
                    for (int i = 0; i < Convert.ToInt32(textBox1.Text); i++)
                        dataGridView1.Columns.Add("X" + (i + 1).ToString(), "X" + (i + 1).ToString());
                }
                else
                {
                    dataGridView1.Columns.Clear();
                    dataGridView1.Columns.Add("0", "Var.Nombre"); dataGridView1.Columns["0"].ReadOnly = true;
                    for (int i = 0; i < Convert.ToInt32(textBox1.Text); i++)
                        dataGridView1.Columns.Add("X" + (i + 1).ToString(), "X" + (i + 1).ToString());
                }
                dataGridView1.Columns.Add("condiciones", "<,>,Ó =");
                dataGridView1.Columns.Add("RHS", "R.H.S");                
                //////////////////////////////////////////////////// Parte para agregar las filas
                if(corregir==false)
                for (int i = 0; i < 1 + Convert.ToInt32(textBox2.Text); i++)                
                    dataGridView1.Rows.Add();
                else
                {
                    dataGridView1.Rows.Clear();
                    for (int i = 0; i < 1 + Convert.ToInt32(textBox2.Text); i++)
                        dataGridView1.Rows.Add();
                }
                /////////////////////////////////////////////////// Parte para renombrar las Filas
                dataGridView1.Rows[0].Cells[0].Value = "Max Z";
                for (int i = 1; i <= Convert.ToInt32(textBox2.Text); i++)
                {
                    dataGridView1.Rows[i].Cells[0].Value = "Restric" + i.ToString();
                }
                ceros();
                button2.Enabled = true;                
            } catch(Exception a) { MessageBox.Show("" + a.Message, "Llena los datos necesarios", MessageBoxButtons.OK, MessageBoxIcon.Warning); }
        }
        void ceros()
        {
            for (int i = 0; i <= Form1.n; i++)
                for (int j = 1; j <= Form1.m; j++)
                    dataGridView1.Rows[i].Cells[j].Value="0.0";
            for (int i = 1; i <= Form1.n; i++)
                dataGridView1.Rows[i].Cells["condiciones"].Value = "<=";
            for (int i = 1; i <= Form1.n; i++)
                dataGridView1.Rows[i].Cells["RHS"].Value = "0.0";
        }
        private void button2_Click(object sender, EventArgs e) //mandar a resolver problema (Seleccionar metodo de resolución)
        {
            try {

                llenarTabla(); MaxMin = dataGridView1.Rows[0].Cells[0].Value.ToString();
                Metodo elegir = new Metodo();
                elegir.Show(this);
            }
            catch (Exception a) { MessageBox.Show("Por Favor Revisa que los datos de la tabla estén llenados correctamente\n"+a.Message, "Advertencia", MessageBoxButtons.OK, MessageBoxIcon.Error); }
        }       
        public void Metodos() //Parte que creará la respectiva ventana al método Seleccionado
        {
            switch (procedimiento)
            {
                case "Método Simplex(Original)":
                    Simplex ventana = new Simplex();
                    ventana.Show(this);                  
                    break;
                case "Método de Doble Fase":
                    DobleFase Ventfase = new DobleFase();
                    Ventfase.Show(this);
                    break;
                case "Gráfico":
                    Grafico grafico = new Grafico();
                    grafico.Show(this);
                    break;
                case "Método de la M Grande":
                    Mgrande grande = new Mgrande();
                    grande.Show(this);
                    break;
            }
        }     
        void llenarTabla() //Pasa los datos del tadaGriedview en un array string
        {
            for (int i = 0; i <= Convert.ToInt32(textBox2.Text); i++)
            {
                for (int j = 1; j <= Convert.ToInt32(textBox1.Text) + 2; j++)
                {
                    tabla[i, j] = Convert.ToString(dataGridView1.Rows[i].Cells[j].Value);
                }
            }
            /*for (int i = 0; i <= Convert.ToInt32(textBox2.Text) ; i++)
            {
                for (int j = 1; j <= Convert.ToInt32(textBox1.Text) + 2; j++)
                {
                    MessageBox.Show("" + tabla[i, j], i.ToString() + "," + j.ToString());
                }
            }*/
        }
        private void dataGridView1_CellContentClick(object sender, DataGridViewCellEventArgs e) //Cambiar Min-Maximizar
        {
            int row = dataGridView1.CurrentCell.RowIndex;
            int col = dataGridView1.CurrentCell.ColumnIndex;
            if (row == 0 & col == 0)
                if (dataGridView1.Rows[0].Cells[0].Value == "Max Z")
                    dataGridView1.Rows[0].Cells[0].Value = "Min Z";
                else if (dataGridView1.Rows[0].Cells[0].Value == "Min Z")
                    dataGridView1.Rows[0].Cells[0].Value = "Max Z";
                else return;

        }
    }
}
