using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;
using System.Data.SqlClient;

namespace CRUDStudentInfo
{
    public partial class FormLogin : Form
    {
        public FormLogin()
        {
            InitializeComponent();
            txtPassword.PasswordChar = '*';
        }

        SqlConnection conn = new SqlConnection(@"Data Source=DESKTOP-OAJ1FB6\SQLEXPRESS;Initial Catalog=CRUDOperationStudentInfoLogin;Integrated Security=True;TrustServerCertificate=True");

        private void btnLogin_Click(object sender, EventArgs e)
        {
            String username = txtUsername.Text;
            String user_password = txtPassword.Text;

            try
            {
                // Open the SQL connection
                conn.Open();

                // Use parameterized query to prevent SQL injection
                String query = "SELECT * FROM Login_new WHERE username = @username AND password = @password";
                SqlCommand cmd = new SqlCommand(query, conn);
                cmd.Parameters.AddWithValue("@username", username);
                cmd.Parameters.AddWithValue("@password", user_password);

                SqlDataAdapter sda = new SqlDataAdapter(cmd);
                DataTable dTable = new DataTable();
                sda.Fill(dTable);

                if (dTable.Rows.Count > 0)
                {
                    // Form que é carregado, após Login bem sucedido
                    FormCRUDAlunos formCRUDAlunos = new FormCRUDAlunos();
                    formCRUDAlunos.Show();
                    this.Hide();
                }
                else
                {
                    MessageBox.Show("Login Invalid! You have three chances, mate! Use them wisely!", "Error", MessageBoxButtons.OK, MessageBoxIcon.Error);
                    txtUsername.Clear();
                    txtPassword.Clear();
                    txtUsername.Focus(); //Focus on txt.Username
                }
            }
            catch (Exception ex)
            {
                MessageBox.Show("Error: " + ex.Message);
            }
            finally
            {
                // close the SQL connection
                conn.Close();
            }
        }

        private void btnClear_Click(object sender, EventArgs e)
        {
            txtUsername.Clear();
            txtPassword.Clear();

            txtUsername.Focus();
        }

        private void btnExit_Click(object sender, EventArgs e)
        {
            DialogResult res;
            res = MessageBox.Show("Do you really want to exit?", "Exit", MessageBoxButtons.YesNo, MessageBoxIcon.Question);
            if ( res == DialogResult.Yes)
            {
                Application.Exit();
            }
        }
    }
}
