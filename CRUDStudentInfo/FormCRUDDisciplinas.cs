using System;
using System.Configuration;
using System.Data;
using System.Data.SqlClient;
using System.Windows.Forms;
using static System.Windows.Forms.VisualStyles.VisualStyleElement;

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

        public event EventHandler TeacherAssigned;

        public FormCRUDDisciplinas(int studentId, string studentName)
        {
            InitializeComponent();
            _studentId = studentId;
            _studentName = studentName;
            txtStudentName.Text = _studentName;
            ShowDataOnGridView();
            LoadSubjectsForStudent();

            // Set AutoSizeColumnsMode to Fill
            dgViewSubject.AutoSizeColumnsMode = DataGridViewAutoSizeColumnsMode.Fill;
        }

        public FormCRUDDisciplinas()
        {
            InitializeComponent();
            ShowDataOnGridView();
            LoadSubjectsForStudent();
        }

        public void ShowDataOnGridView()
        {
            using (con = new SqlConnection(cs))
            {
                string query = "SELECT s.SubjectID, s.SubjectName, ISNULL(t.TeacherName, 'No Teacher Assigned') AS TeacherName " +
                               "FROM Subjects s " +
                               "LEFT JOIN Teachers t ON s.TeacherID = t.TeacherID " +
                               "WHERE s.StudentID = @studentId";
                adapter = new SqlDataAdapter(query, con);
                adapter.SelectCommand.Parameters.AddWithValue("@studentId", _studentId);
                dt = new DataTable();
                adapter.Fill(dt);
                dgViewSubject.DataSource = dt;

                // Set AutoSizeColumnsMode to Fill
                dgViewSubject.AutoSizeColumnsMode = DataGridViewAutoSizeColumnsMode.Fill;

                // Adjust FillWeight, if necessary
                dgViewSubject.Columns["SubjectID"].FillWeight = 10;
                dgViewSubject.Columns["SubjectName"].FillWeight = 15;
                dgViewSubject.Columns["TeacherName"].FillWeight = 15;

                foreach (DataGridViewColumn column in dgViewSubject.Columns)
                {
                    column.DefaultCellStyle.Alignment = DataGridViewContentAlignment.MiddleCenter;
                }
            }
        }

        public void ClearAllData()
        {
            txtSubjectName.Text = "";
            txtStudentName.Text = "";
        }

        private void btnAdd_Click(object sender, EventArgs e)
        {
            if (string.IsNullOrEmpty(txtSubjectName.Text))
            {
                MessageBox.Show("Please enter a subject name.");
                return;
            }
            using (con = new SqlConnection(cs))
            {
                con.Open();

                // Check if the subject already exists for the student
                cmd = new SqlCommand("SELECT COUNT(*) FROM Subjects WHERE SubjectName = @subjectName AND StudentID = @studentID", con);
                cmd.Parameters.AddWithValue("@subjectName", txtSubjectName.Text);
                cmd.Parameters.AddWithValue("@studentID", _studentId);
                int subjectCount = (int)cmd.ExecuteScalar();

                if (subjectCount > 0)
                {
                    MessageBox.Show("This subject already exists for the student.");
                    return;
                }

                // Insert the subject for the student
                cmd = new SqlCommand("INSERT INTO Subjects (SubjectName, StudentID) VALUES (@subjectName, @studentID)", con);
                cmd.Parameters.AddWithValue("@subjectName", txtSubjectName.Text);
                cmd.Parameters.AddWithValue("@studentID", _studentId);

                cmd.ExecuteNonQuery();
                con.Close();
                MessageBox.Show("Subject Added Successfully");

                // Trigger the TeacherAssigned event
                TeacherAssigned?.Invoke(this, EventArgs.Empty);
                ShowDataOnGridView();
                ClearAllData();
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
                        ShowDataOnGridView(); // Refresh the subjects list after updating
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

            // Trigger the TeacherAssigned event
            TeacherAssigned?.Invoke(this, EventArgs.Empty);
            ShowDataOnGridView();
            ClearAllData();
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
                        ShowDataOnGridView(); // Refresh the subjects list after deletion
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
                //lblSID.Text = row.Cells["SubjectID"].Value.ToString();
                txtSubjectName.Text = row.Cells["SubjectName"].Value.ToString();
            }
        }

        private void LoadSubjectsForStudent()
        {
            using (con = new SqlConnection(cs))
            {
                string query = @"
                    SELECT s.SubjectID, s.SubjectName, t.TeacherName 
                    FROM Subjects s 
                    LEFT JOIN Teachers t ON s.SubjectID = t.SubjectID 
                    WHERE s.StudentID = @studentId";

                adapter = new SqlDataAdapter(query, con);
                adapter.SelectCommand.Parameters.AddWithValue("@studentId", _studentId);
                dt = new DataTable();
                adapter.Fill(dt);
                dgViewSubject.DataSource = dt;
            }
        }

        private void FormTeachers_TeacherAssigned(object sender, EventArgs e)
        {
            // Refresh subjects DataGridView to show assigned teachers
            LoadSubjectsForStudent();
        }
    }
}
