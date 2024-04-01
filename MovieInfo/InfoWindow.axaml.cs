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

public partial class InfoWindow : Window
{
    public InfoWindow()
    {
        InitializeComponent();
    }
    
    private void Ok_Exit(object? sender, RoutedEventArgs e)
    {
        this.Close();
    }
}