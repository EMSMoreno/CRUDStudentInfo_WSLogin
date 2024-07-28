using System;
using System.Configuration;
using System.Data;
using System.Data.SqlClient;
using System.Windows.Forms;

namespace CRUDStudentInfo
{
    public partial class FormCRUDProfessores : Form
    {
        public event EventHandler TeacherAssigned; //Trigger que é disparado para passar o Teacher para a tabela Disciplinas, de forma a atualizar o dataGridView

        public DataGridView TeacherDataGridView
        {
            get { return dgViewTeacher; }
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

        public FormCRUDProfessores(int studentId, string studentName)
        {
            InitializeComponent();
            _studentId = studentId;
            _studentName = studentName;
            txtStudentName.Text = _studentName;
            ShowDataOnGridView();
            LoadSubjectsIntoComboBox();
        }

        public FormCRUDProfessores()
        {
            InitializeComponent();
            ShowDataOnGridView();
            LoadSubjectsIntoComboBox();
        }

        public void ShowDataOnGridView()
        {
            using (con = new SqlConnection(cs))
            {
                adapter = new SqlDataAdapter("SELECT t.TeacherID, t.TeacherName, t.Email, t.Phone, s.SubjectID, s.SubjectName FROM Teachers t JOIN Subjects s ON t.SubjectID = s.SubjectID", con);
                dt = new DataTable();
                adapter.Fill(dt);
                dgViewTeacher.AutoGenerateColumns = true;
                dgViewTeacher.DataSource = dt;

                // Set AutoSizeColumnsMode to Fill
                dgViewTeacher.AutoSizeColumnsMode = DataGridViewAutoSizeColumnsMode.Fill;

                // Adjust FillWeight, if necessary
                dgViewTeacher.Columns["TeacherID"].FillWeight = 10;
                dgViewTeacher.Columns["TeacherName"].FillWeight = 20;
                dgViewTeacher.Columns["Email"].FillWeight = 20;
                dgViewTeacher.Columns["Phone"].FillWeight = 10;
                dgViewTeacher.Columns["SubjectID"].FillWeight = 10;
                dgViewTeacher.Columns["SubjectName"].FillWeight = 25;

                foreach (DataGridViewColumn column in dgViewTeacher.Columns)
                {
                    column.DefaultCellStyle.Alignment = DataGridViewContentAlignment.MiddleCenter;
                }
            }
        }

        private void LoadSubjectsIntoComboBox()
        {
            using (con = new SqlConnection(cs))
            {
                string query = "SELECT SubjectID, SubjectName FROM Subjects";
                adapter = new SqlDataAdapter(query, con);
                dt = new DataTable();
                adapter.Fill(dt);

                cmbSubjectName.DisplayMember = "SubjectName";
                cmbSubjectName.ValueMember = "SubjectID";
                cmbSubjectName.DataSource = dt;

                // No initial selection on the comboBox
                cmbSubjectName.SelectedIndex = -1;
            }
        }

        public void ClearAllData()
        {
            txtTeacherName.Text = "";
            txtEmailTeacher.Text = "";
            txtPhoneTeacher.Text = "";
            cmbSubjectName.SelectedIndex = -1;
        }

        private void btnAdd_Click(object sender, EventArgs e)
        {
            if (cmbSubjectName.SelectedItem == null)
            {
                MessageBox.Show("Please select a subject.");
                return;
            }

            using (con = new SqlConnection(cs))
            {
                con.Open();
                cmd = new SqlCommand("INSERT INTO Teachers (TeacherName, Email, Phone, SubjectID) VALUES (@name, @Email, @Phone, @SubjectID)", con);
                cmd.Parameters.AddWithValue("@name", txtTeacherName.Text);
                cmd.Parameters.AddWithValue("@Email", txtEmailTeacher.Text);
                cmd.Parameters.AddWithValue("@Phone", txtPhoneTeacher.Text);
                cmd.Parameters.AddWithValue("@SubjectID", cmbSubjectName.SelectedValue);

                cmd.ExecuteNonQuery();
                con.Close();
                MessageBox.Show("Teacher Added Successfully");

                // Trigger the TeacherAssigned event
                TeacherAssigned?.Invoke(this, EventArgs.Empty);

                ShowDataOnGridView();
                ClearAllData();
            }
        }

        private void btnUpdate_Click(object sender, EventArgs e)
        {
            if (cmbSubjectName.SelectedIndex == -1)
            {
                MessageBox.Show("Please select a subject.");
                return;
            }

            using (con = new SqlConnection(cs))
            {
                con.Open();
                cmd = new SqlCommand("UPDATE Teachers SET TeacherName=@teacherName, Email=@Email, Phone=@Phone, SubjectID=@subjectID WHERE TeacherID=@teacherID", con);
                cmd.Parameters.AddWithValue("@teacherName", txtTeacherName.Text);
                cmd.Parameters.AddWithValue("@Email", txtEmailTeacher.Text);
                cmd.Parameters.AddWithValue("@Phone", txtPhoneTeacher.Text);
                cmd.Parameters.AddWithValue("@subjectID", cmbSubjectName.SelectedValue); // Use selected Subject ID
                cmd.Parameters.AddWithValue("@teacherID", Convert.ToInt32(dgViewTeacher.CurrentRow.Cells["TeacherID"].Value));
                cmd.ExecuteNonQuery();
                con.Close();
                MessageBox.Show("Teacher Updated Successfully");

                // Trigger the TeacherAssigned event
                TeacherAssigned?.Invoke(this, EventArgs.Empty);

                ShowDataOnGridView();
                ClearAllData();
            }
        }

        private void btnDelete_Click(object sender, EventArgs e)
        {
            using (con = new SqlConnection(cs))
            {
                con.Open();
                cmd = new SqlCommand("Delete From Teachers Where TeacherID=@teacherid", con);
                cmd.Parameters.AddWithValue("@teacherID", Convert.ToInt32(dgViewTeacher.CurrentRow.Cells["TeacherID"].Value));
                cmd.ExecuteNonQuery();
                con.Close();
                MessageBox.Show("Teacher Deleted Successfully");
                ShowDataOnGridView();
                ClearAllData();
            }
        }

        private void btnCancel_Click(object sender, EventArgs e)
        {
            ClearAllData();
        }

        private void btnClose_Click(object sender, EventArgs e)
        {
            this.Close();
        }

        private void dgViewTeacher_CellClick(object sender, DataGridViewCellEventArgs e)
        {
            if (e.RowIndex >= 0)
            {
                DataGridViewRow row = dgViewTeacher.Rows[e.RowIndex];
                txtTeacherName.Text = row.Cells["TeacherName"].Value.ToString();
                txtEmailTeacher.Text = row.Cells["Email"].Value.ToString();
                txtPhoneTeacher.Text = row.Cells["Phone"].Value.ToString();
                cmbSubjectName.SelectedValue = row.Cells["SubjectID"].Value;
            }
        }
    }
}
