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
        const int nothing = 999;
        const int verticalline = 998;
        const int horizongtalline = 997;
        class Point
        {
            
            public int x,y,id,num;
            public int cnt,up_cnt, down_cnt, left_cnt, right_cnt;
            public int directions;
            public bool up_flag, down_flag, left_flag, right_flag;
            public int up_id, down_id, left_id, right_id;
            public Point(int a, int b, int c, int d)
            {
                x = a;
                y = b;
                cnt = num = c;
                up_cnt = down_cnt = left_cnt = right_cnt = 0;
                id = d;
                directions = 0;
                up_flag = down_flag = left_flag = right_flag = false;
                up_id = down_id = left_id = right_id = nothing;
            }
        };
        string mode;
        int level;
        private int rows, columns, pointCnt;
        private List<Point> points;
        int[,] map;
        bool choosing_nsd = false;
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

        private void pre_down(object sender, MouseButtonEventArgs e)
        {
            Image pre = sender as Image;
            string pre_name = pre.Name;
            string[] name_split = pre_name.Split('_');
            /*for (int i = 0; i < name_split.Length; ++i)
            {
                MessageBox.Show(name_split[i] + "\n");
            }*/
            int id_1 = Convert.ToInt32(name_split[1]), id_2 = Convert.ToInt32(name_split[2]);
            Image tempimg = new Image();
            tempimg.Name = "shader";
            tempimg.VerticalAlignment = VerticalAlignment.Center;
            tempimg.HorizontalAlignment = HorizontalAlignment.Center;
            tempimg.Source = new BitmapImage(new Uri("Resources/shader.png", UriKind.Relative));
            tempimg.SetValue(Grid.RowSpanProperty, rows);
            tempimg.SetValue(Grid.ColumnSpanProperty, columns);
            tempimg.Width = 720;
            tempimg.Height = 648;
            tempimg.Opacity = 0.75;
            gd_hashi.RegisterName(tempimg.Name, tempimg);
            gd_hashi.Children.Add(tempimg);
            if (points[id_1].cnt > 1 && points[id_2].cnt > 1)
            {
                for (int i = 0; i < 3; ++i)
                {
                    tempimg = new Image();
                    tempimg.Name = "nsdchoose_" + i.ToString() + "_" + name_split[0].Replace("pre","") + "_" + name_split[1] + "_" + name_split[2];
                    tempimg.Margin = new Thickness(i == 2 ? 80 : 0, 0, i == 0 ? 80 : 0, 0);
                    tempimg.VerticalAlignment = VerticalAlignment.Center;
                    tempimg.HorizontalAlignment = HorizontalAlignment.Center;
                    tempimg.Source = new BitmapImage(new Uri("Resources/num/" + i.ToString() + ".png", UriKind.Relative));
                    tempimg.Width = 40;
                    tempimg.Height = 40;
                    tempimg.SetValue(Grid.RowSpanProperty, rows);
                    tempimg.SetValue(Grid.ColumnSpanProperty, columns);
                    tempimg.MouseEnter += image_enter;
                    tempimg.MouseLeave += image_leave;
                    tempimg.MouseDown += nsdchoose_down;
                    gd_hashi.RegisterName(tempimg.Name, tempimg);
                    gd_hashi.Children.Add(tempimg);
                }
            }
            else
            {
                for (int i = 0; i < 2; ++i)
                {
                    tempimg = new Image();
                    tempimg.Name = "nsdchoose_" + i.ToString() + "_" + name_split[0].Replace("pre", "") + "_" + name_split[1] + "_" + name_split[2];
                    tempimg.SetValue(Grid.RowProperty, 0);
                    tempimg.SetValue(Grid.ColumnProperty, 0);
                    tempimg.Margin = new Thickness(i == 1 ? 40 : 0, 0, i == 0 ? 40 : 0, 0);
                    tempimg.VerticalAlignment = VerticalAlignment.Center;
                    tempimg.HorizontalAlignment = HorizontalAlignment.Center;
                    tempimg.Source = new BitmapImage(new Uri("Resources/num/" + i.ToString() + ".png", UriKind.Relative));
                    tempimg.Width = 40;
                    tempimg.Height = 40;
                    tempimg.SetValue(Grid.RowSpanProperty, rows);
                    tempimg.SetValue(Grid.ColumnSpanProperty, columns);
                    tempimg.MouseEnter += image_enter;
                    tempimg.MouseLeave += image_leave;
                    tempimg.MouseDown += nsdchoose_down;
                    gd_hashi.RegisterName(tempimg.Name, tempimg);
                    gd_hashi.Children.Add(tempimg);
                }
            }
            choosing_nsd = true;
        }

        private void nsdchoose_down(object sender, MouseButtonEventArgs e)
        {
            Image nsdchoose = sender as Image;
            string[] name_split = nsdchoose.Name.Split('_');
            int nsd = Convert.ToInt32(name_split[1]);
            string HorV = name_split[2];
            int id_1 = Convert.ToInt32(name_split[3]), id_2 = Convert.ToInt32(name_split[4]);
            switch(nsd)
            {
                case 0:

                    break;
                case 1:
                    break;
                case 2:
                    break;
                default:
                    return;
            }
        }

        private int pre_num_id = 999;
        private bool pre_same;
        private void num_down(object sender, MouseButtonEventArgs e)
        {
            if (choosing_nsd)
                return;
            Image num = (Image)sender;
            string name = num.Name;
            int id = Convert.ToInt32(name.Replace("num",""));
            //MessageBox.Show(name + "\n" + id);
            Image temp_img;
            for (int i = 0; i <= rows; ++i)
            {
                for (int j = 0; j <= columns; ++j)
                {
                    temp_img = FindName("horizontalpre_" + i.ToString() + "_" + j.ToString()) as Image;
                    if(temp_img != null)
                    {
                        gd_hashi.UnregisterName(temp_img.Name);
                        gd_hashi.Children.Remove(temp_img);
                    }
                    temp_img = FindName("verticalpre_" + i.ToString() + "_" + j.ToString()) as Image;
                    if(temp_img != null)
                    {
                        gd_hashi.UnregisterName(temp_img.Name);
                        gd_hashi.Children.Remove(temp_img);
                    }
                }
            }
            if (id == pre_num_id && !pre_same)
            {
                pre_same = true;
                return;
            }
            pre_same = false;
            pre_num_id = id;
            if (points[id].up_flag)
            {
                for (int i = points[id].x - 1; i > points[points[id].up_id].x; --i)
                {
                    createpre("vertical", i, points[id].y, points[id].up_id, id);
                }
            }
            if (points[id].down_flag)
            {
                for (int i = points[id].x + 1; i < points[points[id].down_id].x; ++i)
                {
                    createpre("vertical", i, points[id].y, id, points[id].down_id);
                }
            }
            if (points[id].left_flag)
            {
                for (int j = points[id].y - 1; j > points[points[id].left_id].y; --j)
                {
                    createpre("horizontal", points[id].x, j, points[id].left_id, id);
                }
            }
            if (points[id].right_flag)
            {
                for (int j = points[id].y + 1; j < points[points[id].right_id].y; ++j)
                {
                    createpre("horizontal", points[id].x, j, id, points[id].right_id);
                }
            }
        }
        private void createpre(string mode, int x, int y, int id_1, int id_2)
        {
            Image temp_img;
            temp_img = new Image();
            temp_img.Name = mode + "pre_" + id_1.ToString() + "_" + id_2.ToString() + "_" + x.ToString() + "_" + y.ToString();
            temp_img.SetValue(Grid.RowProperty, x);
            temp_img.SetValue(Grid.ColumnProperty, y);
            Uri uri = new Uri("Resources/" + mode + "pre.png", UriKind.Relative);
            temp_img.Source = new BitmapImage(uri);
            temp_img.VerticalAlignment = VerticalAlignment.Center;
            temp_img.HorizontalAlignment = HorizontalAlignment.Center;
            temp_img.Opacity = 0.75;
            temp_img.MouseDown += pre_down;
            gd_hashi.Children.Add(temp_img);
            gd_hashi.RegisterName(temp_img.Name, temp_img);
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
                for (int i = 0; i < pointCnt; ++i)
                {
                    x = Convert.ToInt32(tr.ReadLine());
                    y = Convert.ToInt32(tr.ReadLine());
                    num = Convert.ToInt32(tr.ReadLine());
                    newpoint = new Point(x, y, num, i);
                    points.Add(newpoint);
                    tr.ReadLine();
                }
                draw_points();
                make_map();
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
        private void make_map()
        {
            map = new int[20,20];
            for (int i = 0; i < 20; ++i)
            {
                for (int j = 0; j < 20; ++j)
                {
                    map[i, j] = nothing;
                }
            }
            for (int i = 0; i < points.Count; ++i)
            {
                map[points[i].x, points[i].y] = points[i].id;
            }
            for (int i = 0; i < rows; ++i)
            {
                for (int j = 0; j < columns; ++j)
                {
                    if (map[i, j] != nothing)
                    {
                        for (int k = i - 1; k >= 0 ; --k)
                        {
                            if (map[k, j] != nothing)
                            {
                                points[map[i, j]].up_id = map[k, j];
                                points[map[i, j]].up_flag = true;
                                points[map[i, j]].directions++;
                                points[map[k, j]].down_id = map[i, j];
                                points[map[k, j]].down_flag = true;
                                points[map[k, j]].directions++;
                                break;
                            }
                        }
                        for (int k = j - 1; k >= 0 ; --k)
                        {
                            if (map[i, k] != nothing)
                            {
                                points[map[i, j]].left_id = map[i, k];
                                points[map[i, j]].left_flag = true;
                                points[map[i, j]].directions++;
                                points[map[i, k]].right_id = map[i, j];
                                points[map[i, k]].right_flag = true;
                                points[map[i, k]].directions++;
                                break;
                            }
                        }
                    }
                }
            }
        }
        private void draw_points()
        {
            Image newnum;
            for (int i = 0; i < points.Count; ++i)
            {
                newnum = new Image();
                newnum.Name = "num" + i.ToString();
                newnum.SetValue(Grid.RowProperty, points[i].x);
                newnum.SetValue(Grid.ColumnProperty, points[i].y);
                Uri uri = new Uri("Resources/num/" + points[i].num.ToString() + ".png", UriKind.Relative);
                newnum.Source = new BitmapImage(uri);
                newnum.VerticalAlignment = VerticalAlignment.Center;
                newnum.HorizontalAlignment = HorizontalAlignment.Center;
                newnum.MouseDown += num_down;
                newnum.MouseEnter += image_enter;
                newnum.MouseLeave += image_leave;
                gd_hashi.Children.Add(newnum);
                gd_hashi.RegisterName(newnum.Name, newnum);
            }
        }







        private void home_down(object sender, MouseButtonEventArgs e)
        {
            (new MainWindow()).Show();
            this.Close();
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
        private void reset_click(object sender, MouseButtonEventArgs e)
        {
            choosing_nsd = false;
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
            else if (mode == "Easy" || mode == "Medium" || mode == "Hard")
            {
                draw_points();
                return;
            }
        }
    }
}
