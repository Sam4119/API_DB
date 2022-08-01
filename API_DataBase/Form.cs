using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;
using System.Data.Sql;
using System.Data.SqlClient;

namespace API_DataBase
{
    public partial class Form : System.Windows.Forms.Form
    {
        private SqlConnection _connection = null;
        private SqlCommandBuilder _command = null;
        private SqlDataAdapter _adapter = null;
        private DataSet _ds = null;
        private string _connectingParametr = @"Data Source=CITNB32\SQLEXPRESS;Initial Catalog=AppCosts;Integrated Security=True";
        public Form()
        {
            InitializeComponent();
        }
        private void LoadDate()
        {

            try
            {
                
                _adapter = new SqlDataAdapter("SELECT * , 'Command' AS [Delete] from Expenses", _connection);
                _command = new SqlCommandBuilder(_adapter);
                _command.GetInsertCommand();
                _command.GetDeleteCommand();
                _command.GetUpdateCommand();
                _ds = new DataSet();
                _adapter.Fill(_ds, "Expenses");
                dataGridView1.DataSource = _ds.Tables["Expenses"];
                for (int i = 0; i < dataGridView1.Rows.Count; i++)
                {
                    DataGridViewLinkCell linkCell = new DataGridViewLinkCell();
                    dataGridView1[5, i] = linkCell;

                }
                
            }
            catch (Exception ex)
            {
                MessageBox.Show(ex.Message, "Ошибка!", MessageBoxButtons.OK, MessageBoxIcon.Error);
            }
        }
        private void ReloadDate()
        {
            try
            {
                _ds.Tables["Expenses"].Clear();
                _adapter.Fill(_ds, "Expenses");
                dataGridView1.DataSource = _ds.Tables["Expenses"];
                for (int i = 0; i < dataGridView1.Rows.Count; i++)
                {
                    DataGridViewLinkCell linkCell = new DataGridViewLinkCell();
                    dataGridView1[5, i] = linkCell;
                }
            }
            catch (Exception ex)
            {
                MessageBox.Show(ex.Message, "Ошибка!", MessageBoxButtons.OK, MessageBoxIcon.Error);
            }
        }


        private void Form_Load(object sender, EventArgs e)
        {
            // TODO: данная строка кода позволяет загрузить данные в таблицу "appCostsDataSet.Expenses". При необходимости она может быть перемещена или удалена.
            this.expensesTableAdapter.Fill(this.appCostsDataSet.Expenses);
            // TODO: данная строка кода позволяет загрузить данные в таблицу "appCostsDataSet.Expenses". При необходимости она может быть перемещена или удалена.

            _connection = new SqlConnection(_connectingParametr);
            _connection.Open();
            LoadDate();
        }

        private void dataGridView1_CellContentClick(object sender, DataGridViewCellEventArgs e)
        {
            try
            {
                if (e.ColumnIndex == 5)
                {
                    string task = dataGridView1.Rows[e.RowIndex].Cells[5].Value.ToString();
                    if (task == "Delete")
                    {
                        if(MessageBox.Show("Удалить?","Удаление", MessageBoxButtons.YesNo,
                            MessageBoxIcon.Question) == DialogResult.Yes)

                        {
                            int rowIndex = e.RowIndex;
                            dataGridView1.Rows.RemoveAt(rowIndex);
                            _ds.Tables["Expenses"].Rows[rowIndex].Delete();
                            _adapter.Update(_ds, "Expensex");

                        }
                    }
                    else if(task == "Insert")
                    {


                    }
                    else if(task == "Update")
                    {

                    }

                    ReloadDate();
                }
               
            }
             catch (Exception ex)
            {
                MessageBox.Show(ex.Message, "Ошибка!", MessageBoxButtons.OK, MessageBoxIcon.Error);
            }
        }

        private void toolStripButton1_Click(object sender, EventArgs e)
        {
            ReloadDate();
        }

        private void dataGridView1_UserAddedRow(object sender, DataGridViewRowEventArgs e)
        {

        }
    }
}
