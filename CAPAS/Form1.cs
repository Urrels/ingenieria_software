using BLL;
using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;

namespace CAPAS
{
    public partial class Form1 : Form
    {
        BE.PERSONA persona;
        BLL.PERSONA gestor = new BLL.PERSONA();

        public Form1()
        {
            InitializeComponent();
        }

        public void Enlazar()
        {
            dataGridView1.DataSource = null;
            dataGridView1.DataSource = gestor.Listar();
        }

        private void Form1_Load(object sender, EventArgs e)
        {
            BE.USUARIO u = BE.SessionManager.getInstane().getUsuario();
            this.Text = "Form1 — Usuario: " + u.Usuario;

            Enlazar();
        }

        private void button1_Click(object sender, EventArgs e) 
        {
            if (!string.IsNullOrEmpty(textBox1.Text) && !string.IsNullOrEmpty(textBox2.Text))
            {
                persona = new BE.PERSONA();
                persona.Nombre = textBox1.Text;
                persona.Apellido = textBox2.Text;
                gestor.Grabar(persona);
                Enlazar();
            }
            else
            {
                MessageBox.Show("Complete los campos");
            }
        }

        private void button2_Click(object sender, EventArgs e) 
        {
            if (persona != null && !string.IsNullOrEmpty(textBox1.Text) && !string.IsNullOrEmpty(textBox2.Text))
            {
                persona.Nombre = textBox1.Text;
                persona.Apellido = textBox2.Text;
                gestor.Grabar(persona);
                Enlazar();
            }
            else
            {
                MessageBox.Show("Complete los campos");
            }
        }

        private void button3_Click(object sender, EventArgs e) 
        {
            if (persona != null)
            {
                gestor.Borrar(persona);
                Enlazar();
            }
            else
            {
                MessageBox.Show("Seleccione una persona");
            }
        }

        private void button4_Click(object sender, EventArgs e) 
        {
            BLL.USUARIO bll = new BLL.USUARIO();
            bll.Logout();

            LogIn login = new LogIn();
            login.Show();
            this.Close();
        }

        private void dataGridView1_CellClick(object sender, DataGridViewCellEventArgs e)
        {
            persona = dataGridView1.Rows[e.RowIndex].DataBoundItem as BE.PERSONA;
            textBox1.Text = persona.Nombre;
            textBox2.Text = persona.Apellido;
        }
    }
}