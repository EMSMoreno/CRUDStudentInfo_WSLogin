using System;
using System.Configuration;
using System.Data;
using System.Data.SqlClient;
using System.Windows.Forms;

namespace CRUDStudentInfo
{
    public partial class FormCRUDAlunos : Form
    {
        public DataGridView StudentDataGridView
        {
            get { return dgViewStudent; }
        }

        public DataTable GetDataTable()
        {
            return dt;
        }

        string cs = ConfigurationManager.ConnectionStrings["dbCon"].ConnectionString;
        SqlConnection con;
        SqlCommand cmd;
        SqlDataAdapter adapter;
        DataTable dt;

        public void ShowDataOnGridView()
        {
            using (con = new SqlConnection(cs))
            {
                adapter = new SqlDataAdapter("Select * From Students", con);
                dt = new DataTable();
                adapter.Fill(dt);
                dgViewStudent.DataSource = dt;
            }
        }
        public void ClearAllData()
        {
            txtName.Text = "";
            txtEmail.Text = "";
            txtPhone.Text = "";
            cmBoxSex.SelectedItem = false;
        }

        public FormCRUDAlunos()
        {
            InitializeComponent();
            ShowDataOnGridView();
        }

        public void dgViewStudent_CellClick(object sender, DataGridViewCellEventArgs e)
        {
            txtName.Text = this.dgViewStudent.CurrentRow.Cells["Name"].Value.ToString();
            cmBoxSex.Text = this.dgViewStudent.CurrentRow.Cells["Sex"].Value.ToString();
            txtPhone.Text = this.dgViewStudent.CurrentRow.Cells["Phone"].Value.ToString();
            txtEmail.Text = this.dgViewStudent.CurrentRow.Cells["Email"].Value.ToString();

            lblSID.Text = this.dgViewStudent.CurrentRow.Cells["StudentID"].Value.ToString();
        }

        private void btnAdd_Click(object sender, EventArgs e)
        {
            using (con = new SqlConnection(cs))
            {
                con.Open();
                cmd = new SqlCommand("Insert Into Students (Name, Sex, Phone, Email) Values (@name, @sex, @phone, @email)", con);
                cmd.Parameters.AddWithValue("@name", txtName.Text);
                cmd.Parameters.AddWithValue("@sex", cmBoxSex.SelectedItem);
                cmd.Parameters.AddWithValue("@phone", txtPhone.Text);
                cmd.Parameters.AddWithValue("@email", txtEmail.Text);

                cmd.ExecuteNonQuery();
                con.Close();
                MessageBox.Show("Data Inserted Successfully");
                ShowDataOnGridView();
                ClearAllData();

            }
        }

        private void btnUpdate_Click(object sender, EventArgs e)
        {
            using (con = new SqlConnection(cs))
            {
                con.Open();
                cmd = new SqlCommand("Update Students Set Name=@name, Sex=@sex, Phone=@phone, Email=@email Where StudentID=@studentid", con);
                cmd.Parameters.AddWithValue("@name", txtName.Text);
                cmd.Parameters.AddWithValue("@sex", cmBoxSex.SelectedItem);
                cmd.Parameters.AddWithValue("@phone", txtPhone.Text);
                cmd.Parameters.AddWithValue("@email", txtEmail.Text);
                cmd.Parameters.AddWithValue("@studentid", lblSID.Text);

                cmd.ExecuteNonQuery();
                con.Close();
                MessageBox.Show("Data Updated Successfully");
                ShowDataOnGridView();
                ClearAllData();

            }
        }

        private void btnDelete_Click(object sender, EventArgs e)
        {
            using (con = new SqlConnection(cs))
            {
                con.Open();
                cmd = new SqlCommand("Delete From Students Where StudentID=@studentid", con);
                cmd.Parameters.AddWithValue("@studentid", lblSID.Text);
                cmd.ExecuteNonQuery();
                con.Close();
                MessageBox.Show("Data Deleted Successfully");
                ShowDataOnGridView();
                ClearAllData();
            }
        }

        private void btnCancel_Click(object sender, EventArgs e)
        {
            ClearAllData();
        }

        private void button1_Click(object sender, EventArgs e)
        {
            this.Close();
        }

        // CRUD that allows you to fetch data directly from SQL.
    }
}
