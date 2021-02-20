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

namespace DatabaseMusicLover
{
    public partial class Change : Form
    {
        string ex, song, id;
        SqlConnection sqlConnection;
        public Change(string ex, string song)
        {
            InitializeComponent();
            this.ex = ex;
            this.song = song;
        }
        private void Change_Load(object sender, EventArgs e)
        {
            /*Запрашиваем данные для выборанного, изменяемого трека*/
            sqlConnection = new SqlConnection(@"Data Source = (LocalDB)\MSSQLLocalDB; AttachDbFilename = C:\Users\Oksana Syrova\source\repos\KR\DatabaseMusicLover\Database.mdf; Integrated Security = True");
            sqlConnection.Open();
            SqlDataReader sqlReader = null;
            SqlCommand command = sqlConnection.CreateCommand();
            command.Parameters.AddWithValue("@S", System.Data.DbType.String).Value = this.song;
            command.Parameters.AddWithValue("@Ex", System.Data.DbType.String).Value = this.ex;
            command.CommandText = "SELECT  Executor, Song, Album, Link, Id FROM Song WHERE Executor = @Ex AND Song = @S";
            try
            {
                sqlReader = command.ExecuteReader();
                while (sqlReader.Read())
                {
                    /*Заполняем поля в зависимости от выбора изменяемого трека*/
                    textBoxEx.Text = sqlReader["Executor"].ToString();
                    textBoxSong.Text = sqlReader["Song"].ToString();
                    textBoxAlbum.Text = sqlReader["Album"].ToString();
                    textBoxLink.Text = sqlReader["Link"].ToString();
                    id = sqlReader["Id"].ToString();
                }
            }
            finally
            {
                if (sqlReader != null)
                    sqlReader.Close();
            }
        }
        private void button1_Click(object sender, EventArgs e)
        {
            if (textBoxEx.Text.Length != 0 && textBoxAlbum.Text.Length != 0 && textBoxSong.Text.Length != 0) //проверка заполненности полей
            {
                /*Внесение изменений в БД*/
                SqlCommand command = sqlConnection.CreateCommand();
                command.Parameters.AddWithValue("@Ex", System.Data.DbType.String).Value = textBoxEx.Text;
                command.Parameters.AddWithValue("@Song", System.Data.DbType.String).Value = textBoxSong.Text;
                command.Parameters.AddWithValue("@Album", System.Data.DbType.String).Value = textBoxAlbum.Text;
                command.Parameters.AddWithValue("@Link", System.Data.DbType.String).Value = textBoxLink.Text;
                command.Parameters.AddWithValue("@id", System.Data.DbType.String).Value = id;
                command.CommandText = "UPDATE Song SET Executor = @EX,Song=@Song,Album=@Album,Link=@Link WHERE Id=@id";
                command.ExecuteNonQuery();
                MessageBox.Show(@"Изменение прошло успешно. Не забудьте обновить таблицу, нажав на кнопку 'Обновить'!", "Информация", MessageBoxButtons.OK, MessageBoxIcon.Information);
                Close();
            }
            else
                MessageBox.Show(@"Не все ключевые поля (*) заполнены!", "Изменение", MessageBoxButtons.OK, MessageBoxIcon.Error);
        }
    }
}
