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
    public partial class DobleFase : Form
    {
        bool salir = false, optimo = false, Segfase = false, optimosegfase = false;
        string[,] Tabla = new string[100, 100];
        string Entra = "", Sale = "", EP = "";        
        int EPi = 0, ITE = 0, ITE2=0, posx = 150, posdtgy = 145, W = 0, contador = Form1.m+1;      
        List<DGVS> tablas = new List<DGVS>();
        public DobleFase()
        {
            InitializeComponent();
        }
        private void DobleFase_Load(object sender, EventArgs e)
        {
            creartabla();
            LlenarTabla();
            Letras();
            Vectorunitario();
            copiartabla();
            VarEntraSale();
            colorear();           
            ITE++;
        }        
        private void siguienteToolStripMenuItem_Click(object sender, EventArgs e)
        {
            if (Segfase == false)
            {
                if (optimo == false)
                {                    
                    CrearLabels();                    
                    Sumatoria();
                    copiartabla();
                    VarEntraSale();
                    colorear();
                    ITE++;
                    optimo = Optimalidad1fase();
                }
                else if (optimo == true)
                {
                    for (int i = 1; i <= W; i++)
                        dataGridView1.Columns.Remove("W" + i.ToString());
                    MessageBox.Show("Se ha obtenido la Primera Fase", "Primera fase obtenedia");                    
                    Segfase = true;
                    dataGridView1.Columns[0].HeaderText = "Z"; dataGridView1.Columns[dataGridView1.Columns.Count-2].HeaderText = "Z0";
                    inicio2da();
                    W = 0;
                    ITE = 0;
                }
            }
            else
            {
                optimosegfase = Optimalidad1fase();
                if (optimosegfase == true)
                {
                    Resultados();
                    MessageBox.Show("Se ha completado Con éxito la segunda Fase", "Resultados obtenidos");
                }
                else
                {
                    ITE++;
                    CrearLabels();
                    VarEntraSale();
                    Sumatoria();
                    copiartabla();
                    ITE2++;
                    colorear();
                    Optimalidad1fase();
                }
            }
        }
        void creartabla()
        {   /////////////////////////////////////////////////LLenar la cantidad de Filas
            int vari = 1 ;
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
            dataGridView1.Columns.Add("Z0", "L0");
            dataGridView1.Columns.Add("VB", "Var.Básicas");
            for (int i = 0; i < dataGridView1.Columns.Count - 1; i++)
                dataGridView1.Columns[i].Width = 50;
            dataGridView1.Columns[dataGridView1.Columns.Count - 1].Width = 70; 
        }
        void LlenarTabla()
        { //Función Objetivo 
            int ww=1,xx=Form1.m;           
            for (int i = 1; i <= Form1.n; i++)            
                dataGridView1.Rows[i].Cells["Z0"].Value = Form1.tabla[i, Form1.m + 2];     dataGridView1.Rows[0].Cells["Z0"].Value = 0;
            for (int i = (dataGridView1.Columns.Count-2-W ); i <= (dataGridView1.Columns.Count-3); i++){
                if (Form1.MaxMin == "Max Z")
                    dataGridView1.Rows[0].Cells[i].Value = 1;
                else if(Form1.MaxMin == "Min Z")
                    dataGridView1.Rows[0].Cells[i].Value = -1;
            }
            for (int i = 1; i <= Form1.n; i++) //llenado del vector de valores (funciona)            
                for (int j = 1; j <= Form1.m; j++)                
                    dataGridView1.Rows[i].Cells[j].Value = Form1.tabla[i, j];               
            for(int i=1;i<=Form1.n;i++) //Llenado de las Variables Básicas
            {
                if (Form1.tabla[i,Form1.m+1] == "<=")
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
            for (int j = 1; j <= contador-1; j++)//Parte de la fun Objetivo (ceros iniciales)
                dataGridView1.Rows[0].Cells[j].Value = 0;            
            for (int i = 1; i <= Form1.n; i++)
                for(int j=(Form1.m+1);j<=(W+contador-1);j++) //LLenado de los vectores unitarios
                {
                    if (dataGridView1.Rows[i].Cells["VB"].Value.ToString() == dataGridView1.Columns[j].Name)
                        dataGridView1.Rows[i].Cells[j].Value = 1;
                    else dataGridView1.Rows[i].Cells[j].Value = 0;
                }
            for(int i=1;i<=Form1.n;i++)
            {
                if(Form1.tabla[i, Form1.m + 1] == ">=" || Form1.tabla[i, Form1.m + 1] == "<=")
                xx++;
                if (Form1.tabla[i, Form1.m + 1] == ">=")
                    dataGridView1.Rows[i].Cells[xx].Value = -1;
            }                
        }  //Aquí comienza primera FAse        
        void Vectorunitario()
        {
            for(int i=1;i<=Form1.n;i++)
            {
                if(dataGridView1.Rows[0].Cells[dataGridView1.Rows[i].Cells["VB"].Value.ToString()].Value.ToString()!="0")
                {
                    for (int j = 1; j <= dataGridView1.Columns.Count-2; j++)
                    {
                        if(Form1.MaxMin=="Min Z")
                        dataGridView1.Rows[0].Cells[j].Value = Convert.ToDouble(dataGridView1.Rows[0].Cells[j].Value.ToString()) + (Convert.ToDouble(dataGridView1.Rows[i].Cells[j].Value.ToString()) * 1);
                        else if(Form1.MaxMin=="Max Z")
                        dataGridView1.Rows[0].Cells[j].Value = Convert.ToDouble(dataGridView1.Rows[0].Cells[j].Value.ToString()) + (Convert.ToDouble(dataGridView1.Rows[i].Cells[j].Value.ToString()) * -1);
                    }
                }
            }
        }
        private void todasLasIteracionesToolStripMenuItem_Click(object sender, EventArgs e)
        {
            do
            {
                siguienteToolStripMenuItem.PerformClick();   
            } while (optimosegfase == false);
        }
        private void gráficoToolStripMenuItem_Click(object sender, EventArgs e)
        {
            Grafico graficar = new Grafico();
            graficar.Show(this);
        }
        void VarEntraSale()
        {
            try {
                Entra = ""; Sale = ""; EP = "";
                double Menor = 0, Mayor = 0;
                double[] Aux = new double[dataGridView1.Columns.Count - 2];
                if (Form1.MaxMin == "Max Z")
                    Aux[0] = 10000000000000000000;
                else if (Form1.MaxMin == "Min Z")
                    Aux[0] = -1000000000000000000;
                for (int j = 1; j <= (dataGridView1.Columns.Count - 3); j++)  //Cuando Ite>0 hará esto     
                    Aux[j] = Convert.ToDouble(dataGridView1.Rows[0].Cells[j].Value);
                if (Form1.MaxMin == "Max Z")
                    Menor = Aux.Min();
                else if (Form1.MaxMin == "Min Z")
                    Mayor = Aux.Max();
                if (Form1.MaxMin == "Max Z") //Para cuando es Maximizar
                    for (int j = 1; j <= (dataGridView1.Columns.Count - 3); j++)//Calcular la Var que Entra (funciona)
                    {
                        if (dataGridView1.Rows[0].Cells[j].Value.ToString() == Menor.ToString())
                        {
                            if (j < (dataGridView1.Columns.Count - 2 - W))
                                Entra = "X" + j.ToString();
                            else if (j >= (dataGridView1.Columns.Count - 2 - W))
                                Entra = "W" + (j - contador).ToString();
                            // MessageBox.Show("Var que entra=" + "X" + j.ToString(), "");
                            break;
                        }

                    } //////////////////////////////////////////////////////////////////Fin de la Var que Entra
                else if (Form1.MaxMin == "Min Z") //Para cuando es Minimizar
                    for (int j = 1; j <= (dataGridView1.Columns.Count - 3); j++)//Calcular la Var que Entra (funciona)
                    {
                        if (dataGridView1.Rows[0].Cells[j].Value.ToString() == Mayor.ToString())
                        {
                            if (j < (dataGridView1.Columns.Count - 2 - W))
                                Entra = "X" + j.ToString();
                            else if (j >= (dataGridView1.Columns.Count - 1 - W))
                                Entra = "W" + (j - contador).ToString();
                            //MessageBox.Show("Var que entra=" + "X" + j.ToString(), "");
                            break;
                        }

                    } //////////////////////////////////////////////////////////////////Fin de la Var que Entra
                double[] Divi = new double[Form1.n + 1]; Divi[0] = 10000000000000000000000000.00;
                int negativos = 0;
                for (int i = 1; i <= Form1.n; i++)
                {
                    if ((Convert.ToDouble(dataGridView1.Rows[i].Cells["Z0"].Value.ToString()) / Convert.ToDouble(dataGridView1.Rows[i].Cells[Entra.ToString()].Value.ToString())) < 0 || Convert.ToDouble(dataGridView1.Rows[i].Cells[Entra.ToString()].Value.ToString()) == 0)
                    {
                        Divi[i] = 100000000000000000.00; negativos++;
                    }
                    else if (Convert.ToDouble(dataGridView1.Rows[i].Cells["Z0"].Value.ToString()) / Convert.ToDouble(dataGridView1.Rows[i].Cells[Entra.ToString()].Value.ToString()) > 0)
                    {
                        Divi[i] = Convert.ToDouble(dataGridView1.Rows[i].Cells["Z0"].Value.ToString()) / Convert.ToDouble(dataGridView1.Rows[i].Cells[Entra.ToString()].Value.ToString());
                    }
                    else if (dataGridView1.Rows[i].Cells["Z0"].Value.ToString() == "0")
                    {
                        if (double.Parse(dataGridView1.Rows[i].Cells[Entra.ToString()].Value.ToString()) > 0)
                        {
                            Divi[i] = 10000000000000000000.00 * -1;
                        }
                        else
                        {
                            Divi[i] = 100000000000000000.00; negativos++;
                        }
                    }
                }
                if (negativos == Form1.n)
                {
                    MessageBox.Show("El problema no tiene Solución!", "sin solución", MessageBoxButtons.OK, MessageBoxIcon.Exclamation);
                    optimo = true;
                    menuStrip1.Enabled = false;
                }
                else
                {
                    for (int i = 1; i <= (Form1.n); i++)//Calcular la Var que Sale
                    {
                        if (Divi[i] == 10000000000000000000.00 * -1)
                        {
                            Sale = dataGridView1.Rows[i].Cells["VB"].Value.ToString();
                            EPi = i;
                            //MessageBox.Show("Var que Sale=" + Sale, "");
                            break;
                        }
                        else if (Divi[i] != 10000000000000000000.00 * -1 && Convert.ToDouble(dataGridView1.Rows[i].Cells["Z0"].Value.ToString()) / Convert.ToDouble(dataGridView1.Rows[i].Cells[Entra.ToString()].Value.ToString()) == Divi.Min())
                        {
                            Sale = dataGridView1.Rows[i].Cells["VB"].Value.ToString();
                            EPi = i;
                            //MessageBox.Show("Var que Sale=" + Sale, "");
                            break;
                        }
                    } ///////////////////Fin de la variable que sale ite>0                  
                }
                EP = dataGridView1.Rows[EPi].Cells[Entra].Value.ToString();
            }
            catch(Exception a) { MessageBox.Show("El problema no tiene Solución!", "sin solución", MessageBoxButtons.OK, MessageBoxIcon.Exclamation); }
        }
        void Sumatoria()
        {
            try {
                for (int j = 1; j <= dataGridView1.Columns.Count - 2; j++) //Multiplicando toda la fila por el recíproco
                {
                    //MessageBox.Show(dataGridView1.Columns[Entra].Index.ToString());
                    if (Convert.ToDouble(EP) < 0)
                    {
                        if (j == dataGridView1.Columns[Entra].Index)
                            dataGridView1.Rows[EPi].Cells[j].Value = 1;
                        else
                            dataGridView1.Rows[EPi].Cells[j].Value = (Convert.ToDouble(dataGridView1.Rows[EPi].Cells[j].Value.ToString()) * (1 / Convert.ToDouble(EP)));
                    }
                    else
                    {
                        if (j == dataGridView1.Columns[Entra].Index)
                            dataGridView1.Rows[EPi].Cells[j].Value = 1;
                        else
                            dataGridView1.Rows[EPi].Cells[j].Value = (Convert.ToDouble(dataGridView1.Rows[EPi].Cells[j].Value.ToString()) * (1 / Convert.ToDouble(EP)));
                    }
                }
                for (int i = 0; i <= Form1.n; i++) //parte que hace la sumatoria
                {
                    if (i == EPi) continue;
                    else
                    {
                        double x = Convert.ToDouble(dataGridView1.Rows[i].Cells[Entra].Value.ToString()) * -1;
                        for (int j = 1; j <= dataGridView1.Columns.Count - 2; j++)
                        {
                            if ((Convert.ToDouble(dataGridView1.Rows[i].Cells[j].Value.ToString()) + (Convert.ToDouble(dataGridView1.Rows[EPi].Cells[j].Value.ToString()) * x)) < 0.0000001 && ((Convert.ToDouble(dataGridView1.Rows[i].Cells[j].Value.ToString()) + (Convert.ToDouble(dataGridView1.Rows[EPi].Cells[j].Value.ToString()) * x)) > -0.0000001))
                                dataGridView1.Rows[i].Cells[j].Value = 0;
                            else
                                dataGridView1.Rows[i].Cells[j].Value = Convert.ToDouble(dataGridView1.Rows[i].Cells[j].Value.ToString()) + (Convert.ToDouble(dataGridView1.Rows[EPi].Cells[j].Value.ToString()) * x);
                        }

                    }
                }
                dataGridView1.Rows[EPi].Cells["VB"].Value = Entra;
            }catch(Exception a) { MessageBox.Show("El problema no tiene Solución!", "sin solución", MessageBoxButtons.OK, MessageBoxIcon.Exclamation); }
        }
        bool Optimalidad1fase()
        {
            bool optimalidad = true;
            if (Form1.MaxMin == "Max Z")
                for (int j = 1; j <= dataGridView1.Columns.Count - 3; j++)
                {
                    if (Convert.ToDouble(dataGridView1.Rows[0].Cells[j].Value.ToString()) < 0)
                    {
                        optimalidad = false; break;
                    }
                }
            else if (Form1.MaxMin == "Min Z")
                for (int j = 1; j <= dataGridView1.Columns.Count - 3; j++)
                {
                    if (Convert.ToDouble(dataGridView1.Rows[0].Cells[j].Value.ToString()) > 0)
                    {
                        optimalidad = false; break;
                    }
                }
            //MessageBox.Show(optimalidad.ToString());
            return optimalidad;
        }
        void colorear()
        {
            for(int i = 0; i <= dataGridView1.Columns.Count - 1; i++)
                tablas[tablas.Count - 1].DGV.Rows[EPi].Cells[i].Style.BackColor = Color.Orange;
            for (int i = 0; i <= dataGridView1.Rows.Count - 1; i++)
                tablas[tablas.Count - 1].DGV.Rows[i].Cells[Entra].Style.BackColor = Color.Orange;
            tablas[tablas.Count - 1].DGV.Rows[EPi].Cells[Entra].Style.BackColor = Color.Red;
        }
        void inicio2da() //Aquí cmienza la Segunda FAse
        {
            try {
                string EPA = string.Empty;
                //for (int j = 1; j <= W; j++) //Eliminar Variables de Penalización
                //    dataGridView1.Columns.Remove("W" + j.ToString());
                for (int i = 1; i <= Form1.m; i++) //Pasar los valores originales de la Func.Objetivo
                    dataGridView1.Rows[0].Cells[i].Value = Form1.tabla[0, i].ToString();
                for (int i = 1; i <= Form1.n; i++)
                {//Elemento Pivote
                    EP = dataGridView1.Rows[i].Cells[dataGridView1.Rows[i].Cells["VB"].Value.ToString()].Value.ToString();
                    EPi = i;
                    Entra = dataGridView1.Rows[i].Cells["VB"].Value.ToString();
                    sumatorio2da();

                }
            } catch(Exception a) { MessageBox.Show("Est problema no puedeconcluirse por este método,\n debido a que una variable de penalizáción no salió de la base",a.Message); menuStrip1.Enabled = false; }
        }
        void sumatorio2da()
        {
            for (int j = 1; j <= dataGridView1.Columns.Count - 2; j++) //Multiplicando toda la fila por el recíproco
            {
                //MessageBox.Show(dataGridView1.Columns[Entra].Index.ToString());
                if (Convert.ToDouble(EP) < 0)
                {
                    if (j == dataGridView1.Columns[Entra].Index)
                        dataGridView1.Rows[EPi].Cells[j].Value = 1;
                    else
                        dataGridView1.Rows[EPi].Cells[j].Value = (Convert.ToDouble(dataGridView1.Rows[EPi].Cells[j].Value.ToString()) * (1 / Convert.ToDouble(EP)));
                }
                else
                {
                    if (j == dataGridView1.Columns[Entra].Index)
                        dataGridView1.Rows[EPi].Cells[j].Value = 1;
                    else
                        dataGridView1.Rows[EPi].Cells[j].Value = (Convert.ToDouble(dataGridView1.Rows[EPi].Cells[j].Value.ToString()) * (1 / Convert.ToDouble(EP)));
                }
            }
            for (int i = 0; i <= Form1.n; i++) //parte que hace la sumatoria
            {
                if (i == EPi) continue;
                else
                {
                    double x = Convert.ToDouble(dataGridView1.Rows[i].Cells[Entra].Value.ToString()) * -1;
                    for (int j = 1; j <= dataGridView1.Columns.Count - 2; j++)
                    {
                        if ((Convert.ToDouble(dataGridView1.Rows[i].Cells[j].Value.ToString()) + (Convert.ToDouble(dataGridView1.Rows[EPi].Cells[j].Value.ToString()) * x)) < 0.0000001 && ((Convert.ToDouble(dataGridView1.Rows[i].Cells[j].Value.ToString()) + (Convert.ToDouble(dataGridView1.Rows[EPi].Cells[j].Value.ToString()) * x)) > -0.0000001))
                            dataGridView1.Rows[i].Cells[j].Value = 0;
                        else
                            dataGridView1.Rows[i].Cells[j].Value = Convert.ToDouble(dataGridView1.Rows[i].Cells[j].Value.ToString()) + (Convert.ToDouble(dataGridView1.Rows[EPi].Cells[j].Value.ToString()) * x);
                    }

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
        void CrearLabels()
        {
            try { 
            string valores = "";
            for (int i = 1; i <= Form1.n; i++)
                valores += dataGridView1.Rows[i].Cells[Entra].Value.ToString() + ",";
            Label entrarsalir = new Label(); //Parte que escribirá que que variable entra y cuál sale
            entrarsalir.Size = new System.Drawing.Size(700, 50);
            entrarsalir.Text = "Insertamos la variable " + Entra + ",y localizamos la Fila a abandonar la Base";
            entrarsalir.Text += "\n𝞱= min(βi / aip) = min{" + valores.Substring(0, valores.Length - 1) + "}=" + EP;
            entrarsalir.Tag = "Label" + ITE.ToString();
            entrarsalir.Location = new Point(25, posx + 35); posx += 35;
            entrarsalir.Font = new Font(entrarsalir.Font.FontFamily, 11);
            panel1.Controls.Add(entrarsalir);
            Label myLabel = new Label();
            // Set the label's Text and ID properties.            
            myLabel.Size = new System.Drawing.Size(150, 25);
            myLabel.Text = "Iteración =" + ITE;
            myLabel.Tag = "Label" + ITE.ToString();
            myLabel.Location = new Point((ActiveForm.Width / 4) * 3 + 25, posx + 20); posx += 20;
            myLabel.Font = new Font(myLabel.Font.FontFamily, 13);
            panel1.Controls.Add(myLabel);
            posdtgy += posx;
            }
            catch(Exception a) { MessageBox.Show("El problema no tiene Solución!", "sin solución", MessageBoxButtons.OK, MessageBoxIcon.Exclamation); }
        }
        void copiartabla()
        {
            try {
                DGVS myDGV = new DGVS();
                ///////////////////////////////////////////////Llenar la cantidad de Columnas
                //myDGV.DGV.Columns.Add("Z", "Z");
                for (int j = 0; j <= dataGridView1.Columns.Count - 1; j++)
                {
                    myDGV.DGV.Columns.Add(dataGridView1.Columns[j].Name, dataGridView1.Columns[j].HeaderText);
                }
                for (int i = 0; i < Form1.n + 1; i++)
                    myDGV.DGV.Rows.Add();
                for (int i = 0; i <= (dataGridView1.Rows.Count - 1); i++)
                    for (int j = 0; j <= (dataGridView1.Columns.Count - 1); j++)
                        myDGV.DGV.Rows[i].Cells[j].Value = dataGridView1.Rows[i].Cells[j].Value;
                for (int i = 0; i < dataGridView1.Columns.Count - 1; i++)
                    myDGV.DGV.Columns[i].Width = 50;
                myDGV.DGV.Columns[dataGridView1.Columns.Count - 1].Width = 70;
                myDGV.DGV.Visible = true; myDGV.DGV.ReadOnly = true;
                myDGV.DGV.AllowUserToAddRows = false;
                myDGV.DGV.AllowUserToDeleteRows = false;
                myDGV.DGV.AllowUserToOrderColumns = false;
                myDGV.DGV.Tag = "DTG" + ITE.ToString();
                myDGV.DGV.Size = new Size(570, 200);
                if (optimo == false)
                {
                    if (ITE == 0)
                        myDGV.DGV.Location = new Point(165, posx);
                    else if (ITE > 0)
                        myDGV.DGV.Location = new Point(165, posx + 35);
                }
                else
                {
                    if (ITE2 == 0)
                    {
                        Label fase2 = new Label();
                        fase2.Size = new System.Drawing.Size(150, 25);
                        fase2.Text = "Segunda Fase";
                        fase2.Tag = "Label" + ITE.ToString();
                        fase2.Location = new Point((ActiveForm.Width / 4) * 2 - 75, posx + 35); posx += 20;
                        fase2.Font = new Font(fase2.Font.FontFamily, 13);
                        panel1.Controls.Add(fase2);
                        myDGV.DGV.Location = new Point(165, posx + 40); 
                    }
                    else if (ITE2 >= 1)
                        myDGV.DGV.Location = new Point(165, posx + 35);
                }
                tablas.Add(myDGV);                
                panel1.Controls.Add(tablas[tablas.Count - 1].DGV);         posx += 220;                                    
            }catch(Exception a) { MessageBox.Show("El problema no tiene Solución!", "sin solución", MessageBoxButtons.OK, MessageBoxIcon.Exclamation);optimosegfase = true; }
            if (tablas.Count <= 4)
                panel1.Size = new Size(870, panel1.Height + 270);
            else
            {
                panel1.Size = new Size(870, panel1.Height + 600);
                vScrollBar1.Maximum += 220;
            }
        } //corregir esta parta , una para copiar con W y otra con puras X'S
        void Resultados()
        {
            string aux = string.Empty;
            panel1.Height += 120;
            TextBox Resultados = new TextBox(); //Parte que escribirá que que variable entra y cuál sale
            Resultados.Size = new System.Drawing.Size(570, 120); Resultados.Multiline = true;
            Resultados.Text = "Puntos Óptimos:\n";
            Resultados.Tag = "Labela" + ITE.ToString();
            Resultados.Location = new Point(165, tablas[tablas.Count - 1].DGV.Location.Y + tablas[tablas.Count - 1].DGV.Height);
            Resultados.TextAlign = HorizontalAlignment.Center;
            Resultados.Font = new Font(Resultados.Font.FontFamily, 11); Resultados.Visible = true;
            Resultados.ReadOnly = true;
            posx = 10;
            string[] vals = new string[Form1.m + 1];
            for (int i = 1; i <= Form1.m; i++)
            {
                vals[i] = "X" + i.ToString() + "=0";
                for (int j = 1; j <= Form1.n; j++)
                {
                    if (vals[i].Substring(0, vals[i].Length - 2) == dataGridView1.Rows[j].Cells["VB"].Value.ToString())
                    {
                        vals[i] = vals[i].Substring(0, vals[i].Length - 1) + dataGridView1.Rows[j].Cells["Z0"].Value.ToString(); break;
                    }                   
                }
            }
            for (int i = 1; i <= Form1.m; i++)
            {
                Form1.ResFinal += Environment.NewLine + vals[i];
                Resultados.Text += Environment.NewLine + vals[i];
            }
            if(Form1.m==2)            
                for (int i = 1; i <= Form1.m; i++)
                    Form1.punto[i] = vals[i].Substring(3, vals[i].Length - 3);            
            panel1.Controls.Add(Resultados);
            if(Form1.m==2 && optimosegfase==true)
                {
                gráficoToolStripMenuItem.Visible = true;
                }
        }
        private void vScrollBar1_Scroll(object sender, ScrollEventArgs e)
        {
            panel1.Location = new Point(7, -vScrollBar1.Value + 16);
        }
    }
    }

