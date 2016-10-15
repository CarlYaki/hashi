using System;
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
using System.Windows.Media.Effects;
using System.Windows.Media.Imaging;
using System.Windows.Navigation;
using System.Windows.Shapes;

namespace hashi
{
    /// <summary>
    /// MainWindow.xaml 的交互逻辑
    /// </summary>
    public partial class MainWindow : Window
    {
        private int easyLevelCnt = 2;
        private int mediumLevelCnt = 2;
        private int hardLevelCnt = 3;
        private string preMode = "";

        public MainWindow()
        {
            InitializeComponent();
            /*Image img_exit = FindName("ibtn_end") as Image;
            img_exit.Source = new BitmapImage(new Uri("Resources/exit.png", UriKind.Relative));
            Image img_start = FindName("ibtn_start") as Image;
            img_start.Source = new BitmapImage(new Uri("Resources/start.png", UriKind.Relative));*/
            Tools.showSlowly(this,Starting);
        }

        private void hashi_start(object sender, MouseButtonEventArgs e)
        {
            ComboBox modeSelection = FindName("modeSelection") as ComboBox;
            if (modeSelection == null)
                return;
            string mode = modeSelection.SelectionBoxItem.ToString();
            if (mode == "")
            {
                MessageBox.Show("Please choose mode.");
            }
            ComboBox levelSelection = FindName("levelSelection") as ComboBox;
            string s_level;
            int i_level;
            switch (mode)
            {
                case "Easy":
                    if (levelSelection == null)
                        return;
                    s_level = levelSelection.SelectionBoxItem.ToString();
                    if (s_level == "")
                    {
                        MessageBox.Show("Please choose level.");
                        return;
                    }
                    i_level = Convert.ToInt32(s_level);
                    (new Window_Hashi(mode, i_level)).Show();
                    this.Close();
                    break;
                case "Medium":
                    if (levelSelection == null)
                        return;
                    s_level = levelSelection.SelectionBoxItem.ToString();
                    if (s_level == "")
                    {
                        MessageBox.Show("Please choose level.");
                        return;
                    }
                    i_level = Convert.ToInt32(s_level);
                    (new Window_Hashi(mode, i_level)).Show();
                    this.Close();
                    break;
                case "Hard":
                    if (levelSelection == null)
                        return;
                    s_level = levelSelection.SelectionBoxItem.ToString();
                    if (s_level == "")
                    {
                        MessageBox.Show("Please choose level.");
                        return;
                    }
                    i_level = Convert.ToInt32(s_level);
                    (new Window_Hashi(mode, i_level)).Show();
                    this.Close();
                    break;
                case "Self Defining":
                    break;
                case "Random New":
                    break;
                default:
                    return;
            }
        }

        private void modeSelete(object sender, EventArgs e)
        {
            Grid settings = FindName("gd_settings") as Grid;
            ComboBox modeSelection = sender as ComboBox;
            if(modeSelection.SelectionBoxItem.ToString() == preMode)
            {
                return;
            }
            else
            {
                preMode = modeSelection.SelectionBoxItem.ToString();
            }

            TextBlock levelMain = FindName("levelMain") as TextBlock;
            if (levelMain != null)
            {
                settings.Children.Remove(levelMain);
                settings.UnregisterName("levelMain");
            }
            ComboBox levelSelection = FindName("levelSelection") as ComboBox;
            if (levelSelection != null)
            {
                settings.Children.Remove(levelSelection);
                settings.UnregisterName("levelSelection");
            }
            TextBlock selfRows = FindName("selfRows") as TextBlock;
            if (selfRows != null)
            {
                settings.Children.Remove(selfRows);
                settings.UnregisterName("selfRows");
            }
            ComboBox rowsSelection = FindName("rowsSelection") as ComboBox;
            if (rowsSelection != null)
            {
                settings.Children.Remove(rowsSelection);
                settings.UnregisterName("rowsSelection");
            }
            TextBlock selfColumns = FindName("selfColumns") as TextBlock;
            if (selfRows != null)
            {
                settings.Children.Remove(selfColumns);
                settings.UnregisterName("selfColumns");
            }
            ComboBox columnsSelection = FindName("columnsSelection") as ComboBox;
            if (columnsSelection != null)
            {
                settings.Children.Remove(columnsSelection);
                settings.UnregisterName("columnsSelection");
            }

            if (modeSelection.SelectionBoxItem.ToString() == "Easy")
            {
                levelMain = new TextBlock();
                levelMain.Name = "levelMain";
                settings.RegisterName("levelMain", levelMain);
                levelMain.Width = 120;
                levelMain.Height = 16;
                levelMain.FontSize = 12;
                levelMain.TextAlignment = TextAlignment.Center;
                levelMain.HorizontalAlignment = HorizontalAlignment.Left;
                levelMain.VerticalAlignment = VerticalAlignment.Center;
                settings.Children.Add(levelMain);
                levelMain.SetValue(Grid.RowProperty, 2);
                levelMain.Text = "Level:";

                levelSelection = new ComboBox();
                levelSelection.Name = "levelSelection";
                settings.RegisterName("levelSelection", levelSelection);
                levelSelection.Width = 120;
                levelSelection.HorizontalAlignment = HorizontalAlignment.Right;
                levelSelection.VerticalAlignment = VerticalAlignment.Center;
                levelSelection.Margin = new Thickness(0,0,40,0);
                settings.Children.Add(levelSelection);
                levelSelection.SetValue(Grid.RowProperty, 2);
                
                levelSelection.Items.Clear();
                for (int i = 0; i < easyLevelCnt; ++i)
                {
                    levelSelection.Items.Add(i.ToString());
                }
            }
            else if (modeSelection.SelectionBoxItem.ToString() == "Medium")
            {
                levelMain = FindName("levelMain") as TextBlock;
                if (levelMain == null)
                {
                    levelMain = new TextBlock();
                    levelMain.Name = "levelMain";
                    settings.RegisterName("levelMain", levelMain);
                    levelMain.Width = 120;
                    levelMain.Height = 16;
                    levelMain.FontSize = 12;
                    levelMain.TextAlignment = TextAlignment.Center;
                    levelMain.HorizontalAlignment = HorizontalAlignment.Left;
                    levelMain.VerticalAlignment = VerticalAlignment.Center;
                    settings.Children.Add(levelMain);
                    levelMain.SetValue(Grid.RowProperty, 2);
                }
                levelMain.Text = "Level:";

                levelSelection = new ComboBox();
                levelSelection.Name = "levelSelection";
                settings.RegisterName("levelSelection", levelSelection);
                levelSelection.Width = 120;
                levelSelection.HorizontalAlignment = HorizontalAlignment.Right;
                levelSelection.VerticalAlignment = VerticalAlignment.Center;
                levelSelection.Margin = new Thickness(0, 0, 40, 0);
                settings.Children.Add(levelSelection);
                levelSelection.SetValue(Grid.RowProperty, 2);

                levelSelection.Items.Clear();
                for (int i = 0; i < mediumLevelCnt; ++i)
                {
                    levelSelection.Items.Add(i.ToString());
                }
            }
            else if (modeSelection.SelectionBoxItem.ToString() == "Hard")
            {
                levelMain = new TextBlock();
                levelMain.Name = "levelMain";
                settings.RegisterName("levelMain", levelMain);
                levelMain.Width = 120;
                levelMain.Height = 16;
                levelMain.FontSize = 12;
                levelMain.TextAlignment = TextAlignment.Center;
                levelMain.HorizontalAlignment = HorizontalAlignment.Left;
                levelMain.VerticalAlignment = VerticalAlignment.Center;
                settings.Children.Add(levelMain);
                levelMain.SetValue(Grid.RowProperty, 2);
                levelMain.Text = "Level:";

                levelSelection = new ComboBox();
                levelSelection.Name = "levelSelection";
                settings.RegisterName("levelSelection", levelSelection);
                levelSelection.Width = 120;
                levelSelection.HorizontalAlignment = HorizontalAlignment.Right;
                levelSelection.VerticalAlignment = VerticalAlignment.Center;
                levelSelection.Margin = new Thickness(0, 0, 40, 0);
                settings.Children.Add(levelSelection);
                levelSelection.SetValue(Grid.RowProperty, 2);

                levelSelection.Items.Clear();
                for (int i = 0; i < hardLevelCnt; ++i)
                {
                    levelSelection.Items.Add(i.ToString());
                }
            }
            else if (modeSelection.SelectionBoxItem.ToString() == "Random New")
            { }
            else if (modeSelection.SelectionBoxItem.ToString() == "Self Defining")
            {
                selfRows = new TextBlock();
                selfRows.Name = "levelMain";
                settings.RegisterName("selfRows", selfRows);
                selfRows.Text = "Rows:";
                selfRows.Width = 120;
                selfRows.Height = 16;
                selfRows.FontSize = 12;
                selfRows.TextAlignment = TextAlignment.Center;
                selfRows.HorizontalAlignment = HorizontalAlignment.Left;
                selfRows.VerticalAlignment = VerticalAlignment.Center;
                settings.Children.Add(selfRows);
                selfRows.SetValue(Grid.RowProperty, 2);


                rowsSelection = new ComboBox();
                rowsSelection.Name = "rowsSelection";
                settings.RegisterName("rowsSelection", rowsSelection);
                rowsSelection.Width = 60;
                rowsSelection.HorizontalAlignment = HorizontalAlignment.Right;
                rowsSelection.VerticalAlignment = VerticalAlignment.Center;
                rowsSelection.Margin = new Thickness(0, 0, 70, 0);
                settings.Children.Add(rowsSelection);
                rowsSelection.SetValue(Grid.RowProperty, 2);

                rowsSelection.Items.Clear();
                for (int i = 7; i < 15; ++i)
                {
                    rowsSelection.Items.Add(i.ToString());
                }

                selfColumns = new TextBlock();
                selfColumns.Name = "levelMain";
                settings.RegisterName("selfColumns", selfColumns);
                selfColumns.Text = "Columns:";
                selfColumns.Width = 120;
                selfColumns.Height = 16;
                selfColumns.FontSize = 12;
                selfColumns.TextAlignment = TextAlignment.Center;
                selfColumns.HorizontalAlignment = HorizontalAlignment.Left;
                selfColumns.VerticalAlignment = VerticalAlignment.Center;
                settings.Children.Add(selfColumns);
                selfColumns.SetValue(Grid.RowProperty, 3);

                columnsSelection = new ComboBox();
                columnsSelection.Name = "columnsSelection";
                settings.RegisterName("columnsSelection", columnsSelection);
                columnsSelection.Width = 60;
                columnsSelection.HorizontalAlignment = HorizontalAlignment.Right;
                columnsSelection.VerticalAlignment = VerticalAlignment.Center;
                columnsSelection.Margin = new Thickness(0, 0, 70, 0);
                settings.Children.Add(columnsSelection);
                columnsSelection.SetValue(Grid.RowProperty,3);

                columnsSelection.Items.Clear();
                for (int i = 7; i < 15; ++i)
                {
                    columnsSelection.Items.Add(i.ToString());
                }
            }
        }

        private void start_enter(object sender, MouseEventArgs e)
        {
            Image img_start = FindName("ibtn_start") as Image;
            img_start.Source = new BitmapImage(new Uri("Resources/start_enter.png", UriKind.Relative));
        }

        private void start_leave(object sender, MouseEventArgs e)
        {
            Image img_start = FindName("ibtn_start") as Image;
            img_start.Source = new BitmapImage(new Uri("Resources/start.png", UriKind.Relative));
        }

        private void hashi_end(object sender, MouseButtonEventArgs e)
        {
            this.Close();
        }

        private void end_enter(object sender, MouseEventArgs e)
        {
            Image ibtn_exit = (Image)sender;
            DropShadowEffect shining = new DropShadowEffect();
            shining.ShadowDepth=0;
            shining.Color=Color.FromArgb(255,0,0,0);  
            shining.Opacity=1;
            shining.BlurRadius = 20;
            ibtn_exit.Effect = shining;
        }

        private void end_leave(object sender, MouseEventArgs e)
        {
            Image ibtn_exit = (Image)sender;
            DropShadowEffect shining = new DropShadowEffect();
            shining.ShadowDepth = 0;
            shining.Color = Color.FromArgb(255, 0, 0, 0);
            shining.Opacity = 0;
            shining.BlurRadius = 20;
            ibtn_exit.Effect = shining;
        }

    }
}
