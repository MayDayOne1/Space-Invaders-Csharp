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
using System.Windows.Media.Imaging;
using System.Windows.Navigation;
using System.Windows.Shapes;
using System.Windows.Threading;

namespace Space_Invaders
{
    /// <summary>
    /// Interaction logic for MainWindow.xaml
    /// </summary>
    public partial class MainWindow : Window
    {
        private bool goLeft, goRight = false;
        private List<Rectangle> itemsToRemove = new List<Rectangle>();
        private int enemyImages = 0;
        private int bulletTimer;
        private int bulletTimerLimit = 90;
        private int totalEnemies;
        private int timeBetweenFrames = 41;
        private DispatcherTimer dispatcherTimer = new DispatcherTimer();
        private ImageBrush playerSkin = new ImageBrush();
        private int enemySpeed = 6;


        public MainWindow()
        {
            InitializeComponent();

            dispatcherTimer.Tick += gameManager;
            dispatcherTimer.Interval = TimeSpan.FromMilliseconds(timeBetweenFrames);
            dispatcherTimer.Start();
            playerSkin.ImageSource = new BitmapImage(new Uri("pack://application:,,,/Images/player.png"));
            player.Fill = playerSkin;
            spawnEnemies(8);
        }

        private void Canvas_KeyIsDown(object sender, KeyEventArgs e)
        {
            if(e.Key == Key.Left || e.Key == Key.A)
            {
                goLeft = true;
            }
            if(e.Key == Key.Right || e.Key == Key.D)
            {
                goRight = true;
            }
        }

        private void Canvas_KeyIsUp(object sender, KeyEventArgs e)
        {
            if(e.Key == Key.Left || e.Key == Key.A)
            {
                goLeft = false;
            }

            if(e.Key == Key.Right || e.Key == Key.D)
            {
                goRight = false;
            }

            if(e.Key == Key.Space)
            {
                itemsToRemove.Clear();
                Rectangle bullet = new Rectangle
                {
                    Tag = "bullet",
                    Height = 20,
                    Width = 5,
                    Fill = Brushes.White,
                    Stroke = Brushes.Red
                };

                Canvas.SetTop(bullet, Canvas.GetTop(player) - bullet.Height);
                Canvas.SetLeft(bullet, Canvas.GetLeft(player) - bullet.Width);
                mainCanvas.Children.Add(bullet);
            }

            
        }

        private void enemyBulletSpawner(double x, double y)
        {
            Rectangle enemyBullet = new Rectangle
            {
                Tag = "enemyBullet",
                Height = 40,
                Width = 15,
                Fill = Brushes.Yellow,
                Stroke = Brushes.White,
                StrokeThickness = 5
            };

            Canvas.SetTop(enemyBullet, y);
            Canvas.SetTop(enemyBullet, x);
            mainCanvas.Children.Add(enemyBullet);
        }

        private void spawnEnemies(int limit)
        {
            int left = 0;
            totalEnemies = limit;
            for(int i = 0; i < limit; i++)
            {
                ImageBrush enemySkin = new ImageBrush();
                Rectangle enemy = new Rectangle
                {
                    Tag = "Enemy",
                    Height = 45,
                    Width = 45,
                    Fill = enemySkin
                };
                Canvas.SetTop(enemy, 10);
                Canvas.SetLeft(enemy, left);
                mainCanvas.Children.Add(enemy);
                left -= 60;

                enemyImages = new Random().Next(1, 8);
                string imageName = "pack://application:,,,/Images/Invader" + enemyImages + ".gif";
                enemySkin.ImageSource = new BitmapImage(new Uri(imageName));
                        
               

            }
        }

        private void gameManager(object sender, EventArgs e)
        {

        }
    }
}
