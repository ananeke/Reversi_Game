using System;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Input;
using System.Windows.Media;

namespace Reversi_Game
{
    /// <summary>
    /// Interaction logic for MainWindow.xaml
    /// </summary>
    public partial class MainWindow : Window
    {
        private ReversiEngine engine;

        private SolidColorBrush[] colors = { Brushes.Ivory, Brushes.Purple, Brushes.Orange };
        string[] namesPlayer = { "", "purple", "orange" };
        private Button[,] board;
        private bool boardOn
        {
            get
            {
                return board[engine.WidthBoard - 1, engine.HeightBoard - 1] != null;
            }
        }

        private void contentBoard()
        {
            if (!boardOn) return;

            for (int i = 0; i < engine.WidthBoard; i++)
                for(int j = 0; j < engine.HeightBoard; j++)
                {
                    board[i, j].Background = colors[engine.fieldState(i, j)];
                    board[i, j].Content = engine.fieldState(i, j).ToString();
                }
            ColorOfPlayer.Background = colors[engine.numberPlayerOfNextRound];
            numberFieldsPurple.Text = engine.numberFieldsOfPlayer1.ToString();
            numberFieldsOrange.Text = engine.numberFieldsOfPlayer2.ToString();
        }

        private struct coordinatesFields
        {
            public int horizontal, vertical;
        }

        private static string symbolField(int horizontal, int vertical)
        {
            if (horizontal > 25 || vertical > 8) return String.Format( "({0}, {1}" + horizontal.ToString() ,vertical.ToString());
            return "" + "ABCDEFGHIJKLMNPRSTUWXYZ"[horizontal] + "123456789"[vertical];
        }

        void clikFieldsBoard(object sender, RoutedEventArgs e)
        {
            Button clikedButton = sender as Button;
            coordinatesFields coordinates = (coordinatesFields)clikedButton.Tag;
            int clikedHorizontal = coordinates.horizontal;
            int clikedVertical = coordinates.vertical;

            int rememberedNumberPlayer = engine.numberPlayerOfNextRound;
            if (engine.putStone(clikedHorizontal, clikedVertical))
            {
                contentBoard();

                switch (rememberedNumberPlayer)
                {
                    case 1:
                        listOfMovesPurple.Items.Add(symbolField(clikedHorizontal, clikedVertical));
                        break;
                    case 2:
                        listOFMoveOrange.Items.Add(symbolField(clikedHorizontal, clikedVertical));
                        break;
                }
                listOfMovesPurple.SelectedIndex = listOfMovesPurple.Items.Count - 1;
                listOFMoveOrange.SelectedIndex = listOFMoveOrange.Items.Count - 1;

                ReversiEngine.situationOnBoard situationOnBoard = engine.checkSituationOnBoard();
                bool endGame = false;

                switch(situationOnBoard)
                {
                    case ReversiEngine.situationOnBoard.CurrentPlayerCanNotMove:
                        MessageBox.Show(string.Format("Gracz {0} zmuszony jest do oddania ruchu", namesPlayer[engine.numberPlayerOfNextRound]));
                        engine.Pas();
                        contentBoard();
                        break;
                    case ReversiEngine.situationOnBoard.BothPlayersCanNotMove:
                        MessageBox.Show("Obaj gracze nie mogą wykonać ruchu");
                        endGame = true;
                        break;
                    case ReversiEngine.situationOnBoard.AllFieldsAreBusy:
                        endGame = true;
                        break;
                }

                if (endGame)
                {
                    int numberOfWinner = engine.numberPlayerWithAdvantage;
                    if (numberOfWinner != 0) MessageBox.Show(string.Format("Wygrał gracz {0}", namesPlayer[numberOfWinner]), Title, MessageBoxButton.OK, MessageBoxImage.Information);
                    else MessageBox.Show("Remis", Title, MessageBoxButton.OK, MessageBoxImage.Information);
                    if(MessageBox.Show("Czy rozpocząć grę od nowa?", "Reversi", MessageBoxButton.YesNo, MessageBoxImage.Question, MessageBoxResult.Yes) == MessageBoxResult.Yes)
                    {
                        prepareBoardToNewGame(1, engine.WidthBoard, engine.HeightBoard);
                    }
                    else
                    {
                        Board.IsEnabled = false;
                        ColorOfPlayer.IsEnabled = false;
                    }
                }
            }              
        }

        private void prepareBoardToNewGame(int numberOfFirstPlayer, int widthBoard = 8, int heightBoard = 8)
        {
            engine = new ReversiEngine(numberOfFirstPlayer, widthBoard, heightBoard);
            listOfMovesPurple.Items.Clear();
            listOFMoveOrange.Items.Clear();
            contentBoard();
            Board.IsEnabled = true;
            ColorOfPlayer.IsEnabled = true;
        }

        private coordinatesFields? setBestMove()
        {
            if (!Board.IsEnabled) return null;

            if(engine.numberEmptyFields == 0)
            {
                MessageBox.Show("Nie ma juz wolnych pól na planszy", Title, MessageBoxButton.OK, MessageBoxImage.Warning);
                return null;
            }

            try
            {
                int horizontal, vertical;
                engine.suggestBestMove(out horizontal, out vertical);
                return new coordinatesFields() { horizontal = horizontal, vertical = vertical };
            }
            catch
            {
                MessageBox.Show("Bieżący gracz nie może wykonać ruchu", Title, MessageBoxButton.OK, MessageBoxImage.Warning);
                return null;
            }
        }

        private void markBestMove()
        {
            coordinatesFields? CoordinatesFields = setBestMove();
            if (CoordinatesFields.HasValue)
            {
                SolidColorBrush colorPrompt = colors[engine.numberPlayerOfNextRound].Lerp(colors[0], 0.5f);
                board[CoordinatesFields.Value.horizontal, CoordinatesFields.Value.vertical].Background = colorPrompt;
            }
        }

        private void makeBestMove()
        {
            coordinatesFields? CoordinatesFields = setBestMove();
            if (CoordinatesFields.HasValue)
            {
                Button button = board[CoordinatesFields.Value.horizontal, CoordinatesFields.Value.vertical];
                clikFieldsBoard(button, null);
            }
        }

        private void buttonColorPlayer_Click(object sender, RoutedEventArgs e)
        {
            if (Keyboard.Modifiers.HasFlag(ModifierKeys.Control)) makeBestMove();
            else markBestMove();
        }

        #region Metody od MenuItem
        private void MenuItem_GameForTwo(object sender, RoutedEventArgs e)
        {
            Title = "Reversi";
            prepareBoardToNewGame(1);
        }
        private void MenuItem_Close(object sender, RoutedEventArgs e)
        {
            Close();
        }

        private void MenuItem_PromptMove(object sender, RoutedEventArgs e)
        {
            markBestMove();
        }

        private void MenuItem_AboutGame(object sender, RoutedEventArgs e)
        {
            MessageBox.Show(Properties.Resources.String1);
        }


        #endregion

        public MainWindow()
        {
            InitializeComponent();

            engine = new ReversiEngine(1);

            for (int i = 0; i < engine.WidthBoard; i++)
                Board.ColumnDefinitions.Add(new ColumnDefinition());

            for(int j = 0; j < engine.HeightBoard; j++)
                Board.RowDefinitions.Add(new RowDefinition());

            board = new Button[engine.WidthBoard, engine.HeightBoard];
            for (int i = 0; i < engine.WidthBoard; i++)
                for (int j = 0; j < engine.HeightBoard; j++)
                {
                    Button button = new Button();
                    button.Margin = new Thickness(0);
                    Board.Children.Add(button);
                    Grid.SetColumn(button, i);
                    Grid.SetRow(button, j);
                    button.Tag = new coordinatesFields { horizontal = i, vertical = j };
                    button.Click += new RoutedEventHandler(clikFieldsBoard);
                    board[i,j] = button;
                }

            contentBoard();

        }
    }
}
