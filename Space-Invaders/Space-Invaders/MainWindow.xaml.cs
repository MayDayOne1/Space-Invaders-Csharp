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
        DispatcherTimer dispatcherTimer = new DispatcherTimer();
        ImageBrush playerSkin = new ImageBrush();
        int enemySpeed = 6;


        public MainWindow()
        {
            InitializeComponent();
        }

        private void Canvas_KeyIsDown(object sender, KeyEventArgs e)
        {

        }

        private void Canvas_KeyIsUp(object sender, KeyEventArgs e)
        {

        }

        private void enemyBulletSpawner(double x, double y)
        {

        }

        private void spawnEnemies(int limit)
        {

        }

        private void gameManager(object sender, EventArgs e)
        {

        }
    }
}
