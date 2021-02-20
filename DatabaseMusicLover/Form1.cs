using System;
using System.Data;
using System.Data.SqlClient;
using System.Collections.Generic;
using System.ComponentModel;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;
using System.Text.RegularExpressions;

namespace DatabaseMusicLover
{
    public partial class Form1 : Form
    {
        private SqlConnection sqlConnection = null; //Класс для подключения к БД SQL server
        private SqlDataAdapter sqlDataAdapter = null; //Класс для заполнения DataSet   и обновления БД
        private DataSet dataSet = null;//Класс для хранения данных из БД
        public Form1()
        {
            InitializeComponent();
        }
        /*При загрузке формы*/
        private void Form1_Load(object sender, EventArgs e)
        {
            /*Подключение БД */
            sqlConnection = new SqlConnection(@"Data Source = (LocalDB)\MSSQLLocalDB; AttachDbFilename = D:\My project\KR\DatabaseMusicLover\Database.mdf; Integrated Security = True");
            sqlConnection.Open(); //открытие БД
            try
            {
                SqlCommand command = new SqlCommand("SELECT * FROM[Song]", sqlConnection); //Запрос для выбора всех данных из БД
                SqlCommand command2 = new SqlCommand("SELECT Album FROM[Song] GROUP BY Album HAVING(Album Is Not Null)", sqlConnection); // Запрос группировки по альбомам
                Load_Data(command); //Вызов метода 
                Load_Album(command2);
            }
            catch (Exception ex)
            {
                MessageBox.Show(ex.Message, "Ошибка загрузки данных", MessageBoxButtons.OK, MessageBoxIcon.Error);
            }
        }

        /*Загрузка данных dataGridView1 по запросам*/
        private void Load_Data(SqlCommand command)
        {
            dataGridView1.DataSource = null;//обнуление источника данных
            this.dataGridView1.Rows.Clear();// очистка 
            sqlDataAdapter = new SqlDataAdapter();
            dataSet = new DataSet();
            sqlDataAdapter.SelectCommand = command;//присвоение запроса
            sqlDataAdapter.Fill(dataSet, "Song"); // заполнение dataSet "Song" по запросу1
            dataGridView1.DataSource = dataSet.Tables["Song"];//вывод результатов в dataGridView1

            for (int i = 0; i < dataGridView1.Rows.Count; i++) //цикл преобразования названия песен в ссылки
            {
                DataGridViewLinkCell linkCell = new DataGridViewLinkCell();
                dataGridView1[1, i] = linkCell;
            }
        }

        /*Загрузка данных dataGridView2 для показа всех альбомов*/
        private void Load_Album(SqlCommand command)
        {
            dataGridView2.DataSource = null;
            this.dataGridView2.Rows.Clear();
            sqlDataAdapter = new SqlDataAdapter();
            dataSet = new DataSet();
            sqlDataAdapter.SelectCommand = command;
            sqlDataAdapter.Fill(dataSet, "Album");
            dataGridView2.DataSource = dataSet.Tables["Album"];
        }

        /*Очищение поля для поиска при нажатии на него*/
        private void textSearch_MouseClick(object sender, MouseEventArgs e)
        {

            if (textSearch.Text == "Поиск музыки")
            {
                textSearch.Text = "";
            }
        }

        /*Поиск музыки при изменении значений в поле*/
        private void textSearch_TextChanged(object sender, EventArgs e)
        {
            if (textSearch.Text == "") //Если поле путое то выгружаются все данные из БД
            {
                SqlCommand command = new SqlCommand("SELECT * FROM[Song]", sqlConnection);
                Load_Data(command);
            }
            else
            {
                /*Запрос для поиска по БД*/
                SqlCommand command = new SqlCommand("SELECT Executor,Song,Album FROM [Song] WHERE Song  LIKE @NA+'%' OR Executor  LIKE @NA+'%' OR Album  LIKE @NA+'%'", sqlConnection);
                command.Parameters.AddWithValue("@NA", System.Data.DbType.String).Value = textSearch.Text; // Параметр берется из поля ввода
                Load_Data(command);
            }
        }

        /*Нажатие на кнопу "Справка"*/
        private void buttonHelp_Click(object sender, EventArgs e)
        {
            MessageBox.Show(@"Приложение 'Music Lover представляет собой базу данных треков, альбомов и исполнителей. 
Здесь вы можете создавать собственную базу данных, а также редактировать ее.
Чтобы добавить новый трек, альбом и исполнителя - нажмите 'Добавить'.
Для удаления - 'Удалить'
Если вы хотите изменить запись - 'Изменить'
Так же вы можете создавать альбомы. Для этого достаточно при добавлении треков указать собственное название альбома и постапенно добавлять в него песни по вашему желанию!
Просмотреть все созданные альбомы можно на вкладке'Альбомы'
Так же для удобства предоставлен поиск.
                   
        Работу выполнили: Сырова О.В. и Туламетова Е. М.", "Информация", MessageBoxButtons.OK, MessageBoxIcon.Information);
        }

        /*Нажатие на кнопу "добавить"*/
        private void buttonAdd_Click(object sender, EventArgs e)
        {
            Addition formAdd = new Addition(); //создаем объект класса 
            formAdd.Show(); //Открытие нового окна 
        }

        /*Нажатие на кнопу "Изменения"*/
        private void buttonEdit_Click(object sender, EventArgs e)
        {
            if (dataGridView1.SelectedRows.Count == 1) //проверка выбора одной строки
            {
                string executor = dataGridView1.SelectedCells[0].Value.ToString(); // получение исполнителя
                string song = dataGridView1.SelectedCells[1].Value.ToString(); // получение трека
                Change formChange = new Change(executor, song);
                formChange.Show(); //Открытие окна для изменения
            }
            else
                MessageBox.Show("Можно изменить только один элемент!", "Ошибка изменения", MessageBoxButtons.OK, MessageBoxIcon.Error);
        }

        /*Нажатие на кнопу "Удалить"*/
        private void buttonDelete_Click(object sender, EventArgs e)
        {
            /*Вывод предупреждения об удалении*/
            if (MessageBox.Show("Вы действительно хотите удалить записи?", "Подтверждение удаления", MessageBoxButtons.YesNo,
             MessageBoxIcon.Question, MessageBoxDefaultButton.Button2) == DialogResult.Yes) // согласие на удаление
            {
                if (dataGridView1.SelectedRows.Count == 1) // проверка выбора одной строки
                {
                    string ex = dataGridView1.SelectedCells[0].Value.ToString();
                    string song = dataGridView1.SelectedCells[1].Value.ToString();
                    string alb = dataGridView1.SelectedCells[2].Value.ToString(); ;
                    if (song != "" && ex != "" && alb != "")
                    {
                        /*Удаляем нужный трек */
                        SqlCommand command = new SqlCommand("DELETE FROM Song WHERE Executor=@EX AND Song=@SO AND Album=@AL", sqlConnection);
                        command.Parameters.AddWithValue("@EX", System.Data.DbType.String).Value = ex;
                        command.Parameters.AddWithValue("@SO", System.Data.DbType.String).Value = song;
                        command.Parameters.AddWithValue("@AL", System.Data.DbType.String).Value = alb;
                        command.ExecuteNonQuery(); //выполнение запроса
                        /*Сразу обновляем данные на форме*/
                        SqlCommand command2 = new SqlCommand("SELECT * FROM[Song]", sqlConnection);
                        Load_Data(command2);
                    }
                }
                else
                    MessageBox.Show("Можно удалить только один элемент!", "Ошибка удаления", MessageBoxButtons.OK, MessageBoxIcon.Error);
            }
        }

        /*Нажатие на кнопу "Обновить"*/
        private void buttonUpdate_Click(object sender, EventArgs e)
        {
            /*Обновление всех треков и альбомов*/
            SqlCommand command = new SqlCommand("SELECT * FROM[Song]", sqlConnection);
            Load_Data(command);
            SqlCommand command2 = new SqlCommand("SELECT Album FROM[Song] GROUP BY Album HAVING(Album Is Not Null)", sqlConnection);
            Load_Album(command2);
        }

        /*Событие нажатия на колонку*/
        private void dataGridView1_CellContentClick(object sender, DataGridViewCellEventArgs e)
        {
            if (e.ColumnIndex == 1) //если нажатие произошло на колонку с названиемм трека
            {
                string executor = dataGridView1.Rows[e.RowIndex].Cells[0].Value.ToString();//получаем названия исполнителя
                string song = dataGridView1.Rows[e.RowIndex].Cells[e.ColumnIndex].Value.ToString();//получаем названия трека
                SqlDataReader sqlReader = null; //Класс для чтения потока строк из БД
                SqlCommand command = new SqlCommand("SELECT Link FROM [Song] WHERE Song=@NA1 AND Executor=@NA2", sqlConnection);//запрос получения ссылки
                /*Задаем параметры*/
                command.Parameters.AddWithValue("@NA1", System.Data.DbType.String).Value = song;
                command.Parameters.AddWithValue("@NA2", System.Data.DbType.String).Value = executor;
                try
                {
                    sqlReader = command.ExecuteReader();//Выполнение запроса и построенние SqlDataReader
                    string link;
                    if (sqlReader.Read())
                    {
                        link = sqlReader["Link"].ToString();//присвоение результата запроса переменной link
                        if (link != "") //если ссылка не пустая

                        {
                            string cheack = @"^(https:\/\/)[^\s@]*";
                            if (Regex.IsMatch(link, cheack, RegexOptions.IgnoreCase)) // проверяем ссылку на корректность
                            {
                                System.Diagnostics.Process.Start(link); //открытие окна браузера
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
                    /*очищение и закрытие SqlDataReader*/
                    if (sqlReader != null)
                        sqlReader.Close();
                }
            }
        }

        /*Проверка выбора целой строки*/
        private void dataGridView1_MouseClick(object sender, MouseEventArgs e)
        {
            if (dataGridView1.SelectedRows.Count == 1 && dataGridView1.SelectedRows.Count != 0) //если выбрана 1 цела строка
            {
                buttonDelete.Enabled = true; //дотсупность кнопки удаления
                buttonEdit.Enabled = true; //дотсупность кнопки изменения
            }
            else
            {
                buttonDelete.Enabled = false;
                buttonEdit.Enabled = false;
            }
        }

        /*нажатие на название альбома*/
        private void dataGridView2_CellContentClick(object sender, DataGridViewCellEventArgs e)
        {
            if (e.ColumnIndex == 0) //проверка нажатия на альбом
            {
                string album = dataGridView2.Rows[e.RowIndex].Cells[0].Value.ToString(); //Получение названия альбома
                AlbumForm formAlbum = new AlbumForm(album); // инициализация окна
                formAlbum.Show(); // открытие окна
            }
        }
    }
}

