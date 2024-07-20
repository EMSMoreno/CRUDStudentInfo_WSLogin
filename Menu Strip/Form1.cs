using System;
using System.Data;
using System.Drawing;
using System.Windows.Forms;

namespace Menu_Strip
{
    public partial class Form1 : Form
    {
        public Form1()
        {
            InitializeComponent();
        }

        private void button1_Click(object sender, EventArgs e)
        {
            this.Close();
        }

        private void inserirToolStripMenuItem_Click(object sender, EventArgs e)
        {
            // Criar uma instância do FormCRUDAlunos
            CRUDStudentInfo.FormCRUDAlunos formCRUDAlunos = new CRUDStudentInfo.FormCRUDAlunos();
            formCRUDAlunos.Show();
        }

        private void listarToolStripMenuItem_Click(object sender, EventArgs e)
        {
            CRUDStudentInfo.FormCRUDAlunos formCRUDAlunos = new CRUDStudentInfo.FormCRUDAlunos();
            DataTable dataTableDoFormCRUDAlunos = formCRUDAlunos.GetDataTable();
            DataGridView dataGridViewDoFormCRUDAlunos = new DataGridView();
            dataGridViewDoFormCRUDAlunos.DataSource = dataTableDoFormCRUDAlunos;
            formCRUDAlunos.Show();
        }

    }
}