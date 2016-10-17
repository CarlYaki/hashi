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
using System.Windows.Threading;

namespace hashi
{
    /// <summary>
    /// Window_Hashi.xaml 的交互逻辑
    /// </summary>
    class Point
    {

        public int x, y, id, num;
        public int available, up_cnt, down_cnt, left_cnt, right_cnt;
        public int directions;
        public bool up_flag, down_flag, left_flag, right_flag;
        public int up_id, down_id, left_id, right_id;
        public Point()
        { }
        public Point(int a, int b, int c, int d)
        {
            x = a;
            y = b;
            available = num = c;
            up_cnt = down_cnt = left_cnt = right_cnt = 0;
            id = d;
            directions = 0;
            up_flag = down_flag = left_flag = right_flag = false;
            up_id = down_id = left_id = right_id = 999;
        }
        public Point(Point p)
        {
            x = p.x; y = p.y; id = p.id; num = p.num;
            available = p.available; up_cnt = p.up_cnt; down_cnt = p.down_cnt; left_cnt = p.left_cnt; right_cnt = p.right_cnt;
            directions = p.directions;
            up_flag = p.up_flag; down_flag = p.down_flag; left_flag = p.left_flag; right_flag = p.right_flag;
            up_id = p.up_id; down_id = p.down_id; left_id = p.left_id; right_id = p.right_id;
        }
    };
    public partial class Window_Hashi : Window
    {
        const int nothing = 999;
        int[] verticalline = { 998, 997 };
        int[] horizontalline = { 996, 995 };
        string mode;
        int level;
        private int rows, columns, pointCnt;
        private List<Point> points,_points;
        int[,] map;
        List<int[,]> map_hist;
        List<List<Point>> points_hist;
        bool choosing_nsd = false;
        DispatcherTimer timer;
        List<string> s_list;
        public Window_Hashi(string m, int l = 0, int r = 0, int c = 0, List<string> sl = null)
        {
            s_list = sl;
            AItimer = new DispatcherTimer();
            AItimer.Tick += AItimer_Tick;
            InitializeComponent();
            passstep.Text = "0";
            passtime.Text = "0";

            timer = new DispatcherTimer();
            timer.Interval = new TimeSpan(0, 0, 1);
            timer.Tick += one_sec;

            points_hist = new List<List<Point>>();
            points_hist.Clear();
            mode = m;
            level = l;
            Tools.showSlowly(this, gd_surface);
            pre_name_list = new List<string>();
            pre_same = false;
            make_gd_hashi();
            timer.Start();
            path = new List<string>();
            path.Clear();
            if (mode != "selfDefining")
                modelevel.Text = mode + " Level." + l.ToString();
            else
                modelevel.Text = mode;
            if (!Tools.find_solution(map, rows, columns, points, path))
            {
                succeed = false;
                MessageBox.Show("No Solution");
            }
            else
            {
                succeed = true;
            }
            path.Add("end");
            //MessageBox.Show(path.Count.ToString());

            delta_t = new TimeSpan(0, 0, (int)AIdeltaT.Value, ((int)(AIdeltaT.Value * 1000)) % 1000);
            p_path = 0;
        }

        public bool succeed;
        List<string> path;
        int p_path;

        private void one_sec(object sender, EventArgs e)
        {
            passtime.Text = (Convert.ToInt32(passtime.Text) + 1).ToString();
        }

        private void pre_down(object sender, MouseButtonEventArgs e)
        {
            Image pre = sender as Image;
            string pre_name = pre.Name;
            string[] name_split = pre_name.Split('_');
            string mode = name_split[0].Replace("pre", "");
            int id_1 = Convert.ToInt32(name_split[1]), id_2 = Convert.ToInt32(name_split[2]), upper = Convert.ToInt32(name_split[5]);
            Image tempimg = new Image();
            tempimg.Name = "shader" + "_" + name_split[0].Replace("pre", "") + "_" + name_split[1] + "_" + name_split[2];
            tempimg.VerticalAlignment = VerticalAlignment.Center;
            tempimg.HorizontalAlignment = HorizontalAlignment.Center;
            tempimg.Source = new BitmapImage(new Uri("Resources/shader.png", UriKind.Relative));
            tempimg.SetValue(Grid.RowSpanProperty, rows);
            tempimg.SetValue(Grid.ColumnSpanProperty, columns);
            tempimg.Width = 720;
            tempimg.Height = 648;
            tempimg.Opacity = 0.75;
            tempimg.MouseDown += nsdchoose_down;
            gd_hashi.RegisterName(tempimg.Name, tempimg);
            gd_hashi.Children.Add(tempimg);
            if (upper >= 2)
            {
                for (int i = 0; i < 3; ++i)
                {
                    tempimg = new Image();
                    tempimg.Name = "nsdchoose_" + i.ToString() + "_" + mode + "_" + name_split[1] + "_" + name_split[2];
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
            else if (upper == 1)
            {
                for (int i = 0; i < 2; ++i)
                {
                    tempimg = new Image();
                    tempimg.Name = "nsdchoose_" + i.ToString() + "_" + mode + "_" + name_split[1] + "_" + name_split[2];
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
            choosing_nsd = false;
            Image nsdchoose = sender as Image;
            //MessageBox.Show(nsdchoose.Name);
            Image tempimg;
            string[] name_split = nsdchoose.Name.Split('_');
            if (name_split[0] == "shader")
            {
                for (int i = 0; i < 3; ++i)
                {
                    tempimg = FindName("nsdchoose_" + i.ToString() + nsdchoose.Name.Replace("shader", "")) as Image;
                    if (tempimg != null)
                    {
                        //MessageBox.Show(tempimg.Name);
                        gd_hashi.UnregisterName(tempimg.Name);
                        gd_hashi.Children.Remove(tempimg);
                    }
                }
                gd_hashi.UnregisterName(nsdchoose.Name);
                gd_hashi.Children.Remove(nsdchoose);
                choosing_nsd = false;
                return;
            }
            int nsd = Convert.ToInt32(name_split[1]);
            string HorV = name_split[2];
            int id_1 = Convert.ToInt32(name_split[3]), id_2 = Convert.ToInt32(name_split[4]);
            for (int i = 0; i < 3; ++i)
            {
                tempimg = FindName("nsdchoose_" + i.ToString() + "_" + HorV + "_" + id_1.ToString() + "_" + id_2.ToString()) as Image;
                if (tempimg != null)
                {
                    //MessageBox.Show(tempimg.Name);
                    gd_hashi.UnregisterName(tempimg.Name);
                    gd_hashi.Children.Remove(tempimg);
                }
            }
            tempimg = FindName("shader_" + HorV + "_" + id_1.ToString() + "_" + id_2.ToString()) as Image;
            gd_hashi.UnregisterName(tempimg.Name);
            gd_hashi.Children.Remove(tempimg);
            if (HorV == "horizontal")
            {
                for (int j = points[id_1].y + 1; j < points[id_2].y; ++j)
                {
                    tempimg = FindName(HorV + "_" + id_1.ToString() + "_" + id_2.ToString() + "_" + points[id_1].x.ToString() + "_" + j.ToString()) as Image;
                    if (tempimg != null)
                    {
                        gd_hashi.UnregisterName(tempimg.Name);
                        gd_hashi.Children.Remove(tempimg);
                    }
                }
            }
            else
            {
                for (int i = points[id_1].x + 1; i < points[id_2].y; ++i)
                {
                    tempimg = FindName(HorV + "_" + id_1.ToString() + "_" + id_2.ToString() + "_" + i.ToString() + "_" + points[id_1].y.ToString()) as Image;
                    if (tempimg != null)
                    {
                        gd_hashi.UnregisterName(tempimg.Name);
                        gd_hashi.Children.Remove(tempimg);
                    }
                }
            }
            if (nsd == 0)
            {
                if (HorV == "horizontal")
                {
                    points[id_1].right_cnt = nsd;
                    points[id_2].left_cnt = nsd;
                    int id_up, id_down;
                    for (int j = points[id_1].y + 1; j < points[id_2].y; ++j)
                    {
                        id_up = nothing;
                        id_down = nothing;
                        map[points[id_1].x, j] = nothing;
                        for (int i = points[id_1].x - 1; i >= 0; --i)
                        {
                            if (map[i, j] == nothing)
                                continue;
                            if (map[i, j] == horizontalline[0] || map[i,j] == horizontalline[1])
                                break;
                            if (map[i, j] < points.Count)
                            {
                                id_up = map[i, j];
                                break;
                            }
                        }
                        if(id_up == nothing)
                            continue;
                        for (int i = points[id_1].x + 1; i < rows; ++i)
                        {
                            if (map[i, j] == nothing)
                                continue;
                            if (map[i, j] == horizontalline[0] || map[i, j] == horizontalline[1])
                                break;
                            if (map[i, j] < points.Count)
                            {
                                id_down = map[i, j];
                                break;
                            }
                        }
                        if (id_down == nothing)
                            continue;
                        points[id_up].down_flag = true;
                        points[id_down].up_flag = true;
                    }
                }
                else
                {
                    points[id_1].down_cnt = nsd;
                    points[id_2].up_cnt = nsd;
                    int id_left, id_right;
                    for (int i = points[id_1].x + 1; i < points[id_2].x; ++i)
                    {
                        id_left = nothing;
                        id_right = nothing;
                        map[i, points[id_1].y] = nothing;
                        for (int j = points[id_1].y - 1; j >= 0; --j)
                        {
                            if (map[i, j] == nothing)
                                continue;
                            if (map[i, j] == verticalline[0] || map[i, j] == verticalline[1])
                                break;
                            if (map[i, j] < points.Count)
                            {
                                id_left = map[i, j];
                                break;
                            }
                        }
                        if (id_left == nothing)
                            continue;
                        for (int j = points[id_1].y + 1; j < columns; ++j)
                        {
                            if (map[i, j] == nothing)
                                continue;
                            if (map[i, j] == verticalline[0] || map[i, j] == verticalline[1])
                                break;
                            if (map[i, j] < points.Count)
                            {
                                id_right = map[i, j];
                                break;
                            }
                        }
                        if (id_right == nothing)
                            continue;
                        points[id_left].right_flag = true;
                        points[id_right].left_flag = true;
                    }
                }
            }
            else
            {
                if (HorV == "horizontal")
                {
                    int id_up, id_down;
                    for (int j = points[id_1].y + 1; j < points[id_2].y; ++j)
                    {
                        map[points[id_1].x, j] = horizontalline[nsd - 1];
                        tempimg = FindName(HorV + "_" + id_1.ToString() + "_" + id_2.ToString() + "_" + points[id_1].x.ToString() + "_" + j.ToString()) as Image;
                        if (tempimg == null)
                        {
                            tempimg = new Image();
                            tempimg.Name = HorV + "_" + id_1.ToString() + "_" + id_2.ToString() + "_" + points[id_1].x.ToString() + "_" + j.ToString();
                            tempimg.VerticalAlignment = VerticalAlignment.Center;
                            tempimg.HorizontalAlignment = HorizontalAlignment.Center;
                            tempimg.SetValue(Grid.RowProperty, points[id_1].x);
                            tempimg.SetValue(Grid.ColumnProperty, j);
                            gd_hashi.RegisterName(tempimg.Name, tempimg);
                            gd_hashi.Children.Add(tempimg);
                            map[points[id_1].x, j] = horizontalline[nsd - 1];
                        }
                        tempimg.Source = new BitmapImage(new Uri("Resources/" + HorV + nsd.ToString() + ".png", UriKind.Relative));


                        id_up = nothing;
                        id_down = nothing;
                        for (int i = points[id_1].x - 1; i >= 0; --i)
                        {
                            if (map[i, j] == nothing)
                                continue;
                            if (map[i, j] == horizontalline[0] || map[i, j] == horizontalline[1])
                                break;
                            if (map[i, j] < points.Count)
                            {
                                id_up = map[i, j];
                                break;
                            }
                        }
                        if (id_up == nothing)
                            continue;
                        for (int i = points[id_1].x + 1; i < rows; ++i)
                        {
                            if (map[i, j] == nothing)
                                continue;
                            if (map[i, j] == horizontalline[0] || map[i, j] == horizontalline[1])
                                break;
                            if (map[i, j] < points.Count)
                            {
                                id_down = map[i, j];
                                break;
                            }
                        }
                        if (id_down == nothing)
                            continue;
                        points[id_up].down_flag = false;
                        points[id_down].up_flag = false;
                    }
                    points[id_1].right_cnt = nsd;
                    points[id_2].left_cnt = nsd;
                }
                else
                {
                    int id_left, id_right;
                    for (int i = points[id_1].x + 1; i < points[id_2].x; ++i)
                    {
                        map[i, points[id_1].y] = verticalline[nsd - 1];
                        tempimg = FindName(HorV + "_" + id_1.ToString() + "_" + id_2.ToString() + "_" + i.ToString() + "_" + points[id_1].y.ToString()) as Image;
                        if (tempimg == null)
                        {
                            tempimg = new Image();
                            tempimg.Name = HorV + "_" + id_1.ToString() + "_" + id_2.ToString() + "_" + i.ToString() + "_" + points[id_1].y.ToString();
                            tempimg.VerticalAlignment = VerticalAlignment.Center;
                            tempimg.HorizontalAlignment = HorizontalAlignment.Center;
                            tempimg.SetValue(Grid.RowProperty, i);
                            tempimg.SetValue(Grid.ColumnProperty, points[id_1].y);
                            gd_hashi.RegisterName(tempimg.Name, tempimg);
                            gd_hashi.Children.Add(tempimg);
                            map[i, points[id_1].y] = verticalline[nsd - 1];
                        }
                        tempimg.Source = new BitmapImage(new Uri("Resources/" + HorV + nsd.ToString() + ".png", UriKind.Relative));

                        id_left = nothing;
                        id_right = nothing;
                        for (int j = points[id_1].y - 1; j >= 0; --j)
                        {
                            if (map[i, j] == nothing)
                                continue;
                            if (map[i, j] == verticalline[0] || map[i, j] == verticalline[1])
                                break;
                            if (map[i, j] < points.Count)
                            {
                                id_left = map[i, j];
                                break;
                            }
                        }
                        if (id_left == nothing)
                            continue;
                        for (int j = points[id_1].y + 1; j < columns; ++j)
                        {
                            if (map[i, j] == nothing)
                                continue;
                            if (map[i, j] == verticalline[0] || map[i, j] == verticalline[1])
                                break;
                            if (map[i, j] < points.Count)
                            {
                                id_right = map[i, j];
                                break;
                            }
                        }
                        if (id_right == nothing)
                            continue;
                        points[id_left].right_flag = false;
                        points[id_right].left_flag = false;
                    }
                    points[id_1].down_cnt = nsd;
                    points[id_2].up_cnt = nsd;
                }
            }
            for (int i = 0; i < pre_name_list.Count; ++i)
            {
                tempimg = FindName(pre_name_list[i]) as Image;
                gd_hashi.UnregisterName(tempimg.Name);
                gd_hashi.Children.Remove(tempimg);
            }
            pre_name_list.Clear();
            pre_same = true;
            renewmap();
        }

        private void renewmap()
        {
            Image tempimg;
            int remain = points.Count;
            for (int i = 0; i < points.Count; ++i)
            {
                points[i].available = points[i].num - points[i].up_cnt - points[i].down_cnt - points[i].left_cnt - points[i].right_cnt;
                tempimg = FindName("num" + points[i].id.ToString()) as Image;
                if (points[i].available == 0)
                {
                    tempimg.Opacity = 0.3;
                    remain--;
                }
                else
                {
                    tempimg.Opacity = 1;
                }
            }
            if (remain == 0)
            {
                passstep.Text = (Convert.ToInt32(passstep.Text) + 1).ToString();
                timer.Stop();
                tempimg = new Image();
                tempimg.Name = "win";
                tempimg.Source = new BitmapImage(new Uri("Resources/win.png", UriKind.Relative));
                tempimg.SetValue(Grid.RowProperty, 0);
                tempimg.SetValue(Grid.ColumnProperty, 0);
                tempimg.Opacity = 0.7;
                gd_surface.RegisterName(tempimg.Name, tempimg);
                gd_surface.Children.Add(tempimg);
                return;
            }
            bool save = false;
            int[,] temp_map = map_hist[map_hist.Count - 1];
            for (int i = 0; i < rows; ++i)
            {
                for (int j = 0; j < columns; ++j)
                {
                    if (temp_map[i, j] != map[i, j])
                    {
                        save = true;
                        break;
                    }
                }
                if (save)
                    break;
            }
            if (!save)
                return;
            passstep.Text = (Convert.ToInt32(passstep.Text) + 1).ToString();
            temp_map = new int[rows, columns];
            for (int i = 0; i < rows; ++i)
            {
                for (int j = 0; j < columns; ++j)
                {
                    temp_map[i, j] = map[i, j];
                }
            }
            map_hist.Add(temp_map);
            _points = new List<Point>();
            _points.Clear();
            Point tempp;
            for (int i = 0; i < points.Count; ++i)
            {
                tempp = new Point(points[i]);
                _points.Add(tempp);
            }
            points_hist.Add(_points);
        }

        private int max(int a, int b)
        {
            return (a > b ? a : b);
        }
        private int min(int a, int b)
        {
            return (a < b ? a : b);
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
            Image tempimg;
            for (int i = 0; i < pre_name_list.Count; ++i)
            {
                tempimg = FindName(pre_name_list[i]) as Image;
                gd_hashi.UnregisterName(tempimg.Name);
                gd_hashi.Children.Remove(tempimg);
            }
            pre_name_list.Clear();
            if (id == pre_num_id && !pre_same)
            {
                pre_same = true;
                return;
            }
            pre_same = false;
            pre_num_id = id;
            if (points[id].available == 0)
            {
                if (points[id].up_cnt > 0)
                {
                    for (int i = points[id].x - 1; i > points[points[id].up_id].x; --i)
                    {
                        createpre("vertical", i, points[id].y, points[id].up_id, id, points[id].up_cnt);
                    }
                }
                if (points[id].down_cnt > 0)
                {
                    for (int i = points[id].x + 1; i < points[points[id].down_id].x; ++i)
                    {
                        createpre("vertical", i, points[id].y, id, points[id].down_id, points[id].down_cnt);
                    }
                }
                if (points[id].left_cnt > 0)
                {
                    for (int j = points[id].y - 1; j > points[points[id].left_id].y; --j)
                    {
                        createpre("horizontal", points[id].x, j, points[id].left_id, id, points[id].left_cnt);
                    }
                }
                if (points[id].right_cnt > 0)
                {
                    for (int j = points[id].y + 1; j < points[points[id].right_id].y; ++j)
                    {
                        createpre("horizontal", points[id].x, j, id, points[id].right_id, points[id].right_cnt);
                    }
                }
            }
            else
            {
                if (points[id].up_flag)
                {
                    if (points[points[id].up_id].available == 0)
                    {
                        if (points[id].up_cnt > 0)
                        {
                            for (int i = points[id].x - 1; i > points[points[id].up_id].x; --i)
                            {
                                createpre("vertical", i, points[id].y, points[id].up_id, id, points[points[id].up_id].down_cnt);
                            }
                        }
                    }
                    else
                    {
                        for (int i = points[id].x - 1; i > points[points[id].up_id].x; --i)
                        {
                            createpre("vertical", i, points[id].y, points[id].up_id, id, min(points[id].available + points[id].up_cnt, points[points[id].up_id].available + points[points[id].up_id].down_cnt));
                        }
                    }
                }
                if (points[id].down_flag)
                {
                    if (points[points[id].down_id].available == 0)
                    {
                        if (points[id].down_cnt > 0)
                        {
                            for (int i = points[id].x + 1; i < points[points[id].down_id].x; ++i)
                            {
                                createpre("vertical", i, points[id].y, id, points[id].down_id, points[points[id].down_id].up_cnt);
                            }
                        }
                    }
                    else
                    {
                        for (int i = points[id].x + 1; i < points[points[id].down_id].x; ++i)
                        {
                            createpre("vertical", i, points[id].y, id, points[id].down_id, min(points[id].available + points[id].down_cnt, points[points[id].down_id].available + points[points[id].down_id].up_cnt));
                        }
                    }
                }
                if (points[id].left_flag)
                {
                    if (points[points[id].left_id].available == 0)
                    {
                        if (points[id].left_cnt > 0)
                        {
                            for (int j = points[id].y - 1; j > points[points[id].left_id].y; --j)
                            {
                                createpre("horizontal", points[id].x, j, points[id].left_id, id, points[points[id].left_id].right_cnt);
                            }
                        }
                    }
                    else
                    {
                        for (int j = points[id].y - 1; j > points[points[id].left_id].y; --j)
                        {
                            createpre("horizontal", points[id].x, j, points[id].left_id, id, min(points[id].available + points[id].left_cnt, points[points[id].left_id].available + points[points[id].left_id].right_cnt));
                        }
                    }
                }
                if (points[id].right_flag)
                {
                    if (points[points[id].right_id].available == 0)
                    {
                        if (points[id].right_cnt > 0)
                        {
                            for (int j = points[id].y + 1; j < points[points[id].right_id].y; ++j)
                            {
                                createpre("horizontal", points[id].x, j, id, points[id].right_id, points[points[id].right_id].left_cnt);
                            }
                        }
                    }
                    else
                    {
                        for (int j = points[id].y + 1; j < points[points[id].right_id].y; ++j)
                        {
                            createpre("horizontal", points[id].x, j, id, points[id].right_id, min(points[id].available + points[id].right_cnt, points[points[id].right_id].available + points[points[id].right_id].left_cnt));
                        }
                    }
                }
            }
        }
        List<string> pre_name_list;
        private void createpre(string mode, int x, int y, int id_1, int id_2, int upper)
        {
            Image tempimg;
            tempimg = new Image();
            tempimg.Name = mode + "pre_" + id_1.ToString() + "_" + id_2.ToString() + "_" + x.ToString() + "_" + y.ToString() + "_" + upper.ToString();
            pre_name_list.Add(tempimg.Name);
            tempimg.SetValue(Grid.RowProperty, x);
            tempimg.SetValue(Grid.ColumnProperty, y);
            Uri uri = new Uri("Resources/" + mode + "pre.png", UriKind.Relative);
            tempimg.Source = new BitmapImage(uri);
            tempimg.VerticalAlignment = VerticalAlignment.Center;
            tempimg.HorizontalAlignment = HorizontalAlignment.Center;
            tempimg.Opacity = 0.75;
            tempimg.MouseDown += pre_down;
            gd_hashi.Children.Add(tempimg);
            gd_hashi.RegisterName(tempimg.Name, tempimg);
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
            }
            else if (mode == "selfDefining")
            {
                int p_txt = 0;
                string tl = s_list[p_txt++];
                rows = Convert.ToInt32(tl);
                tl = s_list[p_txt++];
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
                p_txt++;

                tl = s_list[p_txt++];
                pointCnt = Convert.ToInt32(tl);
                p_txt++;

                int x, y, num;
                Point newpoint;
                for (int i = 0; i < pointCnt; ++i)
                {
                    x = Convert.ToInt32(s_list[p_txt++]);
                    y = Convert.ToInt32(s_list[p_txt++]);
                    num = Convert.ToInt32(s_list[p_txt++]);
                    newpoint = new Point(x, y, num, i);
                    points.Add(newpoint);
                    p_txt++;
                }
            }
            draw_points();
            make_map();
            Point tempp;
            _points = new List<Point>();
            for (int i = 0; i < points.Count; ++i)
            {
                tempp = new Point(points[i]);
                _points.Add(tempp);
            }
            points_hist.Add(_points);
        }
        private void make_map()
        {
            map = new int[rows,columns];
            for (int i = 0; i < rows; ++i)
            {
                for (int j = 0; j < columns; ++j)
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
            map_hist = new List<int[,]>();
            map_hist.Clear();
            int[,] temp_map = new int[rows,columns];
            for (int i = 0; i < rows; ++i)
            {
                for (int j = 0; j < columns; ++j)
                {
                    temp_map[i, j] = map[i, j];
                }
            }
            map_hist.Add(temp_map);
        }
        private void draw_points()
        {
            Image newnum;
            for (int i = 0; i < points.Count; ++i)
            {
                newnum = new Image();
                newnum.Name = "num" + points[i].id.ToString();
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
            timer.Stop();
            AItimer.Stop();
            passstep.Text = "0";
            passtime.Text = "0";
            timer.Start();
            points.Clear();
            Point tempp;
            for (int i = 0; i < points_hist[0].Count; ++i)
            {
                tempp = new Point(points_hist[0][i]);
                points.Add(tempp);
            }
            points_hist.RemoveRange(1, points_hist.Count - 1);
            pre_name_list.Clear();
            choosing_nsd = false;
            make_map();
            UIElementCollection Childrens = gd_hashi.Children;
            Image tempimg;
            tempimg = FindName("win") as Image;
            if (tempimg != null)
            {
                gd_surface.UnregisterName(tempimg.Name);
                gd_surface.Children.Remove(tempimg);
            }
            int upper = Childrens.Count;
            for (int i = upper - 1; i >= 0; --i)
            {
                tempimg = Childrens[i] as Image;
                gd_hashi.UnregisterName(tempimg.Name);
                gd_hashi.Children.Remove(tempimg);
            }
            draw_points();
        }

        private void retract_click(object sender, MouseButtonEventArgs e)
        {
            Image tempimg;
            string name;
            string[] name_split;
            for (int i = gd_hashi.Children.Count - 1; i >= 0; --i)
            {
                tempimg = gd_hashi.Children[i] as Image;
                name = tempimg.Name;
                 name_split = name.Split('_');
                if (name_split[0] == "shader" || name_split[0] == "vertical_pre" || name_split[0] == "horizontalpre" || name_split[0] == "nsdchoose")
                {
                    gd_hashi.UnregisterName(tempimg.Name);
                    gd_hashi.Children.Remove(tempimg);
                }
            }
            choosing_nsd = false;
            pre_same = true;
            if (map_hist.Count == 1)
                return;
            points_hist.RemoveAt(points_hist.Count - 1);
            points.Clear();
            Point tempp;
            for (int i = 0; i < points_hist[points_hist.Count - 1].Count; ++i)
            {
                tempp = new Point(points_hist[points_hist.Count - 1][i]);
                points.Add(tempp);
            }
            int[,] map_now, map_pre;
            map_now = new int[rows, columns];
            for (int i = 0; i < rows; ++i)
            {
                for (int j = 0; j < columns; ++j)
                {
                    map_now[i,j] = map_hist[map_hist.Count - 1][i,j];
                }
            }
            map_hist.RemoveAt(map_hist.Count - 1);
            map_pre = new int[rows, columns];

            int id_1 = nothing, id_2 = nothing;
            for (int i = 0; i < rows; ++i)
            {
                for (int j = 0; j < columns; ++j)
                {
                    map[i, j] = map_pre[i, j] = map_hist[map_hist.Count - 1][i, j];
                    if (map_pre[i, j] != map_now[i, j])
                    {
                        if (map_now[i, j] == horizontalline[0] || map_now[i, j] == horizontalline[1])
                        {
                            for (int k = j; k >= 0; --k)
                            {
                                if (map_now[i, k] < points.Count)
                                {
                                    id_1 = map_now[i, k];
                                    break;
                                }
                            }
                            for (int k = j; k < columns; ++k)
                            {
                                if (map_now[i, k] < points.Count)
                                {
                                    id_2 = map_now[i, k];
                                    break;
                                }
                            }
                            tempimg = FindName("horizontal_" + id_1.ToString() + "_" + id_2.ToString() + "_" + i.ToString() + "_" + j.ToString()) as Image;
                            if (map_pre[i, j] == horizontalline[0])
                            {
                                tempimg.Source = new BitmapImage(new Uri("Resources/horizontal1.png", UriKind.Relative));
                            }
                            else if(map_pre[i, j] == horizontalline[1])
                            {
                                tempimg.Source = new BitmapImage(new Uri("Resources/horizontal2.png", UriKind.Relative));
                            }
                            else if (map_pre[i, j] == nothing)
                            {
                                gd_hashi.UnregisterName(tempimg.Name);
                                gd_hashi.Children.Remove(tempimg);
                            }
                        }
                        else if (map_now[i, j] == verticalline[0] || map_now[i, j] == verticalline[1])
                        {
                            for (int k = i; k >= 0; --k)
                            {
                                if (map_now[k, j] < points.Count)
                                {
                                    id_1 = map_now[k, j];
                                    break;
                                }
                            }
                            for (int k = i; k < rows; ++k)
                            {
                                if (map_now[k, j] < points.Count)
                                {
                                    id_2 = map_now[k, j];
                                    break;
                                }
                            }
                            tempimg = FindName("vertical_" + id_1.ToString() + "_" + id_2.ToString() + "_" + i.ToString() + "_" + j.ToString()) as Image;
                            if (map_pre[i, j] == verticalline[0])
                            {
                                tempimg.Source = new BitmapImage(new Uri("Resources/vertical1.png", UriKind.Relative));
                            }
                            else if (map_pre[i, j] == verticalline[1])
                            {
                                tempimg.Source = new BitmapImage(new Uri("Resources/vertical2.png", UriKind.Relative));
                            }
                            else if (map_pre[i, j] == nothing)
                            {
                                gd_hashi.UnregisterName(tempimg.Name);
                                gd_hashi.Children.Remove(tempimg);
                            }
                        }
                        else if (map_now[i, j] == nothing)
                        {
                            if (map_pre[i, j] == horizontalline[0] || map_pre[i, j] == horizontalline[1])
                            {
                                tempimg = new Image();
                                tempimg.Name = "horizontal_" + id_1.ToString() + "_" + id_2.ToString() + "_" + i.ToString() + "_" + j.ToString();
                                tempimg.VerticalAlignment = VerticalAlignment.Center;
                                tempimg.HorizontalAlignment = HorizontalAlignment.Center;
                                tempimg.Source = new BitmapImage(new Uri("Resources/horizontal" + (map_pre[i, j] == horizontalline[0] ? 1 : 2).ToString() + ".png", UriKind.Relative));
                            }
                            else
                            {
                                tempimg = new Image();
                                tempimg.Name = "vertical_" + id_1.ToString() + "_" + id_2.ToString() + "_" + i.ToString() + "_" + j.ToString();
                                tempimg.VerticalAlignment = VerticalAlignment.Center;
                                tempimg.HorizontalAlignment = HorizontalAlignment.Center;
                                tempimg.Source = new BitmapImage(new Uri("Resources/vertical" + (map_pre[i, j] == verticalline[0] ? 1 : 2).ToString() + ".png", UriKind.Relative));
                            }
                            tempimg.SetValue(Grid.RowProperty, i);
                            tempimg.SetValue(Grid.ColumnProperty, j);
                            gd_hashi.RegisterName(tempimg.Name, tempimg);
                            gd_hashi.Children.Add(tempimg);
                        }
                    }
                    else if (map[i, j] < points.Count)
                    {
                        tempimg = FindName("num" + map_pre[i, j].ToString()) as Image;
                        if (points[map_pre[i, j]].available > 0)
                        {
                            tempimg.Opacity = 1;
                        }
                        else
                        {
                            tempimg.Opacity = 0.3;
                        }
                    }
                }
            }
            passstep.Text = (Convert.ToInt32(passstep.Text)+1).ToString();
        }

        bool AIon;
        TimeSpan delta_t;
        DispatcherTimer AItimer;
        MouseButtonEventArgs t;
        string temp_path;
        string[] pathsplit;
        private void AIstart_Click(object sender, MouseButtonEventArgs e)
        {
            if (AIon)
            {
                AItimer.Stop();
                AIon = false;
                return;
            }
            delta_t = new TimeSpan(0, 0, 0, (int)AIdeltaT.Value, ((int)(AIdeltaT.Value * 1000)) % 1000);
            temp_path = null;
            t = e;
            p_path = 0;
            reset_click(FindName("reset"), e);
            temp_path = path[p_path++];
            AItimer.Interval = delta_t;
            AItimer.Start();
        }

        private void deltaTchange(object sender, RoutedPropertyChangedEventArgs<double> e)
        {
            delta_t = new TimeSpan(0, 0, (int)AIdeltaT.Value, ((int)(AIdeltaT.Value * 1000)) % 1000);
            AItimer.Interval = delta_t;
            AItimer.Start();
        }

        void AItimer_Tick(object sender, EventArgs e)
        {
            int id_1, id_2, cnt;
            Image tempimg;
            if (p_path >= path.Count)
            {
                AItimer.Stop();
                return;
            }
            pathsplit = new string[0];
            pathsplit = temp_path.Split('_');
            if (pathsplit[0] == "retract")
            {
                tempimg = FindName("retract") as Image;
                retract_click(tempimg, t);
                temp_path = path[p_path++];
            }
            else if (pathsplit[0] == "horizontal" || pathsplit[0] == "vertical")
            {
                id_1 = Convert.ToInt32(pathsplit[1]);
                id_2 = Convert.ToInt32(pathsplit[2]);
                cnt = Convert.ToInt32(pathsplit[3]);
                tempimg = FindName("num" + id_1.ToString()) as Image;
                num_down(tempimg, t);
                temp_path = pathsplit[0] + "pre_" + pathsplit[1] + "_" + pathsplit[2] + "_" + pathsplit[3];
            }
            else if (pathsplit[0] == "horizontalpre" || pathsplit[0] == "verticalpre")
            {
                id_1 = Convert.ToInt32(pathsplit[1]);
                id_2 = Convert.ToInt32(pathsplit[2]);
                cnt = Convert.ToInt32(pathsplit[3]);

                if (id_1 > id_2)
                {
                    id_1 ^= id_2;
                    id_2 ^= id_1;
                    id_1 ^= id_2;
                }
                if (pathsplit[0] == "verticalpre")
                {
                    for (int i = 1; ; ++i)
                    {
                        tempimg = FindName("verticalpre_" + id_1.ToString() + "_" + id_2.ToString() + "_" + (points[id_1].x + 1).ToString() + "_" + points[id_1].y.ToString() + "_" + i.ToString()) as Image;
                        if (tempimg == null)
                            continue;
                        else break;
                    }
                }
                else
                {
                    for (int i = 1; ; ++i)
                    {
                        tempimg = FindName("horizontalpre_" + id_1.ToString() + "_" + id_2.ToString() + "_" + points[id_1].x.ToString() + "_" + (points[id_1].y + 1).ToString() + "_" + i.ToString()) as Image;
                        if (tempimg == null)
                            continue;
                        else break;
                    }
                }
                pre_down(tempimg, t);
                temp_path = "nsdchoose_" + pathsplit[3] + "_" + pathsplit[0].Replace("pre", "") + "_" + id_1.ToString() + "_" + id_2.ToString();
            }
            else if (pathsplit[0] == "nsdchoose")
            {
                tempimg = FindName(temp_path) as Image;
                System.Threading.Thread.Sleep((int)AIdeltaT.Value * 1000);
                nsdchoose_down(tempimg, t);
                temp_path = path[p_path++];
            }
            else
            {
                AItimer.Stop();
            }
        }
    }
}
