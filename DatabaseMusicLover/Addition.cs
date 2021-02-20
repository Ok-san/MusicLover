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
    public partial class Addition : Form
    {
        SqlConnection sqlConnection;
        public Addition()
        {
            InitializeComponent();
        }

        /*При загрузке формы*/
        private void Addition_Load(object sender, EventArgs e)
        { 
            /*Подключаем и открываем БД*/
            sqlConnection = new SqlConnection(@"Data Source = (LocalDB)\MSSQLLocalDB; AttachDbFilename = C:\Users\Oksana Syrova\source\repos\KR\DatabaseMusicLover\Database.mdf; Integrated Security = True");
            sqlConnection.Open();
        }
        
        /*При нажатии на кнопку добавления*/
        private void button1_Click(object sender, EventArgs e)
        {
            /*Записываем введенные данные в БД*/
            SqlCommand command = sqlConnection.CreateCommand();
            command.Parameters.AddWithValue("@Ex", System.Data.DbType.String).Value = textBoxEx.Text;
            command.Parameters.AddWithValue("@Song", System.Data.DbType.String).Value = textBoxSong.Text;
            command.Parameters.AddWithValue("@Album", System.Data.DbType.String).Value = textBoxAlbum.Text;
            command.Parameters.AddWithValue("@Link", System.Data.DbType.String).Value = textBoxLink.Text;
            command.CommandText = "INSERT into Song(Executor,Song,Album,Link) values(@Ex,@Song,@Album,@Link)";
            command.ExecuteNonQuery();

            /*Предупрждаем пользователя что данные добавились и нужно обновить*/
            MessageBox.Show(@"Добавление прошло успешно. Не забудьте обновить таблицу, нажав на кнопку 'Обновить'!", "Информация", MessageBoxButtons.OK, MessageBoxIcon.Information);
            Close();
        }
       
        /*Проверка на заполненность ключевых полей*/
        private void textBoxEx_TextChanged(object sender, EventArgs e)
        {
            if ((textBoxEx.Text.Length == 0) || (textBoxSong.Text.Length == 0) || (textBoxAlbum.Text.Length == 0))
            {
                button1.Enabled = false; //активация кнопки при запонении всех ключевых полей
            }
            else
                button1.Enabled = true;
        }
    }
}
