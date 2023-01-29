using System;
using System.Collections.Generic;
using System.Linq;
using System.Reflection.Metadata;
using System.Security.Policy;
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
using System.Windows.Threading;

namespace _2022Fall_IERG3080_Matching_Pairs
{
    /// <summary>
    /// Interaction logic for MainWindow.xaml
    /// </summary>
    public partial class MainWindow : Window
    {
        private static Random rng = new Random();
        private static List<int> gameMap = new()
        {
            0, 1, 2, 3, 4, 5, 6, 7, 8, 9,
            0, 1, 2, 3, 4, 5, 6, 7, 8, 9,
        };
        private static List<SolidColorBrush> colorTable = new()
            {
                Brushes.Red,
                Brushes.Blue,
                Brushes.Lime,
                Brushes.Azure,
                Brushes.Purple,
                Brushes.LightPink,
                Brushes.LightBlue,
                Brushes.Orange,
                Brushes.PaleGreen,
                Brushes.Violet
            };
        private static int lastOpen1 = -1;
        private static int lastOpen2 = -1;
        private static bool closeDelay = false;
        private static int success = 0;

        private static System.Windows.Threading.DispatcherTimer dispatcherTimer = new ();
        private static int nowMinute = 0;
        private static int nowSecond = 0;



        public MainWindow()
        {
            InitializeComponent();
            dispatcherTimer.Tick += TimerTick;
            dispatcherTimer.Interval = TimeSpan.FromSeconds(1);
            GameInit();
        }

        private void TimerTick(object? sender, EventArgs e)
        {
            // Update timer
            nowSecond++;
            if (nowSecond >= 60)
            {
                nowSecond -= 60;
                nowMinute++;
            }
            timer_text.Text = "Time: "+ nowMinute.ToString() + ":" + (nowSecond<10 ? "0":"") + nowSecond.ToString();

            // Close card
            if (closeDelay)
            {
                closeDelay = false;
            }
            else if (lastOpen1 != -1 && lastOpen2 != -1)
            {
                CloseCard((image_cards.Children[lastOpen1] as Button)!);
                CloseCard((image_cards.Children[lastOpen2] as Button)!);
                lastOpen1 = lastOpen2 = -1;
            }
        }

        public static void Shuffle<T>(IList<T> list)
        {
            int n = list.Count;
            while (n > 1)
            {
                n--;
                int k = rng.Next(n + 1);
                (list[n], list[k]) = (list[k], list[n]);
            }
        }

        private void GameInit()
        {
            Shuffle(gameMap);
            for (int i=0; i<20; i++)
            {
                Button? btn = image_cards.Children[i] as Button;

                CloseCard(btn!);
                btn!.Tag = i.ToString();
                btn!.IsEnabled = true;
            }

            dispatcherTimer.Start();
            nowMinute = nowSecond = 0;
            lastOpen1 = lastOpen2 = -1;
            win_text.Visibility = Visibility.Collapsed;
            success = 0;
            match_indicator.Text = "";
        }

        private void OpenCard(Button btn, int pos)
        {
            btn.Content = (gameMap[pos]+1).ToString();
            btn.Background = colorTable[gameMap[pos]];
            btn.Foreground = Brushes.Black;
        }

        private void CloseCard(Button btn)
        {
            btn.Content = "?";
            btn.Background = Brushes.Black;
            btn.Foreground = Brushes.White;
        }

        private void CardClick(object sender, RoutedEventArgs e)
        {
            Button? btn = e.Source as Button;
            int pos = Int32.Parse(btn!.Tag as String ?? "0");
            
            if (lastOpen1 == -1)
            {
                lastOpen1 = pos;
            }
            else if (lastOpen2 == -1)
            {
                if (lastOpen1 == pos) return; // click twice
                lastOpen2 = pos;
                closeDelay = true;
                if (gameMap[lastOpen1] == gameMap[lastOpen2])
                {
                    success++;
                    (image_cards.Children[lastOpen1] as Button)!.IsEnabled = false;
                    (image_cards.Children[lastOpen2] as Button)!.IsEnabled = false;
                    match_indicator.Text = "Match!";
                    match_indicator.Foreground = Brushes.Green;
                    if (success == 10)
                    {
                        GameOver();
                    }
                    lastOpen1 = lastOpen2 = -1;   
                }
                else
                {
                    match_indicator.Text = "Not Match!";
                    match_indicator.Foreground = Brushes.Red;
                }
            }
            else
            {
                return;
            }
            OpenCard(btn!, pos);
        }

        private void GameOver()
        {
            dispatcherTimer.Stop();
            win_text.Visibility = Visibility.Visible;
        }

        private void RestartButtonClick(object sender, RoutedEventArgs e)
        {
            GameInit();
        }
    }
}
