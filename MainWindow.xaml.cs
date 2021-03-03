﻿using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Data;
using System.Windows.Documents;
using System.Windows.Input;
using System.Windows.Media;
using System.Windows.Media.Imaging;
using System.Windows.Navigation;
using System.Windows.Shapes;

namespace TTS_Translator
{
    /// <summary>
    /// MainWindow.xaml에 대한 상호 작용 논리
    /// </summary>
    public partial class MainWindow : Window
    {
        public MainWindow()
        {
            InitializeComponent();
            TB_JSON_path.Text = Environment.GetFolderPath(Environment.SpecialFolder.MyDocuments) + @"\My Games\Tabletop Simulator\Saves";
            TB_mod_folder_path.Text = Environment.GetFolderPath(Environment.SpecialFolder.MyDocuments) + @"\My Games\Tabletop Simulator\Mods";
        }

        private void Button_JSON_open_Click(object sender, RoutedEventArgs e)
        {
            Microsoft.Win32.OpenFileDialog dlg = new Microsoft.Win32.OpenFileDialog();

            dlg.DefaultExt = ".json";
            dlg.Filter = "JSON (*.json)|*.json";
            dlg.InitialDirectory = TB_JSON_path.Text;

            Nullable<bool> result = dlg.ShowDialog();

            if (result == true)
            {
                string filename = dlg.FileName;
                TB_JSON_path.Text = filename;
            }
        }
    }
}
