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
using System.Text.RegularExpressions;

namespace DatabaseMusicLover
{
    public partial class AlbumForm : Form
    {
        private string album;
        private SqlConnection sqlConnection ; //Класс для подключения к БД SQL server
        private SqlDataAdapter sqlDataAdapter; //Класс для заполнения DataSet   и обновления БД
        private DataSet dataSet ;//Класс для хранения данных из БД
        public AlbumForm(string album)
        {
            InitializeComponent();
            this.album = album;
        }
        private void AlbumForm_Load(object sender, EventArgs e)
        {
            /*При загрузке формы заполняем название альбома и создаем запрос для поиска всех треков в этом альбоме*/
            sqlConnection = new SqlConnection(@"Data Source = (LocalDB)\MSSQLLocalDB; AttachDbFilename = D:\My project\KR\DatabaseMusicLover\Database.mdf; Integrated Security = True");
            sqlConnection.Open();
            label1.Text = this.album;
            SqlCommand command = new SqlCommand("SELECT * FROM[Song] WHERE Album = @Al", sqlConnection);
            command.Parameters.AddWithValue("@Al", System.Data.DbType.String).Value = this.album;
            Load_Data(command);
        }

        /*Выводим треки*/
        private void Load_Data(SqlCommand command)
        {
            dataGridView1.DataSource = null;
            this.dataGridView1.Rows.Clear();
            sqlDataAdapter = new SqlDataAdapter();
            dataSet = new DataSet();
            sqlDataAdapter.SelectCommand = command;
            sqlDataAdapter.Fill(dataSet, "Song");
            dataGridView1.DataSource = dataSet.Tables["Song"];

            for (int i = 0; i < dataGridView1.Rows.Count; i++) //Делаем названия треков ссылками
            {
                DataGridViewLinkCell linkCell = new DataGridViewLinkCell();
                dataGridView1[1, i] = linkCell;
            }
        }

        /*При нажатии на название трека*/
        private void dataGridView1_CellContentClick(object sender, DataGridViewCellEventArgs e)
        {
            if (e.ColumnIndex == 1)
            {
                string executor = dataGridView1.Rows[e.RowIndex].Cells[0].Value.ToString();//получаем
                string song = dataGridView1.Rows[e.RowIndex].Cells[e.ColumnIndex].Value.ToString();
                SqlDataReader sqlReader = null;
                /* получаем ссылку выбранной песни*/
                SqlCommand command = new SqlCommand("SELECT Link FROM [Song] WHERE Song=@NA1 AND Executor=@NA2", sqlConnection);
                command.Parameters.AddWithValue("@NA1", System.Data.DbType.String).Value = song;
                command.Parameters.AddWithValue("@NA2", System.Data.DbType.String).Value = executor;
                try
                {
                    sqlReader = command.ExecuteReader();
                    string link;
                    if (sqlReader.Read())
                    {
                        link = sqlReader["Link"].ToString();
                        if (link != "") //проверяем что ссылка не пустая
                        {
                            string cheack = @"^(https:\/\/)[^\s@]*";
                            if (Regex.IsMatch(link, cheack, RegexOptions.IgnoreCase)) //проверяем ссылку на корректность
                            {
                                System.Diagnostics.Process.Start(link); //открываем браузер по ссылке
                            }
                            else
                                MessageBox.Show("Ссылка некорректна для открытия. Вы можете изменить ее нажав на кнопку 'Изменить'!", "Ошибка открытия", MessageBoxButtons.OK, MessageBoxIcon.Error);
                        }
                        else
                            MessageBox.Show("У данной песни нет ссылки. Вы можете добавить ее нажав на кнопку 'Изменить'!", "Ошибка открытия", MessageBoxButtons.OK, MessageBoxIcon.Error);
                    }
                }
                finally
                {
                    /*Очищаем и закрываем SqlDataReader*/
                    if (sqlReader != null)
                        sqlReader.Close();
                }
            }
        }
    }
}
