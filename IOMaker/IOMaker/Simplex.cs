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
    
    public partial class Simplex : Form
    {
        bool salir = false, optimo = false;            
        string[,] Tabla = new string[100,100];
        string Entra = "", Sale = "",EP="";
        int EPi = 0, ITE = 0, posx = 150, posdtgy = 145;
        List<DGVS> tablas = new List<DGVS>();

        public Simplex()
        {
            InitializeComponent();
        }
        private void Simplex_Load(object sender, EventArgs e)
        {            
            creartabla();
            llenartabla();            
            Letras();            
            copiartabla();            
            if (Form1.MaxMin == "Max Z")
                VarSaleEntraMAX();
            else if (Form1.MaxMin == "Min Z")
                VarSaleEntraMIN();
            colorear();
            optimo = Optimo();
            if (optimo == true)
                MessageBox.Show("La tabla ya se encuentra balanceada!", "Tabla óptima", MessageBoxButtons.OK, MessageBoxIcon.Asterisk);            
            ITE++;
        }
        private void siguienteIteraciónToolStripMenuItem_Click(object sender, EventArgs e)
        {
            optimo = Optimo();
            if (optimo==false)
            {
                CrearLabels();                
                Sumatoria();
                copiartabla();
                if (Form1.MaxMin == "Max Z")
                    VarSaleEntraMAX();
                else if (Form1.MaxMin == "Min Z")
                    VarSaleEntraMIN();                
                colorear();                               
                 ITE++;                
            }
                else
                {
                dataGridView1.ReadOnly = false;                
                    MessageBox.Show("Esta es la tabla óptima!","Tabla óptima ya obtenida");
                Resultados();
                 }                                        
        }        
        void creartabla()
        {   /////////////////////////////////////////////////LLenar la cantidad de Filas
            for(int i=0;i<Form1.n+1;i++)
            {
                dataGridView1.Rows.Add();
                if (i == 0)
                    dataGridView1.Rows[0].Cells[0].Value = 1;
                else dataGridView1.Rows[i].Cells[0].Value = 0;
            }
            ///////////////////////////////////////////////Llenar la cantidad de Columnas
            for (int i = 1; i <=Form1.m+Form1.n; i++)            
                dataGridView1.Columns.Add("X" + i.ToString(), "X" + (i).ToString());                                                  

            dataGridView1.Columns.Add("Z0", "Z0"); 
            dataGridView1.Columns.Add("VB", "Var.Básicas");
            for (int i = 0; i < dataGridView1.Columns.Count-1; i++)
                dataGridView1.Columns[i].Width = 50;
                dataGridView1.Columns[dataGridView1.Columns.Count-1].Width = 70;
        }
        void copiartabla()
        {
            DGVS myDGV = new DGVS();                        
            ///////////////////////////////////////////////Llenar la cantidad de Columnas
            myDGV.DGV.Columns.Add("Z", "Z");
            for (int i = 1; i <= Form1.m + Form1.n; i++)
                myDGV.DGV.Columns.Add("X" + i.ToString(), "X" + (i).ToString());
            myDGV.DGV.Columns.Add("Z0", "Z0");
            myDGV.DGV.Columns.Add("VB", "Var.Básicas");
            for (int i = 0; i < dataGridView1.Columns.Count - 1; i++)
                myDGV.DGV.Columns[i].Width = 50;
            myDGV.DGV.Columns[dataGridView1.Columns.Count - 1].Width = 70;
            for (int i = 0; i < Form1.n + 1; i++)
                myDGV.DGV.Rows.Add();
            for (int i = 0; i <=(dataGridView1.Rows.Count - 1); i++)
                for (int j = 0; j <= (dataGridView1.Columns.Count - 1); j++)
                    myDGV.DGV.Rows[i].Cells[j].Value = dataGridView1.Rows[i].Cells[j].Value;
            myDGV.DGV.Visible = true; myDGV.DGV.ReadOnly = true;
            myDGV.DGV.AllowUserToAddRows = false;
            myDGV.DGV.AllowUserToDeleteRows = false;
            myDGV.DGV.AllowUserToOrderColumns = false;
            myDGV.DGV.Tag = "DTG" + ITE.ToString();
            myDGV.DGV.Size = new Size(570,200);
           if(ITE==0)
            myDGV.DGV.Location = new Point(165, posx);             
           else if(ITE>0)
                myDGV.DGV.Location = new Point(165, posx+35);
            tablas.Add(myDGV);
            panel1.Controls.Add(tablas[tablas.Count-1].DGV); posx += 220;
            if (tablas.Count <= 4)
                panel1.Height += 150;
            else
            {
                panel1.Size = new Size(870, panel1.Size.Height + 400);
                vScrollBar1.Maximum += 220;
            }
        }
        void llenartabla()
        {
            //func objetivo
            for(int j=1;j<=(Form1.m+Form1.n)+1;j++)
            {
                if (j <= Form1.m)
                    dataGridView1.Rows[0].Cells[j].Value = Convert.ToDouble(Form1.tabla[0, j]);
                else dataGridView1.Rows[0].Cells[j].Value = 0;
            }   //Work
            for(int i=1;i<=Form1.n;i++) //llenado del vector de valores (funciona)
            {
                for(int j=1;j<=Form1.m;j++)
                {
                    dataGridView1.Rows[i].Cells[j].Value = Form1.tabla[i, j];
                }
            }   
            for(int i= Form1.n; i>=1;i--) //Listado de Variables básica (funciona)
            {
                dataGridView1.Rows[i].Cells["VB"].Value = "X"+(i+Form1.m).ToString();
            }
            //////////////////////////////// Llenado de los vectores unitarios
            for(int i=1;i<=Form1.n;i++)
            {
                for(int j=(Form1.m+1);j<=(Form1.m+Form1.n);j++)
                {
                    if (dataGridView1.Rows[i].Cells[dataGridView1.Columns.Count-1].Value.ToString() == dataGridView1.Columns[j].Name)
                    {
                        dataGridView1.Rows[i].Cells[j].Value = 1;
                    }
                    else
                        dataGridView1.Rows[i].Cells[j].Value = 0;
                }
            }/////Llenado de Valores de Z0
            for(int i=1;i<=Form1.n;i++)
            {
               dataGridView1.Rows[i].Cells["Z0"].Value = Form1.tabla[i, Form1.m +2];
            }
            for (int i = 0; i <= (dataGridView1.Rows.Count-1); i++)   //Pasar Valores De la nueva tabla 
            {
                for (int j = 0; j <= (dataGridView1.Columns.Count-1); j++)
                {
                    Tabla[i, j] = Convert.ToString(dataGridView1.Rows[i].Cells[j].Value);
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
                if(double.Parse(Form1.tabla[0,j])<0)
                aux += Form1.tabla[0, j] + "X"+j.ToString();
                else
                aux += "+"+Form1.tabla[0, j] + "X"+j.ToString();
            }
           textBox2.Text=aux +="=0"+Environment.NewLine;
            aux = string.Empty;
            for(int i=1;i<=Form1.n;i++)
            {
                for (int j = 1; j <= (Form1.m + 2); j++)
                {
                    if (j <= Form1.m)
                    {
                        if(Convert.ToDouble(Form1.tabla[i,j])<0)
                        aux += Form1.tabla[i, j] + "X"+j.ToString();
                        else
                        aux += "+"+Form1.tabla[i, j] + "X"+j.ToString();
                    }
                    else aux += Form1.tabla[i, j];
                }
                textBox2.Text += aux + Environment.NewLine; aux = "";                
            }            
        }
        void CrearLabels()
        {
            string valores="";
            for (int i = 1; i <= Form1.n; i++)
                valores += dataGridView1.Rows[i].Cells[Entra].Value.ToString() + ",";            
            Label entrarsalir = new Label(); //Parte que escribirá que que variable entra y cuál sale
            entrarsalir.Size = new System.Drawing.Size(700, 50);
            entrarsalir.Text = "Insertamos la variable " + Entra + ",y localizamos la Fila a abandonar la Base";
            entrarsalir.Text += "\n𝞱= min(βi / aip) = min{"+valores.Substring(0,valores.Length-1)+"}=" + EP;
                 entrarsalir.Tag = "Label" + ITE.ToString();
            entrarsalir.Location = new Point(25, posx + 35); posx += 35;
            entrarsalir.Font = new Font(entrarsalir.Font.FontFamily, 11);            
            panel1.Controls.Add(entrarsalir);
            Label myLabel = new Label();
            // Set the label's Text and ID properties.            
            myLabel.Size = new System.Drawing.Size(150, 25);
            myLabel.Text = "Iteración =" + ITE;
            myLabel.Tag = "Label" + ITE.ToString();
            myLabel.Location = new Point((ActiveForm.Width /4)*3+25 , posx + 20); posx += 20;
            myLabel.Font = new Font(myLabel.Font.FontFamily, 13);
            panel1.Controls.Add(myLabel);
            posdtgy += posx;
        }
        void Resultados()
        {
            string aux=string.Empty;
            panel1.Height += 120;
            TextBox Resultados = new TextBox(); //Parte que escribirá que que variable entra y cuál sale
            Resultados.Size = new System.Drawing.Size(570, 120); Resultados.Multiline = true;
            Resultados.Text = "Puntos Óptimos:\n";
            Resultados.Tag = "Labela" + ITE.ToString();
            Resultados.Location = new Point(165, tablas[tablas.Count-1].DGV.Location.Y+ tablas[tablas.Count - 1].DGV.Height);
            Resultados.TextAlign=HorizontalAlignment.Center;
            Resultados.Font = new Font(Resultados.Font.FontFamily, 11); Resultados.Visible = true;
            Resultados.ReadOnly = true;
            posx = 10;
            string[] vals = new string[Form1.m + 1];
            for (int i = 1; i <= Form1.m; i++)
            {
                vals[i] = "X" + i.ToString()+"=0";
                for(int j=1;j<=Form1.n;j++)
                {
                    if (vals[i].Substring(0,vals[i].Length-2) == dataGridView1.Rows[j].Cells["VB"].Value.ToString())
                    {
                        vals[i] = vals[i].Substring(0,vals[i].Length-1)+dataGridView1.Rows[j].Cells["Z0"].Value.ToString(); break;
                    }
                    //else
                    //    vals[i] ="X"+i.ToString()+"=0";
                }
            }
            for (int i = 1; i <= Form1.m; i++)
           {
                Form1.ResFinal += Environment.NewLine + vals[i];
                Resultados.Text += Environment.NewLine + vals[i];
            }
            if (Form1.m == 2)
                for (int i = 1; i <= Form1.m; i++)                
                    Form1.punto[i] = vals[i].Substring(3, vals[i].Length - 3);                                    
            panel1.Controls.Add(Resultados);
            if (Form1.m == 2 && optimo == true)
            {
                gráficoToolStripMenuItem.Visible = true;
            }
        }        
        void VarSaleEntraMAX()
        {
            Entra = ""; Sale = ""; EP = "";
            double[] Aux = new double[Form1.m+Form1.n+1];
            if (ITE == 0)
            {
                for (int j = 1; j <= Form1.m; j++)   //Si la itereación es 0, hará esto           
                    Aux[j] = Convert.ToDouble(dataGridView1.Rows[0].Cells[j].Value);
            }
            else if (ITE > 0)
            {
                for (int j = 1; j <= Form1.m + Form1.n; j++)  //Cuando Ite>0 hará esto     
                    Aux[j] = Convert.ToDouble(dataGridView1.Rows[0].Cells[j].Value);
            }
            double Menor = Aux.Min();
            if (ITE==0) //Calcular la Var que Entra (funciona)
            { 
            for (int j = 1; j <= Form1.m; j++)
            {
                if (dataGridView1.Rows[0].Cells[j].Value.ToString() == Menor.ToString())
                {
                    Entra = "X" + j.ToString();
                    //MessageBox.Show("Var que entra=" + "X" + j.ToString(), "");
                    break;
                }

            } //////////////////////////////////////////////////////////////////Fin de la Var que Entra
            }
            else if(ITE>0)  //Calcular Variable que entra cuando ite>0
            {
                for (int j = 1; j <= (Form1.m+Form1.n); j++)//Calcular la Var que Entra (funciona)
                {
                    if (dataGridView1.Rows[0].Cells[j].Value.ToString() == Menor.ToString())
                    {
                        Entra = "X" + j.ToString();
                        //MessageBox.Show("Var que entra=" + "X" + j.ToString(), "");
                        break;
                    }

                } //////////////////////////////////////////////////////////////////Fin de la Var que Entra
            }
            if (ITE == 0)  //////////////////////////////////////////////////////////Variable que sale cuando ite==0
            {
                double[] Divi = new double[Form1.n + 1]; Divi[0] = 10000000000000000000000000.00;
                for (int i = 1; i <= Form1.n; i++)
                {
                    if (Convert.ToDouble(dataGridView1.Rows[i].Cells["Z0"].Value.ToString()) == 0)
                    {
                        if ((Convert.ToDouble(dataGridView1.Rows[i].Cells["Z0"].Value.ToString()) / Convert.ToDouble(dataGridView1.Rows[i].Cells[Entra.ToString()].Value.ToString())) > 0)
                            Divi[i] = 0.0000000000000000001;
                        else
                        {
                            Divi[i] = 100000000000000000.00; 
                        }
                    }
                    if ((Convert.ToDouble(dataGridView1.Rows[i].Cells["Z0"].Value.ToString()) / Convert.ToDouble(dataGridView1.Rows[i].Cells[Entra.ToString()].Value.ToString())) < 0 || Convert.ToDouble(dataGridView1.Rows[i].Cells[Entra.ToString()].Value.ToString()) == 0)
                    {
                        Divi[i] = 100000000000000000.00;
                    }                   
                    else
                    {
                        Divi[i] = Convert.ToDouble(dataGridView1.Rows[i].Cells["Z0"].Value.ToString()) / Convert.ToDouble(dataGridView1.Rows[i].Cells[Entra].Value.ToString());
                    }
                }
                for (int i = 1; i <= Form1.n; i++)//Calcular la Var que Sale
                {
                    if (Convert.ToDouble(dataGridView1.Rows[i].Cells["Z0"].Value.ToString()) / Convert.ToDouble(dataGridView1.Rows[i].Cells[Entra].Value.ToString()) == Divi.Min())
                    {
                        Sale = dataGridView1.Rows[i].Cells["VB"].Value.ToString();
                        EPi = i;
                        //MessageBox.Show("Var que Sale=" + Sale, "");
                        break;
                    }

                } //////////////////////////////////////////////////////////////////Fin de la Var que sale ite=0
            }
            else if (ITE > 0)//Variable que sale cuando ite>0
            {
                double[] Divi = new double[Form1.n + 1]; Divi[0] = 10000000000000000000000000.00;
                int negativos = 0;
                for (int i = 1; i <= Form1.n; i++)
                {                      
                    if ((Convert.ToDouble(dataGridView1.Rows[i].Cells["Z0"].Value.ToString()) / Convert.ToDouble(dataGridView1.Rows[i].Cells[Entra.ToString()].Value.ToString())) < 0 || Convert.ToDouble(dataGridView1.Rows[i].Cells[Entra.ToString()].Value.ToString()) == 0)
                    {
                        Divi[i] = 100000000000000000.00; negativos++;
                    }                    
                    else if(Convert.ToDouble(dataGridView1.Rows[i].Cells["Z0"].Value.ToString()) / Convert.ToDouble(dataGridView1.Rows[i].Cells[Entra.ToString()].Value.ToString()) > 0 )
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
                        if(Divi[i]== 10000000000000000000.00 * -1)
                        {
                            Sale = dataGridView1.Rows[i].Cells["VB"].Value.ToString();
                            EPi = i;
                            //MessageBox.Show("Var que Sale=" + Sale, "");
                            break;
                        }
                        else if (Divi[i]!= 10000000000000000000.00 * -1 && Convert.ToDouble(dataGridView1.Rows[i].Cells["Z0"].Value.ToString()) / Convert.ToDouble(dataGridView1.Rows[i].Cells[Entra.ToString()].Value.ToString()) == Divi.Min())
                        {
                            Sale = dataGridView1.Rows[i].Cells["VB"].Value.ToString();
                            EPi = i;
                             MessageBox.Show("Var que Sale=" + Sale, "");
                            break;
                        }
                    } //////////////////////////////////////////////////////////////////Fin de la variable que sale ite>0                  
                }                
            }
            EP = dataGridView1.Rows[EPi].Cells[Entra].Value.ToString();
        } //Paso #1 & paso #2               
        void VarSaleEntraMIN()
        {
            Entra = ""; Sale = ""; EP = "";
            double[] Aux = new double[Form1.m + Form1.n + 1];
            if (ITE == 0)
            {
                for (int j = 1; j <= Form1.m; j++)   //Si la itereación es 0, hará esto           
                    Aux[j] = Convert.ToDouble(dataGridView1.Rows[0].Cells[j].Value);
            }
            else if (ITE > 0)
            {
                for (int j = 1; j <= Form1.m + Form1.n; j++)  //Cuando Ite>0 hará esto     
                    Aux[j] = Convert.ToDouble(dataGridView1.Rows[0].Cells[j].Value);
            }
            double Menor = Aux.Max();
            if (ITE == 0) //Calcular la Var que Entra (funciona)
            {
                for (int j = 1; j <= Form1.m; j++)
                {
                    if (dataGridView1.Rows[0].Cells[j].Value.ToString() == Menor.ToString())
                    {
                        Entra = "X" + j.ToString();
                        //MessageBox.Show("Var que entra=" + "X" + j.ToString(), "");
                        break;
                    }

                } //////////////////////////////////////////////////////////////////Fin de la Var que Entra
            }
            else if (ITE > 0)  //Calcular Variable que entra cuando ite>0
            {
                for (int j = 1; j <= (Form1.m + Form1.n); j++)//Calcular la Var que Entra (funciona)
                {
                    if (dataGridView1.Rows[0].Cells[j].Value.ToString() == Menor.ToString())
                    {
                        Entra = "X" + j.ToString();
                        //MessageBox.Show("Var que entra=" + "X" + j.ToString(), "");
                        break;
                    }

                } //////////////////////////////////////////////////////////////////Fin de la Var que Entra
            }
            if (ITE == 0)  //////////////////////////////////////////////////////////Variable que sale cuando ite==0
            {
                int negativos = 0;
                double[] Divi = new double[Form1.n + 1]; Divi[0] = 10000000000000000000000000.00;
                for (int i = 1; i <= Form1.n; i++)
                {
                    if ((Convert.ToDouble(dataGridView1.Rows[i].Cells["Z0"].Value.ToString()) / Convert.ToDouble(dataGridView1.Rows[i].Cells[Entra.ToString()].Value.ToString())) < 0 || Convert.ToDouble(dataGridView1.Rows[i].Cells[Entra.ToString()].Value.ToString()) == 0)
                    {
                        Divi[i] = 100000000000000000.00; negativos++;
                    }
                    else if (Convert.ToDouble(dataGridView1.Rows[i].Cells["Z0"].Value.ToString()) == 0 && (Convert.ToDouble(dataGridView1.Rows[i].Cells["Z0"].Value.ToString()) / Convert.ToDouble(dataGridView1.Rows[i].Cells[Entra.ToString()].Value.ToString())) > 0)
                    {
                        Divi[i] = 0.00000000000000000000001;
                    }
                    else
                    {
                        Divi[i] = Convert.ToDouble(dataGridView1.Rows[i].Cells["Z0"].Value.ToString()) / Convert.ToDouble(dataGridView1.Rows[i].Cells[Entra].Value.ToString());
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
                    for (int i = 1; i <= Form1.n; i++)//Calcular la Var que Sale
                    {
                        if (Convert.ToDouble(dataGridView1.Rows[i].Cells["Z0"].Value.ToString()) / Convert.ToDouble(dataGridView1.Rows[i].Cells[Entra].Value.ToString()) == Divi.Min())
                        {
                            Sale = dataGridView1.Rows[i].Cells["VB"].Value.ToString();
                            EPi = i;
                            //MessageBox.Show("Var que Sale=" + Sale, "");
                            break;
                        }

                    } //////////////////////////////////////////////////////////////////Fin de la Var que sale ite=0
                }
            }
            else if (ITE > 0)//Variable que sale cuando ite>0
            {
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
                            MessageBox.Show("Var que Sale=" + Sale, "");
                            break;
                        }
                    } //////////////////////////////////////////////////////////////////Fin de la variable que sale ite>0                  
                }
            }
            EP = dataGridView1.Rows[EPi].Cells[Entra].Value.ToString();
            //MessageBox.Show("EP=" + EP);
        } //Paso #1 & paso #2
        private void cambiarTablaInicialToolStripMenuItem_Click(object sender, EventArgs e)
        {
            do
            {
                siguienteIteraciónToolStripMenuItem.PerformClick();
            } while (optimo == false);
        }

        private void gráficoToolStripMenuItem_Click(object sender, EventArgs e)
        {
            Grafico ventgraf = new Grafico();
            ventgraf.Show(this);
        }
        void Sumatoria()
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
            dataGridView1.Rows[EPi].Cells["VB"].Value = Entra;
        } //Paso #3 & #4
        bool Optimo()
        {
            bool optimalidad = false;
            double[] Aux = new double[Form1.m+Form1.n+1];
            if (ITE == 0)
            {
                for (int j = 1; j <= Form1.m; j++)   //Si la itereación es 0, hará esto           
                    Aux[j] = Convert.ToDouble(dataGridView1.Rows[0].Cells[j].Value.ToString());
            }
            else if (ITE > 0)
            {
                for (int j = 1; j <= Form1.m + Form1.n; j++)  //Cuando Ite>0 hará esto     
                    Aux[j] = Convert.ToDouble(dataGridView1.Rows[0].Cells[j].Value.ToString());
            }
            if (Form1.MaxMin == "Max Z")
            {
                optimalidad = true;
                if (ITE == 0)
                {
                    for (int j = 1; j <= Form1.m; j++)
                    {
                        if (Aux[j] < 0)
                        {
                            optimalidad = false;
                            break;
                        }                        
                    }
                }
                else if (ITE > 0)
                    for (int j = 1; j <= Form1.m + Form1.n; j++)
                    {
                        if (Aux[j] < 0)
                        {
                            optimalidad = false;
                            break;
                        }                        
                    }
            }
            else if (Form1.MaxMin == "Min Z")
            {
                optimalidad = true;
                if (ITE == 0)
                    for (int j = 1; j <= Form1.m; j++)
                    {
                        if (Aux[j] > 0)
                        {
                            optimalidad = false;
                            break;
                        }                        
                    }
                else if(ITE>0)
                    for (int j = 1; j <= Form1.m + Form1.n; j++)
                    {
                        if (Aux[j] > 0)
                        {
                            optimalidad = false;
                            break;
                        }                        
                    }
            }
            return optimalidad;
        }
        void colorear()
        {
            for(int i=0;i<=dataGridView1.Columns.Count-1;i++)
                tablas[tablas.Count-1].DGV.Rows[EPi].Cells[i].Style.BackColor = Color.Orange;
            for (int i = 0; i <= dataGridView1.Rows.Count - 1; i++)
                tablas[tablas.Count - 1].DGV.Rows[i].Cells[Entra].Style.BackColor = Color.Orange;
            tablas[tablas.Count - 1].DGV.Rows[EPi].Cells[Entra].Style.BackColor = Color.Red;
        }
        void descolorear()
        {
            for (int i = 0; i <= dataGridView1.Rows.Count - 1; i++)
                for(int j=0; j<=dataGridView1.Columns.Count-1;j++)
                dataGridView1.Rows[i].Cells[j].Style.BackColor = Color.White;
        }
        private void vScrollBar1_Scroll(object sender, ScrollEventArgs e)
        {
            panel1.Location = new Point(7,-vScrollBar1.Value);
        }
        private void editarEcuacionesToolStripMenuItem_Click(object sender, EventArgs e)
        {
            this.Close();
        }
    }
    class DGVS
    {
        public DataGridView DGV = new DataGridView();
    }
}

