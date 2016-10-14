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
            bool obvious;
            int hole,dire,temp,retract_cnt;
            do
            {
                obvious = false;
                retract_cnt = 0;
                for (int i = 0; i < points.Count; ++i)
                {
                    hole = 0;
                    dire = 0;
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
                            id_left = id_right = nothing;
                            points[i].up_cnt += temp;
                            points[points[i].up_id].down_cnt += temp;
                            points[i].available -= temp;
                            points[points[i].up_id].available -= temp;
                            path.Add("add_" + i.ToString() + "_" + points[i].up_id.ToString() + "_" + points[i].up_cnt);
                            retract_cnt++;
                            for (int x = points[i].x - 1; x > points[points[i].up_id].x; --x)
                            {
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
                            id_left = id_right = nothing;
                            points[i].down_cnt += temp;
                            points[points[i].down_id].up_cnt += temp;
                            points[i].available -= temp;
                            points[points[i].down_id].available -= temp;
                            path.Add("add_" + i.ToString() + "_" + points[i].down_id.ToString() + "_" + points[i].up_cnt);
                            retract_cnt++;
                            for (int x = points[i].x + 1; x < points[points[i].down_id].x; ++x)
                            {
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
                            id_up = id_down = nothing;
                            points[i].left_cnt += temp;
                            points[points[i].left_id].right_cnt += temp;
                            points[i].available -= temp;
                            points[points[i].left_id].available -= temp;
                            path.Add("add_" + i.ToString() + "_" + points[i].left_id.ToString() + "_" + points[i].up_cnt);
                            retract_cnt++;
                            for (int y = points[i].y - 1; y > points[points[i].left_id].y; --y)
                            {
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
                            id_up = id_down = nothing;
                            points[i].right_cnt += temp;
                            points[points[i].right_id].left_cnt += temp;
                            points[i].available -= temp;
                            points[points[i].right_id].available -= temp;
                            path.Add("add_" + i.ToString() + "_" + points[i].right_id.ToString() + "_" + points[i].up_cnt);
                            retract_cnt++;
                            for (int y = points[i].y + 1; y < points[points[i].right_id].y; ++y)
                            {
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
                                id_left = id_right = nothing;
                                points[i].up_cnt += temp;
                                points[points[i].up_id].down_cnt += temp;
                                points[i].available -= temp;
                                points[points[i].up_id].available -= temp;
                                path.Add("add_" + i.ToString() + "_" + points[i].up_id.ToString() + "_" + points[i].up_cnt);
                                retract_cnt++;
                                for (int x = points[i].x - 1; x > points[points[i].up_id].x; --x)
                                {
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
                                id_left = id_right = nothing;
                                points[i].down_cnt += temp;
                                points[points[i].down_id].up_cnt += temp;
                                points[i].available -= temp;
                                points[points[i].down_id].available -= temp;
                                path.Add("add_" + i.ToString() + "_" + points[i].down_id.ToString() + "_" + points[i].down_cnt);
                                retract_cnt++;
                                for (int x = points[i].x + 1; x < points[points[i].down_id].x; ++x)
                                {
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
                                id_up = id_down = nothing;
                                points[i].left_cnt += temp;
                                points[points[i].left_id].right_cnt += temp;
                                points[i].available -= temp;
                                points[points[i].left_id].available -= temp;
                                path.Add("add_" + i.ToString() + "_" + points[i].left_id.ToString() + "_" + points[i].left_cnt);
                                retract_cnt++;
                                for (int y = points[i].y - 1; y > points[points[i].left_id].y; --y)
                                {
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
                                id_up = id_down = nothing;
                                points[i].right_cnt += temp;
                                points[points[i].right_id].left_cnt += temp;
                                points[i].available -= temp;
                                points[points[i].right_id].available -= temp;
                                path.Add("add_" + i.ToString() + "_" + points[i].right_id.ToString() + "_" + points[i].right_cnt);
                                retract_cnt++;
                                for (int y = points[i].y + 1; y < points[points[i].right_id].y; ++y)
                                {
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
                            if (points[i].available == 3)
                            {
                                obvious = true;
                                if (points[i].up_flag)
                                {
                                    temp = 1;
                                    id_left = id_right = nothing;
                                    points[i].up_cnt += temp;
                                    points[points[i].up_id].down_cnt += temp;
                                    points[i].available -= temp;
                                    points[points[i].up_id].available -= temp;
                                    path.Add("add_" + i.ToString() + "_" + points[i].up_id.ToString() + "_" + points[i].up_cnt);
                                    retract_cnt++;
                                    for (int x = points[i].x - 1; x > points[points[i].up_id].x; --x)
                                    {
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
                                    id_left = id_right = nothing;
                                    points[i].down_cnt += temp;
                                    points[points[i].down_id].up_cnt += temp;
                                    points[i].available -= temp;
                                    points[points[i].down_id].available -= temp;
                                    path.Add("add_" + i.ToString() + "_" + points[i].down_id.ToString() + "_" + points[i].up_cnt);
                                    retract_cnt++;
                                    for (int x = points[i].x + 1; x < points[points[i].down_id].x; ++x)
                                    {
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
                                    id_up = id_down = nothing;
                                    points[i].left_cnt += temp;
                                    points[points[i].left_id].right_cnt += temp;
                                    points[i].available -= temp;
                                    points[points[i].left_id].available -= temp;
                                    path.Add("add_" + i.ToString() + "_" + points[i].left_id.ToString() + "_" + points[i].up_cnt);
                                    retract_cnt++;
                                    for (int y = points[i].y - 1; y > points[points[i].left_id].y; --y)
                                    {
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
                                    id_up = id_down = nothing;
                                    points[i].right_cnt += temp;
                                    points[points[i].right_id].left_cnt += temp;
                                    points[i].available -= temp;
                                    points[points[i].right_id].available -= temp;
                                    path.Add("add_" + i.ToString() + "_" + points[i].right_id.ToString() + "_" + points[i].up_cnt);
                                    retract_cnt++;
                                    for (int y = points[i].y + 1; y < points[points[i].right_id].y; ++y)
                                    {
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
                for (int i = 0; i < points.Count; ++i)
                {
                    if (points[i].available == 0)
                    {
                        if (points[i].up_flag)
                        {
                            points[i].up_flag = points[points[i].up_id].down_flag = false;
                        }
                        if (points[i].down_flag)
                        {
                            points[i].down_flag = points[points[i].down_id].up_flag = false;
                        }
                        if (points[i].left_flag)
                        {
                            points[i].left_flag = points[points[i].left_id].right_flag = false;
                        }
                        if (points[i].right_flag)
                        {
                            points[i].right_flag = points[points[i].right_id].left_flag = false;
                        }
                    }
                }
            } while (obvious);
            int notsettled = points.Count, minimun_choice = nothing, upper = nothing;
            for (int i = 0; i < points.Count; ++i)
            {
                hole = dire = 0;
                if (points[i].available == 0)
                    notsettled--;
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
                
            }
            if (notsettled == 0)
            {
                return true;
            }
            /*
             * 
             * 
             * 
             * 要加判断 （展开状态数最少的点，尝试通过dire、hole、available来找到）
             * 展开后不成功便成仁 ：） 不可能不成功  必然有一种是对的
             * 注意这里加points的连边时，要判断是否会吵num上限，需不需要先删（稳妥的话可以全删掉）
             * 
             * 
             * 
             * 
             * 下面那个return是搞笑的
             */
            return false;
        }

        internal static void find_solution(int[,] m, int r, int c, List<Point> p, List<string> _path)
        {
            nothing = 999;
            horizontal = new int[3];
            vertical = new int[3];
            horizontal[0] = nothing; horizontal[1] = 998; horizontal[2] = 997;
            vertical[0] = nothing; vertical[1] = 996; vertical[2] = 995;
            path = new List<string>();
            path.Clear();
            rows = r;
            columns = c;
            dfs(m, p);
            for (int i = 0; i <path.Count; ++i)
            {
                _path.Add(path[i]);
            }
        }



        private static int C(int a, int b)
        {
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
