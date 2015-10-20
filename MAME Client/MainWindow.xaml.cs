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
        private List<Label> labels;
        private Dictionary<Label, Dictionary<string, string>> infoByLabel;
        private Dictionary<string, string[]> csvByGame;
        private Dictionary<string, Label> labelByGame;
        private Label lastClicked = null;
        public MainWindow()
        {
            InitializeComponent();
        }

        private void CreateLabels()
        {
            ColumnDefinition cdef = new ColumnDefinition();
            cdef.Width = new GridLength(100, GridUnitType.Star);
            Grid.ColumnDefinitions.Add(cdef);
            infoByLabel = new Dictionary<Label, Dictionary<string, string>>();
            labelByGame = new Dictionary<string, Label>();
            romPaths = Directory.GetFiles(mamePath + romsPath, "*.zip");
            int len = romPaths.Length;
            labels = new List<Label>();
            Console.WriteLine(" * creating labels ");
            //Array.Sort(romPaths);
            Console.WriteLine("n paths: " + romPaths.Length.ToString());
            int row = 0;
            foreach (string path in romPaths)
            {
                //ZipArchive zip = ZipFile.OpenRead(path); 
                Label label = new Label();
                Dictionary<string, string> props = new Dictionary<string, string>();
                string name = path.Substring(path.LastIndexOf(@"\") + 1, path.Length - 4 - (path.LastIndexOf(@"\") + 1));
                infoByLabel.Add(label, props);
                labels.Add(label);
                props.Add("path", path);
                props.Add("name", name);
                label.Name = name;
                label.Content = name;
                Thickness margin = new Thickness();
                margin.Top = 4;
                margin.Bottom = 0;
                label.Margin = margin;
                Thickness padding = new Thickness();
                padding.Top = 0;
                padding.Bottom = 0;
                padding.Left = 5;
                label.Padding = padding;
                label.BorderBrush = Brushes.Transparent;
                Thickness border = new Thickness();
                border.Top = 0;
                border.Bottom = 0;
                label.BorderThickness = border;
                label.Background = Brushes.Transparent;
                label.Foreground = Brushes.Silver;
                label.VerticalContentAlignment = VerticalAlignment.Top;
                label.HorizontalContentAlignment = HorizontalAlignment.Left;
                label.FontFamily = new FontFamily("Tahoma");
                label.FontSize = 11.0;
                label.SetValue(Grid.ColumnProperty, 0);
                RowDefinition rdef = new RowDefinition();
                rdef.Height = new GridLength(rowHeight);
                Grid.RowDefinitions.Add(rdef);
                label.SetValue(Grid.RowProperty, row);
                label.MouseUp += OnLabelClick;
                Grid.Children.Add(label);
                Grid.Height = rowHeight * (row + 1);
                labelByGame.Add(name, label);
                //Console.WriteLine(" * label " + name);
                row++;
            }
            if (File.Exists(mamePath + "roms.csv")) {
                csvByGame = new Dictionary<string, string[]>();
                StreamReader reader = new StreamReader(File.OpenRead(mamePath + "roms.csv"));
                while (!reader.EndOfStream) {
                    string line = reader.ReadLine();
                    string[] values = line.Split(',');
                    string code = values[0].Trim();
                    if (labelByGame.ContainsKey(code)) {
                        csvByGame.Add(code, values);
                    }
                }
                foreach (KeyValuePair<string, string[]> entry in csvByGame) {
                    Label label = labelByGame[entry.Key];
                    label.Content = entry.Value[1].Trim() + " - " + entry.Value[2].Trim() + " " + entry.Value[3].Trim();
                }
            }
        }
        private void OnLabelClick(object sender, RoutedEventArgs e)
        {
            if (lastClicked != null) {
                lastClicked.Background = Brushes.Transparent;
                lastClicked.Foreground = Brushes.Silver;
            }
            Label label = sender as Label;
            lastClicked = label;
            lastClicked.Background = Brushes.Black;
            lastClicked.Foreground = Brushes.Aqua;
            string gameName = label.Name;
            run(gameName);
        }
        /*
        private void ResizeLabels()
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
            CreateLabels();
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
