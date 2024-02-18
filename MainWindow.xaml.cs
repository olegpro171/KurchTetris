using System;
using System.Runtime.CompilerServices;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Documents;
using System.Windows.Input;
using System.Windows.Media;
using System.Windows.Media.Imaging;


namespace Tetris
{
    public partial class MainWindow : Window
    {
        private readonly ImageSource[] tileImages = new ImageSource[]
        {
            new BitmapImage(new System.Uri("Assets/Board/BG_2.png", UriKind.Relative)),
            new BitmapImage(new System.Uri("Assets/Single Blocks/LightBlue.png", UriKind.Relative)),
            new BitmapImage(new System.Uri("Assets/Single Blocks/Blue.png", UriKind.Relative)),
            new BitmapImage(new System.Uri("Assets/Single Blocks/Orange.png", UriKind.Relative)),
            new BitmapImage(new System.Uri("Assets/Single Blocks/Yellow.png", UriKind.Relative)),
            new BitmapImage(new System.Uri("Assets/Single Blocks/Green.png", UriKind.Relative)),
            new BitmapImage(new System.Uri("Assets/Single Blocks/Purple.png", UriKind.Relative)),
            new BitmapImage(new System.Uri("Assets/Single Blocks/Red.png", UriKind.Relative)),
        };

        private readonly ImageSource[] blockImages = new ImageSource[]
        {
            new BitmapImage(new Uri("Assets/Shape Blocks/Blank.png", UriKind.Relative)),
            new BitmapImage(new Uri("Assets/Shape Blocks/I.png", UriKind.Relative)),
            new BitmapImage(new Uri("Assets/Shape Blocks/J.png", UriKind.Relative)),
            new BitmapImage(new Uri("Assets/Shape Blocks/L.png", UriKind.Relative)),
            new BitmapImage(new Uri("Assets/Shape Blocks/O.png", UriKind.Relative)),
            new BitmapImage(new Uri("Assets/Shape Blocks/S.png", UriKind.Relative)),
            new BitmapImage(new Uri("Assets/Shape Blocks/T.png", UriKind.Relative)),
            new BitmapImage(new Uri("Assets/Shape Blocks/Z.png", UriKind.Relative)),
        };

        private readonly Image[,] imageControls;

        private GameState gameState = new GameState();

        private readonly float maxDelay = 750.0F;
        private readonly float minDelay = 75.0F;
        private readonly float delayDecrease = 1F;

        public MainWindow()
        {
            InitializeComponent();
            imageControls = SetupGameCanvas(gameState.GameField);
        }

        private Image[,] SetupGameCanvas(GameField field)
        {
            Image[,] imageControls = new Image[field.Rows, field.Columns];
            int cellSize = 25;

            for (int row = 0; row < field.Rows; row++)
            {
                for (int col = 0; col < field.Columns; col++)
                {
                    Image imageControl = new Image
                    {
                        Width = cellSize,
                        Height = cellSize,
                    };

                    Canvas.SetTop(imageControl, (row - 2) * cellSize + 10);
                    Canvas.SetLeft(imageControl, col * cellSize);
                    GameCanvas.Children.Add(imageControl);
                    imageControls[row, col] = imageControl;
                }
            }
            return imageControls;
        }

        private void DrawField(GameField field)
        {
            for (int row = 0; row < field.Rows; row++)
            {
                for (int col = 0; col < field.Columns; col++)
                {
                    int id = field[row, col];
                    imageControls[row, col].Opacity = 1.0;
                    imageControls[row, col].Source = tileImages[id];
                }
            }
        }

        private void DrawBlock(Block block)
        {
            foreach (Position pos in block.GetTilesPositions())
            {
                imageControls[pos.Row, pos.Col].Opacity = 1.0;
                imageControls[pos.Row, pos.Col].Source = tileImages[block.Id];
            }
        }

        private void DrawNextBlock(BlockQueue blockQueue)
        {
            Block next = blockQueue.NextBlock;
            NextImage.Source = blockImages[next.Id];
        }

        private void DrawHeldBlock(Block HeldBlock)
        {
            if (HeldBlock == null)
            {
                HoldImage.Source = blockImages[0];
            }
            else
            {
                HoldImage.Source = blockImages[HeldBlock.Id];
                HoldStackPanel.Visibility = Visibility.Visible;
            }
        }

        private void DrawGhostBlock(Block block)
        {
            int distance = gameState.BlockDropDistance();

            foreach (Position pos in block.GetTilesPositions())
            {
                imageControls[pos.Row + distance, pos.Col].Opacity = 0.20;
                imageControls[pos.Row + distance, pos.Col].Source = tileImages[block.Id];
            }
        }

        private void Draw(GameState gameState)
        {
            DrawField(gameState.GameField);
            DrawGhostBlock(gameState.CurrentBlock);
            DrawBlock(gameState.CurrentBlock);
            DrawNextBlock(gameState.Queue);
            ScoreText.Text = $"Счет: {gameState.Score}";
            DrawHeldBlock(gameState.HeldBlock);
        }

        private async Task GameLoop()
        {
            Draw(gameState);

            while (true)
            {
                if (gameState.State != GameState.GameStates.Game)
                { break; }

                int delay = (int)(maxDelay - (gameState.Score * delayDecrease));

                delay = (int)Math.Max(delay, minDelay);
                await Task.Delay(delay);
                gameState.MoveBlockDown();
                Draw(gameState);
            }

            if (gameState.State == GameState.GameStates.GameOver)
            {
                GameOverMenu.Visibility = Visibility.Visible;
                FinalScoreText.Text = $"Счет: {gameState.Score}";
            }
            else if (gameState.State == GameState.GameStates.GamePaused)
            {

            }
            else if (gameState.State == GameState.GameStates.MainMenu)
            {
                GameOverMenu.Visibility = Visibility.Hidden;
                MainMenu.Visibility = Visibility.Visible;
            }
        }

        private void Window_KeyDown(object sender, KeyEventArgs e)
        {
            if (gameState.State != GameState.GameStates.Game)
            {
                return;
            }

            switch (e.Key)
            {
                case Key.A:
                case Key.Left: 
                    gameState.MoveBlockLeft(); 
                    break;

                case Key.D:
                case Key.Right: 
                    gameState.MoveBlockRight(); 
                    break;

                case Key.S:
                case Key.Down: 
                    gameState.MoveBlockDown();
                    break;
            
                case Key.W:
                case Key.Up:
                    gameState.Rotate();
                    break;

                case Key.LeftShift:
                    gameState.HoldBlock();
                    break;

                case Key.Space:
                    gameState.DropBlock();
                    break;

                case Key.Escape:
                    gameState.State = GameState.GameStates.GameOver;
                    break;

                default: return;
            }

            Draw(gameState);
        }

        private async void GameCanvas_Loaded(object sender, RoutedEventArgs e)
        {
            gameState = new GameState();
            gameState.State = GameState.GameStates.MainMenu;
            await GameLoop();
        }

        private async void PlayAgain_Click(object sender, RoutedEventArgs e)
        {

            gameState = new GameState();
            gameState.State = GameState.GameStates.Game;
            GameOverMenu.Visibility = Visibility.Hidden;
            MainMenu.Visibility = Visibility.Hidden;
            About.Visibility = Visibility.Hidden;
            Controls.Visibility = Visibility.Hidden;
            HoldStackPanel.Visibility = Visibility.Hidden;
            await GameLoop();
        }

        private void BackToMainMenu_Click(object sender, RoutedEventArgs e)
        {
            gameState.State = GameState.GameStates.MainMenu;
            GameOverMenu.Visibility = Visibility.Hidden;
            About.Visibility = Visibility.Hidden;
            MainMenu.Visibility = Visibility.Visible;
        }

        private void About_Click(object sender, RoutedEventArgs e)
        {
            MainMenu.Visibility = Visibility.Hidden;
            About.Visibility = Visibility.Visible;
        }

        private void Controls_Click(object sender, RoutedEventArgs e)
        {
            MainMenu.Visibility = Visibility.Hidden;
            Controls.Visibility = Visibility.Visible;
        }
    }
}
