using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;
using info.lundin.Math;
using System.Windows.Forms.DataVisualization.Charting;
namespace IOMaker
{
    public partial class Grafico : Form
    {
        double[,] Valores = new double[Form1.n +1,(Form1.n+Form1.m)];
        double x1 = 0, x2 = 0, y1 = 0, y2 = 0, m = 0, Y = 0; double movx = 0,movy=0;
        double[] X1 = new double[Form1.n + 1];
        double[] X2 = new double[Form1.n + 1];
        double[] Y1 = new double[Form1.n + 1];
        double[] Y2 = new double[Form1.n + 1];
        double[]  M = new double[Form1.n + 1];
        public Grafico()
        {
            InitializeComponent();
        }      
        private void checkBox1_CheckedChanged(object sender, EventArgs e)
        {
            if (checkBox1.Checked == true)
            {
                chart1.ChartAreas[0].Area3DStyle.Enable3D = true;
                chart1.ChartAreas[0].Area3DStyle.Perspective = 2;
            }
            else
            {
                chart1.ChartAreas[0].Area3DStyle.Enable3D = false;
            }            
        }
        private void Grafico_Load(object sender, EventArgs e)
        {
            chart1.Series.Add("óptimo");
            chart1.Series["óptimo"].ChartType = SeriesChartType.Point;
            chart1.Series["óptimo"].Color = Color.DarkBlue;
            chart1.Series["óptimo"].MarkerSize = 15;            
            for (int i = 1; i <= Form1.n; i++)
            {                
                chart1.Series.Add("R" + i.ToString());
                chart1.Series["R" + i.ToString()].ChartType = SeriesChartType.Line;
                chart1.Series["R" + i.ToString()].BorderWidth = 3;                              
            }
            Letras();
        }       
        void Letras()
        {
            string aux = string.Empty;
            textBox2.Text = "";
            aux = Form1.MaxMin+"=";
            for (int j = 1; j <= Form1.m; j++)
            {
                if (Convert.ToDouble(Form1.tabla[0, j]) < 0)
                    aux += Form1.tabla[0, j] + "X" + j.ToString();
                else
                    aux += "+" + Form1.tabla[0, j] + "X" + j.ToString();
            }
            textBox2.Text = aux+Environment.NewLine;
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
        void ValoresXY(int i)
        {
            try {
                X1[i] = 0; X2[i] = Convert.ToDouble(Form1.tabla[i, Form1.m + 2]) / Convert.ToDouble(Form1.tabla[i, 2]);
                Y2[i] = 0; Y1[i] = Convert.ToDouble(Form1.tabla[i, Form1.m + 2]) / Convert.ToDouble(Form1.tabla[i, 1]);
            }catch(Exception a) { MessageBox.Show("No es posible Graficar este modelo", "erro", MessageBoxButtons.OK, MessageBoxIcon.Error); }

        }
        void graficar()
        {
            try {
                for (int i = 1; i <= Form1.n; i++)
                {
                    ValoresXY(i);
                    chart1.Series["R" + i.ToString()].Points.AddXY(X1[i], X2[i]);
                    chart1.Series["R" + i.ToString()].Points.AddXY(Y1[i], Y2[i]);
                    if (i == Form1.n)
                    {
                        chart1.ChartAreas[0].BackColor = Color.Black;
                        chart1.ChartAreas[0].BorderColor = Color.White;
                        chart1.ChartAreas[0].BorderWidth = 2;
                        chart1.ChartAreas[0].AxisY.MaximumAutoSize = 100;
                        chart1.ChartAreas[0].AxisX.Minimum = X1.Min();
                        chart1.ChartAreas[0].AxisX.Interval = double.Parse(Form1.tabla[1, Form1.m + 2]) / 10;

                    }
                }
            } catch(Exception a) { MessageBox.Show("No es posible Graficar este modelo", "erro", MessageBoxButtons.OK, MessageBoxIcon.Error); }
        }
        private void timer1_Tick(object sender, EventArgs e,double i)
        {            
                chart1.Series["point"].Points.AddXY(movx, 0);
        }
        void Point()
        {           
            chart1.Series["óptimo"].Points.AddXY(Convert.ToDouble(Form1.punto[1]), Convert.ToDouble(Form1.punto[2]));
        }
        private void button1_Click(object sender, EventArgs e)
        {
            bool seguir = true;
            for (int i = 1; i <= Form1.n; i++)
                for (int j = 1; j <= Form1.m; j++)
                {
                    if (seguir = false) break;
                    if (Form1.tabla[i, j] == "0" || Form1.tabla[i, j] == "0.0")
                    {
                        seguir = false; break;
                    }
                }
            if (seguir = false)
            {
                MessageBox.Show("No es posible Graficar este modelo", "erro", MessageBoxButtons.OK, MessageBoxIcon.Error);
                button1.Enabled = false;
            }
            else
            {
                try
                {
                    Point();
                    graficar();
                    textBox1.Text = Form1.ResFinal; textBox1.Visible = true; textBox1.Enabled = true;
                    Form1.ResFinal = string.Empty; button1.Enabled = false;
                }
                catch (Exception a) { MessageBox.Show("No es posible Graficar este modelo", "erro", MessageBoxButtons.OK, MessageBoxIcon.Error); }
            }
        }
    }
}
