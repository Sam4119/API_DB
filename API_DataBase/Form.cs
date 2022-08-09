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
    public partial class frmAPI : System.Windows.Forms.Form
    {
        private SqlConnection _connection = null;
        private SqlCommandBuilder _command = null;
        private SqlDataAdapter _adapter = null;
        private DataSet _dataSet= null;
        private bool _addedRow = false;
        private bool _loaded = false;
        private string _connectingParametr = @"Data Source=CITNB32\SQLEXPRESS;Initial Catalog=AppCosts;Integrated Security=True";
        public frmAPI()
        {
            InitializeComponent();

        }
       
        private void LoadDate()
        {
            
            try
            {
                // разнести логику в отдельный класс тут только таблица
                //разнести методы загрузки и заполнения 
                //доделать выход и закрыть соединения
                _adapter = new SqlDataAdapter("SELECT *, 'Delete' AS [Command] from Expenses", _connection);
                _command = new SqlCommandBuilder(_adapter);
                _command.GetInsertCommand();
                _command.GetDeleteCommand();
                _command.GetUpdateCommand();
                _dataSet = new DataSet();
                DBViewTable.AllowUserToResizeColumns = true;
                DBViewTable.AutoGenerateColumns = true;
                //_adapter.Fill(_dataSet, "Expenses");
                //dgvDBViewTable.DataSource = _dataSet.Tables["Expenses"];
                //for (int i = 0; i < dgvDBViewTable.Rows.Count; i++)
                //{
                //    DataGridViewLinkCell linkCell = new DataGridViewLinkCell();
                //    dgvDBViewTable[5, i] = linkCell;
                //}
                FillingGridView(DBViewTable.Rows.Count);
                _loaded = true;
            }
            catch (Exception ex)
            {
                MessageBox.Show(ex.Message, "Ошибка!", MessageBoxButtons.OK, MessageBoxIcon.Error);
            }
        }
        private void FillingGridView(int IndexRow)
        {
            _adapter.Fill(_dataSet, "Expenses");
            DBViewTable.DataSource = _dataSet.Tables["Expenses"];
            for (int i = 0; i < IndexRow; i++)
            {
                DataGridViewLinkCell linkCell = new DataGridViewLinkCell();
                DBViewTable[5, i] = linkCell;
            }
        }
        private void ReloadDate()
        {            try
            {
                _dataSet.Tables["Expenses"].Clear();
                FillingGridView(DBViewTable.Rows.Count);
            }
            catch (Exception ex)
            {
                MessageBox.Show(ex.Message, "Ошибка!", MessageBoxButtons.OK, MessageBoxIcon.Error);
            }
        }


        private void Form_Load(object sender, EventArgs e)
        {

            _connection = new SqlConnection(_connectingParametr);
            _connection.Open();
            LoadDate();
        }

        private void dataGridView1_CellContentClick(object sender, DataGridViewCellEventArgs e)
        {
            try
            {
                if (DBViewTable.Columns[e.ColumnIndex].HeaderText == "Price")
                {
                    DBViewTable.Sort(DBViewTable.Columns[e.ColumnIndex], ListSortDirection.Ascending);// сортировка по колонке.
                }
            }
            catch
            {

            }
            try
            {
                if (DBViewTable.Columns[e.ColumnIndex].HeaderText == "Command")
                {
                    string task = DBViewTable.Rows[e.RowIndex].Cells[e.ColumnIndex].Value.ToString();
                    if (task == "Delete")
                    {
                        if (MessageBox.Show("Удалить?", "Удаление", MessageBoxButtons.YesNo,
                            MessageBoxIcon.Question) == DialogResult.Yes)
                        {
                            int rowIndex = e.RowIndex;
                            DBViewTable.Rows.RemoveAt(rowIndex);
                            _dataSet.Tables["Expenses"].Rows[rowIndex].Delete();//в методы
                            _adapter.Update(_dataSet, "Expenses");

                        }
                    }
                    else if (task == "Insert")
                    {
                        int rowIndex = DBViewTable.Rows.Count - 2;// как избавиться от -2
                        DataRow row = _dataSet.Tables["Expenses"].NewRow();


                        row["Category"] = DBViewTable.Rows[rowIndex].Cells[1].Value;
                        row["ProductName"] = DBViewTable.Rows[rowIndex].Cells[2].Value;
                        row["Price"] = DBViewTable.Rows[rowIndex].Cells[3].Value;
                        row["Who"] = DBViewTable.Rows[rowIndex].Cells[4].Value;

                        _dataSet.Tables["Expenses"].Rows.Add(row);
                        _dataSet.Tables["Expenses"].Rows.RemoveAt(_dataSet.Tables["Expenses"].Rows.Count - 1);

                        DBViewTable.Rows.RemoveAt(DBViewTable.Rows.Count - 2);
                        DBViewTable.Rows[e.RowIndex].Cells[5].Value = "Delete";
                        _adapter.Update(_dataSet, "Expenses");
                        _addedRow = false;
                    }
                    else if (task == "Update")
                    {
                        int Row = e.RowIndex;

                        _dataSet.Tables["Expenses"].Rows[Row]["Category"] = DBViewTable.Rows[Row].Cells[1].Value;
                        _dataSet.Tables["Expenses"].Rows[Row]["ProductName"] = DBViewTable.Rows[Row].Cells[2].Value;
                        _dataSet.Tables["Expenses"].Rows[Row]["Price"] = DBViewTable.Rows[Row].Cells[3].Value;
                        _dataSet.Tables["Expenses"].Rows[Row]["Who"] = DBViewTable.Rows[Row].Cells[4].Value;
                        _adapter.Update(_dataSet, "Expenses");

                        DBViewTable.Rows[e.RowIndex].Cells[5].Value = "Delete";
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

        private void dataGridView1_UserAddedRow(object sender, DataGridViewRowEventArgs e )
        {
            try
            {
                if (_addedRow == false)
                {
                    _addedRow = true;

                    int lastRow = DBViewTable.Rows.Count - 2;

                    DataGridViewRow newRow = DBViewTable.Rows[lastRow];
                    DataGridViewLinkCell linkCell = new DataGridViewLinkCell();
                    DBViewTable[5, lastRow] = linkCell;//избавиться от  5 нельзя так как метод работает через класс DatagridVieRowEventArgs, а там нет ColumnIndex
                    newRow.Cells["Command"].Value = "Insert";
                }
            }
            catch (Exception ex)
            {
                MessageBox.Show(ex.Message, "Ошибка!", MessageBoxButtons.OK, MessageBoxIcon.Error);
            }
        }

        private void dataGridView1_CellValueChanged(object sender, DataGridViewCellEventArgs e)
        {

            try
            {
                if (!_addedRow && _loaded)
                {
                    int RowIndex;
                    RowIndex = DBViewTable.SelectedCells[0].RowIndex;
                    DataGridViewRow edditingRow = DBViewTable.Rows[RowIndex];
                    DataGridViewLinkCell linkCell = new DataGridViewLinkCell();
                    DBViewTable[DBViewTable.Columns[e.ColumnIndex].HeaderText="Command", RowIndex] = linkCell;
                    edditingRow.Cells["Command"].Value = "Update";
                }
                
            }
            catch (Exception ex)
            {
                MessageBox.Show(ex.Message, "Ошибка!", MessageBoxButtons.OK, MessageBoxIcon.Error);
            }
            // доп функционал 
            // писать самые большие расходы, сортировка 
        }
       
        private void выходToolStripMenuItem_Click(object sender, EventArgs e)
        {
            _connection.Close();
            Application.Exit();
        }
    }
}
