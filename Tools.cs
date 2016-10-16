using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Media.Animation;

namespace hashi
{
    class Tools
    {
        /// <summary>
        /// 淡出DependencyObject控件
        /// </summary>
        /// <param name="window">控件所在窗口</param>
        /// <param name="control">控件</param>
        internal static void showSlowly(Window window, FrameworkElement control)
        {
            Storyboard sb = new Storyboard();//创建故事板
            window.Resources.Add(Guid.NewGuid().ToString(), sb);//将故事板添加到window中
            DoubleAnimation da = new DoubleAnimation();//创建动画
            Storyboard.SetTarget(da, control);//将动画添加到控件
            Storyboard.SetTargetProperty(da, new PropertyPath("Opacity", new object[] { }));//设置动画属性
            da.From = 0;
            da.To = 1;
            sb.Duration = new Duration(TimeSpan.FromSeconds(1));
            sb.Children.Add(da);//将动画添加到故事板
            sb.Begin();//开始故事板
        }

        private static int[] horizontal, vertical;
        private static int rows, columns, nothing;
        private static List<string> path;

        private static void renewpoint(int[,] map, List<Point> p)
        {
            for (int i = 0; i < p.Count; ++i)
            {
                p[i].available = p[i].num - p[i].up_cnt - p[i].down_cnt - p[i].left_cnt - p[i].right_cnt;
            }
            for (int i = 0; i < p.Count; ++i)
            {
                if (p[i].available == 0)
                {
                    if (p[i].up_flag)
                    {
                        p[i].up_flag = p[p[i].up_id].down_flag = false;
                    }
                    if (p[i].down_flag)
                    {
                        p[i].down_flag = p[p[i].down_id].up_flag = false;
                    }
                    if (p[i].left_flag)
                    {
                        p[i].left_flag = p[p[i].left_id].right_flag = false;
                    }
                    if (p[i].right_flag)
                    {
                        p[i].right_flag = p[p[i].right_id].left_flag = false;
                    }
                }
            }
            int[] id = new int[4];
            for (int i = 0; i < rows; ++i)
            {
                for (int j = 0; j < columns; ++j)
                {
                    for (int k = 0; k < 4; ++k)
                        id[k] = nothing;
                    if (map[i, j] == vertical[1] || map[i, j] == vertical[2])
                    {
                        for (int k = j - 1; k >= 0; --k)
                        {
                            if (map[i, k] < p.Count)
                            {
                                id[1] = map[i, k];
                                break;
                            }
                        }
                        if(id[1] == nothing)
                            continue;
                        for (int k = j + 1; k < columns; ++k)
                        {
                            if (map[i, k] < p.Count)
                            {
                                id[3] = map[i, k];
                                break;
                            }
                        }
                        if (id[3] == nothing)
                            continue;
                        p[id[1]].right_flag = p[id[3]].left_flag = false;
                    }
                    if (map[i, j] == horizontal[1] || map[i, j] == horizontal[2])
                    {
                        for (int k = i - 1; k >= 0; --k)
                        {
                            if (map[k, j] < p.Count)
                            {
                                id[0] = map[k, j];
                                break;
                            }
                        }
                        if (id[0] == nothing)
                            continue;
                        for (int k = i + 1; k < rows; ++k)
                        {
                            if (map[k, j] < p.Count)
                            {
                                id[2] = map[k, j];
                                break;
                            }
                        }
                        if (id[2] == nothing)
                            continue;
                        p[id[0]].down_flag = p[id[2]].up_flag = false;
                    }
                }
            }
        }

        private static bool dfs(int[,] m, List<Point> p)
        {
            int[,] map = new int[rows,columns];
            for (int i = 0; i < rows; ++i)
            {
                for (int j = 0; j < columns; ++j)
                {
                    map[i, j] = m[i, j];
                }
            }
            List<Point> points=new List<Point>();
            points.Clear();
            Point tempp;
            for (int i = 0; i < p.Count; ++i)
            {
                tempp = new Point(p[i]);
                points.Add(tempp);
            }
            renewpoint(map, points);
            bool obvious;
            int hole,dire,temp,retract_cnt;
            int[] _hole = new int[4];
            do
            {
                obvious = false;
                retract_cnt = 0;
                for (int i = 0; i < points.Count; ++i)
                {
                    if (points[i].available == 0)
                        continue;
                    hole = 0;
                    for (int t = 0; t < 4; ++t)
                    {
                        _hole[t] = 0;
                    }
                    dire = 0;
                    if (points[i].up_flag)
                    {
                        hole += (_hole[0] = min(2 - points[i].up_cnt, points[points[i].up_id].available));
                        dire++;
                    }
                    if (points[i].down_flag)
                    {
                        hole += (_hole[2] = min(2 - points[i].down_cnt, points[points[i].down_id].available));
                        dire++;
                    }
                    if (points[i].left_flag)
                    {
                        hole += (_hole[1] = min(2 - points[i].left_cnt, points[points[i].left_id].available));
                        dire++;
                    }
                    if (points[i].right_flag)
                    {
                        hole += (_hole[3] = min(2 - points[i].right_cnt, points[points[i].right_id].available));
                        dire++;
                    }
                    if (hole < points[i].available)
                    {
                        while (retract_cnt-- > 0)
                        {
                            path.Add("retract");
                        }
                        return false;
                    }
                    else if (hole == points[i].available)
                    {
                        obvious = true;
                        int id_left, id_right, id_up, id_down;
                        if (points[i].up_flag)
                        {
                            temp = min(2 - points[i].up_cnt, points[points[i].up_id].available);
                            points[i].up_cnt += temp;
                            points[points[i].up_id].down_cnt += temp;
                            points[i].available -= temp;
                            points[points[i].up_id].available -= temp;
                            path.Add("vertical_" + i.ToString() + "_" + points[i].up_id.ToString() + "_" + points[i].up_cnt);
                            retract_cnt++;
                            for (int x = points[i].x - 1; x > points[points[i].up_id].x; --x)
                            {
                                id_left = id_right = nothing;
                                map[x, points[i].y] = vertical[points[i].up_cnt];
                                for (int y = points[i].y - 1; y >= 0; --y)
                                {
                                    if (map[x, y] < points.Count)
                                    {
                                        id_left = map[x, y];
                                        break;
                                    }
                                }
                                if (id_left == nothing)
                                    continue;
                                for (int y = points[i].y + 1; y < columns; ++y)
                                {
                                    if (map[x, y] < points.Count)
                                    {
                                        id_right = map[x, y];
                                        break;
                                    }
                                }
                                if (id_right == nothing)
                                    continue;
                                points[id_left].right_flag = points[id_right].left_flag = false;
                            }
                        }
                        if (points[i].down_flag)
                        {
                            temp = min(2 - points[i].down_cnt, points[points[i].down_id].available);
                            points[i].down_cnt += temp;
                            points[points[i].down_id].up_cnt += temp;
                            points[i].available -= temp;
                            points[points[i].down_id].available -= temp;
                            path.Add("vertical_" + i.ToString() + "_" + points[i].down_id.ToString() + "_" + points[i].down_cnt);
                            retract_cnt++;
                            for (int x = points[i].x + 1; x < points[points[i].down_id].x; ++x)
                            {
                                id_left = id_right = nothing;
                                map[x, points[i].y] = vertical[points[i].down_cnt];
                                for (int y = points[i].y - 1; y >= 0; --y)
                                {
                                    if (map[x, y] < points.Count)
                                    {
                                        id_left = map[x, y];
                                        break;
                                    }
                                }
                                if (id_left == nothing)
                                    continue;
                                for (int y = points[i].y + 1; y < columns; ++y)
                                {
                                    if (map[x, y] < points.Count)
                                    {
                                        id_right = map[x, y];
                                        break;
                                    }
                                }
                                if (id_right == nothing)
                                    continue;
                                points[id_left].right_flag = points[id_right].left_flag = false;
                            }
                        }
                        if (points[i].left_flag)
                        {
                            temp = min(2 - points[i].left_cnt, points[points[i].left_id].available);
                            points[i].left_cnt += temp;
                            points[points[i].left_id].right_cnt += temp;
                            points[i].available -= temp;
                            points[points[i].left_id].available -= temp;
                            path.Add("horizontal_" + i.ToString() + "_" + points[i].left_id.ToString() + "_" + points[i].left_cnt);
                            retract_cnt++;
                            for (int y = points[i].y - 1; y > points[points[i].left_id].y; --y)
                            {
                                id_up = id_down = nothing;
                                map[points[i].x, y] = horizontal[points[i].left_cnt];
                                for (int x = points[i].x - 1; x >= 0; --x)
                                {
                                    if (map[x, y] < points.Count)
                                    {
                                        id_up = map[x, y];
                                        break;
                                    }
                                }
                                if (id_up == nothing)
                                    continue;
                                for (int x = points[i].x + 1; x < columns; ++x)
                                {
                                    if (map[x, y] < points.Count)
                                    {
                                        id_down = map[x, y];
                                        break;
                                    }
                                }
                                if (id_down == nothing)
                                    continue;
                                points[id_up].down_flag = points[id_down].up_flag = false;
                            }
                        }
                        if (points[i].right_flag)
                        {
                            temp = min(2 - points[i].right_cnt, points[points[i].right_id].available);
                            points[i].right_cnt += temp;
                            points[points[i].right_id].left_cnt += temp;
                            points[i].available -= temp;
                            points[points[i].right_id].available -= temp;
                            path.Add("horizontal_" + i.ToString() + "_" + points[i].right_id.ToString() + "_" + points[i].right_cnt);
                            retract_cnt++;
                            for (int y = points[i].y + 1; y < points[points[i].right_id].y; ++y)
                            {
                                id_up = id_down = nothing;
                                map[points[i].x, y] = horizontal[points[i].right_cnt];
                                for (int x = points[i].x - 1; x >= 0; --x)
                                {
                                    if (map[x, y] < points.Count)
                                    {
                                        id_up = map[x, y];
                                        break;
                                    }
                                }
                                if (id_up == nothing)
                                    continue;
                                for (int x = points[i].x + 1; x < columns; ++x)
                                {
                                    if (map[x, y] < points.Count)
                                    {
                                        id_down = map[x, y];
                                        break;
                                    }
                                }
                                if (id_down == nothing)
                                    continue;
                                points[id_up].down_flag = points[id_down].up_flag = false;
                            }
                        }
                    }
                    else
                    {
                        int id_up, id_down, id_left, id_right;
                        if (dire == 1)
                        {
                            obvious = true;
                            if (points[i].up_flag)
                            {
                                temp = points[i].available;
                                points[i].up_cnt += temp;
                                points[points[i].up_id].down_cnt += temp;
                                points[i].available -= temp;
                                points[points[i].up_id].available -= temp;
                                path.Add("vertical_" + i.ToString() + "_" + points[i].up_id.ToString() + "_" + points[i].up_cnt);
                                retract_cnt++;
                                for (int x = points[i].x - 1; x > points[points[i].up_id].x; --x)
                                {
                                    id_left = id_right = nothing;
                                    map[x, points[i].y] = vertical[points[i].up_cnt];
                                    for (int y = points[i].y - 1; y >= 0; --y)
                                    {
                                        if (map[x, y] < points.Count)
                                        {
                                            id_left = map[x, y];
                                            break;
                                        }
                                    }
                                    if (id_left == nothing)
                                        continue;
                                    for (int y = points[i].y + 1; y < columns; ++y)
                                    {
                                        if (map[x, y] < points.Count)
                                        {
                                            id_right = map[x, y];
                                            break;
                                        }
                                    }
                                    if (id_right == nothing)
                                        continue;
                                    points[id_left].right_flag = points[id_right].left_flag = false;
                                }
                            }
                            if (points[i].down_flag)
                            {
                                temp = points[i].available;
                                points[i].down_cnt += temp;
                                points[points[i].down_id].up_cnt += temp;
                                points[i].available -= temp;
                                points[points[i].down_id].available -= temp;
                                path.Add("vertical_" + i.ToString() + "_" + points[i].down_id.ToString() + "_" + points[i].down_cnt);
                                retract_cnt++;
                                for (int x = points[i].x + 1; x < points[points[i].down_id].x; ++x)
                                {
                                    id_left = id_right = nothing;
                                    map[x, points[i].y] = vertical[points[i].down_cnt];
                                    for (int y = points[i].y - 1; y >= 0; --y)
                                    {
                                        if (map[x, y] < points.Count)
                                        {
                                            id_left = map[x, y];
                                            break;
                                        }
                                    }
                                    if (id_left == nothing)
                                        continue;
                                    for (int y = points[i].y + 1; y < columns; ++y)
                                    {
                                        if (map[x, y] < points.Count)
                                        {
                                            id_right = map[x, y];
                                            break;
                                        }
                                    }
                                    if (id_right == nothing)
                                        continue;
                                    points[id_left].right_flag = points[id_right].left_flag = false;
                                }
                            }
                            if (points[i].left_flag)
                            {
                                temp = points[i].available;
                                points[i].left_cnt += temp;
                                points[points[i].left_id].right_cnt += temp;
                                points[i].available -= temp;
                                points[points[i].left_id].available -= temp;
                                path.Add("horizontal_" + i.ToString() + "_" + points[i].left_id.ToString() + "_" + points[i].left_cnt);
                                retract_cnt++;
                                for (int y = points[i].y - 1; y > points[points[i].left_id].y; --y)
                                {
                                    id_up = id_down = nothing;
                                    map[points[i].x, y] = horizontal[points[i].left_cnt];
                                    for (int x = points[i].x - 1; x >= 0; --x)
                                    {
                                        if (map[x, y] < points.Count)
                                        {
                                            id_up = map[x, y];
                                            break;
                                        }
                                    }
                                    if (id_up == nothing)
                                        continue;
                                    for (int x = points[i].x + 1; x < columns; ++x)
                                    {
                                        if (map[x, y] < points.Count)
                                        {
                                            id_down = map[x, y];
                                            break;
                                        }
                                    }
                                    if (id_down == nothing)
                                        continue;
                                    points[id_up].down_flag = points[id_down].up_flag = false;
                                }
                            }
                            if (points[i].right_flag)
                            {
                                temp = points[i].available;
                                points[i].right_cnt += temp;
                                points[points[i].right_id].left_cnt += temp;
                                points[i].available -= temp;
                                points[points[i].right_id].available -= temp;
                                path.Add("horizontal_" + i.ToString() + "_" + points[i].right_id.ToString() + "_" + points[i].right_cnt);
                                retract_cnt++;
                                for (int y = points[i].y + 1; y < points[points[i].right_id].y; ++y)
                                {
                                    id_up = id_down = nothing;
                                    map[points[i].x, y] = horizontal[points[i].right_cnt];
                                    for (int x = points[i].x - 1; x >= 0; --x)
                                    {
                                        if (map[x, y] < points.Count)
                                        {
                                            id_up = map[x, y];
                                            break;
                                        }
                                    }
                                    if (id_up == nothing)
                                        continue;
                                    for (int x = points[i].x + 1; x < columns; ++x)
                                    {
                                        if (map[x, y] < points.Count)
                                        {
                                            id_down = map[x, y];
                                            break;
                                        }
                                    }
                                    if (id_down == nothing)
                                        continue;
                                    points[id_up].down_flag = points[id_down].up_flag = false;
                                }
                            }
                        }
                        else if (dire * 2 - 1 == points[i].available)
                        {
                            obvious = true;
                            if (points[i].up_flag)
                            {
                                temp = 1;
                                points[i].up_cnt += temp;
                                points[points[i].up_id].down_cnt += temp;
                                points[i].available -= temp;
                                points[points[i].up_id].available -= temp;
                                path.Add("vertical_" + i.ToString() + "_" + points[i].up_id.ToString() + "_" + points[i].up_cnt);
                                retract_cnt++;
                                for (int x = points[i].x - 1; x > points[points[i].up_id].x; --x)
                                {
                                    id_left = id_right = nothing;
                                    map[x, points[i].y] = vertical[points[i].up_cnt];
                                    for (int y = points[i].y - 1; y >= 0; --y)
                                    {
                                        if (map[x, y] < points.Count)
                                        {
                                            id_left = map[x, y];
                                            break;
                                        }
                                    }
                                    if (id_left == nothing)
                                        continue;
                                    for (int y = points[i].y + 1; y < columns; ++y)
                                    {
                                        if (map[x, y] < points.Count)
                                        {
                                            id_right = map[x, y];
                                            break;
                                        }
                                    }
                                    if (id_right == nothing)
                                        continue;
                                    points[id_left].right_flag = points[id_right].left_flag = false;
                                }
                            }
                            if (points[i].down_flag)
                            {
                                temp = 1;
                                points[i].down_cnt += temp;
                                points[points[i].down_id].up_cnt += temp;
                                points[i].available -= temp;
                                points[points[i].down_id].available -= temp;
                                path.Add("vertical_" + i.ToString() + "_" + points[i].down_id.ToString() + "_" + points[i].down_cnt);
                                retract_cnt++;
                                for (int x = points[i].x + 1; x < points[points[i].down_id].x; ++x)
                                {
                                    id_left = id_right = nothing;
                                    map[x, points[i].y] = vertical[points[i].down_cnt];
                                    for (int y = points[i].y - 1; y >= 0; --y)
                                    {
                                        if (map[x, y] < points.Count)
                                        {
                                            id_left = map[x, y];
                                            break;
                                        }
                                    }
                                    if (id_left == nothing)
                                        continue;
                                    for (int y = points[i].y + 1; y < columns; ++y)
                                    {
                                        if (map[x, y] < points.Count)
                                        {
                                            id_right = map[x, y];
                                            break;
                                        }
                                    }
                                    if (id_right == nothing)
                                        continue;
                                    points[id_left].right_flag = points[id_right].left_flag = false;
                                }
                            }
                            if (points[i].left_flag)
                            {
                                temp = 1;
                                points[i].left_cnt += temp;
                                points[points[i].left_id].right_cnt += temp;
                                points[i].available -= temp;
                                points[points[i].left_id].available -= temp;
                                path.Add("horizontal_" + i.ToString() + "_" + points[i].left_id.ToString() + "_" + points[i].left_cnt);
                                retract_cnt++;
                                for (int y = points[i].y - 1; y > points[points[i].left_id].y; --y)
                                {
                                    id_up = id_down = nothing;
                                    map[points[i].x, y] = horizontal[points[i].left_cnt];
                                    for (int x = points[i].x - 1; x >= 0; --x)
                                    {
                                        if (map[x, y] < points.Count)
                                        {
                                            id_up = map[x, y];
                                            break;
                                        }
                                    }
                                    if (id_up == nothing)
                                        continue;
                                    for (int x = points[i].x + 1; x < columns; ++x)
                                    {
                                        if (map[x, y] < points.Count)
                                        {
                                            id_down = map[x, y];
                                            break;
                                        }
                                    }
                                    if (id_down == nothing)
                                        continue;
                                    points[id_up].down_flag = points[id_down].up_flag = false;
                                }
                            }
                            if (points[i].right_flag)
                            {
                                temp = 1;
                                points[i].right_cnt += temp;
                                points[points[i].right_id].left_cnt += temp;
                                points[i].available -= temp;
                                points[points[i].right_id].available -= temp;
                                path.Add("horizontal_" + i.ToString() + "_" + points[i].right_id.ToString() + "_" + points[i].right_cnt);
                                retract_cnt++;
                                for (int y = points[i].y + 1; y < points[points[i].right_id].y; ++y)
                                {
                                    id_up = id_down = nothing;
                                    map[points[i].x, y] = horizontal[points[i].right_cnt];
                                    for (int x = points[i].x - 1; x >= 0; --x)
                                    {
                                        if (map[x, y] < points.Count)
                                        {
                                            id_up = map[x, y];
                                            break;
                                        }
                                    }
                                    if (id_up == nothing)
                                        continue;
                                    for (int x = points[i].x + 1; x < columns; ++x)
                                    {
                                        if (map[x, y] < points.Count)
                                        {
                                            id_down = map[x, y];
                                            break;
                                        }
                                    }
                                    if (id_down == nothing)
                                        continue;
                                    points[id_up].down_flag = points[id_down].up_flag = false;
                                }
                            }
                        }
                        else
                        {
                            if (points[i].available == hole - 1)
                            {
                                if (hole - dire > 0)
                                {
                                    obvious = true;
                                    if (_hole[0] == 2)
                                    {
                                        temp = 1;
                                        points[i].up_cnt += temp;
                                        points[points[i].up_id].down_cnt += temp;
                                        points[i].available -= temp;
                                        points[points[i].up_id].available -= temp;
                                        path.Add("vertical_" + i.ToString() + "_" + points[i].up_id.ToString() + "_" + points[i].up_cnt);
                                        retract_cnt++;
                                        for (int x = points[i].x - 1; x > points[points[i].up_id].x; --x)
                                        {
                                            id_left = id_right = nothing;
                                            map[x, points[i].y] = vertical[points[i].up_cnt];
                                            for (int y = points[i].y - 1; y >= 0; --y)
                                            {
                                                if (map[x, y] < points.Count)
                                                {
                                                    id_left = map[x, y];
                                                    break;
                                                }
                                            }
                                            if (id_left == nothing)
                                                continue;
                                            for (int y = points[i].y + 1; y < columns; ++y)
                                            {
                                                if (map[x, y] < points.Count)
                                                {
                                                    id_right = map[x, y];
                                                    break;
                                                }
                                            }
                                            if (id_right == nothing)
                                                continue;
                                            points[id_left].right_flag = points[id_right].left_flag = false;
                                        }
                                    }
                                    if (_hole[2] == 2)
                                    {
                                        temp = 1;
                                        points[i].down_cnt += temp;
                                        points[points[i].down_id].up_cnt += temp;
                                        points[i].available -= temp;
                                        points[points[i].down_id].available -= temp;
                                        path.Add("vertical_" + i.ToString() + "_" + points[i].down_id.ToString() + "_" + points[i].down_cnt);
                                        retract_cnt++;
                                        for (int x = points[i].x + 1; x < points[points[i].down_id].x; ++x)
                                        {
                                            id_left = id_right = nothing;
                                            map[x, points[i].y] = vertical[points[i].down_cnt];
                                            for (int y = points[i].y - 1; y >= 0; --y)
                                            {
                                                if (map[x, y] < points.Count)
                                                {
                                                    id_left = map[x, y];
                                                    break;
                                                }
                                            }
                                            if (id_left == nothing)
                                                continue;
                                            for (int y = points[i].y + 1; y < columns; ++y)
                                            {
                                                if (map[x, y] < points.Count)
                                                {
                                                    id_right = map[x, y];
                                                    break;
                                                }
                                            }
                                            if (id_right == nothing)
                                                continue;
                                            points[id_left].right_flag = points[id_right].left_flag = false;
                                        }
                                    }
                                    if (_hole[1] == 2)
                                    {
                                        temp = 1;
                                        points[i].left_cnt += temp;
                                        points[points[i].left_id].right_cnt += temp;
                                        points[i].available -= temp;
                                        points[points[i].left_id].available -= temp;
                                        path.Add("horizontal_" + i.ToString() + "_" + points[i].left_id.ToString() + "_" + points[i].left_cnt);
                                        retract_cnt++;
                                        for (int y = points[i].y - 1; y > points[points[i].left_id].y; --y)
                                        {
                                            id_up = id_down = nothing;
                                            map[points[i].x, y] = horizontal[points[i].left_cnt];
                                            for (int x = points[i].x - 1; x >= 0; --x)
                                            {
                                                if (map[x, y] < points.Count)
                                                {
                                                    id_up = map[x, y];
                                                    break;
                                                }
                                            }
                                            if (id_up == nothing)
                                                continue;
                                            for (int x = points[i].x + 1; x < columns; ++x)
                                            {
                                                if (map[x, y] < points.Count)
                                                {
                                                    id_down = map[x, y];
                                                    break;
                                                }
                                            }
                                            if (id_down == nothing)
                                                continue;
                                            points[id_up].down_flag = points[id_down].up_flag = false;
                                        }
                                    }
                                    if (_hole[3] == 2)
                                    {
                                        temp = 1;
                                        points[i].right_cnt += temp;
                                        points[points[i].right_id].left_cnt += temp;
                                        points[i].available -= temp;
                                        points[points[i].right_id].available -= temp;
                                        path.Add("horizontal_" + i.ToString() + "_" + points[i].right_id.ToString() + "_" + points[i].right_cnt);
                                        retract_cnt++;
                                        for (int y = points[i].y + 1; y < points[points[i].right_id].y; ++y)
                                        {
                                            id_up = id_down = nothing;
                                            map[points[i].x, y] = horizontal[points[i].right_cnt];
                                            for (int x = points[i].x - 1; x >= 0; --x)
                                            {
                                                if (map[x, y] < points.Count)
                                                {
                                                    id_up = map[x, y];
                                                    break;
                                                }
                                            }
                                            if (id_up == nothing)
                                                continue;
                                            for (int x = points[i].x + 1; x < columns; ++x)
                                            {
                                                if (map[x, y] < points.Count)
                                                {
                                                    id_down = map[x, y];
                                                    break;
                                                }
                                            }
                                            if (id_down == nothing)
                                                continue;
                                            points[id_up].down_flag = points[id_down].up_flag = false;
                                        }
                                    }
                                }
                            }
                        }
                    }
                    renewpoint(map, points);
                }
            } while (obvious);
            int notsettled = points.Count, minimum_choice = nothing, upper = nothing;
            for (int i = 0; i < points.Count; ++i)
            {
                hole = dire = 0;
                if (points[i].available == 0)
                {
                    notsettled--;
                    continue;
                }
                if (points[i].up_flag)
                {
                    hole += min(2 - points[i].up_cnt, points[points[i].up_id].available);
                    dire++;
                }
                if (points[i].down_flag)
                {
                    hole += min(2 - points[i].down_cnt, points[points[i].down_id].available);
                    dire++;
                }
                if (points[i].left_flag)
                {
                    hole += min(2 - points[i].left_cnt, points[points[i].left_id].available);
                    dire++;
                }
                if (points[i].right_flag)
                {
                    hole += min(2 - points[i].right_cnt, points[points[i].right_id].available);
                    dire++;
                }
                /*
                 * 要加判断 （展开状态数最少的点，尝试通过dire、hole、available来找到）
                 * 展开后不成功便成仁 ：） 不可能不成功  必然有一种是对的
                 * 注意这里加points的连边时，要判断是否会吵num上限，需不需要先删（稳妥的话可以全删掉）
                 */
                temp = find_choice(dire, hole, points[i].available);
                if (temp < upper)
                {
                    minimum_choice = i;
                    upper = temp;
                }
            }
            if (notsettled == 0)
            {
                return true;
            }
            int[] min_hole = new int[4];
            dire = 0;
            hole = 0;
            if (points[minimum_choice].up_flag)
            {
                min_hole[0] = min(2 - points[minimum_choice].up_cnt, points[points[minimum_choice].up_id].available);
                dire++;
            }
            if (points[minimum_choice].down_flag)
            {
                min_hole[2] = min(2 - points[minimum_choice].down_cnt, points[points[minimum_choice].down_id].available);
                dire++;
            }
            if (points[minimum_choice].left_flag)
            {
                min_hole[1] = min(2 - points[minimum_choice].left_cnt, points[points[minimum_choice].left_id].available);
                dire++;
            }
            if (points[minimum_choice].right_flag)
            {
                min_hole[3] = min(2 - points[minimum_choice].right_cnt, points[points[minimum_choice].right_id].available);
                dire++;
            }
            for (int i = 0; i < 4; ++i)
                hole += min_hole[i];
            int d = hole - dire, s = dire * 2 - hole, ava = points[minimum_choice].available;
            /*
             * 尝试minimum_choice的几个方案
             */

            for (int i = min(ava, min_hole[0]); i >= 0; --i)
            {
                hole -= min_hole[0];
                ava -= i;
                if (ava <= hole)
                {
                    if (i > 0)
                    {
                        points[minimum_choice].up_cnt += i;
                        points[points[minimum_choice].up_id].down_cnt += i;
                        for (int x = points[minimum_choice].x - 1; x > points[points[minimum_choice].up_id].x; --x)
                        {
                            map[x, points[minimum_choice].y] = vertical[points[minimum_choice].up_cnt];
                        }
                        path.Add("vertical_" + minimum_choice.ToString() + "_" + points[minimum_choice].up_id.ToString() + "_" + points[minimum_choice].up_cnt.ToString());
                    }
                    for (int j = min(ava, min_hole[1]); j >= 0; --j)
                    {
                        hole -= min_hole[1];
                        ava -= j;
                        if (ava <= hole)
                        {
                            if (j > 0)
                            {
                                points[minimum_choice].left_cnt += j;
                                points[points[minimum_choice].left_id].right_cnt += j;
                                for (int y = points[minimum_choice].y - 1; y > points[points[minimum_choice].left_id].y; --y)
                                {
                                    map[points[minimum_choice].x, y] = horizontal[points[minimum_choice].left_cnt];
                                }
                                path.Add("horizontal_" + minimum_choice.ToString() + "_" + points[minimum_choice].left_id.ToString() + "_" + points[minimum_choice].left_cnt.ToString());
                            }
                            for (int k = min(ava, min_hole[2]); k >= 0; --k)
                            {
                                hole -= min_hole[2];
                                ava -= k;
                                if (ava <= hole)
                                {
                                    if (k > 0)
                                    {
                                        points[minimum_choice].down_cnt += k;
                                        points[points[minimum_choice].down_id].up_cnt += k;
                                        for (int x = points[minimum_choice].x + 1; x < points[points[minimum_choice].down_id].x; ++x)
                                        {
                                            map[x, points[minimum_choice].y] = vertical[points[minimum_choice].down_cnt];
                                        }
                                        path.Add("vertical_" + minimum_choice.ToString() + "_" + points[minimum_choice].down_id.ToString() + "_" + points[minimum_choice].down_cnt.ToString());
                                    }
                                    if (ava > 0)
                                    {
                                        points[minimum_choice].right_cnt += ava;
                                        points[points[minimum_choice].right_id].left_cnt += ava;
                                        for (int y = points[minimum_choice].y + 1; y < points[points[minimum_choice].right_id].y; ++y)
                                        {
                                            map[points[minimum_choice].x, y] = horizontal[points[minimum_choice].right_cnt];
                                        }
                                        path.Add("horizontal_" + minimum_choice.ToString() + "_" + points[minimum_choice].right_id.ToString() + "_" + points[minimum_choice].right_cnt.ToString());
                                    }

                                    if (dfs(map, points))
                                        return true;

                                    if (ava > 0)
                                    {
                                        points[minimum_choice].right_cnt -= ava;
                                        points[points[minimum_choice].right_id].left_cnt -= ava;
                                        for (int y = points[minimum_choice].y + 1; y < points[points[minimum_choice].right_id].y; ++y)
                                        {
                                            map[points[minimum_choice].x, y] = horizontal[points[minimum_choice].right_cnt];
                                        }
                                        path.Add("retract");
                                    }
                                    if (k > 0)
                                    {
                                        points[minimum_choice].down_cnt -= k;
                                        points[points[minimum_choice].down_id].up_cnt -= k;
                                        for (int x = points[minimum_choice].x + 1; x < points[points[minimum_choice].down_id].x; ++x)
                                        {
                                            map[x, points[minimum_choice].y] = vertical[points[minimum_choice].down_cnt];
                                        }
                                        path.Add("retract");
                                    }
                                }
                                hole += min_hole[2];
                                ava += k;
                            }
                            if (j > 0)
                            {
                                points[minimum_choice].left_cnt -= j;
                                points[points[minimum_choice].left_id].right_cnt -= j;
                                for (int y = points[minimum_choice].y - 1; y > points[points[minimum_choice].left_id].y; --y)
                                {
                                    map[points[minimum_choice].x, y] = horizontal[points[minimum_choice].left_cnt];
                                }
                                path.Add("retract");
                            }
                        }
                        hole += min_hole[1];
                        ava += j;
                    }
                    if (i > 0)
                    {
                        points[minimum_choice].up_cnt -= i;
                        points[points[minimum_choice].up_id].down_cnt -= i;
                        for (int x = points[minimum_choice].x - 1; x > points[points[minimum_choice].up_id].x; --x)
                        {
                            map[x, points[minimum_choice].y] = vertical[points[minimum_choice].up_cnt];
                        }
                        path.Add("retract");
                    }
                }
                hole += min_hole[0];
                ava += i;
            }

            while (retract_cnt-- > 0)
            {
                path.Add("retract");
            }
            return false;
        }

        internal static void find_solution(int[,] m, int r, int c, List<Point> p, List<string> _path)
        {
            nothing = 999;
            horizontal = new int[3];
            vertical = new int[3];
            horizontal[0] = nothing; horizontal[1] = 998; horizontal[2] = 997;
            vertical[0] = nothing; vertical[1] = 996; vertical[2] = 995;
            path = _path;
            path.Clear();
            rows = r;
            columns = c;
            dfs(m, p);
        }



        private static int find_choice(int dires, int holes, int available)
        {
            int ans = 0;
            int s = 2 * dires - holes, d = holes - dires;
            if (d == 0)
            {
                ans = C(s, available);
            }
            else
            {
                for (int i = 0; i <= s; ++i)
                {
                    if (available - i > 2 * d)
                        continue;
                    switch (available - i)
                    {
                        case 0:
                            ans += 1;
                            break;
                        case 1:
                            ans += d;
                            break;
                        case 2:
                            ans += C(d, 2) + d;
                            break;
                        case 3:
                            ans += C(d, 2) * 2 + C(d, 3);
                            break;
                        case 4:
                            ans += C(d, 2) + C(d, 3) * 3 + C(d, 4);
                            break;
                        case 5:
                            ans += C(d, 3) * 3 + C(d, 4) * 4;
                            break;
                        case 6:
                            ans += C(d, 3) + C(4, 2);
                            break;
                        case 7:
                            ans += 4;
                            break;
                        default:
                            ans += 0;
                            break;
                    }
                }
            }
            return ans;
        }
        private static int A(int a, int b)
        {
            if (a == 0)
                return 0;
            if (a < b)
                return 0;
            int ans = 1;
            for (int i = 0; i < b; ++i)
            {
                ans *= (a - i);
            }
            return ans;
        }
        private static int C(int a, int b)
        {
            if (a < b)
                return 0;
            int ans=1;
            for (int i = 1; i <= a; ++i)
            {
                ans *= i;
            }
            for (int i = 1; i <= b; ++i)
            {
                ans /= i;
            }
            for (int i = 1; i <= (a - b); ++i)
            {
                ans /= i;
            }
            return ans;
        }
        private static int max(int a, int b)
        {
            return a > b ? a : b;
        }
        private static int max(int a, int b, int c)
        {
            return max(max(a, b), c);
        }
        private static int max(int a, int b, int c, int d)
        {
            return max(max(a, b, c), d);
        }
        private static int min(int a, int b)
        {
            return a < b ? a : b;
        }
        private static int min(int a, int b, int c)
        {
            return max(max(a, b), c);
        }
        private static int min(int a, int b, int c, int d)
        {
            return max(max(a, b, c), d);
        }
    }
}
