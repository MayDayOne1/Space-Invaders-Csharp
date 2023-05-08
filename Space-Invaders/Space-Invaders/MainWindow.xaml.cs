using System;
using System.Collections.Generic;
using System.Diagnostics;
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
        private int bulletCooldown;
        private int bulletCooldownLimit = 90;
        private int totalEnemies;
        private int timeBetweenFrames = 20;
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
            playerRectangle.Fill = playerSkin;
            spawnEnemies(15);
        }

        private void Canvas_KeyIsDown(object sender, KeyEventArgs e)
        {
            if (e.Key == Key.Left || e.Key == Key.A)
            {
                goLeft = true;
            }
            if (e.Key == Key.Right || e.Key == Key.D)
            {
                goRight = true;
            }
        }

        private void Canvas_KeyIsUp(object sender, KeyEventArgs e)
        {
            if (e.Key == Key.Left || e.Key == Key.A)
            {
                goLeft = false;
            }

            if (e.Key == Key.Right || e.Key == Key.D)
            {
                goRight = false;
            }

            if (e.Key == Key.Space)
            {
                itemsToRemove.Clear();
                playerBulletSpawner();
            }
        }

        private void playerBulletSpawner()
        {
            Rectangle bullet = new Rectangle
            {
                Tag = "Bullet",
                Height = 20,
                Width = 5,
                Fill = Brushes.White,
                Stroke = Brushes.Red
            };

            double bulletLeftOffset = Canvas.GetLeft(playerRectangle) + bullet.Width + 20;

            Canvas.SetTop(bullet, Canvas.GetTop(playerRectangle) - bullet.Height);
            Canvas.SetLeft(bullet, bulletLeftOffset);
            mainCanvas.Children.Add(bullet);
        }


        private void enemyBulletSpawner(double x, double y)
        {
            Rectangle enemyBullet = new Rectangle
            {
                Tag = "EnemyBullet",
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
            Random rnd = new Random();
            totalEnemies = limit;
            for (int i = 0; i < limit; i++)
            {
                ImageBrush enemySkin = randomEnemySkin(rnd);
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
            }
        }

        private ImageBrush randomEnemySkin(Random rnd)
        {
            ImageBrush enemySkin = new ImageBrush();
            int enemyImages = rnd.Next(1, 8);
            string imageName = "pack://application:,,,/Images/Invader" + enemyImages + ".gif";
            enemySkin.ImageSource = new BitmapImage(new Uri(imageName));

            return enemySkin;
        }

        private void movePlayer()
        {
            if (goLeft && Canvas.GetLeft(playerRectangle) > 0)
            {
                Canvas.SetLeft(playerRectangle, Canvas.GetLeft(playerRectangle) - 10);
            }
            else if (goRight && Canvas.GetLeft(playerRectangle) + 80 < Application.Current.MainWindow.Width)
            {
                Canvas.SetLeft(playerRectangle, Canvas.GetLeft(playerRectangle) + 10);
            }
        }

        private void spawnBullets()
        {
            bulletCooldown -= 3;
            if (bulletCooldown < 0)
            {
                enemyBulletSpawner((Canvas.GetLeft(playerRectangle) + 20), 10);
                bulletCooldown = bulletCooldownLimit;
            }
        }

        private void enemySpeedManager()
        {
            if (totalEnemies < 10)
            {
                enemySpeed = 20;
            }
        }

        private void playerBulletManager(Rectangle x)
        {
            Canvas.SetTop(x, Canvas.GetTop(x) - 20);
            Rect bullet = new Rect(Canvas.GetLeft(x), Canvas.GetTop(x), x.Width, x.Height);
            if (Canvas.GetTop(x) < 10) itemsToRemove.Add(x);
            
            foreach (var y in mainCanvas.Children.OfType<Rectangle>())
            {
                if (y is Rectangle && (string)y.Tag == "Enemy")
                {
                    Rect enemy = new Rect(Canvas.GetLeft(y), Canvas.GetTop(y), y.Width, y.Height);

                    if (bullet.IntersectsWith(enemy))
                    {
                        itemsToRemove.Add(x);
                        itemsToRemove.Add(y);
                        totalEnemies--;
                    }
                }
            }
        }
        private void enemyManager(Rectangle x, Rect player)
        {
            Canvas.SetLeft(x, Canvas.GetLeft(x) + enemySpeed);

            if (Canvas.GetLeft(x) > 820)
            {
                Canvas.SetLeft(x, -80);
                Canvas.SetTop(x, Canvas.GetTop(x) + (x.Height + 10));
            }

            Rect enemy = new Rect(Canvas.GetLeft(x), Canvas.GetTop(x), x.Width, x.Height);
            if (player.IntersectsWith(enemy))
            {
                dispatcherTimer.Stop();
                MessageBox.Show("Game Over");
            }
        }

        private void enemyBulletManager(Rectangle x, Rect player)
        {
            Canvas.SetTop(x, Canvas.GetTop(x) + 10);
            if (Canvas.GetTop(x) > 680)
            {
                itemsToRemove.Add(x);
            }

            Rect enemyBullets = new Rect(Canvas.GetLeft(x), Canvas.GetTop(x), x.Width, x.Height);
            if (enemyBullets.IntersectsWith(player))
            {
                dispatcherTimer.Stop();
                MessageBox.Show("Game Over");
            }
        }

        private void gameManager(object sender, EventArgs e)
        {
            Rect player = new Rect(Canvas.GetLeft(playerRectangle),
                Canvas.GetTop(playerRectangle),
                playerRectangle.Width,
                playerRectangle.Height);

            movePlayer();
            spawnBullets();
            enemySpeedManager();

            foreach (var x in mainCanvas.Children.OfType<Rectangle>())
            {
                if (x is Rectangle && (string)x.Tag == "Bullet") playerBulletManager(x);
                if (x is Rectangle && (string)x.Tag == "Enemy") enemyManager(x, player);
                if (x is Rectangle && (string)x.Tag == "EnemyBullet") enemyBulletManager(x, player);
            }

            foreach (Rectangle y in itemsToRemove)  mainCanvas.Children.Remove(y);
            
            if (totalEnemies < 1)
            {
                dispatcherTimer.Stop();
                MessageBox.Show("Victory");
            }
        }
    }
}