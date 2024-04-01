using Avalonia;
using Avalonia.Controls;
using Avalonia.Markup.Xaml;
using System.Collections.Generic;
using System.Linq;
using Avalonia.Controls;
using Avalonia.Interactivity;
using Npgsql;
using System;

namespace MovieInfo
    
{
    public partial class EditorWindow : Window
    {
        private NpgsqlConnection _connection; //поле для подключения к базе данных
        private List<Movie> Movies; //список музыкальных композиций
        private Movie CurrenMusic; //текущая редактируемая композиция
        string connectionString = "Server=localhost;Port=5432;Database=MoviesDB;User Id=postgres;Password=123456;"; //строка подключения

        public EditorWindow(Movie currenMovie, List<Movie> movies)
        {
            InitializeComponent();
            CurrenMusic = currenMovie;
            this.DataContext = currenMovie; //установка в поля значений выбранного элемента
            Movies = movies;
        }

        private void Save_OnClick(object? sender, RoutedEventArgs e)
        {
            var user = Movies.FirstOrDefault(x => x.Код == CurrenMusic.Код); //gолучаем композицию из списка по названию
            // Если композиция не найдена, добавляем ее в базу данных
            if (user == null)
            {
                try
                {
                    _connection = new NpgsqlConnection(connectionString);
                    _connection.Open();

                    // Использование запроса с параметрами для предотвращения проблем с одинарными кавычками
                    string add = @"INSERT INTO Кино (Код, Название, Режиссер, Композитор, Жанр,Сценарист, Звук, Цвет, Формат, Количество_частей) VALUES (@ID, @filmname, @director, @composer, @viewMovie, @scriptAuthor, @soundtrack, @color, @format, @numberOfSeries)";
                    NpgsqlCommand cmd = new NpgsqlCommand(add, _connection);
       
                    cmd.Parameters.AddWithValue("@ID", Convert.ToInt32(id_field.Text));
                    cmd.Parameters.AddWithValue("@filmname", filmName_field.Text);
                    cmd.Parameters.AddWithValue("@director", director_field.Text);
                    cmd.Parameters.AddWithValue("@composer", composer_field.Text);
                    cmd.Parameters.AddWithValue("@viewMovie", viewMovie_field.Text);
                    cmd.Parameters.AddWithValue("@scriptAuthor", scriptAuthor_field.Text);
                    cmd.Parameters.AddWithValue("@soundtrack", soundtrack_field.Text);
                    cmd.Parameters.AddWithValue("@color", color_field.Text);
                    cmd.Parameters.AddWithValue("@format", format_field.Text);
                    cmd.Parameters.AddWithValue("@numberOfSeries", numberOfSeries_field.Text);

                    cmd.ExecuteNonQuery();
                    _connection.Close();
                    
                    InfoWindow success = new InfoWindow();
                    success.Title = "Данные добавлены";
                    success.Titl.Text = "Успех!";
                    success.ExMess.Text = "Данные были успешно добавлены в базу данных.";
                    success.Show();
                }
                catch (Exception exception)
                {
                    InfoWindow fail = new InfoWindow();
                    fail.Title = "Ошибка добавления";
                    fail.Titl.Text = "Ошибка";
                    fail.ExMess.Text = "Описание ошибки: " + exception.Message;
                    fail.Show();
                }
            }
            else //если запись найдена, выполняется редактирование информации по названию трека
            {
                try
                {
                    _connection = new NpgsqlConnection(connectionString);
                    _connection.Open();

                    // Использование запроса с параметрами для предотвращения проблем с одинарными кавычками
                    string upd = "UPDATE Кино SET Название = @filmname, Режиссер = @director, Композитор = @composer, Жанр = @viewMovie, Сценарист = @scriptAuthor, Звук = @soundtrack, Цвет = @color, Формат = @format, Количество_частей = @numberOfSeries WHERE Код = @ID;";
                    NpgsqlCommand cmd = new NpgsqlCommand(upd, _connection);
                    
                    cmd.Parameters.AddWithValue("@ID", Convert.ToInt32(id_field.Text));
                    cmd.Parameters.AddWithValue("@filmname", filmName_field.Text);
                    cmd.Parameters.AddWithValue("@director", director_field.Text);
                    cmd.Parameters.AddWithValue("@composer", composer_field.Text);
                    cmd.Parameters.AddWithValue("@viewMovie", viewMovie_field.Text);
                    cmd.Parameters.AddWithValue("@scriptAuthor", scriptAuthor_field.Text);
                    cmd.Parameters.AddWithValue("@soundtrack", soundtrack_field.Text);
                    cmd.Parameters.AddWithValue("@color", color_field.Text);
                    cmd.Parameters.AddWithValue("@format", format_field.Text);
                    cmd.Parameters.AddWithValue("@numberOfSeries", numberOfSeries_field.Text);

                    cmd.ExecuteNonQuery();
                    _connection.Close();
                    
                    InfoWindow success = new InfoWindow();
                    success.Title = "Данные изменены";
                    success.Titl.Text = "Успех!";
                    success.ExMess.Text = "Данные были успешно отредактированы.";
                    success.Show();
                }
                catch (Exception exception)
                {
                    InfoWindow fail = new InfoWindow();
                    fail.Title = "Ошибка изменения";
                    fail.Titl.Text = "Ошибка";
                    fail.ExMess.Text = "Описание ошибки: " + exception.Message;
                    fail.Show();
                }
            }
        }

        private void GoBack_OnClick(object? sender, RoutedEventArgs e)
        {
            MainWindow back = new MainWindow();
            this.Close();
            back.Show();
        }
    }
}