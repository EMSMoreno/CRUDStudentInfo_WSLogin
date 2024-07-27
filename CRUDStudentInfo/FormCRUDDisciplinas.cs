using System;
using System.Configuration;
using System.Data;
using System.Data.SqlClient;
using System.Windows.Forms;

namespace CRUDStudentInfo
{
    public partial class FormCRUDDisciplinas : Form
    {
        public DataGridView SubjectDataGridView
        {
            get { return dgViewSubject; }
        }

        public DataTable GetDataTable()
        {
            return dt;
        }

        private int _studentId;
        private string _studentName;
        string cs = ConfigurationManager.ConnectionStrings["dbCon"].ConnectionString;
        SqlConnection con;
        SqlCommand cmd;
        SqlDataAdapter adapter;
        DataTable dt;
        private int studentId;

        public void ShowDataOnGridView()
        {
            using (con = new SqlConnection(cs))
            {
                adapter = new SqlDataAdapter("Select * From Subjects Where StudentID = @studentId", con);
                adapter.SelectCommand.Parameters.AddWithValue("@studentId", studentId);
                dt = new DataTable();
                adapter.Fill(dt);
                dgViewSubject.DataSource = dt;
            }
        }

        public void ClearAllData()
        {
            txtSubjectName.Text = "";
            txtStudentName.Text = "";
        }

        public FormCRUDDisciplinas()
        {
            InitializeComponent();
            ShowDataOnGridView();
            LoadSubjectsForStudent();
        }

        public FormCRUDDisciplinas(int studentId, string studentName)
        {
            InitializeComponent();

            _studentId = studentId;
            _studentName = studentName;

            // Check if the txtStudentName control is initialized and set the student name
            if (txtStudentName != null)
            {
                txtStudentName.Text = _studentName; // Display the student's name
            }

            LoadSubjectsForStudent();
        }

        private void btnAdd_Click(object sender, EventArgs e)
        {
            if (!string.IsNullOrWhiteSpace(txtSubjectName.Text))
            {
                try
                {
                    using (con = new SqlConnection(cs))
                    {
                        con.Open();
                        cmd = new SqlCommand("INSERT INTO Subjects (SubjectName, StudentID) VALUES (@subjectName, @studentId)", con);
                        cmd.Parameters.AddWithValue("@subjectName", txtSubjectName.Text);
                        cmd.Parameters.AddWithValue("@studentId", _studentId); // Use the student ID from the constructor

                        cmd.ExecuteNonQuery();
                        con.Close();

                        MessageBox.Show("Subject added successfully.");
                        LoadSubjectsForStudent(); // Refresh the subjects list after adding
                        ClearAllData(); // Clear the input field
                    }
                }
                catch (Exception ex)
                {
                    MessageBox.Show($"An error occurred: {ex.Message}");
                }
            }
            else
            {
                MessageBox.Show("Please enter a subject name.");
            }
        }

        private void btnUpdate_Click(object sender, EventArgs e)
        {
            if (dgViewSubject.CurrentRow != null && !string.IsNullOrWhiteSpace(txtSubjectName.Text))
            {
                try
                {
                    int subjectId = Convert.ToInt32(dgViewSubject.CurrentRow.Cells["SubjectID"].Value); // Get the selected SubjectID
                    using (con = new SqlConnection(cs))
                    {
                        con.Open();
                        cmd = new SqlCommand("UPDATE Subjects SET SubjectName = @subjectName WHERE SubjectID = @subjectId", con);
                        cmd.Parameters.AddWithValue("@subjectName", txtSubjectName.Text);
                        cmd.Parameters.AddWithValue("@subjectId", subjectId); // Use the selected SubjectID

                        cmd.ExecuteNonQuery();
                        con.Close();

                        MessageBox.Show("Subject updated successfully.");
                        LoadSubjectsForStudent(); // Refresh the subjects list after updating
                        ClearAllData();
                    }
                }
                catch (Exception ex)
                {
                    MessageBox.Show($"An error occurred: {ex.Message}");
                }
            }
            else
            {
                MessageBox.Show("Please select a subject and enter a new subject name.");
            }
        }

        private void btnDelete_Click(object sender, EventArgs e)
        {
            if (dgViewSubject.CurrentRow != null)
            {
                try
                {
                    int subjectId = Convert.ToInt32(dgViewSubject.CurrentRow.Cells["SubjectID"].Value); // Get the selected SubjectID
                    using (con = new SqlConnection(cs))
                    {
                        con.Open();
                        cmd = new SqlCommand("DELETE FROM Subjects WHERE SubjectID = @subjectId", con);
                        cmd.Parameters.AddWithValue("@subjectId", subjectId); // Use the selected SubjectID

                        cmd.ExecuteNonQuery();
                        con.Close();

                        MessageBox.Show("Subject deleted successfully.");
                        LoadSubjectsForStudent(); // Refresh the subjects list after deletion
                        ClearAllData();
                    }
                }
                catch (Exception ex)
                {
                    MessageBox.Show($"An error occurred: {ex.Message}");
                }
            }
            else
            {
                MessageBox.Show("Please select a subject to delete.");
            }
        }

        private void btnClose_Click(object sender, EventArgs e)
        {
            this.Close();
        }

        private void dgViewSubjects_CellClick(object sender, DataGridViewCellEventArgs e)
        {
            if (e.RowIndex >= 0)
            {
                DataGridViewRow row = this.dgViewSubject.Rows[e.RowIndex];
                lblSID.Text = row.Cells["SubjectID"].Value.ToString();
                txtSubjectName.Text = row.Cells["SubjectName"].Value.ToString();
            }
        }

        private void LoadSubjectsForStudent()
        {
            using (con = new SqlConnection(cs))
            {
                string query = "SELECT * FROM Subjects WHERE StudentID = @studentId";
                adapter = new SqlDataAdapter(query, con);
                adapter.SelectCommand.Parameters.AddWithValue("@studentId", _studentId);
                dt = new DataTable();
                adapter.Fill(dt);
                dgViewSubject.DataSource = dt;
            }
        }

    }
}
