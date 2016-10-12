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
    }
}
