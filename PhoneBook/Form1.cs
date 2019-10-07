using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Data.SqlClient;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;

namespace PhoneBook
{
    public partial class Form1 : Form
    {   
        SqlCommand command;
        SqlDataAdapter adapter;
        DataTable table;
        int _id;
        bool addNew;
        SqlConnection connection = new SqlConnection(@"Data Source=(LocalDB)\MSSQLLocalDB;AttachDbFilename=C:\Users\Engineer Smart\source\repos\PhoneBook\PhoneBook\Contacts1.mdf;Integrated Security=True");
        public Form1()
        {
            InitializeComponent();
        }

        private void Form1_Load(object sender, EventArgs e)
        {
            textName.BackColor = textEmail.BackColor = textNo.BackColor = Color.DarkGray;
            UpdateData();
        }

        private void btnNew_Click(object sender, EventArgs e)
        {
            textEmail.Enabled = textName.Enabled = textNo.Enabled = btnSave.Enabled = true;
            textName.BackColor = textEmail.BackColor = textNo.BackColor = Color.White;
            textName.Focus();
            _radioEmail.Enabled = _radioName.Enabled = textSearch.Enabled
                = btnEdit.Enabled = btnSearch.Enabled = btnCancel.Enabled = false;
            dataGridView1.Enabled =  false;
            dataGridView1.ForeColor = Color.DarkGray;
            addNew = true;
        }

        private void btnSearch_Click(object sender, EventArgs e)
        {
            if(textSearch.Text == "")
            {
                MessageBox.Show(this,"No valid keyword");
                return;
            }
            if(_radioEmail.Checked == false &&_radioName.Checked == false)
            {
                MessageBox.Show(this,"Search criteria not selected");
                return;
            }
            string value;
            if (_radioName.Checked)
            {
                value = "Name";
            }
            else
            {
                value = "Email";
            }

            
            adapter = new SqlDataAdapter("SELECT * FROM ContactTable WHERE " +
                $"{value} LIKE '{textSearch.Text}%'", connection);
            table = new DataTable();
            adapter.Fill(table);
            dataGridView1.DataSource = table;

        }

        private void btnSave_Click(object sender, EventArgs e)
        {
            if(textName.Text == "" || textNo.Text == "")
            {
                MessageBox.Show(this, "An important field is empty");
                return;
            }


            _radioEmail.Enabled = _radioName.Enabled = textSearch.Enabled
                = btnEdit.Enabled = btnSearch.Enabled = btnCancel.Enabled = true;
            textEmail.Enabled = textName.Enabled = textNo.Enabled = btnSave.Enabled = false;
            textName.BackColor = textEmail.BackColor = textNo.BackColor = Color.DarkGray;

            connection.Open();

            if (addNew)
            {
                command = new SqlCommand("SELECT Count(*) FROM ContactTable", connection);
                int _nCount = Convert.ToInt32(command.ExecuteScalar()) + 1;
                command = new SqlCommand("INSERT INTO ContactTable(SNo,Name,Email,MobileNo)VALUES" +
                    "('" + _nCount + "','" + textName.Text + "','" + textEmail.Text + "','" + textNo.Text + "')"
                    , connection);
            }
            else
            {
                command = new SqlCommand($"UPDATE ContactTable SET Name = '{textName.Text}', " +
                    $"Email =  '{textEmail.Text}', MobileNo = '{textNo.Text}' " +
                    $"WHERE SNo = {_id}", connection);
            }

            
            
            command.ExecuteNonQuery();
            connection.Close();
            UpdateData();
            textEmail.Text = textName.Text = textNo.Text = "";
            dataGridView1.Enabled = true;
            dataGridView1.ForeColor = Color.Black;

        }

        private void UpdateData()
        {
            adapter = new SqlDataAdapter("SELECT * FROM ContactTable ORDER BY SNo", connection);
            table = new DataTable();
            adapter.Fill(table);
            dataGridView1.DataSource = table;

            foreach (DataGridViewColumn column in dataGridView1.Columns)
            {
                    column.AutoSizeMode = DataGridViewAutoSizeColumnMode.Fill;
        
            }
            dataGridView1.BackgroundColor = Color.White;
        }

        private void btnEdit_Click(object sender, EventArgs e)
        {
            dataGridView1.Enabled = false;
            dataGridView1.ForeColor = Color.DarkGray;
            addNew = false;
            textEmail.Enabled = textName.Enabled = textNo.Enabled = btnSave.Enabled = true;
            textName.BackColor = textEmail.BackColor = textNo.BackColor = Color.White;
            textName.Focus();
            _radioEmail.Enabled = _radioName.Enabled = textSearch.Enabled
                = btnEdit.Enabled = btnSearch.Enabled = btnCancel.Enabled = false;

            int _selected = dataGridView1.CurrentCell.RowIndex + 1;
            adapter = new SqlDataAdapter("SELECT * FROM ContactTable WHERE SNo = '" + _selected + "'", connection);
            table = new DataTable();
            adapter.Fill(table);
            textName.Text = table.Rows[0][1].ToString();
            textEmail.Text = table.Rows[0][2].ToString();
            textNo.Text = table.Rows[0][3].ToString();
            
        }

        private void btnDelete_Click(object sender, EventArgs e)
        {
            DialogResult _decision = MessageBox.Show(this,"Are you sure you want to delete selected contact?",
                "Confirm delete", MessageBoxButtons.YesNo, MessageBoxIcon.Question);
            if(_decision == DialogResult.Yes)
            {
                connection.Open();
                command = new SqlCommand($"DELETE FROM ContactTable WHERE SNo = {_id}; UPDATE ContactTable SET " +
                    $"SNo=SNo-1 WHERE SNo > {_id};", connection);
                command.ExecuteNonQuery();
                connection.Close();
                UpdateData();
            }
        }

        private void dataGridView1_CellClick(object sender, DataGridViewCellEventArgs e)
        {
            btnEdit.Enabled = true;
            
        }

        private void dataGridView1_CellDoubleClick(object sender, DataGridViewCellEventArgs e)
        {
            btnEdit_Click(sender, e);
            dataGridView1.Enabled = false;
            dataGridView1.ForeColor = Color.DarkGray;
        }

        private void btnCancel_Click(object sender, EventArgs e)
        {
            UpdateData();
            textSearch.Text = "";
        }

        private void dataGridView1_CellEnter(object sender, DataGridViewCellEventArgs e)
        {
            
        }

        private void dataGridView1_Click(object sender, EventArgs e)
        {
           
        }

        private void btnCancell_Click(object sender, EventArgs e)
        {
            textSearch.Text = textNo.Text = textEmail.Text = textName.Text = "";
            dataGridView1.Enabled = true;
            dataGridView1.ForeColor = Color.Black;
            UpdateData();

            btnSave.Enabled = false;
            _radioEmail.Enabled = _radioName.Enabled = textSearch.Enabled 
                = btnEdit.Enabled = btnSearch.Enabled = btnCancel.Enabled = true;
        }

        private void dataGridView1_SelectionChanged(object sender, EventArgs e)
        {
            _id = dataGridView1.CurrentCell.RowIndex + 1;
        }
    }
}