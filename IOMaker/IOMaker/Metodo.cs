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
    public partial class Metodo : Form
    {
        public static string metodo = "";
        bool continuar=true;
        public Metodo()
        {
            InitializeComponent();
        }

        private void Metodo_Load(object sender, EventArgs e)
        {
            
        }

        private void button1_Click(object sender, EventArgs e) // comprobar que esté bien y mandar el click al form 1
        {        
            if(comboBox1.Text=="")
            {
                MessageBox.Show("Por Favor Selecciona Algún método listado previamente!","Selecciona",MessageBoxButtons.OK,MessageBoxIcon.Information);
            }
            else
            {                
                if (comboBox1.Text == "Método Simplex(Original)" && Comprobarinicio(comboBox1.Text)==true)
                {
                    for (int j = 1; j <= Form1.m; j++)
                        Form1.tabla[0, j] = Convert.ToString(Convert.ToDouble(Form1.tabla[0, j]) * -1);
                    Form1.procedimiento = comboBox1.Text;
                    try {
                        Form1 padre = new Form1();
                        padre.Metodos();
                        this.Close();
                    } catch (Exception a) { MessageBox.Show(a.Message+"no form"); }
                    this.Close();
                }
                else if(comboBox1.Text== "Método de Doble Fase")
                {
                    for (int j = 1; j <= Form1.m; j++)
                        Form1.tabla[0, j] = Convert.ToString(Convert.ToDouble(Form1.tabla[0, j]) * -1);
                    Form1.procedimiento = comboBox1.Text;
                    try
                    {
                        Form1 padre = new Form1();
                        padre.Metodos();
                        this.Close();
                    }
                    catch (Exception a) { MessageBox.Show(a.Message + "no form"); }
                    this.Close();
                }
                else if (comboBox1.Text == "Gráfico")
                {
                    if(Form1.m>2)
                    {
                        MessageBox.Show("Este método sólo puede realizarse cuando existen 2 variables", "Num variables excedidas",MessageBoxButtons.OK,MessageBoxIcon.Hand);
                    }
                    else
                    {
                        Form1.procedimiento = comboBox1.Text;
                        try
                        {
                            Form1 padre = new Form1();
                            padre.Metodos();
                            this.Close();
                        }
                        catch (Exception a) { MessageBox.Show(a.Message + "no form"); }
                        this.Close();
                    }
                }
                else if (comboBox1.Text == "Método de la M Grande")
                {
                    Form1.procedimiento = comboBox1.Text;
                    try
                    {
                        Form1 padre = new Form1();
                        padre.Metodos();
                        this.Close();
                    }
                    catch (Exception a) { MessageBox.Show(a.Message + "no form"); }
                    this.Close();
                }
                else
                {
                    MessageBox.Show("Error al Elegir opción:");
                }
            }            
        }
        bool Comprobarinicio(string metodo)
        {
            switch (metodo)
            {
                case "Método Simplex(Original)":
            for (int i = 1; i <= Form1.n; i++)
            {
                if (Form1.tabla[i, Form1.m + 1] != "<=")
                {
                    continuar = false;
                    MessageBox.Show("Todas las condiciones deben ser de tipo <=", "revisa condiciones", MessageBoxButtons.OK, MessageBoxIcon.Hand);
                    break;
                }
            }
                    break;
        }
            return continuar;            
        }
    }
}
