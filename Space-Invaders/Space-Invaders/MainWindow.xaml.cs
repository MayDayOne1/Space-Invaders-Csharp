using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Input;
using System.Windows.Media;
using System.Windows.Media.Imaging;
using System.Windows.Shapes;
using System.Windows.Threading;
using System.Media;

namespace Space_Invaders
{
    public partial class MainWindow : Window
    {
        private bool goLeft, goRight, gameOver = false;
        private List<Rectangle> itemsToRemove = new List<Rectangle>();
        private int bulletCooldown;
        private int bulletCooldownLimit = 90;
        private int totalEnemies;
        private int timeBetweenFrames = 20;
        private DispatcherTimer dispatcherTimer = new DispatcherTimer();
        private ImageBrush playerSkin = new ImageBrush();
        private float enemySpeed = 6f;
        private float enemySpeedChange = .8f;
        private SoundPlayer mainThemePlayer = new SoundPlayer(AppDomain.CurrentDomain.BaseDirectory + "/Audio/SpaceInvadersTheme.wav");


        public MainWindow()
        {
            InitializeComponent();
            gameOver = false;
            dispatcherTimer.Tick += GameManager;
            dispatcherTimer.Interval = TimeSpan.FromMilliseconds(timeBetweenFrames);
            dispatcherTimer.Start();
            playerSkin.ImageSource = new BitmapImage(new Uri("pack://application:,,,/Images/player.png"));
            playerRectangle.Fill = playerSkin;
            SpawnEnemies(30);
            mainThemePlayer.PlayLooping();
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

            if (e.Key == Key.Space || e.Key == Key.Enter)
            {
                if(gameOver)
                {
                    Application.Current.Shutdown();
                    System.Diagnostics.Process.Start(Application.ResourceAssembly.Location);
                }
                itemsToRemove.Clear();
                PlayerBulletSpawner();
            }

            if(e.Key == Key.Escape)
            {
                Application.Current.Shutdown();
            }
        }

        private void PlayerBulletSpawner()
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

        private void EnemyBulletSpawner(double x, double y)
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
            Canvas.SetLeft(enemyBullet, x);
            mainCanvas.Children.Add(enemyBullet);
        }

        private void SpawnEnemies(int limit)
        {
            int left = 0;
            Random rnd = new Random();
            totalEnemies = limit;
            for (int i = 0; i < limit; i++)
            {
                ImageBrush enemySkin = RandomEnemySkin(rnd);
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

        private ImageBrush RandomEnemySkin(Random rnd)
        {
            ImageBrush enemySkin = new ImageBrush();
            int enemyImages = rnd.Next(1, 8);
            string imageName = "pack://application:,,,/Images/Invader" + enemyImages + ".gif";
            enemySkin.ImageSource = new BitmapImage(new Uri(imageName));

            return enemySkin;
        }

        private Rectangle SpawnExplosion(double x, double y)
        {
            ImageBrush explosionSkin = new ImageBrush();
            string imageName = "pack://application:,,,/Images/explosion.png";
            explosionSkin.ImageSource = new BitmapImage(new Uri(imageName));

            Rectangle explosion = new Rectangle
            {
                Tag = "Explosion",
                Height = 45,
                Width = 45,
                Fill = explosionSkin
            };
            Canvas.SetTop(explosion, y);
            Canvas.SetLeft(explosion, x);
            mainCanvas.Children.Add(explosion);
            return explosion;
        }

        private async void ClearExplosion(object sender, EventArgs e, Rectangle ex)
        {
            await Task.Delay(100);
            mainCanvas.Children.Remove(ex);
        }

        private void MovePlayer()
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

        private void SpawnBullets()
        {
            bulletCooldown -= 3;
            if (bulletCooldown < 0)
            {
                EnemyBulletSpawner((Canvas.GetLeft(playerRectangle) + 20), 10);
                bulletCooldown = bulletCooldownLimit;
            }
        }

        private void PlayerBulletManager(Rectangle x)
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
                        if (enemySpeed <= 20f) enemySpeed += enemySpeedChange;
                    }
                }
            }
        }
        private void EnemyManager(Rectangle x, Rect player)
        {
            Canvas.SetLeft(x, Canvas.GetLeft(x) + enemySpeed);

            if (Canvas.GetLeft(x) > 720)
            {
                Canvas.SetLeft(x, -80);
                Canvas.SetTop(x, Canvas.GetTop(x) + (x.Height + 10));
            }

            Rect enemy = new Rect(Canvas.GetLeft(x), Canvas.GetTop(x), x.Width, x.Height);
            if (player.IntersectsWith(enemy))
            {
                dispatcherTimer.Stop();
                mainThemePlayer.Stop();
                gameOver = true;
                GameOver.Content = "Game Over";
                TryAgain.Content = "Press Enter to try again";
            }
        }

        private void EnemyBulletManager(Rectangle x, Rect player)
        {
            Canvas.SetTop(x, Canvas.GetTop(x) + 10);
            if (Canvas.GetTop(x) > 680)
            {
                itemsToRemove.Add(x);
            }

            Rect enemyBullet = new Rect(Canvas.GetLeft(x), Canvas.GetTop(x), x.Width, x.Height);
            if (enemyBullet.IntersectsWith(player))
            {
                dispatcherTimer.Stop();
                mainThemePlayer.Stop();
                gameOver = true;
                GameOver.Content = "Game Over";
                TryAgain.Content = "Press Enter to try again";
            }
        }

        private void GameManager(object sender, EventArgs e)
        {
            Rect player = new Rect(Canvas.GetLeft(playerRectangle),
                Canvas.GetTop(playerRectangle),
                playerRectangle.Width,
                playerRectangle.Height);

            MovePlayer();
            SpawnBullets();

            foreach (var x in mainCanvas.Children.OfType<Rectangle>())
            {
                if (x is Rectangle && (string)x.Tag == "Bullet") PlayerBulletManager(x);
                if (x is Rectangle && (string)x.Tag == "Enemy") EnemyManager(x, player);
                if (x is Rectangle && (string)x.Tag == "EnemyBullet") EnemyBulletManager(x, player);
            }

            foreach (Rectangle y in itemsToRemove)
            {
                if((string)y.Tag == "Enemy")
                {
                    Rectangle ex = SpawnExplosion(Canvas.GetLeft(y), Canvas.GetTop(y));
                    ClearExplosion(sender, e, ex);
                }
                mainCanvas.Children.Remove(y);
            }
            
            if (totalEnemies < 1)
            {
                dispatcherTimer.Stop();
                mainThemePlayer.Stop();
                gameOver = true;
                GameOver.Content = "You won!";
                TryAgain.Content = "Press Enter to try again";
            }
        }
    }
}