using System;
using System.Collections.Generic;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Input;
using System.Windows.Media;
using System.Windows.Media.Effects;
using System.Windows.Media.Imaging;

namespace hashi
{
    /// <summary>
    /// Window_Self.xaml 的交互逻辑
    /// </summary>
    public partial class Window_Self : Window
    {
        int rows, columns;
        public Window_Self(int r, int c)
        {
            InitializeComponent();
            rows = r;
            columns = c;
            gd_pre.Visibility = Visibility.Collapsed;

            now_c = now_r = 999;

            for (int i = 0; i < r; ++i)
            {
                RowDefinition rd = new RowDefinition();
                rd.Height = new GridLength((648 / c) < (648 / r) ? (648 / c) : (648 / r));
                gd_sd.RowDefinitions.Add(rd);
            }
            for (int i = 0; i < c; ++i)
            {
                ColumnDefinition cd = new ColumnDefinition();
                cd.Width = new GridLength((648 / c) < (648 / r) ? (648 / c) : (648 / r));
                gd_sd.ColumnDefinitions.Add(cd);
            }
            Image tempimg;
            for (int i = 0; i < r; ++i)
            {
                for (int j = 0; j < c; ++j)
                {
                    tempimg = new Image();
                    tempimg.Name = "num_" + i.ToString() + "_" + j.ToString();
                    tempimg.Source = new BitmapImage(new Uri("Resources/num/0.png", UriKind.Relative));
                    tempimg.Opacity = 0.3;
                    tempimg.MouseDown += tempimg_MouseDown;
                    tempimg.MouseEnter += image_enter;
                    tempimg.MouseLeave += image_leave;
                    tempimg.SetValue(Grid.RowProperty, i);
                    tempimg.SetValue(Grid.ColumnProperty, j);
                    gd_sd.RegisterName(tempimg.Name, tempimg);
                    gd_sd.Children.Add(tempimg);
                }
            }
        }

        void tempimg_MouseDown(object sender, MouseButtonEventArgs e)//响应数字按下
        {
            Image tempimg = (Image)sender;
            now_r = Convert.ToInt32(tempimg.Name.Split('_')[1]);
            now_c = Convert.ToInt32(tempimg.Name.Split('_')[2]);
            gd_pre.Visibility = Visibility.Visible;
        }

        List<string> txt;
        private void Image_MouseDown(object sender, MouseButtonEventArgs e)//响应提交按下
        {
            txt = new List<string>();
            txt.Clear();
            txt.Add(rows.ToString());
            txt.Add(columns.ToString());
            txt.Add("");
            int num = 0;
            int[,] map = new int[rows, columns];
            Image tempimg;
            for (int i = 0; i < rows; ++i)
            {
                for (int j = 0; j < columns; ++j)
                {
                    tempimg = FindName("num_" + i.ToString() + "_" + j.ToString()) as Image;
                    string source = tempimg.Source.ToString();
                    map[i, j] = Convert.ToInt32(source[source.Length - 5]) - '0';
                    if (map[i, j] != 0)
                        num++;
                }
            }
            for (int i = 0; i < rows; ++i)
            {
                for (int j = 0; j < columns; ++j)
                {
                    if (map[i, j] > 0)
                    {
                        if (i > 0)
                        {
                            if (map[i - 1, j] > 0)
                            {
                                MessageBox.Show("No solution.");
                                return;
                            }
                        }
                        if (j > 0)
                        {
                            if (map[i, j - 1] > 0)
                            {
                                MessageBox.Show("No solution.");
                                return;
                            }
                        }
                        if (i < rows - 1)
                        {
                            if (map[i + 1, j] > 0)
                            {
                                MessageBox.Show("No solution.");
                                return;
                            }
                        }
                        if (j < columns - 1)
                        {
                            if (map[i, j + 1] > 0)
                            {
                                MessageBox.Show("No solution.");
                                return;
                            }
                        }
                    }
                }
            }
            txt.Add(num.ToString());
            for (int i = 0; i < rows; ++i)
            {
                for (int j = 0; j < columns; ++j)
                {
                    if (map[i, j] != 0)
                    {
                        txt.Add("");
                        txt.Add(i.ToString());
                        txt.Add(j.ToString());
                        txt.Add(map[i, j].ToString());
                    }
                }
            }
            Window_Hashi window = new Window_Hashi("selfDefining", 0, rows, columns, txt);
            if (window.succeed)
            {
                window.Show();
                this.Close();
            }
            else
            {
                window.Close();
            }
        }

        int now_r, now_c;
        private void choice(object sender, MouseButtonEventArgs e)//响应选择的数字按下
        {
            Image tempimg = (Image)sender;
            string source = tempimg.Source.ToString();
            gd_pre.Visibility = Visibility.Collapsed;
            tempimg = FindName("num_" + now_r.ToString() + "_" + now_c.ToString()) as Image;
            tempimg.Source = new BitmapImage(new Uri("Resources/num/" + source[source.Length - 5] + ".png", UriKind.Relative));
            if (source[source.Length - 5] != '0')
            {
                tempimg.Opacity = 1;
            }
            else
            {
                tempimg.Opacity = 0.3;
            }
        }

        private void image_enter(object sender, MouseEventArgs e)
        {
            Image ibtn = (Image)sender;
            DropShadowEffect shining = new DropShadowEffect();
            shining.ShadowDepth = 0;
            shining.Color = Color.FromArgb(255, 0, 0, 0);
            shining.Opacity = 1;
            shining.BlurRadius = 20;
            ibtn.Effect = shining;
        }
        private void image_leave(object sender, MouseEventArgs e)
        {
            Image ibtn = (Image)sender;
            DropShadowEffect shining = new DropShadowEffect();
            shining.ShadowDepth = 0;
            shining.Color = Color.FromArgb(255, 0, 0, 0);
            shining.Opacity = 0;
            shining.BlurRadius = 20;
            ibtn.Effect = shining;
        }

        private void home_down(object sender, MouseButtonEventArgs e)
        {
            (new MainWindow()).Show();
            this.Close();
        }
    }
}
