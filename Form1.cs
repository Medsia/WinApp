using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;
using System.Configuration;
using System.Data.SqlClient;

namespace lab12
{
    public partial class EditorDB : Form
    {
        SqlConnection sqlConnection = null;
        public EditorDB()
        {
            InitializeComponent();
        }

        //UpdateDataGrid
        void GridUpdate()
        {
            try
            {
                SqlDataAdapter dataAdapter = new SqlDataAdapter("SELECT * FROM Computers", sqlConnection);

                DataSet dataSet = new DataSet();

                dataAdapter.Fill(dataSet);

                dataGridView1.DataSource = dataSet.Tables[0];
            }
            catch (Exception ex)
            {
                MessageBox.Show(ex.Message, "Message");
            }
        }

        private void Form1_Load(object sender, EventArgs e)
        {
            // OpenConnection
            try
            {
                sqlConnection = new SqlConnection(ConfigurationManager.ConnectionStrings["ComputerPricesDB"].ConnectionString);

                sqlConnection.Open();

                SqlDataAdapter dataAdapter = new SqlDataAdapter("SELECT * FROM Computers", sqlConnection);

                DataSet dataSet = new DataSet();

                dataAdapter.Fill(dataSet);

                dataGridView1.DataSource = dataSet.Tables[0];
            }
            catch(Exception ex)
            {
                MessageBox.Show(ex.Message, "Message");
                return;
            }

            
        }

        // AddElement
        private void buttonAdd_Click(object sender, EventArgs e)
        {
            EditForm form = new EditForm();
            if (form.ShowDialog() == DialogResult.OK)
            {
                SqlCommand command = new SqlCommand("INSERT INTO [Computers] (Name, Price) VALUES (@Name, @Price)",
                sqlConnection);
                try
                {
                    command.Parameters.AddWithValue("Name", form.textBoxName.Text);
                    command.Parameters.AddWithValue("Price", form.textBoxPrice.Text.Replace(',', '.'));
                    command.ExecuteNonQuery();
                    MessageBox.Show("Line added", "Message");
                    GridUpdate();
                }
                catch (Exception ex)
                {
                    MessageBox.Show("Wrong value\n" + $"({ ex.Message})", "Warning");
                }

            }
        }

        //Edit
        private void buttonEdit_Click(object sender, EventArgs e)
        {
            EditForm form = new EditForm();
            try
            {
                string Name = dataGridView1.SelectedRows[0].Cells["Name"].Value.ToString();
                string Price = dataGridView1.SelectedRows[0].Cells["Price"].Value.ToString();

                form.textBoxName.Text = Name;
                form.textBoxPrice.Text = Price;
            }
            catch(Exception)
            {
                MessageBox.Show("Operation is not possible", "Message");
                return;
            }
            if (form.ShowDialog() == DialogResult.OK)
            {


                SqlCommand command = new SqlCommand("Update Computers SET Name = (@Name), Price =(@Price) WHERE id=@id", sqlConnection);
                try
                {
                    command.Parameters.AddWithValue("id", dataGridView1.SelectedRows[0].Cells["Id"].Value.ToString());
                    command.Parameters.AddWithValue("Name", form.textBoxName.Text);
                    command.Parameters.AddWithValue("Price", form.textBoxPrice.Text.Replace(',','.'));
                    command.ExecuteNonQuery();
                    MessageBox.Show("Information changed", "Message");
                    GridUpdate();
                }
                catch (Exception ex)
                {
                    MessageBox.Show("Wrong value\n" + $"({ ex.Message})", "Warning");
                }
            }
        }

        // Delete
        private void buttonDelete_Click(object sender, EventArgs e)
        {
            string Message = "Are you sure you want to delete this?";
            if(MessageBox.Show(Message, "Warning", MessageBoxButtons.YesNo, MessageBoxIcon.Warning, MessageBoxDefaultButton.Button2) == DialogResult.No)
            {
                return;
            }

            SqlCommand command = new SqlCommand("DELETE FROM Computers WHERE id=@id", sqlConnection);
            try
            {
                command.Parameters.AddWithValue("id", dataGridView1.SelectedRows[0].Cells["Id"].Value.ToString());
                command.ExecuteNonQuery();
                MessageBox.Show("Deleted succesfully", "Message");

                GridUpdate();
            }
            catch (Exception)
            {
                MessageBox.Show("Operation is not possible", "Message");
            }
        }
    }
}
