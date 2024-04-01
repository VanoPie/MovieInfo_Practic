using Avalonia;
using Avalonia.Controls;
using Avalonia.Controls.ApplicationLifetimes;
using Avalonia.Interactivity;
using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.IO;
using Npgsql;
using System.Linq;
using System.Threading.Tasks;
using Avalonia.Layout;
using Newtonsoft.Json.Linq;
using Avalonia.Media;


namespace MovieInfo;

public partial class MainWindow : Window
{
    private NpgsqlConnection _connection; //создание PostgreSQL подключения 
    private List<Movie> movies; //объявление списка mus, которое будет использовано для обращения к базе 
    string connectionString = "Server=localhost;Port=5432;Database=MoviesDB;User Id=postgres;Password=123456;"; //строка подключения к базе 
    private string fullTable = "SELECT * FROM Кино ORDER BY Код"; //запрос отображения записей
    public MainWindow()
    {
        InitializeComponent();
        ShowTable(fullTable); //метод отображения таблицы по запросу 
        _connection = new NpgsqlConnection(connectionString); //объявление нового PostgreSQL подключения 
        CmbViewMovieFill();
    }
    
    // Отображение таблицы 
    public void ShowTable(string sql)
    {
        movies = new List<Movie>();
        _connection = new NpgsqlConnection(connectionString);
        _connection.Open();
        NpgsqlCommand command = new NpgsqlCommand(sql, _connection); //создание команды для выполнения запроса
        NpgsqlDataReader reader = command.ExecuteReader(); //выполнение запроса и получение результата
        while (reader.Read() && reader.HasRows) //чтение результатов запроса
        {
            var Movs = new Movie()
            { 
                //получение результатов столбцов по классу 
                Код = reader.GetInt32(0),
                Название = reader.GetString(1),
                Режиссер = reader.GetString(2),
                Композитор = reader.GetString(3),
                Жанр = reader.GetString(4),
                Сценарист = reader.GetString(5),
                Звук = reader.GetString(6),
                Цвет = reader.GetString(7),
                Формат = reader.GetString(8),
                Количество_частей = reader.GetString(9)
            };
            movies.Add(Movs); //добавление объекта MusicClass в список mus.
        }
        _connection.Close();
        DataGrid.ItemsSource = movies; //установка списка mus в качестве источника данных для DataGrid
        DataGrid.LoadingRow += DataGrid_LoadingRow; //событие загрузки строк 
    }
    
    private void DeleteData_Click(object? sender, RoutedEventArgs e)
    {
        try
        {
            Movie mvs = DataGrid.SelectedItem as Movie; //получение выбранного объекта MusicClass
            if (mvs == null) //если строки не выделены
            {
                return; //выход из метода
            }
            // Создание нового соединения и открытие подключения 
            _connection = new NpgsqlConnection(connectionString);
            _connection.Open();

            // Использование запроса с парамтерами для предотвращения проблем с одинарными кавычками в названиях
            string sql = "DELETE FROM Кино WHERE Код = @id";
            NpgsqlCommand cmd = new NpgsqlCommand(sql, _connection);
            cmd.Parameters.AddWithValue("id", mvs.Код);

            cmd.ExecuteNonQuery(); //выполнение запроса
            _connection.Close();

            movies.Remove(mvs); //удаление объекта из списка 
            ShowTable(fullTable); //отображение обновленной таблицы
        }
        catch (Exception ex) //действия при возникновении ошибок
        {
            Console.WriteLine(ex.Message);
        }
    }
    private void AddData_Click(object? sender, RoutedEventArgs e)
    {
        Movie newMovie = new Movie(); //создание нового экземпляр класса MusicClass
        EditorWindow add = new EditorWindow(newMovie, movies); //открытие окна добавления/редактирования с новым экземпляром класса
        // Настрйока параметров открываемой формы и её отображение
        add.TitleBlock.Text = "Добавление данных";
        add.Title = "Форма ручного добавления данных";
        add.Show();
        this.Hide(); //скрываем текущее окно
    }

    // Изменение данных выбранной записи по нажатию кнопки
    private void EditData_Click(object? sender, RoutedEventArgs e)
    {
        Movie currenMovie = DataGrid.SelectedItem as Movie; //получаем выбранный элемент из DataGrid
        if (currenMovie == null) //если элемент не выбран, выходим из метода
            return;
        EditorWindow edit = new EditorWindow(currenMovie, movies); //открытие окна добавления/редактирования с выбранным экземпляром класса
        // Настрйока параметров открываемой формы и её отображение
        edit.id_field.IsReadOnly = true;
        edit.id_field.Foreground = Brushes.DarkRed;
        edit.TitleBlock.Text = "Редактирование данных";
        edit.Title = "Форма редактирования данных";
        edit.Show();
        this.Hide(); // Скрываем текущее окно
    }

    // Реализация поиска по вводу значения в текстовое поле
    private void SearchingFilm(object? sender, TextChangedEventArgs e)
    {
        var filmName = movies; //получаем список всех элементов из коллекции mus
        filmName = filmName.Where(x => x.Название.Contains(Search_Film.Text)).ToList(); //фильтруем список по введенному значению в поле поиска
        DataGrid.ItemsSource = filmName; //обновление источника данных DataGrid отфильтрованным списком
    }
    // Реализация фильтрации записей по жанру
    private void viewMovieFilter_OnClick(object? sender, SelectionChangedEventArgs e)
    {
        var ComboBox = (ComboBox)sender;
        var currentMusic = ComboBox.SelectedItem as Movie;
        var filteredArtist = movies
            .Where(x => x.Жанр == currentMusic.Жанр)
            .ToList();
        DataGrid.ItemsSource = filteredArtist;
    }

    // Заполнение выпадающего списка значениями
    public void CmbViewMovieFill()
    {
        movies = new List<Movie>();
        _connection = new NpgsqlConnection(connectionString);
        _connection.Open();
        NpgsqlCommand command = new NpgsqlCommand("SELECT * FROM Кино ORDER BY Жанр", _connection); //создание команды для выполнения запроса
        NpgsqlDataReader reader = command.ExecuteReader(); //выполнение запроса и получение результата
        while (reader.Read() && reader.HasRows) //чтение результатов запроса
        {
            var Mov = new Movie()
            { 
                //получение результатов столбцов по классу 
                Код = reader.GetInt32(0),
                Название = reader.GetString(1),
                Режиссер = reader.GetString(2),
                Композитор = reader.GetString(3),
                Жанр = reader.GetString(4),
                Сценарист = reader.GetString(5),
                Звук = reader.GetString(6),
                Цвет = reader.GetString(7),
                Формат = reader.GetString(8),
                Количество_частей = reader.GetString(9)
            };
                movies.Add(Mov); //добавление объекта MusicClass в список mus.
        }
        _connection.Close();
        var GenreCMB = this.Find<ComboBox>("CmbViewMovie");
        GenreCMB.ItemsSource = movies.DistinctBy(x => x.Жанр); //использование метода DistinctBy для удаления дубликатов"
    }
    
    // Сброс фильтрации и поиска
    private void Reset_OnClick(object? sender, RoutedEventArgs e)
    {
        ShowTable(fullTable); //отображение всех записей 
        Search_Film.Text = string.Empty; //сделать строку поиска пустой
    }
    
    //Окрашивание полей с пропущенными данными
    private void DataGrid_LoadingRow(object sender, DataGridRowEventArgs e)
    {
        Movie movs = e.Row.DataContext as Movie; //получаем экземпляр класса Movie из контекста строки
        if (movs != null) // Если экземпляр не пустой проверяем поля на наличие значения
        {
            if (movs.Название == String.Empty || movs.Режиссер == String.Empty || movs.Композитор == String.Empty || movs.Жанр == String.Empty || movs.Сценарист == String.Empty || movs.Звук == String.Empty || movs.Цвет == String.Empty || movs.Формат == String.Empty || movs.Количество_частей == String.Empty) //проверка поля на наличие пустого значения 
            {
                e.Row.Background = Brushes.Red; // Если хотя бы одно поле имеет пустое значение, устанавливаем для строки определенный фон
            }
            if (movs.Название == "-" || movs.Режиссер == "-"|| movs.Композитор == "-" || movs.Жанр == "-" || movs.Сценарист == "-" || movs.Звук == "-" || movs.Цвет == "-" || movs.Формат == "-" || movs.Количество_частей == "-") //проверка поля на наличие пустого значения 
            {
                e.Row.Background = Brushes.Red; // Если хотя бы одно поле имеет пустое значение, устанавливаем для строки определенный фон
            }
            else
            {
                e.Row.Background = Brushes.Transparent; // Если все поля заполнены, устанавливаем для строки прозрачный фон
            }
        }
    }
    
    // Метод на чтение и внос данных в базу через выбор JSON файла
    private async void ReadJsonFile(object sender, Avalonia.Interactivity.RoutedEventArgs e)
    {
        // Открытие и настройка диалогового окна с выбором файлов
        OpenFileDialog dialog = new OpenFileDialog();
        dialog.Title = "Выберите JSON файл для импорта данных о фильмах";
        dialog.Filters.Add(new FileDialogFilter() { Name = "JSON files", Extensions = { "json" } });
        string[] fileNames = await dialog.ShowAsync(this);

        try
        {
            if (fileNames.Length > 0)
            {
                // Чтение файла и выборка данных
                string json = File.ReadAllText(fileNames[0]);
                JArray data = JArray.Parse(json);
        
                List<int> errorIDs = new List<int>();

                foreach (var item in data)
                {
                    try
                    {
                        using (var command = _connection.CreateCommand())
                        {
                            _connection.Open();
                            // Внесение данных в базу с использованием параметров для предотвращения проблем с вводом 
                            command.CommandText = @"INSERT INTO Кино (Код, Название, Режиссер, Композитор, Жанр,Сценарист, Звук, Цвет, Формат, Количество_частей) VALUES (@ID, @filmname, @director, @composer, @viewMovie, @scriptAuthor, @soundtrack, @color, @format, @numberOfSeries)";

                            command.Parameters.AddWithValue("@ID", Convert.ToInt32(item["data"]["general"]["id"].ToString()));
                            command.Parameters.AddWithValue("@filmname", item["data"]["general"]["filmname"]?.ToString() ?? "-");
                            command.Parameters.AddWithValue("@director", item["data"]["general"]["director"]?.ToString() ?? "-");
                            command.Parameters.AddWithValue("@composer", item["data"]["general"]["composer"]?.ToString() ?? "-");
                            command.Parameters.AddWithValue("@viewMovie", item["data"]["general"]["viewMovie"]?.ToString() ?? "-");
                            command.Parameters.AddWithValue("@scriptAuthor", item["data"]["general"]["scriptAuthor"]?.ToString() ?? "-");
                            command.Parameters.AddWithValue("@soundtrack", item["data"]["general"]["soundtrack"]?.ToString() ?? "-");
                            command.Parameters.AddWithValue("@color", item["data"]["general"]["color"]?.ToString() ?? "-");
                            command.Parameters.AddWithValue("@format", item["data"]["general"]["format"]?.ToString() ?? "-");
                            command.Parameters.AddWithValue("@numberOfSeries", item["data"]["general"]["numberOfSeries"]?.ToString() ?? "-");


                            await command.ExecuteNonQueryAsync();

                            Console.WriteLine($"Запись для фильма с ID {item["data"]["general"]["id"]} добавлена в базу данных.");
                            ShowTable(fullTable);
                            CmbViewMovieFill();
                        }
                    }
                    catch (Exception ex)
                    {
                        Console.WriteLine($"Ошибка при добавлении записи с ID {item["data"]["general"]["id"]}: {ex.Message}");
                        errorIDs.Add(Convert.ToInt32(item["data"]["general"]["id"].ToString()));
                        Console.WriteLine();
                    }
                    finally
                    {
                        _connection.Close();
                    }
                }
                
                InfoWindow success = new InfoWindow();
                success.ExMess.Text = "Записи о фильмах были добавлены. \nСледующие ID вызвали ошибку при добавлении: \n";
                
                foreach (int id in errorIDs)
                {
                    success.ExMess.Text += id + "; ";
                }

                success.Title = "Добавление из JSON файла";
                success.Titl.Text = "Результаты добавления";
                success.Show();    
            }
        }
        catch (Exception ex)
        {
            Console.WriteLine(ex.Message);
        }
    }
            
    private void Exit_OnClick(object? sender, RoutedEventArgs e)
    {
        Environment.Exit(0);
    }
}