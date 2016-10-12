using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Data;
using System.Windows.Documents;
using System.Windows.Input;
using System.Windows.Media;
using System.Windows.Media.Effects;
using System.Windows.Media.Imaging;
using System.Windows.Shapes;

namespace hashi
{
    /// <summary>
    /// Window_Hashi.xaml 的交互逻辑
    /// </summary>
    public partial class Window_Hashi : Window
    {
        class Point
        {
            public int x,y,num;
            public Point(int a, int b, int c)
            {
                x = a;
                y = b;
                num = c;
            }
        };
        string mode;
        int level;
        private int rows, columns, pointCnt;
        private List<Point> points;
        public Window_Hashi(string m, int l = 0, int r = 0, int c = 0)
        {
            InitializeComponent();
            mode = m;
            level = l;
            Tools.showSlowly(this, gd_surface);
            if (mode == "Self Defining")
            {
                rows = r;
                columns = c;
            }
            make_gd_hashi();
        }

        private void home_down(object sender, MouseButtonEventArgs e)
        {
            (new MainWindow()).Show();
            this.Close();
        }

        private void home_enter(object sender, MouseEventArgs e)
        {
            Image ibtn = (Image)sender;
            DropShadowEffect shining = new DropShadowEffect();
            shining.ShadowDepth = 0;
            shining.Color = Color.FromArgb(255, 0, 0, 0);
            shining.Opacity = 1;
            shining.BlurRadius = 20;
            ibtn.Effect = shining;
        }

        private void home_leave(object sender, MouseEventArgs e)
        {
            Image ibtn = (Image)sender;
            DropShadowEffect shining = new DropShadowEffect();
            shining.ShadowDepth = 0;
            shining.Color = Color.FromArgb(255, 0, 0, 0);
            shining.Opacity = 0;
            shining.BlurRadius = 20;
            ibtn.Effect = shining;
        }

        private void back_enter(object sender, MouseEventArgs e)
        {
            Image ibtn = (Image)sender;
            DropShadowEffect shining = new DropShadowEffect();
            shining.ShadowDepth = 0;
            shining.Color = Color.FromArgb(255, 0, 0, 0);
            shining.Opacity = 1;
            shining.BlurRadius = 20;
            ibtn.Effect = shining;
        }

        private void back_leave(object sender, MouseEventArgs e)
        {
            Image ibtn = (Image)sender;
            DropShadowEffect shining = new DropShadowEffect();
            shining.ShadowDepth = 0;
            shining.Color = Color.FromArgb(255, 0, 0, 0);
            shining.Opacity = 0;
            shining.BlurRadius = 20;
            ibtn.Effect = shining;
        }

        private void reset_enter(object sender, MouseEventArgs e)
        {
            Image ibtn = (Image)sender;
            DropShadowEffect shining = new DropShadowEffect();
            shining.ShadowDepth = 0;
            shining.Color = Color.FromArgb(255, 0, 0, 0);
            shining.Opacity = 1;
            shining.BlurRadius = 20;
            ibtn.Effect = shining;
        }

        private void reset_leave(object sender, MouseEventArgs e)
        {
            Image ibtn = (Image)sender;
            DropShadowEffect shining = new DropShadowEffect();
            shining.ShadowDepth = 0;
            shining.Color = Color.FromArgb(255, 0, 0, 0);
            shining.Opacity = 0;
            shining.BlurRadius = 20;
            ibtn.Effect = shining;
        }

        private void reset_click(object sender, MouseButtonEventArgs e)
        {
            Grid gd_hashi = FindName("gd_hashi") as Grid;
            UIElementCollection Childrens = gd_hashi.Children;
            Image temp_img;
            int upper = Childrens.Count;
            for (int i = upper - 1; i >= 0; --i)
            {
                temp_img = Childrens[i] as Image;
                gd_hashi.UnregisterName(temp_img.Name);
                gd_hashi.Children.Remove(temp_img);
            }
            if (mode == "Self Defining")
            {
                points = new List<Point>();
                return;
            }
            else if(mode =="Easy" || mode == "Medium" || mode == "Hard")
            {
                Image newnum;
                for (int i = 0; i < points.Count; i++)
                {
                    newnum = new Image();
                    newnum.Source = new BitmapImage(new Uri("Resources/num/" + points[i].num.ToString() + ".png", UriKind.Relative));
                    newnum.Name = "num" + i.ToString();
                    newnum.SetValue(Grid.RowProperty, points[i].x);
                    newnum.SetValue(Grid.ColumnProperty, points[i].y);
                    newnum.VerticalAlignment = VerticalAlignment.Center;
                    newnum.HorizontalAlignment = HorizontalAlignment.Center;
                    gd_hashi.Children.Add(newnum);
                    gd_hashi.RegisterName(newnum.Name, newnum);
                }
                return;
            }
        }

        private void make_gd_hashi()
        {
            Grid gd_hashi = FindName("gd_hashi") as Grid;
            gd_hashi.VerticalAlignment = VerticalAlignment.Center;
            gd_hashi.HorizontalAlignment = HorizontalAlignment.Center;
            points = new List<Point>();
            if (mode == "Easy" || mode == "Medium" || mode == "Hard")
            {
                TextReader tr = new StreamReader("Maps/" + mode + "/" + level.ToString() + ".txt");
                string tl = tr.ReadLine();
                rows = Convert.ToInt32(tl);
                tl = tr.ReadLine();
                columns = Convert.ToInt32(tl);

                for (int i = 0; i < rows; ++i)
                {
                    RowDefinition rd = new RowDefinition();
                    rd.Height = new GridLength((720 / columns) < (648 / rows) ? (720 / columns) : (648 / rows));
                    gd_hashi.RowDefinitions.Add(rd);
                }
                for (int i = 0; i < columns; ++i)
                {
                    ColumnDefinition cd = new ColumnDefinition();
                    cd.Width = new GridLength((720 / columns) < (648 / rows) ? (720 / columns) : (648 / rows));
                    gd_hashi.ColumnDefinitions.Add(cd);
                }
                tr.ReadLine();

                tl = tr.ReadLine();
                pointCnt = Convert.ToInt32(tl);
                tr.ReadLine();

                int x, y, num;
                Point newpoint;
                Image newnum;
                for (int i = 0; i < pointCnt; ++i)
                {
                    x = Convert.ToInt32(tr.ReadLine());
                    y = Convert.ToInt32(tr.ReadLine());
                    num = Convert.ToInt32(tr.ReadLine());
                    newpoint = new Point(x, y, num);
                    points.Add(newpoint);
                    tr.ReadLine();
                    newnum = new Image();
                    newnum.Name = "num" + i.ToString();
                    newnum.SetValue(Grid.RowProperty, x);
                    newnum.SetValue(Grid.ColumnProperty, y);
                    Uri uri = new Uri("Resources/num/" + num.ToString() + ".png", UriKind.Relative);
                    newnum.Source = new BitmapImage(uri);
                    newnum.VerticalAlignment = VerticalAlignment.Center;
                    newnum.HorizontalAlignment = HorizontalAlignment.Center;
                    gd_hashi.Children.Add(newnum);
                    gd_hashi.RegisterName(newnum.Name, newnum);
                }
            }
            else if (mode == "Self Defining")
            {
                for (int i = 0; i < rows; ++i)
                {
                    RowDefinition rd = new RowDefinition();
                    rd.Height = new GridLength((720 / columns) < (648 / rows) ? (720 / columns) : (648 / rows));
                    gd_hashi.RowDefinitions.Add(rd);
                }
                for (int i = 0; i < columns; ++i)
                {
                    ColumnDefinition cd = new ColumnDefinition();
                    cd.Width = new GridLength((720 / columns) < (648 / rows) ? (720 / columns) : (648 / rows));
                    gd_hashi.ColumnDefinitions.Add(cd);
                }
            }
        }
    }
}
