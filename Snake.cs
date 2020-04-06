using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Input;
using System.Windows.Media;
using System.Windows.Shapes;
using System.Windows.Threading;

namespace SnakeWpf
{
    public partial class MainWindow : Window
    {
        private SolidColorBrush snakeBodyBrush = Brushes.Green;
        private SolidColorBrush snakeHeadBrush = Brushes.YellowGreen;
        private List<SnakePart> snakeParts = new List<SnakePart>();
        public enum SnakeDirection { Left, Right, Up, Down };
        private SnakeDirection snakeDirection = SnakeDirection.Right;
        private int snakeLength;
        const int SnakePartSize = 20;
        const int SnakeStartLength = 3;

        const int SnakeStartSpeed = 400;
        const int SnakeSpeedThreshold = 100;
        DispatcherTimer gameTickTimer = new DispatcherTimer();

        private Random rnd = new Random();
        private UIElement snakeFood = null;
        private SolidColorBrush foodBrush = Brushes.Aqua;
        private int currentScore = 0;

        ImagePicker ip = new ImagePicker();
        ImageBrush ib = new ImageBrush();

        private void DrawGameArea()
        {
            bool doneDrawingBackground = false;
            int nextX = 0, nextY = 0;
            int rowCounter = 0;
            bool nextIsOdd = false;

            while (doneDrawingBackground == false)
            {
                Rectangle rect = new Rectangle
                {
                    Width = SnakePartSize,
                    Height = SnakePartSize,
                };

                GameArea.Children.Add(rect);
                Canvas.SetTop(rect, nextY);
                Canvas.SetLeft(rect, nextX);

                nextIsOdd = !nextIsOdd;
                nextX += SnakePartSize;
                if (nextX >= GameArea.ActualWidth)
                {
                    nextX = 0;
                    nextY += SnakePartSize;
                    rowCounter++;
                    nextIsOdd = (rowCounter % 2 != 0);
                }
                if (nextY >= GameArea.ActualHeight) { doneDrawingBackground = true; }
            }
        }
        private void DrawSnakeBody()
        {
            foreach (SnakePart snakePart in snakeParts)
            {
                if (snakePart.UiElement == null)
                {
                    snakePart.UiElement = new Rectangle()
                    {
                        Width = SnakePartSize,
                        Height = SnakePartSize,
                        Fill = (snakePart.IsHead ? snakeHeadBrush : snakeBodyBrush)
                    };

                    GameArea.Children.Add(snakePart.UiElement);
                    Canvas.SetTop(snakePart.UiElement, snakePart.Position.Y);
                    Canvas.SetLeft(snakePart.UiElement, snakePart.Position.X);
                }
            }
        }
        private void MoveSnake()
        {
            // Clear game area  
            while (snakeParts.Count >= snakeLength)
            {
                GameArea.Children.Remove(snakeParts[0].UiElement);
                snakeParts.RemoveAt(0);
            }

            foreach (SnakePart snakePart in snakeParts)
            {
                (snakePart.UiElement as Rectangle).Fill = snakeBodyBrush;
                snakePart.IsHead = false;
            }

            SnakePart snakeHead = snakeParts[snakeParts.Count - 1];
            double nextX = snakeHead.Position.X;
            double nextY = snakeHead.Position.Y;
            switch (snakeDirection)
            {
                case SnakeDirection.Left:
                    nextX -= SnakePartSize;
                    break;
                case SnakeDirection.Right:
                    nextX += SnakePartSize;
                    break;
                case SnakeDirection.Up:
                    nextY -= SnakePartSize;
                    break;
                case SnakeDirection.Down:
                    nextY += SnakePartSize;
                    break;
            }

            snakeParts.Add(new SnakePart()
            {
                Position = new Point(nextX, nextY),
                IsHead = true
            });

            DrawSnakeBody();
            DoCollisionCheck();
        }
        private void GameTickTimer_Tick(object sender, EventArgs e)
        {
            MoveSnake();
        }
        private void StartNewGame()
        {
            // Clear Game OverContent label.
            textWrapGameOver.Text = "";
            // Remove potential dead snake parts and leftover food...
            foreach (SnakePart snakeBodyPart in snakeParts)
            {
                if (snakeBodyPart.UiElement != null)
                    GameArea.Children.Remove(snakeBodyPart.UiElement);
            }
            snakeParts.Clear();
            if (snakeFood != null) { GameArea.Children.Remove(snakeFood); }

            currentScore = 0;
            snakeLength = SnakeStartLength;
            snakeDirection = SnakeDirection.Right;
            snakeParts.Add(new SnakePart() { Position = new Point(SnakePartSize * 5, SnakePartSize * 5) });
            gameTickTimer.Interval = TimeSpan.FromMilliseconds(SnakeStartSpeed);

            DrawSnakeBody();
            DrawSnakeFood();

            // Start:     
            gameTickTimer.IsEnabled = true;
        }
        private Point GetNextFoodPosition()
        {
            int maxX = (int)(GameArea.ActualWidth / SnakePartSize);
            int maxY = (int)(GameArea.ActualHeight / SnakePartSize);
            int foodX = rnd.Next(0, maxX) * SnakePartSize;
            int foodY = rnd.Next(0, maxY) * SnakePartSize;

            foreach (SnakePart snakePart in snakeParts)
            {
                if ((snakePart.Position.X == foodX) && (snakePart.Position.Y == foodY))
                    return GetNextFoodPosition();
            }

            return new Point(foodX, foodY);
        }
        private void DrawSnakeFood()
        {
            Point foodPosition = GetNextFoodPosition();
            snakeFood = new Ellipse()
            {
                Width = SnakePartSize,
                Height = SnakePartSize,
                Fill = foodBrush
            };
            GameArea.Children.Add(snakeFood);
            Canvas.SetTop(snakeFood, foodPosition.Y);
            Canvas.SetLeft(snakeFood, foodPosition.X);
        }
        private void Window_KeyUp(object sender, KeyEventArgs e)
        {
            SnakeDirection originalSnakeDirection = snakeDirection;
            switch (e.Key)
            {
                case Key.Up:
                    if (snakeDirection != SnakeDirection.Down)
                        snakeDirection = SnakeDirection.Up;
                    break;
                case Key.Down:
                    if (snakeDirection != SnakeDirection.Up)
                        snakeDirection = SnakeDirection.Down;
                    break;
                case Key.Left:
                    if (snakeDirection != SnakeDirection.Right)
                        snakeDirection = SnakeDirection.Left;
                    break;
                case Key.Right:
                    if (snakeDirection != SnakeDirection.Left)
                        snakeDirection = SnakeDirection.Right;
                    break;
                case Key.Space:
                    StartNewGame();
                    break;
            }
            if (snakeDirection != originalSnakeDirection)
                MoveSnake();
        }
        private void DoCollisionCheck()
        {
            SnakePart snakeHead = snakeParts[snakeParts.Count - 1];

            if ((snakeHead.Position.X == Canvas.GetLeft(snakeFood)) && (snakeHead.Position.Y == Canvas.GetTop(snakeFood)))
            {
                EatSnakeFood();
                return;
            }

            if ((snakeHead.Position.Y < 0) || (snakeHead.Position.Y >= GameArea.ActualHeight) ||
            (snakeHead.Position.X < 0) || (snakeHead.Position.X >= GameArea.ActualWidth))
            {
                EndGame();
            }

            foreach (SnakePart snakeBodyPart in snakeParts.Take(snakeParts.Count - 1))
            {
                if ((snakeHead.Position.X == snakeBodyPart.Position.X) && (snakeHead.Position.Y == snakeBodyPart.Position.Y))
                    EndGame();
            }
        }
        private void EatSnakeFood()
        {
            snakeLength++;
            currentScore++;

            //Speed-Up:
            int timerInterval = Math.Max(SnakeSpeedThreshold, (int)gameTickTimer.Interval.TotalMilliseconds - (currentScore * 5));
            gameTickTimer.Interval = TimeSpan.FromMilliseconds(timerInterval);

            GameArea.Children.Remove(snakeFood);

            DrawSnakeFood();
            UpdateGameStatus();

            //Preparing collection of PixabayResponse - async
            //Saving random image (randomizing inside below method) - async
            //Saved picture setting as canvas background
            ChangeBackgroundAsync();
        }
        async Task ChangeBackgroundAsync()
        {
            await Task.Run(() => ip.ImageDataPickerMethod());
            GameArea.Background = ip.SaveImage();
        }
        private void EndGame()
        {
            gameTickTimer.IsEnabled = false;
            textWrapGameOver.Text = "Game over! Press Start button, try again.";
            //MessageBox.Show("Game over! Press Start button & try again.", "SnakeWPF");
        }
    }
}