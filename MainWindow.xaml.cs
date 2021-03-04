﻿using Newtonsoft.Json;
using Newtonsoft.Json.Linq;
using System;
using System.Collections.Generic;
using System.Data;
using System.IO;
using System.Linq;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Input;
using System.Windows.Media.Imaging;

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
            //TB_mod_folder_path.Text = Environment.GetFolderPath(Environment.SpecialFolder.MyDocuments) + @"\My Games\Tabletop Simulator\Mods";
            TB_mod_folder_path.Text = @"F:\SteamLibrary\steamapps\common\Tabletop Simulator\Tabletop Simulator_Data\Mods";
        }

        private void Button_JSON_open_Click(object sender, RoutedEventArgs e)
        {
            //Open save json and parsing
            Microsoft.Win32.OpenFileDialog dlg = new Microsoft.Win32.OpenFileDialog
            {
                DefaultExt = ".json",
                Filter = "JSON (*.json)|*.json",
                InitialDirectory = TB_JSON_path.Text
            };

            Nullable<bool> result = dlg.ShowDialog();

            if (result == true)
            {
                string filename = dlg.FileName;
                TB_JSON_path.Text = filename;
            }

            using (StreamReader jsonSave = File.OpenText(TB_JSON_path.Text))
            using (JsonTextReader reader = new JsonTextReader(jsonSave))
            {
                JObject jsonO = (JObject)JToken.ReadFrom(reader);
                JArray Objects = (JArray)jsonO["ObjectStates"];

                SortedSet<string> urls = new SortedSet<string>();

                foreach (JObject ob in Objects)
                {
                    if (ob["Name"].ToString().Equals("Custom_Tile"))
                    {
                        JObject tmp = (JObject)ob["CustomImage"];
                        urls.Add(tmp["ImageURL"].ToString());
                        if (!tmp["ImageSecondaryURL"].ToString().Equals(""))
                        {
                            urls.Add(tmp["ImageSecondaryURL"].ToString());
                        }
                    }
                    else if (ob["Name"].ToString().Equals("Custom_Token"))
                    {
                        JObject tmp = (JObject)ob["CustomImage"];
                        urls.Add(tmp["ImageURL"].ToString());
                        if (!tmp["ImageSecondaryURL"].ToString().Equals(""))
                        {
                            urls.Add(tmp["ImageSecondaryURL"].ToString());
                        }
                    }
                    else if (ob["Name"].ToString().Equals("Deck") || ob["Name"].ToString().Equals("DeckCustom"))
                    {
                        foreach (var x in (JObject)ob["CustomDeck"])
                        {
                            string name = x.Key;
                            JObject tmp = (JObject)x.Value;
                            urls.Add(tmp["FaceURL"].ToString());
                            urls.Add(tmp["BackURL"].ToString());
                        }
                    }
                }
                //System.Console.WriteLine(string.Join("\n", urls.ToArray()));
                DataTable dt = new DataTable();

                dt.Columns.Add("#", typeof(int));
                dt.Columns.Add("Original", typeof(string));
                dt.Columns.Add("New", typeof(string));
                string[] ua = urls.ToArray();
                for (int idx = 0; idx < urls.Count(); idx++)
                {
                    dt.Rows.Add(new string[] { idx.ToString(), ua[idx], "" });
                }
                URLtable.ItemsSource = dt.DefaultView;
                URLtable.IsReadOnly = true;
                URLtable.SelectionMode = DataGridSelectionMode.Single;
                URLtable.Columns[0].Width = DataGridLength.SizeToCells;
                URLtable.Columns[1].MaxWidth = 380;
            }
        }

        private void Button_mods_open_Click(object sender, RoutedEventArgs e)
        {
            Microsoft.Win32.OpenFileDialog dlg = new Microsoft.Win32.OpenFileDialog
            {
                InitialDirectory = TB_mod_folder_path.Text,
                ValidateNames = false,
                CheckFileExists = false,
                CheckPathExists = true,
                FileName = "Select Folder"
            };

            Nullable<bool> result = dlg.ShowDialog();

            if (result == true)
            {
                string folderpath = System.IO.Path.GetDirectoryName(dlg.FileName);
                TB_mod_folder_path.Text = folderpath;
            }
        }

        private void URLtable_SelectionChanged(object sender, SelectionChangedEventArgs e)
        {
            string deleteSpecial(string s)
            {
                return s.Replace(":", "").Replace("/", "").Replace("-", "").Replace("=", "").Replace("?", "").Replace(".", "").Replace("%", "");
            }
            Image_Original.Source = new BitmapImage();
            Image_New.Source = new BitmapImage();

            DataRowView row = (DataRowView)URLtable.SelectedItems[0];
            try
            {
                string[] files = Directory.GetFiles(TB_mod_folder_path.Text + @"\Images\", deleteSpecial(row["original"].ToString()) + ".*");
                Image_Original.Source = new BitmapImage(new Uri(files[0], UriKind.Absolute));
            }
            catch (FileNotFoundException)
            {
                Image_Original.Source = new BitmapImage();
            }

            row = (DataRowView)URLtable.SelectedItems[0];
            if (!row["New"].ToString().Equals(""))
            {
                try
                {
                    System.Console.WriteLine(row["New"].ToString());
                    Image_New.Source = new BitmapImage(new Uri(row["New"].ToString(), UriKind.Absolute));
                }
                catch (FileNotFoundException)
                {
                    Image_New.Source = new BitmapImage();
                }
            }
        }

        private void URLtable_MouseDoubleClick(object sender, MouseButtonEventArgs e)
        {
            if (URLtable.CurrentCell.Column.Header.ToString().Equals("New"))
            {
                Microsoft.Win32.OpenFileDialog dlg = new Microsoft.Win32.OpenFileDialog
                {
                    DefaultExt = ".png",
                    Filter = "Image Files (*.png;*.jpg;*.jpeg)|*.png;*.jpg;*.jpeg",
                    InitialDirectory = TB_JSON_path.Text
                };

                Nullable<bool> result = dlg.ShowDialog();

                if (result == true)
                {
                    string filename = dlg.FileName;
                    ((DataRowView)URLtable.CurrentCell.Item)["New"] = filename;
                }
                URLtable_SelectionChanged(null, null);
            }
        }

        private void Window_SizeChanged(object sender, SizeChangedEventArgs e)
        {
            Image_Original.Height = (Stackpanel_image.ActualHeight - 26) / 2;
            Image_New.Height = (Stackpanel_image.ActualHeight - 26) / 2;
        }
    }
}
