using System;
using System.IO;
using System.IO.Compression;
using System.Windows;
using System.Windows.Controls;
using System.Collections;
using System.Collections.Generic;
using System.Windows.Media;
using System.Diagnostics;
using MAME_Client.Properties;

namespace MAME_Client
{
    /// <summary>
    /// Interaction logic for MainWindow.xaml
    /// </summary>
    public partial class MainWindow : Window
    {
        private int rowHeight = 20;
        private string mamePath = @"c:\mame\";
        private string romsPath = @"roms\";
        private string flags = @"-r 1920x1080";
        private string[] romPaths;
        private List<Button> buttons;
        private Dictionary<Button, Dictionary<string, string>> infoByButton;
        private Dictionary<string, string[]> csvByGame;
        private Dictionary<string, Button> buttonByGame;

        public MainWindow()
        {
            InitializeComponent();
        }

        private void CreateButtons()
        {
            ColumnDefinition cdef = new ColumnDefinition();
            cdef.Width = new GridLength(100, GridUnitType.Star);
            Grid.ColumnDefinitions.Add(cdef);
            infoByButton = new Dictionary<Button, Dictionary<string, string>>();
            buttonByGame = new Dictionary<string, Button>();
            romPaths = Directory.GetFiles(mamePath + romsPath, "*.zip");
            int len = romPaths.Length;
            buttons = new List<Button>();
            Console.WriteLine(" * creating buttons ");
            //Array.Sort(romPaths);
            Console.WriteLine("n paths: " + romPaths.Length.ToString());
            int row = 0;
            foreach (string path in romPaths)
            {
                //ZipArchive zip = ZipFile.OpenRead(path); 
                Button button = new Button();
                Dictionary<string, string> props = new Dictionary<string, string>();
                string name = path.Substring(path.LastIndexOf(@"\") + 1, path.Length - 4 - (path.LastIndexOf(@"\") + 1));
                infoByButton.Add(button, props);
                buttons.Add(button);
                props.Add("path", path);
                props.Add("name", name);
                button.Name = name;
                button.Content = name;
                Thickness margin = new Thickness();
                margin.Top = 0;
                margin.Bottom = 0;
                button.Margin = margin;
                button.BorderBrush = Brushes.Transparent;

                button.Background = Brushes.White;
                button.Foreground = Brushes.Black;
                button.BorderThickness = new Thickness(0.0);
                button.VerticalContentAlignment = VerticalAlignment.Top;
                button.HorizontalContentAlignment = HorizontalAlignment.Left;
                button.FontFamily = new FontFamily("Tahoma");
                button.FontSize = 11.0;
                button.SetValue(Grid.ColumnProperty, 0);
                RowDefinition rdef = new RowDefinition();
                rdef.Height = new GridLength(rowHeight);
                Grid.RowDefinitions.Add(rdef);
                button.SetValue(Grid.RowProperty, row);
                button.Click += OnButtonClick;
                Grid.Children.Add(button);
                Grid.Height = rowHeight * (row + 1);
                buttonByGame.Add(name, button);
                //Console.WriteLine(" * button " + name);
                row++;
            }
            if (File.Exists(mamePath + "roms.csv")) {
                csvByGame = new Dictionary<string, string[]>();
                StreamReader reader = new StreamReader(File.OpenRead(mamePath + "roms.csv"));
                while (!reader.EndOfStream) {
                    string line = reader.ReadLine();
                    string[] values = line.Split(',');
                    string code = values[0].Trim();
                    if (buttonByGame.ContainsKey(code)) {
                        csvByGame.Add(code, values);
                    }
                }
                foreach (KeyValuePair<string, string[]> entry in csvByGame) {
                    Button button = buttonByGame[entry.Key];
                    button.Content = entry.Value[1].Trim() + " - " + entry.Value[2].Trim() + " " + entry.Value[3].Trim();
                }
            }
        }
        private void OnButtonClick(object sender, RoutedEventArgs e)
        {
            Button button = sender as Button;
            string gameName = button.Name;
            run(gameName);
        }
        /*
        private void ResizeButtons()
        {
            Console.WriteLine(" * setting layout values");
            Console.WriteLine("width: " + this.Grid.ActualWidth.ToString());
            Console.WriteLine("height: " + this.Grid.ActualHeight.ToString());
        }
        */

        private void run(string gameName) {
            //string cmd = gameName + (flags.Length > 0 ? " " + flags : "");
            //Process.Start(mamePath + "mame", cmd);

            Process.Start("cmd", "/c \"cd " + mamePath + " && mame " + gameName + " " + flags + "\"");
        }

        private void Grid_Loaded(object sender, RoutedEventArgs e)
        {
            CreateButtons();
        }

        private void MAME_Client_Closing(object sender, System.ComponentModel.CancelEventArgs e)
        {
            //Console.WriteLine("top is " + this.Top);
            Settings.Default.WindowTop = this.Top;
            Settings.Default.WindowLeft = this.Left;
            Settings.Default.WindowWidth = this.Width;
            Settings.Default.WindowHeight = this.Height;
            Settings.Default.Save();
        }

        private void MAME_Client_Loaded(object sender, RoutedEventArgs e)
        {

            this.Top = Settings.Default.WindowTop;
            this.Left = Settings.Default.WindowLeft;
            this.Width = Settings.Default.WindowWidth;
            this.Height = Settings.Default.WindowHeight;
        }
    }
}
